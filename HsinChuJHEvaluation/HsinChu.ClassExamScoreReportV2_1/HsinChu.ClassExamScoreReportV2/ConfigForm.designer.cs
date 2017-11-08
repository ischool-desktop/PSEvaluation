namespace HsinChu.ClassExamScoreReportV21
{
    partial class ConfigForm
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
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.chkTotal = new System.Windows.Forms.CheckBox();
            this.chkWeightTotal = new System.Windows.Forms.CheckBox();
            this.chkAvg = new System.Windows.Forms.CheckBox();
            this.chkWeightAvg = new System.Windows.Forms.CheckBox();
            this.gpItem = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.gpRank = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.rbWeightAvg = new System.Windows.Forms.RadioButton();
            this.rbAvg = new System.Windows.Forms.RadioButton();
            this.rbWeightTotal = new System.Windows.Forms.RadioButton();
            this.rbTotal = new System.Windows.Forms.RadioButton();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.rbB4 = new System.Windows.Forms.RadioButton();
            this.rbA4 = new System.Windows.Forms.RadioButton();
            this.gpItem.SuspendLayout();
            this.gpRank.SuspendLayout();
            this.groupPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(89, 204);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(70, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "儲存設定";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(165, 204);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(60, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // chkTotal
            // 
            this.chkTotal.AutoSize = true;
            this.chkTotal.Location = new System.Drawing.Point(11, 5);
            this.chkTotal.Name = "chkTotal";
            this.chkTotal.Size = new System.Drawing.Size(79, 21);
            this.chkTotal.TabIndex = 0;
            this.chkTotal.Text = "合計總分";
            this.chkTotal.UseVisualStyleBackColor = true;
            this.chkTotal.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // chkWeightTotal
            // 
            this.chkWeightTotal.AutoSize = true;
            this.chkWeightTotal.Location = new System.Drawing.Point(11, 31);
            this.chkWeightTotal.Name = "chkWeightTotal";
            this.chkWeightTotal.Size = new System.Drawing.Size(79, 21);
            this.chkWeightTotal.TabIndex = 1;
            this.chkWeightTotal.Text = "加權總分";
            this.chkWeightTotal.UseVisualStyleBackColor = true;
            this.chkWeightTotal.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // chkAvg
            // 
            this.chkAvg.AutoSize = true;
            this.chkAvg.Location = new System.Drawing.Point(11, 57);
            this.chkAvg.Name = "chkAvg";
            this.chkAvg.Size = new System.Drawing.Size(79, 21);
            this.chkAvg.TabIndex = 2;
            this.chkAvg.Text = "算術平均";
            this.chkAvg.UseVisualStyleBackColor = true;
            this.chkAvg.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // chkWeightAvg
            // 
            this.chkWeightAvg.AutoSize = true;
            this.chkWeightAvg.Location = new System.Drawing.Point(11, 83);
            this.chkWeightAvg.Name = "chkWeightAvg";
            this.chkWeightAvg.Size = new System.Drawing.Size(79, 21);
            this.chkWeightAvg.TabIndex = 3;
            this.chkWeightAvg.Text = "加權平均";
            this.chkWeightAvg.UseVisualStyleBackColor = true;
            this.chkWeightAvg.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // gpItem
            // 
            this.gpItem.BackColor = System.Drawing.Color.Transparent;
            this.gpItem.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpItem.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpItem.Controls.Add(this.chkWeightAvg);
            this.gpItem.Controls.Add(this.chkAvg);
            this.gpItem.Controls.Add(this.chkWeightTotal);
            this.gpItem.Controls.Add(this.chkTotal);
            this.gpItem.DrawTitleBox = false;
            this.gpItem.Location = new System.Drawing.Point(8, 5);
            this.gpItem.Name = "gpItem";
            this.gpItem.Size = new System.Drawing.Size(106, 137);
            // 
            // 
            // 
            this.gpItem.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpItem.Style.BackColorGradientAngle = 90;
            this.gpItem.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpItem.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpItem.Style.BorderBottomWidth = 1;
            this.gpItem.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpItem.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpItem.Style.BorderLeftWidth = 1;
            this.gpItem.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpItem.Style.BorderRightWidth = 1;
            this.gpItem.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpItem.Style.BorderTopWidth = 1;
            this.gpItem.Style.CornerDiameter = 4;
            this.gpItem.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpItem.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpItem.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.gpItem.TabIndex = 0;
            this.gpItem.Text = "列印項目";
            // 
            // gpRank
            // 
            this.gpRank.BackColor = System.Drawing.Color.Transparent;
            this.gpRank.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpRank.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpRank.Controls.Add(this.rbWeightAvg);
            this.gpRank.Controls.Add(this.rbAvg);
            this.gpRank.Controls.Add(this.rbWeightTotal);
            this.gpRank.Controls.Add(this.rbTotal);
            this.gpRank.DrawTitleBox = false;
            this.gpRank.Location = new System.Drawing.Point(121, 5);
            this.gpRank.Name = "gpRank";
            this.gpRank.Size = new System.Drawing.Size(104, 137);
            // 
            // 
            // 
            this.gpRank.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpRank.Style.BackColorGradientAngle = 90;
            this.gpRank.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpRank.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpRank.Style.BorderBottomWidth = 1;
            this.gpRank.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpRank.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpRank.Style.BorderLeftWidth = 1;
            this.gpRank.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpRank.Style.BorderRightWidth = 1;
            this.gpRank.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpRank.Style.BorderTopWidth = 1;
            this.gpRank.Style.CornerDiameter = 4;
            this.gpRank.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpRank.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpRank.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.gpRank.TabIndex = 1;
            this.gpRank.Text = "排名依據";
            // 
            // rbWeightAvg
            // 
            this.rbWeightAvg.AutoSize = true;
            this.rbWeightAvg.Enabled = false;
            this.rbWeightAvg.Location = new System.Drawing.Point(12, 82);
            this.rbWeightAvg.Name = "rbWeightAvg";
            this.rbWeightAvg.Size = new System.Drawing.Size(78, 21);
            this.rbWeightAvg.TabIndex = 3;
            this.rbWeightAvg.TabStop = true;
            this.rbWeightAvg.Text = "加權平均";
            this.rbWeightAvg.UseVisualStyleBackColor = true;
            // 
            // rbAvg
            // 
            this.rbAvg.AutoSize = true;
            this.rbAvg.Enabled = false;
            this.rbAvg.Location = new System.Drawing.Point(12, 56);
            this.rbAvg.Name = "rbAvg";
            this.rbAvg.Size = new System.Drawing.Size(78, 21);
            this.rbAvg.TabIndex = 2;
            this.rbAvg.TabStop = true;
            this.rbAvg.Text = "算術平均";
            this.rbAvg.UseVisualStyleBackColor = true;
            // 
            // rbWeightTotal
            // 
            this.rbWeightTotal.AutoSize = true;
            this.rbWeightTotal.Enabled = false;
            this.rbWeightTotal.Location = new System.Drawing.Point(12, 30);
            this.rbWeightTotal.Name = "rbWeightTotal";
            this.rbWeightTotal.Size = new System.Drawing.Size(78, 21);
            this.rbWeightTotal.TabIndex = 1;
            this.rbWeightTotal.TabStop = true;
            this.rbWeightTotal.Text = "加權總分";
            this.rbWeightTotal.UseVisualStyleBackColor = true;
            // 
            // rbTotal
            // 
            this.rbTotal.AutoSize = true;
            this.rbTotal.Enabled = false;
            this.rbTotal.Location = new System.Drawing.Point(12, 4);
            this.rbTotal.Name = "rbTotal";
            this.rbTotal.Size = new System.Drawing.Size(78, 21);
            this.rbTotal.TabIndex = 0;
            this.rbTotal.TabStop = true;
            this.rbTotal.Text = "合計總分";
            this.rbTotal.UseVisualStyleBackColor = true;
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.rbB4);
            this.groupPanel1.Controls.Add(this.rbA4);
            this.groupPanel1.DrawTitleBox = false;
            this.groupPanel1.Location = new System.Drawing.Point(8, 145);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(216, 53);
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
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.groupPanel1.TabIndex = 2;
            this.groupPanel1.Text = "紙張尺寸";
            // 
            // rbB4
            // 
            this.rbB4.AutoSize = true;
            this.rbB4.Location = new System.Drawing.Point(112, 0);
            this.rbB4.Name = "rbB4";
            this.rbB4.Size = new System.Drawing.Size(42, 21);
            this.rbB4.TabIndex = 1;
            this.rbB4.Text = "B4";
            this.rbB4.UseVisualStyleBackColor = true;
            // 
            // rbA4
            // 
            this.rbA4.AutoSize = true;
            this.rbA4.Checked = true;
            this.rbA4.Location = new System.Drawing.Point(57, 0);
            this.rbA4.Name = "rbA4";
            this.rbA4.Size = new System.Drawing.Size(43, 21);
            this.rbA4.TabIndex = 0;
            this.rbA4.TabStop = true;
            this.rbA4.Text = "A4";
            this.rbA4.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(232, 232);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.gpRank);
            this.Controls.Add(this.gpItem);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(240, 270);
            this.Name = "ConfigForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "";
            this.gpItem.ResumeLayout(false);
            this.gpItem.PerformLayout();
            this.gpRank.ResumeLayout(false);
            this.gpRank.PerformLayout();
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private System.Windows.Forms.CheckBox chkTotal;
        private System.Windows.Forms.CheckBox chkWeightTotal;
        private System.Windows.Forms.CheckBox chkAvg;
        private System.Windows.Forms.CheckBox chkWeightAvg;
        private DevComponents.DotNetBar.Controls.GroupPanel gpItem;
        private DevComponents.DotNetBar.Controls.GroupPanel gpRank;
        private System.Windows.Forms.RadioButton rbWeightAvg;
        private System.Windows.Forms.RadioButton rbAvg;
        private System.Windows.Forms.RadioButton rbWeightTotal;
        private System.Windows.Forms.RadioButton rbTotal;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.RadioButton rbB4;
        private System.Windows.Forms.RadioButton rbA4;
    }
}