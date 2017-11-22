namespace StudentSemesterScoreNotification
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
            this.rbDefault = new System.Windows.Forms.RadioButton();
            this.rbCustomize1 = new System.Windows.Forms.RadioButton();
            this.rbCustomize2 = new System.Windows.Forms.RadioButton();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.lnkView1 = new System.Windows.Forms.LinkLabel();
            this.lnkView2 = new System.Windows.Forms.LinkLabel();
            this.lnkView3 = new System.Windows.Forms.LinkLabel();
            this.lnkUpload1 = new System.Windows.Forms.LinkLabel();
            this.lnkUpload2 = new System.Windows.Forms.LinkLabel();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.lnkAllFields = new System.Windows.Forms.LinkLabel();
            this.groupPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbDefault
            // 
            this.rbDefault.AutoSize = true;
            this.rbDefault.BackColor = System.Drawing.Color.Transparent;
            this.rbDefault.Location = new System.Drawing.Point(7, 3);
            this.rbDefault.Name = "rbDefault";
            this.rbDefault.Size = new System.Drawing.Size(104, 21);
            this.rbDefault.TabIndex = 0;
            this.rbDefault.TabStop = true;
            this.rbDefault.Text = "系統預設範本";
            this.rbDefault.UseVisualStyleBackColor = false;
            // 
            // rbCustomize1
            // 
            this.rbCustomize1.AutoSize = true;
            this.rbCustomize1.BackColor = System.Drawing.Color.Transparent;
            this.rbCustomize1.Location = new System.Drawing.Point(7, 30);
            this.rbCustomize1.Name = "rbCustomize1";
            this.rbCustomize1.Size = new System.Drawing.Size(86, 21);
            this.rbCustomize1.TabIndex = 2;
            this.rbCustomize1.TabStop = true;
            this.rbCustomize1.Text = "自訂範本1";
            this.rbCustomize1.UseVisualStyleBackColor = false;
            // 
            // rbCustomize2
            // 
            this.rbCustomize2.AutoSize = true;
            this.rbCustomize2.BackColor = System.Drawing.Color.Transparent;
            this.rbCustomize2.Location = new System.Drawing.Point(7, 57);
            this.rbCustomize2.Name = "rbCustomize2";
            this.rbCustomize2.Size = new System.Drawing.Size(86, 21);
            this.rbCustomize2.TabIndex = 3;
            this.rbCustomize2.TabStop = true;
            this.rbCustomize2.Text = "自訂範本2";
            this.rbCustomize2.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(144, 135);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(228, 135);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "離開";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // lnkView1
            // 
            this.lnkView1.AutoSize = true;
            this.lnkView1.BackColor = System.Drawing.Color.Transparent;
            this.lnkView1.Location = new System.Drawing.Point(117, 7);
            this.lnkView1.Name = "lnkView1";
            this.lnkView1.Size = new System.Drawing.Size(34, 17);
            this.lnkView1.TabIndex = 6;
            this.lnkView1.TabStop = true;
            this.lnkView1.Text = "檢視";
            this.lnkView1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkView1_LinkClicked);
            // 
            // lnkView2
            // 
            this.lnkView2.AutoSize = true;
            this.lnkView2.BackColor = System.Drawing.Color.Transparent;
            this.lnkView2.Location = new System.Drawing.Point(117, 34);
            this.lnkView2.Name = "lnkView2";
            this.lnkView2.Size = new System.Drawing.Size(34, 17);
            this.lnkView2.TabIndex = 7;
            this.lnkView2.TabStop = true;
            this.lnkView2.Text = "檢視";
            this.lnkView2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkView2_LinkClicked);
            // 
            // lnkView3
            // 
            this.lnkView3.AutoSize = true;
            this.lnkView3.BackColor = System.Drawing.Color.Transparent;
            this.lnkView3.Location = new System.Drawing.Point(117, 61);
            this.lnkView3.Name = "lnkView3";
            this.lnkView3.Size = new System.Drawing.Size(34, 17);
            this.lnkView3.TabIndex = 8;
            this.lnkView3.TabStop = true;
            this.lnkView3.Text = "檢視";
            this.lnkView3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkView3_LinkClicked);
            // 
            // lnkUpload1
            // 
            this.lnkUpload1.AutoSize = true;
            this.lnkUpload1.BackColor = System.Drawing.Color.Transparent;
            this.lnkUpload1.Location = new System.Drawing.Point(157, 34);
            this.lnkUpload1.Name = "lnkUpload1";
            this.lnkUpload1.Size = new System.Drawing.Size(60, 17);
            this.lnkUpload1.TabIndex = 9;
            this.lnkUpload1.TabStop = true;
            this.lnkUpload1.Text = "上傳範本";
            this.lnkUpload1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUpload1_LinkClicked);
            // 
            // lnkUpload2
            // 
            this.lnkUpload2.AutoSize = true;
            this.lnkUpload2.BackColor = System.Drawing.Color.Transparent;
            this.lnkUpload2.Location = new System.Drawing.Point(157, 61);
            this.lnkUpload2.Name = "lnkUpload2";
            this.lnkUpload2.Size = new System.Drawing.Size(60, 17);
            this.lnkUpload2.TabIndex = 10;
            this.lnkUpload2.TabStop = true;
            this.lnkUpload2.Text = "上傳範本";
            this.lnkUpload2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUpload2_LinkClicked);
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.lnkView1);
            this.groupPanel1.Controls.Add(this.rbDefault);
            this.groupPanel1.Controls.Add(this.lnkUpload2);
            this.groupPanel1.Controls.Add(this.lnkView2);
            this.groupPanel1.Controls.Add(this.rbCustomize2);
            this.groupPanel1.Controls.Add(this.lnkUpload1);
            this.groupPanel1.Controls.Add(this.lnkView3);
            this.groupPanel1.Controls.Add(this.rbCustomize1);
            this.groupPanel1.Location = new System.Drawing.Point(12, 12);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(291, 117);
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
            this.groupPanel1.Style.Class = "";
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.Class = "";
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.Class = "";
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 12;
            this.groupPanel1.Text = "請選擇列印範本";
            // 
            // lnkAllFields
            // 
            this.lnkAllFields.AutoSize = true;
            this.lnkAllFields.BackColor = System.Drawing.Color.Transparent;
            this.lnkAllFields.Location = new System.Drawing.Point(19, 141);
            this.lnkAllFields.Name = "lnkAllFields";
            this.lnkAllFields.Size = new System.Drawing.Size(86, 17);
            this.lnkAllFields.TabIndex = 13;
            this.lnkAllFields.TabStop = true;
            this.lnkAllFields.Text = "合併欄位總表";
            this.lnkAllFields.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAllFields_LinkClicked);
            // 
            // PrintConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 170);
            this.Controls.Add(this.lnkAllFields);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.DoubleBuffered = true;
            this.Name = "PrintConfigForm";
            this.Text = "列印設定";
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbDefault;
        private System.Windows.Forms.RadioButton rbCustomize1;
        private System.Windows.Forms.RadioButton rbCustomize2;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private System.Windows.Forms.LinkLabel lnkView1;
        private System.Windows.Forms.LinkLabel lnkView2;
        private System.Windows.Forms.LinkLabel lnkView3;
        private System.Windows.Forms.LinkLabel lnkUpload1;
        private System.Windows.Forms.LinkLabel lnkUpload2;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.LinkLabel lnkAllFields;

    }
}