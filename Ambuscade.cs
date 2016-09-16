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
using RedCorona.Net;
using System.Net.Sockets;

namespace Flipper
{
    public enum JobRole
    {
        Tank,
        Healer,
        Damage
    }
    public class AmbuscadeSettings
    {
        public bool Network;
        public bool Leader;
        public bool FillTrusts;
        public JobRole Role;
        public int PartyCount;
    }
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
        private string _target;
        private volatile bool _interruptRoute;
        public volatile int _targetId = 0;
        public volatile int _partyCount = 3;

        private volatile bool _hasKeyItem = false;
        private volatile bool _initialKeyItem = false;


        private volatile ClientInfo client;
        private volatile bool _netMode = false;
        private volatile bool _leader;
        private JobRole _role = JobRole.Damage;

        #region Bulk
        private string _route1 = "1-amb-homepoint-book.list";
        private string _route2 = "2-amb-exit-homepoint.list";
        private string _route3 = "3-amb-monstercircular.list";
        private string _zone = "Cape Teriggan #1.";
        #endregion




        public void Start(FFACE instance, Monster roeTarget, Monster ambuscadeTarget, string homePoint, bool keyitem, AmbuscadeSettings settings, string difficulty = "Easy. (Level: 114)")
        {
            fface = instance;

            // Load JobClass.
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
                case Job.WHM:
                    job = new WhiteMage(instance, Content.Ambuscade);
                    break; 
            }

            _ambuscade = true;
            _interruptRoute = false;
            _zone = homePoint;
            RoETarget = roeTarget;
            AmbuscadeTarget = ambuscadeTarget;
            fface.Navigator.UseNewMovement = true;
            _initialKeyItem = keyitem;
            DifficultyMenu = difficulty;
            Combat.SetInstance = fface;
            Combat.SetJob = job;

            _netMode = settings.Network;

            if (settings.Network)
            {
                // Connect to Network
                Socket sock = Sockets.CreateTCPSocket("ambuscade.dazusu.com", 6993);
                client = new ClientInfo(sock, false);
                client.Delimiter = "\r\n";
                client.OnRead += new ConnectionRead(ReadData);
                client.BeginReceive();
            }

            if (settings.Leader && settings.Network || !settings.Network)
            {
                taskThread = new Thread(DoTask);
                chatThread = new Thread(DoChat);
                taskThread.Start();
                chatThread.Start();
                _leader = true;
                _partyCount = settings.PartyCount;
                client.Send("RESET");
            }

