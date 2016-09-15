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
            if (Ready(SpellList.Flash) && _fface.NPC.Distance(_fface.Target.ID) < 14)
                UseSpell(SpellList.Flash, 6, true);

            // /WAR49 specific abilities.
            if (_fface.Player.SubJob == Job.WAR)
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

        }

        public override void UseHeals()
        {

        }

        public override void UseSpells()
        {

        }

        public override void UseWeaponskills()
        {
            if (_fface.Player.HasWeaponSkill((uint)WeaponSkillList.Savage_Blade))
                SendCommand("/ws \"Savage Blade\" <t>", 3, false);
        }

        #region Add other methods here that aren't overrides, such as calculating finishing moves, etc

        #endregion
    }
}
