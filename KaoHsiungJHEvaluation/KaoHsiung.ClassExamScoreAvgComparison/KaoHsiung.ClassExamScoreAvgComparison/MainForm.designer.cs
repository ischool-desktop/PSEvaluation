namespace KaoHsiung.ClassExamScoreAvgComparison
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
            this.cbExam = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.gpSubject = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.plSubject = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lvSubject = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.lnConfig = new System.Windows.Forms.LinkLabel();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.gpDomain = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lvDomain = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.cbxNotRankTag = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.gpSubject.SuspendLayout();
            this.plSubject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gpDomain.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // cbExam
            // 
            this.cbExam.DisplayMember = "Text";
            this.cbExam.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbExam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExam.Enabled = false;
            this.cbExam.FormattingEnabled = true;
            this.cbExam.ItemHeight = 19;
            this.cbExam.Location = new System.Drawing.Point(69, 40);
            this.cbExam.Name = "cbExam";
            this.cbExam.Size = new System.Drawing.Size(236, 25);
            this.cbExam.TabIndex = 5;
            this.cbExam.SelectedIndexChanged += new System.EventHandler(this.cbo_SelectedIndexChanged);
            // 
            // gpSubject
            // 
            this.gpSubject.BackColor = System.Drawing.Color.Transparent;
            this.gpSubject.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpSubject.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpSubject.Controls.Add(this.plSubject);
            this.gpSubject.Location = new System.Drawing.Point(8, 110);
            this.gpSubject.Name = "gpSubject";
            this.gpSubject.Size = new System.Drawing.Size(261, 178);
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
            this.plSubject.Controls.Add(this.pictureBox1);
            this.plSubject.Controls.Add(this.lvSubject);
            this.plSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plSubject.Location = new System.Drawing.Point(0, 0);
            this.plSubject.Name = "plSubject";
            this.plSubject.Padding = new System.Windows.Forms.Padding(3);
            this.plSubject.Size = new System.Drawing.Size(255, 151);
            this.plSubject.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Image = global::KaoHsiung.ClassExamScoreAvgComparison.Resource1.loading;
            this.pictureBox1.Location = new System.Drawing.Point(111, 59);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // lvSubject
            // 
            // 
            // 
            // 
            this.lvSubject.Border.Class = "ListViewBorder";
            this.lvSubject.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvSubject.CheckBoxes = true;
            this.lvSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSubject.Location = new System.Drawing.Point(3, 3);
            this.lvSubject.Name = "lvSubject";
            this.lvSubject.Size = new System.Drawing.Size(249, 145);
            this.lvSubject.TabIndex = 0;
            this.lvSubject.UseCompatibleStateImageBehavior = false;
            this.lvSubject.View = System.Windows.Forms.View.List;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(6, 41);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(58, 23);
            this.labelX1.TabIndex = 4;
            this.labelX1.Text = "選擇試別";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // lnConfig
            // 
            this.lnConfig.AutoSize = true;
            this.lnConfig.BackColor = System.Drawing.Color.Transparent;
            this.lnConfig.Location = new System.Drawing.Point(12, 300);
            this.lnConfig.Name = "lnConfig";
            this.lnConfig.Size = new System.Drawing.Size(125, 17);
            this.lnConfig.TabIndex = 8;
            this.lnConfig.TabStop = true;
            this.lnConfig.Text = "列印項目及排名設定";
            this.lnConfig.Visible = false;
            this.lnConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnConfig_LinkClicked);
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Enabled = false;
            this.btnPrint.Location = new System.Drawing.Point(308, 294);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 9;
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(395, 294);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(65, 23);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.Enabled = false;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(69, 7);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(108, 25);
            this.cboSchoolYear.TabIndex = 1;
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(6, 7);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(48, 23);
            this.labelX2.TabIndex = 0;
            this.labelX2.Text = "學年度";
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.Enabled = false;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(223, 7);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(82, 25);
            this.cboSemester.TabIndex = 3;
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(183, 8);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(36, 23);
            this.labelX3.TabIndex = 2;
            this.labelX3.Text = "學期";
            // 
            // gpDomain
            // 
            this.gpDomain.BackColor = System.Drawing.Color.Transparent;
            this.gpDomain.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpDomain.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpDomain.Controls.Add(this.panel1);
            this.gpDomain.Location = new System.Drawing.Point(275, 110);
            this.gpDomain.Name = "gpDomain";
            this.gpDomain.Size = new System.Drawing.Size(185, 178);
            // 
            // 
            // 
            this.gpDomain.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpDomain.Style.BackColorGradientAngle = 90;
            this.gpDomain.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpDomain.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpDomain.Style.BorderBottomWidth = 1;
            this.gpDomain.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpDomain.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpDomain.Style.BorderLeftWidth = 1;
            this.gpDomain.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpDomain.Style.BorderRightWidth = 1;
            this.gpDomain.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpDomain.Style.BorderTopWidth = 1;
            this.gpDomain.Style.Class = "";
            this.gpDomain.Style.CornerDiameter = 4;
            this.gpDomain.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpDomain.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpDomain.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpDomain.StyleMouseDown.Class = "";
            this.gpDomain.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpDomain.StyleMouseOver.Class = "";
            this.gpDomain.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpDomain.TabIndex = 7;
            this.gpDomain.Text = "領域";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.lvDomain);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(179, 151);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.Image = global::KaoHsiung.ClassExamScoreAvgComparison.Resource1.loading;
            this.pictureBox2.Location = new System.Drawing.Point(73, 59);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // lvDomain
            // 
            // 
            // 
            // 
            this.lvDomain.Border.Class = "ListViewBorder";
            this.lvDomain.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvDomain.CheckBoxes = true;
            this.lvDomain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDomain.Location = new System.Drawing.Point(3, 3);
            this.lvDomain.Name = "lvDomain";
            this.lvDomain.Size = new System.Drawing.Size(173, 145);
            this.lvDomain.TabIndex = 0;
            this.lvDomain.UseCompatibleStateImageBehavior = false;
            this.lvDomain.View = System.Windows.Forms.View.List;
            // 
            // cbxNotRankTag
            // 
            this.cbxNotRankTag.DisplayMember = "Text";
            this.cbxNotRankTag.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxNotRankTag.FormattingEnabled = true;
            this.cbxNotRankTag.ItemHeight = 19;
            this.cbxNotRankTag.Location = new System.Drawing.Point(103, 73);
            this.cbxNotRankTag.Name = "cbxNotRankTag";
            this.cbxNotRankTag.Size = new System.Drawing.Size(202, 25);
            this.cbxNotRankTag.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbxNotRankTag.TabIndex = 12;
            this.cbxNotRankTag.SelectedIndexChanged += new System.EventHandler(this.cbxNotRankTag_SelectedIndexChanged);
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
            this.labelX4.Location = new System.Drawing.Point(6, 73);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(101, 21);
            this.labelX4.TabIndex = 11;
            this.labelX4.Text = "不排名學生類別";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(466, 320);
            this.Controls.Add(this.cbxNotRankTag);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.cboSchoolYear);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lnConfig);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.gpDomain);
            this.Controls.Add(this.gpSubject);
            this.Controls.Add(this.cbExam);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximumSize = new System.Drawing.Size(482, 359);
            this.MinimumSize = new System.Drawing.Size(482, 359);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "";
            this.gpSubject.ResumeLayout(false);
            this.plSubject.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gpDomain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ComboBoxEx cbExam;
        private DevComponents.DotNetBar.Controls.GroupPanel gpSubject;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.LinkLabel lnConfig;
        private System.Windows.Forms.Panel plSubject;
        private DevComponents.DotNetBar.Controls.ListViewEx lvSubject;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.GroupPanel gpDomain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private DevComponents.DotNetBar.Controls.ListViewEx lvDomain;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxNotRankTag;
        private DevComponents.DotNetBar.LabelX labelX4;

    }
}

