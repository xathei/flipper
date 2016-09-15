﻿using FFACETools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipper.Classes
{
    class BlueMage:Jobs
    {
        private FFACE _fface;
        private Content _content;
        public BlueMage(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
        }

        public override void UseAbilities()
        {
            if (_content == Content.Dynamis)
            {
                if(Ready(AbilityList.Unbridled_Learning))
                {
                    UseAbility(AbilityList.Unbridled_Learning, 2, false);
                }

              
            }
        }
    }
}