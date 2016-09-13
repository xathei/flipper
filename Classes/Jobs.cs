using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    public class Jobs
    {
        private FFACE fface;
        private Content _content;

        /// <summary>
        /// The current event the player is participating in.
        /// </summary>
        public enum Content
        {
            Dynamis,
            Farming,
            Ambuscade,
            Voidwatch,
            Unity
        }

        /// <summary>
        /// Called when the user is following a strict path, but needs to claim a monster.
        /// </summary>
        public virtual void UseRangedClaim()
        {
            
        }

        /// <summary>
        /// Called to attempt to make a claim on the current target immediately.
        /// This will not be called once the target is claimed or is in range.
        /// </summary>
        public virtual void UseClaim()
        {

        }

        /// <summary>
        /// Called continually in content where a monster needs staggering, and hasn't been staggered yet.
        /// </summary>
        public virtual void Stagger()
        {
            
        }


        /// <summary>
        /// Called continually during battle.
        /// </summary>
        public virtual void UseAbilities()
        {
            
        }


        /// <summary>
        /// Called continually during battle.
        /// </summary>
        public virtual void UseSpells()
        {
            
        }

        /// <summary>
        /// Called continually with priority if the player's HP is below 75%.
        /// </summary>
        public virtual void UseHeals()
        {
            
        }

        /// <summary>
        /// Called continually when the player has over 1000 TP.
        /// </summary>
        public virtual void UseWeaponskills()
        {
            
        }


        #region Helper Methods
        public Dictionary<string, DateTime> CommandsLog = new Dictionary<string, DateTime>();
        public DateTime NextCommandAllowed = DateTime.MinValue;


        public void SendCommand(string command, int Delay = 2, bool PreventGlobal = false)
        {
            if ((!CommandsLog.ContainsKey(command) || (CommandsLog.ContainsKey(command) && CommandsLog[command] < DateTime.Now)) &&
                (NextCommandAllowed <= DateTime.Now || PreventGlobal))
            {
                //WriteLog("BattleBot: Sending command ->" + command, true);

                if (CommandsLog.ContainsKey(command))
                    CommandsLog[command] = DateTime.Now.AddSeconds(Delay);
                else
                    CommandsLog.Add(command, DateTime.Now.AddSeconds(Delay));

                if (!PreventGlobal)
                    NextCommandAllowed = DateTime.Now.AddSeconds(Delay);
                else
                    NextCommandAllowed = DateTime.MinValue;

                fface.Windower.SendString(command);
            }
        }

        /// <summary>
        /// Uses an ability from the AbilityList enum.
        /// </summary>
        /// <example>
        ///     UseAbility(AbilityList.Provoke, 3, true);
        ///     UseAbility(AbilityList.Sentinel, 3, false);
        /// </example>
        /// <param name="ability">The ability to use</param>
        /// <param name="Delay">How long does it take to use this ability? (Consider lockout after use)</param>
        /// <param name="Offensive">Is this ability used on me, or my target?</param>
        public void UseAbility(AbilityList ability, int Delay = 2, bool Offensive = false)
        {
            SendCommand("/ja \"" + ability.ToString().Replace('_', ' ') + "\" " + (Offensive ? "<t>" : "<me>"), Delay);
        }


        /// <summary>
        /// Uses an ability list that belongs to a group of abilities; such as a Pet ability, DNC Step, or DNC Waltz.
        /// </summary>
        /// <example>
        ///     UseAbility("Predator Claws", AbilityList.Pet_commands, 3, true);
        /// </example>
        /// <param name="ability">The ability to use</param>
        /// <param name="abilityGroup">The ability group to use from AbilityList</param>
        /// <param name="Delay">How long does it take to use this ability? (Consider lockout after use)</param>
        /// <param name="Offensive">Is this ability used on me, or my target?</param>
        public void UseAbility(string ability, AbilityList abilityGroup, int Delay = 2, bool Offensive = false)
        {
            SendCommand("/ja \"" + ability.ToString().Replace('_', ' ') + "\" " + (Offensive ? "<t>" : "<me>"), Delay);
        }


        /// <summary>
        /// Uses a spell from the SpellList enum.
        /// </summary>
        /// <example>
        ///     UseSpell(SpellList.Flash, 3, true);
        ///     UseSpell(SpellList.Stoneskin, 3, false);
        /// </example>
        /// <param name="ability">The ability to use</param>
        /// <param name="Delay">How long does it take to use this ability? (Consider lockout after use)</param>
        /// <param name="Offensive">Is this ability used on me, or my target?</param>
        public void UseSpell(SpellList spell, int castTime, bool Offensive = false)
        {
            SendCommand("/ma \"" + spell.ToString().Replace('_', ' ') + "\" " + (Offensive ? "<t>" : "<me>"));
        }


        /// <summary>
        /// Check if the provided spell is ready to use.
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public bool Ready(SpellList spell)
        {
            if (fface.Timer.GetSpellRecast(spell) == 0)
                return true;

            return false;
        }


        /// <summary>
        /// Check if the provided ability is ready to use.
        /// </summary>
        /// <param name="ability"></param>
        /// <returns></returns>
        public bool Ready(AbilityList ability)
        {
            if (fface.Timer.GetAbilityRecast(ability) == 0)// && fface.Player.HasAbility(ability))
                return true;

            return false;
        }

        /// <summary>
        /// Determines if a player is afflicted with a certain effect.
        /// </summary>
        /// <param name="effect">The effect to check for</param>
        /// <returns>True if afflicted, else false.</returns>
        public bool IsAfflicted(StatusEffect effect)
        {
            foreach (StatusEffect status in fface.Player.StatusEffects)
            {
                if (status == effect)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
