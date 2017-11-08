namespace JHEvaluation.AssignmentExamManager
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cboDomain = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.dgvInfo = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.課程編號 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chDomainName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chCourseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chTeacherName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chExamCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chFinishedCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.btnDetailView = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.chkCondition = new System.Windows.Forms.CheckBox();
            this.intTimes = new DevComponents.Editors.IntegerInput();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnQuery = new DevComponents.DotNetBar.ButtonX();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intTimes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(60, 8);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(80, 25);
            this.cboSchoolYear.TabIndex = 1;
            this.cboSchoolYear.TextChanged += new System.EventHandler(this.cboSchoolYearSemester_TextChanged);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            this.labelX1.Location = new System.Drawing.Point(7, 9);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(47, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "學年度";
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(211, 8);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(60, 25);
            this.cboSemester.TabIndex = 3;
            this.cboSemester.TextChanged += new System.EventHandler(this.cboSchoolYearSemester_TextChanged);
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            this.labelX2.Location = new System.Drawing.Point(169, 9);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(32, 23);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "學期";
            // 
            // cboDomain
            // 
            this.cboDomain.DisplayMember = "Text";
            this.cboDomain.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboDomain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDomain.FormattingEnabled = true;
            this.cboDomain.ItemHeight = 19;
            this.cboDomain.Location = new System.Drawing.Point(60, 42);
            this.cboDomain.Name = "cboDomain";
            this.cboDomain.Size = new System.Drawing.Size(211, 25);
            this.cboDomain.TabIndex = 5;
            this.cboDomain.TextChanged += new System.EventHandler(this.cboSchoolYearSemester_TextChanged);
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            this.labelX3.Location = new System.Drawing.Point(7, 43);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(38, 23);
            this.labelX3.TabIndex = 4;
            this.labelX3.Text = "領域";
            // 
            // dgvInfo
            // 
            this.dgvInfo.AllowUserToAddRows = false;
            this.dgvInfo.AllowUserToDeleteRows = false;
            this.dgvInfo.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.AliceBlue;
            this.dgvInfo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInfo.BackgroundColor = System.Drawing.Color.White;
            this.dgvInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.課程編號,
            this.chDomainName,
            this.chCourseName,
            this.chTeacherName,
            this.chExamCount,
            this.chFinishedCount});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvInfo.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvInfo.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvInfo.Location = new System.Drawing.Point(7, 77);
            this.dgvInfo.Name = "dgvInfo";
            this.dgvInfo.RowHeadersVisible = false;
            this.dgvInfo.RowTemplate.Height = 24;
            this.dgvInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInfo.Size = new System.Drawing.Size(629, 397);
            this.dgvInfo.TabIndex = 9;
            // 
            // 課程編號
            // 
            this.課程編號.HeaderText = "課程編號";
            this.課程編號.Name = "課程編號";
            this.課程編號.Visible = false;
            // 
            // chDomainName
            // 
            this.chDomainName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.chDomainName.FillWeight = 35F;
            this.chDomainName.HeaderText = "領域名稱";
            this.chDomainName.Name = "chDomainName";
            this.chDomainName.ReadOnly = true;
            // 
            // chCourseName
            // 
            this.chCourseName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.chCourseName.FillWeight = 42F;
            this.chCourseName.HeaderText = "課程名稱";
            this.chCourseName.Name = "chCourseName";
            this.chCourseName.ReadOnly = true;
            // 
            // chTeacherName
            // 
            this.chTeacherName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.chTeacherName.FillWeight = 23F;
            this.chTeacherName.HeaderText = "授課教師";
            this.chTeacherName.Name = "chTeacherName";
            this.chTeacherName.ReadOnly = true;
            // 
            // chExamCount
            // 
            this.chExamCount.HeaderText = "小考次數";
            this.chExamCount.Name = "chExamCount";
            this.chExamCount.ReadOnly = true;
            this.chExamCount.Width = 110;
            // 
            // chFinishedCount
            // 
            this.chFinishedCount.HeaderText = "完整輸入次數";
            this.chFinishedCount.Name = "chFinishedCount";
            this.chFinishedCount.ReadOnly = true;
            this.chFinishedCount.Width = 110;
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Location = new System.Drawing.Point(338, 480);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(80, 25);
            this.btnPrint.TabIndex = 10;
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnDetailView
            // 
            this.btnDetailView.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDetailView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDetailView.BackColor = System.Drawing.Color.Transparent;
            this.btnDetailView.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnDetailView.Location = new System.Drawing.Point(427, 480);
            this.btnDetailView.Name = "btnDetailView";
            this.btnDetailView.Size = new System.Drawing.Size(120, 25);
            this.btnDetailView.TabIndex = 11;
            this.btnDetailView.Text = "檢視輸入細項";
            this.btnDetailView.Click += new System.EventHandler(this.btnDetailView_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(556, 480);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(80, 25);
            this.btnExit.TabIndex = 12;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // chkCondition
            // 
            this.chkCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCondition.AutoSize = true;
            this.chkCondition.BackColor = System.Drawing.Color.Transparent;
            this.chkCondition.Location = new System.Drawing.Point(350, 44);
            this.chkCondition.Name = "chkCondition";
            this.chkCondition.Size = new System.Drawing.Size(105, 21);
            this.chkCondition.TabIndex = 6;
            this.chkCondition.Text = "完整輸入次數";
            this.chkCondition.UseVisualStyleBackColor = false;
            // 
            // intTimes
            // 
            this.intTimes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.intTimes.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intTimes.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intTimes.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intTimes.Location = new System.Drawing.Point(492, 41);
            this.intTimes.MaxValue = 99;
            this.intTimes.MinValue = 1;
            this.intTimes.Name = "intTimes";
            this.intTimes.ShowUpDown = true;
            this.intTimes.Size = new System.Drawing.Size(55, 25);
            this.intTimes.TabIndex = 8;
            this.intTimes.Value = 1;
            // 
            // labelX4
            // 
            this.labelX4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            this.labelX4.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX4.Location = new System.Drawing.Point(455, 42);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(35, 23);
            this.labelX4.TabIndex = 7;
            this.labelX4.Text = "小於";
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkRate = 0;
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // btnQuery
            // 
            this.btnQuery.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.BackColor = System.Drawing.Color.Transparent;
            this.btnQuery.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnQuery.Location = new System.Drawing.Point(556, 43);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(80, 25);
            this.btnQuery.TabIndex = 13;
            this.btnQuery.Text = "查詢";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 512);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.intTimes);
            this.Controls.Add(this.chkCondition);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnDetailView);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.dgvInfo);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.cboDomain);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.cboSchoolYear);
            this.MaximizeBox = true;
            this.MinimumSize = new System.Drawing.Size(615, 550);
            this.Name = "MainForm";
            this.Text = "小考輸入狀況檢視";
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intTimes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboDomain;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgvInfo;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private DevComponents.DotNetBar.ButtonX btnDetailView;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private System.Windows.Forms.CheckBox chkCondition;
        private DevComponents.Editors.IntegerInput intTimes;
        private DevComponents.DotNetBar.LabelX labelX4;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private DevComponents.DotNetBar.ButtonX btnQuery;
        private System.Windows.Forms.DataGridViewTextBoxColumn 課程編號;
        private System.Windows.Forms.DataGridViewTextBoxColumn chDomainName;
        private System.Windows.Forms.DataGridViewTextBoxColumn chCourseName;
        private System.Windows.Forms.DataGridViewTextBoxColumn chTeacherName;
        private System.Windows.Forms.DataGridViewTextBoxColumn chExamCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn chFinishedCount;
    }
}

