using FFACETools;
using FHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Flipper;
using Flipper.Classes;

namespace FlipperD
{
    public partial class Form1 : Form
    {
        FFACE fface;
        Hook Hook = new Hook();
        Battle Battle;



        public Form1()
        {
            InitializeComponent();
            this.CenterToScreen();
            uxAvatar.SelectedIndex = 0;

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (fface == null)
            {
                MessageBox.Show("Hook a character first...");
                return;
            }

            if (btnStart.Text == "Stop Dynamis")
            {
                btnStart.Text = "Start Dynamis";
                Battle.EndBattle();
                fface.Navigator.Reset();
            }
            else
            {
                bool extensions = uxTimeExtensions.Checked;
                btnStart.Text = "Stop Dynamis";
                Battle = new Battle();
                Battle.StartBattle(fface, Battle.Mode.Dynamis, fface.Player.Zone);
            }
        }

        private void btnStopDynamis_Click(object sender, EventArgs e)
        {
            if (fface == null)
            {
                MessageBox.Show("Hook a character first...");
                return;
            }
            Battle.EndBattle();
            btnStopDynamis.Enabled = false;
            btnStart.Enabled = true;
            fface.Navigator.Reset();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbMode.SelectedIndex = 0;
            ambDifficulty.SelectedIndex = 0;
            fface = null;
        }

        private void btnFarming_Click(object sender, EventArgs e)
        {
            if (fface == null)
            {
                MessageBox.Show("Hook a character first...");
                return;
            }
            if (btnFarming.Text == "Start Farming")
            {
                uxAvatar.Enabled = false;
                Battle = new Battle();
                Battle.Mode m = new Battle.Mode();

                m = cmbMode.SelectedIndex == 0 ? Battle.Mode.Farming : Battle.Mode.Einherjar;

                if (fface.Player.MainJob == Job.SMN)
                {
                    fface.Windower.SendString("/pet Release <me>");
                    Thread.Sleep(1500);
                }

                Battle.StartBattle(fface, m, fface.Player.Zone);
                btnFarming.Text = "Stop Farming";
                btnStopDynamis.Enabled = false;
                btnStart.Enabled = false;
                //btnStopDynamis.Enabled = true;
                //btnStart.Enabled = false;
            }
            else
            {
                uxAvatar.Enabled = true;
                Battle.EndBattle();
                btnStopDynamis.Enabled = false;
                btnStart.Enabled = true;
                btnFarming.Text = "Start Farming";
                fface.Navigator.Reset();
                fface.Windower.SendKey(KeyCode.NP_Number4, false);
                fface.Windower.SendKey(KeyCode.NP_Number2, false);
            }
        }
        public string CalculateMD5Hash(string input)

        {

            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            foreach (byte t in hash)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fface == null)
            {
                MessageBox.Show("Hook a character first...");
                return;
            }
            fface.Navigator.Reset();
            fface.Windower.SendKey(KeyCode.NP_Number4, false);
            fface.Windower.SendKey(KeyCode.NP_Number2, false);
        }

        private void uxCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            fface = null;

            uxTabs.Visible = false;
            string s = "";
            using (WebClient client = new WebClient())
            {
                s = client.DownloadString("http://www.dazusu.com/soap.php?d="+CalculateMD5Hash(uxCharacter.SelectedItem.ToString()).ToLower());
            }

