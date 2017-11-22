namespace StudentSemesterScoreNotification
{
    partial class SelectTypeForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.colTarget = new DevComponents.DotNetBar.Controls.DataGridViewComboBoxExColumn();
            this.colSource = new DevComponents.DotNetBar.Controls.DataGridViewComboBoxExColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTarget,
            this.colSource});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgv.Location = new System.Drawing.Point(12, 12);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(396, 258);
            this.dgv.TabIndex = 4;
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSave.Location = new System.Drawing.Point(323, 278);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 25);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "儲存設定";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // colTarget
            // 
            this.colTarget.DisplayMember = "Text";
            this.colTarget.DropDownHeight = 106;
            this.colTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colTarget.DropDownWidth = 121;
            this.colTarget.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colTarget.HeaderText = "列印變數";
            this.colTarget.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.colTarget.IntegralHeight = false;
            this.colTarget.ItemHeight = 17;
            this.colTarget.Name = "colTarget";
            this.colTarget.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colTarget.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.colTarget.Width = 150;
            // 
            // colSource
            // 
            this.colSource.DisplayMember = "Text";
            this.colSource.DropDownHeight = 106;
            this.colSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colSource.DropDownWidth = 121;
            this.colSource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colSource.HeaderText = "對應假別";
            this.colSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.colSource.IntegralHeight = false;
            this.colSource.ItemHeight = 17;
            this.colSource.Name = "colSource";
            this.colSource.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSource.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.colSource.Width = 200;
            // 
            // SelectTypeForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(424, 310);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btnSave);
            this.DoubleBuffered = true;
            this.Name = "SelectTypeForm";
            this.Text = "假別設定";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgv;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.Controls.DataGridViewComboBoxExColumn colTarget;
        private DevComponents.DotNetBar.Controls.DataGridViewComboBoxExColumn colSource;


    }
}