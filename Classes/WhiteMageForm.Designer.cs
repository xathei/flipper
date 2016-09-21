namespace Flipper.Classes
{
    partial class WhiteMageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WhiteMageForm));
            this.whmFormSave = new System.Windows.Forms.Button();
            this.whmGridView = new System.Windows.Forms.DataGridView();
            this.MemberName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MemberHaste = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MemberRegen = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.whmComboBarElemental = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.whmComboBarStatus = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.whmComboBoostStat = new System.Windows.Forms.ComboBox();
            this.whmBuffsGroupbox = new System.Windows.Forms.GroupBox();
            this.whmCbShellra = new System.Windows.Forms.CheckBox();
            this.whmCbProtectra = new System.Windows.Forms.CheckBox();
            this.whmCuragaGroupbox = new System.Windows.Forms.GroupBox();
            this.whmCbCuraga = new System.Windows.Forms.CheckBox();
            this.whmCbCuragaII = new System.Windows.Forms.CheckBox();
            this.whmCbCuragaIII = new System.Windows.Forms.CheckBox();
            this.whmCbCuragaIV = new System.Windows.Forms.CheckBox();
            this.whmCbCuragaV = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.whmCbAquaveil = new System.Windows.Forms.CheckBox();
            this.whmCbBlink = new System.Windows.Forms.CheckBox();
            this.whmCbStoneskin = new System.Windows.Forms.CheckBox();
            this.whmCbReraise = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.whmGridView)).BeginInit();
            this.whmBuffsGroupbox.SuspendLayout();
            this.whmCuragaGroupbox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // whmFormSave
            // 
            this.whmFormSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.whmFormSave.Location = new System.Drawing.Point(344, 288);
            this.whmFormSave.Name = "whmFormSave";
            this.whmFormSave.Size = new System.Drawing.Size(97, 29);
            this.whmFormSave.TabIndex = 1;
            this.whmFormSave.Text = "Save && Close";
            this.whmFormSave.UseVisualStyleBackColor = true;
            this.whmFormSave.Click += new System.EventHandler(this.whmFormSave_Click);
            // 
            // whmGridView
            // 
            this.whmGridView.AllowUserToAddRows = false;
            this.whmGridView.AllowUserToDeleteRows = false;
            this.whmGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.whmGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MemberName,
            this.MemberHaste,
            this.MemberRegen});
            this.whmGridView.Location = new System.Drawing.Point(12, 12);
            this.whmGridView.Name = "whmGridView";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.whmGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.whmGridView.RowHeadersVisible = false;
            this.whmGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.whmGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.whmGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.whmGridView.Size = new System.Drawing.Size(264, 153);
            this.whmGridView.TabIndex = 2;
            this.whmGridView.SelectionChanged += new System.EventHandler(this.whmFormGridView_SelectionChange);
            // 
            // MemberName
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MemberName.DefaultCellStyle = dataGridViewCellStyle1;
            this.MemberName.HeaderText = "Name";
            this.MemberName.Name = "MemberName";
            this.MemberName.ReadOnly = true;
            this.MemberName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MemberName.Width = 145;
            // 
            // MemberHaste
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.NullValue = false;
            this.MemberHaste.DefaultCellStyle = dataGridViewCellStyle2;
            this.MemberHaste.HeaderText = "Haste";
            this.MemberHaste.Name = "MemberHaste";
            this.MemberHaste.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberHaste.Width = 60;
            // 
            // MemberRegen
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle3.NullValue = false;
            this.MemberRegen.DefaultCellStyle = dataGridViewCellStyle3;
            this.MemberRegen.HeaderText = "Regen";
            this.MemberRegen.Name = "MemberRegen";
            this.MemberRegen.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberRegen.Width = 60;
            // 
            // whmComboBarElemental
            // 
            this.whmComboBarElemental.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.whmComboBarElemental.FormattingEnabled = true;
            this.whmComboBarElemental.Items.AddRange(new object[] {
            "None",
            "Baraera",
            "Barblizzara",
            "Barfira",
            "Barstonra",
            "Barthundra",
            "Barwatera"});
            this.whmComboBarElemental.Location = new System.Drawing.Point(291, 32);
            this.whmComboBarElemental.Name = "whmComboBarElemental";
            this.whmComboBarElemental.Size = new System.Drawing.Size(150, 21);
            this.whmComboBarElemental.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(288, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Bar-Elemental";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label2.Location = new System.Drawing.Point(288, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Bar-Status";
            // 
            // whmComboBarStatus
            // 
            this.whmComboBarStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.whmComboBarStatus.FormattingEnabled = true;
            this.whmComboBarStatus.Items.AddRange(new object[] {
            "None",
            "Baramnesra",
            "Barblindra",
            "Barparalyzra",
            "Barpetra",
            "Barpoisonra",
            "Barsilencera",
            "Barsleepra",
            "Barvira"});
            this.whmComboBarStatus.Location = new System.Drawing.Point(291, 88);
            this.whmComboBarStatus.Name = "whmComboBarStatus";
            this.whmComboBarStatus.Size = new System.Drawing.Size(150, 21);
            this.whmComboBarStatus.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.Location = new System.Drawing.Point(288, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Boost-STAT";
            // 
            // whmComboBoostStat
            // 
            this.whmComboBoostStat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.whmComboBoostStat.FormattingEnabled = true;
            this.whmComboBoostStat.Items.AddRange(new object[] {
            "None",
            "Boost-AGI",
            "Boost-CHR",
            "Boost-DEX",
            "Boost-INT",
            "Boost-MND",
            "Boost-STR",
            "Boost-VIT"});
            this.whmComboBoostStat.Location = new System.Drawing.Point(291, 144);
            this.whmComboBoostStat.Name = "whmComboBoostStat";
            this.whmComboBoostStat.Size = new System.Drawing.Size(150, 21);
            this.whmComboBoostStat.TabIndex = 7;
            // 
            // whmBuffsGroupbox
            // 
            this.whmBuffsGroupbox.Controls.Add(this.whmCbShellra);
            this.whmBuffsGroupbox.Controls.Add(this.whmCbProtectra);
            this.whmBuffsGroupbox.Location = new System.Drawing.Point(12, 262);
            this.whmBuffsGroupbox.Name = "whmBuffsGroupbox";
            this.whmBuffsGroupbox.Size = new System.Drawing.Size(210, 50);
            this.whmBuffsGroupbox.TabIndex = 9;
            this.whmBuffsGroupbox.TabStop = false;
            this.whmBuffsGroupbox.Text = "Party Buffs";
            // 
            // whmCbShellra
            // 
            this.whmCbShellra.AutoSize = true;
            this.whmCbShellra.Checked = true;
            this.whmCbShellra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.whmCbShellra.Location = new System.Drawing.Point(113, 24);
            this.whmCbShellra.Name = "whmCbShellra";
            this.whmCbShellra.Size = new System.Drawing.Size(58, 17);
            this.whmCbShellra.TabIndex = 1;
            this.whmCbShellra.Text = "Shellra";
            this.whmCbShellra.UseVisualStyleBackColor = true;
            // 
            // whmCbProtectra
            // 
            this.whmCbProtectra.AutoSize = true;
            this.whmCbProtectra.Checked = true;
            this.whmCbProtectra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.whmCbProtectra.Location = new System.Drawing.Point(12, 24);
            this.whmCbProtectra.Name = "whmCbProtectra";
            this.whmCbProtectra.Size = new System.Drawing.Size(69, 17);
            this.whmCbProtectra.TabIndex = 1;
            this.whmCbProtectra.Text = "Protectra";
            this.whmCbProtectra.UseVisualStyleBackColor = true;
            // 
            // whmCuragaGroupbox
            // 
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuraga);
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuragaII);
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuragaIII);
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuragaIV);
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuragaV);
            this.whmCuragaGroupbox.Location = new System.Drawing.Point(231, 179);
            this.whmCuragaGroupbox.Name = "whmCuragaGroupbox";
            this.whmCuragaGroupbox.Size = new System.Drawing.Size(210, 100);
            this.whmCuragaGroupbox.TabIndex = 10;
            this.whmCuragaGroupbox.TabStop = false;
            this.whmCuragaGroupbox.Text = "Curaga";
            // 
            // whmCbCuraga
            // 
            this.whmCbCuraga.AutoSize = true;
            this.whmCbCuraga.Location = new System.Drawing.Point(113, 47);
            this.whmCbCuraga.Name = "whmCbCuraga";
            this.whmCbCuraga.Size = new System.Drawing.Size(60, 17);
            this.whmCbCuraga.TabIndex = 4;
            this.whmCbCuraga.Text = "Curaga";
            this.whmCbCuraga.UseVisualStyleBackColor = true;
            // 
            // whmCbCuragaII
            // 
            this.whmCbCuragaII.AutoSize = true;
            this.whmCbCuragaII.Location = new System.Drawing.Point(113, 24);
            this.whmCbCuragaII.Name = "whmCbCuragaII";
            this.whmCbCuragaII.Size = new System.Drawing.Size(69, 17);
            this.whmCbCuragaII.TabIndex = 3;
            this.whmCbCuragaII.Text = "Curaga II";
            this.whmCbCuragaII.UseVisualStyleBackColor = true;
            // 
            // whmCbCuragaIII
            // 
            this.whmCbCuragaIII.AutoSize = true;
            this.whmCbCuragaIII.Location = new System.Drawing.Point(12, 70);
            this.whmCbCuragaIII.Name = "whmCbCuragaIII";
            this.whmCbCuragaIII.Size = new System.Drawing.Size(72, 17);
            this.whmCbCuragaIII.TabIndex = 2;
            this.whmCbCuragaIII.Text = "Curaga III";
            this.whmCbCuragaIII.UseVisualStyleBackColor = true;
            // 
            // whmCbCuragaIV
            // 
            this.whmCbCuragaIV.AutoSize = true;
            this.whmCbCuragaIV.Location = new System.Drawing.Point(12, 47);
            this.whmCbCuragaIV.Name = "whmCbCuragaIV";
            this.whmCbCuragaIV.Size = new System.Drawing.Size(73, 17);
            this.whmCbCuragaIV.TabIndex = 1;
            this.whmCbCuragaIV.Text = "Curaga IV";
            this.whmCbCuragaIV.UseVisualStyleBackColor = true;
            // 
            // whmCbCuragaV
            // 
            this.whmCbCuragaV.AutoSize = true;
            this.whmCbCuragaV.Location = new System.Drawing.Point(12, 24);
            this.whmCbCuragaV.Name = "whmCbCuragaV";
            this.whmCbCuragaV.Size = new System.Drawing.Size(70, 17);
            this.whmCbCuragaV.TabIndex = 1;
            this.whmCbCuragaV.Text = "Curaga V";
            this.whmCbCuragaV.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.whmCbAquaveil);
            this.groupBox1.Controls.Add(this.whmCbBlink);
            this.groupBox1.Controls.Add(this.whmCbStoneskin);
            this.groupBox1.Controls.Add(this.whmCbReraise);
            this.groupBox1.Location = new System.Drawing.Point(12, 179);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(210, 75);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Self Buffs";
            // 
            // whmCbAquaveil
            // 
            this.whmCbAquaveil.AutoSize = true;
            this.whmCbAquaveil.Location = new System.Drawing.Point(113, 47);
            this.whmCbAquaveil.Name = "whmCbAquaveil";
            this.whmCbAquaveil.Size = new System.Drawing.Size(67, 17);
            this.whmCbAquaveil.TabIndex = 3;
            this.whmCbAquaveil.Text = "Aquaveil";
            this.whmCbAquaveil.UseVisualStyleBackColor = true;
            // 
            // whmCbBlink
            // 
            this.whmCbBlink.AutoSize = true;
            this.whmCbBlink.Location = new System.Drawing.Point(12, 47);
            this.whmCbBlink.Name = "whmCbBlink";
            this.whmCbBlink.Size = new System.Drawing.Size(49, 17);
            this.whmCbBlink.TabIndex = 2;
            this.whmCbBlink.Text = "Blink";
            this.whmCbBlink.UseVisualStyleBackColor = true;
            // 
            // whmCbStoneskin
            // 
            this.whmCbStoneskin.AutoSize = true;
            this.whmCbStoneskin.Location = new System.Drawing.Point(113, 24);
            this.whmCbStoneskin.Name = "whmCbStoneskin";
            this.whmCbStoneskin.Size = new System.Drawing.Size(73, 17);
            this.whmCbStoneskin.TabIndex = 1;
            this.whmCbStoneskin.Text = "Stoneskin";
            this.whmCbStoneskin.UseVisualStyleBackColor = true;
            // 
            // whmCbReraise
            // 
            this.whmCbReraise.AutoSize = true;
            this.whmCbReraise.Checked = true;
            this.whmCbReraise.CheckState = System.Windows.Forms.CheckState.Checked;
            this.whmCbReraise.Location = new System.Drawing.Point(12, 24);
            this.whmCbReraise.Name = "whmCbReraise";
            this.whmCbReraise.Size = new System.Drawing.Size(62, 17);
            this.whmCbReraise.TabIndex = 0;
            this.whmCbReraise.Text = "Reraise";
            this.whmCbReraise.UseVisualStyleBackColor = true;
            // 
            // WhiteMageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 328);
            this.Controls.Add(this.whmCuragaGroupbox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.whmBuffsGroupbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.whmComboBoostStat);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.whmComboBarStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.whmComboBarElemental);
            this.Controls.Add(this.whmGridView);
            this.Controls.Add(this.whmFormSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WhiteMageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "White Mage Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.whmForm_FormClosing);
            this.Shown += new System.EventHandler(this.whmForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.whmGridView)).EndInit();
            this.whmBuffsGroupbox.ResumeLayout(false);
            this.whmBuffsGroupbox.PerformLayout();
            this.whmCuragaGroupbox.ResumeLayout(false);
            this.whmCuragaGroupbox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button whmFormSave;
        private System.Windows.Forms.DataGridView whmGridView;
        private System.Windows.Forms.ComboBox whmComboBarElemental;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox whmComboBarStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox whmComboBoostStat;
        private System.Windows.Forms.GroupBox whmBuffsGroupbox;
        private System.Windows.Forms.CheckBox whmCbShellra;
        private System.Windows.Forms.CheckBox whmCbProtectra;
        private System.Windows.Forms.GroupBox whmCuragaGroupbox;
        private System.Windows.Forms.CheckBox whmCbCuragaIV;
        private System.Windows.Forms.CheckBox whmCbCuragaV;
        private System.Windows.Forms.CheckBox whmCbCuraga;
        private System.Windows.Forms.CheckBox whmCbCuragaII;
        private System.Windows.Forms.CheckBox whmCbCuragaIII;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox whmCbAquaveil;
        private System.Windows.Forms.CheckBox whmCbBlink;
        private System.Windows.Forms.CheckBox whmCbStoneskin;
        private System.Windows.Forms.CheckBox whmCbReraise;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MemberRegen;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MemberHaste;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberName;
    }
}