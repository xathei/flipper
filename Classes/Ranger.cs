using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFACETools;
using FlipperD;

namespace Flipper.Classes
{
    class Ranger : Jobs
    {
        public Ranger(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            Melee = true;
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

        public override bool CanStillAttack(int id)
        {
            if (!IsRendered(id))
            {
                //WriteLog("[STOP!] The target isn't rendered.");
                return false;
            }


            //if (_fface.NPC.IsClaimed(id) && !PartyHasHate(id) && _fface.Player.Status != Status.Fighting)
            //{
            //    //WriteLog("[STOP!] The target is claimed to someone else.");
            //    return false;
            //}

            // Skip if the mob more than 5 yalms above or below us
            if (Math.Abs(Math.Abs(_fface.NPC.PosY(id)) - Math.Abs(_fface.Player.PosY)) > 15)
            {
                WriteLog("[STOP!] The target is too far above or below.");
                return false;
            }

            // Skip if the NPC's HP is 0
            if (_fface.NPC.HPPCurrent(id) == 0)
            {
                //WriteLog("[STOP!] Target HP is 0 :(");
                return false;
            }

            if (DistanceTo(id) > MaxDistance() && _fface.Player.Status == Status.Fighting)
            {
                //WriteLog($"[STOP!] Distance: {DistanceTo(id)} > {MaxDistance()}");
                return false;
            }

            return true;
        }

        public override void UseClaim()
        {
            if (Ready(AbilityList.Shadowbind))
            {
                UseAbility(AbilityList.Shadowbind, 2, true);
            }

            if (_fface.Player.SubJob == Job.WAR)
            {
                if (Ready(AbilityList.Provoke))
                {
                    UseAbility(AbilityList.Provoke, 2, true);
                }
            }
        }

        public override void Engage()
        {
            SendCommand("/attack <t>", 3);
        }
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

        public override bool Position(int id, Monster monster, Combat.Mode mode)
        {

            //if (_content == Content.Ambuscade)
            //{
                //double aim = 0;

                //try
                //{
                //    aim = Convert.ToDouble(Program.mainform.rngAimForDistance.Text);
                //}
                //catch (Exception e)
                //{
                //    aim = 5.9;
                //}

                //var min = aim*0.95;
                //var max = aim*1.05;


                //    int fail = 100;
                //    while (_fface.NPC.Distance(id) < min && fail > 0)
                //    {
                //        if (!_fface.Target.IsLocked)
                //        {
                //            _fface.Windower.SendString("/lockon");
                //            Thread.Sleep(500);
                //        }
                //        fail--;
                //        _fface.Windower.SendKey(KeyCode.NP_Number2, true);
                //        Thread.Sleep(50);
                //        _fface.Windower.SendKey(KeyCode.NP_Number2, false);
                //    }
                //    fail = 1000;
                //    while (_fface.NPC.Distance(id) > max && fail > 0)
                //    {
                //        if (!_fface.Target.IsLocked)
                //        {
                //            _fface.Windower.SendString("/lockon");
                //            Thread.Sleep(500);
                //        }
                //        fail--;
                //        _fface.Windower.SendKey(KeyCode.NP_Number8, true);
                //        Thread.Sleep(50);
                //        _fface.Windower.SendKey(KeyCode.NP_Number8, false);
                //    }
                //    if (_fface.Target.IsLocked)
                //    {
                //        _fface.Windower.SendString("/lockon");
                //        Thread.Sleep(500);
                //    }

                //    bool wasLocked = true;
                //    var difference = GetSADifference(id);
                //    while (difference < 66 || difference > 89 && DistanceTo(id) <= 7)
                //    {
                //        if (!_fface.Target.IsLocked)
                //        {
                //            wasLocked = false;
                //            _fface.Windower.SendString("/lockon");
                //        }
                //        _fface.Windower.SendKey(KeyCode.NP_Number6, true);
                //        Thread.Sleep(80);
                //        _fface.Windower.SendKey(KeyCode.NP_Number6, false);
                //        difference = GetSADifference(id);
                //    }
                //    if (!wasLocked && _fface.Target.IsLocked)
                //    {
                //        _fface.Windower.SendString("/lockon");
                //    }
            //}
            //else
            //{
            //    if (DistanceTo(id) > monster.HitBox && CanStillAttack(id))
            //    {
            //        if (mode == Combat.Mode.Meshing && !Combat.IsPositionSafe(_fface.NPC.PosX(id), _fface.NPC.PosZ(id)))
            //            return false;

            //        _fface.Navigator.Reset();
            //        _fface.Navigator.DistanceTolerance = (double)(monster.HitBox * 0.95);
            //        _fface.Navigator.Goto(_fface.NPC.PosX(id), _fface.NPC.PosZ(id), false);
            //    }

            //    // move back from the target
            //    if (DistanceTo(id) < (monster.HitBox * 0.65) && CanStillAttack(id))
            //    {
            //        _fface.Windower.SendKey(KeyCode.NP_Number2, true);
            //        Thread.Sleep(50);
            //        _fface.Windower.SendKey(KeyCode.NP_Number2, false);

            //    }
            //}
            return true;
        }

        private DateTime _nextShot = DateTime.MinValue;

        public override void UseAbilities()
        {

            if (!IsAfflicted(StatusEffect.Velocity_Shot) && Ready(AbilityList.Velocity_Shot))
            {
                UseAbility(AbilityList.Velocity_Shot, 3, false);
            }


            var delay = 4900;
            try
            {
                delay = Convert.ToInt32(Program.mainform.rngDelay.Value);
            }
            catch (Exception e)
            {
                delay = 4900;
            }

            // check ammo
            if (HasItem(21296) && _fface.Item.GetEquippedItem(EquipSlot.Ammo).ID != 21296)
            {
                Thread.Sleep(1000);
                _fface.Windower.SendString("/equip ammo \"Chrono Bullet\"");
            }
            else if (HasItem(21327) && _fface.Item.GetEquippedItem(EquipSlot.Ammo).ID != 21327)
            {
                Thread.Sleep(1000);
                _fface.Windower.SendString("/equip ammo \"Eradicating bullet\"");
            } 

            if (HasItem(6468) && !IsAfflicted(StatusEffect.Food))
            {
                WriteLog("[KITCHEN] Using Sublime Sushi");
                SendCommand("/item \"Sublime Sushi\" <me>", 4);
            }

            if (_fface.Player.SubJob == Job.WAR)
            {
                // use warcry
                if (!IsAfflicted(StatusEffect.Warcry) && Ready(AbilityList.Warcry))
                    UseAbility(AbilityList.Warcry, 2, false);


                if (Ready(AbilityList.Berserk))
                    UseAbility(AbilityList.Berserk, 2, false);
            }

            if (Ready(AbilityList.Scavenge) && _fface.Target.HPPCurrent < 25)
                UseAbility(AbilityList.Scavenge, 2, false);


            if (_fface.Player.Zone != Zone.Den_of_Rancor)
            {
                if (Ready(AbilityList.Sharpshot) && Ready(AbilityList.Barrage))
                {
                    Thread.Sleep(2000);
                    UseAbility(AbilityList.Sharpshot, 4, false);
                    Thread.Sleep(4500);
                    UseAbility(AbilityList.Barrage, 2, false);
                    Thread.Sleep(2000);
                }

                if (!IsAfflicted(StatusEffect.Double_Shot) && Ready(AbilityList.Unlimited_Shot) && _fface.NPC.HPPCurrent(_fface.Target.ID) > 25)
                {
                    UseAbility("Double Shot", AbilityList.Unlimited_Shot, 2, false);
                }
            }

            if (DateTime.Now > _nextShot)
            {
                if ((!IsAfflicted(StatusEffect.Aftermath) && _fface.Player.TPCurrent < 3000 &&
                     _fface.Target.HPPCurrent > 50)
                    ||
                    _fface.Player.TPCurrent <= 1000)
                {

                    _nextShot = DateTime.Now.AddMilliseconds(delay);
                    NextCommandAllowed = DateTime.Now.AddMilliseconds(delay);
                    _fface.Windower.SendString("/ra <t>");
                }
            }
        }


        public override void UseWeaponskills()
        {
            if (!IsAfflicted(StatusEffect.Aftermath) && _fface.Player.TPCurrent == 3000)
            {
                SendCommand("/ws \"Coronach\" <t>", 3, false);
            }
            if (IsAfflicted(StatusEffect.Aftermath))
            {
                SendCommand("/ws \"Last Stand\" <t>", 3, false);
            }

        }

    }
}
