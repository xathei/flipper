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
        public WhiteMageSelfActions SelfActions { get; set; }
        public List<WhiteMageCharacterActions> CharacterActions { get; set; }
        public WhiteMageSpells Spells { get; set; }

        public WhiteMageSettings()
        {
            FileName = "\\WhiteMageSettings.json";
            Engages = false;
            Role = JobRole.Healer;

            SelfActions = new WhiteMageSelfActions();
            CharacterActions = new List<WhiteMageCharacterActions>();
            Spells = new WhiteMageSpells();
        }
    }

    public class WhiteMageSelfActions : CharacterActions
    {
        public bool Reraise { get; set; }
        public bool Stoneskin { get; set; }
        public bool Blink { get; set; }
        public bool Aquaveil { get; set; }
        public bool Protectra { get; set; }
        public bool Shellra { get; set; }
        public bool CuragaV { get; set; }
        public bool CuragaIV { get; set; }
        public bool CuragaIII { get; set; }
        public bool CuragaII { get; set; }
        public bool Curaga { get; set; }
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
