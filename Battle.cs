using FFACETools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeenGames.Utils.AStarPathFinder;
using System.Globalization;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Net.Sockets;
using RedCorona.Net;

namespace FlipperD
{
    public class Battle
    {

        #region Properties
        /// <summary>
        ///  A list of Waypoints.
        /// </summary>
        public List<Node> Waypoints = new List<Node>();

        /// <summary>
        /// A list of hitboxes.
        /// </summary>
        private List<Monster> Monsters = new List<Monster>();

        /// <summary>
        /// A list of blacklisted monster IDs
        /// </summary>
        private Dictionary<int, Monster> Blacklisted = new Dictionary<int, Monster>();

        /// <summary>
        /// Instance of FFACE to use.
        /// </summary>
        public FFACE fface { get; set; }

        /// <summary>
        /// A list of hotspots.
        /// </summary>
        public List<Hotspot> Hotspots = new List<Hotspot>();

        public List<Blacklist> Blacklists = new List<Blacklist>();
        #endregion

        #region Fields
        public DateTime performedAction = DateTime.MinValue;
        public DateTime performedSpell = DateTime.MinValue;
        public int playerServerId;
        public string playerName;
        public bool movementEnabled = true;
        public bool battleEnabled = true;
        public bool learnEnabled = true;
        public Thread movementThread;
        public Thread battleThread;
        public Thread learnThread;
        public Thread chatThread;
        public Thread safeThread;
        public Mode mode;
        public Zone zone;
        public bool pauseMovement = false;
        public bool movementPaused = false;
        public byte[,] Grid { get; set; }
        public int offset = 2000;
        public Random rnd = new Random();
        #endregion

        #region HelperMethods

        public List<Node> SmoothNodes(List<Node> nodes)
        {
            WriteLog(LogType.Pathing, $"Smoothing nodes for a path with {nodes.Count()} nodes.");
            List<Node> SmoothLikeButter = new List<Node>();
            int Index = 0;
            try
            {
                foreach (Node node in nodes)
                {
                    if (Index > 0 && Index != nodes.Count - 1)
                    {
                        float nX = (nodes[Index - 1].X + nodes[Index].X + nodes[Index + 1].X) / 3;
                        float nZ = (nodes[Index - 1].Z + nodes[Index].Z + nodes[Index + 1].Z) / 3;
                        SmoothLikeButter.Add(new Node { X = nX, Z = nZ });
                    }
                    if (Index == 0)
                    {
                        float nX = (nodes[Index].X + nodes[Index + 1].X) / 2;
                        float nZ = (nodes[Index].Z + nodes[Index + 1].Z) / 2;
                        SmoothLikeButter.Add(new Node { X = nX, Z = nZ });
                    }
                    if (Index == nodes.Count - 1)
                    {
                        float nX = (nodes[Index - 1].X + nodes[Index].X) / 2;
                        float nZ = (nodes[Index - 1].Z + nodes[Index].Z) / 2;
                        SmoothLikeButter.Add(new Node { X = nX, Z = nZ });
                    }
                    Index++;
                }
                return SmoothLikeButter;
            }
            catch
            {
                return nodes;
            }

        }
        public Random r = new Random();

        public void UseWeaponskills()
        {
            if (fface.Player.TPCurrent > 1000)
            {
                if (mode == Mode.Dynamis)
                {
                    SendCommand("/ws \"" + Program.mainform.uxDynamisWeaponskill.Text + "\" <t>", 2);
                }
                else
                {
                    if ((Program.mainform.uxAM3.Checked && IsAfflicted(StatusEffect.Aftermath_lvl3) &&
                         fface.Player.TPCurrent < 3000) || !Program.mainform.uxAM3.Checked)
                    {
                        if (Program.mainform.uxWaitFor2000TP.Checked && fface.Player.TPCurrent >= 2000)
                            SendCommand("/ws \"" + Program.mainform.favouredWeaponskill.Text + "\" <t>", 2);
                        else if (!Program.mainform.uxWaitFor2000TP.Checked)
                            SendCommand("/ws \"" + Program.mainform.favouredWeaponskill.Text + "\" <t>", 2);
                    }
                    else if (Program.mainform.uxAM3.Checked && fface.Player.TPCurrent == 3000 && !IsAfflicted(StatusEffect.Aftermath_lvl3))
                    {
                        SendCommand("/ws \"" + Program.mainform.uxAMWS.Text + "\" <t>", 2);
                    }
                }
            }
        }

        public void Healing()
        {
            WriteLog(LogType.Character, $"Called Healing()");
            if (fface.Player.SubJob == Job.DNC || fface.Player.MainJob == Job.DNC)
            {
                if (IsAfflicted(StatusEffect.Attack_Down) || IsAfflicted(StatusEffect.Slow) || IsAfflicted(StatusEffect.Paralysis) || IsAfflicted(StatusEffect.Bind) || (IsAfflicted(StatusEffect.Silence) && fface.Player.MainJob == Job.PLD))
                {
                    if (Ready(AbilityList.Waltzes) && fface.Player.TPCurrent > 500)
                    {
                        UseAbility("Healing Waltz", AbilityList.Waltzes);
                    }
                }
                else if ((fface.Player.HPPCurrent < 50 && fface.Player.MainJob == Job.PLD) || (fface.Player.HPPCurrent < 75 && fface.Player.MainJob != Job.PLD))
                {
                    //if (Ready(AbilityList.Waltzes) && fface.Player.TPCurrent > 500)
                    //{
                    //    UseAbility("Curing Waltz III", AbilityList.Waltzes);
                    //}
                }
            }
            if (fface.Player.MainJob == Job.PLD)
            {
                if (fface.Player.HPPCurrent < 50)
                {
                    if (Ready(AbilityList.Sentinel))
                        UseAbility(AbilityList.Sentinel, 2);
                }

                if (mode == Mode.Einherjar && fface.Player.HPPCurrent < 75)
                {
                    if (Ready(SpellList.Cure_IV) && fface.Player.MPCurrent > 100)
                        UseSpell(SpellList.Cure_IV, 8, false);

                    if (Ready(SpellList.Cure_III) && fface.Player.MPCurrent > 80)
                        UseSpell(SpellList.Cure_III, 8, false);
                }
            }
            if (fface.Player.MainJob == Job.THF)
            {
                if (fface.Player.HPPCurrent < 40 && Ready(AbilityList.Two_Hour))
                    UseAbility("Perfect Dodge", AbilityList.Two_Hour);

            }
        }

        public void Stagger()
        {
            if (fface.Player.MainJob == Job.DNC || fface.Player.SubJob == Job.DNC)
            {

                if (Ready(AbilityList.Steps))
                {
                    UseAbility("Quickstep", AbilityList.Steps, Offensive: true);
                }

                if (Ready(AbilityList.Flourishes_I) && HasFinishingMove())
                {
                    UseAbility("Violent Flourish", AbilityList.Flourishes_I, Offensive: true);
                }
            }

        }

