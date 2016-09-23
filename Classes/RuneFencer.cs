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
        private bool wardAlt = false;
        private bool effusionAlt = false;
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
            if(!IsAfflicted(StatusEffect.Lux, 3) && Ready(AbilityList.Rune_Enchantment))
            {
                UseAbility("Lux", AbilityList.Rune_Enchantment);
            }
            if(IsAfflicted(StatusEffect.Lux, 3))
            {
                if (Ready(AbilityList.Valiance) && !IsAfflicted(StatusEffect.Vallation))
                     UseAbility(AbilityList.Valiance);

                if (Ready(AbilityList.Vallation) && !IsAfflicted(StatusEffect.Valiance))
                    UseAbility(AbilityList.Vallation);

                if (Ready(AbilityList.Lunge))
                    UseAbility(AbilityList.Lunge, 2, true);
            }

            if (Ready(AbilityList.Provoke))
                UseAbility(AbilityList.Provoke, 2, true);

            if (Ready(AbilityList.Swordplay))
                    UseAbility(AbilityList.Swordplay);

            if(Ready(AbilityList.Defender) && _fface.Player.HPPCurrent <= 60)
                    UseAbility(AbilityList.Defender);


            //alternate between Rayke and Gambit

            if (Ready(AbilityList.Embolden) && !IsAfflicted(StatusEffect.Stoneskin) && Ready(SpellList.Stoneskin))
                UseAbility(AbilityList.Embolden);
            //when embolden is up use stoneskin with it
            //one for all?
            //use vivacious pulse when afflicted by silence, blind, poison, paralyze, curse, virus
            if(Ready(AbilityList.Vivacious_Pulse) && ( IsAfflicted(StatusEffect.Silence) ||
                                                       IsAfflicted(StatusEffect.Blindness) ||
                                                       IsAfflicted(StatusEffect.Poison) || 
                                                       IsAfflicted(StatusEffect.Paralysis) || 
                                                       IsAfflicted(StatusEffect.Curse) || 
                                                       IsAfflicted(StatusEffect.Plague) ) )
            {
                UseAbility(AbilityList.Vivacious_Pulse);
            }

        }
        public override void UseSpells()
        {
            if (Ready(SpellList.Stoneskin) && !IsAfflicted(StatusEffect.Stoneskin))
                UseSpell(SpellList.Stoneskin, 6, false);
            if (Ready(SpellList.Flash))
                UseSpell(SpellList.Flash, 6, true);
            if (Ready(SpellList.Phalanx) && !IsAfflicted(StatusEffect.Phalanx))
                UseSpell(SpellList.Phalanx, 6, false);
            if (Ready(SpellList.Crusade) && !IsAfflicted(StatusEffect.Enmity_Boost))
                UseSpell(SpellList.Crusade, 6, false);
            if (Ready(SpellList.Foil) && !IsAfflicted(StatusEffect.Foil))
                UseSpell(SpellList.Foil, 6, false);
            if (Ready(SpellList.Shock_Spikes) && !IsAfflicted(StatusEffect.Shock_Spikes))
                UseSpell(SpellList.Shock_Spikes, 6, false);
            if (Ready(SpellList.Refresh) && _fface.Player.MPPCurrent <= 50 && !IsAfflicted(StatusEffect.Refresh) )
                UseSpell(SpellList.Refresh, 6, false);
            if (Ready(SpellList.Regen_IV) && _fface.Player.HPPCurrent <= 50 && !IsAfflicted(StatusEffect.Regen))
                UseSpell(SpellList.Regen_IV, 6, false);        
            


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
