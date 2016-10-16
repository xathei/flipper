using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    class BlackMage : Jobs
    {
        public BlackMage(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            Melee = false;
        }

        public override bool CanStun()
        {
                if (Ready(SpellList.Stun) && _fface.Player.MPCurrent >= 25 && !IsAfflicted(StatusEffect.Silence) && !IsAfflicted(StatusEffect.Sleep))
                    return true;
                else
                    return false;
        }

        public override void DoStun()
        {
            UseSpell(SpellList.Stun, 4, true);
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

        public override void Engage()
        {
            // Do not engage if we're inside the Specified Area.
            if (_content == Content.Ambuscade && (_fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion || _fface.Player.Zone == Zone.Moh_Gates))
                return;

            SendCommand("/attack <t>", 3);
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

        public override void UseSpells()
        {

            if (_fface.Player.HPPCurrent < 40 && _fface.Player.MPCurrent >= 88 && Ready(SpellList.Cure_IV))
                UseSpell(SpellList.Cure_IV, 8, false);

            if (!IsAfflicted(StatusEffect.Stoneskin) && Ready(SpellList.Stoneskin))
                UseSpell(SpellList.Stoneskin, 8);

            if (!IsAfflicted(StatusEffect.Haste) && Ready(SpellList.Haste))
                UseSpell(SpellList.Haste, 6);

            if (!IsAfflicted(StatusEffect.Refresh) && Ready(SpellList.Refresh))
                UseSpell(SpellList.Refresh, 6);

            if (Ready(SpellList.Thunder_V) && _fface.Player.MPCurrent >= 306)
            {
                if (Ready(AbilityList.Elemental_Seal))
                {
                    UseAbility(AbilityList.Elemental_Seal, 3, false);
                    Thread.Sleep(4000);
                }
                UseSpell(SpellList.Thunder_V, 15, true);
            }

            if (Ready(SpellList.Thunder_IV) && _fface.Player.MPCurrent >= 195)
                UseSpell(SpellList.Thunder_IV, 10, true);

            if (Ready(SpellList.Thunder_III) && _fface.Player.MPCurrent >= 91)
                UseSpell(SpellList.Thunder_III, 7, true);

            if (Ready(SpellList.Drain) && _fface.Player.HPPCurrent <= 90 && _fface.Player.MPCurrent >= 21)
                UseSpell(SpellList.Drain, 7, true);

        }

        public override void UseAbilities()
        {
            if (_fface.Target.HPPCurrent <= 50 && Ready(AbilityList.Enmity_Douse))
            {
                UseAbility(AbilityList.Enmity_Douse, 4, false);
            }
            if (_fface.Player.MPCurrent <= 200 && Ready(AbilityList.Convert) && (IsAfflicted(StatusEffect.Stoneskin) || _fface.Player.MPCurrent <= 100))
            {
                UseSpell(SpellList.Stoneskin, 10, false);
            }
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
