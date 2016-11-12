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
    public class Thief : Jobs
    {
        private int _bullyTimer;
        private ThiefForm settingsForm = new ThiefForm();

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


        public Thief(FFACE instance, Content content)
        {
            _content = content;
            _fface = instance;
        }

        public override void SettingsForm()
        {
            settingsForm.Show();
            settingsForm.PopulateUI("derp");
        }

        public override void UseRangedClaim()
        {
            _fface.Navigator.FaceHeading(_fface.Target.ID);
            Thread.Sleep(2000);
            SendCommand("/ra <t>", 5);
        }

        public override void UseClaim()
        {
            // if we have access to warrior abilities, let's try them first.
            if (_fface.Player.SubJob == Job.WAR && _fface.Player.Zone != Zone.Maquette_Abdhaljs_Legion)
            {
                if (Ready(AbilityList.Provoke))
                    UseAbility(AbilityList.Provoke, 2, true);
            }

            // then let's try bully
            if (Ready(AbilityList.Bully) && _fface.NPC.Distance(_fface.Target.ID) < 6.9)
                UseAbility(AbilityList.Bully, 2, true);

        }


        public override void Stagger()
        {
            
        }

        private double GetAngleOfLineBetweenTwoPoints(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private double GetSADifference(int i)
        {
            var TargetHeading = (int)(_fface.Navigator.PosHToDegrees(_fface.NPC.PosH(i)));
            var MyHeading = (int)(_fface.Navigator.GetPlayerPosHInDegrees());
            double targetHeading = RadianToDegree(_fface.NPC.PosH(i));

            double lineAngle = GetAngleOfLineBetweenTwoPoints(new PointF { X = _fface.Player.PosX, Y = _fface.Player.PosZ }, new PointF { X = _fface.NPC.PosX(i), Y = _fface.NPC.PosZ(i) });
            double difference = (targetHeading + lineAngle) % 360;

            return difference;
        }

        public override void UseAbilities()
        {
            if (HasItem(6468) && !IsAfflicted(StatusEffect.Food))
            {
                WriteLog("[KITCHEN] Using Sublime Sushi");
                SendCommand("/item \"Sublime Sushi\" <me>", 4);
            }

            // stand behind the mob.
            if (_content == Content.Ambuscade && _fface.Player.Zone == Zone.Maquette_Abdhaljs_Legion)
            {
                while (Math.Abs(GetSADifference(_fface.Target.ID)) > 10)
                {
                    _fface.Windower.SendKey(KeyCode.NP_Number4, true);
                    Thread.Sleep(50);
                    _fface.Windower.SendKey(KeyCode.NP_Number4, false);
                }
            }

            //if (Ready(AbilityList.Bully))
            //    UseAbility(AbilityList.Bully, 2, true);

            if (Ready(AbilityList.Conspirator))
                UseAbility(AbilityList.Conspirator, 2, false);

            // check for war abilities
            if (_fface.Player.SubJob == Job.WAR)
            {
                // use warcry
                if (!IsAfflicted(StatusEffect.Warcry) && Ready(AbilityList.Warcry))
                    UseAbility(AbilityList.Warcry, 2, false);

                if (!IsAfflicted(StatusEffect.Aggressor) && Ready(AbilityList.Aggressor))
                    UseAbility(AbilityList.Aggressor, 2, false);

                if (Ready(AbilityList.Berserk))
                   UseAbility(AbilityList.Berserk, 2, false);
            }

            // check for dnc abiliteis
            if (_fface.Player.SubJob == Job.DNC)
            {
                if (IsAfflicted(StatusEffect.Haste_Samba) == false &&
                    Ready(AbilityList.Sambas) == true &&
                    _fface.Player.TPCurrent >= 350)
                {
                    UseAbility("Haste Samba", AbilityList.Sambas, 4, false);
                }

                if ((IsAfflicted(StatusEffect.Paralysis) ||
                    IsAfflicted(StatusEffect.Poison) ||
                    IsAfflicted(StatusEffect.Slow) ||
                    IsAfflicted(StatusEffect.Blindness)) && _fface.Player.TPCurrent >= 200 && Ready(AbilityList.Waltzes))
                {
                    UseAbility("Healing Waltz", AbilityList.Waltzes, 3, false);
                }
            }
        }

        public override void UseHeals()
        {
            if (_fface.Player.HPPCurrent < 75)
            {
                if (_fface.Player.HPPCurrent < 25 && Ready(AbilityList.Two_Hour))
                {
                    SendCommand("/ja \"Perfect Dodge\" <me>");
                }
            }
        }

        public override void UseSpells()
        {

        }

        public override void UseWeaponskills()
        {
            var mobHead = _fface.Target.PosH;
            var myHead = _fface.Player.PosH;

            if (Math.Abs(Math.Abs(mobHead) - Math.Abs(myHead)) < 10 && Ready(AbilityList.Sneak_Attack))
            {
                UseAbility(AbilityList.Sneak_Attack, 1);
                Thread.Sleep(1700);
            }
            if (_fface.Target.Name == "Lycaon")
            {
                SendCommand("/ws \"Shark Bite\" <t>", 3);
            }
            else
            {
                if (_fface.Player.Name == "Dazuto")
                {
                    SendCommand("/ws \"Evisceration\" <t>", 3);
                }
                else if (_fface.Player.Name == "Honeybooboo" || _fface.Player.Name == "Crazie")
                {
                    SendCommand("/ws \"Shark Bite\" <t>", 3);
                }
                else if (_fface.Player.Name == "Riddlex")
                {
                    SendCommand("/ws \"Mercy Stroke\" <t>", 3);
                }
                else 
                {
                    SendCommand("/ws \"Rudra's Storm\" <t>", 3);
                }
            }
        }

        #region Add other methods here that aren't overrides, such as calculating finishing moves, etc

        #endregion
    }
}
