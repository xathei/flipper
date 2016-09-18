using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    class Geomancer : Jobs
    {

        public Geomancer(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            Melee = false;
        }

        public override int MaxDistance()
        {
            if (_fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
            {
                return 50;
            }
            else
            {
                return 20;
            }
        }

        public override void UseClaim()
        {
            return;
        }

        public override void UseRangedClaim()
        {
            return;
        }

        public override void Engage()
        {
            if (!_fface.Target.IsLocked) 
                _fface.Windower.SendString("/lockon");
            return;
        }

        public override bool CanStillAttack(int id)
        {
            if (!IsRendered(id))
                return false;

            if (_fface.NPC.HPPCurrent(id) == 0)
                return false;

            if (Math.Abs(Math.Abs(_fface.NPC.PosY(id)) - Math.Abs(_fface.Player.PosY)) > 15)
            {
                return false;
            }

            if (_fface.NPC.Status(id) == Status.Dead1 ||
                _fface.NPC.Status(id) == Status.Dead2)
                return false;

            return true;
        }

        public override void Position(int id, Monster monster)
        {
            if (DistanceTo(id) > monster.HitBox && CanStillAttack(id))
            {
                _fface.Navigator.Reset();
                _fface.Navigator.DistanceTolerance = 4;
                _fface.Navigator.Goto(_fface.NPC.PosX(id), _fface.NPC.PosZ(id), false);

            }

            if (_fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
            {
                var difference = GetSADifference(id);
                while (difference < 66 || difference > 89)
                {
                    _fface.Windower.SendKey(KeyCode.NP_Number6, true);
                    Thread.Sleep(100);
                    _fface.Windower.SendKey(KeyCode.NP_Number6, false);
                    difference = GetSADifference(id);
                }
            }
        }

        private DateTime _useDematerializeAt = DateTime.MinValue;

        public override void UseSpells()
        {
            if (_fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
            {
                double targetDistanceFromLuopan = 0;
                List<TargetInfo> luopan = FindTargetAll("luopan", 30);

                if (luopan.Any())
                {
                    int luopanId = luopan[0].Id;
                    int monsterId = _fface.Target.ID;
                    targetDistanceFromLuopan = DistanceTo(new Node()
                    {
                        X = _fface.NPC.PosX(luopanId),
                        Z = _fface.NPC.PosZ(luopanId),
                    }, new Node()
                    {
                        X = _fface.NPC.PosX(monsterId),
                        Z = _fface.NPC.PosZ(monsterId),
                    });
                }

                if (!IsAfflicted(StatusEffect.Geo_Haste) && Ready(SpellList.Indi_Haste))
                {
                    UseSpell("Indi-Haste", SpellList.Indi_Haste, 10, false);
                }
                else if (Ready(SpellList.Geo_Frailty) && (!FindTargetAll("luopan", 10).Any() || targetDistanceFromLuopan > 4))
                {
                    if (targetDistanceFromLuopan != 0 && targetDistanceFromLuopan > 4)
                    {
                        UseAbility(AbilityList.Full_Circle, 2, false);
                        Thread.Sleep(2500);


                        if (Ready(AbilityList.Blaze_Glory))
                            UseAbility("Blaze of Glory", AbilityList.Blaze_Glory, 2, false);

                        Thread.Sleep(2500);

                        UseSpell("Geo-Frailty", SpellList.Geo_Frailty, 10, true);

                        _useDematerializeAt = DateTime.Now.AddMinutes(1).AddSeconds(10);
                    }
                    else if (targetDistanceFromLuopan == 0)
                    {


                        if (Ready(AbilityList.Blaze_Glory))
                            UseAbility("Blaze of Glory", AbilityList.Blaze_Glory, 2, false);

                        Thread.Sleep(2500);

                        UseSpell("Geo-Frailty", SpellList.Geo_Frailty, 10, true);
                        _useDematerializeAt = DateTime.Now.AddMinutes(1).AddSeconds(10);
                    }
                }
                else if (FindTargetAll("luopan", 10).Any() && Ready(AbilityList.Dematerialize) &&
                         Ready(AbilityList.Life_Cycle) && DateTime.Now > _useDematerializeAt)
                {
                    UseAbility(AbilityList.Dematerialize, 2, false);
                    Thread.Sleep(2500);
                    UseAbility(AbilityList.Life_Cycle, 2, false);
                }
            }
        }

        public static double DistanceTo(Node point1, Node point2)
        {
            var a = (double)(point2.X - point1.X);
            var b = (double)(point2.Z - point1.Z);

            return Math.Sqrt(a * a + b * b);
        }

        public List<TargetInfo> FindTargetAll(string name, int distance = 20)
        {
            List<TargetInfo> results = new List<TargetInfo>();

            for (short i = 0; i < 4096; i++)
            {
                if (_fface.NPC.Name(i).ToLower().Contains(name.ToLower()) && _fface.NPC.Distance(i) < distance && IsRendered(i))
                {
                    TargetInfo found = new TargetInfo
                    {
                        Id = i,
                        Name = _fface.NPC.Name(i),
                        Status = _fface.NPC.Status(i),
                        Distance = _fface.NPC.Distance(i),
                        IsRendered = IsRendered(i)
                    };
                    results.Add(found);
                }
            }
            return results;
        }

        #region Helper

        private double RadianToDegree(double radians)
        {
            return radians * (180.0 / Math.PI);
        }
        public double PointAngle(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }

        private double GetAngleOfLineBetweenTwoPoints(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }

        private double GetSADifference(int i)
        {
            double targetHeading = RadianToDegree(_fface.NPC.PosH(i));
            double lineAngle = GetAngleOfLineBetweenTwoPoints(new PointF { X = _fface.Player.PosX, Y = _fface.Player.PosZ }, new PointF { X = _fface.NPC.PosX(i), Y = _fface.NPC.PosZ(i) });
            double difference = (targetHeading + lineAngle) % 360;

            return difference;
        }
        #endregion

    }
}
