using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        void DoHide();
        void Engage();
        void Warp();
        void SpawnTrusts();
        bool CanStillAttack(int id);
        bool Position(int id, Monster monster, Combat.Mode mode);
        void SettingsForm();
        bool Engages();
    }
}