            if (settings.Network && !settings.Leader)
                client.Send("LANDED_MHAURA");
        }


        public bool TokenMatch(string data, string match)
        {
            int count = match.Split(' ').Count();

            string[] dataTokens = data.Split(' ');
            string[] matchTokens = match.Split(' ');

            for (int i = 0; i < count; i++)
            {
                if (dataTokens[i] != matchTokens[i])
                    return false;
            }
            return true;

        }

        private void ReadData(ClientInfo ci, string text)
        {
            text = text.Replace("\r\n", "");

            string[] token = text.Split(' ');

            WriteLog("[NET]: " + text);

            if (token[0] == "PING!")
            {
                WriteLog("PONG...");
                client.Send("PONG...");
            }

            if (_leader)
            {
                if (TokenMatch(text, "ROE_ZONE_COUNT"))
                {
                    int count = Convert.ToInt32(token[1]);
                    if (count == _partyCount)
                    {
                        _awaitingRoEZoneInCount = false;
                    }
                }
                if (TokenMatch(text, "MHAURA_COUNT"))
                {
                    int count = Convert.ToInt32(token[1]);
                    if (count == _partyCount)
                    {
                        _awaitingMhauraZoneInCount = false;
                    }
                }
                else if (TokenMatch(text, "LEGION_COUNT"))
                {
                    int count = Convert.ToInt32(token[1]);
                    if (count == _partyCount)
                    {
                        _awaitingLegionZoneInCount = false;
                    }
                }
            }
            else
            {
                if (TokenMatch(text, "TO_MONSTER_ZONE"))
                {
                    DoRoute(_route2);
                    NavigateToZone(_zone, 41);
                }
                else if (TokenMatch(text, "RETURN_HOME"))
                {
                    ReturnHome();
                }
                else if (TokenMatch(text, "ENGAGE_AMBUSCADE"))
                {
                    int target = Convert.ToInt32(token[1]);
                    Combat.Fight(target, AmbuscadeTarget, Combat.Mode.None, 50);
                }
                else if (token[0] == "ENGAGE_ROE")
                {
                    int target = Convert.ToInt32(token[1]);
                    Combat.Fight(target, RoETarget, Combat.Mode.StrictPathing);
                }
            }


        }

        private volatile bool _awaitingMhauraZoneInCount = true;
        private volatile bool _awaitingRoEZoneInCount = true;
        private volatile bool _awaitingLegionZoneInCount = true;

        public void DoTask()
        {
            while (_ambuscade)
            {

                // WAIT FOR PLAYERS TO GET TO MHAURA.
                while (_netMode && _awaitingMhauraZoneInCount)
                {
                    Thread.Sleep(500); 
                }
                _awaitingMhauraZoneInCount = true;

                if (_netMode)
                    client.Send("TO_MONSTER_ZONE");

                // Goto home point crystal.
                DoRoute(_route2);
                // Use home point to get to RoE Zone
                NavigateToZone(_zone, 41);


                // WAIT FOR PLAYERS TO GET TO ROE ZONE.
                while (_netMode && _awaitingRoEZoneInCount)
                {
                    Thread.Sleep(500); 
                }
                _awaitingRoEZoneInCount = true;


                // Go kill monsters.
                DoRoute(_route3, true);
                // Allow time for player to disengage.
                Thread.Sleep(300);
                // Go back to Mhaura.

                if (_netMode)
                    client.Send("RETURN_HOME");

                ReturnHome();


                // WAIT FOR PLAYERS TO GET TO MHAURA.
                while (_netMode && _awaitingMhauraZoneInCount)
                {
                    Thread.Sleep(500);
                }

                Thread.Sleep(1000);
                // Run to ambuscade book.
                DoRoute(_route1);
                // Process entry.
                DoEntry();

                while (fface.Player.Zone != Zone.Maquette_Abdhaljs_Legion)
                {
                    Thread.Sleep(100);
                }

                // WAIT FOR PLAYERS TO ZONE INTO LEGION
                if (_netMode && _awaitingLegionZoneInCount)
                {
                    Thread.Sleep(8000);
                    fface.Windower.SendString("/p We'll get going in a second...");
                    Thread.Sleep(4000);
                }
                else
                {
                    Thread.Sleep(10000);
                }
                _awaitingLegionZoneInCount = true;

                SpawnTrusts();

                List<TargetInfo> targs = Combat.FindTarget(AmbuscadeTarget.MonsterName);
                while (!targs.Any())
                    targs = Combat.FindTarget(AmbuscadeTarget.MonsterName);

                if (_netMode)
                    client.Send("ENGAGE_AMBUSCADE " + targs[0].Id);

                
                Combat.Fight(targs[0].Id, AmbuscadeTarget, Combat.Mode.None, 50);

                Thread.Sleep(10000);
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
            Thread.Sleep(7000);
            if (_netMode)
            {
                client.Send("LANDED_MHAURA");
            }
        }

        public bool MenuSelectedText(string text)
        {
            try
            {
                fface.Windower.SendString(
                    $"/echo {text} == {fface.Menu.DialogText.Options[fface.Menu.DialogOptionIndex]}");
            }
            catch (Exception e)
            {
                return false;
            }

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

            if (_netMode)
            {
                Thread.Sleep(2000);
                fface.Windower.SendString("/target Dazusu");
                Thread.Sleep(500);
                fface.Windower.SendString("/lockon");
                Thread.Sleep(500);
                fface.Windower.SendKey(KeyCode.NP_Number2, true);
                Thread.Sleep(500);
                fface.Windower.SendKey(KeyCode.NP_Number2, false);
                Thread.Sleep(500);
                fface.Windower.SendString("/follow Dazusu");
                client.Send("LANDED_ROE_ZONE");
            }
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
                    path.Add(new Node() { X = x, Z = z });
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
