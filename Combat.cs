using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeenGames.Utils.AStarPathFinder;
using FFACETools;
using Flipper.Classes;
using FlipperD;

namespace Flipper
{
    public static class Combat
    {
        private static IJob job = new Jobs();
        private static FFACE fface;
        private static bool _fighting = true;

        public static FFACE SetInstance
        {
            set { fface = value; }
        }

        public static IJob SetJob
        {
            set { job = value; }
        }


        public static FailType GetFailType { get; } = FailType.NoFail;


        public enum FailType
        {
            NoFail,
            OutOfRange,
            OutClaimed,
            CantSee,
            NoPath
        }

        public enum Mode
        {
            None,
            StrictPathing,
            Meshing
        }


        /// <summary>
        /// Finds the best target to engage based on the passed criteria.
        /// </summary>
        /// <param name="monsterName">An array of monster names that we're looking to engage</param>
        /// <returns></returns>
        public static int FindTarget(double maxDistance = 50.0, params string[] monsterName)
        {
            int bestTarget = 0;
            double bestDistance = maxDistance;

            for (short i = 0; i < 768; i++)
            {
                if (Black.Contains(i))
                    continue;

                if (!IsRendered(i))
                    continue;

                if (fface.NPC.IsClaimed(i) && !PartyHasHate(i))
                    continue;

                if (fface.NPC.Distance(i) < 7 && fface.NPC.Status(i) == Status.Fighting &&
                    (!fface.NPC.IsClaimed(i) || (fface.NPC.Status(i) == Status.Fighting && !fface.NPC.IsClaimed(i)) || PartyHasHate(i)) && IsFacingMe(i))
                {
                    return i;
                }

                if (fface.NPC.HPPCurrent(i) == 100 && monsterName.Contains(fface.NPC.Name(i)))
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

        public static List<TargetInfo> FindTarget(string name)
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

        /// <summary>
        /// Processes the routine of engaging & fighting an enemy.
        /// </summary>
        /// <param name="target">The ID of the target to engage.</param>
        /// <param name="monster">The monster object for the monster you're engaging</param>
        /// <param name="mode">Special mode considerations for engaging this target.</param>
        /// <param name="fail"></param>
        /// <returns></returns>
        public static bool Fight(int target, Monster monster, Mode mode, out FailType fail)
        {
            fail = FailType.NoFail;
            _fighting = true;
            float targetX = 0;
            float targetZ = 0;

            fface.Navigator.Reset();

            if (mode == Mode.Meshing)
            {
                List<Node> path = new List<Node>();
                float destinationX = fface.NPC.PosX(target);
                float destinationZ = fface.NPC.PosZ(target);

                path = GetPath(destinationX, destinationZ);

                if (!path.Any())
                {
                    fail = FailType.NoPath;
                    return false;
                }


                while (path.Any() && CanStillAttack(target) && DistanceTo(target) > (monster.HitBox*1.5) && _fighting)
                {
                    if (!fface.NPC.IsClaimed(target))
                    {
                        Target(target);
                        job.UseClaim();
                    }
                    if (_fighting)
                    {
                        targetX = path[0].X;
                        targetZ = path[0].Z;
                        fface.Navigator.HeadingTolerance = 2;
                        fface.Navigator.DistanceTolerance = 0.7;
                        path.RemoveAt(0);
                        fface.Navigator.Goto(() => targetX, () => targetZ, false);
                    }
                    Thread.Sleep(1);
                }
                fface.Navigator.Reset();
            }

            // battle routine
            while (CanStillAttack(target) && (DistanceTo(target) < 20 || fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion) && _fighting)
            {
                // TARGET
                Target(target);

                // FACE TARGET
                fface.Navigator.FaceHeading(target);

                // CLAIM
                if (!fface.NPC.IsClaimed(target) && _fighting)
                {
                    switch (mode)
                    {
                        case Mode.StrictPathing:
                        {
                            // We don't want to wonder too far from our strict path. Use Strict Pathing.
                            if (DistanceTo(target) >= monster.HitBox*1.5)
                                fface.Navigator.Reset();
                            Thread.Sleep(500);
                            job.UseRangedClaim();
                            break;
                        }
                        case Mode.Meshing:
                        case Mode.None:
                        {
                            job.UseClaim();
                            break;
                        }
                    }
                }

                // ENGAGE

                if (((fface.NPC.IsClaimed(target) && PartyHasHate(target)) ||
                    (DistanceTo(target) < monster.HitBox * 1.5 && !fface.NPC.IsClaimed(target)))
                    && fface.Player.Status != Status.Fighting && fface.Target.ID == target && _fighting)
                {
                    // IF ('TARGET IS CLAIMED && PARTY HAS HATE'
                    // OR 'DISTANCE TO TARGET < 5 && TARGET IS NOT CLAIMED') AND
                    //    PLAYER IS NOT ENGAGED && TARGET ID == TARGET && _FIGHTING
                    job.Engage();
                }

                // MOVE CLOSER

                if (DistanceTo(target) > monster.HitBox && CanStillAttack(target) && _fighting)
                {
                    switch (mode)
                    {
                        case Mode.StrictPathing:
                            {
                                // if we're strict pathing, let the mob get close to us before we move to it.
                                if (DistanceTo(target) < monster.HitBox * 1.25)
                                {
                                    fface.Windower.SendString("/echo Close enough, moving in...");
                                    fface.Navigator.Reset();
                                    fface.Navigator.HeadingTolerance = 7;
                                    fface.Navigator.DistanceTolerance = (double)(monster.HitBox * 0.95);
                                    fface.Navigator.Goto(fface.NPC.PosX(target), fface.NPC.PosZ(target), false);
                                }
                                break;
                            }
                        case Mode.Meshing:
                        case Mode.None:
                            {
                                fface.Navigator.Reset();
                                fface.Navigator.DistanceTolerance = (double)(monster.HitBox * 0.95);
                                fface.Navigator.Goto(fface.NPC.PosX(target), fface.NPC.PosZ(target), false);
                                break;
                            }
                    }
                }

                // MOVE BACK
                if (DistanceTo(target) < (monster.HitBox * 0.65) && CanStillAttack(target) && _fighting)
                {
                    switch (mode)
                    {
                        default:
                            {
                                fface.Windower.SendKey(KeyCode.NP_Number2, true);
                                Thread.Sleep(50);
                                fface.Windower.SendKey(KeyCode.NP_Number2, false);
                                break;
                            }
                    }
                }

                // PLAYER STUFF
                if (fface.Player.Status == Status.Fighting && _fighting)
                {
                    job.UseHeals();

                    job.UseAbilities();

                    if (fface.Player.TPCurrent >= 1000)
                        job.UseWeaponskills();

                    job.UseSpells();
                }

                Thread.Sleep(1);
            }
            Thread.Sleep(500);
            if (!_fighting && fface.Player.Status == Status.Fighting)
            {
                fface.Windower.SendString("/attackoff");
                Thread.Sleep(3000);
                if (fface.Target.IsLocked)
                {
                    fface.Windower.SendString("/lockon");
                    Thread.Sleep(500);
                }
            }

            if (!_fighting || fface.NPC.HPPCurrent(target) != 0) return false;

            _fighting = false;

            Black.Clear();
            return true;
        }

        static Combat()
        {
            if (Black == null)
            {
                WriteLog("Initializing Blacklist!");
                Black = new List<int>();
            }
        }

        public static List<int> Black { get; set; }

        public static void AddBlacklist(int id)
        {
            Black.Add(id);
        }

        public static void Interrupt()
        {
            _fighting = false;
        }


        #region Helper Methods

        public static double DistanceTo(int id)
        {
            return fface.Navigator.DistanceTo(id);
        }

        /// <summary>
        /// Target's the specifid target. Disengages target if it does not match the passed ID.
        /// </summary>
        /// <param name="id">The ID of the intended target.</param>
        /// <returns></returns>
        public static void Target(int id)
        {
            if (fface.Target.ID == 0 || fface.Target.ID != id || string.IsNullOrEmpty(fface.Target.Name))
            {
                // Some how we ended up on the wrong target, disengage....
                if (fface.Target.ID != id && fface.Player.Status == Status.Fighting)
                {
                    fface.Windower.SendString("/attackoff");
                    Thread.Sleep(2000);
                }
                else
                {
                    fface.Target.SetNPCTarget(id);
                    Thread.Sleep(50);
                    fface.Windower.SendString("/target <t>");
                    Thread.Sleep(50);
                }
            }
        }


        /// <summary>
        /// Determine if we can still attack our intended target.
        /// </summary>
        /// <param name="id">The ID of the target.</param>
        /// <returns></returns>
        public static bool CanStillAttack(int id)
        {

            if (!IsRendered(id))
            {
                WriteLog("[CANT ATTACK] Monster is not rendered.");
                return false;
            }

            if (fface.NPC.IsClaimed(id) && !PartyHasHate(id) && fface.Player.Status != Status.Fighting)
            {
                WriteLog("[CANT ATTACK] Monster is claimed, and Party doesn't have hate.");
                return false;
            }

            // Skip if the mob more than 5 yalms above or below us
            if (Math.Abs(Math.Abs(fface.NPC.PosY(id)) - Math.Abs(fface.Player.PosY)) > 15)
            {
                WriteLog("[CANT ATTACK] Mob is too far below/above me!");
                return false;
            }

            // Skip if the NPC's HP is 0
            if (fface.NPC.HPPCurrent(id) == 0 || !IsRendered(id))
            {
                WriteLog("[CANT ATTACK] Mobs HP is 0, or it's not rendered.");
                WriteLog("[CANT ATTACK] Mobs HP is 0, or it's not rendered.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determine if the specified NPC is facing me.
        /// </summary>
        /// <param name="id">The ID of the target.</param>
        /// <returns></returns>
        public static bool IsFacingMe(int id)
        {
            double targetHeading = RadianToDegree(fface.NPC.PosH(id));
            double lineAngle = GetAngleOfLineBetweenTwoPoints(new PointF { X = fface.Player.PosX, Y = fface.Player.PosZ }, new PointF { X = fface.NPC.PosX(id), Y = fface.NPC.PosZ(id) });
            double difference = (targetHeading + lineAngle) - 180;
            return difference < 3.5 && difference > -3.5;
        }


        /// <summary>
        /// Determine if a player or NPC in your party currently has hate on the intended target.
        /// </summary>
        /// <param name="id">The ID of the target.</param>
        /// <returns></returns>
        public static bool PartyHasHate(int id)
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


        /// <summary>
        /// Determine if the target is rendered on screen.
        /// </summary>
        /// <param name="id">The ID of the target.</param>
        /// <returns></returns>
        public static bool IsRendered(int id)
        {
            byte[] b = fface.NPC.GetRawNPCData(id, 0x120, 4);
            if (b != null)
                return (BitConverter.ToInt32(b, 0) & 0x200) != 0;
            return false;
        }


        /// <summary>
        /// Convert radian to degree
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }


        /// <summary>
        /// Get Angle of Line between two points.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <returns>The angle between the two points.</returns>
        private static double GetAngleOfLineBetweenTwoPoints(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }
        #endregion

        public static string lastLog = "";

        public static void WriteLog(string log, bool verbose = false)
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


        #region Navigation
        public static List<Node> Waypoints = new List<Node>();
        public static List<Blacklist> Blacklists = new List<Blacklist>();
        public static byte[,] Grid { get; set; }
        public static int offset = 2000;
        private static PathFinderFast mesh;
        public static List<Hotspot> Hotspots = new List<Hotspot>();

        public static List<Hotspot> GetHotspots()
        {
            return Hotspots;
        }


        public static bool LoadBlacklist(Zone zone)
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
        /// Loads coordinates from a file.
        /// </summary>
        /// <returns></returns>
        public static bool LoadCoords(Zone zone)
        {
            List<string> files = new List<string>();

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

            Waypoints.Clear();
            string loadFile = AppDomain.CurrentDomain.BaseDirectory + @"\assets\global.mesh";
            string loadFileM = AppDomain.CurrentDomain.BaseDirectory + @"\assets\" + (int)zone + ".mesh";
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
                        if (Convert.ToInt32(token[1]) == (int)zone)
                        {
                            bool inblackspot = false;
                            foreach (Blacklist b in Blacklists)
                            {
                                if (InCircle((int)b.Waypoint.X, (int)b.Waypoint.Z, Convert.ToInt32(token[2]),
                                    Convert.ToInt32(token[4]), Convert.ToInt32(b.Radius)))
                                {
                                    inblackspot = true;
                                    caught++;
                                    WriteLog("In Blackpost (Ignoring!!): " + token[2] + " / " + token[4]);
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
                                Grid[int.Parse((token[2]), CultureInfo.InvariantCulture) + offset,
                                     int.Parse((token[4]), CultureInfo.InvariantCulture) + offset] = PathFinderHelper.EMPTY_TILE;
                            }
                        }
                    }
                    WriteLog($"Excluded {caught} nodes, due to being in blackspots.");
                    WriteLog($"Loaded {Waypoints.Count} ({f.Split(Convert.ToChar(@"\"))[f.Split(Convert.ToChar(@"\")).Count() - 1].Replace(".mesh", "")}) nodes.");
                    file.Close();
                }
            }
            return false;
        }


        public static bool LoadHotspots(Zone zone)
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



        public static List<Node> GetPath(float X, float Z)
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

            WriteLog($"Generating a path to {X} , {Z}");
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
                WriteLog($"Generation succeeded! {path.Count()} nodes were assembled");
                foreach (PathFinderNode point in path)
                {
                    ReturnPath.Add(new Node { X = point.X - offset, Z = point.Y - offset });
                }
            }
            else
            {
                WriteLog($"Pathing failed (No route): {Convert.ToInt32(fface.Player.PosX)},{Convert.ToInt32(fface.Player.PosZ)} to {X},{Z}!");
            }
            ReturnPath = SmoothNodes(ReturnPath);

            ReturnPath.Reverse();

            return ReturnPath;
        }

        /// <summary>
        /// Smoothes nodes in a list.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static List<Node> SmoothNodes(List<Node> nodes)
        {
            WriteLog($"Smoothing nodes for a path with {nodes.Count()} nodes.");
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


        /// <summary>
        /// Checks of a coordinate exists in a circle.
        /// </summary>
        /// <returns></returns>
        public static bool InCircle(int centerx, int centery, int x, int y, int radius = 4)
        {
            var dist = Math.Sqrt(Math.Pow((centerx - x), 2) + Math.Pow((centery - y), 2));
            return dist < radius;
        }



        public class Blacklist
        {
            public Node Waypoint { get; set; }
            public double Radius { get; set; }
        }


        #endregion
    }

    public class Hotspot
    {
        public string Name { get; set; }
        public Node Waypoint { get; set; }
        public bool TimeSpecific { get; set; }
        public FFACE.TimerTools.VanaTime TimeBegin { get; set; }
        public FFACE.TimerTools.VanaTime TimeEnd { get; set; }
    }

    public class TargetInfo
    {
        public int Id;
        public string Name;
        public FFACETools.Status Status;
        public double Distance;
        public bool IsRendered;
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
}
