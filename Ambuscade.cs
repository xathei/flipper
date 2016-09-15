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

        private volatile bool _ambuscade;
        private volatile bool _leader;
        private string _target;
        private volatile bool _interruptRoute;
        public volatile int _townHomepoint = 41;
        public volatile int _targetId = 0;

        #region Bulk
        private string _route1 = "1-amb-homepoint-book.list";
        private string _route2 = "2-amb-exit-homepoint.list";
        private string _route3 = "3-amb-monstercircular.list";
        private string _zone = "Cape Teriggan #1.";
        #endregion

        public void Start(FFACE instance)
        {
            fface = instance;
            fface.Navigator.UseNewMovement = true;


            switch (fface.Player.MainJob)
            {
                case Job.THF:
                    job = new Thief(instance, Content.Ambuscade);
                    break;
                case Job.PLD:
                    job = new Paladin(instance, Content.Ambuscade);
                    break;
            }

            Combat.SetInstance = fface;
            Combat.SetJob = job;

            _ambuscade = true;
            _leader = true;
            _interruptRoute = false;
            _target = "Greater Manticore";

            taskThread = new Thread(DoTask);
            chatThread = new Thread(DoChat);

            taskThread.Start();
            chatThread.Start();
        }


        public void DoTask()
        {
            while (_ambuscade)
            {
                // #1 - FETCH AMBUSCADE KI

                //if (!fface.Player.HasKeyitem(KeyItem.Ambuscade_Primter_Volume_Two))
                //{
                    // #1 - FOLLOW ROUTE TO HOME POINT CRYSTAL
                    DoRoute(_route2);
                    NavigateToZone(_zone, _townHomepoint);
                    DoRoute(_route3, true);

                    _ambuscade = false;
                //}

                //wat
                Thread.Sleep(1);
            }
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
            List<string> nodes = File.ReadAllLines($"assets\\paths\\{route}").ToList();
            List<Node> path = new List<Node>();
            foreach (string node in nodes)
            {
                string[] token = node.Split(',');
                var x = float.Parse((token[0]), CultureInfo.InvariantCulture);
                var z = float.Parse((token[1]), CultureInfo.InvariantCulture);
                path.Add(new Node() { X = x, Z = z });
            }

            float X = 0;
            float Y = 0;

            while (path.Any() && !_interruptRoute)
            {

                if (targets)
                {
                    _targetId = Combat.FindTarget("Greater Manticore");
                    WriteLog("Target found was: " + _targetId);
                }

                if (_targetId > 0)
                {
                    Combat.Fight(_targetId, new Monster(), Combat.Mode.StrictPathing);
                }
                
                Node n = path[0];
                path.RemoveAt(0);
                fface.Navigator.DistanceTolerance = 0.2;
                fface.Navigator.HeadingTolerance = 1;
                fface.Navigator.Goto(() => n.X, () => n.Z, path.Any());
                Thread.Sleep(1);
            }

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
            while (_ambuscade)
            {
                

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
