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
            ((System.ComponentModel.ISupportInitialize)(this.whmGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // whmFormSave
            // 
            this.whmFormSave.Font = new System.Drawing.Font("Roboto Condensed", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.whmFormSave.Location = new System.Drawing.Point(363, 188);
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
            this.MemberName.Width = 141;
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
            // WhiteMageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 229);
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
        private System.Windows.Forms.DataGridViewCheckBoxColumn MemberRegen;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MemberHaste;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberName;
    }
}