namespace JHEvaluation.ScoreCalculation
{
    partial class StudentsView<T>
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgStudents = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.chClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chSeatNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chStudentNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblMsg = new DevComponents.DotNetBar.LabelX();
            this.btnAddToTemp = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.btnExportExcel = new DevComponents.DotNetBar.ButtonX();
            this.btnContinue = new DevComponents.DotNetBar.ButtonX();
            ((System.ComponentModel.ISupportInitialize)(this.dgStudents)).BeginInit();
            this.SuspendLayout();
            // 
            // dgStudents
            // 
            this.dgStudents.AllowUserToAddRows = false;
            this.dgStudents.AllowUserToDeleteRows = false;
            this.dgStudents.AllowUserToResizeColumns = false;
            this.dgStudents.AllowUserToResizeRows = false;
            this.dgStudents.BackgroundColor = System.Drawing.Color.White;
            this.dgStudents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgStudents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chClassName,
            this.chSeatNo,
            this.chName,
            this.chStudentNumber});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgStudents.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgStudents.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgStudents.Location = new System.Drawing.Point(12, 58);
            this.dgStudents.Name = "dgStudents";
            this.dgStudents.ReadOnly = true;
            this.dgStudents.RowTemplate.Height = 24;
            this.dgStudents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgStudents.Size = new System.Drawing.Size(518, 247);
            this.dgStudents.TabIndex = 0;
            // 
            // chClassName
            // 
            this.chClassName.DataPropertyName = "ClassName";
            this.chClassName.HeaderText = "班級";
            this.chClassName.Name = "chClassName";
            this.chClassName.ReadOnly = true;
            // 
            // chSeatNo
            // 
            this.chSeatNo.DataPropertyName = "SeatNo";
            this.chSeatNo.HeaderText = "座號";
            this.chSeatNo.Name = "chSeatNo";
            this.chSeatNo.ReadOnly = true;
            // 
            // chName
            // 
            this.chName.DataPropertyName = "Name";
            this.chName.HeaderText = "姓名";
            this.chName.Name = "chName";
            this.chName.ReadOnly = true;
            // 
            // chStudentNumber
            // 
            this.chStudentNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.chStudentNumber.DataPropertyName = "StudentNumber";
            this.chStudentNumber.HeaderText = "學號";
            this.chStudentNumber.Name = "chStudentNumber";
            this.chStudentNumber.ReadOnly = true;
            // 
            // lblMsg
            // 
            this.lblMsg.BackColor = System.Drawing.Color.Transparent;
            this.lblMsg.Location = new System.Drawing.Point(12, 12);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(518, 40);
            this.lblMsg.TabIndex = 1;
            this.lblMsg.Text = "Message";
            this.lblMsg.WordWrap = true;
            // 
            // btnAddToTemp
            // 
            this.btnAddToTemp.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAddToTemp.BackColor = System.Drawing.Color.Transparent;
            this.btnAddToTemp.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnAddToTemp.Location = new System.Drawing.Point(12, 311);
            this.btnAddToTemp.Name = "btnAddToTemp";
            this.btnAddToTemp.Size = new System.Drawing.Size(95, 23);
            this.btnAddToTemp.TabIndex = 2;
            this.btnAddToTemp.Text = "加入待處理";
            this.btnAddToTemp.Click += new System.EventHandler(this.btnAddToTemp_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(455, 311);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "關閉";
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExportExcel.BackColor = System.Drawing.Color.Transparent;
            this.btnExportExcel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExportExcel.Location = new System.Drawing.Point(113, 311);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(95, 23);
            this.btnExportExcel.TabIndex = 4;
            this.btnExportExcel.Text = "匯出 Excel";
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnContinue.BackColor = System.Drawing.Color.Transparent;
            this.btnContinue.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.btnContinue.Location = new System.Drawing.Point(312, 311);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(137, 23);
            this.btnContinue.TabIndex = 5;
            this.btnContinue.Text = "不管...我要繼續...";
            this.btnContinue.Visible = false;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // StudentsView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(542, 341);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.btnExportExcel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAddToTemp);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.dgStudents);
            this.Name = "StudentsView";
            this.Text = "資料檢視";
            this.Load += new System.EventHandler(this.StudentsView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgStudents)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgStudents;
        private System.Windows.Forms.DataGridViewTextBoxColumn chClassName;
        private System.Windows.Forms.DataGridViewTextBoxColumn chSeatNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn chName;
        private System.Windows.Forms.DataGridViewTextBoxColumn chStudentNumber;
        private DevComponents.DotNetBar.LabelX lblMsg;
        private DevComponents.DotNetBar.ButtonX btnAddToTemp;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.ButtonX btnExportExcel;
        private DevComponents.DotNetBar.ButtonX btnContinue;
    }
}