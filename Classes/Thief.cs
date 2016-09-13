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
        private FFACE fface;

        public Thief(FFACE instance)
        {
            fface = instance;
        }


        public override void UseClaim(Content content)
        {
            // if we have access to warrior abilities, let's try them first.
            if (fface.Player.SubJob == Job.WAR)
            {
                if (base.Ready(AbilityList.Provoke))
                    base.UseAbility(AbilityList.Provoke, 2, true);
            }

            // then let's try bully
            if (base.Ready(AbilityList.Bully) && fface.NPC.Distance(fface.Target.ID) < 6.9)
                base.UseAbility(AbilityList.Bully, 2, true);

        }


        public override void Stagger(Content content)
        {
            
        }

        public override void UseAbilities(Content content)
        {
            // use flee on cool down, no idea why.
            if (!IsAfflicted(StatusEffect.Flee) && Ready(AbilityList.Flee))
                base.UseAbility(AbilityList.Flee, 2, false);

            // check for war abilities
            if (fface.Player.SubJob == Job.WAR)
            {
                // use warcry
                if (!IsAfflicted(StatusEffect.Warcry) && Ready(AbilityList.Warcry))
                    base.UseAbility(AbilityList.Warcry, 2, false);


                if (Ready(AbilityList.Berserk))
                    base.UseAbility(AbilityList.Berserk, 2, false);
            }
        }

        public override void UseHeals(Content content)
        {
        }

        public override void UseSpells(Content content)
        {
        }


        #region Add other methods here that aren't overrides, such as calculating finishing moves, etc

        #endregion
    }
}
