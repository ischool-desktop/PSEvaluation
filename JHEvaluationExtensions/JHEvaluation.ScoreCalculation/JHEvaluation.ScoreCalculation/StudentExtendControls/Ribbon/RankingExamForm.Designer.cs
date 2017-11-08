namespace JHSchool.Evaluation.StudentExtendControls.Ribbon
{
    partial class RankingExamForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.chCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.chPeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chAttendCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cboExam = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.btnRankingOption = new DevComponents.DotNetBar.ButtonX();
            this.btnRank = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbCombine = new System.Windows.Forms.RadioButton();
            this.rbSeparate = new System.Windows.Forms.RadioButton();
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.rbAverage = new System.Windows.Forms.RadioButton();
            this.rbTotal = new System.Windows.Forms.RadioButton();
            this.rbWeightAverage = new System.Windows.Forms.RadioButton();
            this.rbWeightTotal = new System.Windows.Forms.RadioButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.chkPrintByClass = new System.Windows.Forms.CheckBox();
            this.rbGradeYear = new System.Windows.Forms.RadioButton();
            this.rbClass = new System.Windows.Forms.RadioButton();
            this.panelEx2 = new DevComponents.DotNetBar.PanelEx();
            this.txtTopRank = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label1 = new System.Windows.Forms.Label();
            this.rbTopRank = new System.Windows.Forms.RadioButton();
            this.rbAllRank = new System.Windows.Forms.RadioButton();
            this.errorProvider = new Framework.EnhancedErrorProvider();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSelectedCount = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panelEx1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panelEx2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chCheck,
            this.chPeriod,
            this.chCategory,
            this.chAttendCount});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgv.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgv.Location = new System.Drawing.Point(7, 69);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(323, 303);
            this.dgv.TabIndex = 0;
            this.dgv.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgv_RowsAdded);
            this.dgv.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellEndEdit);
            // 
            // chCheck
            // 
            this.chCheck.HeaderText = "";
            this.chCheck.Name = "chCheck";
            this.chCheck.Width = 30;
            // 
            // chPeriod
            // 
            this.chPeriod.HeaderText = "權數";
            this.chPeriod.Name = "chPeriod";
            this.chPeriod.Width = 50;
            // 
            // chCategory
            // 
            this.chCategory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.chCategory.DefaultCellStyle = dataGridViewCellStyle2;
            this.chCategory.HeaderText = "";
            this.chCategory.Name = "chCategory";
            this.chCategory.ReadOnly = true;
            // 
            // chAttendCount
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.chAttendCount.DefaultCellStyle = dataGridViewCellStyle3;
            this.chAttendCount.HeaderText = "修習人數";
            this.chAttendCount.Name = "chAttendCount";
            this.chAttendCount.ReadOnly = true;
            this.chAttendCount.Width = 75;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.cboSchoolYear);
            this.panel1.Controls.Add(this.labelX3);
            this.panel1.Controls.Add(this.labelX1);
            this.panel1.Controls.Add(this.cboSemester);
            this.panel1.Controls.Add(this.cboExam);
            this.panel1.Controls.Add(this.labelX2);
            this.panel1.Location = new System.Drawing.Point(7, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(323, 60);
            this.panel1.TabIndex = 1;
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(53, 3);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(88, 25);
            this.cboSchoolYear.TabIndex = 0;
            this.cboSchoolYear.TextChanged += new System.EventHandler(this.cboSchoolYear_TextChanged);
            // 
            // labelX3
            // 
            this.labelX3.Location = new System.Drawing.Point(160, 4);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(34, 23);
            this.labelX3.TabIndex = 1;
            this.labelX3.Text = "學期";
            // 
            // labelX1
            // 
            this.labelX1.Location = new System.Drawing.Point(3, 33);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(72, 23);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "請選擇評量";
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(196, 3);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(103, 25);
            this.cboSemester.TabIndex = 0;
            this.cboSemester.TextChanged += new System.EventHandler(this.cboSemester_TextChanged);
            // 
            // cboExam
            // 
            this.cboExam.DisplayMember = "Text";
            this.cboExam.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboExam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExam.FormattingEnabled = true;
            this.cboExam.ItemHeight = 19;
            this.cboExam.Location = new System.Drawing.Point(79, 33);
            this.cboExam.Name = "cboExam";
            this.cboExam.Size = new System.Drawing.Size(220, 25);
            this.cboExam.TabIndex = 0;
            this.cboExam.SelectedIndexChanged += new System.EventHandler(this.cboExam_SelectedIndexChanged);
            // 
            // labelX2
            // 
            this.labelX2.Location = new System.Drawing.Point(3, 4);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(47, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "學年度";
            // 
            // btnRankingOption
            // 
            this.btnRankingOption.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRankingOption.BackColor = System.Drawing.Color.Transparent;
            this.btnRankingOption.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRankingOption.Location = new System.Drawing.Point(7, 378);
            this.btnRankingOption.Name = "btnRankingOption";
            this.btnRankingOption.Size = new System.Drawing.Size(75, 23);
            this.btnRankingOption.TabIndex = 2;
            this.btnRankingOption.Text = "排名設定";
            this.btnRankingOption.Click += new System.EventHandler(this.btnRankingOption_Click);
            // 
            // btnRank
            // 
            this.btnRank.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRank.BackColor = System.Drawing.Color.Transparent;
            this.btnRank.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRank.Location = new System.Drawing.Point(521, 378);
            this.btnRank.Name = "btnRank";
            this.btnRank.Size = new System.Drawing.Size(75, 23);
            this.btnRank.TabIndex = 2;
            this.btnRank.Text = "排名";
            this.btnRank.Click += new System.EventHandler(this.btnRank_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(603, 378);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.panel3);
            this.groupPanel1.Controls.Add(this.panel4);
            this.groupPanel1.Location = new System.Drawing.Point(336, 6);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(342, 366);
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
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.groupPanel1.TabIndex = 3;
            this.groupPanel1.Text = "排名項目";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbCombine);
            this.panel3.Controls.Add(this.rbSeparate);
            this.panel3.Controls.Add(this.panelEx1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(336, 224);
            this.panel3.TabIndex = 0;
            // 
            // rbCombine
            // 
            this.rbCombine.AutoSize = true;
            this.rbCombine.Location = new System.Drawing.Point(19, 45);
            this.rbCombine.Name = "rbCombine";
            this.rbCombine.Size = new System.Drawing.Size(206, 21);
            this.rbCombine.TabIndex = 1;
            this.rbCombine.Text = "運算排名 (多選科目後進行排名)";
            this.rbCombine.UseVisualStyleBackColor = true;
            this.rbCombine.CheckedChanged += new System.EventHandler(this.rbCombine_CheckedChanged);
            // 
            // rbSeparate
            // 
            this.rbSeparate.AutoSize = true;
            this.rbSeparate.Checked = true;
            this.rbSeparate.Location = new System.Drawing.Point(19, 16);
            this.rbSeparate.Name = "rbSeparate";
            this.rbSeparate.Size = new System.Drawing.Size(167, 21);
            this.rbSeparate.TabIndex = 1;
            this.rbSeparate.TabStop = true;
            this.rbSeparate.Text = "分科排名 (單一科目排名)";
            this.rbSeparate.UseVisualStyleBackColor = true;
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.panelEx1.Controls.Add(this.rbAverage);
            this.panelEx1.Controls.Add(this.rbTotal);
            this.panelEx1.Controls.Add(this.rbWeightAverage);
            this.panelEx1.Controls.Add(this.rbWeightTotal);
            this.panelEx1.Enabled = false;
            this.panelEx1.Location = new System.Drawing.Point(38, 71);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(276, 138);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 0;
            // 
            // rbAverage
            // 
            this.rbAverage.AutoSize = true;
            this.rbAverage.Location = new System.Drawing.Point(18, 108);
            this.rbAverage.Name = "rbAverage";
            this.rbAverage.Size = new System.Drawing.Size(170, 21);
            this.rbAverage.TabIndex = 1;
            this.rbAverage.Tag = "算數平均";
            this.rbAverage.Text = "算數平均排名 (80+90)/2";
            this.rbAverage.UseVisualStyleBackColor = true;
            // 
            // rbTotal
            // 
            this.rbTotal.AutoSize = true;
            this.rbTotal.Location = new System.Drawing.Point(18, 76);
            this.rbTotal.Name = "rbTotal";
            this.rbTotal.Size = new System.Drawing.Size(157, 21);
            this.rbTotal.TabIndex = 1;
            this.rbTotal.Tag = "合計總分";
            this.rbTotal.Text = "合計總分排名 (80+90)";
            this.rbTotal.UseVisualStyleBackColor = true;
            // 
            // rbWeightAverage
            // 
            this.rbWeightAverage.AutoSize = true;
            this.rbWeightAverage.Location = new System.Drawing.Point(18, 44);
            this.rbWeightAverage.Name = "rbWeightAverage";
            this.rbWeightAverage.Size = new System.Drawing.Size(198, 21);
            this.rbWeightAverage.TabIndex = 1;
            this.rbWeightAverage.Tag = "加權平均";
            this.rbWeightAverage.Text = "加權平均排名 (80*2+90*3)/5";
            this.rbWeightAverage.UseVisualStyleBackColor = true;
            // 
            // rbWeightTotal
            // 
            this.rbWeightTotal.AutoSize = true;
            this.rbWeightTotal.Checked = true;
            this.rbWeightTotal.Location = new System.Drawing.Point(18, 12);
            this.rbWeightTotal.Name = "rbWeightTotal";
            this.rbWeightTotal.Size = new System.Drawing.Size(185, 21);
            this.rbWeightTotal.TabIndex = 1;
            this.rbWeightTotal.TabStop = true;
            this.rbWeightTotal.Tag = "加權總分";
            this.rbWeightTotal.Text = "加權總分排名 (80*2+90*3)";
            this.rbWeightTotal.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.chkPrintByClass);
            this.panel4.Controls.Add(this.rbGradeYear);
            this.panel4.Controls.Add(this.rbClass);
            this.panel4.Controls.Add(this.panelEx2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 224);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(336, 115);
            this.panel4.TabIndex = 0;
            // 
            // chkPrintByClass
            // 
            this.chkPrintByClass.AutoSize = true;
            this.chkPrintByClass.Location = new System.Drawing.Point(38, 88);
            this.chkPrintByClass.Name = "chkPrintByClass";
            this.chkPrintByClass.Size = new System.Drawing.Size(92, 21);
            this.chkPrintByClass.TabIndex = 2;
            this.chkPrintByClass.Text = "依班級分頁";
            this.chkPrintByClass.UseVisualStyleBackColor = true;
            this.chkPrintByClass.Visible = false;
            // 
            // rbGradeYear
            // 
            this.rbGradeYear.AutoSize = true;
            this.rbGradeYear.Location = new System.Drawing.Point(108, 10);
            this.rbGradeYear.Name = "rbGradeYear";
            this.rbGradeYear.Size = new System.Drawing.Size(78, 21);
            this.rbGradeYear.TabIndex = 1;
            this.rbGradeYear.Text = "年級排名";
            this.rbGradeYear.UseVisualStyleBackColor = true;
            // 
            // rbClass
            // 
            this.rbClass.AutoSize = true;
            this.rbClass.Checked = true;
            this.rbClass.Location = new System.Drawing.Point(19, 10);
            this.rbClass.Name = "rbClass";
            this.rbClass.Size = new System.Drawing.Size(78, 21);
            this.rbClass.TabIndex = 1;
            this.rbClass.TabStop = true;
            this.rbClass.Text = "班級排名";
            this.rbClass.UseVisualStyleBackColor = true;
            this.rbClass.CheckedChanged += new System.EventHandler(this.rbClass_CheckedChanged);
            // 
            // panelEx2
            // 
            this.panelEx2.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.panelEx2.Controls.Add(this.txtTopRank);
            this.panelEx2.Controls.Add(this.label1);
            this.panelEx2.Controls.Add(this.rbTopRank);
            this.panelEx2.Controls.Add(this.rbAllRank);
            this.panelEx2.Location = new System.Drawing.Point(38, 37);
            this.panelEx2.Name = "panelEx2";
            this.panelEx2.Size = new System.Drawing.Size(276, 46);
            this.panelEx2.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx2.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx2.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx2.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx2.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx2.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx2.Style.GradientAngle = 90;
            this.panelEx2.TabIndex = 0;
            // 
            // txtTopRank
            // 
            // 
            // 
            // 
            this.txtTopRank.Border.Class = "TextBoxBorder";
            this.txtTopRank.Location = new System.Drawing.Point(162, 11);
            this.txtTopRank.Name = "txtTopRank";
            this.txtTopRank.Size = new System.Drawing.Size(45, 25);
            this.txtTopRank.TabIndex = 3;
            this.txtTopRank.TextChanged += new System.EventHandler(this.txtTopRank_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(211, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "名/%";
            // 
            // rbTopRank
            // 
            this.rbTopRank.AutoSize = true;
            this.rbTopRank.Location = new System.Drawing.Point(122, 13);
            this.rbTopRank.Name = "rbTopRank";
            this.rbTopRank.Size = new System.Drawing.Size(39, 21);
            this.rbTopRank.TabIndex = 1;
            this.rbTopRank.Text = "前";
            this.rbTopRank.UseVisualStyleBackColor = true;
            // 
            // rbAllRank
            // 
            this.rbAllRank.AutoSize = true;
            this.rbAllRank.Checked = true;
            this.rbAllRank.Location = new System.Drawing.Point(18, 13);
            this.rbAllRank.Name = "rbAllRank";
            this.rbAllRank.Size = new System.Drawing.Size(78, 21);
            this.rbAllRank.TabIndex = 1;
            this.rbAllRank.TabStop = true;
            this.rbAllRank.Text = "所有名次";
            this.rbAllRank.UseVisualStyleBackColor = true;
            this.rbAllRank.CheckedChanged += new System.EventHandler(this.rbAllRank_CheckedChanged);
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkRate = 0;
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn1.HeaderText = "領域";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn2.HeaderText = "權數";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewTextBoxColumn3.HeaderText = "修習人數";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 75;
            // 
            // lblSelectedCount
            // 
            this.lblSelectedCount.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectedCount.Location = new System.Drawing.Point(119, 380);
            this.lblSelectedCount.Name = "lblSelectedCount";
            this.lblSelectedCount.Size = new System.Drawing.Size(211, 21);
            this.lblSelectedCount.TabIndex = 4;
            this.lblSelectedCount.Text = "已選擇人數：000";
            this.lblSelectedCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RankingExamForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(690, 406);
            this.Controls.Add(this.lblSelectedCount);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRank);
            this.Controls.Add(this.btnRankingOption);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgv);
            this.Name = "RankingExamForm";
            this.Text = "排名";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panelEx1.ResumeLayout(false);
            this.panelEx1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panelEx2.ResumeLayout(false);
            this.panelEx2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgv;
        private System.Windows.Forms.Panel panel1;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboExam;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.ButtonX btnRankingOption;
        private DevComponents.DotNetBar.ButtonX btnRank;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbSeparate;
        private DevComponents.DotNetBar.PanelEx panelEx1;
        private System.Windows.Forms.Panel panel4;
        private DevComponents.DotNetBar.PanelEx panelEx2;
        private System.Windows.Forms.RadioButton rbCombine;
        private System.Windows.Forms.RadioButton rbAverage;
        private System.Windows.Forms.RadioButton rbTotal;
        private System.Windows.Forms.RadioButton rbWeightAverage;
        private System.Windows.Forms.RadioButton rbWeightTotal;
        private System.Windows.Forms.RadioButton rbGradeYear;
        private System.Windows.Forms.RadioButton rbClass;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTopRank;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbTopRank;
        private System.Windows.Forms.RadioButton rbAllRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private Framework.EnhancedErrorProvider errorProvider;
        private System.Windows.Forms.CheckBox chkPrintByClass;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.Label lblSelectedCount;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn chPeriod;
        private System.Windows.Forms.DataGridViewTextBoxColumn chCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn chAttendCount;
    }
}