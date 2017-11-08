namespace KH_StudentScoreSummaryReport
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
            this.lnkSetStudType = new System.Windows.Forms.LinkLabel();
            this.chkUploadEpaper = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.OneFileSave = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.txtFinalComputeDate = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label3 = new System.Windows.Forms.Label();
            this.gpFormat.SuspendLayout();
            this.panel6.SuspendLayout();
            this.groupPanel3.SuspendLayout();
            this.groupPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.AutoSize = true;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(281, 390);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 25);
            this.btnExit.TabIndex = 3;
            this.btnExit.Tag = "StatusVarying";
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.AutoSize = true;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Location = new System.Drawing.Point(200, 390);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 25);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Tag = "StatusVarying";
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(18, 112);
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
            this.txtEntranceDate.Location = new System.Drawing.Point(84, 108);
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
            this.label2.Location = new System.Drawing.Point(18, 143);
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
            this.txtGraduateDate.Location = new System.Drawing.Point(84, 139);
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
            this.gpFormat.Location = new System.Drawing.Point(21, 201);
            this.gpFormat.Name = "gpFormat";
            this.gpFormat.Size = new System.Drawing.Size(337, 55);
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
            this.panel6.Size = new System.Drawing.Size(331, 28);
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
            this.groupPanel3.Location = new System.Drawing.Point(18, 15);
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
            // lnkSetStudType
            // 
            this.lnkSetStudType.AutoSize = true;
            this.lnkSetStudType.BackColor = System.Drawing.Color.Transparent;
            this.lnkSetStudType.Location = new System.Drawing.Point(18, 394);
            this.lnkSetStudType.Name = "lnkSetStudType";
            this.lnkSetStudType.Size = new System.Drawing.Size(177, 17);
            this.lnkSetStudType.TabIndex = 18;
            this.lnkSetStudType.TabStop = true;
            this.lnkSetStudType.Tag = "StatusVarying";
            this.lnkSetStudType.Text = "設定特種身分加分比與不排名";
            this.lnkSetStudType.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSetStudType_LinkClicked_1);
            // 
            // chkUploadEpaper
            // 
            this.chkUploadEpaper.AutoSize = true;
            this.chkUploadEpaper.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkUploadEpaper.BackgroundStyle.Class = "";
            this.chkUploadEpaper.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkUploadEpaper.Location = new System.Drawing.Point(32, 362);
            this.chkUploadEpaper.Name = "chkUploadEpaper";
            this.chkUploadEpaper.Size = new System.Drawing.Size(147, 21);
            this.chkUploadEpaper.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkUploadEpaper.TabIndex = 19;
            this.chkUploadEpaper.Text = "列印並上傳電子報表";
            this.chkUploadEpaper.TextColor = System.Drawing.Color.Red;
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.panel4);
            this.groupPanel1.DrawTitleBox = false;
            this.groupPanel1.Location = new System.Drawing.Point(21, 277);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(337, 60);
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
            this.groupPanel1.TabIndex = 20;
            this.groupPanel1.Text = "單檔列印";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.OneFileSave);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(331, 33);
            this.panel4.TabIndex = 0;
            // 
            // OneFileSave
            // 
            this.OneFileSave.AutoSize = true;
            this.OneFileSave.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.OneFileSave.BackgroundStyle.Class = "";
            this.OneFileSave.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.OneFileSave.Location = new System.Drawing.Point(3, 3);
            this.OneFileSave.Name = "OneFileSave";
            this.OneFileSave.Size = new System.Drawing.Size(327, 21);
            this.OneFileSave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.OneFileSave.TabIndex = 0;
            this.OneFileSave.Text = "使用單檔列印(檔名：學號_身分證_班級_座號_姓名)";
            this.OneFileSave.TextColor = System.Drawing.Color.Black;
            // 
            // txtFinalComputeDate
            // 
            // 
            // 
            // 
            this.txtFinalComputeDate.Border.Class = "TextBoxBorder";
            this.txtFinalComputeDate.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtFinalComputeDate.Location = new System.Drawing.Point(110, 177);
            this.txtFinalComputeDate.Name = "txtFinalComputeDate";
            this.txtFinalComputeDate.Size = new System.Drawing.Size(238, 25);
            this.txtFinalComputeDate.TabIndex = 21;
            this.txtFinalComputeDate.Tag = "StatusVarying";
            this.txtFinalComputeDate.WatermarkText = "請輸入最後計算日期";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(18, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 17);
            this.label3.TabIndex = 22;
            this.label3.Tag = "StatusVarying";
            this.label3.Text = "最後計算日期";
            // 
            // PrintForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(362, 422);
            this.Controls.Add(this.txtFinalComputeDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.chkUploadEpaper);
            this.Controls.Add(this.lnkSetStudType);
            this.Controls.Add(this.groupPanel3);
            this.Controls.Add(this.gpFormat);
            this.Controls.Add(this.txtGraduateDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtEntranceDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnExit);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PrintForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "高雄學生免試入學列印設定";
            this.Load += new System.EventHandler(this.PrintForm_Load);
            this.gpFormat.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.groupPanel3.ResumeLayout(false);
            this.groupPanel3.PerformLayout();
            this.groupPanel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private System.Windows.Forms.Label label1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtEntranceDate;
        private System.Windows.Forms.Label label2;
        private DevComponents.DotNetBar.Controls.TextBoxX txtGraduateDate;
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
        private System.Windows.Forms.LinkLabel lnkSetStudType;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkUploadEpaper;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.Panel panel4;
        private DevComponents.DotNetBar.Controls.CheckBoxX OneFileSave;
        private DevComponents.DotNetBar.Controls.TextBoxX txtFinalComputeDate;
        private System.Windows.Forms.Label label3;
    }
}