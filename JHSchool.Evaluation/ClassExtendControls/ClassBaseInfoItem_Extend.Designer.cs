namespace JHSchool.Evaluation.ClassExtendControls
{
    partial class ClassBaseInfoItem_Extend
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
            this.cboProgramPlan = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // cboProgramPlan
            // 
            this.cboProgramPlan.DisplayMember = "Name";
            this.cboProgramPlan.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboProgramPlan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProgramPlan.FormattingEnabled = true;
            this.cboProgramPlan.ItemHeight = 19;
            this.cboProgramPlan.Location = new System.Drawing.Point(84, 0);
            this.cboProgramPlan.Name = "cboProgramPlan";
            this.cboProgramPlan.Size = new System.Drawing.Size(250, 25);
            this.cboProgramPlan.TabIndex = 0;
            this.cboProgramPlan.ValueMember = "ID";
            this.cboProgramPlan.SelectedIndexChanged += new System.EventHandler(this.cboProgramPlan_SelectedIndexChanged);
            // 
            // labelX1
            // 
            this.labelX1.Location = new System.Drawing.Point(18, 1);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(60, 23);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "課程規劃";
            // 
            // ClassBaseInfoItem_Extend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.cboProgramPlan);
            this.Name = "ClassBaseInfoItem_Extend";
            this.Size = new System.Drawing.Size(550, 40);
            this.Load += new System.EventHandler(this.ClassBaseInfoItem_Extend_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ComboBoxEx cboProgramPlan;
        private DevComponents.DotNetBar.LabelX labelX1;
    }
}
