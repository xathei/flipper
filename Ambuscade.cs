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
                    _targetId = FindTarget();
                    WriteLog("Target found was: " + _targetId);
                }

                if (_targetId > 0)
                {
                    fface.Navigator.Reset();
                    Thread.Sleep(500);
                    Fight(_targetId);
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

        public bool TargetAlive(int targetId)
        {
            if (fface.NPC.HPPCurrent(targetId) > 0)
                return true;

            return false;
        }


        public bool Fight(int target)
        {
            while (CanStillAttack(target) && fface.Navigator.DistanceTo(target) < 20 && _ambuscade)
            {
                // attempt to retarget always
                Target(target);

                // if not claimed, attempt to claim with a ranged attack
                if (!fface.NPC.IsClaimed(target))
                    job.UseRangedClaim();

                // if not engaged, engaged
                if (((fface.NPC.IsClaimed(target) && PartyHasHate(target)) || fface.Navigator.DistanceTo(target) <= 5) &&
                    fface.Player.Status != Status.Fighting && fface.Target.ID == target && _ambuscade)
                {
                    fface.Windower.SendString("/attack <t>");
                    Thread.Sleep(3000);
                }

                // Move closer if too far away.
                if (fface.Navigator.DistanceTo(target) > 5 && fface.Player.Status == Status.Fighting && _ambuscade)
                {
                    fface.Navigator.Reset();
                    fface.Navigator.HeadingTolerance = 7;
                    fface.Navigator.DistanceTolerance = 4;
                    fface.Navigator.Goto(fface.NPC.PosX(target), fface.NPC.PosZ(target), false);
                }

                if (fface.Navigator.DistanceTo(target) < 2.5 && _ambuscade)
                {
                    fface.Windower.SendKey(KeyCode.NP_Number2, true);
                    Thread.Sleep(100);
                    fface.Windower.SendKey(KeyCode.NP_Number2, false);
                }

                if (fface.Player.Status == Status.Fighting)
                {
                    if (fface.Player.HPPCurrent <= 75)
                        job.UseHeals();

                    job.UseAbilities();

                    if (fface.Player.TPCurrent >= 1000)
                        job.UseWeaponskills();

                    job.UseSpells();
                } 
                Thread.Sleep(10);
            }
            return false;
        }

        public bool Target(int id)
        {
            if (fface.Target.ID == 0 || fface.Target.ID != id)
            {
                // Some how we ended up on the wrong target, disengage....
                if (fface.Target.ID != id && fface.Player.Status == Status.Fighting)
                {
                    fface.Windower.SendString("/attackoff");
                    Thread.Sleep(2000);
                    return true;
                }
                else
                {
                    fface.Target.SetNPCTarget(id);
                    Thread.Sleep(50);
                    fface.Windower.SendString("/target <t>");
                    Thread.Sleep(50);
                    return true;
                }
            }
            return true;
        }


        public bool CanStillAttack(int id)
        {

            if (!IsRendered(id))
            {
                return false;
            }

            if (fface.NPC.IsClaimed(id) && !PartyHasHate(id) && !IsFacingMe(id)) // fface.NPC.ClaimedID(id) != playerServerId))
            {
                return false;
            }

            // Skip if the mob more than 7 yalms above or below us
            if (Math.Abs(Math.Abs(fface.NPC.PosY(id)) - Math.Abs(fface.Player.PosY)) > 20)
            {
                return false;
            }

            //if (!fface.NPC.IsRendered(id))
            //    return false;

            // Skip if the NPC's HP is 0
            if (fface.NPC.HPPCurrent(id) == 0 || !IsRendered(id))
            {
                return false;
            }

            return true;
        }

        public bool IsFacingMe(int i)
        {

            double targetHeading = RadianToDegree(fface.NPC.PosH(i));
            double lineAngle = GetAngleOfLineBetweenTwoPoints(new PointF { X = fface.Player.PosX, Y = fface.Player.PosZ }, new PointF { X = fface.NPC.PosX(i), Y = fface.NPC.PosZ(i) });
            double difference = (targetHeading + lineAngle) - 180;
            if (difference < 3.5 && difference > -3.5)
            {
                return true;
            }

            return false;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }


        /// <summary>
        /// Gets the angle of a line between two points.
        /// </summary>
        /// <param name="p1">Player's Coordinates</param>
        /// <param name="p2">Target's Coordinates</param>
        /// <returns></returns>
        private double GetAngleOfLineBetweenTwoPoints(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }


        public int FindTarget()
        {
            int bestTarget = 0;
            double bestDistance = 20.0;

            for (short i = 0; i < 768; i++)
            {
                if (!IsRendered(i))
                    continue;

                if (fface.NPC.IsClaimed(i) && !PartyHasHate(i))
                    continue;

                if (fface.NPC.Distance(i) < 7 && fface.NPC.Status(i) == Status.Fighting &&
                    (!fface.NPC.IsClaimed(i) || PartyHasHate(i) || IsFacingMe(i)))
                {
                    WriteLog("Aggroed mob, taking care of it...");
                    return i;
                }

                if (fface.NPC.HPPCurrent(i) == 100 && fface.NPC.Name(i) == "Greater Manticore")
                {
                    if (fface.NPC.Distance(i) < bestDistance)
                    {
                        bestDistance = fface.NPC.Distance(i);
                        bestTarget = i;
                    }
                }
            }

            return bestTarget;
        }

        public bool IsRendered(int id)
        {
            byte[] b;
            b = fface.NPC.GetRawNPCData(id, 0x120, 4);
            if (b != null)
                return (BitConverter.ToInt32(b, 0) & 0x200) != 0;
            return false;
        }

        public bool PartyHasHate(int id)
        {
            for (int i = 0; i < 6; i++)
            {
                var members = fface.PartyMember[Convert.ToByte(i)];

                if (fface.NPC.ClaimedID(id) == members.ServerID && fface.NPC.HPPCurrent(id) > 0 && fface.NPC.Status(id) != Status.Dead1 && fface.NPC.Status(id) != Status.Dead2)
                {
                    return true;
                }
            }
            return false;
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
