namespace JHSchool.Evaluation.CourseExtendControls.Ribbon
{
    partial class CalculateionWizard
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
            this.lblTitle = new DevComponents.DotNetBar.LabelX();
            this.lblDescription = new DevComponents.DotNetBar.LabelX();
            this.btnCalcuate = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.lblCourseCount = new DevComponents.DotNetBar.LabelX();
            this.btnExport = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(98, 17);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "計算課程成績";
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(12, 58);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(373, 77);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "提醒您：學生於各次評量中若成績空白或為缺考狀況者，若您確定進行計算，則均以0分進行課程成績計算。若課程「未設定評分樣版」或「不需評分」，則系統不進行課程成績計算。" +
                "";
            this.lblDescription.TextLineAlignment = System.Drawing.StringAlignment.Near;
            this.lblDescription.WordWrap = true;
            // 
            // btnCalcuate
            // 
            this.btnCalcuate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCalcuate.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCalcuate.Location = new System.Drawing.Point(225, 252);
            this.btnCalcuate.Name = "btnCalcuate";
            this.btnCalcuate.Size = new System.Drawing.Size(75, 23);
            this.btnCalcuate.TabIndex = 3;
            this.btnCalcuate.Text = "計算";
            this.btnCalcuate.Click += new System.EventHandler(this.btnCalcuate_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(310, 252);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblCourseCount
            // 
            this.lblCourseCount.AutoSize = true;
            this.lblCourseCount.Location = new System.Drawing.Point(12, 146);
            this.lblCourseCount.Name = "lblCourseCount";
            this.lblCourseCount.Size = new System.Drawing.Size(211, 93);
            this.lblCourseCount.TabIndex = 1;
            this.lblCourseCount.Text = "已選擇課程總數：10\r\n不需評分之課程數：3\r\n未設定評分樣版課程數：3\r\n含有「不強制繳交」課程數：8\r\n含有「成績缺漏」之課程數：3\r\n\r\n";
            this.lblCourseCount.TextLineAlignment = System.Drawing.StringAlignment.Near;
            this.lblCourseCount.WordWrap = true;
            // 
            // btnExport
            // 
            this.btnExport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExport.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExport.Location = new System.Drawing.Point(12, 252);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(153, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "匯出選取課程總覽明細";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // CalculateWizard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(399, 288);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCalcuate);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lblCourseCount);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(405, 249);
            this.Name = "CalculateWizard";
            this.Text = "計算課程成績";
            this.Load += new System.EventHandler(this.CalculateWizard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX lblTitle;
        private DevComponents.DotNetBar.LabelX lblDescription;
        private DevComponents.DotNetBar.ButtonX btnCalcuate;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.LabelX lblCourseCount;
        private DevComponents.DotNetBar.ButtonX btnExport;
    }
}