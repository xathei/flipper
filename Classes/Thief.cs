using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    public class Thief : Jobs
    {
        private FFACE _fface;
        private Content _content;

        public Thief(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
        }


        public override void UseClaim()
        {
            // if we have access to warrior abilities, let's try them first.
            if (_fface.Player.SubJob == Job.WAR)
            {
                if (base.Ready(AbilityList.Provoke))
                    base.UseAbility(AbilityList.Provoke, 2, true);
            }

            // then let's try bully
            if (base.Ready(AbilityList.Bully) && _fface.NPC.Distance(_fface.Target.ID) < 6.9)
                base.UseAbility(AbilityList.Bully, 2, true);

        }


        public override void Stagger()
        {
            
        }

        public override void UseAbilities()
        {
            // use flee on cool down, no idea why.
            if (!IsAfflicted(StatusEffect.Flee) && Ready(AbilityList.Flee))
                base.UseAbility(AbilityList.Flee, 2, false);

            // check for war abilities
            if (_fface.Player.SubJob == Job.WAR)
            {
                // use warcry
                if (!IsAfflicted(StatusEffect.Warcry) && Ready(AbilityList.Warcry))
                    base.UseAbility(AbilityList.Warcry, 2, false);


                if (Ready(AbilityList.Berserk))
                    base.UseAbility(AbilityList.Berserk, 2, false);
            }
        }

        public override void UseHeals()
        {

        }

        public override void UseSpells()
        {

        }

        public override void UseWeaponskills()
        {
        }

        #region Add other methods here that aren't overrides, such as calculating finishing moves, etc

        #endregion
    }
}
