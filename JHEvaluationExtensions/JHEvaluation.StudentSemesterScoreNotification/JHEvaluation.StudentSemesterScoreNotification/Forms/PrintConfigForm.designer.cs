namespace JHEvaluation.StudentSemesterScoreNotification.Forms
{
    partial class PrintConfigForm
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
            this.chkCredit = new System.Windows.Forms.CheckBox();
            this.chkPeriod = new System.Windows.Forms.CheckBox();
            this.gpDomainSubject = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbSubject = new System.Windows.Forms.RadioButton();
            this.rbDomain = new System.Windows.Forms.RadioButton();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.gpPC = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gpTemplate = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lnClear = new System.Windows.Forms.LinkLabel();
            this.lnUpload = new System.Windows.Forms.LinkLabel();
            this.lnView = new System.Windows.Forms.LinkLabel();
            this.gpLearnDomain = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.chkLearnDomain = new System.Windows.Forms.CheckBox();
            this.gpDomainSubject.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gpPC.SuspendLayout();
            this.panel2.SuspendLayout();
            this.gpTemplate.SuspendLayout();
            this.panel3.SuspendLayout();
            this.gpLearnDomain.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkCredit
            // 
            this.chkCredit.AutoSize = true;
            this.chkCredit.BackColor = System.Drawing.Color.Transparent;
            this.chkCredit.Location = new System.Drawing.Point(171, 5);
            this.chkCredit.Name = "chkCredit";
            this.chkCredit.Size = new System.Drawing.Size(79, 21);
            this.chkCredit.TabIndex = 9;
            this.chkCredit.Text = "列印權數";
            this.chkCredit.UseVisualStyleBackColor = false;
            // 
            // chkPeriod
            // 
            this.chkPeriod.AutoSize = true;
            this.chkPeriod.BackColor = System.Drawing.Color.Transparent;
            this.chkPeriod.Location = new System.Drawing.Point(67, 5);
            this.chkPeriod.Name = "chkPeriod";
            this.chkPeriod.Size = new System.Drawing.Size(79, 21);
            this.chkPeriod.TabIndex = 8;
            this.chkPeriod.Text = "列印節數";
            this.chkPeriod.UseVisualStyleBackColor = false;
            // 
            // gpDomainSubject
            // 
            this.gpDomainSubject.BackColor = System.Drawing.Color.Transparent;
            this.gpDomainSubject.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpDomainSubject.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpDomainSubject.Controls.Add(this.panel1);
            this.gpDomainSubject.DrawTitleBox = false;
            this.gpDomainSubject.Location = new System.Drawing.Point(7, 69);
            this.gpDomainSubject.Name = "gpDomainSubject";
            this.gpDomainSubject.Size = new System.Drawing.Size(322, 135);
            // 
            // 
            // 
            this.gpDomainSubject.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpDomainSubject.Style.BackColorGradientAngle = 90;
            this.gpDomainSubject.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpDomainSubject.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpDomainSubject.Style.BorderBottomWidth = 1;
            this.gpDomainSubject.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpDomainSubject.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpDomainSubject.Style.BorderLeftWidth = 1;
            this.gpDomainSubject.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpDomainSubject.Style.BorderRightWidth = 1;
            this.gpDomainSubject.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpDomainSubject.Style.BorderTopWidth = 1;
            this.gpDomainSubject.Style.Class = "";
            this.gpDomainSubject.Style.CornerDiameter = 4;
            this.gpDomainSubject.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpDomainSubject.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpDomainSubject.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpDomainSubject.StyleMouseDown.Class = "";
            this.gpDomainSubject.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpDomainSubject.StyleMouseOver.Class = "";
            this.gpDomainSubject.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpDomainSubject.TabIndex = 11;
            this.gpDomainSubject.Text = "領域科目";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.rbSubject);
            this.panel1.Controls.Add(this.rbDomain);
            this.panel1.Controls.Add(this.labelX2);
            this.panel1.Controls.Add(this.labelX1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(316, 108);
            this.panel1.TabIndex = 12;
            // 
            // rbSubject
            // 
            this.rbSubject.AutoSize = true;
            this.rbSubject.Location = new System.Drawing.Point(13, 58);
            this.rbSubject.Name = "rbSubject";
            this.rbSubject.Size = new System.Drawing.Size(91, 21);
            this.rbSubject.TabIndex = 0;
            this.rbSubject.TabStop = true;
            this.rbSubject.Text = "只列印科目";
            this.rbSubject.UseVisualStyleBackColor = true;
            // 
            // rbDomain
            // 
            this.rbDomain.AutoSize = true;
            this.rbDomain.Checked = true;
            this.rbDomain.Location = new System.Drawing.Point(13, 4);
            this.rbDomain.Name = "rbDomain";
            this.rbDomain.Size = new System.Drawing.Size(91, 21);
            this.rbDomain.TabIndex = 0;
            this.rbDomain.TabStop = true;
            this.rbDomain.Text = "只列印領域";
            this.rbDomain.UseVisualStyleBackColor = true;
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(29, 77);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(189, 25);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "列印全部領域的所有科目。";
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(29, 23);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(286, 25);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "包含「語文」及「彈性課程」領域的所有科目。";
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(187, 338);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "儲存設定";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(269, 338);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(60, 23);
            this.btnClose.TabIndex = 12;
            this.btnClose.Text = "離開";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // gpPC
            // 
            this.gpPC.BackColor = System.Drawing.Color.Transparent;
            this.gpPC.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpPC.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpPC.Controls.Add(this.panel2);
            this.gpPC.DrawTitleBox = false;
            this.gpPC.Location = new System.Drawing.Point(7, 272);
            this.gpPC.Name = "gpPC";
            this.gpPC.Size = new System.Drawing.Size(322, 60);
            // 
            // 
            // 
            this.gpPC.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpPC.Style.BackColorGradientAngle = 90;
            this.gpPC.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpPC.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpPC.Style.BorderBottomWidth = 1;
            this.gpPC.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpPC.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpPC.Style.BorderLeftWidth = 1;
            this.gpPC.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpPC.Style.BorderRightWidth = 1;
            this.gpPC.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpPC.Style.BorderTopWidth = 1;
            this.gpPC.Style.Class = "";
            this.gpPC.Style.CornerDiameter = 4;
            this.gpPC.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpPC.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpPC.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpPC.StyleMouseDown.Class = "";
            this.gpPC.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpPC.StyleMouseOver.Class = "";
            this.gpPC.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpPC.TabIndex = 11;
            this.gpPC.Text = "節權數";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.chkPeriod);
            this.panel2.Controls.Add(this.chkCredit);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(316, 33);
            this.panel2.TabIndex = 12;
            // 
            // gpTemplate
            // 
            this.gpTemplate.BackColor = System.Drawing.Color.Transparent;
            this.gpTemplate.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpTemplate.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpTemplate.Controls.Add(this.panel3);
            this.gpTemplate.DrawTitleBox = false;
            this.gpTemplate.Location = new System.Drawing.Point(7, 5);
            this.gpTemplate.Name = "gpTemplate";
            this.gpTemplate.Size = new System.Drawing.Size(322, 60);
            // 
            // 
            // 
            this.gpTemplate.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpTemplate.Style.BackColorGradientAngle = 90;
            this.gpTemplate.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpTemplate.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpTemplate.Style.BorderBottomWidth = 1;
            this.gpTemplate.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpTemplate.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpTemplate.Style.BorderLeftWidth = 1;
            this.gpTemplate.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpTemplate.Style.BorderRightWidth = 1;
            this.gpTemplate.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpTemplate.Style.BorderTopWidth = 1;
            this.gpTemplate.Style.Class = "";
            this.gpTemplate.Style.CornerDiameter = 4;
            this.gpTemplate.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpTemplate.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpTemplate.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpTemplate.StyleMouseDown.Class = "";
            this.gpTemplate.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpTemplate.StyleMouseOver.Class = "";
            this.gpTemplate.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpTemplate.TabIndex = 11;
            this.gpTemplate.Text = "範本";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.lnClear);
            this.panel3.Controls.Add(this.lnUpload);
            this.panel3.Controls.Add(this.lnView);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(316, 33);
            this.panel3.TabIndex = 12;
            // 
            // lnClear
            // 
            this.lnClear.AutoSize = true;
            this.lnClear.Location = new System.Drawing.Point(186, 6);
            this.lnClear.Name = "lnClear";
            this.lnClear.Size = new System.Drawing.Size(86, 17);
            this.lnClear.TabIndex = 0;
            this.lnClear.TabStop = true;
            this.lnClear.Text = "移除自訂範本";
            this.lnClear.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnClear_LinkClicked);
            // 
            // lnUpload
            // 
            this.lnUpload.AutoSize = true;
            this.lnUpload.Location = new System.Drawing.Point(140, 6);
            this.lnUpload.Name = "lnUpload";
            this.lnUpload.Size = new System.Drawing.Size(34, 17);
            this.lnUpload.TabIndex = 0;
            this.lnUpload.TabStop = true;
            this.lnUpload.Text = "上傳";
            this.lnUpload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnUpload_LinkClicked);
            // 
            // lnView
            // 
            this.lnView.AutoSize = true;
            this.lnView.Location = new System.Drawing.Point(42, 6);
            this.lnView.Name = "lnView";
            this.lnView.Size = new System.Drawing.Size(86, 17);
            this.lnView.TabIndex = 0;
            this.lnView.TabStop = true;
            this.lnView.Text = "檢視目前範本";
            this.lnView.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnView_LinkClicked);
            // 
            // gpLearnDomain
            // 
            this.gpLearnDomain.BackColor = System.Drawing.Color.Transparent;
            this.gpLearnDomain.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpLearnDomain.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpLearnDomain.Controls.Add(this.panel4);
            this.gpLearnDomain.DrawTitleBox = false;
            this.gpLearnDomain.Location = new System.Drawing.Point(7, 208);
            this.gpLearnDomain.Name = "gpLearnDomain";
            this.gpLearnDomain.Size = new System.Drawing.Size(322, 60);
            // 
            // 
            // 
            this.gpLearnDomain.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpLearnDomain.Style.BackColorGradientAngle = 90;
            this.gpLearnDomain.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpLearnDomain.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpLearnDomain.Style.BorderBottomWidth = 1;
            this.gpLearnDomain.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpLearnDomain.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpLearnDomain.Style.BorderLeftWidth = 1;
            this.gpLearnDomain.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpLearnDomain.Style.BorderRightWidth = 1;
            this.gpLearnDomain.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpLearnDomain.Style.BorderTopWidth = 1;
            this.gpLearnDomain.Style.Class = "";
            this.gpLearnDomain.Style.CornerDiameter = 4;
            this.gpLearnDomain.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpLearnDomain.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpLearnDomain.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpLearnDomain.StyleMouseDown.Class = "";
            this.gpLearnDomain.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpLearnDomain.StyleMouseOver.Class = "";
            this.gpLearnDomain.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpLearnDomain.TabIndex = 11;
            this.gpLearnDomain.Text = "學習領域";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.chkLearnDomain);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(316, 33);
            this.panel4.TabIndex = 12;
            // 
            // chkLearnDomain
            // 
            this.chkLearnDomain.AutoSize = true;
            this.chkLearnDomain.BackColor = System.Drawing.Color.Transparent;
            this.chkLearnDomain.Location = new System.Drawing.Point(86, 5);
            this.chkLearnDomain.Name = "chkLearnDomain";
            this.chkLearnDomain.Size = new System.Drawing.Size(144, 21);
            this.chkLearnDomain.TabIndex = 8;
            this.chkLearnDomain.Text = "列印學習領域總成績";
            this.chkLearnDomain.UseVisualStyleBackColor = false;
            // 
            // PrintConfigForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(337, 367);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gpTemplate);
            this.Controls.Add(this.gpLearnDomain);
            this.Controls.Add(this.gpPC);
            this.Controls.Add(this.gpDomainSubject);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PrintConfigForm";
            this.Text = "列印設定";
            this.gpDomainSubject.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gpPC.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.gpTemplate.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.gpLearnDomain.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkCredit;
        private System.Windows.Forms.CheckBox chkPeriod;
        private DevComponents.DotNetBar.Controls.GroupPanel gpDomainSubject;
        private System.Windows.Forms.Panel panel1;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.Controls.GroupPanel gpPC;
        private System.Windows.Forms.Panel panel2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.RadioButton rbSubject;
        private System.Windows.Forms.RadioButton rbDomain;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.GroupPanel gpTemplate;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.LinkLabel lnClear;
        private System.Windows.Forms.LinkLabel lnUpload;
        private System.Windows.Forms.LinkLabel lnView;
        private DevComponents.DotNetBar.Controls.GroupPanel gpLearnDomain;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.CheckBox chkLearnDomain;
    }
}