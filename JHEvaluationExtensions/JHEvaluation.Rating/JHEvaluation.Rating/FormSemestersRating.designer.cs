namespace JHEvaluation.Rating
{
    partial class FormSemestersRating
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.chCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.chScoreItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRank = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chk3Down = new System.Windows.Forms.CheckBox();
            this.chk3Up = new System.Windows.Forms.CheckBox();
            this.chk2Down = new System.Windows.Forms.CheckBox();
            this.chk2Up = new System.Windows.Forms.CheckBox();
            this.chk1Down = new System.Windows.Forms.CheckBox();
            this.chk1Up = new System.Windows.Forms.CheckBox();
            this.panelEx2 = new DevComponents.DotNetBar.PanelEx();
            this.txtLastPercentage = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label3 = new System.Windows.Forms.Label();
            this.rbLastPercentage = new System.Windows.Forms.RadioButton();
            this.txtLastRank = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label2 = new System.Windows.Forms.Label();
            this.rbLastRank = new System.Windows.Forms.RadioButton();
            this.txtPercentage = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.rbPercentage = new System.Windows.Forms.RadioButton();
            this.txtTopRank = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.lblTopRank = new System.Windows.Forms.Label();
            this.rbTopRank = new System.Windows.Forms.RadioButton();
            this.rbAllRank = new System.Windows.Forms.RadioButton();
            this.rbGradeYear = new System.Windows.Forms.RadioButton();
            this.rbClass = new System.Windows.Forms.RadioButton();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSelectedCount = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.groupPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panelEx2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chCheck,
            this.chScoreItem});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgv.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgv.Location = new System.Drawing.Point(7, 12);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(323, 304);
            this.dgv.TabIndex = 0;
            this.dgv.Tag = "StatusVarying";
            // 
            // chCheck
            // 
            this.chCheck.HeaderText = "";
            this.chCheck.Name = "chCheck";
            this.chCheck.Width = 30;
            // 
            // chScoreItem
            // 
            this.chScoreItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.chScoreItem.DefaultCellStyle = dataGridViewCellStyle2;
            this.chScoreItem.HeaderText = "名稱";
            this.chScoreItem.Name = "chScoreItem";
            this.chScoreItem.ReadOnly = true;
            // 
            // btnRank
            // 
            this.btnRank.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRank.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRank.BackColor = System.Drawing.Color.Transparent;
            this.btnRank.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRank.Location = new System.Drawing.Point(499, 331);
            this.btnRank.Name = "btnRank";
            this.btnRank.Size = new System.Drawing.Size(75, 23);
            this.btnRank.TabIndex = 2;
            this.btnRank.Tag = "StatusVarying";
            this.btnRank.Text = "排名";
            this.btnRank.Click += new System.EventHandler(this.btnRank_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(581, 331);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "離開";
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.panel3);
            this.groupPanel1.Location = new System.Drawing.Point(336, 6);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(323, 313);
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
            this.groupPanel1.Style.Class = "";
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.Class = "";
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.Class = "";
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 3;
            this.groupPanel1.Tag = "StatusVarying";
            this.groupPanel1.Text = "排名選項";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.chk3Down);
            this.panel3.Controls.Add(this.chk3Up);
            this.panel3.Controls.Add(this.chk2Down);
            this.panel3.Controls.Add(this.chk2Up);
            this.panel3.Controls.Add(this.chk1Down);
            this.panel3.Controls.Add(this.chk1Up);
            this.panel3.Controls.Add(this.panelEx2);
            this.panel3.Controls.Add(this.rbGradeYear);
            this.panel3.Controls.Add(this.rbClass);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(317, 286);
            this.panel3.TabIndex = 0;
            // 
            // chk3Down
            // 
            this.chk3Down.AutoSize = true;
            this.chk3Down.Location = new System.Drawing.Point(126, 55);
            this.chk3Down.Name = "chk3Down";
            this.chk3Down.Size = new System.Drawing.Size(53, 21);
            this.chk3Down.TabIndex = 21;
            this.chk3Down.Text = "三下";
            this.chk3Down.UseVisualStyleBackColor = true;
            // 
            // chk3Up
            // 
            this.chk3Up.AutoSize = true;
            this.chk3Up.Location = new System.Drawing.Point(126, 22);
            this.chk3Up.Name = "chk3Up";
            this.chk3Up.Size = new System.Drawing.Size(53, 21);
            this.chk3Up.TabIndex = 20;
            this.chk3Up.Text = "三上";
            this.chk3Up.UseVisualStyleBackColor = true;
            // 
            // chk2Down
            // 
            this.chk2Down.AutoSize = true;
            this.chk2Down.Location = new System.Drawing.Point(71, 55);
            this.chk2Down.Name = "chk2Down";
            this.chk2Down.Size = new System.Drawing.Size(53, 21);
            this.chk2Down.TabIndex = 16;
            this.chk2Down.Text = "二下";
            this.chk2Down.UseVisualStyleBackColor = true;
            // 
            // chk2Up
            // 
            this.chk2Up.AutoSize = true;
            this.chk2Up.Location = new System.Drawing.Point(71, 22);
            this.chk2Up.Name = "chk2Up";
            this.chk2Up.Size = new System.Drawing.Size(53, 21);
            this.chk2Up.TabIndex = 19;
            this.chk2Up.Text = "二上";
            this.chk2Up.UseVisualStyleBackColor = true;
            // 
            // chk1Down
            // 
            this.chk1Down.AutoSize = true;
            this.chk1Down.Location = new System.Drawing.Point(16, 55);
            this.chk1Down.Name = "chk1Down";
            this.chk1Down.Size = new System.Drawing.Size(53, 21);
            this.chk1Down.TabIndex = 18;
            this.chk1Down.Text = "一下";
            this.chk1Down.UseVisualStyleBackColor = true;
            // 
            // chk1Up
            // 
            this.chk1Up.AutoSize = true;
            this.chk1Up.Location = new System.Drawing.Point(16, 22);
            this.chk1Up.Name = "chk1Up";
            this.chk1Up.Size = new System.Drawing.Size(53, 21);
            this.chk1Up.TabIndex = 17;
            this.chk1Up.Text = "一上";
            this.chk1Up.UseVisualStyleBackColor = true;
            // 
            // panelEx2
            // 
            this.panelEx2.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.panelEx2.Controls.Add(this.txtLastPercentage);
            this.panelEx2.Controls.Add(this.label3);
            this.panelEx2.Controls.Add(this.rbLastPercentage);
            this.panelEx2.Controls.Add(this.txtLastRank);
            this.panelEx2.Controls.Add(this.label2);
            this.panelEx2.Controls.Add(this.rbLastRank);
            this.panelEx2.Controls.Add(this.txtPercentage);
            this.panelEx2.Controls.Add(this.lblPercentage);
            this.panelEx2.Controls.Add(this.rbPercentage);
            this.panelEx2.Controls.Add(this.txtTopRank);
            this.panelEx2.Controls.Add(this.lblTopRank);
            this.panelEx2.Controls.Add(this.rbTopRank);
            this.panelEx2.Controls.Add(this.rbAllRank);
            this.panelEx2.Location = new System.Drawing.Point(13, 138);
            this.panelEx2.Name = "panelEx2";
            this.panelEx2.Size = new System.Drawing.Size(290, 140);
            this.panelEx2.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx2.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx2.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx2.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx2.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx2.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx2.Style.GradientAngle = 90;
            this.panelEx2.TabIndex = 15;
            // 
            // txtLastPercentage
            // 
            // 
            // 
            // 
            this.txtLastPercentage.Border.Class = "TextBoxBorder";
            this.txtLastPercentage.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtLastPercentage.Location = new System.Drawing.Point(135, 101);
            this.txtLastPercentage.Name = "txtLastPercentage";
            this.txtLastPercentage.Size = new System.Drawing.Size(45, 25);
            this.txtLastPercentage.TabIndex = 12;
            this.txtLastPercentage.TextChanged += new System.EventHandler(this.txtLastPercentage_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(181, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "的學生";
            // 
            // rbLastPercentage
            // 
            this.rbLastPercentage.AutoSize = true;
            this.rbLastPercentage.Location = new System.Drawing.Point(19, 103);
            this.rbLastPercentage.Name = "rbLastPercentage";
            this.rbLastPercentage.Size = new System.Drawing.Size(117, 21);
            this.rbLastPercentage.TabIndex = 10;
            this.rbLastPercentage.Text = "名次於後百分之";
            this.rbLastPercentage.UseVisualStyleBackColor = true;
            // 
            // txtLastRank
            // 
            // 
            // 
            // 
            this.txtLastRank.Border.Class = "TextBoxBorder";
            this.txtLastRank.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtLastRank.Location = new System.Drawing.Point(187, 39);
            this.txtLastRank.Name = "txtLastRank";
            this.txtLastRank.Size = new System.Drawing.Size(45, 25);
            this.txtLastRank.TabIndex = 9;
            this.txtLastRank.TextChanged += new System.EventHandler(this.txtLastRank_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(236, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "名";
            // 
            // rbLastRank
            // 
            this.rbLastRank.AutoSize = true;
            this.rbLastRank.Location = new System.Drawing.Point(147, 41);
            this.rbLastRank.Name = "rbLastRank";
            this.rbLastRank.Size = new System.Drawing.Size(39, 21);
            this.rbLastRank.TabIndex = 7;
            this.rbLastRank.Text = "後";
            this.rbLastRank.UseVisualStyleBackColor = true;
            // 
            // txtPercentage
            // 
            // 
            // 
            // 
            this.txtPercentage.Border.Class = "TextBoxBorder";
            this.txtPercentage.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtPercentage.Location = new System.Drawing.Point(135, 70);
            this.txtPercentage.Name = "txtPercentage";
            this.txtPercentage.Size = new System.Drawing.Size(45, 25);
            this.txtPercentage.TabIndex = 6;
            this.txtPercentage.TextChanged += new System.EventHandler(this.txtPercentage_TextChanged);
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Location = new System.Drawing.Point(181, 74);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(47, 17);
            this.lblPercentage.TabIndex = 5;
            this.lblPercentage.Text = "的學生";
            // 
            // rbPercentage
            // 
            this.rbPercentage.AutoSize = true;
            this.rbPercentage.Location = new System.Drawing.Point(19, 72);
            this.rbPercentage.Name = "rbPercentage";
            this.rbPercentage.Size = new System.Drawing.Size(117, 21);
            this.rbPercentage.TabIndex = 4;
            this.rbPercentage.Text = "名次於前百分之";
            this.rbPercentage.UseVisualStyleBackColor = true;
            // 
            // txtTopRank
            // 
            // 
            // 
            // 
            this.txtTopRank.Border.Class = "TextBoxBorder";
            this.txtTopRank.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtTopRank.Location = new System.Drawing.Point(59, 39);
            this.txtTopRank.Name = "txtTopRank";
            this.txtTopRank.Size = new System.Drawing.Size(45, 25);
            this.txtTopRank.TabIndex = 3;
            this.txtTopRank.TextChanged += new System.EventHandler(this.txtTopRank_TextChanged);
            // 
            // lblTopRank
            // 
            this.lblTopRank.AutoSize = true;
            this.lblTopRank.Location = new System.Drawing.Point(108, 43);
            this.lblTopRank.Name = "lblTopRank";
            this.lblTopRank.Size = new System.Drawing.Size(21, 17);
            this.lblTopRank.TabIndex = 2;
            this.lblTopRank.Text = "名";
            // 
            // rbTopRank
            // 
            this.rbTopRank.AutoSize = true;
            this.rbTopRank.Location = new System.Drawing.Point(19, 41);
            this.rbTopRank.Name = "rbTopRank";
            this.rbTopRank.Size = new System.Drawing.Size(39, 21);
            this.rbTopRank.TabIndex = 1;
            this.rbTopRank.Text = "前";
            this.rbTopRank.UseVisualStyleBackColor = true;
            // 
            // rbAllRank
            // 
            this.rbAllRank.AutoSize = true;
            this.rbAllRank.Checked = true;
            this.rbAllRank.Location = new System.Drawing.Point(19, 13);
            this.rbAllRank.Name = "rbAllRank";
            this.rbAllRank.Size = new System.Drawing.Size(78, 21);
            this.rbAllRank.TabIndex = 1;
            this.rbAllRank.TabStop = true;
            this.rbAllRank.Text = "所有名次";
            this.rbAllRank.UseVisualStyleBackColor = true;
            // 
            // rbGradeYear
            // 
            this.rbGradeYear.AutoSize = true;
            this.rbGradeYear.Location = new System.Drawing.Point(101, 105);
            this.rbGradeYear.Name = "rbGradeYear";
            this.rbGradeYear.Size = new System.Drawing.Size(78, 21);
            this.rbGradeYear.TabIndex = 12;
            this.rbGradeYear.Text = "年級排名";
            this.rbGradeYear.UseVisualStyleBackColor = true;
            // 
            // rbClass
            // 
            this.rbClass.AutoSize = true;
            this.rbClass.Checked = true;
            this.rbClass.Location = new System.Drawing.Point(14, 105);
            this.rbClass.Name = "rbClass";
            this.rbClass.Size = new System.Drawing.Size(78, 21);
            this.rbClass.TabIndex = 13;
            this.rbClass.TabStop = true;
            this.rbClass.Text = "班級排名";
            this.rbClass.UseVisualStyleBackColor = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn1.HeaderText = "領域";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn2.HeaderText = "權數";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn3.HeaderText = "修習人數";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 75;
            // 
            // lblSelectedCount
            // 
            this.lblSelectedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelectedCount.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectedCount.Location = new System.Drawing.Point(115, 331);
            this.lblSelectedCount.Name = "lblSelectedCount";
            this.lblSelectedCount.Size = new System.Drawing.Size(211, 21);
            this.lblSelectedCount.TabIndex = 4;
            this.lblSelectedCount.Text = "已選擇人數：000";
            this.lblSelectedCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(4, 331);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "※ 藍色字體為領域。";
            // 
            // FormSemestersRating
            // 
            this.AcceptButton = this.btnRank;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(665, 366);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSelectedCount);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRank);
            this.Controls.Add(this.dgv);
            this.Name = "FormSemestersRating";
            this.Tag = "StatusVarying";
            this.Text = "學期成績排名(多學期)";
            this.Load += new System.EventHandler(this.FormSemestersSubject_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.groupPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panelEx2.ResumeLayout(false);
            this.panelEx2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgv;
        private DevComponents.DotNetBar.ButtonX btnRank;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.Label lblSelectedCount;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton rbGradeYear;
        private System.Windows.Forms.RadioButton rbClass;
        private DevComponents.DotNetBar.PanelEx panelEx2;
        private DevComponents.DotNetBar.Controls.TextBoxX txtPercentage;
        private System.Windows.Forms.Label lblPercentage;
        private System.Windows.Forms.RadioButton rbPercentage;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTopRank;
        private System.Windows.Forms.Label lblTopRank;
        private System.Windows.Forms.RadioButton rbTopRank;
        private System.Windows.Forms.RadioButton rbAllRank;
        private System.Windows.Forms.CheckBox chk3Down;
        private System.Windows.Forms.CheckBox chk3Up;
        private System.Windows.Forms.CheckBox chk2Down;
        private System.Windows.Forms.CheckBox chk2Up;
        private System.Windows.Forms.CheckBox chk1Down;
        private System.Windows.Forms.CheckBox chk1Up;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn chScoreItem;
        private System.Windows.Forms.Label label1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtLastRank;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbLastRank;
        private DevComponents.DotNetBar.Controls.TextBoxX txtLastPercentage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbLastPercentage;
    }
}