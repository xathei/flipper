using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFACETools;
using Flipper.Classes;
using FlipperD;

namespace Flipper
{
    public class Ambuscade
    {
        private FFACE fface;
        public IJob job;

        private Thread chatThread;
        private Thread taskThread;

        private Monster RoETarget;
        private Monster AmbuscadeTarget;

        private string DifficultyMenu;

        private volatile bool _ambuscade;
        private volatile bool _leader;
        private string _target;
        private volatile bool _interruptRoute;
        public volatile int _townHomepoint = 41;
        public volatile int _targetId = 0;
        private volatile bool _hasKeyItem = false;
        private volatile bool _initialKeyItem = false;

        #region Bulk
        private string _route1 = "1-amb-homepoint-book.list";
        private string _route2 = "2-amb-exit-homepoint.list";
        private string _route3 = "3-amb-monstercircular.list";
        private string _zone = "Cape Teriggan #1.";
        #endregion

        public void Start(FFACE instance, Monster roeTarget, Monster ambuscadeTarget, string homePoint, bool keyitem, string difficulty = "Easy. (Level: 114)")
        {
            _initialKeyItem = keyitem;
            DifficultyMenu = difficulty;
            fface = instance;
            fface.Navigator.UseNewMovement = true;
            RoETarget = roeTarget;
            AmbuscadeTarget = ambuscadeTarget;
            _zone = homePoint;

            switch (fface.Player.MainJob)
            {
                case Job.THF:
                    job = new Thief(instance, Content.Ambuscade);
                    break;
                case Job.PLD:
                    job = new Paladin(instance, Content.Ambuscade);
                    break;
                case Job.BLU:
                    job = new BlueMage(instance, Content.Ambuscade);
                    break;
            }

            Combat.SetInstance = fface;
            Combat.SetJob = job;

            _ambuscade = true;
            _leader = true;
            _interruptRoute = false;

            taskThread = new Thread(DoTask);
            chatThread = new Thread(DoChat);

            taskThread.Start();
            chatThread.Start();
        }


        public void DoTask()
        {
            while (_ambuscade)
            {

                // #1 - FOLLOW ROUTE TO HOME POINT CRYSTAL
                DoRoute(_route2);


                if (!_initialKeyItem)
                {
                    // #2 - USE MENU TO TRAVEL TO APPROPRIATE ZONE
                    NavigateToZone(_zone, _townHomepoint);

                    // #3 - KILL MOSNTERS UNTIL KI IS OBTAINED
                    DoRoute(_route3, true);

                    Thread.Sleep(3000);

                    // #4 - RETURN HOME
                    ReturnHome();
                }
                else
                {
                    _initialKeyItem = false;
                    Thread.Sleep(1000);
                }

                // #4 - RUN UP TO AMBUSCADE BOOK
                DoRoute(_route1);

                DoEntry();

                while (fface.Player.Zone != Zone.Maquette_Abdhaljs_Legion)
                {
                    Thread.Sleep(100);
                }

                Thread.Sleep(10000);

                SpawnTrusts();

                List<TargetInfo> targs = Combat.FindTarget(AmbuscadeTarget.MonsterName);

                while (!targs.Any())
                    targs = Combat.FindTarget(AmbuscadeTarget.MonsterName);

                Combat.Fight(targs[0].Id, AmbuscadeTarget, Combat.Mode.None, 50);

                Thread.Sleep(15000);
                //wat
                Thread.Sleep(1);
            }
        }

        public void SpawnTrusts()
        {
            job.SpawnTrusts();
        }

        public void DoEntry()
        {

            int bookId = 146;

            while ((fface.Target.ID != bookId || string.IsNullOrEmpty(fface.Target.Name)) && _ambuscade)
            {
                fface.Target.SetNPCTarget(bookId);
                Thread.Sleep(100);
                fface.Windower.SendString("/target <t>");
                Thread.Sleep(500);
            }


            while (!fface.Menu.IsOpen)
            {
                fface.Windower.SendKeyPress(KeyCode.EnterKey);
                Thread.Sleep(1000);
            }

            while (!MenuSelectedText("Regular Ambuscade."))
            {
                fface.Windower.SendKeyPress(KeyCode.DownArrow);
                Thread.Sleep(400);
            }

            fface.Windower.SendKeyPress(KeyCode.EnterKey);
            Thread.Sleep(1000);

            while (!MenuSelectedText(DifficultyMenu))
            {
                fface.Windower.SendKeyPress(KeyCode.DownArrow);
                Thread.Sleep(400);
            }

            Thread.Sleep(1500);
            fface.Windower.SendKeyPress(KeyCode.EnterKey);
            Thread.Sleep(1500);

            fface.Windower.SendKeyPress(KeyCode.EnterKey);
        }

