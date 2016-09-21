using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Flipper.Classes.JobSettings
{
    public class JobSetting
    {
        [JsonIgnoreAttribute]
        public string SettingsFolder = "Settings\\";
        [JsonIgnoreAttribute]
        public string CharacterFolder;
        [JsonIgnoreAttribute]
        public string FileName;

        public bool Engages;
        public Combat.CombatRoutines Routines;
        public JobRole Role;
    }

    public class CharacterActions
    {
        public string Name;
    }

    public class Spells
    {
        
    }
}
