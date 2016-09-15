using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    public class Paladin : Jobs
    {
        public double RangedDistance = 20;

        public Paladin(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
        }

        /// <summary>
        /// Abilities and Spells to use when trying to claim while standing still from a distance
        /// </summary>
        public override void UseRangedClaim()
        {
            // Standard PLD abilities
            if (Ready(SpellList.Flash))
                UseSpell(SpellList.Flash, 6, true);

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
            if (_fface.Player.SubJob == Job.WAR)
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
            if (_fface.Player.SubJob == Job.WAR)
            {
                // use warcry
                if (!IsAfflicted(StatusEffect.Warcry) && Ready(AbilityList.Warcry))
                    UseAbility(AbilityList.Warcry, 2, false);


                if (Ready(AbilityList.Berserk))
                    UseAbility(AbilityList.Berserk, 2, false);
            }

            if (Ready(AbilityList.Shield_Bash))
                UseAbility(AbilityList.Shield_Bash, 3, true);


        }

        public override void UseHeals()
        {
            if (_fface.Player.HPPCurrent <= 50)
            {
                if (Ready(AbilityList.Sentinel) && !IsAfflicted(StatusEffect.Invincible))
                UseAbility(AbilityList.Sentinel, 3, false);

                if (Ready(AbilityList.Two_Hour) && !IsAfflicted(StatusEffect.Sentinel))
                    UseAbility("Invincible", AbilityList.Two_Hour, 3, false);

                if (_fface.Player.MPCurrent >= 88 && Ready(SpellList.Cure_IV))
                    UseSpell(SpellList.Cure_IV, 8, false);
                else if (_fface.Player.MPCurrent >= 46 && Ready(SpellList.Cure_III))
                    UseSpell(SpellList.Cure_III, 8, false);

            }
        }

        public override void UseSpells()
        {
            if (Ready(SpellList.Reprisal))
                UseSpell(SpellList.Reprisal, 6, false);

            if (Ready(SpellList.Phalanx) && !IsAfflicted(StatusEffect.Phalanx))
                UseSpell(SpellList.Phalanx, 6, false);

            if (Ready(SpellList.Crusade) && !IsAfflicted(StatusEffect.Enmity_Boost))
                UseSpell(SpellList.Crusade, 6, false);
        }

        public override void UseWeaponskills()
        {
            if (_fface.Player.MPCurrent <= 200)
            {
                if (Ready(AbilityList.Chivalry))
                {
                    UseAbility(AbilityList.Chivalry, 3, false);
                }
            }

            SendCommand("/ws \"Savage Blade\" <t>", 3, false);
        }

        #region Add other methods here that aren't overrides, such as calculating finishing moves, etc

        #endregion
    }
}
