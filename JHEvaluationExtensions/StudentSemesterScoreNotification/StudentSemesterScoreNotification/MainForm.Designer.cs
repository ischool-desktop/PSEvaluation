namespace StudentSemesterScoreNotification
{
    partial class MainForm
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
            this.btnConfirm = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.lnkPrintSetting = new System.Windows.Forms.LinkLabel();
            this.lnkAbsentSetting = new System.Windows.Forms.LinkLabel();
            this.chkReScore = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.txtReExammark = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.cbxScoreType = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // btnConfirm
            // 
            this.btnConfirm.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.btnConfirm.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnConfirm.Location = new System.Drawing.Point(194, 124);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "確認";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(13, 13);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(47, 21);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "學年度";
            this.labelX1.Click += new System.EventHandler(this.labelX1_Click);
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(194, 13);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(34, 21);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "學期";
            this.labelX2.Click += new System.EventHandler(this.labelX2_Click);
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(67, 12);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(121, 25);
            this.cboSchoolYear.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSchoolYear.TabIndex = 3;
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(233, 12);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(121, 25);
            this.cboSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSemester.TabIndex = 4;
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(279, 124);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "離開";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lnkPrintSetting
            // 
            this.lnkPrintSetting.AutoSize = true;
            this.lnkPrintSetting.BackColor = System.Drawing.Color.Transparent;
            this.lnkPrintSetting.Location = new System.Drawing.Point(13, 125);
            this.lnkPrintSetting.Name = "lnkPrintSetting";
            this.lnkPrintSetting.Size = new System.Drawing.Size(60, 17);
            this.lnkPrintSetting.TabIndex = 6;
            this.lnkPrintSetting.TabStop = true;
            this.lnkPrintSetting.Text = "列印設定";
            this.lnkPrintSetting.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lnkAbsentSetting
            // 
            this.lnkAbsentSetting.AutoSize = true;
            this.lnkAbsentSetting.BackColor = System.Drawing.Color.Transparent;
            this.lnkAbsentSetting.Location = new System.Drawing.Point(79, 125);
            this.lnkAbsentSetting.Name = "lnkAbsentSetting";
            this.lnkAbsentSetting.Size = new System.Drawing.Size(60, 17);
            this.lnkAbsentSetting.TabIndex = 7;
            this.lnkAbsentSetting.TabStop = true;
            this.lnkAbsentSetting.Text = "假別設定";
            this.lnkAbsentSetting.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // chkReScore
            // 
            this.chkReScore.AutoSize = true;
            this.chkReScore.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkReScore.BackgroundStyle.Class = "";
            this.chkReScore.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkReScore.Location = new System.Drawing.Point(16, 87);
            this.chkReScore.Name = "chkReScore";
            this.chkReScore.Size = new System.Drawing.Size(161, 21);
            this.chkReScore.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkReScore.TabIndex = 8;
            this.chkReScore.Text = "只產生有補考成績學生";
            // 
            // labelX3
            // 
            this.labelX3.AutoSize = true;
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(194, 87);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(87, 21);
            this.labelX3.TabIndex = 9;
            this.labelX3.Text = "補考成績加註";
            // 
            // txtReExammark
            // 
            // 
            // 
            // 
            this.txtReExammark.Border.Class = "TextBoxBorder";
            this.txtReExammark.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtReExammark.Location = new System.Drawing.Point(287, 85);
            this.txtReExammark.Name = "txtReExammark";
            this.txtReExammark.Size = new System.Drawing.Size(55, 25);
            this.txtReExammark.TabIndex = 10;
            this.txtReExammark.Text = "*";
            // 
            // cbxScoreType
            // 
            this.cbxScoreType.DisplayMember = "Text";
            this.cbxScoreType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxScoreType.FormattingEnabled = true;
            this.cbxScoreType.ItemHeight = 19;
            this.cbxScoreType.Location = new System.Drawing.Point(66, 48);
            this.cbxScoreType.Name = "cbxScoreType";
            this.cbxScoreType.Size = new System.Drawing.Size(121, 25);
            this.cbxScoreType.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbxScoreType.TabIndex = 12;
            // 
            // labelX4
            // 
            this.labelX4.AutoSize = true;
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(26, 49);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(34, 21);
            this.labelX4.TabIndex = 11;
            this.labelX4.Text = "成績";
            this.labelX4.Click += new System.EventHandler(this.labelX4_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(373, 154);
            this.Controls.Add(this.cbxScoreType);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.txtReExammark);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.chkReScore);
            this.Controls.Add(this.lnkAbsentSetting);
            this.Controls.Add(this.lnkPrintSetting);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.cboSchoolYear);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.btnConfirm);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.Text = "學期成績通知單(含補考成績)";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnConfirm;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private System.Windows.Forms.LinkLabel lnkPrintSetting;
        private System.Windows.Forms.LinkLabel lnkAbsentSetting;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkReScore;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.TextBoxX txtReExammark;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxScoreType;
        private DevComponents.DotNetBar.LabelX labelX4;
    }
}