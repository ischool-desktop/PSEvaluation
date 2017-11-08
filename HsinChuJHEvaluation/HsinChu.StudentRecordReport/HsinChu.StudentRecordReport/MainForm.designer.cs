namespace HsinChu.StudentRecordReport
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
            this.lnTypeConfig = new System.Windows.Forms.LinkLabel();
            this.lnPrintConfig = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Location = new System.Drawing.Point(151, 13);
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
            this.lnTypeConfig.Location = new System.Drawing.Point(75, 19);
            this.lnTypeConfig.Name = "lnTypeConfig";
            this.lnTypeConfig.Size = new System.Drawing.Size(60, 17);
            this.lnTypeConfig.TabIndex = 1;
            this.lnTypeConfig.TabStop = true;
            this.lnTypeConfig.Text = "假別設定";
            this.lnTypeConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnTypeConfig_LinkClicked);
            // 
            // lnPrintConfig
            // 
            this.lnPrintConfig.AutoSize = true;
            this.lnPrintConfig.BackColor = System.Drawing.Color.Transparent;
            this.lnPrintConfig.Location = new System.Drawing.Point(14, 19);
            this.lnPrintConfig.Name = "lnPrintConfig";
            this.lnPrintConfig.Size = new System.Drawing.Size(60, 17);
            this.lnPrintConfig.TabIndex = 1;
            this.lnPrintConfig.TabStop = true;
            this.lnPrintConfig.Text = "列印設定";
            this.lnPrintConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnPrintConfig_LinkClicked);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(240, 48);
            this.Controls.Add(this.lnPrintConfig);
            this.Controls.Add(this.lnTypeConfig);
            this.Controls.Add(this.btnPrint);
            this.Name = "MainForm";
            this.Text = "";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnPrint;
        private System.Windows.Forms.LinkLabel lnTypeConfig;
        private System.Windows.Forms.LinkLabel lnPrintConfig;
    }
}