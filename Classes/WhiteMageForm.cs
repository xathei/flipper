﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FFACETools;
using Flipper.Classes.JobSettings;
using Newtonsoft.Json;

namespace Flipper.Classes
{
    public partial class WhiteMageForm : Form
    {
        private readonly FFACE _fface;
        private WhiteMageSettings _jobSettings;
        private WhiteMage _whiteMage;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WhiteMageForm(FFACE fface)
        {
            // Set the local FFACe value.
            _fface = fface;

            // Initialize base form components.
            InitializeComponent();
        }

        public void InitJob(WhiteMage whiteMage)
        {
            // Set a reference to the WhiteMage class.
            _whiteMage = whiteMage;
        }

        public void LoadJobSettings()
        {
            // Initialize the _jsonSettings as a new object.
            _jobSettings = new WhiteMageSettings
            {
                CharacterFolder = _fface.Player.Name
            };

            // Check if the .json settings file exists.
            if (Utilities.IsFileValid(_jobSettings.SettingsFolder + _jobSettings.CharacterFolder + _jobSettings.FileName))
                // Get the data from the file and deserialize the data into the _jsonSettings object.
                _jobSettings = JsonConvert.DeserializeObject<WhiteMageSettings>(Utilities.GetFileContents(_jobSettings.SettingsFolder + _jobSettings.CharacterFolder + _jobSettings.FileName));

            // Set the character values.
            _jobSettings.CharacterFolder = _fface.Player.Name;
            _jobSettings.SelfActions.Name = _fface.Player.Name;

            // Loop through each row in the DataGridView.
            foreach (DataGridViewRow row in whmGridView.Rows)
            {
                // Get the character data from the settings.
                WhiteMageCharacterActions characterAction = _jobSettings.CharacterActions.SingleOrDefault(x => x.Name == row.Cells[0].Value.ToString());

                // Continue the loop if the character is not found.
                if (characterAction == null) continue;

                // Get the haste/regen values and set their checkboxes accordingly.
                DataGridViewCheckBoxCell hastecell = (DataGridViewCheckBoxCell) row.Cells[1];
                hastecell.Value = characterAction.CastHasteOn;
                DataGridViewCheckBoxCell regenCell = (DataGridViewCheckBoxCell) row.Cells[2];
                regenCell.Value = characterAction.CastRegenOn;
            }

            // Check if the BarElemental spell was defined, otherwise select default.
            whmComboBarElemental.SelectedIndex = !string.IsNullOrEmpty(_jobSettings.Spells.BarElementalSpell)
                ? whmComboBarElemental.Items.IndexOf(_jobSettings.Spells.BarElementalSpell)
                : 0;

            // Check if the BarStatus spell was defined, otherwise select default.
            whmComboBarStatus.SelectedIndex = !string.IsNullOrEmpty(_jobSettings.Spells.BarStatusSpell)
                ? whmComboBarStatus.Items.IndexOf(_jobSettings.Spells.BarStatusSpell)
                : 0;

            // Check if the BoostStat spell was defined, otherwise select default.
            whmComboBoostStat.SelectedIndex = !string.IsNullOrEmpty(_jobSettings.Spells.BoostStatSpell)
                ? whmComboBoostStat.Items.IndexOf(_jobSettings.Spells.BoostStatSpell)
                : 0;

            // Set the Self Buffs values.
            whmCbReraise.Checked = _jobSettings.SelfActions.Reraise;
            whmCbStoneskin.Checked = _jobSettings.SelfActions.Stoneskin;
            whmCbBlink.Checked = _jobSettings.SelfActions.Blink;
            whmCbAquaveil.Checked = _jobSettings.SelfActions.Aquaveil;

            // Set the Party Buffs values.
            whmCbProtectra.Checked = _jobSettings.SelfActions.Protectra;
            whmCbShellra.Checked = _jobSettings.SelfActions.Shellra;

            // Set the Curaga values.
            whmCbCuragaV.Checked = _jobSettings.SelfActions.CuragaV;
            whmCbCuragaIV.Checked = _jobSettings.SelfActions.CuragaIV;
            whmCbCuragaIII.Checked = _jobSettings.SelfActions.CuragaIII;
            whmCbCuragaII.Checked = _jobSettings.SelfActions.CuragaII;
            whmCbCuraga.Checked = _jobSettings.SelfActions.Curaga;
        }

