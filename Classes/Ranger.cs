using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFACETools;
using FlipperD;

namespace Flipper.Classes
{
    class Ranger : Jobs
    {
        public Ranger(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
            Melee = true;
        }

        public override int MaxDistance()
        {
            if (_fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
            {
                return 50;
            }
            else
            {
                return 20;
            }
        }

        public override void UseClaim()
        {
            if (Ready(AbilityList.Shadowbind))
            {
                UseAbility(AbilityList.Shadowbind, 2, true);
            }

            if (_fface.Player.SubJob == Job.WAR)
            {
                if (Ready(AbilityList.Provoke))
                {
                    UseAbility(AbilityList.Provoke, 2, true);
                }
            }
        }

        public override void Engage()
        {
            SendCommand("/attack <t>", 3);
        }

        public override bool Position(int id, Monster monster, Combat.Mode mode)
        {

            if (_content == Content.Ambuscade)
            {
                if (_fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
                {
                    int fail = 900;
                    while (_fface.NPC.Distance(id) < 7 && fail > 0)
                    {
                        if (!_fface.Target.IsLocked)
                        {
                            _fface.Windower.SendString("/lockon");
                            Thread.Sleep(500);
                        }
                        fail--;
                        _fface.Windower.SendKey(KeyCode.NP_Number2, true);
                        Thread.Sleep(100);
                        _fface.Windower.SendKey(KeyCode.NP_Number2, false);
                    }
                    fail = 500;
                    while (_fface.NPC.Distance(id) > 10 && fail > 0)
                    {
                        if (!_fface.Target.IsLocked)
                        {
                            _fface.Windower.SendString("/lockon");
                            Thread.Sleep(500);
                        }
                        fail--;
                        _fface.Windower.SendKey(KeyCode.NP_Number8, true);
                        Thread.Sleep(100);
                        _fface.Windower.SendKey(KeyCode.NP_Number8, false);
                    }
                    if (_fface.Target.IsLocked)
                    {
                        _fface.Windower.SendString("/lockon");
                        Thread.Sleep(500);
                    }
                }
            }
            else
            {
                if (DistanceTo(id) > monster.HitBox && CanStillAttack(id))
                {
                    if (mode == Combat.Mode.Meshing && !Combat.IsPositionSafe(_fface.NPC.PosX(id), _fface.NPC.PosZ(id)))
                        return false;

                    _fface.Navigator.Reset();
                    _fface.Navigator.DistanceTolerance = (double)(monster.HitBox * 0.95);
                    _fface.Navigator.Goto(_fface.NPC.PosX(id), _fface.NPC.PosZ(id), false);
                }

                // move back from the target
                if (DistanceTo(id) < (monster.HitBox * 0.65) && CanStillAttack(id))
                {
                    _fface.Windower.SendKey(KeyCode.NP_Number2, true);
                    Thread.Sleep(50);
                    _fface.Windower.SendKey(KeyCode.NP_Number2, false);

                }
            }
            return true;
        }

        private DateTime _lastShot = DateTime.MinValue;

        public override void UseAbilities()
        {
            var delay = 5;
            try
            {
                delay = Convert.ToInt32(Program.mainform.rngDelay.Value);
            }
            catch (Exception e)
            {
                delay = 5;
            }

            // check ammo
            if (HasItem(21296))
            {
                _fface.Windower.SendString("/equip Ammo \"Chrono Bullet\"");
            }
            else if (HasItem(21331))
            {
                _fface.Windower.SendString("/equip Ammo \"Eminent Bullet\"");
            } 

            if (HasItem(6468) && !IsAfflicted(StatusEffect.Food))
            {
                WriteLog("[KITCHEN] Using Sublime Sushi");
                SendCommand("/item \"Sublime Sushi\" <me>", 4);
            }

            if (_fface.Player.SubJob == Job.WAR)
            {
                // use warcry
                if (!IsAfflicted(StatusEffect.Warcry) && Ready(AbilityList.Warcry))
                    UseAbility(AbilityList.Warcry, 2, false);


                if (Ready(AbilityList.Berserk))
                    UseAbility(AbilityList.Berserk, 2, false);
            }

            if (!IsAfflicted(StatusEffect.Velocity_Shot) && Ready(AbilityList.Velocity_Shot))
            {
                UseAbility(AbilityList.Velocity_Shot, 2, false);
            }
            if (_fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
            {
                if (!IsAfflicted(StatusEffect.Double_Shot) && Ready(AbilityList.Double_Up) && _fface.NPC.HPPCurrent(_fface.Target.ID) > 25)
                {
                    UseAbility(AbilityList.Velocity_Shot, 2, false);
                }

                if (Ready(AbilityList.Sharpshot) && Ready(AbilityList.Barrage))
                {
                    UseAbility(AbilityList.Sharpshot, 2, false);
                    Thread.Sleep(2000);
                    UseAbility(AbilityList.Barrage, 2, false);
                    Thread.Sleep(2000);
                }
            }

            if (_fface.Player.TPCurrent <= 1000)
                SendCommand("/ra <t>", delay);
        }


        public override void UseWeaponskills()
        {
            SendCommand("/ws \"Last Stand\" <t>", 3);
        }

    }
}
