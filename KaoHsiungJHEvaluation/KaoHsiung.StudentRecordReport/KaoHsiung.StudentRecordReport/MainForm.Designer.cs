namespace KaoHsiung.StudentRecordReport
{
    partial class MainForm
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
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.lnPrintConfig = new System.Windows.Forms.LinkLabel();
            this.lnType = new System.Windows.Forms.LinkLabel();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.txtTransName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtRegName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Location = new System.Drawing.Point(150, 85);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // lnPrintConfig
            // 
            this.lnPrintConfig.AutoSize = true;
            this.lnPrintConfig.BackColor = System.Drawing.Color.Transparent;
            this.lnPrintConfig.Location = new System.Drawing.Point(10, 85);
            this.lnPrintConfig.Name = "lnPrintConfig";
            this.lnPrintConfig.Size = new System.Drawing.Size(60, 17);
            this.lnPrintConfig.TabIndex = 3;
            this.lnPrintConfig.TabStop = true;
            this.lnPrintConfig.Text = "列印設定";
            this.lnPrintConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnPrintConfig_LinkClicked);
            // 
            // lnType
            // 
            this.lnType.AutoSize = true;
            this.lnType.BackColor = System.Drawing.Color.Transparent;
            this.lnType.Location = new System.Drawing.Point(74, 85);
            this.lnType.Name = "lnType";
            this.lnType.Size = new System.Drawing.Size(60, 17);
            this.lnType.TabIndex = 3;
            this.lnType.TabStop = true;
            this.lnType.Text = "假別設定";
            this.lnType.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnType_LinkClicked);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            this.labelX1.Location = new System.Drawing.Point(13, 13);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(59, 23);
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "承辦人員";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            this.labelX2.Location = new System.Drawing.Point(15, 45);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(67, 23);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "註冊組長";
            // 
            // txtTransName
            // 
            // 
            // 
            // 
            this.txtTransName.Border.Class = "TextBoxBorder";
            this.txtTransName.Location = new System.Drawing.Point(79, 13);
            this.txtTransName.Name = "txtTransName";
            this.txtTransName.Size = new System.Drawing.Size(146, 25);
            this.txtTransName.TabIndex = 1;
            // 
            // txtRegName
            // 
            // 
            // 
            // 
            this.txtRegName.Border.Class = "TextBoxBorder";
            this.txtRegName.Location = new System.Drawing.Point(79, 45);
            this.txtRegName.Name = "txtRegName";
            this.txtRegName.Size = new System.Drawing.Size(146, 25);
            this.txtRegName.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(242, 113);
            this.Controls.Add(this.txtRegName);
            this.Controls.Add(this.txtTransName);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.lnType);
            this.Controls.Add(this.lnPrintConfig);
            this.Controls.Add(this.btnPrint);
            this.MaximumSize = new System.Drawing.Size(250, 147);
            this.MinimumSize = new System.Drawing.Size(250, 147);
            this.Name = "MainForm";
            this.Text = "";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnPrint;
        private System.Windows.Forms.LinkLabel lnPrintConfig;
        private System.Windows.Forms.LinkLabel lnType;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTransName;
        private DevComponents.DotNetBar.Controls.TextBoxX txtRegName;
    }
}