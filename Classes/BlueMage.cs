using FFACETools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flipper.Classes
{
    public class BlueMage:Jobs
    {
        public BlueMage(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
        }

        public override void UseRangedClaim()
        {
            _fface.Navigator.FaceHeading(_fface.Target.ID);
            Thread.Sleep(1000);
            SendCommand("/ra <t>", 5);
        }

        public override void UseAbilities()
        {
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
        }

    }
}
