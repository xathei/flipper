using FFACETools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Flipper.Classes.JobSettings;
using Newtonsoft.Json;

namespace Flipper.Classes
{
    public class WhiteMage : Jobs
    {
        private Dictionary<string, bool> _hasteStates;
        private Dictionary<string, bool> _regenStates;
        private List<PartyMember> _partyMembers;
        private WhiteMageForm _settingsForm;
        public WhiteMageSettings WhiteMageSettings = new WhiteMageSettings();
        
        public override void SettingsForm()
        {
            _settingsForm.InitJob(this);
            _settingsForm.Show();
        }

        public WhiteMage(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            _settingsForm = new WhiteMageForm(instance);
            _hasteStates = new Dictionary<string, bool>();
            _regenStates = new Dictionary<string, bool>();
            _partyMembers = new List<PartyMember>();

            Melee = false;

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active))
            {
                // Add a new PartyMember object to the list of party members.
                _partyMembers.Add(new PartyMember
                {
                    Name = partyMember.Value.Name,
                    HpCurrent = partyMember.Value.HPCurrent,
                    HpCurrentMax = CalculateMaxHp(partyMember.Value.HPCurrent, partyMember.Value.HPPCurrent)
                });
            }

            WhiteMageSettings = JsonConvert.DeserializeObject<WhiteMageSettings>(Utilities.GetFileContents(WhiteMageSettings.FolderPath + WhiteMageSettings.FileName));
        }

        public override int MaxDistance()
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

        public override void UseClaim()
        {
            return;
        }

        public override void UseRangedClaim()
        {
            return;
        }

        public override void Engage()
        {
            return;
        }

        public override bool CanStillAttack(int id)
        {
            if (!IsRendered(id))
                return false;

            if (_fface.NPC.HPPCurrent(id) == 0)
                return false;

            if (Math.Abs(Math.Abs(_fface.NPC.PosY(id)) - Math.Abs(_fface.Player.PosY)) > 15)
            {
                return false;
            }

            if (_fface.NPC.Status(id) == Status.Dead1 ||
                _fface.NPC.Status(id) == Status.Dead2)
                return false;

            return true;
        }

        public override bool Position(int id, Monster monster, Combat.Mode mode)
        {
            if (DistanceTo(id) > monster.HitBox && CanStillAttack(id))
            {
                if (mode == Combat.Mode.Meshing && !Combat.IsPositionSafe(_fface.NPC.PosX(id), _fface.NPC.PosZ(id)))
                    return false;

                _fface.Navigator.Reset();
                _fface.Navigator.DistanceTolerance = 4;
                _fface.Navigator.Goto(_fface.NPC.PosX(id), _fface.NPC.PosZ(id), false);

            }

            bool wasLocked = true;
            if (_fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
            {
                var difference = GetSADifference(id);
                while (difference < 66 || difference > 89)
                {
                    if (!_fface.Target.IsLocked)
                    {
                        wasLocked = false;
                        _fface.Windower.SendString("/lockon");
                    }
                    _fface.Windower.SendKey(KeyCode.NP_Number6, true);
                    Thread.Sleep(100);
                    _fface.Windower.SendKey(KeyCode.NP_Number6, false);
                    difference = GetSADifference(id);
                }
                if (!wasLocked && _fface.Target.IsLocked)
                {
                    _fface.Windower.SendString("/lockon");
                }
            }
            return true;
        }

        public override void UseHeals()
        {

        }

        public override void UseAbilities()
        {

        }

        public override void UseSpells()
        {
            // Check to see if the player is currently not under the effect of 'Afflatus Solace' and that the ability is not on cooldown.
            if (!IsAfflicted(StatusEffect.Afflatus_Solace) && Ready(AbilityList.Afflatus_Solace))
                if (UseAbility(AbilityList.Afflatus_Solace))
                    return;

            // Check if the player's SubJob is set to Scholar, is currently not under the effect of Light Arts and that the ability is not on cooldown.
            if (_fface.Player.SubJob == Job.SCH && !IsAfflicted(StatusEffect.Light_Arts) && Ready(AbilityList.Light_Arts))
                if (UseAbility(AbilityList.Light_Arts))
                    return;

            // Check if the player's SubJob is set to Scholar, is currently not under the effect of Aurorastorm and that the spell is not on cooldown.
            if (_fface.Player.SubJob == Job.SCH && !IsAfflicted(StatusEffect.Aurorastorm) && Ready(SpellList.Aurorastorm))
                if (UseSpell(SpellList.Aurorastorm, 8))
                    return;

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active))
            {
                // Get the PartyMember object from the current list.
                PartyMember member = _partyMembers.Single(x => x.Name == partyMember.Value.Name);

                // Update the HP values for the object.
                member.HpCurrent = partyMember.Value.HPCurrent;
                member.HpCurrentMax = CalculateMaxHp(partyMember.Value.HPCurrent, partyMember.Value.HPPCurrent);
                member.HpMissing = member.HpCurrentMax - member.HpCurrent;
            }

            // Calculate the average HP missing among party members.
            int totalHpMissing = _partyMembers.Where(x => x.HpMissing > 100).Sum(x => x.HpMissing);
            int partyMemberCount = _partyMembers.Count(x => x.HpMissing > 100);
            int averageHpMissing = 0;

            // Only set the average hp missing is the total hp missing is greater than 0 and it affects more than 1 party member.
            if (totalHpMissing > 0 && partyMemberCount > 1)
                averageHpMissing = totalHpMissing / partyMemberCount;

            // Determine what curaga to cast based on the average missing HP.
            if (partyMemberCount > 1 && averageHpMissing >= 960 * partyMemberCount && Ready(SpellList.Curaga_V) && WhiteMageSettings.SelfActions.CuragaV)
                UseSpell(SpellList.Curaga_V, 7);
            else if (partyMemberCount > 1 && averageHpMissing >= 690 * partyMemberCount && Ready(SpellList.Curaga_IV) && WhiteMageSettings.SelfActions.CuragaIV)
                UseSpell(SpellList.Curaga_IV, 7);
            else if (partyMemberCount > 1 && averageHpMissing >= 390 * partyMemberCount && Ready(SpellList.Curaga_III) && WhiteMageSettings.SelfActions.CuragaIII)
                UseSpell(SpellList.Curaga_III, 7);
            else if (partyMemberCount > 1 && averageHpMissing >= 190 * partyMemberCount && Ready(SpellList.Curaga_II) && WhiteMageSettings.SelfActions.CuragaII)
                UseSpell(SpellList.Curaga_II, 7);
            else if (partyMemberCount > 1 && averageHpMissing >= 90 * partyMemberCount && Ready(SpellList.Curaga) && WhiteMageSettings.SelfActions.Curaga)
                UseSpell(SpellList.Curaga, 7);

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active))
            {
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

            // Check if Reraise is checked as a self action.
            if (WhiteMageSettings.SelfActions.Reraise)
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

            // Check if Protectra is checked as a self action.
            if (WhiteMageSettings.SelfActions.Protectra)
                // Check if we have Protect status and cast Protectra if not.
                if (!IsAfflicted(StatusEffect.Protect))
                    if (!UseSpell(_fface.Player.KnowsSpell(SpellList.Protectra_V) ? SpellList.Protectra_V : SpellList.Protectra_IV, 7))
                        return;

            // Check if Shellra is checked as a self action.
            if (WhiteMageSettings.SelfActions.Shellra)
                // Check if we have Shell status and cast Shellra if not.
                if (!IsAfflicted(StatusEffect.Shell))
                    if (UseSpell(_fface.Player.KnowsSpell(SpellList.Shellra_V) ? SpellList.Shellra_V : SpellList.Shellra_IV, 7))
                        return;

            // Check if a Bar Elemental Spell was selected.
            if (WhiteMageSettings.Spells.BarElementalSpell != "None")
            {
                // If Barfira was selected.
                if (!IsAfflicted(StatusEffect.Barfire) && WhiteMageSettings.Spells.BarElementalSpell == "Barfira")
                    UseSpell(WhiteMageSettings.Spells.BarElementalSpell, SpellList.Barfira, 5);
                // If Barblizzara was selected.
                else if (!IsAfflicted(StatusEffect.Barblizzard) && WhiteMageSettings.Spells.BarElementalSpell == "Barblizzara")
                    UseSpell(WhiteMageSettings.Spells.BarElementalSpell, SpellList.Barblizzara, 5);
                // If Baraera was selected.
                else if (!IsAfflicted(StatusEffect.Baraero) && WhiteMageSettings.Spells.BarElementalSpell == "Baraera")
                    UseSpell(WhiteMageSettings.Spells.BarElementalSpell, SpellList.Baraera, 5);
                // If Barstonra was selected.
                else if (!IsAfflicted(StatusEffect.Barstone) && WhiteMageSettings.Spells.BarElementalSpell == "Barstonra")
                    UseSpell(WhiteMageSettings.Spells.BarElementalSpell, SpellList.Barstonra, 5);
                // If Barthundra was selected.
                else if (!IsAfflicted(StatusEffect.Barthunder) && WhiteMageSettings.Spells.BarElementalSpell == "Barthundra")
                    UseSpell(WhiteMageSettings.Spells.BarElementalSpell, SpellList.Barthundra, 5);
                // If Barwatera was selected.
                else if (!IsAfflicted(StatusEffect.Barwater) && WhiteMageSettings.Spells.BarElementalSpell == "Barwatera")
                    UseSpell(WhiteMageSettings.Spells.BarElementalSpell, SpellList.Barwatera, 5);
            }

            // Check if a Bar Status Spell was selected.
            if (WhiteMageSettings.Spells.BarStatusSpell != "None")
            {
                // If Baramnesra was selected.
                if (!IsAfflicted(StatusEffect.Baramnesia) && WhiteMageSettings.Spells.BarStatusSpell == "Baramnesra")
                    UseSpell(WhiteMageSettings.Spells.BarStatusSpell, SpellList.Baramnesra, 10);
                // If Barvira was selected.
                else if (!IsAfflicted(StatusEffect.Barvirus) && WhiteMageSettings.Spells.BarStatusSpell == "Barvira")
                    UseSpell(WhiteMageSettings.Spells.BarStatusSpell, SpellList.Barvira, 10);
                // If Barparalyzra was selected.
                else if (!IsAfflicted(StatusEffect.Barparalyze) && WhiteMageSettings.Spells.BarStatusSpell == "Barparalyzra")
                    UseSpell(WhiteMageSettings.Spells.BarStatusSpell, SpellList.Barparalyzra, 10);
                // If Barsilencera was selected.
                else if (!IsAfflicted(StatusEffect.Barsilence) && WhiteMageSettings.Spells.BarStatusSpell == "Barsilencera")
                    UseSpell(WhiteMageSettings.Spells.BarStatusSpell, SpellList.Barsilencera, 10);
                // If Barpetra was selected.
                else if (!IsAfflicted(StatusEffect.Barpetrify) && WhiteMageSettings.Spells.BarStatusSpell == "Barpetra")
                    UseSpell(WhiteMageSettings.Spells.BarStatusSpell, SpellList.Barpetra, 10);
                // If Barpoisonra was selected.
                else if (!IsAfflicted(StatusEffect.Barpoison) && WhiteMageSettings.Spells.BarStatusSpell == "Barpoisonra")
                    UseSpell(WhiteMageSettings.Spells.BarStatusSpell, SpellList.Barpoisonra, 10);
                // If Barblindra was selected.
                else if (!IsAfflicted(StatusEffect.Barblind) && WhiteMageSettings.Spells.BarStatusSpell == "Barblindra")
                    UseSpell(WhiteMageSettings.Spells.BarStatusSpell, SpellList.Barblindra, 10);
                // If Barsleepra was selected.
                else if (!IsAfflicted(StatusEffect.Barsleep) && WhiteMageSettings.Spells.BarStatusSpell == "Barsleepra")
                    UseSpell(WhiteMageSettings.Spells.BarStatusSpell, SpellList.Barsleepra, 10);

            }

            // Check if a Boost-STAT spell was selected.
            if (WhiteMageSettings.Spells.BoostStatSpell != "None")
            {
                // If Boost-AGI was selected.
                if (!IsAfflicted(StatusEffect.AGI_Boost2) && WhiteMageSettings.Spells.BoostStatSpell == "Boost-AGI")
                    UseSpell(WhiteMageSettings.Spells.BoostStatSpell, SpellList.Boost_AGI, 10);
                // If Boost-CHR was selected.
                else if (!IsAfflicted(StatusEffect.CHR_Boost2) && WhiteMageSettings.Spells.BoostStatSpell == "Boost-CHR")
                    UseSpell(WhiteMageSettings.Spells.BoostStatSpell, SpellList.Boost_CHR, 10);
                // If Boost-DEX was selected.
                else if (!IsAfflicted(StatusEffect.DEX_Boost2) && WhiteMageSettings.Spells.BoostStatSpell == "Boost-DEX")
                    UseSpell(WhiteMageSettings.Spells.BoostStatSpell, SpellList.Boost_DEX, 10);
                // If Boost-INT was selected.
                else if (!IsAfflicted(StatusEffect.INT_Boost2) && WhiteMageSettings.Spells.BoostStatSpell == "Boost-INT")
                    UseSpell(WhiteMageSettings.Spells.BoostStatSpell, SpellList.Boost_INT, 10);
                // If Boost-MND was selected.
                else if (!IsAfflicted(StatusEffect.MND_Boost2) && WhiteMageSettings.Spells.BoostStatSpell == "Boost-MND")
                    UseSpell(WhiteMageSettings.Spells.BoostStatSpell, SpellList.Boost_MND, 10);
                // If Boost-STR was selected.
                else if (!IsAfflicted(StatusEffect.STR_Boost2) && WhiteMageSettings.Spells.BoostStatSpell == "Boost-STR")
                    UseSpell(WhiteMageSettings.Spells.BoostStatSpell, SpellList.Boost_STR, 10);
                // If Boost-CHR was selected.
                else if (!IsAfflicted(StatusEffect.VIT_Boost2) && WhiteMageSettings.Spells.BoostStatSpell == "Boost-VIT")
                    UseSpell(WhiteMageSettings.Spells.BoostStatSpell, SpellList.Boost_VIT, 10);
            }

            // Check if the player is not under the effect of Haste and if Haste is not on cooldown.
            if (!IsAfflicted(StatusEffect.Haste) && Ready(SpellList.Haste))
                // Check if the _hasteStates Dictionary contains more than one entry.
                if (_hasteStates.Count > 1)
                    // Clear the Dictionary.
                    _hasteStates.Clear();

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active))
                // Check if the Haste Spell is ready to be cast.
                if (Ready(SpellList.Haste))
                    // Search for the party member's key in the dictionary and check to see if the value is false.
                    if (!_hasteStates.SingleOrDefault(x => x.Key == partyMember.Value.Name).Value
                        && WhiteMageSettings.Characters.Single(x => x.Name == partyMember.Value.Name).CastHasteOn)
                        // Cast the spell on the target party member.
                        if (UseSpell(SpellList.Haste, 8, partyMember.Value.Name))
                            // Add the current user to the dictionary and set their haste state to true.
                            _hasteStates.Add(partyMember.Value.Name, true);

            // Check if the player is not under the effect of Haste and if Haste is not on cooldown.
            if (!IsAfflicted(StatusEffect.Regen) && Ready(SpellList.Regen))
                // Check if the _hasteStates Dictionary contains more than one entry.
                if (_regenStates.Count > 1)
                    // Clear the Dictionary.
                    _regenStates.Clear();

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active))
                // Check if the Haste Spell is ready to be cast.
                if (Ready(SpellList.Regen_IV))
                    // Search for the party member's key in the dictionary and check to see if the value is false.
                    if (!_regenStates.SingleOrDefault(x => x.Key == partyMember.Value.Name).Value
                        && WhiteMageSettings.Characters.Single(x => x.Name == partyMember.Value.Name).CastRegenOn)
                        // Cast the spell on the target party member.
                        if (UseSpell(SpellList.Regen_IV, 8, partyMember.Value.Name))
                            // Add the current user to the dictionary and set their haste state to true.
                            _regenStates.Add(partyMember.Value.Name, true);
        }

        public override void UseWeaponskills()
        {
            SendCommand("/ws \"Hexa Strike\" <t>", 3);
        }

        #region Helper Methods
        private double RadianToDegree(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        private double GetAngleOfLineBetweenTwoPoints(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }

        private double GetSADifference(int i)
        {
            double targetHeading = RadianToDegree(_fface.NPC.PosH(i));
            double lineAngle = GetAngleOfLineBetweenTwoPoints(new PointF { X = _fface.Player.PosX, Y = _fface.Player.PosZ }, new PointF { X = _fface.NPC.PosX(i), Y = _fface.NPC.PosZ(i) });
            double difference = (targetHeading + lineAngle) % 360;

            return difference;
        }

        /// <summary>
        /// Calculates the Max Hit Points based on the current value and it's percentage value against 100%.
        /// </summary>
        /// <param name="currentHp"></param>
        /// <param name="currentHpPercentage"></param>
        /// <returns></returns>
        private static int CalculateMaxHp(int currentHp, int currentHpPercentage)
        {
            if (currentHp == 0 || currentHpPercentage == 0)
                return 0;

            decimal divisor = (decimal)currentHpPercentage / 100;

            return Convert.ToInt32(Math.Round(currentHp / divisor));
        }
        #endregion
    }
}