            if (s == "0 " || uxCharacter.SelectedItem.ToString() == "Dazusu" || uxCharacter.SelectedItem.ToString() == "Dazuto" || uxCharacter.SelectedItem.ToString() == "Otsureku" || uxCharacter.SelectedItem.ToString() == "Damarijanette")
            {

                fface = Hook.HookCharacter(uxCharacter.SelectedItem.ToString());
                uxTabs.Visible = true;

            }
            else
            {
                MessageBox.Show("This character/server combination is not authorized.");
            }
        }

        private void uxCharacter_DropDown(object sender, EventArgs e)
        {
            uxCharacter.Items.Clear();

            foreach (string c in Hook.GetCharacterList())
            {
                uxCharacter.Items.Add(c);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fface.Windower.SendString("/echo Target ID: " + fface.Target.ID);
            fface.Windower.SendString("/echo Menu DialogID: " + fface.Menu.DialogID);
            fface.Windower.SendString("/echo OptionCount: " + fface.Menu.DialogOptionCount);
            fface.Windower.SendString("/echo OptionIndex: " + fface.Menu.DialogOptionIndex);

            try
            {
                fface.Windower.SendString("/echo SelectedText: " + fface.Menu.DialogText.Options[fface.Menu.DialogOptionIndex]);
            }
            catch (Exception ex)
            {
                
            }
            //int i = fface.Target.ID;
            //var TargetHeading = (int)(fface.Navigator.PosHToDegrees(fface.NPC.PosH(i)));
            //var MyHeading = (int)(fface.Navigator.GetPlayerPosHInDegrees());
            //double targetHeading = RadianToDegree(fface.NPC.PosH(i));

            //double lineAngle = PointAngle(new PointF { X = fface.Player.PosX, Y = fface.Player.PosZ }, new PointF { X = fface.NPC.PosX(i), Y = fface.NPC.PosZ(i) });
            //double difference = (targetHeading + lineAngle) % 360;
            //fface.Windower.SendString("/echo Target: " + TargetHeading + "  Player: " + MyHeading + "  Calc: " + (TargetHeading - MyHeading) + "  Angle: " + lineAngle + " / " + difference);
        }

        private double RadianToDegree(double radians)
        {
            return radians * (180.0 / Math.PI);
        }
        public double PointAngle(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }

        private double GetAngleOfLineBetweenTwoPoints(PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }

        private double GetSADifference(int i)
        {
            var TargetHeading = (int)(fface.Navigator.PosHToDegrees(fface.NPC.PosH(i)));
            var MyHeading = (int)(fface.Navigator.GetPlayerPosHInDegrees());
            double targetHeading = RadianToDegree(fface.NPC.PosH(i));

            double lineAngle = GetAngleOfLineBetweenTwoPoints(new PointF { X = fface.Player.PosX, Y = fface.Player.PosZ }, new PointF { X = fface.NPC.PosX(i), Y = fface.NPC.PosZ(i) });
            double difference = (targetHeading + lineAngle) % 360;

            return difference;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            var difference = GetSADifference(fface.Target.ID);
            while (difference < 66 || difference > 89)
            {
                fface.Windower.SendKey(KeyCode.NP_Number6, true);
                Thread.Sleep(100);
                fface.Windower.SendKey(KeyCode.NP_Number6, false);
                difference = GetSADifference(fface.Target.ID);
            }

            //double x = GetSADifference(fface.Target.ID);
            //fface.Windower.SendString($"/echo diff: {x}");
            //Thread t = new Thread(Claim);
            //t.Start();

        }

        bool claim = true;

        public void Claim()
        {
            while (claim)
            {
                for (short i = 0; i < 768; i++)
                {                                                                                                               //fface.NPC.ClaimedID(i) == playerServerId
                    if (fface.NPC.Name(i) == "Perfervid Naraka" && !fface.NPC.IsClaimed(i) && fface.NPC.Distance(i) < 20)
                    {
                        fface.Target.SetNPCTarget(i);
                        Thread.Sleep(500);
                        fface.Windower.SendString("/target <t>");
                        Thread.Sleep(2200);
                        fface.Windower.SendString("/ma Flash <t>");
                        claim = false;
                    }
                }
                Thread.Sleep(1);
            }
            fface.Windower.SendString("/echo End....");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            fface.Windower.SendString($"/p Pos: {fface.Player.PosX}  ,  {fface.Player.PosY}  ,  {fface.Player.PosZ}");
        }

        private void uxUnity_Click(object sender, EventArgs e)
        {
            Thread Uni = new Thread(Unity);
            Uni.Start();
        }

        private void Unity()
        {
            bool unity = true;
            while (unity)
            {

                fface.Windower.SendString("/echo ============= New Cycle.... ==============");
                if (uxFightOnry.Checked)
                {
                    List<TargetInfo> targets = new List<TargetInfo>();

                    while (!targets.Any())
                    {
                        targets = FindTarget("Planar", 15);
                        Thread.Sleep(100);
                    }

                    targets = targets.OrderBy(x => x.Distance).ToList();

                    if (targets.Any())
                    {
                        TargetInfo t = targets[0];

                        if (t.Distance > 5.5)
                        {
                            fface.Navigator.DistanceTolerance = 4;
                            fface.Navigator.GotoNPC(t.Id);
                        }

                        while (fface.Target.ID != t.Id)
                        {
                            fface.Windower.SendKeyPress(KeyCode.EscapeKey);
                            Thread.Sleep(100);
                            fface.Windower.SendKeyPress(KeyCode.EscapeKey);
                            Thread.Sleep(100);
                            fface.Target.SetNPCTarget(t.Id);
                            Thread.Sleep(100);
                            fface.Navigator.FaceHeading(t.Id);
                            Thread.Sleep(500);
                            fface.Windower.SendString("/target <t>");
                        }

                        fface.Windower.SendString("/item \"Rubicund Cell\" <t>");
                        Thread.Sleep(1000);
                    }
                }
                else if (!uxFightOnry.Checked)
                { 

                List<TargetInfo> targets = new List<TargetInfo>();

                while (!targets.Any())
                {
                    targets = FindTarget("Planar",15);
                    Thread.Sleep(100);
                }

                targets = targets.OrderBy(x => x.Distance).ToList();
                Thread.Sleep(100);
                    if (targets.Any())
                    {
                        TargetInfo t = targets[0];

                        if (t.Distance > 5.5)
                        {
                            fface.Navigator.DistanceTolerance = 4;
                            fface.Navigator.GotoNPC(t.Id);
                        }

                        while (fface.Target.ID != t.Id)
                        {
                            fface.Windower.SendKeyPress(KeyCode.EscapeKey);
                            Thread.Sleep(100);
                            fface.Windower.SendKeyPress(KeyCode.EscapeKey);
                            Thread.Sleep(100);
                            fface.Target.SetNPCTarget(t.Id);
                            Thread.Sleep(100);
                            fface.Navigator.FaceHeading(t.Id);
                            Thread.Sleep(500);
                            fface.Windower.SendString("/target <t>");
                        }

                        fface.Windower.SendString("/item \"Rubicund Cell\" <t>");
                        Thread.Sleep(800);
                        fface.Windower.SendString("/item \"Phase Displacer\" <t>");
                        Thread.Sleep(800);
                        fface.Windower.SendString("/item \"Phase Displacer\" <t>");
                        Thread.Sleep(800);
                        fface.Windower.SendString("/item \"Phase Displacer\" <t>");
                        Thread.Sleep(3000);

                        fface.Windower.SendKeyPress(KeyCode.EnterKey);
                        Thread.Sleep(700);
                        fface.Windower.SendKeyPress(KeyCode.EnterKey);
                        Thread.Sleep(700);

                        while (!fface.Menu.IsOpen || fface.Menu.DialogID != 306)
                        {
                            Thread.Sleep(50);
                        }

                        while (fface.Menu.DialogOptionIndex != 1)
                        {
                            fface.Windower.SendKeyPress(KeyCode.DownArrow);
                            Thread.Sleep(500);
                        }

                        fface.Windower.SendKeyPress(KeyCode.EnterKey);

                        while (!fface.Menu.IsOpen || fface.Menu.DialogID != 294)
                        {
                            Thread.Sleep(50);
                        }

                        while (fface.Menu.DialogOptionIndex != 0)
                        {
                            fface.Windower.SendKeyPress(KeyCode.UpArrow);
                            Thread.Sleep(500);
                        }

                        fface.Windower.SendKeyPress(KeyCode.EnterKey);

                        while (!fface.Menu.IsOpen || fface.Menu.DialogID != 295)
                        {
                            Thread.Sleep(50);
                        }

                        while (fface.Menu.DialogOptionIndex != 3)
                        {
                            fface.Windower.SendKeyPress(KeyCode.DownArrow);
                            Thread.Sleep(500);
                        }

                        fface.Windower.SendKeyPress(KeyCode.EnterKey);

                    }
                }

                    List<TargetInfo> mobs = new List<TargetInfo>();

                    while (!mobs.Any())
                    {
                        mobs = FindTarget("Aello");
                        Thread.Sleep(100);
                    }

                    if (mobs.Any())
                    {
                        fface.Windower.SendString("/item \"Stalwart's Tonic\" <me>");
                        Thread.Sleep(4200);
                        fface.Windower.SendString("/item \"Fanatic's Drink\" <me>");
                        Thread.Sleep(4200);
                        fface.Windower.SendString("/item \"Champion's Tonic\" <me>");
                        Thread.Sleep(4200);
                        fface.Windower.SendString("/item \"Dusty Wing\" <me>");
                        Thread.Sleep(4200);

                        TargetInfo mob = mobs[0];
                        while (fface.Target.ID != mob.Id)
                        {

                            fface.Target.SetNPCTarget(mob.Id);
                            Thread.Sleep(500);
                            fface.Windower.SendString("/target <t>");
                            Thread.Sleep(500);
                        }

                        fface.Windower.SendString("/attack <t>");
                        Thread.Sleep(100);
                        fface.Navigator.FaceHeading(mob.Id);
                        Thread.Sleep(2000);

                        while (fface.NPC.HPPCurrent(mob.Id) > 0 && fface.NPC.Status(mob.Id) != Status.Dead1 &&
                               fface.NPC.Status(mob.Id) != Status.Dead2)
                        {
                            if (fface.Player.TPCurrent > 1000)
                            {
                                if (fface.Player.MainJob == Job.PLD)
                                {
                                    if (fface.Player.MainJob == Job.PLD && !IsAfflicted(StatusEffect.Aftermath_lvl3))
                                    {
                                        if (fface.Player.TPCurrent >= 3000)
                                        {
                                            fface.Windower.SendString($"/ws \"Atonement\" <t>");
                                            Thread.Sleep(1000);
                                        }
                                        else
                                        {
                                            Thread.Sleep(100);
                                            continue;
                                        }
                                    }
                                    if (uxUnityWS.Text == "Savage Blade" && fface.Player.TPCurrent < 1000)
                                        continue;
                                    else
                                    {
                                        fface.Windower.SendString($"/ws \"{uxUnityWS.Text}\" <t>");
                                        Thread.Sleep(1000);
                                    }
                                }
                                if (fface.Player.MainJob == Job.THF)
                                {
                                    fface.Windower.SendString($"/ws \"{uxUnityWS.Text}\" <t>");
                                    Thread.Sleep(1000);
                                }
                                if (fface.Player.MainJob == Job.BLU)
                                {
                                    fface.Windower.SendString($"/ws \"{uxUnityWS.Text}\" <t>");
                                    Thread.Sleep(1000);
                                }
                                Thread.Sleep(100);
                            }
                        }

                        Thread.Sleep(1500);

                        List<TargetInfo> chests = new List<TargetInfo>();

                        while (!chests.Any())
                        {
                            chests = FindTarget("Riftworn", 15);
                            Thread.Sleep(100);
                        }

                        TargetInfo chest = chests[0];
                        while (fface.Target.ID != chest.Id)
                        {
                            Thread.Sleep(3000);
                            fface.Windower.SendKeyPress(KeyCode.EscapeKey);
                            Thread.Sleep(100);
                            fface.Windower.SendKeyPress(KeyCode.EscapeKey);
                            Thread.Sleep(100);
                            fface.Target.SetNPCTarget(chest.Id);
                            Thread.Sleep(500);
                            fface.Windower.SendString("/target <t>");
                            Thread.Sleep(500);
                            fface.Windower.SendString("/target <t>");
                            Thread.Sleep(500);
                        }

                        fface.Windower.SendKeyPress(KeyCode.EnterKey);
                        Thread.Sleep(100);
                        fface.Windower.SendKeyPress(KeyCode.EnterKey);
                        Thread.Sleep(2000);
                        fface.Windower.SendKeyPress(KeyCode.RightArrow);
                        Thread.Sleep(500);
                        fface.Windower.SendKeyPress(KeyCode.RightArrow);
                        Thread.Sleep(500);
                        fface.Windower.SendKeyPress(KeyCode.RightArrow);
                        Thread.Sleep(500);
                        fface.Windower.SendKeyPress(KeyCode.RightArrow);
                        Thread.Sleep(1500);
                        fface.Windower.SendKeyPress(KeyCode.EnterKey);
                        Thread.Sleep(1000);
                    }
            }
        }

        public bool IsAfflicted(StatusEffect effect)
        {
            foreach (StatusEffect status in fface.Player.StatusEffects)
            {
                if (status == effect)
                {
                    return true;
                }
            }
            return false;
        }

        public List<TargetInfo> FindTarget(string name, int distance = 50)
        {
            List<TargetInfo> results = new List<TargetInfo>();

            for (short i = 0; i < 768; i++)
            {
                if (fface.NPC.Name(i).ToLower().Contains(name.ToLower()) && fface.NPC.Distance(i) < distance && IsRendered(i) && fface.NPC.HPPCurrent(i) == 100 && fface.NPC.Status(i) != Status.Dead1 && fface.NPC.Status(i) != Status.Dead2)
                {
                    TargetInfo found = new TargetInfo
                    {
                        Id = i,
                        Name = fface.NPC.Name(i),
                        Status = fface.NPC.Status(i),
                        Distance = fface.NPC.Distance(i),
                        IsRendered = IsRendered(i)
                    };
                    results.Add(found);
                }
            }

            return results;
        }

        public bool IsRendered(int id)
        {
            byte[] b;
            b = fface.NPC.GetRawNPCData(id, 0x120, 4);
            if (b != null)
                return (BitConverter.ToInt32(b, 0) & 0x200) != 0;
            return false;
        }

        public bool Ready(AbilityList ability)
        {
            if (fface.Timer.GetAbilityRecast(ability) == 0)
                return true;

            return false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Thread chests = new Thread(DoChest);
            chests.Start();
        }

        public volatile bool _chests = true;

        private void DoChest()
        {
            var chests = FindTarget("Sturdy", 15);

            foreach (TargetInfo i in chests)
            {
                fface.Target.SetNPCTarget(i.Id);
                Thread.Sleep(100);
                fface.Windower.SendString("/target <t>");
                Thread.Sleep(100);
                fface.Windower.SendString("/item \"Forbidden Key\" <t>");
                Thread.Sleep(900);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Thread currency = new Thread(DoCurrency);
            currency.Start();
        }

        public void DoCurrency()
        {
            fface.Windower.SendString("//lua load Trader");
            fface.Windower.SendString("//lua load fastcs");
            fface.Windower.SendString("//load config");
            Thread.Sleep(500);
            if (fface.Player.Zone == Zone.Davoi)
            {
                while (fface.Item.GetInventoryItemCount(1452) >= 100)
                {
                    var npc = FindTarget("Loot", 10)[0].Id;
                    fface.Windower.SendString($"//trade {npc} \"O. Bronzepiece\" 100");
                    Thread.Sleep(1200);
                    fface.Windower.SendKeyPress(KeyCode.EnterKey);
                    Thread.Sleep(1400);
                }
            }

            if (fface.Player.Zone == Zone.Castle_Oztroja)
            {
                while (fface.Item.GetInventoryItemCount(1449) >= 100)
                {
                    var npc = FindTarget("Anti", 10)[0].Id;
                    fface.Windower.SendString($"//trade {npc} \"T. Whiteshell\" 100");
                    Thread.Sleep(1200);
                    fface.Windower.SendKeyPress(KeyCode.EnterKey);
                    Thread.Sleep(1400);
                }
            }

            if (fface.Player.Zone == Zone.Beadeaux)
            {
                while (fface.Item.GetInventoryItemCount(1455) >= 100)
                {
                    var npc = FindTarget("Hagg", 10)[0].Id;
                    fface.Windower.SendString($"//trade {npc} \"1 Byne Bill\" 100");
                    Thread.Sleep(120);
                    fface.Windower.SendKeyPress(KeyCode.EnterKey);
                    Thread.Sleep(1400);
                }
            }
            fface.Windower.SendString("/echo ========= Currency Conversion Completed ===========");

        }

        private void menuIndex_Click(object sender, EventArgs e)
        {
            fface.Windower.SendString("/echo Menu ID: " + fface.Menu.DialogOptionIndex + " : " + fface.Menu.DialogID + " : ");

        }

        private void uxUnityNM_Click(object sender, EventArgs e)
        {
            if (uxUnityNM.Text == "Start Unity")
            {
                uxUnityNM.Text = "Stop Unity";
                Thread u = new Thread(DoUnity);
                _unity = true;
                u.Start();
            }
            else
            {
                fface.Windower.SendString("/echo ===== Stopping Unity after this Cycle... ====");
                uxUnityNM.Text = "Start Unity";
                _unity = false;
            }
        }

        public volatile bool _unity = false;

        public void DoUnity()
        {
            if (string.IsNullOrEmpty(UnityTargetName.Text))
            {

                fface.Windower.SendString("/echo ==== Please specify a target name! ==== ");
                return;
            }
            while (_unity)
            {
                fface.Windower.SendString("/echo =============== Unity! ===============");

                var junction = new List<TargetInfo>();

                while (!junction.Any() && _unity)
                {
                    junction = FindTarget("Ethereal Junc");
                    Thread.Sleep(1000);
                }

                var ej = junction[0];

                if (ej.Distance > 5.5 && _unity)
                {
                    fface.Navigator.DistanceTolerance = 4;
                    fface.Navigator.GotoNPC(ej.Id);
                }

                while (fface.Target.ID != ej.Id && fface.Target.Name != "Ethereal Junction" && _unity)
                {
                    fface.Windower.SendKeyPress(KeyCode.EscapeKey);
                    Thread.Sleep(100);
                    fface.Windower.SendKeyPress(KeyCode.EscapeKey);
                    Thread.Sleep(100);
                    fface.Target.SetNPCTarget(ej.Id);
                    Thread.Sleep(100);
                    fface.Navigator.FaceHeading(ej.Id);
                    Thread.Sleep(500);
                    fface.Windower.SendString("/target <t>");
                }

                Thread.Sleep(1000);

                fface.Windower.SendKeyPress(KeyCode.EnterKey);

                while (!fface.Menu.IsOpen && _unity)
                {
                    Thread.Sleep(100);
                }

                Thread.Sleep(500);
                fface.Windower.SendKeyPress(KeyCode.LeftArrow);
                Thread.Sleep(500);
                fface.Windower.SendKeyPress(KeyCode.EnterKey);

                var mobs = FindTarget(Program.mainform.UnityTargetName.Text);

                while (!mobs.Any() && _unity)
                {
                     mobs = FindTarget(Program.mainform.UnityTargetName.Text);
                    Thread.Sleep(100);
                }

                var mob = mobs[0];

                fface.Target.SetNPCTarget(mob.Id);
                Thread.Sleep(100);
                fface.Windower.SendString("/target <t>");
                Thread.Sleep(100);
                fface.Windower.SendString("/attack <t>");
                Thread.Sleep(4000);

                while (fface.NPC.HPPCurrent(mob.Id) > 0 && fface.NPC.Status(mob.Id) != Status.Dead1 && fface.NPC.Status(mob.Id) != Status.Dead2 && _unity)
                {
                    fface.Navigator.FaceHeading(mob.Id);

                    if (fface.Player.TPCurrent > 1000 && fface.Player.Status == Status.Fighting)
                    {
                        if (!string.IsNullOrEmpty(UnityWS.Text))
                        {
                            fface.Windower.SendString("/ws \"" + UnityWS.Text + "\" <t>");
                            Thread.Sleep(1200);
                        }
                    }
                    else if (fface.Player.Status != Status.Fighting)
                    {
                        fface.Target.SetNPCTarget(mob.Id);
                        Thread.Sleep(100);
                        fface.Windower.SendString("/target <t>");
                        Thread.Sleep(100);
                        fface.Windower.SendString("/attack <t>");
                        Thread.Sleep(5000);
                    }
                    Thread.Sleep(100);
                }

                if (!string.IsNullOrEmpty(UnityItemContainer.Text) && _unity)
                {
                    Thread.Sleep(6000);
                    fface.Windower.SendString("/item \"" + UnityItemContainer.Text + "\" <me>");
                    Thread.Sleep(4000);
                }

            }
            fface.Windower.SendString("/echo =============== Unity has completed! ===============");
        }

        private void uxButtonDBox_Click(object sender, EventArgs e)
        {
            Thread c = new Thread(DoChat);
            c.Start();
        }

        private void DoChat()
        {
            bool _chat = true;
            while (_chat)
            {
                string newChat = "";
                string oldChat = "";
                try
                {
                    newChat = fface.Chat.GetNextLine().Text;
                }
                catch
                {

                }
                if (!String.IsNullOrEmpty(newChat) && newChat != oldChat)
                {
                    oldChat = newChat;
                    //WriteLog("Chat Line: " + newChat, true);
                    string[] token = newChat.Split(' ');

                    if (newChat.ToLower().Contains("stormwind") || newChat.ToLower().Contains("dans") || newChat.ToLower().Contains("tidal guillotine") || newChat.ToLower().Contains("carcharian verve") || newChat.ToLower().Contains("mayhem lantern"))
                    {
                        if (fface.Player.TPCurrent >= 1000)
                        {
                            fface.Windower.SendString("/ws \"Flat blade\" <t>");
                        }
                        Thread.Sleep(100);
                        fface.Windower.SendString("/echo ==== Stunning! ====");
                        Thread.Sleep(1000);
                    }
                    if (newChat.ToLower().Contains("follow me!"))
                    {
                        fface.Windower.SendString("/target Dazusu");
                        Thread.Sleep(1000);
                        fface.Windower.SendString("/follow <t>");
                        Thread.Sleep(100);
                        fface.Windower.SendString("/nod Dazusu");
                    }
                }
                Thread.Sleep(10);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void ambStartRecording_Click(object sender, EventArgs e)
        {
            if (ambStartRecording.Text == "Start")
            {
                ambStartRecording.Text = "Stop";
                _waypointRecordingThread = true;
                Thread record = new Thread(DoRecordWaypoints);
                record.Start();
            }
            else
            {
                ambStartRecording.Text = "Start";
                _waypointRecordingThread = false;
            }
        }

        public volatile bool _waypointRecordingThread;
        public List<string> waypoints = new List<string>();


        public void DoRecordWaypoints()
        {
            float pX = 0;
            float pZ = 0;
            while (_waypointRecordingThread)
            {
                if (fface.Navigator.DistanceTo(pX, pZ) > 1)
                {

                    ambWaypointList.Invoke((MethodInvoker) delegate
                    {
                        ambWaypointList.Items.Add($"{Math.Round(fface.Player.PosX, 2)},{Math.Round(fface.Player.PosZ, 2)}");
                    });
                    waypoints.Add($"{Math.Round(fface.Player.PosX, 2)},{Math.Round(fface.Player.PosZ, 2)}");
                    pX = fface.Player.PosX;
                    pZ = fface.Player.PosZ;
                }

                Thread.Sleep(5);
            }



        }

        private void ambClearButton_Click(object sender, EventArgs e)
        {
            _waypointRecordingThread = false;
            ambStartRecording.Text = "Start";
            waypoints.Clear();
            ambWaypointList.Items.Clear();
        }

        private void ambSaveWaypoints_Click(object sender, EventArgs e)
        {
            _waypointRecordingThread = false;
            File.WriteAllLines(ambFilename.Text, waypoints);
            MessageBox.Show($"File {ambFilename.Text} has been saved.", "Save OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Ambuscade _ambuscade;

        private void ambStartButton_Click(object sender, EventArgs e)
        {
            if (_ambuscade == null)
                _ambuscade = new Ambuscade(fface);

           Flipper.Monster roe = new Flipper.Monster()
            {
                MonsterName = ambRoETarget.Text,
                HitBox = Double.Parse(ambRoEHitbox.Text)
            };

            Flipper.Monster amb = new Flipper.Monster()
            {
                MonsterName = ambTarget.Text,
                HitBox = Double.Parse(ambHitbox.Text)
            };

            AmbuscadeSettings settings = new AmbuscadeSettings()
            {
                FillTrusts = ambTrustCheckbox.Checked,
                Leader = ambLeaderCheckbox.Checked,
                Network = ambNetworkMode.Checked,
                Role = DetermineJobRole(),
                PartyCount = Convert.ToInt32(ambPartyCount.Text)
            };

            _ambuscade.Start(fface, roe, amb, ambHomePoint.Text, uxAmbKeyItem.Checked, settings, ambDifficulty.SelectedItem.ToString());
        }

        public JobRole DetermineJobRole()
        {
            if (ambRoleDamage.Checked)
                return JobRole.Damage;
            if (ambRoleSupport.Checked)
                return JobRole.Healer;
            if (ambRoleTank.Checked)
                return JobRole.Tank;

            return JobRole.Damage;
        }

        private void AmbJobConfigButton_Click(object sender, EventArgs e)
        {
            if (_ambuscade == null)
                _ambuscade = new Ambuscade(fface);

            _ambuscade?.JobClass.SettingsForm();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            fface.Windower.SendKey(KeyCode.NP_Number4,false);
            fface.Windower.SendKey(KeyCode.NP_Number6, false);
            fface.Windower.SendKey(KeyCode.NP_Number2, false);

            //IJob job = new Geomancer(fface, Content.Ambuscade);
            //Flipper.Monster mob = new Flipper.Monster()
            //{
            //    HitBox = 2.5,
            //    MonsterName = "Sylvestre",
            //    TimeSpecific = false
            //};

            //Combat.SetInstance = fface;
            //Combat.SetJob = job;
            //Combat.FailType fail = Combat.FailType.NoFail;
            //Combat.Fight(fface.Target.ID, mob, Combat.Mode.None, out fail);
        }
    }

    public class TargetInfo
    {
        public int Id;
        public string Name;
        public FFACETools.Status Status;
        public double Distance;
        public bool IsRendered;
    }
}
