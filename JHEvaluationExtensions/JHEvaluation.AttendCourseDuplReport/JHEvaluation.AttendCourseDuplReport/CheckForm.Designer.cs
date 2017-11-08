namespace JHEvaluation.AttendCourseDuplReport
{
    partial class CheckForm
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.lblStudentCount = new DevComponents.DotNetBar.LabelX();
            this.lblCourseCount = new DevComponents.DotNetBar.LabelX();
            this.btnStudentTemporary = new DevComponents.DotNetBar.ButtonX();
            this.btnCourseTemporary = new DevComponents.DotNetBar.ButtonX();
            this.btnExport = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            this.labelX1.Location = new System.Drawing.Point(13, 13);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(86, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "重覆學生數：";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            this.labelX2.Location = new System.Drawing.Point(13, 47);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(86, 23);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "重覆課程數：";
            // 
            // lblStudentCount
            // 
            this.lblStudentCount.BackColor = System.Drawing.Color.Transparent;
            this.lblStudentCount.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStudentCount.Location = new System.Drawing.Point(97, 14);
            this.lblStudentCount.Name = "lblStudentCount";
            this.lblStudentCount.Size = new System.Drawing.Size(52, 23);
            this.lblStudentCount.TabIndex = 1;
            this.lblStudentCount.Text = "0";
            // 
            // lblCourseCount
            // 
            this.lblCourseCount.BackColor = System.Drawing.Color.Transparent;
            this.lblCourseCount.Location = new System.Drawing.Point(97, 48);
            this.lblCourseCount.Name = "lblCourseCount";
            this.lblCourseCount.Size = new System.Drawing.Size(52, 23);
            this.lblCourseCount.TabIndex = 4;
            this.lblCourseCount.Text = "0";
            // 
            // btnStudentTemporary
            // 
            this.btnStudentTemporary.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnStudentTemporary.BackColor = System.Drawing.Color.Transparent;
            this.btnStudentTemporary.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnStudentTemporary.Location = new System.Drawing.Point(155, 11);
            this.btnStudentTemporary.Name = "btnStudentTemporary";
            this.btnStudentTemporary.Size = new System.Drawing.Size(125, 25);
            this.btnStudentTemporary.TabIndex = 2;
            this.btnStudentTemporary.Text = "將學生加入待處理";
            this.btnStudentTemporary.Click += new System.EventHandler(this.btnStudentTemporary_Click);
            // 
            // btnCourseTemporary
            // 
            this.btnCourseTemporary.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCourseTemporary.BackColor = System.Drawing.Color.Transparent;
            this.btnCourseTemporary.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCourseTemporary.Location = new System.Drawing.Point(155, 45);
            this.btnCourseTemporary.Name = "btnCourseTemporary";
            this.btnCourseTemporary.Size = new System.Drawing.Size(125, 25);
            this.btnCourseTemporary.TabIndex = 5;
            this.btnCourseTemporary.Text = "將課程加入待處理";
            this.btnCourseTemporary.Click += new System.EventHandler(this.btnCourseTemporary_Click);
            // 
            // btnExport
            // 
            this.btnExport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExport.BackColor = System.Drawing.Color.Transparent;
            this.btnExport.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExport.Location = new System.Drawing.Point(73, 97);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(125, 28);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "匯出報表";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(210, 97);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(70, 28);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // CheckForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(292, 137);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnCourseTemporary);
            this.Controls.Add(this.btnStudentTemporary);
            this.Controls.Add(this.lblCourseCount);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.lblStudentCount);
            this.Controls.Add(this.labelX1);
            this.Name = "CheckForm";
            this.Text = "學生學期修課檢查表";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX lblStudentCount;
        private DevComponents.DotNetBar.LabelX lblCourseCount;
        private DevComponents.DotNetBar.ButtonX btnStudentTemporary;
        private DevComponents.DotNetBar.ButtonX btnCourseTemporary;
        private DevComponents.DotNetBar.ButtonX btnExport;
        private DevComponents.DotNetBar.ButtonX btnClose;
    }
}