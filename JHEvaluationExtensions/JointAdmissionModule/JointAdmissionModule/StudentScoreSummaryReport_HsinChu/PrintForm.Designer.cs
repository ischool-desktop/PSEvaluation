namespace JointAdmissionModule.StudentScoreSummaryReport_HsinChu
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
            this.lnkSetStudSpceType = new System.Windows.Forms.LinkLabel();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.chkOutputImportFile = new System.Windows.Forms.CheckBox();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.cbxAddressType = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.txtReportTitle = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.chkWst3aa = new System.Windows.Forms.CheckBox();
            this.chkWst3ab = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lnkSetStudSpceType
            // 
            this.lnkSetStudSpceType.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lnkSetStudSpceType.AutoSize = true;
            this.lnkSetStudSpceType.BackColor = System.Drawing.Color.Transparent;
            this.lnkSetStudSpceType.Location = new System.Drawing.Point(12, 107);
            this.lnkSetStudSpceType.Name = "lnkSetStudSpceType";
            this.lnkSetStudSpceType.Size = new System.Drawing.Size(177, 17);
            this.lnkSetStudSpceType.TabIndex = 0;
            this.lnkSetStudSpceType.TabStop = true;
            this.lnkSetStudSpceType.Text = "設定特種身分加分比與不排名";
            this.lnkSetStudSpceType.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSetStudSpceType_LinkClicked);
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Location = new System.Drawing.Point(275, 105);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(66, 23);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(347, 105);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(62, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // chkOutputImportFile
            // 
            this.chkOutputImportFile.AutoSize = true;
            this.chkOutputImportFile.BackColor = System.Drawing.Color.Transparent;
            this.chkOutputImportFile.Location = new System.Drawing.Point(5, 12);
            this.chkOutputImportFile.Name = "chkOutputImportFile";
            this.chkOutputImportFile.Size = new System.Drawing.Size(196, 21);
            this.chkOutputImportFile.TabIndex = 3;
            this.chkOutputImportFile.Text = "產生免試國中報名系統匯入檔";
            this.chkOutputImportFile.UseVisualStyleBackColor = false;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(295, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(43, 23);
            this.labelX1.TabIndex = 4;
            this.labelX1.Text = "地址";
            // 
            // cbxAddressType
            // 
            this.cbxAddressType.DisplayMember = "Text";
            this.cbxAddressType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxAddressType.FormattingEnabled = true;
            this.cbxAddressType.ItemHeight = 19;
            this.cbxAddressType.Location = new System.Drawing.Point(338, 12);
            this.cbxAddressType.Name = "cbxAddressType";
            this.cbxAddressType.Size = new System.Drawing.Size(71, 25);
            this.cbxAddressType.TabIndex = 5;
            // 
            // txtReportTitle
            // 
            this.txtReportTitle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            // 
            // 
            // 
            this.txtReportTitle.Border.Class = "TextBoxBorder";
            this.txtReportTitle.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtReportTitle.Location = new System.Drawing.Point(68, 66);
            this.txtReportTitle.Name = "txtReportTitle";
            this.txtReportTitle.Size = new System.Drawing.Size(341, 25);
            this.txtReportTitle.TabIndex = 6;
            this.txtReportTitle.Text = "101學年度國民中學在校學習領域成績證明單(高中職入學專用)";
            // 
            // labelX2
            // 
            this.labelX2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelX2.AutoSize = true;
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(2, 68);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(60, 21);
            this.labelX2.TabIndex = 11;
            this.labelX2.Text = "報表抬頭";
            // 
            // chkWst3aa
            // 
            this.chkWst3aa.AutoSize = true;
            this.chkWst3aa.BackColor = System.Drawing.Color.Transparent;
            this.chkWst3aa.Location = new System.Drawing.Point(5, 12);
            this.chkWst3aa.Name = "chkWst3aa";
            this.chkWst3aa.Size = new System.Drawing.Size(157, 21);
            this.chkWst3aa.TabIndex = 12;
            this.chkWst3aa.Text = "產生在校學習成績資料";
            this.chkWst3aa.UseVisualStyleBackColor = false;
            // 
            // chkWst3ab
            // 
            this.chkWst3ab.AutoSize = true;
            this.chkWst3ab.BackColor = System.Drawing.Color.Transparent;
            this.chkWst3ab.Location = new System.Drawing.Point(5, 39);
            this.chkWst3ab.Name = "chkWst3ab";
            this.chkWst3ab.Size = new System.Drawing.Size(144, 21);
            this.chkWst3ab.TabIndex = 13;
            this.chkWst3ab.Text = "產生三學期各科成績";
            this.chkWst3ab.UseVisualStyleBackColor = false;
            // 
            // PrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 129);
            this.Controls.Add(this.chkWst3ab);
            this.Controls.Add(this.chkWst3aa);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.txtReportTitle);
            this.Controls.Add(this.cbxAddressType);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.chkOutputImportFile);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lnkSetStudSpceType);
            this.DoubleBuffered = true;
            this.Name = "PrintForm";
            this.Text = "列印設定";
            this.Load += new System.EventHandler(this.PrintForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnkSetStudSpceType;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private System.Windows.Forms.CheckBox chkOutputImportFile;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxAddressType;
        private DevComponents.DotNetBar.Controls.TextBoxX txtReportTitle;
        private DevComponents.DotNetBar.LabelX labelX2;
        private System.Windows.Forms.CheckBox chkWst3aa;
        private System.Windows.Forms.CheckBox chkWst3ab;
    }
}