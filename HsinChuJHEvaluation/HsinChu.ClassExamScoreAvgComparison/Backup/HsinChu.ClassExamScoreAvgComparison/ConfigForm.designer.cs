namespace HsinChu.ClassExamScoreAvgComparison
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.gpItem = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.gpRank = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.gpItem.SuspendLayout();
            this.gpRank.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(89, 148);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(70, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "儲存設定";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(165, 148);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(60, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(11, 5);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(79, 21);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "合計總分";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(11, 31);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(79, 21);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "加權總分";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(11, 57);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(79, 21);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "算術平均";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(11, 83);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(79, 21);
            this.checkBox4.TabIndex = 3;
            this.checkBox4.Text = "加權平均";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // gpItem
            // 
            this.gpItem.BackColor = System.Drawing.Color.Transparent;
            this.gpItem.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpItem.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpItem.Controls.Add(this.checkBox4);
            this.gpItem.Controls.Add(this.checkBox3);
            this.gpItem.Controls.Add(this.checkBox2);
            this.gpItem.Controls.Add(this.checkBox1);
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
            this.gpRank.Controls.Add(this.radioButton4);
            this.gpRank.Controls.Add(this.radioButton3);
            this.gpRank.Controls.Add(this.radioButton2);
            this.gpRank.Controls.Add(this.radioButton1);
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
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Enabled = false;
            this.radioButton4.Location = new System.Drawing.Point(12, 82);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(78, 21);
            this.radioButton4.TabIndex = 3;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "加權平均";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Enabled = false;
            this.radioButton3.Location = new System.Drawing.Point(12, 56);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(78, 21);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "算術平均";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Enabled = false;
            this.radioButton2.Location = new System.Drawing.Point(12, 30);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(78, 21);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "加權總分";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Enabled = false;
            this.radioButton1.Location = new System.Drawing.Point(12, 4);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(78, 21);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "合計總分";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(232, 176);
            this.Controls.Add(this.gpRank);
            this.Controls.Add(this.gpItem);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(200, 175);
            this.Name = "ConfigForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "";
            this.gpItem.ResumeLayout(false);
            this.gpItem.PerformLayout();
            this.gpRank.ResumeLayout(false);
            this.gpRank.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private DevComponents.DotNetBar.Controls.GroupPanel gpItem;
        private DevComponents.DotNetBar.Controls.GroupPanel gpRank;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}