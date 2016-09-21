using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFACETools;

namespace Flipper.Classes
{
    
    public class RuneFencer : Jobs
    {
        private bool wardAlt;
        private bool effusionAlt;
        //private RuneFencerForm settingsForm = new RuneFencerForm();
        public RuneFencer(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
        }
        public override void UseRangedClaim()
        {
            
            if (Ready(SpellList.Flash))
                UseSpell(SpellList.Flash, 6, true);

            // /WAR49 specific abilities.
            if (_fface.Player.SubJob == Job.WAR && _fface.Navigator.DistanceTo(_fface.Target.ID) < 16)
            {
                if (Ready(AbilityList.Provoke))
                    UseAbility(AbilityList.Provoke, 2, true);
            }
        }
        public override void UseClaim()
        {
            if (_fface.Player.SubJob == Job.WAR && DistanceTo(_fface.Target.ID) <= 15.8)
            {
                if (Ready(AbilityList.Provoke))
                    UseAbility(AbilityList.Provoke, 2, true);
            }
        }
        public override void Stagger()
        {

        }
        public override void UseAbilities()
        {
            //keep 3 runes up -- count how many runes and update when effused
            //alternate between vallation and valiance
            //alternate between Rayke and Gambit
            //use lunge when?
            //Keep defender up
            //provoke
            //swordplay
            //when embolden is up use stoneskin with it
            //one for all?
            //use vivacious pulse when afflicted by silence, blind, poison, paralyze, curse, virus

        }
        public override void UseSpells()
        {
            if (Ready(SpellList.Flash))
                UseSpell(SpellList.Flash, 6, true);
            if (Ready(SpellList.Phalanx) && !IsAfflicted(StatusEffect.Phalanx))
                UseSpell(SpellList.Phalanx, 6, false);
            if (Ready(SpellList.Crusade) && !IsAfflicted(StatusEffect.Enmity_Boost))
                UseSpell(SpellList.Crusade, 6, false);
            if (Ready(SpellList.Stoneskin) && !IsAfflicted(StatusEffect.Stoneskin))
                UseSpell(SpellList.Stoneskin, 6, false);
            // JW - StatusEffect.Foil doesn't exist. Cast Foil on yourself and loop through your StatusEffects to see
            //      what the status effect is to see what effect you need to check for.
            //if (Ready(SpellList.Foil) && !IsAfflicted(StatusEffect.Foil))
                //UseSpell(SpellList.Foil, 6, false);
            if (Ready(SpellList.Shock_Spikes) && !IsAfflicted(StatusEffect.Shock_Spikes))
                UseSpell(SpellList.Shock_Spikes, 6, false);

        }
        public override void UseHeals()
        {
            if (_fface.Player.HPPCurrent <= 60)
            {
                if (Ready(SpellList.Regen_IV) && !IsAfflicted(StatusEffect.Regen))
                    UseSpell(SpellList.Regen_IV, 4, false);
            }
            if (_fface.Player.MPPCurrent <= 50)
            {
                if (Ready(SpellList.Refresh) && !IsAfflicted(StatusEffect.Refresh))
                {
                    UseSpell(SpellList.Refresh, 4, false);
                }
            }
        }
        public override void UseWeaponskills()
        {
            if (_fface.Player.TPCurrent >= 2000)
            {
                SendCommand("/ws \"Dimidiation\" <t>", 3, false);

            }
        }
        //void DoHide();
        public override void Engage()
        {
            SendCommand("/attack <t>", 3);
        }
        //void Warp();
        // JW - SpawnTrusts is not virtual in the Jobs.cs therefore it can not be overwritten at the moment.
        //public override void SpawnTrusts()
        //{
        //    /* select which trusts to spawn when lowmanning
        //    SendCommand("/ma \"Some NPC\" <me>");
        //    Thread.Sleep(7000);
        //    */
        //}
        //bool CanStillAttack(int id);
        //bool Position(int id, Monster monster, Combat.Mode mode);
        //void SettingsForm();
        //bool Engages();
    }
}
