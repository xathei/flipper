using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

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
        void MemberGainEffect(string name, JobRole role, StatusEffect effect);
        void MemberLoseEffect(string name, JobRole role, StatusEffect effect);
        bool InBattle();
        void SetBattle(bool battle);
        void TrackBuffs(bool track);
        bool Tracking();

        // This event is fired when the user gains a status effect
        event Jobs.GainEffectHandler GainEffect;
        // This event is fired when the user loses a status effect.
        event Jobs.LoseEffectHandler LoseEffect;
    }
}