        public void SetPartyMemberDataRows()
        {
            // Loop through each of the columns (checkboxes).
            foreach (DataGridViewColumn column in whmGridView.Columns)
                // If the column is editable, set the header alignment.
                if (!column.ReadOnly)
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Clear the existing rows.
            whmGridView.Rows.Clear();

            // Loop through the active party members and add them as rows.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active))
                whmGridView.Rows.Add(partyMember.Value.Name);
        }

        private void whmFormGridView_SelectionChange(object sendor, EventArgs e)
        {
            // Do not allow cell selection in the grid view.
            whmGridView.ClearSelection();
        }

        private void whmFormSave_Click(object sender, EventArgs e)
        {
            // Loop through each of the rows in the DataGridView.
            foreach (DataGridViewRow row in whmGridView.Rows)
            {
                // Get the character data from the settings.
                WhiteMageCharacterActions character = _jobSettings.CharacterActions.SingleOrDefault(x => x.Name == row.Cells[0].Value.ToString());

                // If no character was found.
                if (character == null)
                {
                    // Add a new CharacterAction.
                    _jobSettings.CharacterActions.Add(new WhiteMageCharacterActions
                    {
                        Name = row.Cells[0].Value.ToString(),
                        CastHasteOn = (bool?) row.Cells[1].Value ?? false,
                        CastRegenOn = (bool?) row.Cells[2].Value ?? false,
                    });
                }
                else
                {
                    // Update the character settings.
                    character.CastHasteOn = (bool?) row.Cells[1].Value ?? false;
                    character.CastRegenOn = (bool?) row.Cells[2].Value ?? false;
                }
            }

            // Set the spell values.
            _jobSettings.Spells.BarElementalSpell = whmComboBarElemental.SelectedItem.ToString();
            _jobSettings.Spells.BarStatusSpell = whmComboBarStatus.SelectedItem.ToString();
            _jobSettings.Spells.BoostStatSpell = whmComboBoostStat.SelectedItem.ToString();

            // Set the Self Buffs values.
            _jobSettings.SelfActions.Reraise = whmCbReraise.Checked;
            _jobSettings.SelfActions.Stoneskin = whmCbStoneskin.Checked;
            _jobSettings.SelfActions.Blink = whmCbBlink.Checked;
            _jobSettings.SelfActions.Aquaveil = whmCbAquaveil.Checked;

            // Set the Party Buffs values.
            _jobSettings.SelfActions.Protectra = whmCbProtectra.Checked;
            _jobSettings.SelfActions.Shellra = whmCbShellra.Checked;

            // Set the Curaga values.
            _jobSettings.SelfActions.CuragaV = whmCbCuragaV.Checked;
            _jobSettings.SelfActions.CuragaIV = whmCbCuragaIV.Checked;
            _jobSettings.SelfActions.CuragaIII = whmCbCuragaIII.Checked;
            _jobSettings.SelfActions.CuragaII = whmCbCuragaII.Checked;
            _jobSettings.SelfActions.Curaga = whmCbCuraga.Checked;

            // Serialize the object to a json string.
            string jsonData = JsonConvert.SerializeObject(_jobSettings, Formatting.Indented);

            // Save the data to a .json file.
            Utilities.SaveToFile(_jobSettings.SettingsFolder + _jobSettings.CharacterFolder, _jobSettings.FileName, jsonData);

            // Close the form.
            Close();
        }

        private void whmForm_Shown(object sender, EventArgs e)
        {
            // Set the PartyMember rows.
            SetPartyMemberDataRows();

            // Load the saved json settings.
            LoadJobSettings();
        }

        private void whmForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hide the form.
            Hide();

            // Set the Job Settings to the WhiteMage class object.
            _whiteMage.WhiteMageSettings = _jobSettings;

            // Cancel the disposition event.
            e.Cancel = true;
        }
    }
}