        public void UseAbilities()
        {

            if (fface.Player.MainJob == Job.RDM)
            {
                if (!IsAfflicted(StatusEffect.Haste) && Ready(SpellList.Haste_II))
                {
                    UseSpell(SpellList.Haste_II, 5, false);
                }
                if (!IsAfflicted(StatusEffect.Refresh) && Ready(SpellList.Refresh_II))
                {
                    UseSpell(SpellList.Refresh_II, 9, false);
                }
                if (!IsAfflicted(StatusEffect.Protect) && Ready(SpellList.Protect_V))
                {
                    UseSpell(SpellList.Protect_V, 5, false);
                }
                if (!IsAfflicted(StatusEffect.Shell) && Ready(SpellList.Shell_V))
                {
                    UseSpell(SpellList.Shell_V, 5, false);
                }
                if (!IsAfflicted(StatusEffect.Multi_Strikes) && Ready(SpellList.Temper))
                {
                    UseSpell(SpellList.Temper, 5, false);
                }
                if (!IsAfflicted(StatusEffect.Stoneskin) && Ready(SpellList.Stoneskin))
                {
                    UseSpell(SpellList.Stoneskin, 8, false);
                }
                if (!IsAfflicted(StatusEffect.Phalanx) && Ready(SpellList.Phalanx))
                {
                    UseSpell(SpellList.Phalanx, 8, false);
                }
                if (!IsAfflicted(StatusEffect.Composure) && Ready(AbilityList.Composure))
                {
                    UseAbility(AbilityList.Composure, 2, false);
                }
                if (!IsAfflicted(StatusEffect.Enthunder_2) && Ready(SpellList.Enthunder_II))
                {
                    UseSpell(SpellList.Enthunder_II, 5, false);
                }
            }
            if (fface.Player.MainJob == Job.SMN)
            {
                if (Program.mainform.uxAvatar.SelectedIndex == 0)
                {
                    if (!FindTargetAll("Diabolos").Any() && Ready(SpellList.Diabolos))
                    {
                        UseSpell(SpellList.Diabolos, 8, false);
                    }
                    else
                    {
                        // else
                        if (fface.Target.HPPCurrent > 97 && Ready(AbilityList.Assault))
                        {
                            SendCommand("/pet Assault <t>", 2, false);
                        }
                        if (Ready(AbilityList.Blood_Pact_Rage))
                        {
                            SendCommand("/pet \"Night Terror\" <t>", 2, false);
                        }
                    }
                }
                if (Program.mainform.uxAvatar.SelectedIndex == 1)
                {
                    if (!FindTargetAll("Fenrir").Any() && Ready(SpellList.Diabolos))
                    {
                        UseSpell(SpellList.Fenrir, 8, false);
                    }
                    else
                    {
                        // else
                        if (fface.Target.HPPCurrent > 97 && Ready(AbilityList.Assault))
                        {
                            SendCommand("/pet Assault <t>", 2, false);
                        }
                        if (Ready(AbilityList.Blood_Pact_Rage))
                        {
                            SendCommand("/pet \"Impact\" <t>", 2, false);
                        }
                    }
                }
                if (Program.mainform.uxAvatar.SelectedIndex == 2)
                {
                    if (!FindTargetAll("Ifrit").Any() && Ready(SpellList.Diabolos))
                    {
                        UseSpell(SpellList.Ifrit, 8, false);
                    }
                    else
                    {
                        // else
                        if (fface.Target.HPPCurrent > 97 && Ready(AbilityList.Assault))
                        {
                            SendCommand("/pet Assault <t>", 2, false);
                        }
                        if (Ready(AbilityList.Blood_Pact_Rage))
                        {
                            SendCommand("/pet \"Flaming Crush\" <t>", 2, false);
                        }
                    }
                }

            }
            if (fface.Player.MainJob == Job.BLU)
            {
                if (!IsAfflicted(StatusEffect.Refresh) && Ready(SpellList.Battery_Charge))
                    UseSpell(SpellList.Battery_Charge, 6, false);

                if (!IsAfflicted(StatusEffect.Attack_Boost) && Ready(SpellList.Nat_Meditation))
                    SendCommand("/ma \"Nat. Meditation\" <me>", 5, false);

                if (Ready(AbilityList.Efflux))
                    UseAbility(AbilityList.Efflux, 2, false);

                if (Ready(AbilityList.Chain_Affinity))
                    UseAbility(AbilityList.Chain_Affinity, 2, false);

                if (Ready(SpellList.Thrashing_Assault))
                    UseSpell(SpellList.Thrashing_Assault, 4, true);

                if (Ready(SpellList.Quad_Continuum))
                    SendCommand("/ma \"Quad. Continuum\" <t>", 4, false);

                if (Ready(SpellList.Heavy_Strike))
                    UseSpell(SpellList.Heavy_Strike, 4, true);
            }
            if (fface.Player.SubJob == Job.DNC || fface.Player.MainJob == Job.DNC)
            {
                //if (!IsAfflicted(StatusEffect.Haste_Samba) && fface.Player.TPCurrent > 350 && (mode == Mode.Dynamis || mode == Mode.Einherjar))
                //    UseAbility("Haste Samba", AbilityList.Sambas);

                if (mode == Mode.Farming && HasFinishingMove())
                {
                    if (Ready(AbilityList.Flourishes_I))
                        UseAbility("Violent Flourish", AbilityList.Flourishes_I, 2, true);
                }
            }
            if (fface.Player.MainJob == Job.WAR || fface.Player.SubJob == Job.WAR)
            {
                if (Ready(AbilityList.Berserk) && !Program.mainform.chkAntiBerserk.Checked)
                    UseAbility(AbilityList.Berserk);

                else if (Ready(AbilityList.Warcry))
                    UseAbility(AbilityList.Warcry);
            }
            if (fface.Player.MainJob == Job.PLD)
            {

                if (!Program.mainform.uxSpamWS.Checked && !Program.mainform.uxHoldTP.Checked)
                {
                    if (fface.Player.SubJob == Job.WAR)
                    {
                        if (Ready(AbilityList.Aggressor))
                            UseAbility(AbilityList.Aggressor, 2, false);

                        if (Ready(AbilityList.Provoke) && fface.Target.HPPCurrent > 50 && !Program.mainform.chkNoProvoke.Checked && !Program.mainform.uxRapidClaim.Checked)
                            UseAbility(AbilityList.Provoke, 3, true);

                        if (Ready(SpellList.Flash) && fface.Target.HPPCurrent > 80 && !Program.mainform.chkNoProvoke.Checked)
                            UseSpell(SpellList.Flash, 4, true);

                        if (Ready(AbilityList.Shield_Bash) && !Program.mainform.chkNoProvoke.Checked)
                            UseAbility(AbilityList.Shield_Bash, 2, true);
                    }

                    if (fface.Target.HPPCurrent > 90 && Ready(SpellList.Flash))
                    {
                        UseSpell(SpellList.Flash, 4, true);
                    }
                    if (fface.Target.HPPCurrent > 75 && !IsAfflicted(StatusEffect.Enlight))
                    {
                        SendCommand("/ma \"Enlight II\" <me>", 8, false);
                    }

                    if (mode == Mode.Einherjar)
                    {
                        if (Ready(SpellList.Reprisal) && !IsAfflicted(StatusEffect.Reprisal))
                            UseSpell(SpellList.Reprisal, 5, false);

                        if (!IsAfflicted(StatusEffect.Protect) && Ready(SpellList.Protect_V))
                            UseSpell(SpellList.Protect_V, 9, false);

                        if (!IsAfflicted(StatusEffect.Shell) && Ready(SpellList.Shell_IV))
                            UseSpell(SpellList.Shell_IV, 9, false);
                    }
                }
            }
            if (fface.Player.MainJob == Job.THF)
            {


            }
        }

        public void MovingClaim(int id)
        {
            //WriteLog(LogType.Battle, $"Attempting to claim while moving! MovingClaim({id})");
            Target(id);
            double Distance = fface.Navigator.DistanceTo(id);

            if (_RangedOnly)
            {
                fface.Navigator.FaceHeading(id);
                WriteLog($"Facing ({id}) and /ra'ing!");
                Thread.Sleep(1000);
                SendCommand("/ra <t>", 5);
            }

            if ((fface.Player.MainJob == Job.WAR || fface.Player.SubJob == Job.WAR) && Distance < 17.8)
            {
                if (Program.mainform.uxRapidClaim.Checked && Ready(AbilityList.Provoke))
                {
                    WriteLog("Rapid Claim Attempt!", true);
                    UseAbility(AbilityList.Provoke, Offensive: true);
                    return;
                }
                else
                {
                    if (Ready(AbilityList.Provoke) && !Program.mainform.chkNoProvoke.Checked)
                        UseAbility(AbilityList.Provoke, Offensive: true);
                }
            }

            else if (fface.Player.MainJob == Job.MNK && Distance < 17.4)
            {
                if (Ready(AbilityList.Chi_Blast))
                    UseAbility(AbilityList.Chi_Blast, Offensive: true);
            }

            else if ((fface.Player.MainJob == Job.DNC || fface.Player.SubJob == Job.DNC) && Distance < 17)
            {
                if (Ready(AbilityList.Flourishes_I) && HasFinishingMove())
                    UseAbility("Animated Flourish", AbilityList.Flourishes_I, Offensive: true);
            }

            else if (fface.Player.MainJob == Job.THF)
            {
                if (Ready(AbilityList.Bully) && Distance < 12)
                    UseAbility(AbilityList.Bully, Offensive: true);
            }

        }

        public bool HasFinishingMove()
        {
            if (IsAfflicted(StatusEffect.Finishing_Move1) ||
                   IsAfflicted(StatusEffect.Finishing_Move2) ||
                   IsAfflicted(StatusEffect.Finishing_Move3) ||
                   IsAfflicted(StatusEffect.Finishing_Move4) ||
                   IsAfflicted(StatusEffect.Finishing_Move5))
                return true;

            return false;
        }


        /// <summary>
        /// Check if our current target is still the best target.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool StillBestTarget(int id)
        {
            if (mode == Mode.Farming)
                return true;

            if (id == FindBestTarget())
                return true;

            return false;
        }

        List<string> RangedAttackers = new List<string> {
            "Vanguard Prelate",
            "Vanguard Assassin",
            "Vanguard Salvager",
            "Vanguard Backstabbe",
            "Vanguard Shaman",
            "Vanguard Hitman"
        };

        public bool HasAggro()
        {
            WriteLog(LogType.Battle, $"Checking if player HasAggro()");
            if (RangedAttacker())
            {
                WriteLog(LogType.Battle, $"==> TRUE. The Attacker is Ranged.");
                return true;
            }

            if (fface.Player.Status == Status.Fighting)
            {
                WriteLog(LogType.Battle, $"==> TRUE. The player is currently engaged.");
                return true;
            }

            if (HasLiveAggro())
                return true;

            if (FindBestTarget() == 0)
            {
                WriteLog(LogType.Battle, $"==> FALSE. No Target is in Range!");
                return false;
            }

            if (fface.NPC.Distance(FindBestTarget()) > 10 && FindBestTarget() > 0)
            {
                WriteLog(LogType.Battle, $"==> FALSE. There's a target in range - but more than 10 yalms away!");
                return false;
            }

            WriteLog(LogType.Battle, $"==> TRUE. Because all checks to eliminate aggro failed.");
            return true;
        }

