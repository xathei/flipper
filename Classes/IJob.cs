using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipper.Classes
{
    public interface IJob
    {
        void UseRangedClaim();
        void UseClaim();
        void Stagger();
        void UseAbilities();
        void UseSpells();
        void UseHeals();
        void UseWeaponskills();
    }
}
