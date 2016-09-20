using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipper.Classes.JobSettings
{
    public class WhiteMageSettings : JobSetting
    {
        public List<WhiteMageCharacterActions> Characters { get; set; }
        public WhiteMageSpells Spells { get; set; }

        public WhiteMageSettings()
        {
            FolderPath = "assets\\json\\";
            FileName = "whmsettings.json";

            Characters = new List<WhiteMageCharacterActions>();
            Spells = new WhiteMageSpells();
        }
    }

    public class WhiteMageCharacterActions : CharacterActions
    {
        public bool CastHasteOn { get; set; }
        public bool CastRegenOn { get; set; }
    }

    public class WhiteMageSpells : Spells
    {
        public string BarElementalSpell { get; set; }
        public string BarStatusSpell { get; set; }
        public string BoostStatSpell { get; set; }
    }
}
