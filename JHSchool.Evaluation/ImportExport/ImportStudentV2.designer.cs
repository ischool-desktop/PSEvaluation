namespace JHSchool.Evaluation.ImportExport
{
    partial class ImportStudentV2
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportStudentV2));
            this.wizard1 = new DevComponents.DotNetBar.Wizard();
            this.wizardPage1 = new DevComponents.DotNetBar.WizardPage();
            this.chkTrim = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.lblReqFields = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.txtFile = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.wizardPage2 = new DevComponents.DotNetBar.WizardPage();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.wizardPage3 = new DevComponents.DotNetBar.WizardPage();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lblErrCount = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.lblWarningCount = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.progressBarX1 = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.SelectSourceFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.errorFile = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorKey = new System.Windows.Forms.ErrorProvider(this.components);
            this.cboStudStatus = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.wizard1.SuspendLayout();
            this.wizardPage1.SuspendLayout();
            this.wizardPage2.SuspendLayout();
            this.wizardPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorKey)).BeginInit();
            this.SuspendLayout();
            // 
            // wizard1
            // 
            this.wizard1.BackButtonText = "上一步";
            this.wizard1.BackColor = System.Drawing.Color.Transparent;
            this.wizard1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("wizard1.BackgroundImage")));
            this.wizard1.ButtonStyle = DevComponents.DotNetBar.eWizardStyle.Office2007;
            this.wizard1.CancelButtonText = "關閉";
            this.wizard1.Cursor = System.Windows.Forms.Cursors.Default;
            this.wizard1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizard1.FinishButtonTabIndex = 3;
            this.wizard1.FinishButtonText = "開始匯入";
            this.wizard1.FooterHeight = 33;
            // 
            // 
            // 
            this.wizard1.FooterStyle.BackColor = System.Drawing.Color.Transparent;
            this.wizard1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(57)))), ((int)(((byte)(129)))));
            this.wizard1.HeaderCaptionFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wizard1.HeaderImage = ((System.Drawing.Image)(resources.GetObject("wizard1.HeaderImage")));
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
            this.wizard1.NextButtonText = "下一步";
            this.wizard1.Size = new System.Drawing.Size(464, 323);
            this.wizard1.TabIndex = 0;
            this.wizard1.WizardPages.AddRange(new DevComponents.DotNetBar.WizardPage[] {
            this.wizardPage1,
            this.wizardPage2,
            this.wizardPage3});
            this.wizard1.WizardPageChanged += new DevComponents.DotNetBar.WizardPageChangeEventHandler(this.wizard1_WizardPageChanged);
            // 
            // wizardPage1
            // 
            this.wizardPage1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wizardPage1.AntiAlias = false;
            this.wizardPage1.BackButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage1.BackColor = System.Drawing.Color.Transparent;
            this.wizardPage1.Controls.Add(this.chkTrim);
            this.wizardPage1.Controls.Add(this.linkLabel2);
            this.wizardPage1.Controls.Add(this.lblReqFields);
            this.wizardPage1.Controls.Add(this.labelX1);
            this.wizardPage1.Controls.Add(this.buttonX1);
            this.wizardPage1.Controls.Add(this.txtFile);
            this.wizardPage1.Location = new System.Drawing.Point(7, 72);
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.NextButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage1.PageDescription = "選取匯入檔案";
            this.wizardPage1.Size = new System.Drawing.Size(450, 206);
            this.wizardPage1.TabIndex = 7;
            // 
            // chkTrim
            // 
            this.chkTrim.AutoSize = true;
            this.chkTrim.Checked = true;
            this.chkTrim.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTrim.CheckValue = "Y";
            this.chkTrim.Location = new System.Drawing.Point(35, 65);
            this.chkTrim.Name = "chkTrim";
            this.chkTrim.Size = new System.Drawing.Size(161, 21);
            this.chkTrim.TabIndex = 4;
            this.chkTrim.Text = "自動過慮頭尾空白字元";
            this.chkTrim.CheckedChanged += new System.EventHandler(this.chkTrim_CheckedChanged);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(357, 65);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(60, 17);
            this.linkLabel2.TabIndex = 3;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "重新整理";
            this.linkLabel2.Visible = false;
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // lblReqFields
            // 
            this.lblReqFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReqFields.Location = new System.Drawing.Point(105, 85);
            this.lblReqFields.Name = "lblReqFields";
            this.lblReqFields.Size = new System.Drawing.Size(312, 121);
            this.lblReqFields.TabIndex = 2;
            this.lblReqFields.TextLineAlignment = System.Drawing.StringAlignment.Near;
            this.lblReqFields.WordWrap = true;
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.Location = new System.Drawing.Point(35, 85);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(74, 21);
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "必要欄位：";
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1.Location = new System.Drawing.Point(394, 36);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(23, 25);
            this.buttonX1.TabIndex = 1;
            this.buttonX1.Text = "‧‧‧";
            this.buttonX1.Click += new System.EventHandler(this.buttonX1_Click);
            // 
            // txtFile
            // 
            this.txtFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtFile.Border.Class = "TextBoxBorder";
            this.txtFile.Location = new System.Drawing.Point(35, 36);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(353, 25);
            this.txtFile.TabIndex = 0;
            this.txtFile.TextChanged += new System.EventHandler(this.txtFile_TextChanged);
            // 
            // wizardPage2
            // 
            this.wizardPage2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wizardPage2.AntiAlias = false;
            this.wizardPage2.BackColor = System.Drawing.Color.Transparent;
            this.wizardPage2.Controls.Add(this.checkBox1);
            this.wizardPage2.Controls.Add(this.listView1);
            this.wizardPage2.Location = new System.Drawing.Point(7, 72);
            this.wizardPage2.Name = "wizardPage2";
            this.wizardPage2.PageDescription = "選取匯入欄位";
            this.wizardPage2.Size = new System.Drawing.Size(450, 206);
            this.wizardPage2.TabIndex = 8;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.Location = new System.Drawing.Point(0, -2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(79, 21);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "全部選取";
            this.checkBox1.UseVisualStyleBackColor = false;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.CheckBoxes = true;
            this.listView1.Location = new System.Drawing.Point(0, 17);
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(450, 189);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // wizardPage3
            // 
            this.wizardPage3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wizardPage3.AntiAlias = false;
            this.wizardPage3.BackColor = System.Drawing.Color.Transparent;
            this.wizardPage3.Controls.Add(this.linkLabel3);
            this.wizardPage3.Controls.Add(this.linkLabel1);
            this.wizardPage3.Controls.Add(this.lblErrCount);
            this.wizardPage3.Controls.Add(this.labelX4);
            this.wizardPage3.Controls.Add(this.lblWarningCount);
            this.wizardPage3.Controls.Add(this.labelX3);
            this.wizardPage3.Controls.Add(this.progressBarX1);
            this.wizardPage3.Controls.Add(this.labelX2);
            this.wizardPage3.FinishButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage3.Location = new System.Drawing.Point(7, 72);
            this.wizardPage3.Name = "wizardPage3";
            this.wizardPage3.NextButtonVisible = DevComponents.DotNetBar.eWizardButtonState.False;
            this.wizardPage3.PageDescription = "驗證匯入資料";
            this.wizardPage3.Size = new System.Drawing.Size(450, 206);
            this.wizardPage3.TabIndex = 9;
            this.wizardPage3.BackButtonClick += new System.ComponentModel.CancelEventHandler(this.wizardPage3_BackButtonClick);
            this.wizardPage3.FinishButtonClick += new System.ComponentModel.CancelEventHandler(this.wizardPage3_FinishButtonClick);
            this.wizardPage3.AfterPageDisplayed += new DevComponents.DotNetBar.WizardPageChangeEventHandler(this.wizardPage3_AfterPageDisplayed);
            // 
            // linkLabel3
            // 
            this.linkLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(353, 187);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(60, 17);
            this.linkLabel3.TabIndex = 4;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "終止驗證";
            this.linkLabel3.Visible = false;
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(314, 187);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(99, 17);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "檢視驗證後資料";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lblErrCount
            // 
            this.lblErrCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblErrCount.AutoSize = true;
            this.lblErrCount.Location = new System.Drawing.Point(159, 139);
            this.lblErrCount.Name = "lblErrCount";
            this.lblErrCount.Size = new System.Drawing.Size(15, 21);
            this.lblErrCount.TabIndex = 2;
            this.lblErrCount.Text = "0";
            // 
            // labelX4
            // 
            this.labelX4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelX4.AutoSize = true;
            this.labelX4.Location = new System.Drawing.Point(58, 139);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(101, 21);
            this.labelX4.TabIndex = 2;
            this.labelX4.Text = "錯誤資料筆數：";
            // 
            // lblWarningCount
            // 
            this.lblWarningCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblWarningCount.AutoSize = true;
            this.lblWarningCount.Location = new System.Drawing.Point(159, 101);
            this.lblWarningCount.Name = "lblWarningCount";
            this.lblWarningCount.Size = new System.Drawing.Size(15, 21);
            this.lblWarningCount.TabIndex = 2;
            this.lblWarningCount.Text = "0";
            // 
            // labelX3
            // 
            this.labelX3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelX3.AutoSize = true;
            this.labelX3.Location = new System.Drawing.Point(58, 101);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(101, 21);
            this.labelX3.TabIndex = 2;
            this.labelX3.Text = "警告資料筆數：";
            // 
            // progressBarX1
            // 
            this.progressBarX1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarX1.Location = new System.Drawing.Point(118, 30);
            this.progressBarX1.Name = "progressBarX1";
            this.progressBarX1.Size = new System.Drawing.Size(295, 23);
            this.progressBarX1.TabIndex = 1;
            this.progressBarX1.Text = "progressBarX1";
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            this.labelX2.Location = new System.Drawing.Point(37, 32);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(74, 21);
            this.labelX2.TabIndex = 0;
            this.labelX2.Text = "資料驗證中";
            // 
            // SelectSourceFileDialog
            // 
            this.SelectSourceFileDialog.Filter = "Excel 檔案 (*.xls)| *.xls";
            // 
            // errorFile
            // 
            this.errorFile.BlinkRate = 0;
            this.errorFile.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorFile.ContainerControl = this;
            // 
            // errorKey
            // 
            this.errorKey.ContainerControl = this;
            // 
            // cboStudStatus
            // 
            this.cboStudStatus.DisplayMember = "Text";
            this.cboStudStatus.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboStudStatus.FormattingEnabled = true;
            this.cboStudStatus.ItemHeight = 19;
            this.cboStudStatus.Location = new System.Drawing.Point(140, 294);
            this.cboStudStatus.Name = "cboStudStatus";
            this.cboStudStatus.Size = new System.Drawing.Size(87, 25);
            this.cboStudStatus.TabIndex = 6;
            // 
            // labelX6
            // 
            this.labelX6.BackColor = System.Drawing.Color.Transparent;
            this.labelX6.Location = new System.Drawing.Point(82, 295);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(60, 23);
            this.labelX6.TabIndex = 7;
            this.labelX6.Text = "學生狀態";
            // 
            // ImportStudentV2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(464, 323);
            this.Controls.Add(this.labelX6);
            this.Controls.Add(this.cboStudStatus);
            this.Controls.Add(this.wizard1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ImportStudentV2";
            this.Text = "";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportStudent_FormClosed);
            this.wizard1.ResumeLayout(false);
            this.wizardPage1.ResumeLayout(false);
            this.wizardPage1.PerformLayout();
            this.wizardPage2.ResumeLayout(false);
            this.wizardPage2.PerformLayout();
            this.wizardPage3.ResumeLayout(false);
            this.wizardPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorKey)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Wizard wizard1;
        private DevComponents.DotNetBar.WizardPage wizardPage1;
        private DevComponents.DotNetBar.WizardPage wizardPage2;
        private DevComponents.DotNetBar.ButtonX buttonX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtFile;
        private System.Windows.Forms.OpenFileDialog SelectSourceFileDialog;
        private System.Windows.Forms.ErrorProvider errorFile;
        private System.Windows.Forms.ErrorProvider errorKey;
        private DevComponents.DotNetBar.LabelX lblReqFields;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.WizardPage wizardPage3;
        private System.Windows.Forms.ListView listView1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.ProgressBarX progressBarX1;
        private DevComponents.DotNetBar.LabelX lblErrCount;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX lblWarningCount;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkTrim;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboStudStatus;
        private DevComponents.DotNetBar.LabelX labelX6;

    }
}