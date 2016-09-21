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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.whmCbReraise = new System.Windows.Forms.CheckBox();
            this.whmCuragaGroupbox = new System.Windows.Forms.GroupBox();
            this.whmCbCuragaIV = new System.Windows.Forms.CheckBox();
            this.whmCbCuragaV = new System.Windows.Forms.CheckBox();
            this.whmCbCuraga = new System.Windows.Forms.CheckBox();
            this.whmCbCuragaII = new System.Windows.Forms.CheckBox();
            this.whmCbCuragaIII = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.whmGridView)).BeginInit();
            this.whmBuffsGroupbox.SuspendLayout();
            this.whmCuragaGroupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // whmFormSave
            // 
            this.whmFormSave.Font = new System.Drawing.Font("Roboto Condensed", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.whmFormSave.Location = new System.Drawing.Point(363, 252);
            this.whmFormSave.Name = "whmFormSave";
            this.whmFormSave.Size = new System.Drawing.Size(78, 29);
            this.whmFormSave.TabIndex = 1;
            this.whmFormSave.Text = "Save";
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
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.whmGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.whmGridView.RowHeadersVisible = false;
            this.whmGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.whmGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.whmGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.whmGridView.Size = new System.Drawing.Size(264, 153);
            this.whmGridView.TabIndex = 2;
            this.whmGridView.SelectionChanged += new System.EventHandler(this.whmFormGridView_SelectionChange);
            // 
            // MemberName
            // 
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MemberName.DefaultCellStyle = dataGridViewCellStyle5;
            this.MemberName.HeaderText = "Name";
            this.MemberName.Name = "MemberName";
            this.MemberName.ReadOnly = true;
            this.MemberName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MemberName.Width = 130;
            // 
            // MemberHaste
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.NullValue = false;
            this.MemberHaste.DefaultCellStyle = dataGridViewCellStyle6;
            this.MemberHaste.HeaderText = "Haste";
            this.MemberHaste.Name = "MemberHaste";
            this.MemberHaste.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberHaste.Width = 60;
            // 
            // MemberRegen
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle7.NullValue = false;
            this.MemberRegen.DefaultCellStyle = dataGridViewCellStyle7;
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
            "Barfira",
            "Barblizzara",
            "Baraera",
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
            "Barvira",
            "Barparalyzra",
            "Barsilencera",
            "Barpetra",
            "Barpoisonra",
            "Barblindra",
            "Barsleepra"});
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
            this.whmBuffsGroupbox.Controls.Add(this.whmCbReraise);
            this.whmBuffsGroupbox.Location = new System.Drawing.Point(12, 179);
            this.whmBuffsGroupbox.Name = "whmBuffsGroupbox";
            this.whmBuffsGroupbox.Size = new System.Drawing.Size(90, 102);
            this.whmBuffsGroupbox.TabIndex = 9;
            this.whmBuffsGroupbox.TabStop = false;
            this.whmBuffsGroupbox.Text = "Buffs";
            // 
            // whmCbShellra
            // 
            this.whmCbShellra.AutoSize = true;
            this.whmCbShellra.Location = new System.Drawing.Point(12, 70);
            this.whmCbShellra.Name = "whmCbShellra";
            this.whmCbShellra.Size = new System.Drawing.Size(58, 17);
            this.whmCbShellra.TabIndex = 1;
            this.whmCbShellra.Text = "Shellra";
            this.whmCbShellra.UseVisualStyleBackColor = true;
            // 
            // whmCbProtectra
            // 
            this.whmCbProtectra.AutoSize = true;
            this.whmCbProtectra.Location = new System.Drawing.Point(12, 47);
            this.whmCbProtectra.Name = "whmCbProtectra";
            this.whmCbProtectra.Size = new System.Drawing.Size(69, 17);
            this.whmCbProtectra.TabIndex = 1;
            this.whmCbProtectra.Text = "Protectra";
            this.whmCbProtectra.UseVisualStyleBackColor = true;
            // 
            // whmCbReraise
            // 
            this.whmCbReraise.AutoSize = true;
            this.whmCbReraise.Location = new System.Drawing.Point(12, 24);
            this.whmCbReraise.Name = "whmCbReraise";
            this.whmCbReraise.Size = new System.Drawing.Size(62, 17);
            this.whmCbReraise.TabIndex = 0;
            this.whmCbReraise.Text = "Reraise";
            this.whmCbReraise.UseVisualStyleBackColor = true;
            // 
            // whmCuragaGroupbox
            // 
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuraga);
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuragaII);
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuragaIII);
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuragaIV);
            this.whmCuragaGroupbox.Controls.Add(this.whmCbCuragaV);
            this.whmCuragaGroupbox.Location = new System.Drawing.Point(108, 179);
            this.whmCuragaGroupbox.Name = "whmCuragaGroupbox";
            this.whmCuragaGroupbox.Size = new System.Drawing.Size(168, 102);
            this.whmCuragaGroupbox.TabIndex = 10;
            this.whmCuragaGroupbox.TabStop = false;
            this.whmCuragaGroupbox.Text = "Curaga";
            // 
            // whmCbCuragaIV
            // 
            this.whmCbCuragaIV.AutoSize = true;
            this.whmCbCuragaIV.Location = new System.Drawing.Point(10, 47);
            this.whmCbCuragaIV.Name = "whmCbCuragaIV";
            this.whmCbCuragaIV.Size = new System.Drawing.Size(73, 17);
            this.whmCbCuragaIV.TabIndex = 1;
            this.whmCbCuragaIV.Text = "Curaga IV";
            this.whmCbCuragaIV.UseVisualStyleBackColor = true;
            // 
            // whmCbCuragaV
            // 
            this.whmCbCuragaV.AutoSize = true;
            this.whmCbCuragaV.Location = new System.Drawing.Point(10, 24);
            this.whmCbCuragaV.Name = "whmCbCuragaV";
            this.whmCbCuragaV.Size = new System.Drawing.Size(70, 17);
            this.whmCbCuragaV.TabIndex = 1;
            this.whmCbCuragaV.Text = "Curaga V";
            this.whmCbCuragaV.UseVisualStyleBackColor = true;
            // 
            // whmCbCuraga
            // 
            this.whmCbCuraga.AutoSize = true;
            this.whmCbCuraga.Location = new System.Drawing.Point(88, 47);
            this.whmCbCuraga.Name = "whmCbCuraga";
            this.whmCbCuraga.Size = new System.Drawing.Size(60, 17);
            this.whmCbCuraga.TabIndex = 4;
            this.whmCbCuraga.Text = "Curaga";
            this.whmCbCuraga.UseVisualStyleBackColor = true;
            // 
            // whmCbCuragaII
            // 
            this.whmCbCuragaII.AutoSize = true;
            this.whmCbCuragaII.Location = new System.Drawing.Point(88, 24);
            this.whmCbCuragaII.Name = "whmCbCuragaII";
            this.whmCbCuragaII.Size = new System.Drawing.Size(69, 17);
            this.whmCbCuragaII.TabIndex = 3;
            this.whmCbCuragaII.Text = "Curaga II";
            this.whmCbCuragaII.UseVisualStyleBackColor = true;
            // 
            // whmCbCuragaIII
            // 
            this.whmCbCuragaIII.AutoSize = true;
            this.whmCbCuragaIII.Location = new System.Drawing.Point(10, 70);
            this.whmCbCuragaIII.Name = "whmCbCuragaIII";
            this.whmCbCuragaIII.Size = new System.Drawing.Size(72, 17);
            this.whmCbCuragaIII.TabIndex = 2;
            this.whmCbCuragaIII.Text = "Curaga III";
            this.whmCbCuragaIII.UseVisualStyleBackColor = true;
            // 
            // WhiteMageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 291);
            this.Controls.Add(this.whmCuragaGroupbox);
            this.Controls.Add(this.whmBuffsGroupbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.whmComboBoostStat);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.whmComboBarStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.whmComboBarElemental);
            this.Controls.Add(this.whmGridView);
            this.Controls.Add(this.whmFormSave);
            this.Name = "WhiteMageForm";
            this.Text = "WhiteMageForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.whmForm_FormClosing);
            this.Shown += new System.EventHandler(this.whmForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.whmGridView)).EndInit();
            this.whmBuffsGroupbox.ResumeLayout(false);
            this.whmBuffsGroupbox.PerformLayout();
            this.whmCuragaGroupbox.ResumeLayout(false);
            this.whmCuragaGroupbox.PerformLayout();
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
        private System.Windows.Forms.CheckBox whmCbReraise;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MemberRegen;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MemberHaste;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberName;
        private System.Windows.Forms.GroupBox whmCuragaGroupbox;
        private System.Windows.Forms.CheckBox whmCbCuragaIV;
        private System.Windows.Forms.CheckBox whmCbCuragaV;
        private System.Windows.Forms.CheckBox whmCbCuraga;
        private System.Windows.Forms.CheckBox whmCbCuragaII;
        private System.Windows.Forms.CheckBox whmCbCuragaIII;
    }
}