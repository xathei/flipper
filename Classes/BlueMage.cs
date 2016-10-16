using FFACETools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace Flipper.Classes
{
    public class BlueMage:Jobs
    {
        public BlueMage(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            Melee = true;
        }

        public override bool CanStun()
        {
            if (Ready(SpellList.Sudden_Lunge) && _fface.Player.MPCurrent >= 18 && !IsAfflicted(StatusEffect.Silence))
                return true;
            else
                return false;
            // return true if your timers are up, you're not silenced, and your have MP
        }

        public override void DoStun()
        {
           UseSpell(SpellList.Sudden_Lunge, 4, true);
        }

        public override void Warp()
        {
            _fface.Windower.SendString("//lua unload gearswap");
            Thread.Sleep(200);
            SendCommand("/equip l.ring \"Warp ring\"", 9);
            Thread.Sleep(11000);
            SendCommand("/item \"Warp Ring\" <me>", 10);
            Thread.Sleep(5000);
            _fface.Windower.SendString("//lua load gearswap");
        }

        public override void Engage()
        {
            SendCommand("/attack <t>", 3);
        }

        public override void UseRangedClaim()
        {
            _fface.Navigator.FaceHeading(_fface.Target.ID);
            Thread.Sleep(1000);
            SendCommand("/ra <t>", 5);
        }

        public override void UseAbilities()
        {
            if (HasItem(6468) && !IsAfflicted(StatusEffect.Food))
            {
                WriteLog("[KITCHEN] Using Sublime Sushi");
                SendCommand("/item \"Sublime Sushi\" <me>", 4);
            }

            if (_content == Content.Dynamis)
            {
                if(Ready(AbilityList.Unbridled_Learning))
                {
                    UseAbility(AbilityList.Unbridled_Learning, 2, false);
                  
                }
            }
            if(_content == Content.Ambuscade)
            {
                if(Ready(AbilityList.Berserk))
                {
                    UseAbility(AbilityList.Berserk, 2, false);
                }
                if(Ready(AbilityList.Aggressor))
                {
                    UseAbility(AbilityList.Aggressor, 2, false);
                }
            }
        }

        public override void UseClaim()
        {
            if (_fface.Player.SubJob == Job.WAR)
            {
                if (Ready(AbilityList.Provoke))
                    UseAbility(AbilityList.Provoke, 2, true);
            }
        }

        public override void UseWeaponskills()
        {
            if (IsAfflicted(StatusEffect.Aftermath_lvl3) && _fface.Player.TPCurrent >= 2000 )
            {
                SendCommand("/ws \"Savage Blade\" <t>", 3, false);

            }
            if(!IsAfflicted(StatusEffect.Aftermath_lvl3) && (_fface.Player.TPCurrent == 3000))
            {
                if (Ready(AbilityList.Warcry))
                {
                    UseAbility(AbilityList.Warcry, 3, false);
                }
                SendCommand("/ws \"Expiacion\" <t>", 3, false);
                

            }
        }
        public override void UseSpells()
        {
            if (!IsAfflicted(StatusEffect.Haste) && Ready(SpellList.Erratic_Flutter))
            {
                UseSpell(SpellList.Erratic_Flutter, 4, false);
            }

            if (!IsAfflicted(StatusEffect.Attack_Boost) && Ready(SpellList.Nat_Meditation))
            {
                SendCommand("/ma \"Nat. Meditation\" <me>", 5, false);
            }

            if (!IsAfflicted(StatusEffect.Defense_Boost) && Ready(SpellList.Cocoon))
            {
                UseSpell(SpellList.Cocoon, 3, false);
            }

            if(_fface.Player.HPPCurrent <= 50 && Ready(SpellList.Plenilune_Embrace))
            {
                UseSpell(SpellList.Plenilune_Embrace, 4, false);
            }
        }

        public override bool Position(int id, Monster monster, Combat.Mode mode)
        {
            if (DistanceTo(id) > monster.HitBox && CanStillAttack(id))
            {
                if (mode == Combat.Mode.Meshing && !Combat.IsPositionSafe(_fface.NPC.PosX(id), _fface.NPC.PosZ(id)))
                    return false;

                _fface.Navigator.Reset();
                _fface.Navigator.DistanceTolerance = monster.HitBox * 0.5;
                _fface.Navigator.Goto(_fface.NPC.PosX(id), _fface.NPC.PosZ(id), false);

            }

            bool wasLocked = true;
            if (_fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
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

        private double GetSADifference(int i)
        {
            var TargetHeading = (int)(_fface.Navigator.PosHToDegrees(_fface.NPC.PosH(i)));
            var MyHeading = (int)(_fface.Navigator.GetPlayerPosHInDegrees());
            double targetHeading = RadianToDegree(_fface.NPC.PosH(i));

            double lineAngle = GetAngleOfLineBetweenTwoPoints(new PointF { X = _fface.Player.PosX, Y = _fface.Player.PosZ }, new PointF { X = _fface.NPC.PosX(i), Y = _fface.NPC.PosZ(i) });
            double difference = (targetHeading + lineAngle) % 360;

            return difference;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private double GetAngleOfLineBetweenTwoPoints(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }
    }
}
