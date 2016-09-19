using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using FlipperD;

namespace Flipper.Classes
{
    public class Jobs : IJob
    {
        public bool Melee = true;
        public FFACE _fface;
        public Content _content;

        public bool Engages()
        {
            return Melee;
        }
           
        /// <summary>
        /// The furthest distance you can be from the enemy before abandoning battle.
        /// </summary>
        /// <returns></returns>
        public virtual int MaxDistance()
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

        /// <summary>
        /// Determines if the character is still engaged and can still attack their target.
        /// Called periodically during combat.
        /// </summary>
        /// <param name="id">The ID of your target</param>
        public virtual bool CanStillAttack(int id)
        {

            if (!IsRendered(id))
            {
               WriteLog("[STOP!] The target isn't rendered.");
                return false;
            }

            if (_fface.NPC.IsClaimed(id) && !PartyHasHate(id) && _fface.Player.Status != Status.Fighting)
            {
               WriteLog("[STOP!] The target is claimed to someone else.");
                return false;
            }

            // Skip if the mob more than 5 yalms above or below us
            if (Math.Abs(Math.Abs(_fface.NPC.PosY(id)) - Math.Abs(_fface.Player.PosY)) > 15)
            {
                WriteLog("[STOP!] The target is too far above or below.");
                return false;
            }

            // Skip if the NPC's HP is 0
            if (_fface.NPC.HPPCurrent(id) == 0)
            {
                WriteLog("[STOP!] Target HP is 0 :(");
                return false;
            }

            if (DistanceTo(id) > MaxDistance() && _fface.Player.Status == Status.Fighting)
            {
                WriteLog($"[STOP!] Distance: {DistanceTo(id)} > {MaxDistance()}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Called continually during battle to ensure characters are in the correct position.
        /// </summary>
        public virtual void Position(int id, Monster monster)
        {
            // move closer to the mob
            if (DistanceTo(id) > monster.HitBox && CanStillAttack(id))
            {
                _fface.Navigator.Reset();
                _fface.Navigator.DistanceTolerance = (double) (monster.HitBox*0.95);
                _fface.Navigator.Goto(_fface.NPC.PosX(id), _fface.NPC.PosZ(id), false);
            }

            // move back from the target
            if (DistanceTo(id) < (monster.HitBox*0.65) && CanStillAttack(id))
            {
                _fface.Windower.SendKey(KeyCode.NP_Number2, true);
                Thread.Sleep(50);
                _fface.Windower.SendKey(KeyCode.NP_Number2, false);

            }
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
        /// Shows the settings form
        /// </summary>
        public virtual void SettingsForm()
        {
            MessageBox.Show($"Settings have not yet been implemented for {_fface.Player.MainJob}.", "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /// <summary>
        /// Engages the current target.
        /// </summary>
        public virtual void Engage()
        {
            SendCommand("/attack <t>", 3);
        }

        /// <summary>
        /// Spawns trusts.
        /// </summary>
        public void SpawnTrusts()
        {
            //SendCommand("/ma \"Ulmia\" <me>");
            //Thread.Sleep(7000);
            //SendCommand("/ma \"Koru-Moru\" <me>");
            //Thread.Sleep(7000);
            SendCommand("/ma \"Apururu (UC)\" <me>");
            Thread.Sleep(7000);
            SendCommand("/ma \"Koru-Moru\" <me>");
            Thread.Sleep(7000);
        }

        /// <summary>
        /// Causes the player to warp from their current location via any means possible.
        /// </summary>
        public virtual void Warp()
        {
            _fface.Windower.SendString("//gs disable lring");
            Thread.Sleep(200);
            SendCommand("/equip l.ring \"Warp ring\"", 9);
            Thread.Sleep(11000);
            SendCommand("/item \"Warp Ring\" <me>", 10);
            Thread.Sleep(5000);
            _fface.Windower.SendString("//gs enable lring");
        }

        /// <summary>
        /// Called when the player needs to be under the status effect of Sneak and Invisible.
        /// </summary>
        public void DoHide()
        {
            // Check if the player's MainJob or SubJob is set to Dancer.
            if (_fface.Player.MainJob == Job.DNC || _fface.Player.SubJob == Job.DNC)
            {
                // Check if Spectral Jig is not on cooldown.
                if (Ready(AbilityList.Spectral_Jig))
                {
                    // If the player is under the effect of 'Sneak', remove it.
                    if (IsAfflicted(StatusEffect.Sneak))
                    {
                        _fface.Windower.SendString("//cancel sneak");
                    }

                    // If the player is not under the effect of 'Sneak' and 'Invisible', use 'Spectral Jig'.
                    if (!IsAfflicted(StatusEffect.Sneak) && !IsAfflicted(StatusEffect.Invisible))
                    {
                        UseAbility(AbilityList.Spectral_Jig);
                    }
                }
            }
            // Check if the player's MainJob or SubJob is set to either White Mage, Red Mage or Scholar.
            else if ((_fface.Player.MainJob == Job.WHM || _fface.Player.MainJob == Job.RDM || _fface.Player.MainJob == Job.SCH)
                    || (_fface.Player.SubJob == Job.WHM || _fface.Player.SubJob == Job.RDM || _fface.Player.SubJob == Job.SCH))
            {
                // Check if the player is not under the effect of 'Sneak' has learned the spell 'Sneak' and 'Sneak' is not on cooldown.
                if (!IsAfflicted(StatusEffect.Sneak) && HasSpell(SpellList.Sneak) && Ready(SpellList.Sneak))
                {
                    UseSpell(SpellList.Sneak, 5);
                }

                // Check if the player is not under the effect of 'Invisible' has learned the spell 'Invisible' and 'Invisible' is not on cooldown.
                if (!IsAfflicted(StatusEffect.Invisible) && HasSpell(SpellList.Invisible) && Ready(SpellList.Invisible))
                {
                    UseSpell(SpellList.Invisible, 5);
                }
            }
            // Check if the player's MainJob or SubJob is set to Ninja.
            else if (_fface.Player.MainJob == Job.NIN || _fface.Player.SubJob == Job.NIN)
            {
                // Check if the player is not under the effect of 'Sneak' and has learned the spell 'Monomi: Ichi' and the spell 'Monomi: Ichi' is not on cooldown.
                if (!IsAfflicted(StatusEffect.Sneak) && HasSpell(SpellList.Monomi_Ichi) && Ready(SpellList.Monomi_Ichi))
                {
                    // If the player's MainJob is Ninja, allow use of 'Shikanofuda'. Otherwise use 'Sanjaku-Tenugui'.
                    if (_fface.Player.MainJob == Job.NIN && HasItem(2972) || HasItem(2553))
                    {
                        UseSpell(SpellList.Monomi_Ichi, 5);
                    }
                }

                // Check if the player is not under the effect of 'Invisible' has learned the spell 'Tonko: Ni' and the spell 'Tonki: Ni' is not on cooldown.
                if (!IsAfflicted(StatusEffect.Invisible) && HasSpell(SpellList.Tonko_Ni) && Ready(SpellList.Tonko_Ni))
                {
                    // If the player's MainJob is Ninja, allow use of 'Shikanofuda'. Otherwise use 'Shinobi-Tabi'.
                    if (_fface.Player.MainJob == Job.NIN && HasItem(2972) || HasItem(1194))
                    {
                        UseSpell(SpellList.Tonko_Ni, 5);
                    }
                }
                // Check if the player is not under the effect of 'Invisible' has learned the spell 'Tonko: Ichi' and the spell 'Tonki: Ichi' is not on cooldown.
                else if (!IsAfflicted(StatusEffect.Invisible) && HasSpell(SpellList.Tonko_Ichi) && Ready(SpellList.Tonko_Ichi))
                {
                    // If the player's MainJob is Ninja, allow use of 'Shikanofuda'. Otherwise use 'Shinobi-Tabi'.
                    if (_fface.Player.MainJob == Job.NIN && HasItem(2972) || HasItem(1194))
                    {
                        UseSpell(SpellList.Tonko_Ichi, 5);
                    }
                }
            }
            // Use 'Silent Oil' and 'Prism Powder' if none of the spells are abilities are able to be used.
            else
            {
                // Check if the player  and if the player has any 'Silent Oil' in their inventory.
                if (!IsAfflicted(StatusEffect.Sneak) && HasItem(4165))
                {
                    UseItem(4165, 5);
                }

                // Check if the player is not under the effect of 'Invisible' and if the player has any 'Prism Powder' in their inventory.
                if (!IsAfflicted(StatusEffect.Invisible) && HasItem(4164))
                {
                    UseItem(4164, 5);
                }
            }
        }

        #region Helper Methods

        /// <summary>
        /// A lazy way to return distance from target.
        /// </summary>
        /// <param name="id">ID of target.</param>
        /// <returns>Double indicating distance from target.</returns>
        public double DistanceTo(int id)
        {
            return _fface.Navigator.DistanceTo(id);
        }

        /// <summary>
        /// Checks if a party member has hate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool PartyHasHate(int id)
        {
            for (int i = 0; i < 6; i++)
            {
                var members = _fface.PartyMember[Convert.ToByte(i)];

                if (_fface.NPC.ClaimedID(id) == members.ServerID && _fface.NPC.HPPCurrent(id) > 0 && _fface.NPC.Status(id) != Status.Dead1 && _fface.NPC.Status(id) != Status.Dead2)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Is a mosnter rendered?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsRendered(int id)
        {
            byte[] b = _fface.NPC.GetRawNPCData(id, 0x120, 4);
            if (b != null)
                return (BitConverter.ToInt32(b, 0) & 0x200) != 0;
            return false;
        }


        private Dictionary<string, DateTime> CommandsLog = new Dictionary<string, DateTime>();
        private DateTime NextCommandAllowed = DateTime.MinValue;

        /// <summary>
        /// Sends commands to the server, and ensures commands are not spammed.
        /// </summary>
        /// <param name="command">The fully qualified command.</param>
        /// <param name="Delay">How long to lockout other avilities.</param>
        /// <param name="PreventGlobal"></param>
        /// <returns></returns>
        public bool SendCommand(string command, int Delay = 2, bool PreventGlobal = false)
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

                Debug.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Sent Command: " + command);

                _fface.Windower.SendString(command);

                return true;
            }

            return false;
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
        public bool UseAbility(AbilityList ability, int Delay = 2, bool Offensive = false)
        {
            return SendCommand("/ja \"" + ability.ToString().Replace('_', ' ') + "\" " + (Offensive ? "<t>" : "<me>"), Delay);
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
        public bool UseAbility(string ability, AbilityList abilityGroup, int Delay = 2, bool Offensive = false)
        {
            return SendCommand("/ja \"" + ability.ToString().Replace('_', ' ') + "\" " + (Offensive ? "<t>" : "<me>"), Delay);
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
        public bool UseSpell(SpellList spell, int castTime, bool Offensive = false)
        {
            return SendCommand("/ma \"" + spell.ToString().Replace('_', ' ') + "\" " + (Offensive ? "<t>" : "<me>"), castTime);
        }

        public bool UseSpell(string spell, SpellList spellGroup, int castTime, bool Offensive = false)
        {
            return SendCommand("/ma \"" + spell + "\" " + (Offensive ? "<t>" : "<me>"), castTime);
        }

        /// <summary>
        /// Uses a spell from the SpellList enum by target name.
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="castTime"></param>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public bool UseSpell(SpellList spell, int castTime, string targetName)
        {
            return SendCommand("/ma \"" + spell.ToString().Replace('_', ' ') + "\" " + targetName, castTime);
        }

        /// <summary>
        /// Gets an item by it's passed item name and uses it.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="castTime">How long does it take to use this item?</param>
        /// <param name="offensive">Is this item used on me, or my target?</param>
        public bool UseItem(string itemName, int castTime, bool offensive = false)
        {
            // Get the item by name from the Item list.
            Item item = Items.GetItem(itemName);
            // Send the command to use the item.
            return SendCommand("/item \"" + item.Name + "\" " + (offensive ? "<t>" : "<me>"), castTime);
        }

        /// <summary>
        /// Gets an item by it's passed item id and uses it.
        /// </summary>
        /// <param name="itemId">The id of the item.</param>
        /// <param name="castTime">How long does it take to use this item?</param>
        /// <param name="offensive">Is this item used on me, or my target?</param>
        public bool UseItem(ushort itemId, int castTime, bool offensive = false)
        {
            // Get the item by id from the Item list.
            Item item = Items.GetItem(itemId);
            // Send the command to use the item.
            return SendCommand("/item \"" + item.Name + "\" " + (offensive ? "<t>" : "<me>"), castTime);
        }

        /// <summary>
        /// Check if the provided spell is ready to use.
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public bool Ready(SpellList spell)
        {
            if (_fface.Timer.GetSpellRecast(spell) == 0)
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
            if (_fface.Timer.GetAbilityRecast(ability) == 0)// && fface.Player.HasAbility(ability))
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
            foreach (StatusEffect status in _fface.Player.StatusEffects)
            {
                if (status == effect)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if a player has learned a specific spell.
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public bool HasSpell(SpellList spell)
        {
            return _fface.Player.KnowsSpell(spell);
        }

        /// <summary>
        /// Checks if the specified item is found in the player's inventory.
        /// Will get the item through the itemlist via ItemName.
        /// </summary>
        /// <param name="itemName">The Name of the Item.</param>
        /// <returns></returns>
        public bool HasItem(string itemName)
        {
            // Get the item from the item list.
            Item item = Items.GetItem(itemName);
            // Check if the item is found in the player's inventory.
            return _fface.Item.GetInventoryItemCount(item.Id) >= 1;
        }

        /// <summary>
        /// Checks if the specified item is found in the player's inventory.
        /// </summary>
        /// <param name="itemId">The Id of the Item.</param>
        /// <returns></returns>
        public bool HasItem(ushort itemId)
        {
            return _fface.Item.GetInventoryItemCount(itemId) >= 1;
        }


        public static string lastLog = "";
        public static void WriteLog(string log, bool verbose = false)
        {
            if (lastLog != log)
            {
                lastLog = log;
                Program.mainform.uxLog.Invoke((MethodInvoker)delegate
                {
                    Program.mainform.uxLog.Items.Add("[" + DateTime.Now.ToString("HH:mm:ss tt") + "]: " + log);
                    int visibleItems = Program.mainform.uxLog.ClientSize.Height / Program.mainform.uxLog.ItemHeight;
                    Program.mainform.uxLog.TopIndex = Math.Max(Program.mainform.uxLog.Items.Count - visibleItems + 1, 0);
                });
            }
        }
        #endregion
    }
}
