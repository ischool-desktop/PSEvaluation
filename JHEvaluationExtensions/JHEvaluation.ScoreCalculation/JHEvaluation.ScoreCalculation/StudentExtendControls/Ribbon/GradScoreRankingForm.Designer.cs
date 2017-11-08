namespace JHSchool.Evaluation.StudentExtendControls.Ribbon
{
    partial class GradScoreRankingForm
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
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.rbGradeYear = new System.Windows.Forms.RadioButton();
            this.rbClass = new System.Windows.Forms.RadioButton();
            this.groupPanel2 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.txtTopRank = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label1 = new System.Windows.Forms.Label();
            this.rbTopRank = new System.Windows.Forms.RadioButton();
            this.rbAllRank = new System.Windows.Forms.RadioButton();
            this.chkPrintByClass = new System.Windows.Forms.CheckBox();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.btnRank = new DevComponents.DotNetBar.ButtonX();
            this.btnRankingOption = new DevComponents.DotNetBar.ButtonX();
            this.dgv = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.chCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.errorProvider = new Framework.EnhancedErrorProvider();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupPanel1.SuspendLayout();
            this.groupPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.panel1);
            this.groupPanel1.Location = new System.Drawing.Point(216, 7);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(170, 110);
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
            this.groupPanel1.TabIndex = 1;
            this.groupPanel1.Text = "排名項目";
            // 
            // rbGradeYear
            // 
            this.rbGradeYear.AutoSize = true;
            this.rbGradeYear.Location = new System.Drawing.Point(24, 44);
            this.rbGradeYear.Name = "rbGradeYear";
            this.rbGradeYear.Size = new System.Drawing.Size(78, 21);
            this.rbGradeYear.TabIndex = 0;
            this.rbGradeYear.Text = "年級排名";
            this.rbGradeYear.UseVisualStyleBackColor = true;
            // 
            // rbClass
            // 
            this.rbClass.AutoSize = true;
            this.rbClass.Checked = true;
            this.rbClass.Location = new System.Drawing.Point(24, 17);
            this.rbClass.Name = "rbClass";
            this.rbClass.Size = new System.Drawing.Size(78, 21);
            this.rbClass.TabIndex = 0;
            this.rbClass.TabStop = true;
            this.rbClass.Text = "班級排名";
            this.rbClass.UseVisualStyleBackColor = true;
            // 
            // groupPanel2
            // 
            this.groupPanel2.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel2.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel2.Controls.Add(this.panel2);
            this.groupPanel2.Location = new System.Drawing.Point(216, 123);
            this.groupPanel2.Name = "groupPanel2";
            this.groupPanel2.Size = new System.Drawing.Size(170, 90);
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
            this.groupPanel2.TabIndex = 1;
            // 
            // txtTopRank
            // 
            // 
            // 
            // 
            this.txtTopRank.Border.Class = "TextBoxBorder";
            this.txtTopRank.Location = new System.Drawing.Point(65, 39);
            this.txtTopRank.Name = "txtTopRank";
            this.txtTopRank.Size = new System.Drawing.Size(40, 25);
            this.txtTopRank.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "名/%";
            // 
            // rbTopRank
            // 
            this.rbTopRank.AutoSize = true;
            this.rbTopRank.Location = new System.Drawing.Point(23, 41);
            this.rbTopRank.Name = "rbTopRank";
            this.rbTopRank.Size = new System.Drawing.Size(39, 21);
            this.rbTopRank.TabIndex = 0;
            this.rbTopRank.Text = "前";
            this.rbTopRank.UseVisualStyleBackColor = true;
            // 
            // rbAllRank
            // 
            this.rbAllRank.AutoSize = true;
            this.rbAllRank.Checked = true;
            this.rbAllRank.Location = new System.Drawing.Point(23, 14);
            this.rbAllRank.Name = "rbAllRank";
            this.rbAllRank.Size = new System.Drawing.Size(78, 21);
            this.rbAllRank.TabIndex = 0;
            this.rbAllRank.TabStop = true;
            this.rbAllRank.Text = "所有名次";
            this.rbAllRank.UseVisualStyleBackColor = true;
            // 
            // chkPrintByClass
            // 
            this.chkPrintByClass.AutoSize = true;
            this.chkPrintByClass.BackColor = System.Drawing.Color.Transparent;
            this.chkPrintByClass.Location = new System.Drawing.Point(219, 222);
            this.chkPrintByClass.Name = "chkPrintByClass";
            this.chkPrintByClass.Size = new System.Drawing.Size(92, 21);
            this.chkPrintByClass.TabIndex = 2;
            this.chkPrintByClass.Text = "依班級分頁";
            this.chkPrintByClass.UseVisualStyleBackColor = false;
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(311, 248);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "離開";
            // 
            // btnRank
            // 
            this.btnRank.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRank.BackColor = System.Drawing.Color.Transparent;
            this.btnRank.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRank.Location = new System.Drawing.Point(229, 248);
            this.btnRank.Name = "btnRank";
            this.btnRank.Size = new System.Drawing.Size(75, 23);
            this.btnRank.TabIndex = 4;
            this.btnRank.Text = "排名";
            // 
            // btnRankingOption
            // 
            this.btnRankingOption.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRankingOption.BackColor = System.Drawing.Color.Transparent;
            this.btnRankingOption.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRankingOption.Location = new System.Drawing.Point(6, 248);
            this.btnRankingOption.Name = "btnRankingOption";
            this.btnRankingOption.Size = new System.Drawing.Size(75, 23);
            this.btnRankingOption.TabIndex = 3;
            this.btnRankingOption.Text = "排名設定";
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chCheck,
            this.chCategory});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgv.Location = new System.Drawing.Point(6, 7);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(204, 236);
            this.dgv.TabIndex = 6;
            // 
            // chCheck
            // 
            this.chCheck.HeaderText = "";
            this.chCheck.Name = "chCheck";
            this.chCheck.Width = 30;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkRate = 0;
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbClass);
            this.panel1.Controls.Add(this.rbGradeYear);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(164, 83);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbAllRank);
            this.panel2.Controls.Add(this.txtTopRank);
            this.panel2.Controls.Add(this.rbTopRank);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(164, 84);
            this.panel2.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "領域";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // chCategory
            // 
            this.chCategory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.chCategory.HeaderText = "領域";
            this.chCategory.Name = "chCategory";
            // 
            // GradScoreRankingForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(392, 276);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRank);
            this.Controls.Add(this.btnRankingOption);
            this.Controls.Add(this.chkPrintByClass);
            this.Controls.Add(this.groupPanel2);
            this.Controls.Add(this.groupPanel1);
            this.Name = "GradScoreRankingForm";
            this.Text = "畢業成績排名";
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.RadioButton rbGradeYear;
        private System.Windows.Forms.RadioButton rbClass;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel2;
        private System.Windows.Forms.RadioButton rbTopRank;
        private System.Windows.Forms.RadioButton rbAllRank;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTopRank;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkPrintByClass;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnRank;
        private DevComponents.DotNetBar.ButtonX btnRankingOption;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgv;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn chCategory;
        private Framework.EnhancedErrorProvider errorProvider;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    }
}