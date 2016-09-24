using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFACETools;
using Flipper.Classes.JobSettings;
using Newtonsoft.Json;

namespace Flipper.Classes
{
    public partial class BardForm : Form
    {
        private readonly FFACE _fface;
        private BardSettings _jobSettings;
        private Bard _bard;

        public BardForm(FFACE fface)
        {
            _fface = fface;

            InitializeComponent();
        }

        public void InitJob(Bard bard)
        {
            _bard = bard;
        }

        public void LoadJobSettings()
        {
            // Initialize the _jsonSettings as a new object.
            _jobSettings = new BardSettings
            {
                CharacterFolder = _fface.Player.Name
            };

            // Check if the .json settings file exists.
            if (Utilities.IsFileValid(_jobSettings.SettingsFolder + _jobSettings.CharacterFolder + _jobSettings.FileName))
                // Get the data from the file and deserialize the data into the _jsonSettings object.
                _jobSettings = JsonConvert.DeserializeObject<BardSettings>(Utilities.GetFileContents(_jobSettings.SettingsFolder + _jobSettings.CharacterFolder + _jobSettings.FileName));

            // Set the character values.
            _jobSettings.CharacterFolder = _fface.Player.Name;
            _jobSettings.SelfActions.Name = _fface.Player.Name;

            // Loop through each row in the DataGridView.
            foreach (DataGridViewRow row in brdGridView.Rows)
            {
                // Get the character data from the settings.
                BardCharacterActions characterAction = _jobSettings.CharacterActions.SingleOrDefault(x => x.Name == row.Cells[0].Value.ToString());

                // Continue the loop if the character is not found.
                if (characterAction == null) continue;

                // Get the haste/regen values and set their checkboxes accordingly.
                DataGridViewCheckBoxCell pianBalladII = (DataGridViewCheckBoxCell) row.Cells[1];
                pianBalladII.Value = characterAction.PianissimoBalladII;
                DataGridViewCheckBoxCell pianBalladIII = (DataGridViewCheckBoxCell) row.Cells[2];
                pianBalladIII.Value = characterAction.PianissimoBalladIII;
            }

            // Check if abilities are set.
            brdCbTroubadour.Checked = _jobSettings.SelfActions.Troubadour;
            brdCbNightingale.Checked = _jobSettings.SelfActions.Nightingale;

            // Check if the first song was defined, otherwise select default.
            brdSongOne.SelectedIndex = !string.IsNullOrEmpty(_jobSettings.Songs.BardSongOne)
                ? brdSongOne.Items.IndexOf(_jobSettings.Songs.BardSongOne)
                : 0;

            // Check if Marcato is set for the first song.
            brdCbMarcatoSongOne.Checked = _jobSettings.Songs.BardSongOneMarcato;

            // Check if the second song was defined, otherwise select default.
            brdSongTwo.SelectedIndex = !string.IsNullOrEmpty(_jobSettings.Songs.BardSongTwo)
                ? brdSongTwo.Items.IndexOf(_jobSettings.Songs.BardSongTwo)
                : 0;

            // Check if Marcato is set for the second song.
            brdCbMarcatoSongTwo.Checked = _jobSettings.Songs.BardSongTwoMarcato;
        }

        public void SetPartyMemberDataRows()
        {
            // Loop through each of the columns (checkboxes).
            foreach (DataGridViewColumn column in brdGridView.Columns)
                // If the column is editable, set the header alignment.
                if (!column.ReadOnly)
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Clear the existing rows.
            brdGridView.Rows.Clear();

            // Loop through the active party members and add them as rows.
            foreach (KeyValuePair<byte, FFACE.PartyMemberTools> partyMember in _fface.PartyMember.Where(x => x.Value.Active))
                brdGridView.Rows.Add(partyMember.Value.Name);
        }

        private void brdFormGridView_SelectionChange(object sendor, EventArgs e)
        {
            // Do not allow cell selection in the grid view.
            brdGridView.ClearSelection();
        }

        private void brdFormSave_Click(object sender, EventArgs e)
        {
            // Loop through each of the rows in the DataGridView.
            foreach (DataGridViewRow row in brdGridView.Rows)
            {
                // Get the character data from the settings.
                BardCharacterActions character = _jobSettings.CharacterActions.SingleOrDefault(x => x.Name == row.Cells[0].Value.ToString());

                // If no character was found.
                if (character == null)
                {
                    // Add a new CharacterAction.
                    _jobSettings.CharacterActions.Add(new BardCharacterActions
                    {
                        Name = row.Cells[0].Value.ToString(),
                        PianissimoBalladII = (bool?)row.Cells[1].Value ?? false,
                        PianissimoBalladIII = (bool?)row.Cells[2].Value ?? false,
                    });
                }
                else
                {
                    // Update the character settings.
                    character.PianissimoBalladII = (bool?)row.Cells[1].Value ?? false;
                    character.PianissimoBalladIII = (bool?)row.Cells[2].Value ?? false;
                }
            }

            // Set the abilities.
            _jobSettings.SelfActions.Troubadour = brdCbTroubadour.Checked;
            _jobSettings.SelfActions.Nightingale = brdCbNightingale.Checked;

            // Set the spell values.
            _jobSettings.Songs.BardSongOne = brdSongOne.SelectedItem.ToString();
            _jobSettings.Songs.BardSongOneMarcato = brdCbMarcatoSongOne.Checked;
            _jobSettings.Songs.BardSongTwo = brdSongTwo.SelectedItem.ToString();
            _jobSettings.Songs.BardSongTwoMarcato = brdCbMarcatoSongTwo.Checked;

            // Serialize the object to a json string.
            string jsonData = JsonConvert.SerializeObject(_jobSettings, Formatting.Indented);

            // Save the data to a .json file.
            Utilities.SaveToFile(_jobSettings.SettingsFolder + _jobSettings.CharacterFolder, _jobSettings.FileName, jsonData);

            // Close the form.
            Close();
        }

        private void brdForm_Shown(object sender, EventArgs e)
        {
            SetPartyMemberDataRows();

            LoadJobSettings();
        }

        private void brdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();

            _bard.BardSettings = _jobSettings;

            e.Cancel = true;
        }
    }
}
