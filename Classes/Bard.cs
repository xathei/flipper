using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFACETools;
using Flipper.Classes.JobSettings;
using Newtonsoft.Json;

namespace Flipper.Classes
{
    public class Bard : Jobs
    {
        private BardForm _settingsForm;

        public BardSettings BardSettings;

        public override void SettingsForm()
        {
            _settingsForm.InitJob(this);
            _settingsForm.Show();
        }

        public Bard(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            _settingsForm = new BardForm(instance);

            BardSettings = new BardSettings
            {
                CharacterFolder = _fface.Player.Name
            };

            Melee = false;

            // Check if the .json settings file exists.
            if (Utilities.IsFileValid(BardSettings.SettingsFolder + BardSettings.CharacterFolder + BardSettings.FileName))
                BardSettings = JsonConvert.DeserializeObject<BardSettings>(Utilities.GetFileContents(BardSettings.SettingsFolder + BardSettings.CharacterFolder + BardSettings.FileName));
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
            if (_content == Content.Ambuscade && _fface.Player.Zone != Zone.Maquette_Abdhaljs_Legion)
                return;

            // Keep haste on self.
            if (_fface.Player.SubJob == Job.WHM)
                if (!IsAfflicted(StatusEffect.Haste) && Ready(SpellList.Haste))
                    UseSpell(SpellList.Haste, 5);

            // Check if troubadour is needed.
            if (BardSettings.SelfActions.Troubadour && Ready(AbilityList.Troubadour))
                UseAbility(AbilityList.Troubadour, 5);

            // Check if Nightingale is needed.
            if (BardSettings.SelfActions.Nightingale && Ready(AbilityList.Nightingale))
                UseAbility(AbilityList.Nightingale, 5);

            // Check if the first song was selected.
            if (BardSettings.Songs.BardSongOne != "None")
            {
                // Check if the first song is supposed to be marcato'd.
                if (BardSettings.Songs.BardSongOneMarcato)
                    if (Ready(AbilityList.Marcato))
                        UseAbility(AbilityList.Marcato);

                // If Valor Minuet V was selected.
                if (!IsAfflicted(StatusEffect.Minuet) && BardSettings.Songs.BardSongOne == "Valor Minuet V")
                    UseSpell(BardSettings.Songs.BardSongOne, SpellList.Valor_Minuet_V, 12);
                // If Valor Minuet IV was selected.
                else if (!IsAfflicted(StatusEffect.Minuet) && BardSettings.Songs.BardSongOne == "Valor Minuet IV")
                    UseSpell(BardSettings.Songs.BardSongOne, SpellList.Valor_Minuet_IV, 12);
                // If Victory March was selected.
                else if (!IsAfflicted(StatusEffect.March) && BardSettings.Songs.BardSongOne == "Victory March")
                    UseSpell(BardSettings.Songs.BardSongOne, SpellList.Victory_March, 12);
                // If Advancing March was selected.
                else if (!IsAfflicted(StatusEffect.March) && BardSettings.Songs.BardSongOne == "Advancing March")
                    UseSpell(BardSettings.Songs.BardSongOne, SpellList.Advancing_March, 12);
                // If Blade Madrigal was selected.
                else if (!IsAfflicted(StatusEffect.Madrigal) && BardSettings.Songs.BardSongOne == "Blade Madrigal")
                    UseSpell(BardSettings.Songs.BardSongOne, SpellList.Blade_Madrigal, 12);
                // If Sword Madrigal was selected.
                else if (!IsAfflicted(StatusEffect.Madrigal) && BardSettings.Songs.BardSongOne == "Sword Madrigal")
                    UseSpell(BardSettings.Songs.BardSongOne, SpellList.Sword_Madrigal, 12);
                // If Mages Ballad III was selected.
                else if (!IsAfflicted(StatusEffect.Ballad) && BardSettings.Songs.BardSongOne == "Mages Ballad III")
                    UseSpell(BardSettings.Songs.BardSongOne, SpellList.Mages_Ballad_III, 12);
                // If Mages Ballad II was selected.
                else if (!IsAfflicted(StatusEffect.Ballad) && BardSettings.Songs.BardSongOne == "Mages Ballad II")
                    UseSpell(BardSettings.Songs.BardSongOne, SpellList.Mages_Ballad_II, 12);
            }

            // Check if the second song was selected.
            if (BardSettings.Songs.BardSongTwo != "None")
            {
                // Check if the second song is supposed to be marcato'd.
                if (BardSettings.Songs.BardSongTwoMarcato)
                    if (Ready(AbilityList.Marcato))
                        UseAbility(AbilityList.Marcato);

                // If Valor Minuet V was selected.
                if (!IsAfflicted(StatusEffect.Minuet) && BardSettings.Songs.BardSongTwo == "Valor Minuet V")
                    UseSpell(BardSettings.Songs.BardSongTwo, SpellList.Valor_Minuet_V, 12);
                // If Valor Minuet IV was selected.
                else if (!IsAfflicted(StatusEffect.Minuet) && BardSettings.Songs.BardSongTwo == "Valor Minuet IV")
                    UseSpell(BardSettings.Songs.BardSongTwo, SpellList.Valor_Minuet_IV, 12);
                // If Victory March was selected.
                else if (!IsAfflicted(StatusEffect.March) && BardSettings.Songs.BardSongTwo == "Victory March")
                    UseSpell(BardSettings.Songs.BardSongTwo, SpellList.Victory_March, 12);
                // If Advancing March was selected.
                else if (!IsAfflicted(StatusEffect.March) && BardSettings.Songs.BardSongTwo == "Advancing March")
                    UseSpell(BardSettings.Songs.BardSongTwo, SpellList.Advancing_March, 12);
                // If Blade Madrigal was selected.
                else if (!IsAfflicted(StatusEffect.Madrigal) && BardSettings.Songs.BardSongTwo == "Blade Madrigal")
                    UseSpell(BardSettings.Songs.BardSongTwo, SpellList.Blade_Madrigal, 12);
                // If Sword Madrigal was selected.
                else if (!IsAfflicted(StatusEffect.Madrigal) && BardSettings.Songs.BardSongTwo == "Sword Madrigal")
                    UseSpell(BardSettings.Songs.BardSongTwo, SpellList.Sword_Madrigal, 12);
                // If Mages Ballad III was selected.
                else if (!IsAfflicted(StatusEffect.Ballad) && BardSettings.Songs.BardSongTwo == "Mages Ballad III")
                    UseSpell(BardSettings.Songs.BardSongTwo, SpellList.Mages_Ballad_III, 12);
                // If Mages Ballad II was selected.
                else if (!IsAfflicted(StatusEffect.Ballad) && BardSettings.Songs.BardSongTwo == "Mages Ballad II")
                    UseSpell(BardSettings.Songs.BardSongTwo, SpellList.Mages_Ballad_II, 12);
            }

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active))
            {
                if (!Ready(SpellList.Mages_Ballad_III) && !Ready(AbilityList.Pianissimo)) continue;

                // Get the Character Data from the WhiteMageSettings.
                BardCharacterActions character = BardSettings.CharacterActions.SingleOrDefault(x => x.Name == partyMember.Value.Name);

                // Check if the character was set.
                if (character == null) continue;

                if (!character.PianissimoBalladIII) continue;

                if (Ready(AbilityList.Pianissimo))
                    UseAbility(AbilityList.Pianissimo, 4);

                UseSpell("Mage's Ballad III", SpellList.Mages_Ballad_III, 12, character.Name);
            }

            // Loop through each active party member in the party list.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active))
            {
                if (!Ready(SpellList.Mages_Ballad_II) && !Ready(AbilityList.Pianissimo)) continue;

                // Get the Character Data from the WhiteMageSettings.
                BardCharacterActions character = BardSettings.CharacterActions.SingleOrDefault(x => x.Name == partyMember.Value.Name);

                // Check if the character was set.
                if (character == null) continue;

                // Search for the party member's key in the dictionary and check to see if the value is false.
                if (!character.PianissimoBalladII) continue;

                if (Ready(AbilityList.Pianissimo))
                    UseAbility(AbilityList.Pianissimo, 4);

                UseSpell("Mage's Ballad II", SpellList.Mages_Ballad_II, 12, character.Name);
            }
        }

        public override void UseWeaponskills()
        {

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
        private int CalculateMaxHp(int currentHp, int currentHpPercentage)
        {
            if (currentHp == 0 || currentHpPercentage == 0)
                return 0;

            decimal divisor = (decimal)currentHpPercentage / 100;

            return Convert.ToInt32(Math.Round(currentHp / divisor));
        }
        #endregion
    }
}
