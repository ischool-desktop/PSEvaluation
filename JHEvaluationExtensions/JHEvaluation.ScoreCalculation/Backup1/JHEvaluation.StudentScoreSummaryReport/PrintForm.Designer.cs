namespace JHEvaluation.StudentScoreSummaryReport
{
    partial class PrintForm
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
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.rbSubjectOnly = new System.Windows.Forms.RadioButton();
            this.rbDomainOnly = new System.Windows.Forms.RadioButton();
            this.lnkAbsence = new System.Windows.Forms.LinkLabel();
            this.groupPanel2 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.intRankEnd = new DevComponents.Editors.IntegerInput();
            this.intRankStart = new DevComponents.Editors.IntegerInput();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.chkPercentage = new System.Windows.Forms.CheckBox();
            this.chkRankFilter = new System.Windows.Forms.CheckBox();
            this.chkRank = new System.Windows.Forms.CheckBox();
            this.lnkTemplate = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEntranceDate = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label2 = new System.Windows.Forms.Label();
            this.txtGraduateDate = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.gpFormat = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.rtnPDF = new System.Windows.Forms.RadioButton();
            this.rtnWord = new System.Windows.Forms.RadioButton();
            this.groupPanel3 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.chk3Down = new System.Windows.Forms.CheckBox();
            this.chk3Up = new System.Windows.Forms.CheckBox();
            this.chk2Down = new System.Windows.Forms.CheckBox();
            this.chk2Up = new System.Windows.Forms.CheckBox();
            this.chk1Down = new System.Windows.Forms.CheckBox();
            this.chk1Up = new System.Windows.Forms.CheckBox();
            this.groupPanel1.SuspendLayout();
            this.groupPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.intRankEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intRankStart)).BeginInit();
            this.gpFormat.SuspendLayout();
            this.panel6.SuspendLayout();
            this.groupPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(268, 479);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 3;
            this.btnExit.Tag = "StatusVarying";
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Location = new System.Drawing.Point(187, 479);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Tag = "StatusVarying";
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.labelX2);
            this.groupPanel1.Controls.Add(this.labelX1);
            this.groupPanel1.Controls.Add(this.rbSubjectOnly);
            this.groupPanel1.Controls.Add(this.rbDomainOnly);
            this.groupPanel1.DrawTitleBox = false;
            this.groupPanel1.Location = new System.Drawing.Point(12, 12);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(330, 143);
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
            this.groupPanel1.TabIndex = 0;
            this.groupPanel1.Tag = "StatusVarying";
            this.groupPanel1.Text = "領域科目";
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(32, 85);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(167, 21);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "列印全部領域的所有科目。";
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(32, 24);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(288, 21);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "包含「語文」及「彈性課程」領域的所有科目。";
            // 
            // rbSubjectOnly
            // 
            this.rbSubjectOnly.AutoSize = true;
            this.rbSubjectOnly.Location = new System.Drawing.Point(16, 58);
            this.rbSubjectOnly.Name = "rbSubjectOnly";
            this.rbSubjectOnly.Size = new System.Drawing.Size(91, 21);
            this.rbSubjectOnly.TabIndex = 1;
            this.rbSubjectOnly.Text = "只列印科目";
            this.rbSubjectOnly.UseVisualStyleBackColor = true;
            // 
            // rbDomainOnly
            // 
            this.rbDomainOnly.AutoSize = true;
            this.rbDomainOnly.Location = new System.Drawing.Point(16, 3);
            this.rbDomainOnly.Name = "rbDomainOnly";
            this.rbDomainOnly.Size = new System.Drawing.Size(91, 21);
            this.rbDomainOnly.TabIndex = 0;
            this.rbDomainOnly.TabStop = true;
            this.rbDomainOnly.Text = "只列印領域";
            this.rbDomainOnly.UseVisualStyleBackColor = true;
            // 
            // lnkAbsence
            // 
            this.lnkAbsence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkAbsence.AutoSize = true;
            this.lnkAbsence.BackColor = System.Drawing.Color.Transparent;
            this.lnkAbsence.Location = new System.Drawing.Point(6, 482);
            this.lnkAbsence.Name = "lnkAbsence";
            this.lnkAbsence.Size = new System.Drawing.Size(60, 17);
            this.lnkAbsence.TabIndex = 4;
            this.lnkAbsence.TabStop = true;
            this.lnkAbsence.Tag = "StatusVarying";
            this.lnkAbsence.Text = "假別設定";
            this.lnkAbsence.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAbsence_LinkClicked);
            // 
            // groupPanel2
            // 
            this.groupPanel2.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel2.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel2.Controls.Add(this.intRankEnd);
            this.groupPanel2.Controls.Add(this.intRankStart);
            this.groupPanel2.Controls.Add(this.labelX3);
            this.groupPanel2.Controls.Add(this.chkPercentage);
            this.groupPanel2.Controls.Add(this.chkRankFilter);
            this.groupPanel2.Controls.Add(this.chkRank);
            this.groupPanel2.DrawTitleBox = false;
            this.groupPanel2.Location = new System.Drawing.Point(12, 158);
            this.groupPanel2.Name = "groupPanel2";
            this.groupPanel2.Size = new System.Drawing.Size(330, 110);
            // 
            // 
            // 
            this.groupPanel2.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel2.Style.BackColorGradientAngle = 90;
            this.groupPanel2.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel2.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderBottomWidth = 1;
            this.groupPanel2.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel2.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderLeftWidth = 1;
            this.groupPanel2.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderRightWidth = 1;
            this.groupPanel2.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderTopWidth = 1;
            this.groupPanel2.Style.Class = "";
            this.groupPanel2.Style.CornerDiameter = 4;
            this.groupPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel2.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel2.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseDown.Class = "";
            this.groupPanel2.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseOver.Class = "";
            this.groupPanel2.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel2.TabIndex = 1;
            this.groupPanel2.Tag = "StatusVarying";
            this.groupPanel2.Text = "學習領域(列印離校生請不要勾選排名的任何功能)";
            // 
            // intRankEnd
            // 
            // 
            // 
            // 
            this.intRankEnd.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intRankEnd.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intRankEnd.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intRankEnd.Location = new System.Drawing.Point(202, 49);
            this.intRankEnd.Name = "intRankEnd";
            this.intRankEnd.ShowUpDown = true;
            this.intRankEnd.Size = new System.Drawing.Size(54, 25);
            this.intRankEnd.TabIndex = 4;
            this.intRankEnd.Value = 10;
            // 
            // intRankStart
            // 
            // 
            // 
            // 
            this.intRankStart.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intRankStart.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intRankStart.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intRankStart.Location = new System.Drawing.Point(121, 49);
            this.intRankStart.Name = "intRankStart";
            this.intRankStart.ShowUpDown = true;
            this.intRankStart.Size = new System.Drawing.Size(54, 25);
            this.intRankStart.TabIndex = 3;
            this.intRankStart.Value = 1;
            // 
            // labelX3
            // 
            this.labelX3.AutoSize = true;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(179, 51);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(20, 21);
            this.labelX3.TabIndex = 3;
            this.labelX3.Text = "到";
            // 
            // chkPercentage
            // 
            this.chkPercentage.AutoSize = true;
            this.chkPercentage.Location = new System.Drawing.Point(128, 16);
            this.chkPercentage.Name = "chkPercentage";
            this.chkPercentage.Size = new System.Drawing.Size(92, 21);
            this.chkPercentage.TabIndex = 1;
            this.chkPercentage.Text = "列印百分比";
            this.chkPercentage.UseVisualStyleBackColor = true;
            // 
            // chkRankFilter
            // 
            this.chkRankFilter.AutoSize = true;
            this.chkRankFilter.Location = new System.Drawing.Point(16, 51);
            this.chkRankFilter.Name = "chkRankFilter";
            this.chkRankFilter.Size = new System.Drawing.Size(105, 21);
            this.chkRankFilter.TabIndex = 2;
            this.chkRankFilter.Text = "限定名次範圍";
            this.chkRankFilter.UseVisualStyleBackColor = true;
            this.chkRankFilter.CheckedChanged += new System.EventHandler(this.RankScope_CheckedChanged);
            // 
            // chkRank
            // 
            this.chkRank.AutoSize = true;
            this.chkRank.Location = new System.Drawing.Point(16, 16);
            this.chkRank.Name = "chkRank";
            this.chkRank.Size = new System.Drawing.Size(79, 21);
            this.chkRank.TabIndex = 0;
            this.chkRank.Text = "列印名次";
            this.chkRank.UseVisualStyleBackColor = true;
            // 
            // lnkTemplate
            // 
            this.lnkTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkTemplate.AutoSize = true;
            this.lnkTemplate.BackColor = System.Drawing.Color.Transparent;
            this.lnkTemplate.Location = new System.Drawing.Point(72, 482);
            this.lnkTemplate.Name = "lnkTemplate";
            this.lnkTemplate.Size = new System.Drawing.Size(86, 17);
            this.lnkTemplate.TabIndex = 5;
            this.lnkTemplate.TabStop = true;
            this.lnkTemplate.Tag = "StatusVarying";
            this.lnkTemplate.Text = "報表樣版設定";
            this.lnkTemplate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTemplate_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 360);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 5;
            this.label1.Tag = "StatusVarying";
            this.label1.Text = "入學日期";
            // 
            // txtEntranceDate
            // 
            // 
            // 
            // 
            this.txtEntranceDate.Border.Class = "";
            this.txtEntranceDate.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtEntranceDate.Location = new System.Drawing.Point(78, 356);
            this.txtEntranceDate.Name = "txtEntranceDate";
            this.txtEntranceDate.Size = new System.Drawing.Size(264, 18);
            this.txtEntranceDate.TabIndex = 0;
            this.txtEntranceDate.Tag = "StatusVarying";
            this.txtEntranceDate.WatermarkText = "未輸入將列印「新生異動」日期。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 391);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 17);
            this.label2.TabIndex = 5;
            this.label2.Tag = "StatusVarying";
            this.label2.Text = "畢業日期";
            // 
            // txtGraduateDate
            // 
            // 
            // 
            // 
            this.txtGraduateDate.Border.Class = "TextBoxBorder";
            this.txtGraduateDate.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtGraduateDate.Location = new System.Drawing.Point(78, 387);
            this.txtGraduateDate.Name = "txtGraduateDate";
            this.txtGraduateDate.Size = new System.Drawing.Size(264, 25);
            this.txtGraduateDate.TabIndex = 1;
            this.txtGraduateDate.Tag = "StatusVarying";
            this.txtGraduateDate.WatermarkText = "未輸入將列印「畢業異動」日期。";
            // 
            // gpFormat
            // 
            this.gpFormat.BackColor = System.Drawing.Color.Transparent;
            this.gpFormat.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpFormat.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpFormat.Controls.Add(this.panel6);
            this.gpFormat.DrawTitleBox = false;
            this.gpFormat.Location = new System.Drawing.Point(12, 418);
            this.gpFormat.Name = "gpFormat";
            this.gpFormat.Size = new System.Drawing.Size(330, 55);
            // 
            // 
            // 
            this.gpFormat.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpFormat.Style.BackColorGradientAngle = 90;
            this.gpFormat.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpFormat.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpFormat.Style.BorderBottomWidth = 1;
            this.gpFormat.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpFormat.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpFormat.Style.BorderLeftWidth = 1;
            this.gpFormat.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpFormat.Style.BorderRightWidth = 1;
            this.gpFormat.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpFormat.Style.BorderTopWidth = 1;
            this.gpFormat.Style.Class = "";
            this.gpFormat.Style.CornerDiameter = 4;
            this.gpFormat.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpFormat.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpFormat.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpFormat.StyleMouseDown.Class = "";
            this.gpFormat.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpFormat.StyleMouseOver.Class = "";
            this.gpFormat.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpFormat.TabIndex = 15;
            this.gpFormat.Text = "列印格式";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Transparent;
            this.panel6.Controls.Add(this.rtnPDF);
            this.panel6.Controls.Add(this.rtnWord);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(324, 28);
            this.panel6.TabIndex = 0;
            // 
            // rtnPDF
            // 
            this.rtnPDF.AutoSize = true;
            this.rtnPDF.Location = new System.Drawing.Point(162, 3);
            this.rtnPDF.Name = "rtnPDF";
            this.rtnPDF.Size = new System.Drawing.Size(91, 21);
            this.rtnPDF.TabIndex = 0;
            this.rtnPDF.TabStop = true;
            this.rtnPDF.Text = "PDF (*.pdf)";
            this.rtnPDF.UseVisualStyleBackColor = true;
            // 
            // rtnWord
            // 
            this.rtnWord.AutoSize = true;
            this.rtnWord.Checked = true;
            this.rtnWord.Location = new System.Drawing.Point(42, 3);
            this.rtnWord.Name = "rtnWord";
            this.rtnWord.Size = new System.Drawing.Size(102, 21);
            this.rtnWord.TabIndex = 0;
            this.rtnWord.TabStop = true;
            this.rtnWord.Text = "Word (*.doc)";
            this.rtnWord.UseVisualStyleBackColor = true;
            // 
            // groupPanel3
            // 
            this.groupPanel3.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel3.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel3.Controls.Add(this.chk3Down);
            this.groupPanel3.Controls.Add(this.chk3Up);
            this.groupPanel3.Controls.Add(this.chk2Down);
            this.groupPanel3.Controls.Add(this.chk2Up);
            this.groupPanel3.Controls.Add(this.chk1Down);
            this.groupPanel3.Controls.Add(this.chk1Up);
            this.groupPanel3.DrawTitleBox = false;
            this.groupPanel3.Location = new System.Drawing.Point(12, 274);
            this.groupPanel3.Name = "groupPanel3";
            this.groupPanel3.Size = new System.Drawing.Size(331, 76);
            // 
            // 
            // 
            this.groupPanel3.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel3.Style.BackColorGradientAngle = 90;
            this.groupPanel3.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel3.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel3.Style.BorderBottomWidth = 1;
            this.groupPanel3.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel3.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel3.Style.BorderLeftWidth = 1;
            this.groupPanel3.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel3.Style.BorderRightWidth = 1;
            this.groupPanel3.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel3.Style.BorderTopWidth = 1;
            this.groupPanel3.Style.Class = "";
            this.groupPanel3.Style.CornerDiameter = 4;
            this.groupPanel3.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel3.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel3.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel3.StyleMouseDown.Class = "";
            this.groupPanel3.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel3.StyleMouseOver.Class = "";
            this.groupPanel3.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel3.TabIndex = 16;
            this.groupPanel3.Tag = "";
            this.groupPanel3.Text = "列印學期";
            // 
            // chk3Down
            // 
            this.chk3Down.AutoSize = true;
            this.chk3Down.Checked = true;
            this.chk3Down.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk3Down.Location = new System.Drawing.Point(230, 24);
            this.chk3Down.Name = "chk3Down";
            this.chk3Down.Size = new System.Drawing.Size(53, 21);
            this.chk3Down.TabIndex = 27;
            this.chk3Down.Text = "三下";
            this.chk3Down.UseVisualStyleBackColor = true;
            // 
            // chk3Up
            // 
            this.chk3Up.AutoSize = true;
            this.chk3Up.Checked = true;
            this.chk3Up.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk3Up.Location = new System.Drawing.Point(230, -2);
            this.chk3Up.Name = "chk3Up";
            this.chk3Up.Size = new System.Drawing.Size(53, 21);
            this.chk3Up.TabIndex = 26;
            this.chk3Up.Text = "三上";
            this.chk3Up.UseVisualStyleBackColor = true;
            // 
            // chk2Down
            // 
            this.chk2Down.AutoSize = true;
            this.chk2Down.Checked = true;
            this.chk2Down.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk2Down.Location = new System.Drawing.Point(135, 24);
            this.chk2Down.Name = "chk2Down";
            this.chk2Down.Size = new System.Drawing.Size(53, 21);
            this.chk2Down.TabIndex = 22;
            this.chk2Down.Text = "二下";
            this.chk2Down.UseVisualStyleBackColor = true;
            // 
            // chk2Up
            // 
            this.chk2Up.AutoSize = true;
            this.chk2Up.Checked = true;
            this.chk2Up.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk2Up.Location = new System.Drawing.Point(135, -2);
            this.chk2Up.Name = "chk2Up";
            this.chk2Up.Size = new System.Drawing.Size(53, 21);
            this.chk2Up.TabIndex = 25;
            this.chk2Up.Text = "二上";
            this.chk2Up.UseVisualStyleBackColor = true;
            // 
            // chk1Down
            // 
            this.chk1Down.AutoSize = true;
            this.chk1Down.Checked = true;
            this.chk1Down.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk1Down.Location = new System.Drawing.Point(40, 24);
            this.chk1Down.Name = "chk1Down";
            this.chk1Down.Size = new System.Drawing.Size(53, 21);
            this.chk1Down.TabIndex = 24;
            this.chk1Down.Text = "一下";
            this.chk1Down.UseVisualStyleBackColor = true;
            // 
            // chk1Up
            // 
            this.chk1Up.AutoSize = true;
            this.chk1Up.Checked = true;
            this.chk1Up.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk1Up.Location = new System.Drawing.Point(40, -2);
            this.chk1Up.Name = "chk1Up";
            this.chk1Up.Size = new System.Drawing.Size(53, 21);
            this.chk1Up.TabIndex = 23;
            this.chk1Up.Text = "一上";
            this.chk1Up.UseVisualStyleBackColor = true;
            // 
            // PrintForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(355, 508);
            this.Controls.Add(this.groupPanel3);
            this.Controls.Add(this.gpFormat);
            this.Controls.Add(this.txtGraduateDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtEntranceDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lnkTemplate);
            this.Controls.Add(this.lnkAbsence);
            this.Controls.Add(this.groupPanel2);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnExit);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PrintForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "列印設定";
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            this.groupPanel2.ResumeLayout(false);
            this.groupPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.intRankEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intRankStart)).EndInit();
            this.gpFormat.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.groupPanel3.ResumeLayout(false);
            this.groupPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.RadioButton rbSubjectOnly;
        private System.Windows.Forms.RadioButton rbDomainOnly;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.LinkLabel lnkAbsence;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel2;
        private System.Windows.Forms.CheckBox chkRank;
        private System.Windows.Forms.CheckBox chkPercentage;
        private System.Windows.Forms.LinkLabel lnkTemplate;
        private System.Windows.Forms.Label label1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtEntranceDate;
        private System.Windows.Forms.Label label2;
        private DevComponents.DotNetBar.Controls.TextBoxX txtGraduateDate;
        private System.Windows.Forms.CheckBox chkRankFilter;
        private DevComponents.Editors.IntegerInput intRankEnd;
        private DevComponents.Editors.IntegerInput intRankStart;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.GroupPanel gpFormat;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.RadioButton rtnPDF;
        private System.Windows.Forms.RadioButton rtnWord;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel3;
        private System.Windows.Forms.CheckBox chk3Down;
        private System.Windows.Forms.CheckBox chk3Up;
        private System.Windows.Forms.CheckBox chk2Down;
        private System.Windows.Forms.CheckBox chk2Up;
        private System.Windows.Forms.CheckBox chk1Down;
        private System.Windows.Forms.CheckBox chk1Up;
    }
}