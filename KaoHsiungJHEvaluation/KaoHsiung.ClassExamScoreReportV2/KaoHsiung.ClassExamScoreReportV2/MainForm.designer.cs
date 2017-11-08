namespace KaoHsiung.ClassExamScoreReportV2
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Group1", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Group2", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Group1", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Group2", System.Windows.Forms.HorizontalAlignment.Left);
            this.gpSubject = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.plSubject = new System.Windows.Forms.Panel();
            this.LoadingSubject = new System.Windows.Forms.PictureBox();
            this.lvSubject = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.lnConfig = new System.Windows.Forms.LinkLabel();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LoadingDomain = new System.Windows.Forms.PictureBox();
            this.lvDomain = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.labelX8 = new DevComponents.DotNetBar.LabelX();
            this.cbExam = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.gpSubject.SuspendLayout();
            this.plSubject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadingSubject)).BeginInit();
            this.groupPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadingDomain)).BeginInit();
            this.SuspendLayout();
            // 
            // gpSubject
            // 
            this.gpSubject.BackColor = System.Drawing.Color.Transparent;
            this.gpSubject.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpSubject.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpSubject.Controls.Add(this.plSubject);
            this.gpSubject.Location = new System.Drawing.Point(11, 77);
            this.gpSubject.Name = "gpSubject";
            this.gpSubject.Size = new System.Drawing.Size(266, 252);
            // 
            // 
            // 
            this.gpSubject.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpSubject.Style.BackColorGradientAngle = 90;
            this.gpSubject.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpSubject.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpSubject.Style.BorderBottomWidth = 1;
            this.gpSubject.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpSubject.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpSubject.Style.BorderLeftWidth = 1;
            this.gpSubject.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpSubject.Style.BorderRightWidth = 1;
            this.gpSubject.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpSubject.Style.BorderTopWidth = 1;
            this.gpSubject.Style.Class = "";
            this.gpSubject.Style.CornerDiameter = 4;
            this.gpSubject.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpSubject.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpSubject.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpSubject.StyleMouseDown.Class = "";
            this.gpSubject.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpSubject.StyleMouseOver.Class = "";
            this.gpSubject.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpSubject.TabIndex = 6;
            this.gpSubject.Text = "科目";
            // 
            // plSubject
            // 
            this.plSubject.Controls.Add(this.LoadingSubject);
            this.plSubject.Controls.Add(this.lvSubject);
            this.plSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plSubject.Location = new System.Drawing.Point(0, 0);
            this.plSubject.Name = "plSubject";
            this.plSubject.Padding = new System.Windows.Forms.Padding(3);
            this.plSubject.Size = new System.Drawing.Size(260, 225);
            this.plSubject.TabIndex = 0;
            // 
            // LoadingSubject
            // 
            this.LoadingSubject.BackColor = System.Drawing.Color.White;
            this.LoadingSubject.Image = global::KaoHsiung.ClassExamScoreReportV2.Prc.loading;
            this.LoadingSubject.Location = new System.Drawing.Point(114, 96);
            this.LoadingSubject.Name = "LoadingSubject";
            this.LoadingSubject.Size = new System.Drawing.Size(32, 32);
            this.LoadingSubject.TabIndex = 1;
            this.LoadingSubject.TabStop = false;
            // 
            // lvSubject
            // 
            this.lvSubject.AutoArrange = false;
            // 
            // 
            // 
            this.lvSubject.Border.Class = "ListViewBorder";
            this.lvSubject.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvSubject.CheckBoxes = true;
            this.lvSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewGroup1.Header = "Group1";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup2.Header = "Group2";
            listViewGroup2.Name = "listViewGroup2";
            this.lvSubject.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.lvSubject.Location = new System.Drawing.Point(3, 3);
            this.lvSubject.Name = "lvSubject";
            this.lvSubject.Size = new System.Drawing.Size(254, 219);
            this.lvSubject.TabIndex = 0;
            this.lvSubject.Tag = "";
            this.lvSubject.UseCompatibleStateImageBehavior = false;
            this.lvSubject.View = System.Windows.Forms.View.List;
            // 
            // lnConfig
            // 
            this.lnConfig.AutoSize = true;
            this.lnConfig.BackColor = System.Drawing.Color.Transparent;
            this.lnConfig.Location = new System.Drawing.Point(6, 337);
            this.lnConfig.Name = "lnConfig";
            this.lnConfig.Size = new System.Drawing.Size(125, 17);
            this.lnConfig.TabIndex = 8;
            this.lnConfig.TabStop = true;
            this.lnConfig.Tag = "StatusVarying";
            this.lnConfig.Text = "列印項目及排名設定";
            this.lnConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnConfig_LinkClicked);
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Enabled = false;
            this.btnPrint.Location = new System.Drawing.Point(299, 335);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 9;
            this.btnPrint.Tag = "StatusVarying";
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(380, 335);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(65, 23);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.panel1);
            this.groupPanel1.Location = new System.Drawing.Point(285, 77);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(160, 252);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.Class = "";
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.Class = "";
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.Class = "";
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 7;
            this.groupPanel1.Text = "領域";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LoadingDomain);
            this.panel1.Controls.Add(this.lvDomain);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(154, 225);
            this.panel1.TabIndex = 0;
            // 
            // LoadingDomain
            // 
            this.LoadingDomain.BackColor = System.Drawing.Color.White;
            this.LoadingDomain.Image = global::KaoHsiung.ClassExamScoreReportV2.Prc.loading;
            this.LoadingDomain.Location = new System.Drawing.Point(61, 96);
            this.LoadingDomain.Name = "LoadingDomain";
            this.LoadingDomain.Size = new System.Drawing.Size(32, 32);
            this.LoadingDomain.TabIndex = 1;
            this.LoadingDomain.TabStop = false;
            // 
            // lvDomain
            // 
            this.lvDomain.AutoArrange = false;
            // 
            // 
            // 
            this.lvDomain.Border.Class = "ListViewBorder";
            this.lvDomain.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvDomain.CheckBoxes = true;
            this.lvDomain.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewGroup3.Header = "Group1";
            listViewGroup3.Name = "listViewGroup1";
            listViewGroup4.Header = "Group2";
            listViewGroup4.Name = "listViewGroup2";
            this.lvDomain.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
            this.lvDomain.Location = new System.Drawing.Point(3, 3);
            this.lvDomain.Name = "lvDomain";
            this.lvDomain.Size = new System.Drawing.Size(148, 219);
            this.lvDomain.TabIndex = 0;
            this.lvDomain.Tag = "";
            this.lvDomain.UseCompatibleStateImageBehavior = false;
            this.lvDomain.View = System.Windows.Forms.View.List;
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(72, 10);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(98, 25);
            this.cboSchoolYear.TabIndex = 1;
            this.cboSchoolYear.Tag = "StatusVarying";
            // 
            // labelX5
            // 
            this.labelX5.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.Class = "";
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelX5.Location = new System.Drawing.Point(176, 11);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(34, 23);
            this.labelX5.TabIndex = 2;
            this.labelX5.Text = "學期";
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(212, 10);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(103, 25);
            this.cboSemester.TabIndex = 3;
            this.cboSemester.Tag = "StatusVarying";
            // 
            // labelX6
            // 
            this.labelX6.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX6.BackgroundStyle.Class = "";
            this.labelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelX6.Location = new System.Drawing.Point(12, 11);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(47, 23);
            this.labelX6.TabIndex = 0;
            this.labelX6.Text = "學年度";
            // 
            // labelX8
            // 
            this.labelX8.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX8.BackgroundStyle.Class = "";
            this.labelX8.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX8.Location = new System.Drawing.Point(12, 43);
            this.labelX8.Name = "labelX8";
            this.labelX8.Size = new System.Drawing.Size(58, 23);
            this.labelX8.TabIndex = 4;
            this.labelX8.Text = "選擇試別";
            this.labelX8.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // cbExam
            // 
            this.cbExam.DisplayMember = "Text";
            this.cbExam.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbExam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExam.FormattingEnabled = true;
            this.cbExam.ItemHeight = 19;
            this.cbExam.Location = new System.Drawing.Point(72, 42);
            this.cbExam.Name = "cbExam";
            this.cbExam.Size = new System.Drawing.Size(243, 25);
            this.cbExam.TabIndex = 5;
            this.cbExam.Tag = "StatusVarying";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(456, 367);
            this.Controls.Add(this.cboSchoolYear);
            this.Controls.Add(this.labelX5);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.labelX6);
            this.Controls.Add(this.labelX8);
            this.Controls.Add(this.cbExam);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lnConfig);
            this.Controls.Add(this.gpSubject);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "班級評量成績單";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gpSubject.ResumeLayout(false);
            this.plSubject.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LoadingSubject)).EndInit();
            this.groupPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LoadingDomain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.GroupPanel gpSubject;
        private System.Windows.Forms.LinkLabel lnConfig;
        private System.Windows.Forms.Panel plSubject;
        private DevComponents.DotNetBar.Controls.ListViewEx lvSubject;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private System.Windows.Forms.PictureBox LoadingSubject;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox LoadingDomain;
        private DevComponents.DotNetBar.Controls.ListViewEx lvDomain;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.LabelX labelX6;

        private DevComponents.DotNetBar.LabelX labelX8;

        private DevComponents.DotNetBar.Controls.ComboBoxEx cbExam;

    }
}