        private void ReturnHome()
        {
            while (fface.Player.Zone != Zone.Mhaura)
            {
                job.Warp();
                Thread.Sleep(1);
            }
            _hasKeyItem = false;
            Thread.Sleep(10000);
        }

        public bool MenuSelectedText(string text)
        {
            fface.Windower.SendString($"/echo {text} == {fface.Menu.DialogText.Options[fface.Menu.DialogOptionIndex]}");
            return (text == fface.Menu.DialogText.Options[fface.Menu.DialogOptionIndex]);
        }

        public bool NavigateToZone(string zone, int target)
        {
            while ((fface.Target.ID != target || string.IsNullOrEmpty(fface.Target.Name)) && _ambuscade)
            {
                fface.Target.SetNPCTarget(target);
                Thread.Sleep(100);
                fface.Windower.SendString("/target <t>");
                Thread.Sleep(500);
            }

            while (!fface.Menu.IsOpen)
            {
                fface.Windower.SendKeyPress(KeyCode.EnterKey);
                Thread.Sleep(1000);
            }

            if (MenuSelectedText("Travel to another home point."))
            {
                fface.Windower.SendKeyPress(KeyCode.EnterKey);
                Thread.Sleep(1000);
            }
            else
            {
                return false;
            }

            while (!MenuSelectedText("Select from favorites."))
            {
                fface.Windower.SendKeyPress(KeyCode.DownArrow);
                Thread.Sleep(400);
            }

            fface.Windower.SendKeyPress(KeyCode.EnterKey);
            Thread.Sleep(1000);

            while (!MenuSelectedText(zone))
            {
                fface.Windower.SendKeyPress(KeyCode.DownArrow);
                Thread.Sleep(400);
            }

            fface.Windower.SendKeyPress(KeyCode.EnterKey);
            Thread.Sleep(1000);

            while (!MenuSelectedText("Yes, please."))
            {
                fface.Windower.SendKeyPress(KeyCode.UpArrow);
                Thread.Sleep(400);
            }

            fface.Windower.SendKeyPress(KeyCode.EnterKey);
            Thread.Sleep(15000);
            return true;
        }

        public bool DoRoute(string route, bool targets = false)
        {
            fface.Windower.SendString("/echo running route: " + route);
            do
            {
                List<string> nodes = File.ReadAllLines($"assets\\paths\\{route}").ToList();
                List<Node> path = new List<Node>();

                foreach (string node in nodes)
                {
                    string[] token = node.Split(',');
                    var x = float.Parse((token[0]), CultureInfo.InvariantCulture);
                    var z = float.Parse((token[1]), CultureInfo.InvariantCulture);
                    path.Add(new Node() {X = x, Z = z});
                }

                float X = 0;
                float Y = 0;

                while (path.Any() && !_hasKeyItem && !_interruptRoute)
                {

                    if (targets)
                    {
                        _targetId = Combat.FindTarget(20, RoETarget.MonsterName);
                        WriteLog("Target found was: " + _targetId);
                    }

                    if (_targetId > 0)
                    {
                        Combat.Fight(_targetId, RoETarget, Combat.Mode.StrictPathing);
                    }

                    Node n = path[0];
                    path.RemoveAt(0);
                    fface.Navigator.DistanceTolerance = 0.2;
                    fface.Navigator.HeadingTolerance = 1;
                    fface.Navigator.Goto(() => n.X, () => n.Z, path.Any());
                    Thread.Sleep(1);
                }

                Thread.Sleep(1);
            } while (!_hasKeyItem && targets);

            fface.Navigator.Reset();

            return true;
        }

        public string lastLog = "";

        public void WriteLog(string log, bool verbose = false)
        {
            if (lastLog != log)
            {
                lastLog = log;
                Program.mainform.uxLog.Invoke((MethodInvoker)delegate
                {
                    Program.mainform.uxLog.Items.Add("[" + DateTime.Now.ToString("HH:mm:ss tt") + "]: " + log);
                    int visibleItems = Program.mainform.uxLog.ClientSize.Height / Program.mainform.uxLog.ItemHeight;
                    Program.mainform.uxLog.TopIndex = Math.Max(Program.mainform.uxLog.Items.Count - visibleItems + 1, 0);
                });
            }
        }

        public void DoChat()
        {
            string newChat = "";
            string oldChat = "";

            while (_ambuscade)
            {
                try
                {
                    newChat = fface.Chat.GetNextLine().Text;
                }
                catch
                {

                }
                if (!String.IsNullOrEmpty(newChat) && newChat != oldChat)
                {
                    oldChat = newChat;
                    string[] token = newChat.Split(' ');

                    // Watch for KeyItem obtainment

                    if (newChat.Contains("obtained an Ambuscade Primer Volume Two"))
                    {
                        _hasKeyItem = true;
                    }

                }
                Thread.Sleep(1);
            }
        }

    }

    public class Node
    {
        public float X;
        public float Z;
    }
}
