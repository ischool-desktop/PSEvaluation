using Framework;
namespace JHEvaluation.ExamScoreCopy
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cboExam = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.cboSource = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.gpCopy = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.dgvTargets = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.chTargetSubjects = new JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ProgramPlanRelated.DataGridViewComboBoxExColumn();
            this.btnStart = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.errorProvider = new Framework.EnhancedErrorProvider();
            this.gpCopy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTargets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(81, 12);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(81, 25);
            this.cboSchoolYear.TabIndex = 1;
            this.cboSchoolYear.TextChanged += new System.EventHandler(this.cboSchoolYearSemester_TextChanged);
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(223, 12);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(63, 25);
            this.cboSemester.TabIndex = 3;
            this.cboSemester.TextChanged += new System.EventHandler(this.cboSchoolYearSemester_TextChanged);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            this.labelX1.Location = new System.Drawing.Point(26, 13);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(46, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "學年度";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            this.labelX2.Location = new System.Drawing.Point(181, 13);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(39, 23);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "學期";
            // 
            // cboExam
            // 
            this.cboExam.DisplayMember = "Name";
            this.cboExam.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboExam.FormattingEnabled = true;
            this.cboExam.ItemHeight = 19;
            this.cboExam.Location = new System.Drawing.Point(86, 10);
            this.cboExam.Name = "cboExam";
            this.cboExam.Size = new System.Drawing.Size(170, 25);
            this.cboExam.TabIndex = 1;
            this.cboExam.ValueMember = "ID";
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            this.labelX3.Location = new System.Drawing.Point(18, 11);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(60, 23);
            this.labelX3.TabIndex = 0;
            this.labelX3.Text = "試別名稱";
            // 
            // cboSource
            // 
            this.cboSource.DisplayMember = "Text";
            this.cboSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSource.FormattingEnabled = true;
            this.cboSource.ItemHeight = 19;
            this.cboSource.Location = new System.Drawing.Point(86, 52);
            this.cboSource.Name = "cboSource";
            this.cboSource.Size = new System.Drawing.Size(170, 25);
            this.cboSource.TabIndex = 3;
            // 
            // labelX4
            // 
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            this.labelX4.Location = new System.Drawing.Point(18, 53);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(60, 23);
            this.labelX4.TabIndex = 2;
            this.labelX4.Text = "來源科目";
            // 
            // labelX5
            // 
            this.labelX5.BackColor = System.Drawing.Color.Transparent;
            this.labelX5.Location = new System.Drawing.Point(18, 95);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(60, 23);
            this.labelX5.TabIndex = 4;
            this.labelX5.Text = "目的科目";
            // 
            // gpCopy
            // 
            this.gpCopy.BackColor = System.Drawing.Color.Transparent;
            this.gpCopy.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpCopy.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpCopy.Controls.Add(this.dgvTargets);
            this.gpCopy.Controls.Add(this.labelX3);
            this.gpCopy.Controls.Add(this.labelX5);
            this.gpCopy.Controls.Add(this.cboExam);
            this.gpCopy.Controls.Add(this.labelX4);
            this.gpCopy.Controls.Add(this.cboSource);
            this.gpCopy.DrawTitleBox = false;
            this.gpCopy.Enabled = false;
            this.gpCopy.Location = new System.Drawing.Point(16, 49);
            this.gpCopy.Name = "gpCopy";
            this.gpCopy.Size = new System.Drawing.Size(280, 250);
            // 
            // 
            // 
            this.gpCopy.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpCopy.Style.BackColorGradientAngle = 90;
            this.gpCopy.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpCopy.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpCopy.Style.BorderBottomWidth = 1;
            this.gpCopy.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpCopy.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpCopy.Style.BorderLeftWidth = 1;
            this.gpCopy.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpCopy.Style.BorderRightWidth = 1;
            this.gpCopy.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpCopy.Style.BorderTopWidth = 1;
            this.gpCopy.Style.CornerDiameter = 4;
            this.gpCopy.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpCopy.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpCopy.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.gpCopy.TabIndex = 4;
            this.gpCopy.Text = "選擇成績複製來源及目的";
            // 
            // dgvTargets
            // 
            this.dgvTargets.AllowUserToResizeColumns = false;
            this.dgvTargets.AllowUserToResizeRows = false;
            this.dgvTargets.BackgroundColor = System.Drawing.Color.White;
            this.dgvTargets.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvTargets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTargets.ColumnHeadersVisible = false;
            this.dgvTargets.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chTargetSubjects});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTargets.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTargets.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvTargets.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvTargets.Location = new System.Drawing.Point(86, 95);
            this.dgvTargets.Name = "dgvTargets";
            this.dgvTargets.RowHeadersVisible = false;
            this.dgvTargets.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvTargets.Size = new System.Drawing.Size(170, 112);
            this.dgvTargets.TabIndex = 5;
            // 
            // chTargetSubjects
            // 
            this.chTargetSubjects.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.chTargetSubjects.HeaderText = "目的科目";
            this.chTargetSubjects.Items = ((System.Collections.Generic.List<string>)(resources.GetObject("chTargetSubjects.Items")));
            this.chTargetSubjects.Name = "chTargetSubjects";
            this.chTargetSubjects.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // btnStart
            // 
            this.btnStart.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnStart.BackColor = System.Drawing.Color.Transparent;
            this.btnStart.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(133, 309);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 25);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "預覽";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(221, 309);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 25);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkRate = 0;
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 347);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.gpCopy);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.cboSchoolYear);
            this.Name = "MainForm";
            this.ShowInTaskbar = true;
            this.Text = "評量成績複製";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.gpCopy.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTargets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboExam;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSource;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.Controls.GroupPanel gpCopy;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgvTargets;
        private DevComponents.DotNetBar.ButtonX btnStart;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private EnhancedErrorProvider errorProvider;
        private JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ProgramPlanRelated.DataGridViewComboBoxExColumn chTargetSubjects;
    }
}

