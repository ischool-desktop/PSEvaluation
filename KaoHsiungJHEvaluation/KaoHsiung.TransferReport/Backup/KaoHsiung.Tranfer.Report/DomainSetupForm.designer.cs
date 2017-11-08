//namespace KaoHsiung.TransferReport
//{
//    partial class DomainSetupForm
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.cardPanel = new KaoHsiung.TransferReport.CardPanelEx();
//            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
//            this.btnSave = new DevComponents.DotNetBar.ButtonX();
//            this.btnClose = new DevComponents.DotNetBar.ButtonX();
//            this.panelEx1.SuspendLayout();
//            this.SuspendLayout();
//            // 
//            // cardPanel
//            // 
//            this.cardPanel.CanvasColor = System.Drawing.SystemColors.Control;
//            this.cardPanel.CardWidth = 270;
//            this.cardPanel.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
//            this.cardPanel.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.cardPanel.Location = new System.Drawing.Point(0, 0);
//            this.cardPanel.MinWidth = 2;
//            this.cardPanel.Name = "cardPanel";
//            this.cardPanel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
//            this.cardPanel.Size = new System.Drawing.Size(592, 483);
//            this.cardPanel.Style.Alignment = System.Drawing.StringAlignment.Center;
//            this.cardPanel.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
//            this.cardPanel.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
//            this.cardPanel.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
//            this.cardPanel.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
//            this.cardPanel.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
//            this.cardPanel.Style.GradientAngle = 90;
//            this.cardPanel.TabIndex = 0;
//            // 
//            // panelEx1
//            // 
//            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
//            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
//            this.panelEx1.Controls.Add(this.btnClose);
//            this.panelEx1.Controls.Add(this.btnSave);
//            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Bottom;
//            this.panelEx1.Location = new System.Drawing.Point(0, 483);
//            this.panelEx1.Name = "panelEx1";
//            this.panelEx1.Size = new System.Drawing.Size(592, 33);
//            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
//            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
//            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
//            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
//            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
//            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
//            this.panelEx1.Style.GradientAngle = 90;
//            this.panelEx1.TabIndex = 0;
//            // 
//            // btnSave
//            // 
//            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
//            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
//            this.btnSave.Location = new System.Drawing.Point(431, 5);
//            this.btnSave.Name = "btnSave";
//            this.btnSave.Size = new System.Drawing.Size(75, 23);
//            this.btnSave.TabIndex = 0;
//            this.btnSave.Text = "儲存設定";
//            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
//            // 
//            // btnClose
//            // 
//            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
//            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
//            this.btnClose.Location = new System.Drawing.Point(511, 5);
//            this.btnClose.Name = "btnClose";
//            this.btnClose.Size = new System.Drawing.Size(75, 23);
//            this.btnClose.TabIndex = 0;
//            this.btnClose.Text = "取消";
//            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
//            // 
//            // DomainSetupForm
//            // 
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
//            this.ClientSize = new System.Drawing.Size(592, 516);
//            this.Controls.Add(this.cardPanel);
//            this.Controls.Add(this.panelEx1);
//            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
//            this.MaximizeBox = true;
//            this.Name = "DomainSetupForm";
//            this.Text = "領域科目設定";
//            this.panelEx1.ResumeLayout(false);
//            this.ResumeLayout(false);

//        }

//        #endregion

//        private CardPanelEx cardPanel;
//        private DevComponents.DotNetBar.PanelEx panelEx1;
//        private DevComponents.DotNetBar.ButtonX btnClose;
//        private DevComponents.DotNetBar.ButtonX btnSave;

//    }
//}