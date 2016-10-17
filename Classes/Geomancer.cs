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

        public override bool CanStun()
        {
            if (Ready(SpellList.Stun) && _fface.Player.MPCurrent >= 25 && !IsAfflicted(StatusEffect.Silence))
                return true;
            else
                return false;
            // return true if your timers are up, you're not silenced, and your have MP
        }

        public override void DoStun()
        {
            SendCommand("/ma \"Stun\" <t>", 4, true);
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
            if(_content != Content.Ambuscade)
                SendCommand("/attack <t>", 3);
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

        public override bool Position(int id, Monster monster, Combat.Mode mode)
        {
            if (DistanceTo(id) > monster.HitBox && CanStillAttack(id))
            {
                if (mode == Combat.Mode.Meshing && !Combat.IsPositionSafe(_fface.NPC.PosX(id), _fface.NPC.PosZ(id)))
                    return false;

                _fface.Navigator.Reset();
                _fface.Navigator.DistanceTolerance = monster.HitBox * 0.95;
                _fface.Navigator.Goto(_fface.NPC.PosX(id), _fface.NPC.PosZ(id), false);

            }

            bool wasLocked = true;
            if (_fface.Player.Zone != Zone.Cape_Teriggan)
            {
                var difference = GetSADifference(id);
                while ((difference < 66 || difference > 89) && _fface.NPC.HPPCurrent(id) > 0)
                {
                    if (!_fface.Target.IsLocked)
                    {
                        wasLocked = false;
                        _fface.Windower.SendString("/lockon");
                    }
                    _fface.Windower.SendKey(KeyCode.NP_Number6, true);
                    Thread.Sleep(80);
                    _fface.Windower.SendKey(KeyCode.NP_Number6, false);
                    difference = GetSADifference(id);
                }
                if (!wasLocked && _fface.Target.IsLocked)
                {
                    _fface.Windower.SendString("/lockon");
                }
            }
            return true;
        }

        private DateTime _useDematerializeAt = DateTime.MinValue;

        public override void UseSpells()
        {
            if (_fface.Player.Zone != Zone.Cape_Teriggan | _fface.Player.Zone != Zone.Den_of_Rancor)
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
                    if (!IsAfflicted(StatusEffect.Geo_Fury) && Ready(SpellList.Indi_Fury))
                    {
                        UseSpell("Indi-Fury", SpellList.Indi_Fury, 10, false);
                    }
                    else if (Ready(SpellList.Geo_Frailty) &&
                             (!FindTargetAll("luopan", 10).Any() || targetDistanceFromLuopan > 4))
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
            if (_fface.Player.SubJob == Job.WHM)
            {
                foreach(Player player in Players)
                {
                    foreach(StatusEffect e in player.Effects)
                    {
                        if (e == StatusEffect.Attack_Down)
                        {
                            if (Ready(SpellList.Erase))
                            {
                                UseSpell(SpellList.Erase, 4, player.Name);
                            }
                        }
                    }
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

        public override void UseAbilities()
        {
            if (Ready(AbilityList.Radial_Arcana) && _fface.Player.MPPCurrent <= 50)
            {
                UseAbility(AbilityList.Radial_Arcana);
            }
        }

        public override void UseWeaponskills()
        {
            if (_fface.Player.TPCurrent >= 2000)
            {
                SendCommand("/ws \"Hexa Strike\" <t>", 3, false);

            }
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
