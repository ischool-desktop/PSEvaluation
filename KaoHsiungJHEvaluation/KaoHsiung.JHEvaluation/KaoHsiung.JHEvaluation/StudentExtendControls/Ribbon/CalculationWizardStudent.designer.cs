namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon
{
    partial class CalculationWizardStudent
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
            this.wizard1 = new DevComponents.DotNetBar.Wizard();
            this.wizardPage1 = new DevComponents.DotNetBar.WizardPage();
            this.panelEduAdmin = new System.Windows.Forms.Panel();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.intGradeYear = new DevComponents.Editors.IntegerInput();
            this.panelStudent = new System.Windows.Forms.Panel();
            this.intSemester = new DevComponents.Editors.IntegerInput();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.intSchoolYear = new DevComponents.Editors.IntegerInput();
            this.wizardPage2 = new DevComponents.DotNetBar.WizardPage();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.progressBarX1 = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.lnErrorViewHistory = new System.Windows.Forms.LinkLabel();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.wizardPage3 = new DevComponents.DotNetBar.WizardPage();
            this.prgsBarCalc = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.lnErrorView = new System.Windows.Forms.LinkLabel();
            this.lblMsgCalc = new DevComponents.DotNetBar.LabelX();
            this.wizardPage4 = new DevComponents.DotNetBar.WizardPage();
            this.prgsBarUpload = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.lblMsgUpload = new DevComponents.DotNetBar.LabelX();
            this.wizard1.SuspendLayout();
            this.wizardPage1.SuspendLayout();
            this.panelEduAdmin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.intGradeYear)).BeginInit();
            this.panelStudent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.intSemester)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).BeginInit();
            this.wizardPage2.SuspendLayout();
            this.wizardPage3.SuspendLayout();
            this.wizardPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard1
            // 
            this.wizard1.BackButtonText = "< 上一步";
            this.wizard1.BackButtonWidth = 65;
            this.wizard1.BackColor = System.Drawing.Color.Transparent;
            this.wizard1.ButtonStyle = DevComponents.DotNetBar.eWizardStyle.Office2007;
            this.wizard1.CancelButtonText = "取消";
            this.wizard1.CancelButtonWidth = 65;
            this.wizard1.Cursor = System.Windows.Forms.Cursors.Default;
            this.wizard1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizard1.FinishButtonTabIndex = 3;
            this.wizard1.FinishButtonText = "關閉";
            this.wizard1.FinishButtonWidth = 65;
            this.wizard1.FooterHeight = 25;
            // 
            // 
            // 
            this.wizard1.FooterStyle.BackColor = System.Drawing.Color.Transparent;
            this.wizard1.ForeColor = System.Drawing.Color.DarkBlue;
            this.wizard1.HeaderCaptionFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.wizard1.HeaderHeight = 30;
            this.wizard1.HeaderImageSize = new System.Drawing.Size(32, 32);
            this.wizard1.HeaderImageVisible = false;
            // 
            // 
            // 
            this.wizard1.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(215)))), ((int)(((byte)(243)))));
            this.wizard1.HeaderStyle.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(241)))), ((int)(((byte)(254)))));
            this.wizard1.HeaderStyle.BackColorGradientAngle = 90;
            this.wizard1.HeaderStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.wizard1.HeaderStyle.BorderBottomColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(157)))), ((int)(((byte)(182)))));
            this.wizard1.HeaderStyle.BorderBottomWidth = 1;
            this.wizard1.HeaderStyle.BorderColor = System.Drawing.SystemColors.Control;
            this.wizard1.HeaderStyle.BorderLeftWidth = 1;
            this.wizard1.HeaderStyle.BorderRightWidth = 1;
            this.wizard1.HeaderStyle.BorderTopWidth = 1;
            this.wizard1.HeaderStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.wizard1.HeaderStyle.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.wizard1.HelpButtonVisible = false;
            this.wizard1.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
            this.wizard1.Location = new System.Drawing.Point(0, 0);
            this.wizard1.Name = "wizard1";
            this.wizard1.NextButtonText = "下一步 >";
            this.wizard1.NextButtonWidth = 65;
            this.wizard1.Size = new System.Drawing.Size(336, 200);
            this.wizard1.TabIndex = 0;
            this.wizard1.WizardPages.AddRange(new DevComponents.DotNetBar.WizardPage[] {
            this.wizardPage1,
            this.wizardPage2,
            this.wizardPage3,
            this.wizardPage4});
            this.wizard1.CancelButtonClick += new System.ComponentModel.CancelEventHandler(this.FormClose);
            this.wizard1.FinishButtonClick += new System.ComponentModel.CancelEventHandler(this.FormClose);
            // 
            // wizardPage1
            // 
            this.wizardPage1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wizardPage1.AntiAlias = false;
            this.wizardPage1.BackButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage1.BackButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage1.BackColor = System.Drawing.Color.Transparent;
            this.wizardPage1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.wizardPage1.Controls.Add(this.panelStudent);
            this.wizardPage1.Controls.Add(this.panelEduAdmin);
            this.wizardPage1.HelpButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage1.Location = new System.Drawing.Point(7, 42);
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.PageTitle = "選擇學年度學期";
            this.wizardPage1.Size = new System.Drawing.Size(322, 121);
            this.wizardPage1.TabIndex = 7;
            // 
            // panelEduAdmin
            // 
            this.panelEduAdmin.Controls.Add(this.labelX3);
            this.panelEduAdmin.Controls.Add(this.intGradeYear);
            this.panelEduAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEduAdmin.Location = new System.Drawing.Point(0, 0);
            this.panelEduAdmin.Name = "panelEduAdmin";
            this.panelEduAdmin.Size = new System.Drawing.Size(322, 121);
            this.panelEduAdmin.TabIndex = 4;
            this.panelEduAdmin.Visible = false;
            // 
            // labelX3
            // 
            this.labelX3.Location = new System.Drawing.Point(170, 52);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(36, 23);
            this.labelX3.TabIndex = 1;
            this.labelX3.Text = "年級";
            // 
            // intGradeYear
            // 
            // 
            // 
            // 
            this.intGradeYear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intGradeYear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intGradeYear.Location = new System.Drawing.Point(122, 51);
            this.intGradeYear.MaxValue = 3;
            this.intGradeYear.MinValue = 1;
            this.intGradeYear.Name = "intGradeYear";
            this.intGradeYear.ShowUpDown = true;
            this.intGradeYear.Size = new System.Drawing.Size(45, 22);
            this.intGradeYear.TabIndex = 0;
            this.intGradeYear.Value = 1;
            // 
            // panelStudent
            // 
            this.panelStudent.BackColor = System.Drawing.Color.Transparent;
            this.panelStudent.Controls.Add(this.intSemester);
            this.panelStudent.Controls.Add(this.labelX1);
            this.panelStudent.Controls.Add(this.labelX2);
            this.panelStudent.Controls.Add(this.intSchoolYear);
            this.panelStudent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStudent.Location = new System.Drawing.Point(0, 0);
            this.panelStudent.Name = "panelStudent";
            this.panelStudent.Size = new System.Drawing.Size(322, 121);
            this.panelStudent.TabIndex = 3;
            this.panelStudent.Visible = false;
            // 
            // intSemester
            // 
            // 
            // 
            // 
            this.intSemester.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intSemester.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intSemester.Location = new System.Drawing.Point(216, 51);
            this.intSemester.MaxValue = 2;
            this.intSemester.MinValue = 1;
            this.intSemester.Name = "intSemester";
            this.intSemester.ShowUpDown = true;
            this.intSemester.Size = new System.Drawing.Size(40, 22);
            this.intSemester.TabIndex = 2;
            this.intSemester.Value = 1;
            // 
            // labelX1
            // 
            this.labelX1.Location = new System.Drawing.Point(73, 52);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(47, 23);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "學年度";
            // 
            // labelX2
            // 
            this.labelX2.Location = new System.Drawing.Point(180, 52);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(35, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "學期";
            // 
            // intSchoolYear
            // 
            // 
            // 
            // 
            this.intSchoolYear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intSchoolYear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intSchoolYear.Location = new System.Drawing.Point(122, 51);
            this.intSchoolYear.MaxValue = 150;
            this.intSchoolYear.MinValue = 50;
            this.intSchoolYear.Name = "intSchoolYear";
            this.intSchoolYear.ShowUpDown = true;
            this.intSchoolYear.Size = new System.Drawing.Size(50, 22);
            this.intSchoolYear.TabIndex = 2;
            this.intSchoolYear.Value = 50;
            // 
            // wizardPage2
            // 
            this.wizardPage2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wizardPage2.AntiAlias = false;
            this.wizardPage2.BackButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage2.BackButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage2.BackColor = System.Drawing.Color.Transparent;
            this.wizardPage2.Controls.Add(this.labelX5);
            this.wizardPage2.Controls.Add(this.progressBarX1);
            this.wizardPage2.Controls.Add(this.lnErrorViewHistory);
            this.wizardPage2.Controls.Add(this.labelX4);
            this.wizardPage2.Location = new System.Drawing.Point(7, 42);
            this.wizardPage2.Name = "wizardPage2";
            this.wizardPage2.PageTitle = "檢查學期歷程";
            this.wizardPage2.Size = new System.Drawing.Size(322, 121);
            this.wizardPage2.TabIndex = 10;
            this.wizardPage2.AfterPageDisplayed += new DevComponents.DotNetBar.WizardPageChangeEventHandler(this.wizardPage2_AfterPageDisplayed);
            // 
            // labelX5
            // 
            this.labelX5.Location = new System.Drawing.Point(14, -2);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(309, 68);
            this.labelX5.TabIndex = 4;
            this.labelX5.Text = "部分學生沒有學期歷程！計算成績時，需要歷程中學生本學期的年級資料，請選取『下一步』，系統將自動寫入學生學期歷程並繼續進行成績計算，您也可以選取『取消』離開。";
            this.labelX5.TextLineAlignment = System.Drawing.StringAlignment.Near;
            this.labelX5.Visible = false;
            this.labelX5.WordWrap = true;
            // 
            // progressBarX1
            // 
            this.progressBarX1.Location = new System.Drawing.Point(14, 68);
            this.progressBarX1.Name = "progressBarX1";
            this.progressBarX1.Size = new System.Drawing.Size(300, 23);
            this.progressBarX1.TabIndex = 3;
            this.progressBarX1.Text = "progressBarX1";
            // 
            // lnErrorViewHistory
            // 
            this.lnErrorViewHistory.AutoSize = true;
            this.lnErrorViewHistory.Location = new System.Drawing.Point(150, 94);
            this.lnErrorViewHistory.Name = "lnErrorViewHistory";
            this.lnErrorViewHistory.Size = new System.Drawing.Size(149, 12);
            this.lnErrorViewHistory.TabIndex = 2;
            this.lnErrorViewHistory.TabStop = true;
            this.lnErrorViewHistory.Text = "檢視本學期無學期歷程學生";
            this.lnErrorViewHistory.Visible = false;
            this.lnErrorViewHistory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnErrorView_LinkClicked);
            // 
            // labelX4
            // 
            this.labelX4.Location = new System.Drawing.Point(14, 45);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(180, 23);
            this.labelX4.TabIndex = 0;
            this.labelX4.Text = "檢查學期歷程中…";
            // 
            // wizardPage3
            // 
            this.wizardPage3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wizardPage3.AntiAlias = false;
            this.wizardPage3.BackButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage3.BackButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage3.BackColor = System.Drawing.Color.Transparent;
            this.wizardPage3.Controls.Add(this.prgsBarCalc);
            this.wizardPage3.Controls.Add(this.lnErrorView);
            this.wizardPage3.Controls.Add(this.lblMsgCalc);
            this.wizardPage3.Location = new System.Drawing.Point(7, 42);
            this.wizardPage3.Name = "wizardPage3";
            this.wizardPage3.NextButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage3.NextButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage3.PageTitle = "計算學期成績";
            this.wizardPage3.Size = new System.Drawing.Size(322, 121);
            this.wizardPage3.TabIndex = 8;
            this.wizardPage3.BackButtonClick += new System.ComponentModel.CancelEventHandler(this.wizardPage3_BackButtonClick);
            this.wizardPage3.AfterPageDisplayed += new DevComponents.DotNetBar.WizardPageChangeEventHandler(this.wizardPage3_AfterPageDisplayed);
            // 
            // prgsBarCalc
            // 
            this.prgsBarCalc.Location = new System.Drawing.Point(14, 68);
            this.prgsBarCalc.Name = "prgsBarCalc";
            this.prgsBarCalc.Size = new System.Drawing.Size(300, 23);
            this.prgsBarCalc.TabIndex = 2;
            this.prgsBarCalc.Text = "progressBarX1";
            // 
            // lnErrorView
            // 
            this.lnErrorView.AutoSize = true;
            this.lnErrorView.Location = new System.Drawing.Point(228, 94);
            this.lnErrorView.Name = "lnErrorView";
            this.lnErrorView.Size = new System.Drawing.Size(77, 12);
            this.lnErrorView.TabIndex = 1;
            this.lnErrorView.TabStop = true;
            this.lnErrorView.Text = "檢視錯誤訊息";
            this.lnErrorView.Visible = false;
            this.lnErrorView.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnErrorView_LinkClicked);
            // 
            // lblMsgCalc
            // 
            this.lblMsgCalc.Location = new System.Drawing.Point(14, 45);
            this.lblMsgCalc.Name = "lblMsgCalc";
            this.lblMsgCalc.Size = new System.Drawing.Size(180, 23);
            this.lblMsgCalc.TabIndex = 0;
            this.lblMsgCalc.Text = "計算學期成績...";
            // 
            // wizardPage4
            // 
            this.wizardPage4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wizardPage4.AntiAlias = false;
            this.wizardPage4.BackButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage4.BackButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage4.BackColor = System.Drawing.Color.Transparent;
            this.wizardPage4.CancelButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage4.CancelButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage4.Controls.Add(this.prgsBarUpload);
            this.wizardPage4.Controls.Add(this.lblMsgUpload);
            this.wizardPage4.Location = new System.Drawing.Point(7, 42);
            this.wizardPage4.Name = "wizardPage4";
            this.wizardPage4.PageTitle = "上傳學期成績";
            this.wizardPage4.Size = new System.Drawing.Size(322, 121);
            this.wizardPage4.TabIndex = 9;
            // 
            // prgsBarUpload
            // 
            this.prgsBarUpload.Location = new System.Drawing.Point(14, 68);
            this.prgsBarUpload.Name = "prgsBarUpload";
            this.prgsBarUpload.Size = new System.Drawing.Size(300, 23);
            this.prgsBarUpload.TabIndex = 4;
            this.prgsBarUpload.Text = "progressBarX1";
            // 
            // lblMsgUpload
            // 
            this.lblMsgUpload.Location = new System.Drawing.Point(14, 45);
            this.lblMsgUpload.Name = "lblMsgUpload";
            this.lblMsgUpload.Size = new System.Drawing.Size(180, 23);
            this.lblMsgUpload.TabIndex = 3;
            this.lblMsgUpload.Text = "上傳學期成績...";
            // 
            // CalculationWizardStudent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 200);
            this.Controls.Add(this.wizard1);
            this.Name = "CalculationWizardStudent";
            this.Text = "成績計算";
            this.wizard1.ResumeLayout(false);
            this.wizardPage1.ResumeLayout(false);
            this.panelEduAdmin.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.intGradeYear)).EndInit();
            this.panelStudent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.intSemester)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).EndInit();
            this.wizardPage2.ResumeLayout(false);
            this.wizardPage2.PerformLayout();
            this.wizardPage3.ResumeLayout(false);
            this.wizardPage3.PerformLayout();
            this.wizardPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Wizard wizard1;
        private DevComponents.DotNetBar.WizardPage wizardPage1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.WizardPage wizardPage3;
        private DevComponents.DotNetBar.WizardPage wizardPage4;
        private DevComponents.DotNetBar.Controls.ProgressBarX prgsBarCalc;
        private System.Windows.Forms.LinkLabel lnErrorView;
        private DevComponents.DotNetBar.LabelX lblMsgCalc;
        private DevComponents.DotNetBar.Controls.ProgressBarX prgsBarUpload;
        private DevComponents.DotNetBar.LabelX lblMsgUpload;
        private DevComponents.Editors.IntegerInput intSemester;
        private DevComponents.Editors.IntegerInput intSchoolYear;
        private System.Windows.Forms.Panel panelStudent;
        private System.Windows.Forms.Panel panelEduAdmin;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.Editors.IntegerInput intGradeYear;
        private DevComponents.DotNetBar.WizardPage wizardPage2;
        private DevComponents.DotNetBar.LabelX labelX4;
        private System.Windows.Forms.LinkLabel lnErrorViewHistory;
        private DevComponents.DotNetBar.Controls.ProgressBarX progressBarX1;
        private DevComponents.DotNetBar.LabelX labelX5;
    }
}