using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            CantSee
        }

        public enum Mode
        {
            None,
            StrictPathing
        }


        /// <summary>
        /// Finds the best target to engage based on the passed criteria.
        /// </summary>
        /// <param name="monsterName">An array of monster names that we're looking to engage</param>
        /// <returns></returns>
        public static int FindTarget(double maxDistance = 50.0, params string[] monsterName)
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
                    (!fface.NPC.IsClaimed(i) || PartyHasHate(i)) && IsFacingMe(i))
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
        /// <returns></returns>
        public static bool Fight(int target, Monster monster, Mode mode = Mode.None, double maxDistance = 20.0)
        {
            _fighting = true;
            fface.Navigator.Reset();

            while (CanStillAttack(target) && DistanceTo(target) < maxDistance &&  _fighting)
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
                            if (DistanceTo(target) >= monster.HitBox * 1.5)
                                job.UseRangedClaim();
                            break;
                        }
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
                            if (DistanceTo(target) < monster.HitBox*2)
                            {
                                fface.Windower.SendString("/echo Close enough, moving in...");
                                fface.Navigator.Reset();
                                fface.Navigator.HeadingTolerance = 7;
                                fface.Navigator.DistanceTolerance = (double) (monster.HitBox*0.95);
                                fface.Navigator.Goto(fface.NPC.PosX(target), fface.NPC.PosZ(target), false);
                            }
                            break;
                        }
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
                if (DistanceTo(target) < (monster.HitBox * 0.50) && CanStillAttack(target) && _fighting)
                {
                    switch (mode)
                    {
                        default:
                        {
                            fface.Windower.SendKey(KeyCode.NP_Number2, true);
                            Thread.Sleep(75);
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

            if (!_fighting) return false;

            _fighting = false;
            return true;
        }

        static Combat()
        {
            
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
                return false;
            }

            if (fface.NPC.IsClaimed(id) && !PartyHasHate(id) && !IsFacingMe(id)) 
            {
                return false;
            }

            // Skip if the mob more than 5 yalms above or below us
            if (Math.Abs(Math.Abs(fface.NPC.PosY(id)) - Math.Abs(fface.Player.PosY)) > 5)
            {
                return false;
            }

            // Skip if the NPC's HP is 0
            if (fface.NPC.HPPCurrent(id) == 0 || !IsRendered(id))
            {
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


    }
}
