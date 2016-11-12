using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFACETools;
using Flipper.Classes;
using RedCorona.Net;
using System.Net.Sockets;
using FlipperD;

namespace Flipper
{
    public class Ambuscade
    {
        private FFACE fface;
        private IJob job;

        private Thread chatThread;
        private Thread taskThread;
        private Monster roeTargetMonster;
        private Monster ambuscadeTargetMonster;
        public volatile int _targetId = 0;

        private string _difficultyMenuString;
        private string _hpMenuItemString = "Cape Teriggan #1.";

        private volatile bool _ambuscade;
        private volatile bool _interruptRoute;

        private volatile bool _hasKeyItem = false;
        private volatile bool _initialKeyItem = false;

        #region Network and Static Variables
        private Thread _task;
        private volatile bool _inTask = false;
        private JobRole _role = JobRole.Damage;
        private string _route1 = "1-amb-homepoint-book.list";
        private string _route2 = "2-amb-exit-homepoint.list";
        private string _route3 = "3-amb-monstercircular.list";
        private volatile bool _Leader = false;
        private volatile int _HumanPlayers = 0;
        private volatile int _KeyItemCount = 0;
        private volatile ClientInfo client;
        public volatile bool _proceed;
        public volatile bool _KeyCapped = false;
        public int PartyID = 0;

        private List<string> events = new List<string>()
        {
            "readies Hypnic Lamp",
            "readies Bewitching",
            "readies Seismic",
            "readies Leeching",
            "readies Vile"
        };

        public IJob JobClass => job;
        #endregion

        public Ambuscade(FFACE instance)
        {
            // Assign fface instance.
            fface = instance;

            // Load Job Class to handle battle.
            LoadJobClass();

            job.GainEffect += Job_GainEffect;
            job.LoseEffect += Job_LoseEffect;
        }

        private void Job_LoseEffect(StatusEffect effect)
        {
            //client.Send($"BUFF_LOSE {(int)effect} {effect}");
        }

        private void Job_GainEffect(StatusEffect effect)
        {
            //client.Send($"BUFF_GAIN {(int)effect} {effect}");
        }

        private void LoadJobClass()
        {
            switch (fface.Player.MainJob)
            {
                case Job.THF:
                    job = new Thief(fface, Content.Ambuscade);
                    break;
                case Job.PLD:
                    job = new Paladin(fface, Content.Ambuscade);
                    break;
                case Job.BLU:
                    job = new BlueMage(fface, Content.Ambuscade);
                    break;
                case Job.WHM:
                    job = new WhiteMage(fface, Content.Ambuscade);
                    break;
                case Job.GEO:
                    job = new Geomancer(fface, Content.Ambuscade);
                    break;
                case Job.BRD:
                    job = new Bard(fface, Content.Ambuscade);
                    break;
                case Job.RNG:
                    job = new Ranger(fface, Content.Ambuscade);
                    break;
                case Job.RUN:
                    job = new RuneFencer(fface, Content.Ambuscade);
                    break;
                case Job.BLM:
                    job = new BlackMage(fface, Content.Ambuscade);
                    break;
            }
        }

        public void EndAmbuscade()
        {
            _ambuscade = false;
            client.Close();
            Combat.Interrupt();
            fface.Windower.SendKey(KeyCode.NP_Number4, false);
            fface.Windower.SendKey(KeyCode.NP_Number6, false);
            fface.Windower.SendKey(KeyCode.NP_Number2, false);
            return;
        }

