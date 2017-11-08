namespace HsinChu.TransferReport
{
    partial class TransferReportForm
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
            this.lnTypeConfig = new System.Windows.Forms.LinkLabel();
            this.lnDomainSetup = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Location = new System.Drawing.Point(170, 19);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "列印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // lnTypeConfig
            // 
            this.lnTypeConfig.AutoSize = true;
            this.lnTypeConfig.BackColor = System.Drawing.Color.Transparent;
            this.lnTypeConfig.Location = new System.Drawing.Point(78, 24);
            this.lnTypeConfig.Name = "lnTypeConfig";
            this.lnTypeConfig.Size = new System.Drawing.Size(55, 13);
            this.lnTypeConfig.TabIndex = 1;
            this.lnTypeConfig.TabStop = true;
            this.lnTypeConfig.Text = "假別設定";
            this.lnTypeConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnTypeConfig_LinkClicked);
            // 
            // lnDomainSetup
            // 
            this.lnDomainSetup.AutoSize = true;
            this.lnDomainSetup.BackColor = System.Drawing.Color.Transparent;
            this.lnDomainSetup.Location = new System.Drawing.Point(15, 24);
            this.lnDomainSetup.Name = "lnDomainSetup";
            this.lnDomainSetup.Size = new System.Drawing.Size(55, 13);
            this.lnDomainSetup.TabIndex = 1;
            this.lnDomainSetup.TabStop = true;
            this.lnDomainSetup.Text = "列印設定";
            this.lnDomainSetup.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnDomainSetup_LinkClicked);
            // 
            // TransferReportForm
            // 
            this.ClientSize = new System.Drawing.Size(261, 60);
            this.Controls.Add(this.lnDomainSetup);
            this.Controls.Add(this.lnTypeConfig);
            this.Controls.Add(this.btnPrint);
            this.Name = "TransferReportForm";
            this.Text = "轉學成績證明書";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnPrint;
        private System.Windows.Forms.LinkLabel lnTypeConfig;
        private System.Windows.Forms.LinkLabel lnDomainSetup;
    }
}