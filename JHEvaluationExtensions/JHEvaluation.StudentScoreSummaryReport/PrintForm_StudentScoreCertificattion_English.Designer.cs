namespace JHEvaluation.StudentScoreSummaryReport
{
    partial class PrintForm_StudentScoreCertificattion_English
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
            this.lnkTemplate = new System.Windows.Forms.LinkLabel();
            this.gpFormat = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.rtnPDF = new System.Windows.Forms.RadioButton();
            this.rtnWord = new System.Windows.Forms.RadioButton();
            this.groupPanel4 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.OneFileSave = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.gpFormat.SuspendLayout();
            this.panel6.SuspendLayout();
            this.groupPanel4.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(294, 155);
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
            this.btnPrint.Location = new System.Drawing.Point(213, 155);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Tag = "StatusVarying";
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // lnkTemplate
            // 
            this.lnkTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkTemplate.AutoSize = true;
            this.lnkTemplate.BackColor = System.Drawing.Color.Transparent;
            this.lnkTemplate.Location = new System.Drawing.Point(9, 161);
            this.lnkTemplate.Name = "lnkTemplate";
            this.lnkTemplate.Size = new System.Drawing.Size(86, 17);
            this.lnkTemplate.TabIndex = 5;
            this.lnkTemplate.TabStop = true;
            this.lnkTemplate.Tag = "StatusVarying";
            this.lnkTemplate.Text = "報表樣版設定";
            this.lnkTemplate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTemplate_LinkClicked);
            // 
            // gpFormat
            // 
            this.gpFormat.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.gpFormat.BackColor = System.Drawing.Color.Transparent;
            this.gpFormat.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpFormat.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpFormat.Controls.Add(this.panel6);
            this.gpFormat.DrawTitleBox = false;
            this.gpFormat.Location = new System.Drawing.Point(5, 21);
            this.gpFormat.Name = "gpFormat";
            this.gpFormat.Size = new System.Drawing.Size(364, 55);
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
            this.panel6.Size = new System.Drawing.Size(358, 28);
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
            // groupPanel4
            // 
            this.groupPanel4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.groupPanel4.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel4.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel4.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel4.Controls.Add(this.panel4);
            this.groupPanel4.DrawTitleBox = false;
            this.groupPanel4.Location = new System.Drawing.Point(5, 82);
            this.groupPanel4.Name = "groupPanel4";
            this.groupPanel4.Size = new System.Drawing.Size(364, 60);
            // 
            // 
            // 
            this.groupPanel4.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel4.Style.BackColorGradientAngle = 90;
            this.groupPanel4.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel4.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel4.Style.BorderBottomWidth = 1;
            this.groupPanel4.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel4.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel4.Style.BorderLeftWidth = 1;
            this.groupPanel4.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel4.Style.BorderRightWidth = 1;
            this.groupPanel4.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel4.Style.BorderTopWidth = 1;
            this.groupPanel4.Style.Class = "";
            this.groupPanel4.Style.CornerDiameter = 4;
            this.groupPanel4.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel4.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel4.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel4.StyleMouseDown.Class = "";
            this.groupPanel4.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel4.StyleMouseOver.Class = "";
            this.groupPanel4.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel4.TabIndex = 21;
            this.groupPanel4.Text = "單檔列印";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.OneFileSave);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(358, 33);
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
            this.OneFileSave.Enabled = false;
            this.OneFileSave.Location = new System.Drawing.Point(3, 3);
            this.OneFileSave.Name = "OneFileSave";
            this.OneFileSave.Size = new System.Drawing.Size(327, 21);
            this.OneFileSave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.OneFileSave.TabIndex = 0;
            this.OneFileSave.Text = "使用單檔列印(檔名：學號_身分證_班級_座號_姓名)";
            this.OneFileSave.TextColor = System.Drawing.Color.Black;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.Location = new System.Drawing.Point(95, 161);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(112, 17);
            this.linkLabel1.TabIndex = 22;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Tag = "StatusVarying";
            this.linkLabel1.Text = "檢視合併欄位總表";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // PrintForm_StudentScoreCertificattion_English
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(374, 184);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.groupPanel4);
            this.Controls.Add(this.gpFormat);
            this.Controls.Add(this.lnkTemplate);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnExit);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximumSize = new System.Drawing.Size(390, 223);
            this.MinimumSize = new System.Drawing.Size(390, 223);
            this.Name = "PrintForm_StudentScoreCertificattion_English";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "學生在校成績證明書(英文)列印設定";
            this.gpFormat.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.groupPanel4.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private System.Windows.Forms.LinkLabel lnkTemplate;
        private DevComponents.DotNetBar.Controls.GroupPanel gpFormat;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.RadioButton rtnPDF;
        private System.Windows.Forms.RadioButton rtnWord;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel4;
        private System.Windows.Forms.Panel panel4;
        private DevComponents.DotNetBar.Controls.CheckBoxX OneFileSave;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}