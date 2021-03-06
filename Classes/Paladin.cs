﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;

namespace Flipper.Classes
{
    public class Paladin : Jobs
    {
        public double RangedDistance = 20;

        public Paladin(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            Melee = true;
        }


        public override bool CanStun()
        {
           // if (Ready(AbilityList.Shield_Bash))
             //   return true;

            //if (_fface.Player.TPCurrent > 1000)
            //    return true;

            return false;
        }

        public override void DoStun()
        {
            if (Ready(AbilityList.Shield_Bash))
            {
                UseAbility(AbilityList.Shield_Bash, 2, true);
            }
            else if (_fface.Player.TPCurrent > 1000)
            {
                SendCommand("/ws \"Flat Blade\" <t>", 2);
            }
        }

        public override void Engage()
        {
            SendCommand("/attack <t>", 3);
        }

        /// <summary>
        /// Abilities and Spells to use when trying to claim while standing still from a distance
        /// </summary>
        public override void UseRangedClaim()
        {
            // Standard PLD abilities
            if (Ready(SpellList.Flash))
                UseSpell(SpellList.Flash, 6, true);

            // /WAR49 specific abilities.
            if (_fface.Player.SubJob == Job.WAR && _fface.Navigator.DistanceTo(_fface.Target.ID) < 16)
            {
                if (Ready(AbilityList.Provoke))
                    UseAbility(AbilityList.Provoke, 2, true);
            }
        }

        /// <summary>
        /// Attempts to claim while running towards the mob
        /// </summary>
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
            if (HasItem(6468) && !IsAfflicted(StatusEffect.Food))
            {
                //WriteLog("[KITCHEN] Using Sublime Sushi");
                SendCommand("/item \"Sublime Sushi\" <me>", 4);
            }

            if (_fface.Player.HPPCurrent > 80 && PDTSet)
            {
                PDTSet = false;
                _fface.Windower.SendString("//gs c reset defense");
            }

            if (_fface.Player.SubJob == Job.WAR)
            {
                // use warcry
                if (!IsAfflicted(StatusEffect.Warcry) && Ready(AbilityList.Warcry))
                    UseAbility(AbilityList.Warcry, 2, false);

                if ((_content == Content.Ambuscade && _fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion) || _content != Content.Ambuscade)
                    if (Ready(AbilityList.Provoke))
                        UseAbility(AbilityList.Provoke, 2, true);

            }

            if (Ready(AbilityList.Shield_Bash))
                UseAbility(AbilityList.Shield_Bash, 3, true);


        }

        /// <summary>
        /// Determines if the character is still engaged and can still attack their target.
        /// Called periodically during combat.
        /// </summary>
        /// <param name="id">The ID of your target</param>
        public override bool CanStillAttack(int id)
        {

            if (!IsRendered(id))
            {
                //WriteLog("[STOP!] The target isn't rendered.");
                return false;
            }


            //if (_fface.NPC.IsClaimed(id) && !PartyHasHate(id) && _fface.Player.Status != Status.Fighting)
            //{
            //    //WriteLog("[STOP!] The target is claimed to someone else.");
            //    return false;
            //}

            // Skip if the mob more than 5 yalms above or below us
            if (Math.Abs(Math.Abs(_fface.NPC.PosY(id)) - Math.Abs(_fface.Player.PosY)) > 15)
            {
                WriteLog("[STOP!] The target is too far above or below.");
                return false;
            }

            // Skip if the NPC's HP is 0
            if (_fface.NPC.HPPCurrent(id) == 0)
            {
                //WriteLog("[STOP!] Target HP is 0 :(");
                return false;
            }

            if (DistanceTo(id) > MaxDistance() && _fface.Player.Status == Status.Fighting)
            {
                //WriteLog($"[STOP!] Distance: {DistanceTo(id)} > {MaxDistance()}");
                return false;
            }

            return true;
        }

        private bool PDTSet = false;

        public override void UseHeals()
        {
            if (_fface.Player.HPPCurrent <= 75)
            {
                if (Ready(AbilityList.Two_Hour) && !IsAfflicted(StatusEffect.Sentinel))
                    UseAbility("Invincible", AbilityList.Two_Hour, 2, false);

            }
            if (_fface.Player.HPPCurrent <= 30)
            {
                if (Ready(AbilityList.Sentinel) && !IsAfflicted(StatusEffect.Invincible))
                    UseAbility(AbilityList.Sentinel, 2, false);
            }
            if (_fface.Player.HPPCurrent <= 60)
            {
                if (!PDTSet)
                {
                    _fface.Windower.SendString("//gs c activate PhysicalDefense");
                    PDTSet = true;
                }

                if (_fface.Player.MPCurrent >= 88 && Ready(SpellList.Cure_IV))
                    UseSpell(SpellList.Cure_IV, 8, false);

                else if (_fface.Player.MPCurrent >= 46 && Ready(SpellList.Cure_III))
                    UseSpell(SpellList.Cure_III, 8, false);
            }
        }

        public override void UseSpells()
        {
            if ((_content == Content.Ambuscade && _fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion) || _content != Content.Ambuscade)
                if (Ready(SpellList.Flash))
                    UseSpell(SpellList.Flash, 5, true);

            if (Ready(SpellList.Reprisal))
                UseSpell(SpellList.Reprisal, 5, false);

            if ((_content == Content.Ambuscade && _fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion) || _content != Content.Ambuscade)
                if (Ready(SpellList.Phalanx) && !IsAfflicted(StatusEffect.Phalanx))
                    UseSpell(SpellList.Phalanx, 6, false);

            if ((_content == Content.Ambuscade && _fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion) || _content != Content.Ambuscade)
                if (Ready(SpellList.Crusade) && !IsAfflicted(StatusEffect.Enmity_Boost))
                    UseSpell(SpellList.Crusade, 6, false);

            if (!IsAfflicted(StatusEffect.Enlight))
                SendCommand("/ma \"Enlight II\" <me>", 6);
        }

        public override void UseWeaponskills()
        {
            if (_fface.Player.MPCurrent <= 200)
            {
                if (Ready(AbilityList.Chivalry))
                {
                    UseAbility(AbilityList.Chivalry, 3, false);
                }
            }

            if (_fface.Player.Name == "Lawaan" || _fface.Player.Name == "Udoopudooo" || _fface.Player.Name == "Dirtaru")
            {
                SendCommand("/ws \"Savage Blade\" <t>", 3, false);
            }

            if (IsAfflicted(StatusEffect.Aftermath_lvl3) && _fface.Player.TPCurrent >= 1000)
            {
                SendCommand("/ws \"Savage Blade\" <t>", 3, false);

            }
            if (!IsAfflicted(StatusEffect.Aftermath_lvl3) && _fface.Player.TPCurrent == 3000)
            {
                SendCommand("/ws \"Atonement\" <t>", 3, false);
            }
        }

        #region Add other methods here that aren't overrides, such as calculating finishing moves, etc

        #endregion
    }
}
