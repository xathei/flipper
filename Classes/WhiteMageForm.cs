using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFACETools;
using Flipper.Classes.JobSettings;
using Flipper.Properties;
using Newtonsoft.Json;
using Trust;

namespace Flipper.Classes
{
    public partial class WhiteMageForm : Form
    {
        private readonly FFACE _fface;
        private WhiteMageSettings _jobSettings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WhiteMageForm(FFACE fface)
        {
            _fface = fface;

            // Initialize base form components.
            InitializeComponent();
        }

        public void LoadJsonSettings()
        {
            // Initialize the _jsonSettings as a new object.
            _jobSettings = new WhiteMageSettings();

            // Check if the .json settings file exists.
            if (Utilities.IsFileValid(_jobSettings.FolderPath + _jobSettings.FileName))
                // Get the data from the file and deserialize the data into the _jsonSettings object.
                _jobSettings = JsonConvert.DeserializeObject<WhiteMageSettings>(Utilities.GetFileContents(_jobSettings.FolderPath + _jobSettings.FileName));

            // Loop through each row in the DataGridView.
            foreach (DataGridViewRow row in whmGridView.Rows)
            {
                // Get the character data from the settings.
                WhiteMageCharacterActions character = _jobSettings.Characters.SingleOrDefault(x => x.Name == row.Cells[0].Value.ToString());

                // Continue the loop if the character is not found.
                if (character == null) continue;

                // Get the haste/regen values and set their checkboxes accordingly.
                DataGridViewCheckBoxCell hastecell = (DataGridViewCheckBoxCell) row.Cells[1];
                DataGridViewCheckBoxCell regenCell = (DataGridViewCheckBoxCell) row.Cells[2];
                hastecell.Value = character.CastHasteOn;
                regenCell.Value = character.CastRegenOn;
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
            whmGridView.ClearSelection();
        }

        private void whmFormSave_Click(object sender, EventArgs e)
        {
            // Loop through each of the rows in the DataGridView.
            foreach (DataGridViewRow row in whmGridView.Rows)
            {
                // Get the character data from the settings.
                WhiteMageCharacterActions character = _jobSettings.Characters.SingleOrDefault(x => x.Name == row.Cells[0].Value.ToString());

                // If no character was found.
                if (character == null)
                {
                    // Add a new CharacterAction.
                    _jobSettings.Characters.Add(new WhiteMageCharacterActions
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

            // Serialize the object to a json string.
            string jsonData = JsonConvert.SerializeObject(_jobSettings);
            
            // Save the data to a .json file.
            Utilities.SaveToFile(_jobSettings.FolderPath, _jobSettings.FileName, jsonData);
        }

        private void whmForm_Shown(object sender, EventArgs e)
        {
            // Set the PartyMember rows.
            SetPartyMemberDataRows();

            // Load the saved json settings.
            LoadJsonSettings();
        }

        private void whmForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hide the form.
            Hide();
            // Cancel the disposition event.
            e.Cancel = true;
        }
    }
}
