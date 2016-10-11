using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    public class Player
    {
        public string Name;
        public JobRole Role;

        public List<StatusEffect> Effects
        {
            get
            {
                lock (_buffs)
                {
                    List<StatusEffect> t = _buffs.Where(x => true).ToList();
                    return t;
                }
            }
        }

        /// <summary>
        /// Please don't access this list.
        /// </summary>
        public List<StatusEffect> _buffs = new List<StatusEffect>();
    }
}