        /// <summary>
        /// Starts the Ambuscade Process.
        /// </summary>
        /// <param name="instance">FFACE Instance.</param>
        /// <param name="roeTarget">RoE Target Monster</param>
        /// <param name="ambuscadeTarget">Ambuscade Target NM</param>
        /// <param name="homePoint">Home Point Menu Item to Click (i.e.: "Cape Teriggan #1.")</param>
        /// <param name="keyitem">Does the player already possess the KI upon starting?</param>
        /// <param name="settings">Settings object added as an afterthought to prevent more parameters being added.</param>
        /// <param name="difficulty">The difficulty string to select when entering ambuscade.</param>
        public void Start(FFACE instance, Monster roeTarget, Monster ambuscadeTarget, string homePoint, bool keyitem, AmbuscadeSettings settings, string difficulty = "Easy. (Level: 114)")
        {
            // Enables looping in the class.
            _ambuscade = true;

            // The HP Menu Item String to select when moving to the RoE Zone.
            _hpMenuItemString = homePoint;

            // The RoE And Ambuscade Target Monsters.
            roeTargetMonster = roeTarget;
            ambuscadeTargetMonster = ambuscadeTarget;

            // Use new FFACE Movement.
            fface.Navigator.UseNewMovement = true;
            fface.Navigator.SetViewMode(ViewMode.ThirdPerson);

            // Does the player have a key item already upon starting?
            _initialKeyItem = keyitem;
            if (_initialKeyItem) _hasKeyItem = true;

            // The Difficulty to select when entering ambuscade.
            _difficultyMenuString = difficulty;

            // Assign the job and fface instance to the combat static class.
            Combat.SetInstance = fface;
            Combat.SetJob = job;

            Chat.SetInstance(fface);
            Chat.SetMatches(events);
            Chat.FoundMatchHandler += Chat_FoundMatchHandler;
            Chat.Watch(true);

            Thread Status = new Thread(DoReportStunStatus);
            Status.Start();

            // Network and Other Settings:
            _Leader = settings.Leader;
            _HumanPlayers = settings.PartyCount;
            fface.Windower.SendString("//lua load enternity");
            Thread.Sleep(100);
            fface.Windower.SendString("//lua load knockblock");


            // Connect to Network
            Socket sock = Sockets.CreateTCPSocket(settings.Server, 6993);
            //Socket sock = Sockets.CreateTCPSocket("127.0.0.1", 6993);


            client = new ClientInfo(sock, false);
            client.Delimiter = "\r\n";
            client.OnRead += new ConnectionRead(ReadData);
            client.OnReady += Client_OnReady;
            client.BeginReceive();
            Thread.Sleep(100);
            if (_initialKeyItem && !_Leader)
            {
                client.Send("KEY");
                Thread.Sleep(1000);
            }
            if (_Leader)
            {
                client.Send($"PARTY {fface.Player.Name} {string.Join(",", GetPartyMembers())}");
                taskThread = new Thread(DoTask);
                taskThread.Start();
            }
            else
            {
                client.Send($"JOIN {fface.Player.Name}");
            }
            chatThread = new Thread(DoChat);
            chatThread.Start();

            Thread Safe = new Thread(SafePong);
            Safe.Start();

        }


        private void DoReportStunStatus()
        {
            while (_ambuscade)
            {
                //if (fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
                //{
                //    if (!_canStun && job.CanStun())
                //    {
                //        _canStun = true;
                //        WriteLog("[STAHP!] Server => STUN AVAILABLE");
                //        client.Send("STUN 1");
                //    }
                //    else if (_canStun && !job.CanStun())
                //    {
                //        _canStun = false;
                //        WriteLog("[STAHP!] Server => STUN DOWN!");
                //        client.Send("STUN 0");
                //    }
                //}
                Thread.Sleep(1);
            }
        }

        private bool _canStun = false;
        private bool _AwaitingStun = false;

        private void Chat_FoundMatchHandler(string match)
        {
            //// It's my turn to stun! I'm gonna stun.
            //if (_AwaitingStun)
            //{
            //    WriteLog("[STAHP] Matched (Stunning!...): " + match);
            //    job.DoStun();
            //}
            //else
            //{
            //    WriteLog("[STAHP] Matched (off doody): " + match);
            //}
        }