        public bool RangedAttacker(int i)
        {
            if (RangedAttackers.IndexOf(fface.NPC.Name(i)) > -1)
            {
                if (fface.NPC.Distance(i) < 20 &&
                    fface.NPC.Status(i) == Status.Fighting &&
                    !fface.NPC.IsClaimed(i) &&
                    IsFacingMe(i) && IsRendered(i) && !fface.NPC.Name(i).Contains("Vanguard's"))
                {
                    WriteLog("Focusing on this target... " + i.ToString());
                    RangedTarget = i;
                    FocusRanged = true;
                    WriteLog(LogType.Battle, $"There's a ranged attacker, and it's facing me!");
                    return true;

                }
            }
            RangedTarget = 0;
            FocusRanged = false;
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

        public double TankDistance(int i, string Name = "Trion")
        {
            for (int y = 0; y < 6; y++)
            {
                var member = fface.PartyMember[Convert.ToByte(y)];
                if (member.Name == Name)
                {
                    WriteLog("Found Trion!");
                    Node s = new Node() { X = fface.NPC.PosX(i), Z = fface.NPC.PosZ(i) };
                    Node d = new Node() { X = fface.NPC.PosX(member.ID), Z = fface.NPC.PosZ(member.ID) };
                    return DistanceTo(s, d);
                }
            }
            return 0;
        }

        public int MasterClaimedId(string Name)
        {
            int master = 0;
            for (int i = 0; i < 5; i++)
            {
                var members = fface.PartyMember[Convert.ToByte(i)];

                if (members.Name == Name)
                    master = members.ServerID;
            }
            if (master == 0)
            {
                WriteLog("Can't find master, douchekabob.");
                return 0;
            }

            for (short i = 0; i < 768; i++)
            {
                if (fface.NPC.ClaimedID(i) == master && fface.NPC.HPPCurrent(i) > 0)
                {
                    return i;
                }

            }
            return 0;
        }


        public bool RangedAttacker()
        {
            for (short i = 0; i < 768; i++)
            {
                if (RangedAttackers.IndexOf(fface.NPC.Name(i)) > -1)
                {
                    if (fface.NPC.Distance(i) < 20 &&
                        fface.NPC.Status(i) == Status.Fighting &&
                        !fface.NPC.IsClaimed(i) &&
                        IsFacingMe(i))
                    {
                        WriteLog(LogType.Battle, $"There's a ranged attacker, and it's facing me! {i} - {fface.NPC.Name(i)}");
                        RangedTarget = i;
                        FocusRanged = true;
                        return true;

                    }
                }
            }
            RangedTarget = 0;
            FocusRanged = false;
            return false;
        }

        public int RangedTarget = 0;
        public bool FocusRanged = false;

        public bool IsFacingMe(int i)
        {

            double targetHeading = RadianToDegree(fface.NPC.PosH(i));
            double lineAngle = GetAngleOfLineBetweenTwoPoints(new PointF { X = fface.Player.PosX, Y = fface.Player.PosZ }, new PointF { X = fface.NPC.PosX(i), Y = fface.NPC.PosZ(i) });
            double difference = (targetHeading + lineAngle) - 180;
            if (difference < 3.5 && difference > -3.5)
            {
                WriteLog(LogType.Battle, $"{fface.NPC.Name(i)} is facing me!!");
                return true;
            }

            return false;
        }

        private double GetSADifference(int i)
        {
            var TargetHeading = (int)(fface.Navigator.PosHToDegrees(fface.NPC.PosH(i)));
            var MyHeading = (int)(fface.Navigator.GetPlayerPosHInDegrees());
            double targetHeading = RadianToDegree(fface.NPC.PosH(i));

            double lineAngle = GetAngleOfLineBetweenTwoPoints(new PointF { X = fface.Player.PosX, Y = fface.Player.PosZ }, new PointF { X = fface.NPC.PosX(i), Y = fface.NPC.PosZ(i) });
            double difference = (targetHeading + lineAngle) % 360;

            return difference;
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


        public bool CanStillAttack(int id)
        {
            if (Blacklisted.ContainsKey(id))
            {
                WriteLog(LogType.Battle, $"I can't attack {fface.NPC.Name(id)} - BLISTED - CanStillAttack({id})");
                return false;
            }

            if (!IsRendered(id))
            {
                WriteLog(LogType.Battle, $"I can't attack {fface.NPC.Name(id)} - NOT RENDERED - CanStillAttack({id})");
                return false;
            }

            if (fface.NPC.IsClaimed(id) && !PartyHasHate(id) && !IsFacingMe(id)) // fface.NPC.ClaimedID(id) != playerServerId))
            {
                WriteLog(LogType.Battle, $"I can't attack {fface.NPC.Name(id)} - NOT CLAIMED TO ME - CanStillAttack({id})");
                return false;
            }

            // Skip if the mob more than 7 yalms above or below us
            if (Math.Abs(Math.Abs(fface.NPC.PosY(id)) - Math.Abs(fface.Player.PosY)) > 20)
            {
                WriteLog(LogType.Battle, $"I can't attack {fface.NPC.Name(id)} - Z-DIFFERENCE - CanStillAttack({id})");
                return false;
            }

            //if (!fface.NPC.IsRendered(id))
            //    return false;

            // Skip if the NPC's HP is 0
            if (fface.NPC.HPPCurrent(id) == 0 || !IsRendered(id))
            {
                WriteLog(LogType.Battle, $"I can't attack {fface.NPC.Name(id)} - DEAD! - CanStillAttack({id})");
                return false;
            }


            return true;
        }

        /// <summary>
        /// Find the best target for us to engage.
        /// </summary>
        /// <returns></returns>
        public int FindBestTarget()
        {
            int BestTarget = 0;
            double BestDistance =  _RangedOnly ? 20 : 50;

            // Set CurrentTargets to the monsters list.
            List<Monster> CurrentTargets = Monsters;

            if (mode == Mode.Dynamis)
            {
                //Filter targets by time due to being in Dynamis
                FFACE.TimerTools.VanaTime currentTime = fface.Timer.GetVanaTime();
                CurrentTargets = CurrentTargets.Where(target =>
                        target.TimeBegin.Hour <= currentTime.Hour &&
                        target.TimeEnd.Hour >= currentTime.Hour
                    ).ToList();
                
            }
            if (!FocusRanged)
            {
                for (short i = 0; i < 768; i++)
                {

                    if (fface.NPC.Name(i).Contains("Vanguard's"))
                        continue;

                    // Blacklisted.
                    if (Blacklisted.ContainsKey(i))
                        continue;

                    if (fface.NPC.IsClaimed(i) && !PartyHasHate(i) && fface.NPC.Distance(i) < 50 && IsRendered(i))
                    {
                        continue;
                    }

                    if (RangedAttacker(i) && IsRendered(i))
                    {
                        WriteLog(LogType.Targeting, $"{fface.NPC.Name(i)} is my new target RangedAttacker() and IsRendered()");
                        return i;
                    }

                    if (FocusRanged && i == RangedTarget && IsRendered(i))
                    {
                        WriteLog(LogType.Targeting, $"{fface.NPC.Name(i)} is my new target RangedAttacker() and IsRendered() and FocusRanged");
                        return i;
                    }

                    //if (ChangingArea)
                    //    return 0;

                    if (mode == Mode.Dynamis)
                    {
                        // The monster is 12 yalms away, and 'Fighting', probably aggroed someone else.
                        if (fface.NPC.Distance(i) > 7 && fface.NPC.Status(i) == Status.Fighting && !PartyHasHate(i))//fface.NPC.ClaimedID(i) != playerServerId)
                            continue;

                        // Mob is within 7 yalms, unclaimed (or claimed to us) and fighting. It's probably aggroed us, whatever it is.
                        if (fface.NPC.Distance(i) <= 7 && fface.NPC.Status(i) == Status.Fighting && ((fface.NPC.ClaimedID(i) == 0) || PartyHasHate(i)) && IsRendered(i)) 
                        {
                            if (Monsters.Count(x => x.MonsterName == fface.NPC.Name(BestTarget)) == 0 ||
                               (Monsters.Count(x => x.MonsterName == fface.NPC.Name(BestTarget)) > 0 && BestDistance >= 9))
                            {
                                if (IsRendered(i))
                                {
                                    WriteLog(LogType.Targeting, $"{fface.NPC.Name(i)} is my new target - AGGROED!");
                                    BestTarget = i;
                                    BestDistance = fface.NPC.Distance(i);
                                    return i;
                                }
                                continue;
                            }
                        }
                    }

                    if (CurrentTargets.Count(x => x.MonsterName == fface.NPC.Name(i)) == 0 && mode != Mode.Einherjar)
                        continue;



                    // Too far away.
                    if (fface.NPC.Distance(i) > BestDistance || fface.NPC.Distance(i) >= 50)
                        continue;

                    // Skip if the mob is charmed.
                    //if (IsCharmed(i))
                    //{
                    //    WriteLog("Ignoring charmed monster: " + fface.NPC.Name(i));
                    //    continue;
                    //}

                    // Skip if the mob is claimed to someone other than us.
                    if (fface.NPC.IsClaimed(i) && !PartyHasHate(playerServerId))//fface.NPC.ClaimedID(i) != playerServerId)
                        continue;



                    // Skip if the mob is dead or not rendered
                    if (fface.NPC.Status(i) == Status.Dead1 || fface.NPC.Status(i) == Status.Dead2 || !IsRendered(i))
                    {
                        continue;
                    }


                    // Skip if the mob more than 7 yalms above or below us
                    if (Math.Abs(Math.Abs(Math.Abs(fface.NPC.PosY(i)) - Math.Abs(fface.Player.PosY))) > 7)
                        continue;


                    // Skip if the mob is not rendered
                    //if (!fface.NPC.IsRendered(i))
                    //{
                    //    WriteLog("Not Rendered :(" + fface.NPC.Name(i));
                    //    continue;
                    //}

                    // Skip if the NPC's HP is 0
                    if (fface.NPC.HPPCurrent(i) == 0)
                        continue;


                    // Finally, let's check if the mob is in our list...
                    if (CurrentTargets.Count(x => x.MonsterName == fface.NPC.Name(i)) > 0 && mode != Mode.Einherjar)
                    {
                        BestTarget = i;
                        BestDistance = fface.NPC.Distance(i);
                    }
                    if (mode == Mode.Einherjar)
                    {

                        BestTarget = i;
                        BestDistance = fface.NPC.Distance(i);
                    }
                }
            }
            else
            {
                WriteLog(LogType.Targeting, $"For some reason we gave up on our ranged target. Trying again!");
                return RangedTarget;
            }
            //WriteLog(LogType.Targeting, $"FindBestTarget() => Returning {BestTarget} - {fface.NPC.Name(BestTarget)}");
            return BestTarget;
        }


        /// <summary>
        /// Check if the target is charmed.
        /// </summary>
        /// <param name="mobId"></param>
        /// <returns></returns>
        public bool IsCharmed(int mobId)
        {
            byte[] data = fface.NPC.GetRawNPCData(mobId, 0, 0x266);
            return ((data[0x10C] & 0x20) > 0);
        }


        /// <summary>
        /// Get the closest waypoint to a given X,Z
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Node GetWaypointClosestTo(float x, float z)
        {
            Node ClosestWP = new Node();
            double ClosestDistance = 9999;
            foreach (Node wp in Waypoints)
            {
                Node start = new Node { X = x, Z = z };
                Node end = new Node { X = wp.X, Z = wp.Z };
                if (DistanceTo(start, end) < ClosestDistance)
                {
                    ClosestWP = wp;
                    ClosestDistance = DistanceTo(start, end);
                }
            }
            return ClosestWP;
        }

        public bool PauseRoaming()
        {
            _roam = false;
            fface.Navigator.Reset();
            
            if (!_IsMovementPaused)
            {
                fface.Navigator.Reset();
                return false;
            }
            return true;

        }

        public bool ResumeRoaming()
        {
            _roam = true;
            return true;
        }


        /// <summary>
        /// Pause movement
        /// </summary>
        public void PauseMovement2()
        {
            fface.Navigator.Reset();
            pauseMovement = true;
            fface.Navigator.Reset();
        }

        public bool EligibleTime(Monster monster)
        {
            FFACE.TimerTools.VanaTime currentTime = fface.Timer.GetVanaTime();
            if (monster.TimeBegin.Hour <= currentTime.Hour && monster.TimeEnd.Hour >= currentTime.Hour && monster.TimeSpecific)
            {
                return true;
            }
            return false;           
        }

        /// <summary>
        /// Get a list of HotSpots.
        /// </summary>
        /// <returns></returns>
        public List<Hotspot> GetHotSpots()
        {
            List<Hotspot> timedTest = Hotspots.Where(x => x.TimeSpecific).ToList();
            bool timed = (timedTest.Count > 0);

            List<Hotspot> hotspots = Hotspots;
            if (timed && movementEnabled)
            {
                // Clear hotspots, we need to select some specific ones.
                FFACE.TimerTools.VanaTime currentTime = fface.Timer.GetVanaTime();
                // Current time is 19

                // hotspot begin is 1
                // hotspot end is 19
                hotspots = hotspots.Where(hs =>
                           hs.TimeBegin.Hour <= currentTime.Hour &&
                           hs.TimeEnd.Hour >= currentTime.Hour).ToList();
            }
            else
            {
                hotspots = Hotspots;
            }

            return hotspots;
        }


        private PathFinderFast mesh;

        /// <summary>
        /// Get a path to destination X,Z
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public List<Node> GetPath(float X, float Z)
        {
            if (mesh == null)
            {
                mesh = new PathFinderFast(Grid)
                {
                    Formula = HeuristicFormula.Manhattan,
                    Diagonals = true,
                    PunishChangeDirection = false,
                    TieBreaker = false,
                    SearchLimit = 1000000
                };
            }

            List<Node> ReturnPath = new List<Node>();
            int StartingX = Convert.ToInt32(fface.Player.PosX) + offset;
            int StartingZ = Convert.ToInt32(fface.Player.PosZ) + offset;
            int DestinationX = Convert.ToInt32(X) + offset;
            int DestinationZ = Convert.ToInt32(Z) + offset;

            WriteLog(LogType.Pathing, $"Generating a path to {X} , {Z}");
            List<PathFinderNode> path = new List<PathFinderNode>();
            try
            {
                path = mesh.FindPath(new DeenGames.Utils.Point(StartingX, StartingZ),
                    new DeenGames.Utils.Point(DestinationX, DestinationZ));
            }
            catch
            {

                path = null;
            }
            if (path != null)
            {
                WriteLog(LogType.Pathing, $"Generation succeeded! {path.Count()} nodes were assembled");
                foreach (PathFinderNode point in path)
                {
                    ReturnPath.Add(new Node {X = point.X - offset, Z = point.Y - offset});
                }
            }
            else
            {
                WriteLog(LogType.Pathing, $"Pathing failed (No route): {Convert.ToInt32(fface.Player.PosX)},{Convert.ToInt32(fface.Player.PosZ)} to {X},{Z}!");
            }
            ReturnPath = SmoothNodes(ReturnPath);

            ReturnPath.Reverse();

            return ReturnPath;
        }

        private void Mesh_PathFinderDebug(int fromX, int fromY, int x, int y, PathFinderNodeType type, int totalCost, int cost)
        {
            WriteLog(LogType.Pathing, $"{type.ToString()} {totalCost} {cost}");
        }


        /// <summary>
        /// Get the closest hotspot to us.
        /// </summary>
        /// <param name="hotspots"></param>
        /// <returns></returns>
        public int GetClosestHotspot(List<Hotspot> hotspots)
        {
            int index = 0;
            int closest = 0;
            int closestDistance = 9999;
            foreach (Hotspot hotspot in hotspots)
            {
                if (fface.Navigator.DistanceTo(hotspot.Waypoint.X, hotspot.Waypoint.Z) < closestDistance)
                {
                    closest = index;
                    closestDistance = Convert.ToInt32(fface.Navigator.DistanceTo(hotspot.Waypoint.X, hotspot.Waypoint.Z));
                }
                index++;
            }
            return closest;
        }
        public Dictionary<string, DateTime> CommandsLog = new Dictionary<string, DateTime>();
        public DateTime NextCommandAllowed = DateTime.MinValue;


        public void SendCommand(string command, int Delay = 2, bool PreventGlobal = false)
        {
            if ((!CommandsLog.ContainsKey(command) || (CommandsLog.ContainsKey(command) && CommandsLog[command] < DateTime.Now)) &&
                (NextCommandAllowed <= DateTime.Now || PreventGlobal))
            {
                //WriteLog("BattleBot: Sending command ->" + command, true);

                if (CommandsLog.ContainsKey(command))
                    CommandsLog[command] = DateTime.Now.AddSeconds(Delay);
                else
                    CommandsLog.Add(command, DateTime.Now.AddSeconds(Delay));

                if (!PreventGlobal)
                    NextCommandAllowed = DateTime.Now.AddSeconds(Delay);
                else
                    NextCommandAllowed = DateTime.MinValue;

                fface.Windower.SendString(command);
            }
        }


        public void UseAbility(AbilityList ability, int Delay = 2, bool Offensive = false)
        {
            SendCommand("/ja \"" + ability.ToString().Replace('_', ' ') + "\" " + (Offensive ? "<t>" : "<me>"), Delay);
        }


        public void UseAbility(string ability, AbilityList abilityGroup, int Delay = 2, bool Offensive = false)
        {
            SendCommand("/ja \"" + ability.ToString().Replace('_', ' ') + "\" " + (Offensive ? "<t>" : "<me>"), Delay);
        }


        public void UseSpell(SpellList spell, int castTime, bool Offensive = false)
        {
            SendCommand("/ma \"" + spell.ToString().Replace('_', ' ') + "\" " + (Offensive ? "<t>" : "<me>"));
        }

        public string lastLog = "";
        /// <summary>
        /// Write a line to the log.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="verbose"></param>
        public void WriteLog(string log, bool verbose = false)
        {
            if (Program.mainform.chkVerboseLogging.Checked && verbose || !verbose)
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
        }

        public void WriteLog(LogType type, string log)
        {
            //if (lastLog == log) return;
            //lastLog = log;
            //Program.mainform.uxLog.Invoke((MethodInvoker)delegate
            //{
            //    Program.mainform.uxLog.Items.Add("[" + DateTime.Now.ToString("HH:mm:ss") + "]["+type+"]: " + log);
            //    int visibleItems = Program.mainform.uxLog.ClientSize.Height / Program.mainform.uxLog.ItemHeight;
            //    Program.mainform.uxLog.TopIndex = Math.Max(Program.mainform.uxLog.Items.Count - visibleItems + 1, 0);
            //});
        }

        public enum LogType
        {
            Generic,
            Targeting,
            Battle,
            BattleNav,
            Roaming,
            Hotspot,
            Pathing,
            Character
        }

        /// <summary>
        /// Check if the user is afflicted with the given buff or enfeeblmenet.
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        public bool IsAfflicted(StatusEffect effect)
        {
            foreach (StatusEffect status in fface.Player.StatusEffects)
            {
                if (status == effect)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        ///  Check if the provided spell is ready to use.
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public bool Ready(SpellList spell)
        {
            if (fface.Timer.GetSpellRecast(spell) == 0)
                return true;

            return false;
        }


        /// <summary>
        /// Check if the provided ability is ready to use.
        /// </summary>
        /// <param name="ability"></param>
        /// <returns></returns>
        public bool Ready(AbilityList ability)
        {
            if (fface.Timer.GetAbilityRecast(ability) == 0)// && fface.Player.HasAbility(ability))
                return true;

            return false;
        }


        /// <summary>
        /// Check the distance between two points (FFACE.Position)
        /// </summary>
        /// <param name="start">Starting Position</param>
        /// <param name="dest">Ending Position</param>
        /// <returns></returns>
        public double DistanceTo(Node start, Node dest)
        {
            return Math.Sqrt(Math.Pow(start.X - dest.X, 2) + Math.Pow(start.Z - dest.Z, 2));
        }

        #endregion

        #region Loading Methods

        public bool LoadCoords(Zone zone, Mode mode)
        {
            List<string> files = new List<string>();

            Waypoints.Clear();
            string loadFile = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\global.mesh";
            string loadFileM = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\" + (int)zone + ".mesh";
            files.Add(loadFile);
            files.Add(loadFileM);
            int caught = 0;

            foreach (string f in files)
            {
                if (File.Exists(f))
                {
                    string line;
                    // Read the file and display it line by line.
                    System.IO.StreamReader file = new System.IO.StreamReader(f);
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] token = line.Split(',');
                        if (Convert.ToInt32(token[1]) == (int) zone)
                        {
                            bool inblackspot = false;
                            foreach (Blacklist b in Blacklists)
                            {
                                if (InCircle((int) b.Waypoint.X, (int) b.Waypoint.Z, Convert.ToInt32(token[2]),
                                    Convert.ToInt32(token[4]), Convert.ToInt32(b.Radius)))
                                {
                                    inblackspot = true;
                                    caught++;
                                    //WriteLog("In Blackpost (Ignoring!!): " + token[2] + " / " + token[4]);
                                }
                            }
                            if (!inblackspot)
                            {
                                Waypoints.Add(new Node
                                {
                                    X = float.Parse((token[2]), CultureInfo.InvariantCulture),
                                    Y = float.Parse((token[3]), CultureInfo.InvariantCulture),
                                    Z = float.Parse((token[4]), CultureInfo.InvariantCulture)
                                });
                                Grid[
                                        int.Parse((token[2]), CultureInfo.InvariantCulture) + offset,
                                        int.Parse((token[4]), CultureInfo.InvariantCulture) + offset] =
                                    PathFinderHelper.EMPTY_TILE;
                            }
                        }
                    }
                    WriteLog($"Excluded {caught} nodes, due to being in blackspots.");
                    WriteLog($"Loaded {Waypoints.Count} ({f.Split(Convert.ToChar(@"\"))[f.Split(Convert.ToChar(@"\")).Count()-1].Replace(".mesh","")}) nodes.");
                    file.Close();
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="mode"></param>
        /// <returns>()
        /// Format: zone,name,hitbox,timespecific,[start],[end]
        /// Eg: 103,Nightmare Treant,6.5,true,0:00,8:00
        /// Eg: 203,Orcish Grunt,4.5,false
        /// </returns>
        public bool LoadMonsters(Zone zone, Mode mode)
        {
            Monsters.Clear();

            // Default monster.
            Monsters.Add(new Monster
            {
                HitBox = 2.5,
                MonsterName = "Default",
                TimeSpecific = false
            });

            string loadFile = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\monsters.list";
            if (File.Exists(loadFile))
            {
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(loadFile);
                while ((line = file.ReadLine()) != null)
                {
                    string[] token = line.Split(',');
                    if (Convert.ToInt32(token[0]) == Convert.ToInt32(zone))
                    {
                        Monster monster = new Monster();
                        monster.MonsterName = token[1];
                        monster.HitBox = Convert.ToDouble(token[2], CultureInfo.InvariantCulture);
                        monster.TimeSpecific = Convert.ToBoolean(token[3]);
                        if (monster.TimeSpecific)
                        {
                            monster.TimeBegin = new FFACE.TimerTools.VanaTime { Hour = Convert.ToByte(token[4].Split(':')[0]), Minute = Convert.ToByte(token[4].Split(':')[1]) };
                            monster.TimeEnd = new FFACE.TimerTools.VanaTime { Hour = Convert.ToByte(token[5].Split(':')[0]), Minute = Convert.ToByte(token[5].Split(':')[1]) };
                        }
                        Monsters.Add(monster);
                    }
                }
                WriteLog($"Loaded {Monsters.Count-1} targets.");
                file.Close();
            }

            return true;
        }

        public bool LoadBlacklist(Zone zone, Mode mode)
        {
            Blacklists.Clear();
            string loadFile = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\" + Convert.ToInt32(zone) + ".blackspot";
            if (File.Exists(loadFile))
            {
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(loadFile);
                while ((line = file.ReadLine()) != null)
                {
                    string[] token = line.Split(',');
                    if (Convert.ToInt32(token[0]) == Convert.ToInt32(zone))
                    {

                        Blacklist blacklist = new Blacklist();
                        blacklist.Waypoint = new Node { X = (float)Convert.ToDouble(token[1], CultureInfo.InvariantCulture), Y = (float)Convert.ToDouble(token[2], CultureInfo.InvariantCulture), Z = (float)Convert.ToDouble(token[3], CultureInfo.InvariantCulture) };
                        blacklist.Radius = Convert.ToDouble(token[4]);
                        Blacklists.Add(blacklist);
                    }
                }
                WriteLog($"Loaded {Blacklists.Count} blackspots.");

            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="mode"></param>
        /// <returns>
        /// Format:
        /// zone,name,x,y,z,timespecific,start,end
        /// 103,First,-1,-1,-1,true,0:00,8:00
        /// 103,First,-1,-1,-1,false
        /// </returns>
        public bool LoadHotspots(Zone zone, Mode mode)
        {
            Hotspots.Clear();
            string loadFile = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\hotspot.list";
            if (File.Exists(loadFile))
            {
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(loadFile);
                while ((line = file.ReadLine()) != null)
                {
                    string[] token = line.Split(',');
                    if (Convert.ToInt32(token[0]) == Convert.ToInt32(zone))
                    {

                        Hotspot hotspot = new Hotspot();
                        hotspot.Name = token[1];
                        hotspot.Waypoint = new Node { X = (float)Convert.ToDouble(token[2], CultureInfo.InvariantCulture), Y = (float)Convert.ToDouble(token[3], CultureInfo.InvariantCulture), Z = (float)Convert.ToDouble(token[4], CultureInfo.InvariantCulture) };
                        hotspot.TimeSpecific = Convert.ToBoolean(token[5]);
                        if (hotspot.TimeSpecific)
                        {
                            hotspot.TimeBegin = new FFACE.TimerTools.VanaTime { Hour = Convert.ToByte(token[6].Split(':')[0]), Minute = Convert.ToByte(token[6].Split(':')[1]) };
                            hotspot.TimeEnd = new FFACE.TimerTools.VanaTime { Hour = Convert.ToByte(token[7].Split(':')[0]), Minute = Convert.ToByte(token[7].Split(':')[1]) };
                        }
                        Hotspots.Add(hotspot);
                    }
                }
                WriteLog($"Loaded {Hotspots.Count} hotspots.");
                file.Close();
            }
            return true;
        }

        public bool InCircle(int centerx, int centery, int x, int y, int radius = 4)
        {
            var dist = Math.Sqrt(Math.Pow((centerx - x), 2) + Math.Pow((centery - y), 2));
            return dist < radius;
        }

        #endregion


        #region Enums
        public enum Mode
        {
            Farming = 0,
            Dynamis = 1,
            Salvage = 2,
            Limbus = 3,
            Einherjar
        }

        #endregion

        ClientInfo client;

        public Zone _startZone;

        public volatile int swhiteCount = 0;
        public volatile int sbyneCount = 0;
        public volatile int sordelleCount = 0;
        public volatile bool _RangedOnly = false;


        #region Methods

        /// <summary>
        /// Start BattleBot
        /// </summary>
        /// <param name="hitbox"></param>
        /// <param name="ffref"></param>
        public void StartBattle(FFACE ffref, Mode reqMode, Zone reqZone)
        {

            if (Program.mainform.chkRangedOnly.Checked)
                _RangedOnly = true;

            _startZone = ffref.Player.Zone;

            if (Program.mainform.uxNyzul.Checked)
            {
                chatEnabled = true;
                fface = ffref;
                chatThread = new Thread(ChatRoutine);
                chatThread.Start();

            }
            else
            {

                // Prepare the Grid.
                Grid =
                    new byte[PathFinderHelper.RoundToNearestPowerOfTwo(4000),
                        PathFinderHelper.RoundToNearestPowerOfTwo(4000)];
                for (int i = 0; i < 4096; i++)
                {
                    for (int j = 0; j < 4096; j++)
                    {
                        Grid[i, j] = PathFinderHelper.BLOCKED_TILE;
                    }
                }

                fface = ffref;
                mode = reqMode;


                swhiteCount = Convert.ToInt32(fface.Item.GetInventoryItemCount(1449));
                sbyneCount = Convert.ToInt32(fface.Item.GetInventoryItemCount(1455));
                sordelleCount = Convert.ToInt32(fface.Item.GetInventoryItemCount(1452));

                // Ensure both threads are permitted to run.
                _roam = true;
                _movementThread = true;

                movementEnabled = true;
                pauseMovement = false;
                battleEnabled = true;
                _battle = true;
                learnEnabled = false;
                chatEnabled = true;
                safeCheckEnabled = true;

                LoadBlacklist(reqZone, reqMode);
                LoadMonsters(reqZone, reqMode);
                if (mode != Mode.Einherjar)
                {
                    LoadHotspots(reqZone, reqMode);
                    LoadCoords(reqZone, reqMode);
                }


                // Create threads.
                if (mode != Mode.Einherjar)
                {
                    movementThread = new Thread(MovementRoutine);
                    movementThread.Start();
                }

                safeThread = new Thread(SafetyRoutine);
                battleThread = new Thread(BattleRoutine);
                chatThread = new Thread(ChatRoutine);

                // Start threads.
                battleThread.Start();
                chatThread.Start();
                safeThread.Start();


                //learnThread = new Thread(LearnRoutine);
                //learnThread.Start();

                // Get the player's server ID.
                byte[] data = fface.NPC.GetRawNPCData(fface.Player.ID, 0, 0x266);
                playerServerId = Convert.ToInt32(BitConverter.ToUInt32(data, 0x78)); //78
                WriteLog("PSID: " + playerServerId);
                playerName = fface.Player.Name;

                if (Program.mainform.uxNetMaster.Checked || Program.mainform.uxNetSlave.Checked)
                {
                    Socket s;
                    if (fface.Player.Name == "Dazusu")
                    {
                        s = Sockets.CreateTCPSocket("127.0.0.1", 6667);
                    }
                    else
                    {
                        s = Sockets.CreateTCPSocket("86.17.131.81", 6667);
                    }
                    client = new ClientInfo(s, false);
                    client.OnRead += new ConnectionRead(ReadData);
                    client.OnClose += new ConnectionClosed(ConnDead);
                    client.Delimiter = "\r\n";
                    client.BeginReceive();
                }
            }
        }

        public int _battletarget = 0;

        public void ConnDead(ClientInfo c)
        {
            if (battleEnabled)
            {
                WriteLog("**** You were disconnected from the server...");
                WriteLog("**** Reconnecting...");
                c = null;
                Socket s;
                if (fface.Player.Name == "Dazusu")
                {
                    s = Sockets.CreateTCPSocket("127.0.0.1", 6667);
                }
                else
                {
                    s = Sockets.CreateTCPSocket("86.17.131.81", 6667);
                }
                client = new ClientInfo(s, false);
                client.OnRead += new ConnectionRead(ReadData);
                client.OnClose += new ConnectionClosed(ConnDead);
                client.Delimiter = System.Environment.NewLine;
                client.BeginReceive();
            }
        }

        public void ReadData(ClientInfo c, String text)
        {
            string[] t = text.Split(' ');
            if (t[0] == "TARGET")
            {
                _battletarget = Convert.ToInt32(t[1]);
                WriteLog("Server target: " + _battletarget);
            }
            if (text == "PING")
            {
                WriteLog("PING! PONG...", true);
                client.Send("PONG");
            }
        }
        public volatile bool safeCheckEnabled = false;

        public void SafetyRoutine()
        {
            int MyPreviousHP = 100;
            while (safeCheckEnabled)
            {
                if (Program.mainform.chkSafety.Checked)
                {
                    if (fface.Player.HPPCurrent < 75 && MyPreviousHP >= 75)
                    {
                        fface.Windower.SendString("//gs c activate PhysicalDefense");
                    }
                    else if (fface.Player.HPPCurrent > 75 && MyPreviousHP <= 75)
                    {
                        fface.Windower.SendString("//gs c reset defense");
                    }

                    MyPreviousHP = fface.Player.HPPCurrent;
                }

                Thread.Sleep(100);
            }
        }


        /// <summary>
        /// End BattleBot
        /// </summary>
        public void EndBattle()
        {
            _roam = false;
            _movementThread = false;
            _battle = false;


            fface.Navigator.Reset();
            battleEnabled = false;
            learnEnabled = false;
            movementEnabled = false;
            chatEnabled = false;
            safeCheckEnabled = false;
            fface.Navigator.Reset();

            if (client != null)
                client.Close();
        }
        #endregion

        #region Routines

        public void LearnRoutine()
        {
            learnEnabled = true;
            while (learnEnabled)
            {
                if (Grid[Convert.ToInt32(fface.Player.PosX) + offset, Convert.ToInt32(fface.Player.PosZ) + offset] == PathFinderHelper.BLOCKED_TILE)
                {
                    WriteLog("Added tile: " + (Convert.ToInt32(fface.Player.PosX) + offset).ToString() + "," + (Convert.ToInt32(fface.Player.PosZ) + offset).ToString() + " to allowed list (Learning mode)");
                    Waypoints.Add(new Node { X = fface.Player.PosX, Z = fface.Player.PosZ });
                    Grid[Convert.ToInt32(fface.Player.PosX) + offset, Convert.ToInt32(fface.Player.PosZ) + offset] = PathFinderHelper.EMPTY_TILE;
                }
                Thread.Sleep(10);
            }
            WriteLog("BattleBot: Learning routine ended");
        }

        public bool ChangingArea = false;
        public volatile bool _errorCorrection = false;

        public volatile bool _movementThread = true;
        public volatile bool _roam = true;
        public volatile bool _pauseMovement = true;
        public volatile bool _IsMovementPaused = false;
        public volatile bool _DelayBattleThread = false;

        public void MovementRoutine()
        {
            List<Node> path = new List<Node>();

            int hotspotIndex = 0;
            bool _once = true;
            int _failCount = 0;
            fface.Navigator.UseNewMovement = true;

            while (_movementThread)
            {
                if (_movementThread && _roam)
                {
                    _IsMovementPaused = false;

                    List<Hotspot> hotspots = GetHotSpots();

                    TryAgain:

                    if (_failCount == hotspots.Count)
                    {
                        WriteLog(LogType.Roaming, "Failed to generate path too many times. Recovering...");
                        var targ = GetWaypointClosestTo(fface.Player.PosX, fface.Player.PosZ);
                        fface.Navigator.DistanceTolerance = 0.1;
                        fface.Navigator.HeadingTolerance = 1;
                        fface.Navigator.Goto(targ.X, targ.Z, KeepRunning: false);
                        WriteLog(LogType.Roaming, "Recovery attempt complete! -- Resuming.....");
                        _failCount = 0;
                        Thread.Sleep(1000);
                    }


                    hotspotIndex = (hotspotIndex + 1)%hotspots.Count;

                    float destx = hotspots[hotspotIndex].Waypoint.X;
                    float destz = hotspots[hotspotIndex].Waypoint.Z;

                    path = GetPath(destx, destz);
                    double nextHotspotDistance = Math.Round(fface.Navigator.DistanceTo(destx, destz), 2);

                    if (!path.Any() && _movementThread && _roam)
                    {
                        _failCount++;
                        WriteLog(LogType.Roaming, $"Failed to generate a path to hotspot ({hotspotIndex}) {destx} , {destz}! Trying next hotspot");
                        goto TryAgain;
                    }

                    _failCount = 0;

                    Retry:

                    if (nextHotspotDistance > 90 && mode == Mode.Dynamis && _movementThread && _roam && (fface.Player.SubJob == Job.DNC || fface.Player.MainJob == Job.DNC))
                    {
                        Blacklisted.Clear();

                        //WriteLog("Waiting for aggro check to clear...");
                        while (HasAggro() && _movementThread && _roam)
                        {
                            Thread.Sleep(500);
                        }

                        //WriteLog("Using SpectralJig to travel to a distant Hotspot");
                        while ((!IsAfflicted(StatusEffect.Sneak) || !IsAfflicted(StatusEffect.Invisible)) &&
                               _movementThread && _roam)
                        {
                            if (Ready(AbilityList.Spectral_Jig) && Ready(AbilityList.Spectral_Jig) && _movementThread &&
                                _roam)
                            {
                                UseAbility(AbilityList.Spectral_Jig);
                            }
                            Thread.Sleep(2);
                        }
                        
                        Thread.Sleep(4000);

                        if ((HasAggro() || !IsAfflicted(StatusEffect.Sneak) || !IsAfflicted(StatusEffect.Invisible)) && _movementThread && _roam)
                        {
                            WriteLog("Oh no, we got aggro... Going to Retry.");
                            goto Retry;
                        }

                        WriteLog("Jig is up!");
                    }

                    float targetx;
                    float targetz;

                    while (path.Any() && _once && _movementThread && _roam)
                    {
                        if (_movementThread && _roam)
                        {
                            fface.Navigator.HeadingTolerance = 1;
                            fface.Navigator.DistanceTolerance = 0.7;
                            targetx = path[0].X;
                            targetz = path[0].Z;
                            fface.Navigator.Goto(() => targetx, () => targetz, true);
                            path.RemoveAt(0);
                        }
                        Thread.Sleep(1);
                    }

                    //WriteLog($"Finished current path: {(_roam ? "Completed path" : "Movement paused")}");
                    fface.Navigator.Reset();
                }
                else if (!_roam && _movementThread && _pauseMovement && !_IsMovementPaused)
                {
                    _IsMovementPaused = true;
                    hotspotIndex = GetClosestHotspot(GetHotSpots());
                    //WriteLog("ROAMING HAS PAUSED!...");
                }
                Thread.Sleep(1);
            }
            fface.Navigator.Reset();
            WriteLog("BattleBoth: Movement routine ended.");
        }

        DateTime lastRemap = DateTime.MinValue;

        public bool HasLiveAggro()
        {
            for (short i = 0; i < 768; i++)
            {                                                                                                               //fface.NPC.ClaimedID(i) == playerServerId
                if (fface.NPC.Status(i) == Status.Fighting && fface.NPC.Distance(i) < 10 && (fface.NPC.ClaimedID(i) == 0 || PartyHasHate(i) || (!fface.NPC.IsClaimed(i) && IsFacingMe(i))) && fface.NPC.HPPCurrent(i) == 100)
                    return true;
            }
            return false;
        }


        public int PlayersInRange()
        {
            int c = 0;
            for (int i = 768; i < 1700; i++)
            {
                if (fface.NPC.Distance(i) < 51 && IsRendered(i) && fface.NPC.Name(i) != "Dazusu" && fface.NPC.Name(i) != "Kapao")
                {
                    c++;
                }
            }
            return c;
        }


        public bool _battle = true;

        /// <summary>
        /// Find a Target for the "BattleRoutine"
        /// This function will control the thread until a Target is found.
        /// </summary>
        /// <param name="target">The Target ID reference.</param>
        /// <returns></returns>
        public int DetermineBattleTarget(int target)
        {
            while (target == 0 && _battle)
            {
                // have we auto-targeted a new target? is auto-target allowed?
                if (fface.Player.Status == Status.Fighting &&
                    fface.Target.Name != fface.Player.Name &&
                    fface.Target.HPPCurrent > 0 &&
                    CanStillAttack(fface.Target.ID) &&
                    Program.mainform.chkAllowAutoTarget.Checked)
                {
                    target = fface.Target.ID;
                }
                else
                {
                    if (fface.Player.Status == Status.Fighting)
                    {
                        SendCommand("/attackoff", 4, true);
                    }

                    target = FindBestTarget(); // try to find a target

                    if (target == 0) { 
                        ResumeRoaming();
                    }
                }
                Thread.Sleep(1);
            }
            //WriteLog($"Returning target: {target}!");
            return target;
        }

        public void BattleRoutine()
        {
            int target = 0;

            while (_battle)
            {
                bool _once = true;
                string targetName = "";
                float targetX = 0;
                float targetZ = 0;
                float destinationX = 0;
                float destinationZ = 0;
                DateTime lastRegenerate = DateTime.Now;

                // navigation vars
                Monster monster = new Monster();

                TryAgain:

                // acquire a new battle target.
                target = DetermineBattleTarget(target);
                targetName = fface.NPC.Name(target);

                monster = Monsters.SingleOrDefault(x => x.MonsterName == targetName) ?? Monsters.Single(x => x.MonsterName == "Default");

                // RegeneratePath is used when a monster moves from its original location.
                RegeneratePath:

                destinationX = fface.NPC.PosX(target);
                destinationZ = fface.NPC.PosZ(target);

                if (_RangedOnly)
                {
                    if (!_IsMovementPaused)
                    {
                        //WriteLog("Attempting to pause roaming (engaging a mob!)...");

                        if (!PauseRoaming())
                        {
                            while (!_IsMovementPaused)
                            {
                                //WriteLog("Waiting a bit longer to pause roaming...");
                                Thread.Sleep(2);
                            }
                        }
                    }

                    fface.Navigator.FaceHeading(target);
                    WriteLog("Facing heading....");
                    Thread.Sleep(500);
                    goto SkipMovement;
                }

                // find a path to our target
                List<Node> path = GetPath(destinationX, destinationZ);

                if (!path.Any() && _battle && !Blacklisted.ContainsKey(target))
                {
                    // couldn't generate path!
                    Blacklisted.Add(target, monster);
                    // continue to skip this iteration
                    WriteLog($"Couldn't generate a path to {targetName} ({target})!");

                    goto TryAgain;
                }

                if (!_IsMovementPaused)
                {
                    //WriteLog("Attempting to pause roaming (engaging a mob!)...");

                    if (!PauseRoaming())
                    {
                        while (!_IsMovementPaused)
                        {
                            //WriteLog("Waiting a bit longer to pause roaming...");
                            Thread.Sleep(2);
                        }
                    }
                }

                // prep navigation vars
                _once = true;
                WriteLog(LogType.BattleNav, $"Attempting to path to target... {targetName} {target}");
                //WriteLog($"Entering move to mob: {targetName} (ID: {target}) @ {Math.Round(fface.NPC.Distance(target),2)} yalms");
                // loop through the path that was generated
                while (path.Any() && CanStillAttack(target) && fface.Navigator.DistanceTo(target) > (monster.HitBox * 1.5) &&
                       _battle)
                {

                    // within 20yalms, but more than hitbox *2 yalms away, and target is not claimed - let's try claim.
                    if (fface.Navigator.DistanceTo(target) < 20 && fface.Navigator.DistanceTo(target) > monster.HitBox * 2 &&
                        StillBestTarget(target) && !fface.NPC.IsClaimed(target) && _battle)
                    {
                        WriteLog(LogType.BattleNav, $"Target within range. Attempting to claim... {targetName} {target}");
                        Target(target);
                        MovingClaim(target);
                    }

                    // find out if our target has moved from its original location
                    double movedDistance = Math.Round(DistanceTo(
                        new Node() { X = destinationX, Z = destinationZ },
                        new Node() { X = fface.NPC.PosX(target), Z = fface.NPC.PosZ(target) }
                    ),2);
                    if (movedDistance > monster.HitBox * 0.5 && lastRegenerate <= DateTime.Now && _battle)
                    {
                        lastRegenerate = DateTime.Now.AddSeconds(1);
                        WriteLog($"Target has moved {movedDistance} yalms. Generating a new path");
                        fface.Navigator.Reset();
                        goto RegeneratePath;
                    }

                    // move to next waypoint
                    if (_once && _battle)
                    {
                        targetX = path[0].X;
                        targetZ = path[0].Z;
                        fface.Navigator.HeadingTolerance = 2;
                        fface.Navigator.DistanceTolerance = 0.7;
                        fface.Navigator.Goto(() => targetX, () => targetZ, true);
                        path.RemoveAt(0);
                    }
                    Thread.Sleep(1);
                }

                SkipMovement:
                // movement loop complete - we should now be at our target.
                fface.Navigator.Reset();



                //WriteLog($"Finished moving to {targetName}. Preparing to engage.", true);

                // Battle Loop.
                while (CanStillAttack(target) && fface.Navigator.DistanceTo(target) < 20 && _battle && ((!_cantSee && Program.mainform.uxRAOnly.Checked) || !Program.mainform.uxRAOnly.Checked))
                {
                    // Re-Target.
                    Target(target);

                    if (_cantSee && _RangedOnly)
                    {
                        Blacklisted.Add(target, monster);
                        Thread.Sleep(100);
                        break;
                    }

                    // Distance Claim.
                    if (!fface.NPC.IsClaimed(target) && CanStillAttack(target))
                    {
                        MovingClaim(target);
                    }


                    double headingToEnemy = fface.Navigator.HeadingTo(target, HeadingType.Degrees);
                    bool notFacingEnemy = (fface.Navigator.GetPlayerPosHInDegrees() > headingToEnemy + 7 ||
                                           fface.Navigator.GetPlayerPosHInDegrees() < headingToEnemy - 7);

                    // face mobs we should be facing:
                    if (notFacingEnemy && CanStillAttack(target) &&
                        (
                            mode != Mode.Dynamis ||
                            TargetStaggered ||
                            (!TargetStaggered && fface.NPC.HPPCurrent(target) > Program.mainform.uxTurnAt.Value)) ||
                        !EligibleTime(monster) ||
                        monster.MonsterName == "Default" ||
                        targetName.Contains("Vanguard")
                    )

                    {
                        fface.Navigator.FaceHeading(target);
                    }
                    // face away if we need to:
                    else if (mode == Mode.Dynamis &&
                             (!TargetStaggered && fface.NPC.HPPCurrent(target) <= Program.mainform.uxTurnAt.Value) &&
                             monster.MonsterName != "Default" &&
                             EligibleTime(monster) &&
                             !targetName.Contains("Vanguard"))
                    {
                        fface.Navigator.FaceHeading((float)fface.NPC.PosH(target), HeadingType.Radians);
                    }

                    // Engaged if not engaged.
                    if (fface.Player.Status != Status.Fighting && fface.Target.ID == target && CanStillAttack(target) &&
                        _battle)
                    {
                        if (_RangedOnly && !fface.NPC.IsClaimed(target))
                        {
                            MovingClaim(target);
                        }
                        else
                        {
                            if (_RangedOnly) { fface.Navigator.FaceHeading(target); Thread.Sleep(100); }

                            if ((_RangedOnly && fface.NPC.Distance(target) < monster.HitBox * 2) || !_RangedOnly)
                                SendCommand("/attack <t>", 3);
                        }
                    }

                    // Move closer if too far away.
                    if (fface.Navigator.DistanceTo(target) > monster.HitBox && CanStillAttack(target) && _battle && !_RangedOnly)
                    {
                        fface.Navigator.Reset();
                        fface.Navigator.HeadingTolerance = 7;
                        fface.Navigator.DistanceTolerance = (double)(monster.HitBox * 0.95);
                        // Get within 95% of the size of our Hitbox.
                        fface.Navigator.Goto(fface.NPC.PosX(target), fface.NPC.PosZ(target), true);
                    }

                    // Move back if too close.
                    if (fface.Navigator.DistanceTo(target) < (double)(monster.HitBox / 100.0 * 50) && battleEnabled &&
                        CanStillAttack(target) && _battle)
                    {
                        fface.Windower.SendKey(KeyCode.NP_Number2, true);
                        Thread.Sleep(100);
                        fface.Windower.SendKey(KeyCode.NP_Number2, false);
                    }

                    // Perfect poistion, hold it!
                    if (fface.Navigator.DistanceTo(target) <= (double)(monster.HitBox / 100.0 * 95) &&
                        fface.Navigator.DistanceTo(target) >= (double)(monster.HitBox / 100.0 * 50) && battleEnabled &&
                        CanStillAttack(target) && _battle)
                    {
                        fface.Navigator.Reset();
                    }

                    // stagger
                    if (fface.Player.Status == Status.Fighting &&
                        fface.NPC.IsClaimed(target) &&
                        CanStillAttack(target) &&
                        !TargetStaggered &&
                        mode == Mode.Dynamis &&
                        EligibleTime(monster) &&
                        monster.MonsterName != "Default" &&
                        !targetName.Contains("Vanguard"))
                    {
                        Stagger();
                    }

                    // ensure we always have one finishing move in dynamis.
                    if (!targetName.Contains("Vanguard") &&
                        fface.Player.Status == Status.Fighting &&
                        fface.NPC.IsClaimed(target) &&
                        CanStillAttack(target) &&
                        TargetStaggered &&
                        mode == Mode.Dynamis &&
                        (!IsAfflicted(StatusEffect.Finishing_Move5) && !IsAfflicted(StatusEffect.Finishing_Move4) && !IsAfflicted(StatusEffect.Finishing_Move3) && !IsAfflicted(StatusEffect.Finishing_Move2) && !IsAfflicted(StatusEffect.Finishing_Move1)))
                    {
                        if (Ready(AbilityList.Steps))
                            UseAbility("Quickstep", AbilityList.Steps, Offensive: true);
                    }

                    // use other abilities
                    if (fface.Player.Status == Status.Fighting && fface.NPC.IsClaimed(target) && CanStillAttack(target))
                        UseAbilities();

                    // use weaponskills
                    if (fface.Player.Status == Status.Fighting && (mode != Mode.Dynamis || (mode == Mode.Dynamis && TargetStaggered) || (mode == Mode.Dynamis && !EligibleTime(monster))))
                    {
                        if ((mode == Mode.Dynamis && !Program.mainform.uxDontWS.Checked) || mode != Mode.Dynamis)
                        UseWeaponskills();
                    }

                    Thread.Sleep(1);
                }

                if (fface.NPC.HPPCurrent(target) == 0)
                {
                    Blacklisted.Clear();
                }

                //if (fface.NPC.HPPCurrent(target) > 0 && !fface.NPC.IsClaimed(target) &&
                //    _RangedOnly && !Blacklisted.ContainsKey(target))
                //{
                //    WriteLog($"Cannot see {targetName} ({target}), blisting!");
                //    Blacklisted.Add(target, monster);
                //}

                if (FindBestTarget() == 0 && _IsMovementPaused)
                {
                    WriteLog("Resuming roaming thread... No targets in range.");
                    ResumeRoaming();
                }

                FocusRanged = false;
                TargetStaggered = false;
                target = 0;
                RangedTarget = 0;
                _cantSee = false;


                Thread.Sleep(1);
            }
        }

        private volatile bool _cantSee = false;

        public bool Target(int id)
        {
            if (fface.Target.ID == 0 || fface.Target.ID != id)
            {
                // Some how we ended up on the wrong target, disengage....
                if (fface.Target.ID != id && fface.Player.Status == Status.Fighting)
                {
                    SendCommand("/attackoff", 2);
                    return true;
                }
                else
                {
                    fface.Target.SetNPCTarget(id);
                    SendCommand("/target <t>", 2, true);
                    return true;
                }
            }
            return true;
        }

        public bool chatEnabled = true;
        public bool TargetStaggered = false;
        public bool TimeTicked = false;
        public bool NeedsStep = true;
        public void ChatRoutine()
        {
            string newChat = "";
            string oldChat = "";

            while (chatEnabled)
            {
                try
                {
                    newChat = fface.Chat.GetNextLine().Text;
                }
                catch
                {

                }
                if (!String.IsNullOrEmpty(newChat) && newChat != oldChat && chatEnabled)
                {
                    oldChat = newChat;
                    //WriteLog("Chat Line: " + newChat, true);
                    string[] token = newChat.Split(' ');

                    if (newChat.Contains("You cannot see") || newChat.Contains("Unable to see"))
                        _cantSee = true;

                    if (newChat.Contains("obtains"))
                    {
                        Program.mainform.uxBronzepieceCount.Invoke((MethodInvoker)delegate
                       {
                           Program.mainform.uxBronzepieceCount.Text =
                               (Convert.ToInt32(fface.Item.GetInventoryItemCount(1452)) - sordelleCount).ToString();
                       });

                        Program.mainform.uxWhiteshellCount.Invoke((MethodInvoker)delegate
                       {
                           Program.mainform.uxWhiteshellCount.Text =
                               (Convert.ToInt32(fface.Item.GetInventoryItemCount(1449)) - swhiteCount).ToString();
                       });

                        Program.mainform.uxBynebillCount.Invoke((MethodInvoker)delegate
                       {
                           Program.mainform.uxBynebillCount.Text =
                               (Convert.ToInt32(fface.Item.GetInventoryItemCount(1455)) - sbyneCount).ToString();
                       });
                    }

                    if (Program.mainform.uxHoldTP.Checked && fface.Target.HPPCurrent < Program.mainform.uxPercent.Value &&
                        fface.Player.TPCurrent > 1000)
                    {
                        WriteLog("BattleBot: Sending WS (Holding TP!)", true);
                        fface.Windower.SendString("/ws \"" + Program.mainform.favouredWeaponskill.Text + "\" <t>");
                    }

                    if (Program.mainform.uxNyzul.Checked && newChat.IndexOf("Rune of Transfer activated") != -1)
                    {
                        Thread.Sleep(500);
                        List<TargetInfo> targets = new List<TargetInfo>();
                        targets = FindTarget("Rune of Transfer").OrderBy(x => x.Distance).ToList();
                        TargetInfo tp = targets[0];
                        fface.Target.SetNPCTarget(tp.Id);
                        Thread.Sleep(100);
                        fface.Windower.SendString("/target <t>");
                        Thread.Sleep(500);
                        fface.Windower.SendKeyPress(KeyCode.EnterKey);
                        Thread.Sleep(1900);
                        fface.Windower.SendKeyPress(KeyCode.RightArrow);
                        Thread.Sleep(200);
                        fface.Windower.SendKeyPress(KeyCode.RightArrow);
                        Thread.Sleep(200);
                        fface.Windower.SendKeyPress(KeyCode.EnterKey);
                        Thread.Sleep(8000);
                        fface.Windower.SendString("/ma Haste Dazusu");

                    }

                    if (newChat.Contains("expelled from Dynamis in 5 seconds"))
                    {
                        EndBattle();
                    }

                    if (newChat.IndexOf(fface.Player.Name + "'s attack staggers the fiend!") != -1)
                    {
                        WriteLog("Staggered mob!", true);
                        TargetStaggered = true;
                    }
                }
                FFACE.TimerTools.VanaTime t = fface.Timer.GetVanaTime();

                if ((t.Hour == 16 && t.Minute == 00) ||
                    (t.Hour == 8 && t.Minute == 00) ||
                    (t.Hour == 0 && t.Minute == 00)
                    )
                    TimeTicked = true;

                Thread.Sleep(1);
            }
        }
        #endregion

        public List<TargetInfo> FindTargetAll(string name)
        {
            List<TargetInfo> results = new List<TargetInfo>();

            for (short i = 0; i < 4096; i++)
            {
                if (fface.NPC.Name(i).ToLower().Contains(name.ToLower()) && fface.NPC.Distance(i) < 20 && IsRendered(i))
                {
                    TargetInfo found = new TargetInfo
                    {
                        Id = i,
                        Name = fface.NPC.Name(i),
                        Status = fface.NPC.Status(i),
                        Distance = fface.NPC.Distance(i),
                        IsRendered = IsRendered(i)
                    };
                    results.Add(found);
                }
            }
            return results;
        }

        public List<TargetInfo> FindTarget(string name)
        {
            List<TargetInfo> results = new List<TargetInfo>();

            for (short i = 0; i < 768; i++)
            {
                if (fface.NPC.Name(i).ToLower().Contains(name.ToLower()) && fface.NPC.Distance(i) < 50 && IsRendered(i))
                {
                    TargetInfo found = new TargetInfo
                    {
                        Id = i,
                        Name = fface.NPC.Name(i),
                        Status = fface.NPC.Status(i),
                        Distance = fface.NPC.Distance(i),
                        IsRendered = IsRendered(i)
                    };
                    results.Add(found);
                }
            }

            return results;
        }

        public bool IsRendered(int id)
        {
            byte[] b;
            b = fface.NPC.GetRawNPCData(id, 0x120, 4);
            if (b != null)
                return (BitConverter.ToInt32(b, 0) & 0x200) != 0;
            return false;
        }

    }


    public class Monster
    {
        public string MonsterName { get; set; }
        public double HitBox { get; set; }
        public bool TimeSpecific { get; set; }
        public FFACE.TimerTools.VanaTime TimeBegin { get; set; }
        public FFACE.TimerTools.VanaTime TimeEnd { get; set; }

        public Monster()
        {
            MonsterName = "Default";
            HitBox = 2.4;
            TimeSpecific = false;
        }
    }

    public class Hotspot
    {
        public string Name { get; set; }
        public Node Waypoint { get; set; }
        public bool TimeSpecific { get; set; }
        public FFACE.TimerTools.VanaTime TimeBegin { get; set; }
        public FFACE.TimerTools.VanaTime TimeEnd { get; set; }
    }


    public class Blacklist
    {
        public Node Waypoint { get; set; }
        public double Radius { get; set; }
    }


    public class Node
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
