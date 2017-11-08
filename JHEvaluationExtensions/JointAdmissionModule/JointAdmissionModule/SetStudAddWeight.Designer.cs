namespace JointAdmissionModule
{
    partial class SetStudAddWeight
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvSHWeight = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.groupPanel2 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.dgvSH5Weight = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.JoinStudType5 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.StudType5 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AddWeight5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JoinStudTypeCode5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkNonRank5 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.txtMsg = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JoinStudType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.StudType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AddWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JoinStudTypeCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkNonRank = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSHWeight)).BeginInit();
            this.groupPanel1.SuspendLayout();
            this.groupPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSH5Weight)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSHWeight
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSHWeight.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSHWeight.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSHWeight.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.JoinStudType,
            this.StudType,
            this.AddWeight,
            this.JoinStudTypeCode,
            this.checkNonRank});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSHWeight.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSHWeight.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvSHWeight.Location = new System.Drawing.Point(3, 4);
            this.dgvSHWeight.Name = "dgvSHWeight";
            this.dgvSHWeight.RowTemplate.Height = 24;
            this.dgvSHWeight.Size = new System.Drawing.Size(759, 155);
            this.dgvSHWeight.TabIndex = 0;
            this.dgvSHWeight.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvSHWeight_CurrentCellDirtyStateChanged);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(601, 477);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(701, 477);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 0;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.dgvSHWeight);
            this.groupPanel1.Location = new System.Drawing.Point(12, 86);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(771, 189);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.groupPanel1.TabIndex = 2;
            this.groupPanel1.Text = "高中特種身分加分比、不參與排名";
            // 
            // groupPanel2
            // 
            this.groupPanel2.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel2.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel2.Controls.Add(this.dgvSH5Weight);
            this.groupPanel2.Location = new System.Drawing.Point(12, 281);
            this.groupPanel2.Name = "groupPanel2";
            this.groupPanel2.Size = new System.Drawing.Size(771, 190);
            // 
            // 
            // 
            this.groupPanel2.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel2.Style.BackColorGradientAngle = 90;
            this.groupPanel2.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel2.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderBottomWidth = 1;
            this.groupPanel2.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel2.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderLeftWidth = 1;
            this.groupPanel2.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderRightWidth = 1;
            this.groupPanel2.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderTopWidth = 1;
            this.groupPanel2.Style.CornerDiameter = 4;
            this.groupPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel2.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel2.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.groupPanel2.TabIndex = 3;
            this.groupPanel2.Text = "五專特種身分加分比、不參與排名";
            // 
            // dgvSH5Weight
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSH5Weight.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSH5Weight.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSH5Weight.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.JoinStudType5,
            this.StudType5,
            this.AddWeight5,
            this.JoinStudTypeCode5,
            this.checkNonRank5});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSH5Weight.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvSH5Weight.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvSH5Weight.Location = new System.Drawing.Point(3, 4);
            this.dgvSH5Weight.Name = "dgvSH5Weight";
            this.dgvSH5Weight.RowTemplate.Height = 24;
            this.dgvSH5Weight.Size = new System.Drawing.Size(756, 152);
            this.dgvSH5Weight.TabIndex = 0;
            this.dgvSH5Weight.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvSH5Weight_CurrentCellDirtyStateChanged);
            // 
            // JoinStudType5
            // 
            this.JoinStudType5.HeaderText = "考生特種身分別";
            this.JoinStudType5.Name = "JoinStudType5";
            this.JoinStudType5.Width = 220;
            // 
            // StudType5
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.StudType5.DefaultCellStyle = dataGridViewCellStyle5;
            this.StudType5.HeaderText = "學生類別";
            this.StudType5.Name = "StudType5";
            this.StudType5.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.StudType5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.StudType5.Width = 250;
            // 
            // AddWeight5
            // 
            this.AddWeight5.HeaderText = "加分比重";
            this.AddWeight5.Name = "AddWeight5";
            this.AddWeight5.Width = 80;
            // 
            // JoinStudTypeCode5
            // 
            this.JoinStudTypeCode5.HeaderText = "特種身分代號";
            this.JoinStudTypeCode5.Name = "JoinStudTypeCode5";
            // 
            // checkNonRank5
            // 
            this.checkNonRank5.HeaderText = "不參與排名";
            this.checkNonRank5.Name = "checkNonRank5";
            // 
            // txtMsg
            // 
            // 
            // 
            // 
            this.txtMsg.Border.Class = "TextBoxBorder";
            this.txtMsg.Location = new System.Drawing.Point(12, 13);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(771, 67);
            this.txtMsg.TabIndex = 4;
            this.txtMsg.Text = "使用說明：請設定考生特種身分別與學生類別對照，透過學生類別來辨識學生的加分比重與學生是否不參與排名，加分比重以1為基準，例如加分35%，請輸入1.35；當不參與排" +
                "名勾起，表示該類別學生不會被列入排名範圍，在報表也不會出現。";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "加分比重";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "加分比重";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // JoinStudType
            // 
            this.JoinStudType.HeaderText = "考生特種身分別";
            this.JoinStudType.Name = "JoinStudType";
            this.JoinStudType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.JoinStudType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.JoinStudType.Width = 220;
            // 
            // StudType
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.StudType.DefaultCellStyle = dataGridViewCellStyle2;
            this.StudType.HeaderText = "學生類別";
            this.StudType.Name = "StudType";
            this.StudType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.StudType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.StudType.Width = 250;
            // 
            // AddWeight
            // 
            this.AddWeight.HeaderText = "加分比重";
            this.AddWeight.Name = "AddWeight";
            this.AddWeight.Width = 80;
            // 
            // JoinStudTypeCode
            // 
            this.JoinStudTypeCode.HeaderText = "特種身分代碼";
            this.JoinStudTypeCode.Name = "JoinStudTypeCode";
            // 
            // checkNonRank
            // 
            this.checkNonRank.HeaderText = "不參與排名";
            this.checkNonRank.Name = "checkNonRank";
            // 
            // SetStudAddWeight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 506);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.groupPanel2);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Name = "SetStudAddWeight";
            this.Text = "設定學生特種身分加分比、不參與排名";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSHWeight)).EndInit();
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSH5Weight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgvSHWeight;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel2;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgvSH5Weight;
        private DevComponents.DotNetBar.Controls.TextBoxX txtMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewComboBoxColumn JoinStudType5;
        private System.Windows.Forms.DataGridViewComboBoxColumn StudType5;
        private System.Windows.Forms.DataGridViewTextBoxColumn AddWeight5;
        private System.Windows.Forms.DataGridViewTextBoxColumn JoinStudTypeCode5;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkNonRank5;
        private System.Windows.Forms.DataGridViewComboBoxColumn JoinStudType;
        private System.Windows.Forms.DataGridViewComboBoxColumn StudType;
        private System.Windows.Forms.DataGridViewTextBoxColumn AddWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn JoinStudTypeCode;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkNonRank;
    }
}