        public void SafePong()
        {
            while (true)
            {
                client.Send("PONG");
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// Event for the client connecting to the server.
        /// </summary>
        /// <param name="ci"></param>
        private void Client_OnReady(ClientInfo ci)
        {
            WriteLog("[SERVER] Connected!");
        }


        /// <summary>
        /// Get a string list of party members.
        /// </summary>
        /// <returns></returns>
        private List<string> GetPartyMembers()
        {
            List<string> membersList = new List<string>();

            for (int i = 0; i < 6; i++)
            {
                var member = fface.PartyMember[Convert.ToByte(i)];

                if (member.HPPCurrent > 0 && member.Active)
                {
                    membersList.Add(member.Name);
                }
            }

            return membersList;
        }

        /// <summary>
        /// Data read from the network socket.
        /// </summary>
        /// <param name="ci">The ClientInfo the read belongs to.</param>
        /// <param name="text">The string of data that was read, appended with \r\n.</param>
        private void ReadData(ClientInfo ci, string text)
        {

            text = text.Replace(Environment.NewLine, "|");
            string[] packets = text.Split('|');
            text = packets[0];

            string[] token = text.Split(' ');

            WriteLog($"[RAW]: {text}");

            //using (StreamWriter w = File.AppendText("log.txt"))
            //{
            //w.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss tt") + "]: " + text);
            //w.WriteLine("---------------------");

            //WriteLog("---------------------");
            //}

            //if (token[0] == "AWAIT_STUN")
            //{
            //    WriteLog("[STAHP] =========== I AM UP FOR STUN =======");
            //    _AwaitingStun = true;
            //    job.SetHaltActions(true);
            //}

            //if (token[0] == "STOP_AWAIT_STUN")
            //{
            //    WriteLog("[STAHP] =========== I /NOT/ UP FOR STUN =======");
            //    _AwaitingStun = false;
            //    job.SetHaltActions(false);
            //}

            if (token[0] == "GAIN_BUFF")
            {
                job.MemberGainEffect(token[1], JobRole.Damage, (StatusEffect)Convert.ToInt32(token[2]));
            }

            if (token[0] == "LOSE_BUFF")
            {
                job.MemberLoseEffect(token[1], JobRole.Damage, (StatusEffect)Convert.ToInt32(token[2]));
            }

            if (text != "PING!")
            {
                //WriteLog("[DEBUG_S>>] " + text);
            }

            // Always reply to ping.
            if (token[0] == "PING!" || (packets.Length >= 2 && packets[1] == "PING!"))
            {
                //WriteLog("[REPLY] >> PONG");
                client.Send("PONG");
            }

            if (token[0] == "PROCEED" && _Leader)
            {
                _proceed = true;
            }

            if (token[0] == "KEY_CAP" && _Leader)
            {
                _KeyCapped = true;
            }

            if (token[0] == "LIST")
            {
                string[] members = token[2].Split(',');
                foreach (string member in members)
                {
                    
                }
            }

            if (token[0] == "MEMBER")
            {
                if (token[1] == "0")
                    WriteLog("[PARTY JOIN]: " + token[2]);
                if (token[1] == "1")
                    WriteLog("[PARTY PART]: " + token[2]);
                if (token[1] == "2")
                {
                    WriteLog("[PARTY STOP]: " + token[2]);
                    EndAmbuscade();
                }
                if (token[1] == "3")
                    WriteLog("[PARTY WAIT]: Waiting for the party leader to Connect...");
            }

            if (token[0] == "TASK" && !_Leader)
            {
                // When the server requests a new task, let's run it on another thread.
                if (!_inTask)
                {
                    // task is always an int.
                    AmbuscadeTaskType task = (AmbuscadeTaskType)Convert.ToInt32(token[1]);
                    // parameter is usually a monster ID.
                    int parameter = token.Count() == 3 ? Convert.ToInt32(token[2]) : 0;

                    _task = new Thread(() => NetworkTask(task, parameter));

                    _task.Start();
                }
            }
        }

        public enum AmbuscadeTaskType
        {
            GotoMonsterZone = 1,
            ReturnHome = 2,
            Fight = 3,
            CancelEngage = 4,
            ObtainKI = 5,
            CappedKI = 6
        }

        public void NetworkTask(AmbuscadeTaskType task, int parameter = 0)
        {
            if (!_ambuscade)
                return;

            if (!job.Tracking())
            {
                job.TrackBuffs(true);
            }

            switch (task)
            {
                case AmbuscadeTaskType.GotoMonsterZone:
                    {
                        WriteLog($"[TASK] >> PERFORMING ({task}) ...");
                        DoRoute(_route2);
                        NavigateToZone(_hpMenuItemString, 41);
                        client.Send("DOCK_ROE");
                        WriteLog($"[REPLY] >> TASK COMPLETED ({task})");
                        break;
                    }
                case AmbuscadeTaskType.ReturnHome:
                    {
                        WriteLog($"[TASK] >> PERFORMING ({task}) ...");
                        ReturnHome();
                        client.Send("DOCK_MHAURA");
                        WriteLog($"[REPLY] >> TASK COMPLETED ({task})");
                        break;

                    }
                case AmbuscadeTaskType.Fight:
                    {
                        WriteLog($"[TASK] >> PERFORMING ({task}) ...");
                        if (fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
                        {

                            _hasKeyItem = false;
                            _initialKeyItem = false;
                            _KeyCapped = false;
                        }
                        Fight(parameter, Combat.Mode.None);
                        WriteLog($"[REPLY] >> TASK COMPLETED ({task})");
                        break;
                    }
                case AmbuscadeTaskType.CancelEngage:
                    {
                        WriteLog($"[TASK] >> PERFORMING ({task}) ...");
                        Combat.Interrupt();
                        WriteLog($"[REPLY] >> TASK COMPLETED ({task})");
                        break;
                    }
                case AmbuscadeTaskType.ObtainKI:
                    {
                        WriteLog($"[TASK] >> PERFORMING ({task}) ...");
                        ObtainKI();
                        client.Send("DOCK_KI");
                        WriteLog($"[REPLY] >> TASK COMPLETED ({task}) (Everyone has a KI!)");

                        break;
                    }
                case AmbuscadeTaskType.CappedKI:
                    {
                        WriteLog($"[TASK] >> PERFORMING ({task}) ...");
                        _KeyCapped = true;
                        if (Combat.InCombat)
                        {
                            WriteLog("[TASK INFO] I'm in Combat, and attempting to stop Combat!");
                        }
                        else
                        {
                            WriteLog("[TASK INFO] I'm not currently in combat! .. Waiting for CappedKI to Complete");
                        }
                        if (!job.Engages() && Combat.InCombat)
                        {
                            WriteLog("[TASK INFO INNER] Attempting to Interrupt Combat...");
                            Combat.Interrupt();
                        }
                        WriteLog($"[REPLY] >> TASK COMPLETED ({task})");
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(task), task, null);
            }
            WriteLog("[TASK] >> IDLE. Waiting for a new task...");
            _inTask = false;
        }

        public bool HasKeyItem()
        {
            if (_hasKeyItem && _KeyCapped)
                return true;

            return false;
        }

        public bool ObtainKI()
        {
            Combat.LoadBlacklist(fface.Player.Zone);
            Combat.LoadCoords(fface.Player.Zone);
            Combat.LoadHotspots(fface.Player.Zone);
            List<Hotspot> hotspots = Combat.GetHotspots();
            List<Node> path = new List<Node>();
            int hotspotIndex = 0;

            List<Monster> targets = new List<Monster>();

            targets.Add(roeTargetMonster);
            targets.Add(new Monster()
            {
                HitBox = 4.5,
                MonsterName = "Korrigan",
                TimeSpecific = false
            });


            while (!HasKeyItem() && _ambuscade)
            {
                NoPath:
                if (HasKeyItem())
                {
                    goto done;
                }

                hotspotIndex = (hotspotIndex + 1) % hotspots.Count;
                float destx = hotspots[hotspotIndex].Waypoint.X;
                float destz = hotspots[hotspotIndex].Waypoint.Z;


                NewPath:
                path = Combat.GetPath(destx, destz);

                if (path == null)
                    goto NoPath;

                float targetx;
                float targetz;

                while (path.Any() && !HasKeyItem() && _ambuscade)
                {
                    foreach (Monster m in targets)
                    {
                        _targetId = Combat.FindTarget(50, m.MonsterName);

                        if (_targetId > 0 && !HasKeyItem())
                        {
                            fface.Navigator.Reset();
                            Fight(_targetId, Combat.Mode.Meshing);
                            _targetId = 0;
                            goto NewPath;
                        }
                    }

                    fface.Navigator.HeadingTolerance = 1;
                    fface.Navigator.DistanceTolerance = 0.7;
                    targetx = path[0].X;
                    targetz = path[0].Z;
                    path.RemoveAt(0);
                    fface.Navigator.Goto(() => targetx, () => targetz, path.Any());
                }
                fface.Navigator.Reset();
                Thread.Sleep(1);
            }
            done:
            fface.Navigator.Reset();
            return true;
        }


        public bool Fight(int id, Combat.Mode mode)
        {
            bool result = false;
            // If we're in Ambuscade, let people know it's time to fight.
            if (_Leader && fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
            {
                client.Send("RELAY TASK 3 " + id);
            }

            Combat.FailType fail = Combat.FailType.NoFail;
            result = Combat.Fight(id, ambuscadeTargetMonster, mode, out fail);

            if (result == false)
            {
                Combat.AddBlacklist(id);
            }
            return result;
        }

        public void DoTask()
        {
            while (_ambuscade)
            {

                // Wait for all palyers to start.
                while (!_proceed && _ambuscade)
                    Thread.Sleep(100);
                _proceed = false;
                Thread.Sleep(1000);

                if (!job.Tracking())
                {
                    job.TrackBuffs(true);
                }

                if (_KeyCapped && _initialKeyItem)
                {
                    _proceed = true;
                    goto SkipRoEFarming;
                }

                // Travel to Home Point then RoE Zone.
                client.Send("RELAY TASK 1 0");
                DoRoute(_route2);
                NavigateToZone(_hpMenuItemString, 41);


                // Wait for all players to arrive at the RoE Zone.
                while (!_proceed && _ambuscade)
                    Thread.Sleep(100);
                _proceed = false;

                // Farm Key Item, then ReturnHome()
                //DoRoute(_route3, true);
                client.Send("RELAY TASK 5 0"); // -- obtain KI
                ObtainKI();
                fface.Navigator.Reset();
                Thread.Sleep(4000);
                client.Send("RELAY TASK 6 0"); // -- tell players to stop farming KI


                while (!_proceed && _ambuscade)
                    Thread.Sleep(100);
                _proceed = false;

                Thread.Sleep(3500); // allow time to disengage

                // Tell clients to return home!
                client.Send("RELAY TASK 2 0");
                ReturnHome();

                // Wait for everyone to return home.
                while (!_proceed && _ambuscade)
                    Thread.Sleep(100);

                SkipRoEFarming:

                // Run to the book, and enter legion.
                DoRoute(_route1);
                DoEntry();

                // Wait until I'm in legion to proceed.
                while (fface.Player.Zone != Zone.Maquette_Abdhaljs_Legion && _ambuscade)
                {
                    Thread.Sleep(100);
                }

                // Once in Legion, wait for everything to load.
                Thread.Sleep(15000);

                // Add trusts to the party.
                if (fface.Party.Party0Count < 6 && _ambuscade)
                    SummonTrusts();

                // Look for a target
                List<TargetInfo> targs = Combat.FindTarget(ambuscadeTargetMonster.MonsterName);
                while (!targs.Any() && _ambuscade)
                {
                    targs = Combat.FindTarget(ambuscadeTargetMonster.MonsterName);
                    Thread.Sleep(1);
                }

                if (targs.Any() && _ambuscade)
                    // Fight le target.
                    Fight(targs[0].Id, Combat.Mode.None);

                _hasKeyItem = false;
                _KeyCapped = false;
                _initialKeyItem = false;

                while (fface.Player.Zone != Zone.Mhaura && _ambuscade)
                {
                    Thread.Sleep(100);
                }

                // After fighting the target, loop back to the start.
                Thread.Sleep(15000);
            }
        }

        // calculates and summons appropriate trusts.
        public void SummonTrusts()
        {
            if (!_ambuscade)
                return;

            job.SpawnTrusts();
        }

        public void DoEntry()
        {
            if (!_ambuscade)
                return;

            Thread.Sleep(3000);
            int bookId = 146;

            MenuClosed:
            fface.Windower.SendKeyPress(KeyCode.EscapeKey);
            Thread.Sleep(100);
            fface.Windower.SendKeyPress(KeyCode.EscapeKey);

            while ((fface.Target.ID != bookId || fface.Target.Name != "Ambuscade Tome") && _ambuscade)
            {

                fface.Target.SetNPCTarget(bookId);
                Thread.Sleep(1000);
                fface.Target.SetNPCTarget(bookId);
                fface.Windower.SendString("/target <t>");
                Thread.Sleep(100);
            }

            Thread.Sleep(1000);

            while (!fface.Menu.IsOpen)
            {
                fface.Windower.SendKeyPress(KeyCode.EnterKey);
                Thread.Sleep(3000);
            }

            while (!MenuSelectedText("Regular Ambuscade."))
            {
                if (!fface.Menu.IsOpen) goto MenuClosed;
                fface.Windower.SendKeyPress(KeyCode.DownArrow);
                Thread.Sleep(400);
            }

            fface.Windower.SendKeyPress(KeyCode.EnterKey);
            Thread.Sleep(1000);

            while (!MenuSelectedText(_difficultyMenuString))
            {
                if (!fface.Menu.IsOpen) goto MenuClosed;
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
            if (!_ambuscade)
                return;

            do
            {
                job.Warp();
                Thread.Sleep(10);

            } while (fface.Player.Zone != Zone.Mhaura);

            Thread.Sleep(15000);
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
            if (!_ambuscade)
                return false;

            Random r = new Random();
            Thread.Sleep(3000);


            MenuClosed:
            fface.Windower.SendKeyPress(KeyCode.EscapeKey);
            Thread.Sleep(100);
            fface.Windower.SendKeyPress(KeyCode.EscapeKey);

            while ((fface.Target.ID != target || fface.Target.Name != "Home Point #1") && _ambuscade)
            {

                fface.Target.SetNPCTarget(target);
                Thread.Sleep(500);
                fface.Target.SetNPCTarget(target);
                fface.Windower.SendString("/target <t>");
                Thread.Sleep(100);
                fface.Windower.SendString("/lockon");
                Thread.Sleep(5000);
            }


            while (!fface.Menu.IsOpen)
            {
                fface.Windower.SendKeyPress(KeyCode.EnterKey);
                Thread.Sleep(4000);
            }

            if (MenuSelectedText("Travel to another home point."))
            {
                if (!fface.Menu.IsOpen) goto MenuClosed;
                fface.Windower.SendKeyPress(KeyCode.EnterKey);
                Thread.Sleep(1000);
            }
            else
            {
                goto MenuClosed;
            }

            while (!MenuSelectedText("Select from favorites."))
            {
                if (!fface.Menu.IsOpen) goto MenuClosed;
                fface.Windower.SendKeyPress(KeyCode.DownArrow);
                Thread.Sleep(400);
            }


            fface.Windower.SendKeyPress(KeyCode.EnterKey);
            Thread.Sleep(1000);

            while (!MenuSelectedText(zone))
            {
                if (!fface.Menu.IsOpen) goto MenuClosed;
                fface.Windower.SendKeyPress(KeyCode.DownArrow);
                Thread.Sleep(400);
            }



            fface.Windower.SendKeyPress(KeyCode.EnterKey);
            Thread.Sleep(1000);

            while (!MenuSelectedText("Yes, please."))
            {
                if (!fface.Menu.IsOpen) goto MenuClosed;
                fface.Windower.SendKeyPress(KeyCode.UpArrow);
                Thread.Sleep(400);
            }



            fface.Windower.SendKeyPress(KeyCode.EnterKey);
            while (fface.Player.Zone == Zone.Mhaura || fface.Player.GetLoginStatus == LoginStatus.Loading)
                Thread.Sleep(1000);

            Thread.Sleep(13000);
            return true;
        }

        public bool DoRoute(string route)
        {
            if (!_ambuscade)
                return false;

            fface.Windower.SendString("/echo running route: " + route);
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

            while (path.Any())
            {
                Node n = path[0];
                path.RemoveAt(0);
                fface.Navigator.DistanceTolerance = 0.2;
                fface.Navigator.HeadingTolerance = 1;
                fface.Navigator.Goto(() => n.X, () => n.Z, path.Any());
                Thread.Sleep(1);
            }

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
                if (!string.IsNullOrEmpty(newChat) && newChat != oldChat)
                {
                    oldChat = newChat;
                    string[] token = newChat.Split(' ');

                    // Watch for KeyItem obtainment

                    if (newChat.Contains("obtained an Ambuscade Primer Volume Two"))
                    {
                        if (!_Leader)
                        {
                            //WriteLog("[REPLY] >> REPORTING KEY ITEM");
                            client.Send("KEY");
                        }
                        _hasKeyItem = true;
                    }

                }
                Thread.Sleep(1);
            }
        }

        private bool TokenMatch(string data, string match)
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

    }

    public class Node
    {
        public float X;
        public float Y;
        public float Z;
    }

    public enum JobRole
    {
        Tank,
        Healer,
        Damage,
        Support
    }
    public class AmbuscadeSettings
    {
        public bool Network;
        public bool Leader;
        public bool FillTrusts;
        public JobRole Role;
        public int PartyCount;
        public string Server;
    }
}
