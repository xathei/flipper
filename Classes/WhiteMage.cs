using FFACETools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipper.Classes
{
    public class WhiteMage : Jobs
    {
        private Dictionary<int, bool> _hasteStates;

        public WhiteMage(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            _hasteStates = new Dictionary<int, bool>();
        }

        public override void UseHeals()
        {
            // Check to see if the player is currently not under the effect of 'Afflatus Solace' and that the ability is not on cooldown.
            if (!IsAfflicted(StatusEffect.Afflatus_Solace) && Ready(AbilityList.Afflatus_Solace))
                UseAbility(AbilityList.Afflatus_Solace, 2);

            // Check if the player's SubJob is set to Scholar, is currently not under the effect of Light Arts and that the ability is not on cooldown.
            if (_fface.Player.SubJob == Job.SCH && !IsAfflicted(StatusEffect.Light_Arts) && Ready(AbilityList.Light_Arts))
                UseAbility(AbilityList.Light_Arts, 2);

            // Check if the player's SubJob is set to Scholar, is currently not under the effect of Aurorastorm and that the spell is not on cooldown.
            if (_fface.Player.SubJob == Job.SCH && !IsAfflicted(StatusEffect.Aurorastorm) && Ready(SpellList.Aurorastorm))
                UseSpell(SpellList.Aurorastorm, 8);

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active == true))
            {

                // If the party member's HP is equal or less than 20%, cast Cure VI.
                if (partyMember.Value.HPPCurrent <= 30 && Ready(SpellList.Cure_VI))
                    UseSpell(SpellList.Cure_VI, 4, partyMember.Value.Name);

                // If the party member's HP is equal or less than 30%, cast Cure V.
                if (partyMember.Value.HPPCurrent <= 30 && Ready(SpellList.Cure_V))
                    UseSpell(SpellList.Cure_V, 4, partyMember.Value.Name);

                // If the party member's HP is equal or less than 40%, cast Cure IV.
                if (partyMember.Value.HPPCurrent <= 40 && Ready(SpellList.Cure_IV))
                    UseSpell(SpellList.Cure_IV, 4, partyMember.Value.Name);

                // If the party member's HP is equal or less than 55%, cast Cure III.
                if (partyMember.Value.HPPCurrent <= 55 && Ready(SpellList.Cure_III))
                    UseSpell(SpellList.Cure_III, 4, partyMember.Value.Name);

                // If the party member's HP is equal or less than 75%, cast Cure II.
                if (partyMember.Value.HPPCurrent <= 75 && Ready(SpellList.Cure_II))
                    UseSpell(SpellList.Cure_II, 3, partyMember.Value.Name);

                // If the party member's HP is equal or less than 90%, cast Cure II.
                if (partyMember.Value.HPPCurrent <= 90 && Ready(SpellList.Cure))
                    UseSpell(SpellList.Cure, 3, partyMember.Value.Name);
            }
        }

        public override void UseAbilities()
        {

        }

        public override void UseSpells()
        {
            // Check if we have Reraise status and cast if not.
            if (!IsAfflicted(StatusEffect.Reraise))
                UseSpell(SpellList.Reraise_III, 8);

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active == true))
            {
                // Check if we're hitting the player and the player is not under the effect of Haste.
                if (partyMember.Value.Name == _fface.Player.Name && !IsAfflicted(StatusEffect.Haste))
                    // Clear the dictionary.
                    _hasteStates.Clear();

                // Check if the Haste Spell is ready to be cast.
                if (Ready(SpellList.Haste))
                {
                    // Search for the party member's key in the dictionary and check to see if the value is false.
                    if (!_hasteStates.SingleOrDefault(x => x.Key == partyMember.Key).Value)
                    {
                        // Cast the spell on the target party member.
                        UseSpell(SpellList.Haste, 7, partyMember.Value.Name);

                        // Add the current user to the dictionary and set their haste state to true.
                        _hasteStates.Add(partyMember.Key, true);
                    }
                }
            }
        }

        public override void UseWeaponskills()
        {
            SendCommand("/ws \"Hexa Strike\" <t>", 3);
        }
    }
}
