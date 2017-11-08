namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard.Forms
{
    partial class CalculateProgress
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
            this.Progress = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.SuspendLayout();
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(15, 11);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(377, 23);
            this.Progress.TabIndex = 6;
            this.Progress.Text = "progressBarX1";
            // 
            // CalculateProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 46);
            this.ControlBox = false;
            this.Controls.Add(this.Progress);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalculateProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "儲存課程成績";
            this.Load += new System.EventHandler(this.CalculateProgress_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ProgressBarX Progress;
    }
}