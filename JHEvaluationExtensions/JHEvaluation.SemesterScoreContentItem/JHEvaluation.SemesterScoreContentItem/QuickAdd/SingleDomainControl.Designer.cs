namespace JHEvaluation.SemesterScoreContentItem.QuickAdd
{
    partial class SingleDomainControl
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDomain = new DevComponents.DotNetBar.LabelX();
            this.txtPC = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtScore = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtEffort = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtText = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.SuspendLayout();
            // 
            // lblDomain
            // 
            this.lblDomain.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lblDomain.Location = new System.Drawing.Point(20, 64);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(127, 22);
            this.lblDomain.TabIndex = 0;
            this.lblDomain.Text = "自然與生活科技";
            this.lblDomain.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // txtPC
            // 
            // 
            // 
            // 
            this.txtPC.Border.Class = "TextBoxBorder";
            this.txtPC.Location = new System.Drawing.Point(153, 64);
            this.txtPC.Name = "txtPC";
            this.txtPC.Size = new System.Drawing.Size(50, 25);
            this.txtPC.TabIndex = 1;
            this.txtPC.Text = "3.6/5.5";
            // 
            // txtScore
            // 
            // 
            // 
            // 
            this.txtScore.Border.Class = "TextBoxBorder";
            this.txtScore.Location = new System.Drawing.Point(212, 64);
            this.txtScore.Name = "txtScore";
            this.txtScore.Size = new System.Drawing.Size(40, 25);
            this.txtScore.TabIndex = 1;
            this.txtScore.Text = "100";
            // 
            // txtEffort
            // 
            // 
            // 
            // 
            this.txtEffort.Border.Class = "TextBoxBorder";
            this.txtEffort.Location = new System.Drawing.Point(260, 64);
            this.txtEffort.Name = "txtEffort";
            this.txtEffort.Size = new System.Drawing.Size(30, 25);
            this.txtEffort.TabIndex = 1;
            // 
            // txtText
            // 
            // 
            // 
            // 
            this.txtText.Border.Class = "TextBoxBorder";
            this.txtText.Location = new System.Drawing.Point(296, 64);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(241, 25);
            this.txtText.TabIndex = 1;
            // 
            // SingleDomainControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.txtEffort);
            this.Controls.Add(this.txtScore);
            this.Controls.Add(this.txtPC);
            this.Controls.Add(this.lblDomain);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SingleDomainControl";
            this.Size = new System.Drawing.Size(560, 226);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX lblDomain;
        private DevComponents.DotNetBar.Controls.TextBoxX txtPC;
        private DevComponents.DotNetBar.Controls.TextBoxX txtScore;
        private DevComponents.DotNetBar.Controls.TextBoxX txtEffort;
        private DevComponents.DotNetBar.Controls.TextBoxX txtText;
    }
}
