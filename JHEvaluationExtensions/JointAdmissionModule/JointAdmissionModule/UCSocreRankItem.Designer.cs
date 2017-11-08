namespace JointAdmissionModule
{
    partial class UCSocreRankItem
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.SchoolYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Semester = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GradeYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GradeRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GradeRankPercent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GradeRankAdd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GradeRankPercentAdd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SchoolYear,
            this.Semester,
            this.GradeYear,
            this.GradeRank,
            this.GradeRankPercent,
            this.GradeRankAdd,
            this.GradeRankPercentAdd});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgv.Location = new System.Drawing.Point(3, 4);
            this.dgv.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(547, 140);
            this.dgv.TabIndex = 3;
            this.dgv.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgv_CurrentCellDirtyStateChanged);
            // 
            // SchoolYear
            // 
            this.SchoolYear.HeaderText = "學年度";
            this.SchoolYear.Name = "SchoolYear";
            this.SchoolYear.Width = 60;
            // 
            // Semester
            // 
            this.Semester.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Semester.HeaderText = "學期";
            this.Semester.Name = "Semester";
            this.Semester.Width = 53;
            // 
            // GradeYear
            // 
            this.GradeYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.GradeYear.HeaderText = "年級";
            this.GradeYear.Name = "GradeYear";
            this.GradeYear.Width = 53;
            // 
            // GradeRank
            // 
            this.GradeRank.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.GradeRank.HeaderText = "年排名";
            this.GradeRank.Name = "GradeRank";
            this.GradeRank.Width = 64;
            // 
            // GradeRankPercent
            // 
            this.GradeRankPercent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.GradeRankPercent.HeaderText = "年排名百分比";
            this.GradeRankPercent.Name = "GradeRankPercent";
            this.GradeRankPercent.Width = 75;
            // 
            // GradeRankAdd
            // 
            this.GradeRankAdd.HeaderText = "加分後年排名";
            this.GradeRankAdd.Name = "GradeRankAdd";
            // 
            // GradeRankPercentAdd
            // 
            this.GradeRankPercentAdd.HeaderText = "加分後年排名百分比";
            this.GradeRankPercentAdd.Name = "GradeRankPercentAdd";
            this.GradeRankPercentAdd.Width = 97;
            // 
            // UCSocreRankItem
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgv);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UCSocreRankItem";
            this.Size = new System.Drawing.Size(550, 155);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn SchoolYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn Semester;
        private System.Windows.Forms.DataGridViewTextBoxColumn GradeYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn GradeRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn GradeRankPercent;
        private System.Windows.Forms.DataGridViewTextBoxColumn GradeRankAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn GradeRankPercentAdd;
    }
}
