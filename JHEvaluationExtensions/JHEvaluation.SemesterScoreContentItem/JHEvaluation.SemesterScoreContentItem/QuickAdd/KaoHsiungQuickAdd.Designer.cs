namespace JHEvaluation.SemesterScoreContentItem.QuickAdd
{
    partial class KaoHsiungQuickAdd
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
            this.dgvDomain = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.chDomain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chPC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chEffort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDomain)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDomain
            // 
            this.dgvDomain.AllowUserToAddRows = false;
            this.dgvDomain.AllowUserToDeleteRows = false;
            this.dgvDomain.BackgroundColor = System.Drawing.Color.White;
            this.dgvDomain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvDomain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDomain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chDomain,
            this.chPC,
            this.chScore,
            this.chEffort,
            this.chText});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDomain.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDomain.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvDomain.Location = new System.Drawing.Point(8, 50);
            this.dgvDomain.Name = "dgvDomain";
            this.dgvDomain.RowHeadersVisible = false;
            this.dgvDomain.Size = new System.Drawing.Size(626, 264);
            this.dgvDomain.TabIndex = 0;
            // 
            // chDomain
            // 
            this.chDomain.HeaderText = "領域";
            this.chDomain.Name = "chDomain";
            this.chDomain.ReadOnly = true;
            this.chDomain.Width = 150;
            // 
            // chPC
            // 
            this.chPC.HeaderText = "節權";
            this.chPC.Name = "chPC";
            this.chPC.Width = 45;
            // 
            // chScore
            // 
            this.chScore.HeaderText = "分數";
            this.chScore.Name = "chScore";
            this.chScore.Width = 45;
            // 
            // chEffort
            // 
            this.chEffort.HeaderText = "努力程度";
            this.chEffort.Name = "chEffort";
            this.chEffort.Width = 59;
            // 
            // chText
            // 
            this.chText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.chText.HeaderText = "文字評量";
            this.chText.Name = "chText";
            // 
            // KaoHsiungQuickAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 512);
            this.Controls.Add(this.dgvDomain);
            this.Name = "KaoHsiungQuickAdd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KaoHsiungQuickAdd";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDomain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgvDomain;
        private System.Windows.Forms.DataGridViewTextBoxColumn chDomain;
        private System.Windows.Forms.DataGridViewTextBoxColumn chPC;
        private System.Windows.Forms.DataGridViewTextBoxColumn chScore;
        private System.Windows.Forms.DataGridViewTextBoxColumn chEffort;
        private System.Windows.Forms.DataGridViewTextBoxColumn chText;
    }
}