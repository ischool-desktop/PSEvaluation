namespace JHEvaluation.ClassSemesterScoreAvgComparison
{
    partial class ClassSemsScoreAvgCmpForm
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cbxSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cbxSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.lstSubj = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.lstDomain = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.gpSubj = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gpDomain = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.cbxNotRankTag = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.gpSubj.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gpDomain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(13, 24);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(57, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "學年度";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(165, 24);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(46, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "學期";
            // 
            // cbxSchoolYear
            // 
            this.cbxSchoolYear.DisplayMember = "Text";
            this.cbxSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSchoolYear.FormattingEnabled = true;
            this.cbxSchoolYear.ItemHeight = 19;
            this.cbxSchoolYear.Location = new System.Drawing.Point(63, 24);
            this.cbxSchoolYear.Name = "cbxSchoolYear";
            this.cbxSchoolYear.Size = new System.Drawing.Size(80, 25);
            this.cbxSchoolYear.TabIndex = 2;
            this.cbxSchoolYear.SelectedIndexChanged += new System.EventHandler(this.cbxSchoolYear_SelectedIndexChanged);
            // 
            // cbxSemester
            // 
            this.cbxSemester.DisplayMember = "Text";
            this.cbxSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSemester.FormattingEnabled = true;
            this.cbxSemester.ItemHeight = 19;
            this.cbxSemester.Location = new System.Drawing.Point(205, 24);
            this.cbxSemester.Name = "cbxSemester";
            this.cbxSemester.Size = new System.Drawing.Size(64, 25);
            this.cbxSemester.TabIndex = 3;
            this.cbxSemester.SelectedIndexChanged += new System.EventHandler(this.cbxSemester_SelectedIndexChanged);
            // 
            // lstSubj
            // 
            // 
            // 
            // 
            this.lstSubj.Border.Class = "ListViewBorder";
            this.lstSubj.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lstSubj.CheckBoxes = true;
            this.lstSubj.FullRowSelect = true;
            this.lstSubj.Location = new System.Drawing.Point(3, 3);
            this.lstSubj.Name = "lstSubj";
            this.lstSubj.Size = new System.Drawing.Size(240, 174);
            this.lstSubj.TabIndex = 4;
            this.lstSubj.UseCompatibleStateImageBehavior = false;
            this.lstSubj.View = System.Windows.Forms.View.List;
            // 
            // lstDomain
            // 
            // 
            // 
            // 
            this.lstDomain.Border.Class = "ListViewBorder";
            this.lstDomain.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lstDomain.CheckBoxes = true;
            this.lstDomain.FullRowSelect = true;
            this.lstDomain.Location = new System.Drawing.Point(3, 3);
            this.lstDomain.Name = "lstDomain";
            this.lstDomain.Size = new System.Drawing.Size(185, 174);
            this.lstDomain.TabIndex = 5;
            this.lstDomain.UseCompatibleStateImageBehavior = false;
            this.lstDomain.View = System.Windows.Forms.View.List;
            // 
            // gpSubj
            // 
            this.gpSubj.BackColor = System.Drawing.Color.Transparent;
            this.gpSubj.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpSubj.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpSubj.Controls.Add(this.pictureBox1);
            this.gpSubj.Controls.Add(this.lstSubj);
            this.gpSubj.Location = new System.Drawing.Point(13, 86);
            this.gpSubj.Name = "gpSubj";
            this.gpSubj.Size = new System.Drawing.Size(252, 207);
            // 
            // 
            // 
            this.gpSubj.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpSubj.Style.BackColorGradientAngle = 90;
            this.gpSubj.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpSubj.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpSubj.Style.BorderBottomWidth = 1;
            this.gpSubj.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpSubj.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpSubj.Style.BorderLeftWidth = 1;
            this.gpSubj.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpSubj.Style.BorderRightWidth = 1;
            this.gpSubj.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpSubj.Style.BorderTopWidth = 1;
            this.gpSubj.Style.Class = "";
            this.gpSubj.Style.CornerDiameter = 4;
            this.gpSubj.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpSubj.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpSubj.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpSubj.StyleMouseDown.Class = "";
            this.gpSubj.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpSubj.StyleMouseOver.Class = "";
            this.gpSubj.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpSubj.TabIndex = 6;
            this.gpSubj.Text = "科目";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImage = global::JHEvaluation.ClassSemesterScoreAvgComparison.Properties.Resources.loading;
            this.pictureBox1.Location = new System.Drawing.Point(109, 69);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 34);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // gpDomain
            // 
            this.gpDomain.BackColor = System.Drawing.Color.Transparent;
            this.gpDomain.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpDomain.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpDomain.Controls.Add(this.pictureBox2);
            this.gpDomain.Controls.Add(this.lstDomain);
            this.gpDomain.Location = new System.Drawing.Point(271, 86);
            this.gpDomain.Name = "gpDomain";
            this.gpDomain.Size = new System.Drawing.Size(198, 207);
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
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.BackgroundImage = global::JHEvaluation.ClassSemesterScoreAvgComparison.Properties.Resources.loading;
            this.pictureBox2.Location = new System.Drawing.Point(80, 69);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 34);
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Location = new System.Drawing.Point(292, 299);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 8;
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(397, 299);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(70, 23);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // cbxNotRankTag
            // 
            this.cbxNotRankTag.DisplayMember = "Text";
            this.cbxNotRankTag.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxNotRankTag.FormattingEnabled = true;
            this.cbxNotRankTag.ItemHeight = 19;
            this.cbxNotRankTag.Location = new System.Drawing.Point(110, 55);
            this.cbxNotRankTag.Name = "cbxNotRankTag";
            this.cbxNotRankTag.Size = new System.Drawing.Size(159, 25);
            this.cbxNotRankTag.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbxNotRankTag.TabIndex = 14;
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
            this.labelX4.Location = new System.Drawing.Point(13, 55);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(101, 21);
            this.labelX4.TabIndex = 13;
            this.labelX4.Text = "不排名學生類別";
            // 
            // ClassSemsScoreAvgCmpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 334);
            this.Controls.Add(this.cbxNotRankTag);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.gpDomain);
            this.Controls.Add(this.gpSubj);
            this.Controls.Add(this.cbxSemester);
            this.Controls.Add(this.cbxSchoolYear);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(497, 373);
            this.MinimumSize = new System.Drawing.Size(497, 373);
            this.Name = "ClassSemsScoreAvgCmpForm";
            this.Text = "班級學期成績平均比較表";
            this.Load += new System.EventHandler(this.ClassSemsScoreAvgCmpForm_Load);
            this.gpSubj.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gpDomain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxSemester;
        private DevComponents.DotNetBar.Controls.ListViewEx lstSubj;
        private DevComponents.DotNetBar.Controls.ListViewEx lstDomain;
        private DevComponents.DotNetBar.Controls.GroupPanel gpSubj;
        private DevComponents.DotNetBar.Controls.GroupPanel gpDomain;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxNotRankTag;
        private DevComponents.DotNetBar.LabelX labelX4;
    }
}