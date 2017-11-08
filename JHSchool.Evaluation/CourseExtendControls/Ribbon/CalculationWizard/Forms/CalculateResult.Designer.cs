namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.CalculationWizard.Forms
{
    partial class CalculateResult
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.CourseList = new SmartSchool.Common.ListViewEX();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.StudentList = new SmartSchool.Common.ListViewEX();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnClearData = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // CourseList
            // 
            this.CourseList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            // 
            // 
            // 
            this.CourseList.Border.Class = "ListViewBorder";
            this.CourseList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.CourseList.FullRowSelect = true;
            this.CourseList.Location = new System.Drawing.Point(12, 12);
            this.CourseList.Name = "CourseList";
            this.CourseList.Size = new System.Drawing.Size(191, 342);
            this.CourseList.TabIndex = 0;
            this.CourseList.UseCompatibleStateImageBehavior = false;
            this.CourseList.View = System.Windows.Forms.View.Details;
            this.CourseList.SelectedIndexChanged += new System.EventHandler(this.CourseList_Click);
            this.CourseList.Click += new System.EventHandler(this.CourseList_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "課程名稱";
            this.columnHeader1.Width = 159;
            // 
            // StudentList
            // 
            this.StudentList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.StudentList.Border.Class = "ListViewBorder";
            this.StudentList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader6,
            this.columnHeader5});
            this.StudentList.FullRowSelect = true;
            this.StudentList.Location = new System.Drawing.Point(209, 11);
            this.StudentList.Name = "StudentList";
            this.StudentList.ShowItemToolTips = true;
            this.StudentList.Size = new System.Drawing.Size(423, 342);
            this.StudentList.TabIndex = 1;
            this.StudentList.UseCompatibleStateImageBehavior = false;
            this.StudentList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "班級";
            this.columnHeader2.Width = 74;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "學號";
            this.columnHeader3.Width = 82;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "姓名";
            this.columnHeader4.Width = 77;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "計算前成績";
            this.columnHeader6.Width = 91;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "成績";
            this.columnHeader5.Width = 69;
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(557, 365);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(421, 365);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "儲存所有計算結果";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClearData
            // 
            this.btnClearData.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClearData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearData.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClearData.Location = new System.Drawing.Point(12, 366);
            this.btnClearData.Name = "btnClearData";
            this.btnClearData.Size = new System.Drawing.Size(131, 23);
            this.btnClearData.TabIndex = 2;
            this.btnClearData.Text = "清除資料庫中結果";
            this.btnClearData.Visible = false;
            this.btnClearData.Click += new System.EventHandler(this.btnClearData_Click);
            // 
            // CalculateResult
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(644, 400);
            this.Controls.Add(this.btnClearData);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.StudentList);
            this.Controls.Add(this.CourseList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(660, 436);
            this.Name = "CalculateResult";
            this.Text = "計算結果";
            this.DoubleClick += new System.EventHandler(this.CalculateResult_DoubleClick);
            this.ResumeLayout(false);

        }

        #endregion

        private SmartSchool.Common.ListViewEX CourseList;
        private SmartSchool.Common.ListViewEX StudentList;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private DevComponents.DotNetBar.ButtonX btnClearData;
    }
}