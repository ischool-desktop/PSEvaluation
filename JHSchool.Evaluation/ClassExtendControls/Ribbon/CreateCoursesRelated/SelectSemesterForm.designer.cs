namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.CreateCoursesRelated
{
    partial class SelectSemesterForm
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
            if (disposing && (components != null))
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem1 = new DevComponents.Editors.ComboItem();
            this.comboItem2 = new DevComponents.Editors.ComboItem();
            this.btnOK = new DevComponents.DotNetBar.ButtonX();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.errorProvider1 = new Framework.EnhancedErrorProvider();
            this.gpOptions = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.chkCopyAssessmentSetup = new System.Windows.Forms.CheckBox();
            this.chkCopyTeacher = new System.Windows.Forms.CheckBox();
            this.cboPreviousSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem3 = new DevComponents.Editors.ComboItem();
            this.comboItem4 = new DevComponents.Editors.ComboItem();
            this.cboPreviousSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.chkOption = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.gpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            this.labelX1.Location = new System.Drawing.Point(25, 13);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(60, 21);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "學年度：";
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(87, 11);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(70, 25);
            this.cboSchoolYear.TabIndex = 0;
            // 
            // labelX2
            // 
            this.labelX2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelX2.AutoSize = true;
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            this.labelX2.Location = new System.Drawing.Point(172, 13);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(47, 21);
            this.labelX2.TabIndex = 0;
            this.labelX2.Text = "學期：";
            // 
            // cboSemester
            // 
            this.cboSemester.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Items.AddRange(new object[] {
            this.comboItem1,
            this.comboItem2});
            this.cboSemester.Location = new System.Drawing.Point(221, 11);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(62, 25);
            this.cboSemester.TabIndex = 1;
            // 
            // comboItem1
            // 
            this.comboItem1.Text = "1";
            // 
            // comboItem2
            // 
            this.comboItem2.Text = "2";
            // 
            // btnOK
            // 
            this.btnOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnOK.Location = new System.Drawing.Point(138, 50);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "確定";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.Location = new System.Drawing.Point(219, 50);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // gpOptions
            // 
            this.gpOptions.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.gpOptions.BackColor = System.Drawing.Color.Transparent;
            this.gpOptions.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpOptions.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpOptions.Controls.Add(this.chkCopyAssessmentSetup);
            this.gpOptions.Controls.Add(this.chkCopyTeacher);
            this.gpOptions.Controls.Add(this.cboPreviousSemester);
            this.gpOptions.Controls.Add(this.cboPreviousSchoolYear);
            this.gpOptions.Controls.Add(this.labelX3);
            this.gpOptions.Controls.Add(this.labelX4);
            this.gpOptions.DrawTitleBox = false;
            this.gpOptions.Location = new System.Drawing.Point(11, 50);
            this.gpOptions.Name = "gpOptions";
            this.gpOptions.Size = new System.Drawing.Size(283, 118);
            // 
            // 
            // 
            this.gpOptions.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpOptions.Style.BackColorGradientAngle = 90;
            this.gpOptions.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpOptions.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpOptions.Style.BorderBottomWidth = 1;
            this.gpOptions.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpOptions.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpOptions.Style.BorderLeftWidth = 1;
            this.gpOptions.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpOptions.Style.BorderRightWidth = 1;
            this.gpOptions.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpOptions.Style.BorderTopWidth = 1;
            this.gpOptions.Style.CornerDiameter = 4;
            this.gpOptions.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpOptions.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpOptions.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.gpOptions.TabIndex = 4;
            this.gpOptions.Text = "複製授課教師及評量設定";
            this.gpOptions.Visible = false;
            // 
            // chkCopyAssessmentSetup
            // 
            this.chkCopyAssessmentSetup.AutoSize = true;
            this.chkCopyAssessmentSetup.BackColor = System.Drawing.Color.Transparent;
            this.chkCopyAssessmentSetup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.chkCopyAssessmentSetup.Location = new System.Drawing.Point(11, 62);
            this.chkCopyAssessmentSetup.Name = "chkCopyAssessmentSetup";
            this.chkCopyAssessmentSetup.Size = new System.Drawing.Size(105, 21);
            this.chkCopyAssessmentSetup.TabIndex = 5;
            this.chkCopyAssessmentSetup.Text = "複製評量設定";
            this.chkCopyAssessmentSetup.UseVisualStyleBackColor = true;
            this.chkCopyAssessmentSetup.CheckedChanged += new System.EventHandler(this.chkCopyAssessmentSetup_CheckedChanged);
            // 
            // chkCopyTeacher
            // 
            this.chkCopyTeacher.AutoSize = true;
            this.chkCopyTeacher.BackColor = System.Drawing.Color.Transparent;
            this.chkCopyTeacher.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.chkCopyTeacher.Location = new System.Drawing.Point(11, 35);
            this.chkCopyTeacher.Name = "chkCopyTeacher";
            this.chkCopyTeacher.Size = new System.Drawing.Size(105, 21);
            this.chkCopyTeacher.TabIndex = 5;
            this.chkCopyTeacher.Text = "複製授課教師";
            this.chkCopyTeacher.UseVisualStyleBackColor = true;
            this.chkCopyTeacher.CheckedChanged += new System.EventHandler(this.chkCopyTeacher_CheckedChanged);
            // 
            // cboPreviousSemester
            // 
            this.cboPreviousSemester.DisplayMember = "Text";
            this.cboPreviousSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboPreviousSemester.Enabled = false;
            this.cboPreviousSemester.FormattingEnabled = true;
            this.cboPreviousSemester.ItemHeight = 19;
            this.cboPreviousSemester.Items.AddRange(new object[] {
            this.comboItem3,
            this.comboItem4});
            this.cboPreviousSemester.Location = new System.Drawing.Point(207, 4);
            this.cboPreviousSemester.Name = "cboPreviousSemester";
            this.cboPreviousSemester.Size = new System.Drawing.Size(62, 25);
            this.cboPreviousSemester.TabIndex = 5;
            // 
            // comboItem3
            // 
            this.comboItem3.Text = "1";
            // 
            // comboItem4
            // 
            this.comboItem4.Text = "2";
            // 
            // cboPreviousSchoolYear
            // 
            this.cboPreviousSchoolYear.DisplayMember = "Text";
            this.cboPreviousSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboPreviousSchoolYear.Enabled = false;
            this.cboPreviousSchoolYear.FormattingEnabled = true;
            this.cboPreviousSchoolYear.ItemHeight = 19;
            this.cboPreviousSchoolYear.Location = new System.Drawing.Point(73, 4);
            this.cboPreviousSchoolYear.Name = "cboPreviousSchoolYear";
            this.cboPreviousSchoolYear.Size = new System.Drawing.Size(70, 25);
            this.cboPreviousSchoolYear.TabIndex = 4;
            // 
            // labelX3
            // 
            this.labelX3.AutoSize = true;
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            this.labelX3.Location = new System.Drawing.Point(158, 6);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(47, 21);
            this.labelX3.TabIndex = 2;
            this.labelX3.Text = "學期：";
            // 
            // labelX4
            // 
            this.labelX4.AutoSize = true;
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            this.labelX4.Location = new System.Drawing.Point(11, 6);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(60, 21);
            this.labelX4.TabIndex = 3;
            this.labelX4.Text = "學年度：";
            // 
            // chkOption
            // 
            this.chkOption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkOption.AutoSize = true;
            this.chkOption.BackColor = System.Drawing.Color.Transparent;
            this.chkOption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.chkOption.Location = new System.Drawing.Point(11, 54);
            this.chkOption.Name = "chkOption";
            this.chkOption.Size = new System.Drawing.Size(105, 21);
            this.chkOption.TabIndex = 5;
            this.chkOption.Text = "開啟進階選項";
            this.chkOption.UseVisualStyleBackColor = false;
            this.chkOption.CheckedChanged += new System.EventHandler(this.chkOP_CheckedChanged);
            // 
            // SelectSemesterForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(305, 82);
            this.Controls.Add(this.chkOption);
            this.Controls.Add(this.gpOptions);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.cboSchoolYear);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Name = "SelectSemesterForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.gpOptions.ResumeLayout(false);
            this.gpOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.ButtonX btnOK;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.Editors.ComboItem comboItem1;
        private DevComponents.Editors.ComboItem comboItem2;
        private Framework.EnhancedErrorProvider errorProvider1;
        private DevComponents.DotNetBar.Controls.GroupPanel gpOptions;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboPreviousSemester;
        private DevComponents.Editors.ComboItem comboItem3;
        private DevComponents.Editors.ComboItem comboItem4;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboPreviousSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX4;
        private System.Windows.Forms.CheckBox chkOption;
        private System.Windows.Forms.CheckBox chkCopyAssessmentSetup;
        private System.Windows.Forms.CheckBox chkCopyTeacher;
    }
}