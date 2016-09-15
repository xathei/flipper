using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    public class Thief : Jobs
    {
        public Thief(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
        }

        public override void UseRangedClaim()
        {
            _fface.Navigator.FaceHeading(_fface.Target.ID);
            Thread.Sleep(2000);
            SendCommand("/ra <t>", 5);
        }

        public override void UseClaim()
        {
            // if we have access to warrior abilities, let's try them first.
            if (_fface.Player.SubJob == Job.WAR)
            {
                if (Ready(AbilityList.Provoke))
                    UseAbility(AbilityList.Provoke, 2, true);
            }

            // then let's try bully
            if (Ready(AbilityList.Bully) && _fface.NPC.Distance(_fface.Target.ID) < 6.9)
                UseAbility(AbilityList.Bully, 2, true);

        }


        public override void Stagger()
        {
            
        }

        public override void UseAbilities()
        {
            if (Ready(AbilityList.Bully))
                UseAbility(AbilityList.Bully, 2, true);

            if (Ready(AbilityList.Conspirator))
                UseAbility(AbilityList.Conspirator, 2, false);

            // check for war abilities
            if (_fface.Player.SubJob == Job.WAR)
            {
                // use warcry
                if (!IsAfflicted(StatusEffect.Warcry) && Ready(AbilityList.Warcry))
                    UseAbility(AbilityList.Warcry, 2, false);


                if (Ready(AbilityList.Berserk))
                   UseAbility(AbilityList.Berserk, 2, false);
            }

            // check for dnc abiliteis
            if (_fface.Player.SubJob == Job.DNC)
            {
                if (IsAfflicted(StatusEffect.Haste_Samba) == false &&
                    Ready(AbilityList.Sambas) == true &&
                    _fface.Player.TPCurrent >= 350)
                {
                    UseAbility("Haste Samba", AbilityList.Sambas, 4, false);
                }

                if ((IsAfflicted(StatusEffect.Paralysis) ||
                    IsAfflicted(StatusEffect.Poison) ||
                    IsAfflicted(StatusEffect.Slow) ||
                    IsAfflicted(StatusEffect.Blindness)) && _fface.Player.TPCurrent >= 200 && Ready(AbilityList.Waltzes))
                {
                    UseAbility("Healing Waltz", AbilityList.Waltzes, 3, false);
                }
            }
        }

        public override void UseHeals()
        {
            if (_fface.Player.HPPCurrent < 75)
            {
                if (_fface.Player.HPPCurrent < 25 && Ready(AbilityList.Two_Hour))
                {
                    SendCommand("/ja \"Perfect Dodge\" <me>");
                }

                //if (_fface.Player.SubJob == Job.DNC)
                //{
                //    if (_fface.Player.TPCurrent >= 500)
                //    {
                //        UseAbility("Curing Waltz III", AbilityList.Waltzes, 3, false);
                //    }
                //    else if (_fface.Player.TPCurrent >= 350)
                //    {
                //        UseAbility("Curing Waltz II", AbilityList.Waltzes, 3, false);
                //    }
                //    else if (_fface.Player.TPCurrent >= 200)
                //    {
                //        UseAbility("Curing Waltz", AbilityList.Waltzes, 3, false);
                //    }
                //}
            }
        }

        public override void UseSpells()
        {

        }

        public override void UseWeaponskills()
        {
            SendCommand("/ws \"Rudra's Storm\" <t>", 3);
        }

        #region Add other methods here that aren't overrides, such as calculating finishing moves, etc

        #endregion
    }
}
