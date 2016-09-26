using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipper.Classes.JobSettings
{
    public class BardSettings : JobSetting
    {
        public List<BardCharacterActions> CharacterActions { get; set; }

        public BardSelfActions SelfActions { get; set; }

        public BardSongs Songs { get; set; }

        public BardSettings()
        {
            FileName = "\\BardSettings.json";
            Engages = false;
            Role = JobRole.Support;

            CharacterActions = new List<BardCharacterActions>();
            SelfActions = new BardSelfActions();
            Songs = new BardSongs();
        }
    }

    public class BardSelfActions : CharacterActions
    {
        public bool Perform { get; set; }

        public bool Troubadour { get; set; }

        public bool Nightingale { get; set; }
    }

    public class BardCharacterActions : CharacterActions
    {
        public bool PianissimoBalladII { get; set; }

        public bool PianissimoBalladIII { get; set; }
    }

    public class BardSongs : Spells
    {
        public string BardSongOne { get; set; }

        public bool BardSongOneMarcato { get; set; }

        public string BardSongTwo { get; set; }

        public bool BardSongTwoMarcato { get; set; }

        public string BardSongThree { get; set; }

        public bool BardSongThreeMarcato { get; set; }

        public string BardSongFour { get; set; }

        public bool BardSongFourMarcato { get; set; }
    }
}
