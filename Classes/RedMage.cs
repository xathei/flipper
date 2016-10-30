using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    public class RedMage : Jobs
    {
        public double RangedDistance = 20;

        public RedMage(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            Melee = true;
        }


        public override bool CanStun()
        {
            // if (Ready(AbilityList.Shield_Bash))
            //   return true;

            //if (_fface.Player.TPCurrent > 1000)
            //    return true;

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

        }

        /// <summary>
        /// Attempts to claim while running towards the mob
        /// </summary>
        public override void UseClaim()
        {

        }


        public override void Stagger()
        {

        }

        public override void UseAbilities()
        {    

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

        private bool PDTSet = false;

        public override void UseHeals()
        {

        }

        public override void UseSpells()
        {
            if (_fface.Player.HPPCurrent < 50 && Ready(SpellList.Cure_IV))
            {
                UseSpell(SpellList.Cure_IV, 7, false);
            }
            if (!IsAfflicted(StatusEffect.Composure) && Ready(AbilityList.Composure))
            {
                UseAbility(AbilityList.Composure, 2, false);
            }
            if (!IsAfflicted(StatusEffect.Haste) && Ready(SpellList.Haste_II))
            {
                UseSpell(SpellList.Haste_II, 8, false);
            }
            if (!IsAfflicted(StatusEffect.Refresh) && Ready(SpellList.Refresh_II))
            {
                UseSpell("Refresh III", SpellList.Refresh_II, 12, false);
            }
            if (!IsAfflicted(StatusEffect.Protect) && Ready(SpellList.Protect_V))
            {
                UseSpell(SpellList.Protect_V, 8, false);
            }
            if (!IsAfflicted(StatusEffect.Shell) && Ready(SpellList.Shell_V))
            {
                UseSpell(SpellList.Shell_V, 8, false);
            }
            if (!IsAfflicted(StatusEffect.Multi_Strikes) && Ready(SpellList.Temper))
            {
                UseSpell("Temper II", SpellList.Temper, 10, false);
            }
            if (!IsAfflicted(StatusEffect.Stoneskin) && Ready(SpellList.Stoneskin))
            {
                UseSpell(SpellList.Stoneskin, 12, false);
            }
            if (!IsAfflicted(StatusEffect.Phalanx) && Ready(SpellList.Phalanx))
            {
                UseSpell(SpellList.Phalanx, 12, false);
            }
            if (!IsAfflicted(StatusEffect.Enthunder_2) && Ready(SpellList.Enthunder_II))
            {
                UseSpell(SpellList.Enthunder_II, 16, false);
            }
            if (!IsAfflicted(StatusEffect.STR_Boost2) && Ready(SpellList.Gain_STR))
            {
                UseSpell(SpellList.Gain_STR, 17, false);
            }
            if (!IsAfflicted(StatusEffect.Aquaveil) && Ready(SpellList.Aquaveil))
            {
                UseSpell(SpellList.Aquaveil, 12, false);
            }
        }

        public override void UseWeaponskills()
        {
            if (IsAfflicted(StatusEffect.Aftermath_lvl3) && _fface.Player.TPCurrent >= 1000)
            {
                SendCommand("/ws \"Savage Blade\" <t>", 3, false);

            }
            if (!IsAfflicted(StatusEffect.Aftermath_lvl3) && _fface.Player.TPCurrent == 3000)
            {
                SendCommand("/ws \"Death Blossom\" <t>", 3, false);
            }
        }

        #region Add other methods here that aren't overrides, such as calculating finishing moves, etc

        #endregion
    }
}
