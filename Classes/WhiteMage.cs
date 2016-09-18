using FFACETools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Flipper.Classes
{
    public class WhiteMage : Jobs
    {
        private Dictionary<string, bool> _hasteStates;
        private List<PartyMember> _partyMembers;
        public Form _settingsForm = new WhiteMageForm();
        
        public override void SettingsForm()
        {
            _settingsForm.

            
        }

        public WhiteMage(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            _hasteStates = new Dictionary<string, bool>();
            _partyMembers = new List<PartyMember>();

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active == true))
            {
                // Add a new PartyMember object to the list of party members.
                _partyMembers.Add(new PartyMember {
                    Name = partyMember.Value.Name,
                    HpCurrent = partyMember.Value.HPCurrent,
                    HpCurrentMax = CalculateMaxHp(partyMember.Value.HPCurrent, partyMember.Value.HPPCurrent)
                });
            }
        }

        public override void UseHeals()
        {
            // Check if the player's SubJob is set to Scholar, is currently not under the effect of Aurorastorm and that the spell is not on cooldown.
            if (_fface.Player.SubJob == Job.SCH && !IsAfflicted(StatusEffect.Aurorastorm) && Ready(SpellList.Aurorastorm))
                UseSpell(SpellList.Aurorastorm, 8);

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active == true))
            {
                // Get the PartyMember object from the current list.
                PartyMember member = _partyMembers.SingleOrDefault(x => x.Name == partyMember.Value.Name);

                // Update the HP values for the object.
                member.HpCurrent = partyMember.Value.HPCurrent;
                member.HpCurrentMax = CalculateMaxHp(partyMember.Value.HPCurrent, partyMember.Value.HPPCurrent);
                member.HpMissing = member.HpCurrentMax - member.HpCurrent;

                // If the party member's HP is equal or less than 20%, cast Cure VI.
                if (partyMember.Value.HPPCurrent <= 20 && Ready(SpellList.Cure_VI))
                    UseSpell(SpellList.Cure_VI, 5, partyMember.Value.Name);

                // If the party member's HP is equal or less than 35%, cast Cure V.
                if (partyMember.Value.HPPCurrent <= 30 && Ready(SpellList.Cure_V))
                    UseSpell(SpellList.Cure_V, 5, partyMember.Value.Name);

                // If the party member's HP is equal or less than 50%, cast Cure IV.
                if (partyMember.Value.HPPCurrent <= 40 && Ready(SpellList.Cure_IV))
                    UseSpell(SpellList.Cure_IV, 5, partyMember.Value.Name);

                // If the party member's HP is equal or less than 75%, cast Cure III.
                if (partyMember.Value.HPPCurrent <= 75 && Ready(SpellList.Cure_III))
                    UseSpell(SpellList.Cure_III, 5, partyMember.Value.Name);
            }

            // Calculate the average HP missing among party members.
            int averageHpMissing = (_partyMembers.Where(x => x.HpMissing > 100).Sum(x => x.HpMissing) / _partyMembers.Where(x => x.HpMissing > 100).Count());

            // Determine what curaga to cast based on the average missing HP.
            if (averageHpMissing >= 960 && Ready(SpellList.Curaga_V))
                UseSpell(SpellList.Curaga_V, 7);
            else if (averageHpMissing >= 690 && Ready(SpellList.Curaga_IV))
                UseSpell(SpellList.Curaga_IV, 7);
            else if (averageHpMissing >= 390 && Ready(SpellList.Curaga_III))
                UseSpell(SpellList.Curaga_III, 7);
            else if (averageHpMissing >= 190 && Ready(SpellList.Curaga_II))
                UseSpell(SpellList.Curaga_II, 7);
            else if (averageHpMissing >= 90 && Ready(SpellList.Curaga))
                UseSpell(SpellList.Curaga, 7);
        }

        public override void UseAbilities()
        {
            // Check to see if the player is currently not under the effect of 'Afflatus Solace' and that the ability is not on cooldown.
            if (!IsAfflicted(StatusEffect.Afflatus_Solace) && Ready(AbilityList.Afflatus_Solace))
                UseAbility(AbilityList.Afflatus_Solace, 2);

            // Check if the player's SubJob is set to Scholar, is currently not under the effect of Light Arts and that the ability is not on cooldown.
            if (_fface.Player.SubJob == Job.SCH && !IsAfflicted(StatusEffect.Light_Arts) && Ready(AbilityList.Light_Arts))
                UseAbility(AbilityList.Light_Arts, 2);
        }

        public override void UseSpells()
        {
            // Check if we have Reraise status.
            if (!IsAfflicted(StatusEffect.Reraise))
            {   
                // Cast Reraise III if ready.
                if (Ready(SpellList.Reraise_III))
                    UseSpell(SpellList.Reraise_III, 10);
                // If Reraise II isn't ready, cast Reraise II.
                else if (Ready(SpellList.Reraise_II))
                    UseSpell(SpellList.Reraise_II, 10);
                // If neither Reraise III and Reraise II are ready, cast Reraise.
                else if (Ready(SpellList.Reraise))
                    UseSpell(SpellList.Reraise, 10);
            }

            // Check if we have Protect status and cast Protectra if not.
            if (!IsAfflicted(StatusEffect.Protect))
                if (!UseSpell((_fface.Player.KnowsSpell(SpellList.Protectra_V) ? SpellList.Protectra_V : SpellList.Protectra_IV), 7))
                    return;

            // Check if we have Shell status and cast Shellra if not.
            if (!IsAfflicted(StatusEffect.Shell))
                if (UseSpell((_fface.Player.KnowsSpell(SpellList.Shellra_V) ? SpellList.Shellra_V : SpellList.Shellra_IV), 7))
                    return;

            // Check if both Protect and Shell are applied before starting the Haste cycle.
            if (IsAfflicted(StatusEffect.Protect) && IsAfflicted(StatusEffect.Shell))
            {
                // Check if the player is not under the effect of Haste and if Haste is not on cooldown.
                if (!IsAfflicted(StatusEffect.Haste) && Ready(SpellList.Haste))
                    // Check if the _hasteStates Dictionary contains more than one entry.
                    if (_hasteStates.Count > 1)
                        // Clear the Dictionary.
                        _hasteStates.Clear();

                // Loop through each active party member in the party list.
                foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active == true))
                    // Check if the Haste Spell is ready to be cast.
                    if (Ready(SpellList.Haste) && _fface.Player.CastCountDown == 0)
                        // Search for the party member's key in the dictionary and check to see if the value is false.
                        if (!_hasteStates.SingleOrDefault(x => x.Key == partyMember.Value.Name).Value)
                            // Cast the spell on the target party member.
                            if (UseSpell(SpellList.Haste, 8, partyMember.Value.Name))
                                // Add the current user to the dictionary and set their haste state to true.
                                _hasteStates.Add(partyMember.Value.Name, true);
            }
        }

        public override void UseWeaponskills()
        {
            SendCommand("/ws \"Hexa Strike\" <t>", 3);
        }

        #region Helper Methods
        /// <summary>
        /// Calculates the Max Hit Points based on the current value and it's percentage value against 100%.
        /// </summary>
        /// <param name="currentHp"></param>
        /// <param name="currentHpPercentage"></param>
        /// <returns></returns>
        private int CalculateMaxHp(int currentHp, int currentHpPercentage)
        {
            decimal divisor = ((decimal)currentHpPercentage) / 100;

            return Convert.ToInt32(Math.Round(currentHp / divisor));
        }
        #endregion
    }
}
