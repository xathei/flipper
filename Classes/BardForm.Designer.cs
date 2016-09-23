namespace Flipper.Classes
{
    partial class BardForm
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
            this.brdGridView = new System.Windows.Forms.DataGridView();
            this.MemberName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MemberBalladII = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MemberBalladIII = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.brdSongsContainer = new System.Windows.Forms.GroupBox();
            this.brdCbMarcatoSongTwo = new System.Windows.Forms.CheckBox();
            this.brdCbMarcatoSongOne = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.brdSongTwoLabel = new System.Windows.Forms.Label();
            this.brdSongOneLabel = new System.Windows.Forms.Label();
            this.brdSongTwo = new System.Windows.Forms.ComboBox();
            this.brdSongOne = new System.Windows.Forms.ComboBox();
            this.brdFormSave = new System.Windows.Forms.Button();
            this.brdAbilities = new System.Windows.Forms.GroupBox();
            this.brdCbTroubadour = new System.Windows.Forms.CheckBox();
            this.brdCbNightingale = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.brdGridView)).BeginInit();
            this.brdSongsContainer.SuspendLayout();
            this.brdAbilities.SuspendLayout();
            this.SuspendLayout();
            // 
            // brdGridView
            // 
            this.brdGridView.AllowUserToAddRows = false;
            this.brdGridView.AllowUserToDeleteRows = false;
            this.brdGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.brdGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MemberName,
            this.MemberBalladII,
            this.MemberBalladIII});
            this.brdGridView.Location = new System.Drawing.Point(12, 12);
            this.brdGridView.Name = "brdGridView";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.brdGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.brdGridView.RowHeadersVisible = false;
            this.brdGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.brdGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.brdGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.brdGridView.Size = new System.Drawing.Size(264, 153);
            this.brdGridView.TabIndex = 3;
            this.brdGridView.SelectionChanged += new System.EventHandler(this.brdFormGridView_SelectionChange);
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
            this.MemberName.Width = 145;
            // 
            // MemberBalladII
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.NullValue = false;
            this.MemberBalladII.DefaultCellStyle = dataGridViewCellStyle6;
            this.MemberBalladII.HeaderText = "Ballad II";
            this.MemberBalladII.Name = "MemberBalladII";
            this.MemberBalladII.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberBalladII.Width = 60;
            // 
            // MemberBalladIII
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle7.NullValue = false;
            this.MemberBalladIII.DefaultCellStyle = dataGridViewCellStyle7;
            this.MemberBalladIII.HeaderText = "Ballad III";
            this.MemberBalladIII.Name = "MemberBalladIII";
            this.MemberBalladIII.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberBalladIII.Width = 60;
            // 
            // brdSongsContainer
            // 
            this.brdSongsContainer.Controls.Add(this.brdCbMarcatoSongTwo);
            this.brdSongsContainer.Controls.Add(this.brdCbMarcatoSongOne);
            this.brdSongsContainer.Controls.Add(this.label1);
            this.brdSongsContainer.Controls.Add(this.brdSongTwoLabel);
            this.brdSongsContainer.Controls.Add(this.brdSongOneLabel);
            this.brdSongsContainer.Controls.Add(this.brdSongTwo);
            this.brdSongsContainer.Controls.Add(this.brdSongOne);
            this.brdSongsContainer.Location = new System.Drawing.Point(13, 239);
            this.brdSongsContainer.Name = "brdSongsContainer";
            this.brdSongsContainer.Size = new System.Drawing.Size(264, 122);
            this.brdSongsContainer.TabIndex = 4;
            this.brdSongsContainer.TabStop = false;
            this.brdSongsContainer.Text = "Songs";
            // 
            // brdCbMarcatoSongTwo
            // 
            this.brdCbMarcatoSongTwo.AutoSize = true;
            this.brdCbMarcatoSongTwo.Location = new System.Drawing.Point(227, 90);
            this.brdCbMarcatoSongTwo.Name = "brdCbMarcatoSongTwo";
            this.brdCbMarcatoSongTwo.Size = new System.Drawing.Size(15, 14);
            this.brdCbMarcatoSongTwo.TabIndex = 3;
            this.brdCbMarcatoSongTwo.UseVisualStyleBackColor = true;
            // 
            // brdCbMarcatoSongOne
            // 
            this.brdCbMarcatoSongOne.AutoSize = true;
            this.brdCbMarcatoSongOne.Location = new System.Drawing.Point(227, 42);
            this.brdCbMarcatoSongOne.Name = "brdCbMarcatoSongOne";
            this.brdCbMarcatoSongOne.Size = new System.Drawing.Size(15, 14);
            this.brdCbMarcatoSongOne.TabIndex = 3;
            this.brdCbMarcatoSongOne.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(211, 23);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Marcato";
            // 
            // brdSongTwoLabel
            // 
            this.brdSongTwoLabel.AutoSize = true;
            this.brdSongTwoLabel.Location = new System.Drawing.Point(6, 71);
            this.brdSongTwoLabel.Name = "brdSongTwoLabel";
            this.brdSongTwoLabel.Size = new System.Drawing.Size(72, 13);
            this.brdSongTwoLabel.TabIndex = 1;
            this.brdSongTwoLabel.Text = "Second Song";
            // 
            // brdSongOneLabel
            // 
            this.brdSongOneLabel.AutoSize = true;
            this.brdSongOneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.brdSongOneLabel.Location = new System.Drawing.Point(6, 23);
            this.brdSongOneLabel.Name = "brdSongOneLabel";
            this.brdSongOneLabel.Size = new System.Drawing.Size(54, 13);
            this.brdSongOneLabel.TabIndex = 1;
            this.brdSongOneLabel.Text = "First Song";
            // 
            // brdSongTwo
            // 
            this.brdSongTwo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.brdSongTwo.FormattingEnabled = true;
            this.brdSongTwo.Items.AddRange(new object[] {
            "None",
            "Valor Minuet V",
            "Valor Minuet IV",
            "Victory March",
            "Advancing March",
            "Blade Madrigal",
            "Sword Madrigal",
            "Mages Ballad III",
            "Mages Ballad II"});
            this.brdSongTwo.Location = new System.Drawing.Point(9, 87);
            this.brdSongTwo.Name = "brdSongTwo";
            this.brdSongTwo.Size = new System.Drawing.Size(191, 21);
            this.brdSongTwo.TabIndex = 0;
            // 
            // brdSongOne
            // 
            this.brdSongOne.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.brdSongOne.FormattingEnabled = true;
            this.brdSongOne.Items.AddRange(new object[] {
            "None",
            "Valor Minuet V",
            "Valor Minuet IV",
            "Victory March",
            "Advancing March",
            "Blade Madrigal",
            "Sword Madrigal",
            "Mages Ballad III",
            "Mages Ballad II"});
            this.brdSongOne.Location = new System.Drawing.Point(9, 39);
            this.brdSongOne.Name = "brdSongOne";
            this.brdSongOne.Size = new System.Drawing.Size(191, 21);
            this.brdSongOne.TabIndex = 0;
            // 
            // brdFormSave
            // 
            this.brdFormSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.brdFormSave.Location = new System.Drawing.Point(181, 373);
            this.brdFormSave.Name = "brdFormSave";
            this.brdFormSave.Size = new System.Drawing.Size(97, 29);
            this.brdFormSave.TabIndex = 5;
            this.brdFormSave.Text = "Save && Close";
            this.brdFormSave.UseVisualStyleBackColor = true;
            this.brdFormSave.Click += new System.EventHandler(this.brdFormSave_Click);
            // 
            // brdAbilities
            // 
            this.brdAbilities.Controls.Add(this.brdCbNightingale);
            this.brdAbilities.Controls.Add(this.brdCbTroubadour);
            this.brdAbilities.Location = new System.Drawing.Point(12, 175);
            this.brdAbilities.Name = "brdAbilities";
            this.brdAbilities.Size = new System.Drawing.Size(264, 58);
            this.brdAbilities.TabIndex = 6;
            this.brdAbilities.TabStop = false;
            this.brdAbilities.Text = "Abilities";
            // 
            // brdCbTroubadour
            // 
            this.brdCbTroubadour.AutoSize = true;
            this.brdCbTroubadour.Location = new System.Drawing.Point(13, 25);
            this.brdCbTroubadour.Name = "brdCbTroubadour";
            this.brdCbTroubadour.Size = new System.Drawing.Size(81, 17);
            this.brdCbTroubadour.TabIndex = 0;
            this.brdCbTroubadour.Text = "Troubadour";
            this.brdCbTroubadour.UseVisualStyleBackColor = true;
            // 
            // brdCbNightingale
            // 
            this.brdCbNightingale.AutoSize = true;
            this.brdCbNightingale.Location = new System.Drawing.Point(153, 25);
            this.brdCbNightingale.Name = "brdCbNightingale";
            this.brdCbNightingale.Size = new System.Drawing.Size(79, 17);
            this.brdCbNightingale.TabIndex = 0;
            this.brdCbNightingale.Text = "Nightingale";
            this.brdCbNightingale.UseVisualStyleBackColor = true;
            // 
            // BardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 416);
            this.Controls.Add(this.brdAbilities);
            this.Controls.Add(this.brdFormSave);
            this.Controls.Add(this.brdSongsContainer);
            this.Controls.Add(this.brdGridView);
            this.Name = "BardForm";
            this.Text = "Bard Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.brdForm_FormClosing);
            this.Shown += new System.EventHandler(this.brdForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.brdGridView)).EndInit();
            this.brdSongsContainer.ResumeLayout(false);
            this.brdSongsContainer.PerformLayout();
            this.brdAbilities.ResumeLayout(false);
            this.brdAbilities.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView brdGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MemberBalladII;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MemberBalladIII;
        private System.Windows.Forms.GroupBox brdSongsContainer;
        private System.Windows.Forms.ComboBox brdSongOne;
        private System.Windows.Forms.CheckBox brdCbMarcatoSongTwo;
        private System.Windows.Forms.CheckBox brdCbMarcatoSongOne;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label brdSongTwoLabel;
        private System.Windows.Forms.Label brdSongOneLabel;
        private System.Windows.Forms.ComboBox brdSongTwo;
        private System.Windows.Forms.Button brdFormSave;
        private System.Windows.Forms.GroupBox brdAbilities;
        private System.Windows.Forms.CheckBox brdCbNightingale;
        private System.Windows.Forms.CheckBox brdCbTroubadour;
    }
}