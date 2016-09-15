using FFACETools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flipper.Classes
{
    public class BlueMage : Jobs
    {
        public BlueMage(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
        }

        public override void UseRangedClaim()
        {
            _fface.Navigator.FaceHeading(_fface.Target.ID);
            Thread.Sleep(1000);
            SendCommand("/ra <t>", 5);
        }

        public override void UseAbilities()
        {

        }

        public override void UseWeaponskills()
        {
            SendCommand("/ws \"Savage Blade\" <t>", 3);
        }
    }
}
