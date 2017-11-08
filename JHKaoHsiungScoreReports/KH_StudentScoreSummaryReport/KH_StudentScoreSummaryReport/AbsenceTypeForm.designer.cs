namespace KH_StudentScoreSummaryReport
{
    partial class AbsenceTypeForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.dgAbsence = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.chCatalog = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chAbsenceTemplate = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.chkSelectAll = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.lblLost = new DevComponents.DotNetBar.LabelX();
            ((System.ComponentModel.ISupportInitialize)(this.dgAbsence)).BeginInit();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(141, 21);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "請選擇您要列印的假別";
            // 
            // dgAbsence
            // 
            this.dgAbsence.AllowUserToAddRows = false;
            this.dgAbsence.AllowUserToDeleteRows = false;
            this.dgAbsence.AllowUserToResizeRows = false;
            this.dgAbsence.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgAbsence.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgAbsence.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgAbsence.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAbsence.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chCatalog,
            this.chAbsenceTemplate});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgAbsence.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgAbsence.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgAbsence.Location = new System.Drawing.Point(12, 39);
            this.dgAbsence.Name = "dgAbsence";
            this.dgAbsence.RowHeadersVisible = false;
            this.dgAbsence.RowHeadersWidth = 35;
            this.dgAbsence.RowTemplate.Height = 24;
            this.dgAbsence.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgAbsence.Size = new System.Drawing.Size(561, 128);
            this.dgAbsence.TabIndex = 1;
            // 
            // chCatalog
            // 
            this.chCatalog.HeaderText = "節次分類";
            this.chCatalog.Name = "chCatalog";
            this.chCatalog.ReadOnly = true;
            this.chCatalog.Width = 75;
            // 
            // chAbsenceTemplate
            // 
            this.chAbsenceTemplate.HeaderText = "ColumnName";
            this.chAbsenceTemplate.Name = "chAbsenceTemplate";
            this.chAbsenceTemplate.Width = 60;
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(498, 173);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "儲存設定";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkSelectAll.BackgroundStyle.Class = "";
            this.chkSelectAll.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkSelectAll.Location = new System.Drawing.Point(12, 38);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(54, 21);
            this.chkSelectAll.TabIndex = 3;
            this.chkSelectAll.Text = "全選";
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // lblLost
            // 
            this.lblLost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLost.AutoSize = true;
            this.lblLost.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblLost.BackgroundStyle.Class = "";
            this.lblLost.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblLost.ForeColor = System.Drawing.Color.Red;
            this.lblLost.Location = new System.Drawing.Point(136, 174);
            this.lblLost.Name = "lblLost";
            this.lblLost.Size = new System.Drawing.Size(356, 21);
            this.lblLost.TabIndex = 4;
            this.lblLost.Text = "※ 部份資料並未顯示在畫面上，儲存後可能會遺失該資料。";
            this.lblLost.Visible = false;
            // 
            // AbsenceTypeForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(585, 206);
            this.Controls.Add(this.lblLost);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgAbsence);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.Name = "AbsenceTypeForm";
            this.Text = "假別設定";
            this.Load += new System.EventHandler(this.AbsenceTypeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgAbsence)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgAbsence;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkSelectAll;
        private DevComponents.DotNetBar.LabelX lblLost;
        private System.Windows.Forms.DataGridViewTextBoxColumn chCatalog;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chAbsenceTemplate;
    }
}