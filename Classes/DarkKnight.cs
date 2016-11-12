using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    public class DarkKnight : Jobs
    {
        public double RangedDistance = 20;

        public DarkKnight(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            Melee = true;
        }


        public override bool CanStun()
        {
            return false;
        }

        public override void DoStun()
        {
        }

        public override void Engage()
        {
            SendCommand("/attack <t>", 3);
        }

        /// <summary>
        /// Abilities and Spells to use when trying to claim while standing still from a distance
        /// </summary>
        public override void UseRangedClaim()
        {
            // /WAR49 specific abilities.
            if (_fface.Player.SubJob == Job.WAR && _fface.Navigator.DistanceTo(_fface.Target.ID) < 16)
            {
                if (Ready(AbilityList.Provoke))
                    UseAbility(AbilityList.Provoke, 2, true);
            }
        }

        /// <summary>
        /// Attempts to claim while running towards the mob
        /// </summary>
        public override void UseClaim()
        {
            if (_fface.Player.SubJob == Job.WAR && DistanceTo(_fface.Target.ID) <= 15.8)
            {
                if (Ready(AbilityList.Provoke))
                    UseAbility(AbilityList.Provoke, 2, true);
            }
        }


        public override void Stagger()
        {

        }

        public override void UseAbilities()
        {
            if (HasItem(6468) && !IsAfflicted(StatusEffect.Food))
            {
                SendCommand("/item \"Sublime Sushi\" <me>", 4);
            }

            if (_fface.Player.SubJob == Job.WAR)
            {
                // use warcry
                if (!IsAfflicted(StatusEffect.Warcry) && Ready(AbilityList.Warcry))
                    UseAbility(AbilityList.Warcry, 2, false);

                // use berserk
                if (!IsAfflicted(StatusEffect.Berserk) && Ready(AbilityList.Berserk))
                    UseAbility(AbilityList.Berserk, 2, false);

            }

            if (Ready(AbilityList.Last_Resort))
            {
                UseAbility(AbilityList.Last_Resort, 2, false);
            }
        }

        /// <summary>
        /// Determines if the character is still engaged and can still attack their target.
        /// Called periodically during combat.
        /// </summary>
        /// <param name="id">The ID of your target</param>
        public override bool CanStillAttack(int id)
        {

            if (!IsRendered(id))
            {
                return false;
            }

            if (Math.Abs(Math.Abs(_fface.NPC.PosY(id)) - Math.Abs(_fface.Player.PosY)) > 15)
            {
                WriteLog("[STOP!] The target is too far above or below.");
                return false;
            }

            if (_fface.NPC.HPPCurrent(id) == 0)
            {
                return false;
            }

            if (DistanceTo(id) > MaxDistance() && _fface.Player.Status == Status.Fighting)
            {
                return false;
            }

            return true;
        }

        public override void UseHeals()
        {
            if (_fface.Player.HPPCurrent <= 50)
            {
                if (Ready(AbilityList.Two_Hour))
                    UseAbility("Blood Weapon", AbilityList.Two_Hour, 2, false);

            }

        }

        public override void UseSpells()
        {
            if (!IsAfflicted(StatusEffect.Endark) && Ready(SpellList.Endark))
                SendCommand("/ma \"Endark II\" <me>", 6);
        }

        public override void UseWeaponskills()
        {
            if (_fface.Player.TPCurrent >= 1000)
            {
                SendCommand("/ws \"Catastrophe\" <t>", 3, false);

            }
        }

        #region Add other methods here that aren't overrides, such as calculating finishing moves, etc

        #endregion
    }
}
