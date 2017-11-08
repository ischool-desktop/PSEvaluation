namespace JointAdmissionModule.StudentScoreSummaryReport5
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.cbxAddressType = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.txtReportTitle = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdo1 = new System.Windows.Forms.RadioButton();
            this.rdo2 = new System.Windows.Forms.RadioButton();
            this.rdo3 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lnkSetStudSpceType
            // 
            this.lnkSetStudSpceType.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lnkSetStudSpceType.AutoSize = true;
            this.lnkSetStudSpceType.BackColor = System.Drawing.Color.Transparent;
            this.lnkSetStudSpceType.Location = new System.Drawing.Point(16, 113);
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
            this.btnPrint.Location = new System.Drawing.Point(291, 111);
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
            this.btnExit.Location = new System.Drawing.Point(370, 111);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(62, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(295, 58);
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
            this.cbxAddressType.Location = new System.Drawing.Point(342, 58);
            this.cbxAddressType.Name = "cbxAddressType";
            this.cbxAddressType.Size = new System.Drawing.Size(71, 25);
            this.cbxAddressType.TabIndex = 5;
            // 
            // txtReportTitle
            // 
            // 
            // 
            // 
            this.txtReportTitle.Border.Class = "TextBoxBorder";
            this.txtReportTitle.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtReportTitle.Location = new System.Drawing.Point(78, 12);
            this.txtReportTitle.Name = "txtReportTitle";
            this.txtReportTitle.Size = new System.Drawing.Size(354, 25);
            this.txtReportTitle.TabIndex = 11;
            this.txtReportTitle.Text = "101學年度國民中學在校學習領域成績證明單(五專入學專用)";
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(11, 18);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(60, 21);
            this.labelX2.TabIndex = 10;
            this.labelX2.Text = "報表抬頭";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.rdo3);
            this.groupBox1.Controls.Add(this.rdo2);
            this.groupBox1.Controls.Add(this.rdo1);
            this.groupBox1.Location = new System.Drawing.Point(11, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(228, 54);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "產生五專免試國中報名系統匯入檔";
            // 
            // rdo1
            // 
            this.rdo1.AutoSize = true;
            this.rdo1.Location = new System.Drawing.Point(12, 24);
            this.rdo1.Name = "rdo1";
            this.rdo1.Size = new System.Drawing.Size(52, 21);
            this.rdo1.TabIndex = 0;
            this.rdo1.TabStop = true;
            this.rdo1.Text = "北區";
            this.rdo1.UseVisualStyleBackColor = true;
            // 
            // rdo2
            // 
            this.rdo2.AutoSize = true;
            this.rdo2.Location = new System.Drawing.Point(72, 24);
            this.rdo2.Name = "rdo2";
            this.rdo2.Size = new System.Drawing.Size(52, 21);
            this.rdo2.TabIndex = 1;
            this.rdo2.TabStop = true;
            this.rdo2.Text = "中區";
            this.rdo2.UseVisualStyleBackColor = true;
            // 
            // rdo3
            // 
            this.rdo3.AutoSize = true;
            this.rdo3.Location = new System.Drawing.Point(140, 24);
            this.rdo3.Name = "rdo3";
            this.rdo3.Size = new System.Drawing.Size(52, 21);
            this.rdo3.TabIndex = 2;
            this.rdo3.TabStop = true;
            this.rdo3.Text = "南區";
            this.rdo3.UseVisualStyleBackColor = true;
            // 
            // PrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 139);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtReportTitle);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.cbxAddressType);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lnkSetStudSpceType);
            this.Name = "PrintForm";
            this.Text = "列印設定";
            this.Load += new System.EventHandler(this.PrintForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnkSetStudSpceType;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxAddressType;
        private DevComponents.DotNetBar.Controls.TextBoxX txtReportTitle;
        private DevComponents.DotNetBar.LabelX labelX2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdo3;
        private System.Windows.Forms.RadioButton rdo2;
        private System.Windows.Forms.RadioButton rdo1;
    }
}