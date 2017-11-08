namespace JHSchool.Evaluation.CourseExtendControls.Ribbon
{
    partial class AssessmentSetupManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssessmentSetupManager));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataview = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.expandableSplitter1 = new DevComponents.DotNetBar.ExpandableSplitter();
            this.buttonItem3 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem2 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem1 = new DevComponents.DotNetBar.ButtonItem();
            this.ipList = new DevComponents.DotNetBar.ItemPanel();
            this.loading = new System.Windows.Forms.PictureBox();
            this.btnAddNew = new DevComponents.DotNetBar.ButtonX();
            this.btnDelete = new DevComponents.DotNetBar.ButtonX();
            this.peTemplateName1 = new DevComponents.DotNetBar.PanelEx();
            this.lblIsDirty = new DevComponents.DotNetBar.LabelX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.buttonItem4 = new DevComponents.DotNetBar.ButtonItem();
            this.navigationPanePanel1 = new DevComponents.DotNetBar.NavigationPanePanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.npLeft = new DevComponents.DotNetBar.NavigationPane();
            this.panel1 = new DevComponents.DotNetBar.PanelEx();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtOEndTime = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtOStartTime = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtTStartTime = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtTEndTime = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.ExamID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Weight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UseScore = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.UseEffort = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.UseText = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.OpenTeacherAccess = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.StartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InputRequired = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataview)).BeginInit();
            this.ipList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loading)).BeginInit();
            this.peTemplateName1.SuspendLayout();
            this.navigationPanePanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.npLeft.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataview
            // 
            this.dataview.AllowUserToAddRows = false;
            this.dataview.AllowUserToResizeRows = false;
            this.dataview.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataview.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataview.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ExamID,
            this.Weight,
            this.UseScore,
            this.UseEffort,
            this.UseText,
            this.OpenTeacherAccess,
            this.StartTime,
            this.EndTime,
            this.InputRequired});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataview.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataview.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataview.Location = new System.Drawing.Point(159, 37);
            this.dataview.Name = "dataview";
            this.dataview.RowHeadersWidth = 25;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataview.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataview.RowTemplate.Height = 24;
            this.dataview.Size = new System.Drawing.Size(622, 286);
            this.dataview.TabIndex = 2;
            this.dataview.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataview_RowValidating);
            this.dataview.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataview_CellValidated);
            this.dataview.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataview_CellValidating);
            this.dataview.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataview_RowsAdded);
            this.dataview.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataview_CellEndEdit);
            this.dataview.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataview_DataError);
            // 
            // expandableSplitter1
            // 
            this.expandableSplitter1.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.expandableSplitter1.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandableSplitter1.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandableSplitter1.ExpandFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.expandableSplitter1.ExpandFillColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandableSplitter1.ExpandLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.expandableSplitter1.ExpandLineColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandableSplitter1.GripDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.expandableSplitter1.GripDarkColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandableSplitter1.GripLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.expandableSplitter1.GripLightColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.expandableSplitter1.HotBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(151)))), ((int)(((byte)(61)))));
            this.expandableSplitter1.HotBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(184)))), ((int)(((byte)(94)))));
            this.expandableSplitter1.HotBackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemPressedBackground2;
            this.expandableSplitter1.HotBackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemPressedBackground;
            this.expandableSplitter1.HotExpandFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.expandableSplitter1.HotExpandFillColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandableSplitter1.HotExpandLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.expandableSplitter1.HotExpandLineColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandableSplitter1.HotGripDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.expandableSplitter1.HotGripDarkColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandableSplitter1.HotGripLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.expandableSplitter1.HotGripLightColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.expandableSplitter1.Location = new System.Drawing.Point(156, 37);
            this.expandableSplitter1.Name = "expandableSplitter1";
            this.expandableSplitter1.Size = new System.Drawing.Size(3, 377);
            this.expandableSplitter1.Style = DevComponents.DotNetBar.eSplitterStyle.Office2007;
            this.expandableSplitter1.TabIndex = 12;
            this.expandableSplitter1.TabStop = false;
            // 
            // buttonItem3
            // 
            this.buttonItem3.Name = "buttonItem3";
            this.buttonItem3.OptionGroup = "TemplateItem";
            this.buttonItem3.Text = "國防通識";
            // 
            // buttonItem2
            // 
            this.buttonItem2.Name = "buttonItem2";
            this.buttonItem2.OptionGroup = "TemplateItem";
            this.buttonItem2.Text = "體育評量";
            // 
            // buttonItem1
            // 
            this.buttonItem1.Name = "buttonItem1";
            this.buttonItem1.OptionGroup = "TemplateItem";
            this.buttonItem1.Text = "一般科目<b><font color=\"#ED1C24\">(已修改)</font></b>";
            // 
            // ipList
            // 
            this.ipList.AutoScroll = true;
            // 
            // 
            // 
            this.ipList.BackgroundStyle.BackColor = System.Drawing.Color.White;
            this.ipList.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.ipList.BackgroundStyle.BorderBottomWidth = 1;
            this.ipList.BackgroundStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.ipList.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.ipList.BackgroundStyle.BorderLeftWidth = 1;
            this.ipList.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.ipList.BackgroundStyle.BorderRightWidth = 1;
            this.ipList.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.ipList.BackgroundStyle.BorderTopWidth = 1;
            this.ipList.BackgroundStyle.PaddingBottom = 1;
            this.ipList.BackgroundStyle.PaddingLeft = 1;
            this.ipList.BackgroundStyle.PaddingRight = 1;
            this.ipList.BackgroundStyle.PaddingTop = 1;
            this.ipList.Controls.Add(this.loading);
            this.ipList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ipList.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem1,
            this.buttonItem2,
            this.buttonItem3});
            this.ipList.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.ipList.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
            this.ipList.Location = new System.Drawing.Point(0, 0);
            this.ipList.Name = "ipList";
            this.ipList.Size = new System.Drawing.Size(154, 325);
            this.ipList.TabIndex = 0;
            this.ipList.Text = "itemPanel1";
            // 
            // loading
            // 
            this.loading.Image = global::JHSchool.Evaluation.Properties.Resources.loading;
            this.loading.Location = new System.Drawing.Point(61, 127);
            this.loading.Name = "loading";
            this.loading.Size = new System.Drawing.Size(32, 32);
            this.loading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.loading.TabIndex = 0;
            this.loading.TabStop = false;
            this.loading.Visible = false;
            // 
            // btnAddNew
            // 
            this.btnAddNew.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAddNew.Location = new System.Drawing.Point(6, 3);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(66, 23);
            this.btnAddNew.TabIndex = 1;
            this.btnAddNew.Text = "新增";
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDelete.Location = new System.Drawing.Point(78, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(71, 23);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "刪除";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // peTemplateName1
            // 
            this.peTemplateName1.CanvasColor = System.Drawing.SystemColors.Control;
            this.peTemplateName1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.peTemplateName1.Controls.Add(this.lblIsDirty);
            this.peTemplateName1.Controls.Add(this.btnSave);
            this.peTemplateName1.Dock = System.Windows.Forms.DockStyle.Top;
            this.peTemplateName1.Location = new System.Drawing.Point(156, 0);
            this.peTemplateName1.Name = "peTemplateName1";
            this.peTemplateName1.Size = new System.Drawing.Size(625, 37);
            this.peTemplateName1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.peTemplateName1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.peTemplateName1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.peTemplateName1.Style.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            this.peTemplateName1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.peTemplateName1.Style.GradientAngle = 90;
            this.peTemplateName1.Style.MarginLeft = 15;
            this.peTemplateName1.TabIndex = 1;
            this.peTemplateName1.Text = "一般科目";
            // 
            // lblIsDirty
            // 
            this.lblIsDirty.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblIsDirty.ForeColor = System.Drawing.Color.Red;
            this.lblIsDirty.Location = new System.Drawing.Point(485, 8);
            this.lblIsDirty.Name = "lblIsDirty";
            this.lblIsDirty.Size = new System.Drawing.Size(63, 23);
            this.lblIsDirty.TabIndex = 4;
            this.lblIsDirty.Text = "未儲存";
            this.lblIsDirty.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(549, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(67, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // buttonItem4
            // 
            this.buttonItem4.Checked = true;
            this.buttonItem4.Image = ((System.Drawing.Image)(resources.GetObject("buttonItem4.Image")));
            this.buttonItem4.ImageFixedSize = new System.Drawing.Size(16, 16);
            this.buttonItem4.Name = "buttonItem4";
            this.buttonItem4.OptionGroup = "navBar";
            this.buttonItem4.Text = "評量設定";
            // 
            // navigationPanePanel1
            // 
            this.navigationPanePanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.navigationPanePanel1.Controls.Add(this.ipList);
            this.navigationPanePanel1.Controls.Add(this.panel2);
            this.navigationPanePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationPanePanel1.Location = new System.Drawing.Point(1, 25);
            this.navigationPanePanel1.Name = "navigationPanePanel1";
            this.navigationPanePanel1.ParentItem = this.buttonItem4;
            this.navigationPanePanel1.Size = new System.Drawing.Size(154, 356);
            this.navigationPanePanel1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.navigationPanePanel1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.navigationPanePanel1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.navigationPanePanel1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.navigationPanePanel1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.navigationPanePanel1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.navigationPanePanel1.Style.GradientAngle = 90;
            this.navigationPanePanel1.TabIndex = 2;
            this.navigationPanePanel1.Text = "這是什麼？";
            this.navigationPanePanel1.Visible = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.btnAddNew);
            this.panel2.Controls.Add(this.btnDelete);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 325);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(154, 31);
            this.panel2.TabIndex = 14;
            // 
            // npLeft
            // 
            this.npLeft.BackColor = System.Drawing.Color.Transparent;
            this.npLeft.CanCollapse = true;
            this.npLeft.Controls.Add(this.navigationPanePanel1);
            this.npLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.npLeft.ItemPaddingBottom = 2;
            this.npLeft.ItemPaddingTop = 2;
            this.npLeft.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem4});
            this.npLeft.Location = new System.Drawing.Point(0, 0);
            this.npLeft.Name = "npLeft";
            this.npLeft.Padding = new System.Windows.Forms.Padding(1);
            this.npLeft.Size = new System.Drawing.Size(156, 414);
            this.npLeft.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.npLeft.TabIndex = 0;
            // 
            // 
            // 
            this.npLeft.TitlePanel.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.npLeft.TitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.npLeft.TitlePanel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.npLeft.TitlePanel.Location = new System.Drawing.Point(1, 1);
            this.npLeft.TitlePanel.Name = "panelTitle";
            this.npLeft.TitlePanel.Size = new System.Drawing.Size(154, 24);
            this.npLeft.TitlePanel.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.npLeft.TitlePanel.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.npLeft.TitlePanel.Style.Border = DevComponents.DotNetBar.eBorderType.RaisedInner;
            this.npLeft.TitlePanel.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.npLeft.TitlePanel.Style.BorderSide = DevComponents.DotNetBar.eBorderSide.Bottom;
            this.npLeft.TitlePanel.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.npLeft.TitlePanel.Style.GradientAngle = 90;
            this.npLeft.TitlePanel.Style.MarginLeft = 4;
            this.npLeft.TitlePanel.TabIndex = 0;
            this.npLeft.TitlePanel.Text = "評量設定";
            // 
            // panel1
            // 
            this.panel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.panel1.Controls.Add(this.labelX4);
            this.panel1.Controls.Add(this.labelX2);
            this.panel1.Controls.Add(this.labelX3);
            this.panel1.Controls.Add(this.txtTEndTime);
            this.panel1.Controls.Add(this.labelX1);
            this.panel1.Controls.Add(this.txtTStartTime);
            this.panel1.Controls.Add(this.txtOEndTime);
            this.panel1.Controls.Add(this.txtOStartTime);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(159, 323);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(622, 91);
            this.panel1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panel1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panel1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panel1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panel1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panel1.Style.GradientAngle = 90;
            this.panel1.TabIndex = 13;
            // 
            // labelX2
            // 
            this.labelX2.Location = new System.Drawing.Point(294, 16);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(16, 23);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "~";
            // 
            // labelX1
            // 
            this.labelX1.Location = new System.Drawing.Point(16, 16);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(117, 23);
            this.labelX1.TabIndex = 3;
            this.labelX1.Text = "平時評量繳交時間";
            // 
            // txtOEndTime
            // 
            // 
            // 
            // 
            this.txtOEndTime.Border.Class = "TextBoxBorder";
            this.txtOEndTime.Location = new System.Drawing.Point(316, 15);
            this.txtOEndTime.Name = "txtOEndTime";
            this.txtOEndTime.Size = new System.Drawing.Size(150, 25);
            this.txtOEndTime.TabIndex = 2;
            this.txtOEndTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtEndTime_Validating);
            // 
            // txtOStartTime
            // 
            // 
            // 
            // 
            this.txtOStartTime.Border.Class = "TextBoxBorder";
            this.txtOStartTime.Location = new System.Drawing.Point(136, 15);
            this.txtOStartTime.Name = "txtOStartTime";
            this.txtOStartTime.Size = new System.Drawing.Size(150, 25);
            this.txtOStartTime.TabIndex = 1;
            this.txtOStartTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtStartTime_Validating);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Weight";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn1.HeaderText = "比重";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 50;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "StartTime";
            this.dataGridViewTextBoxColumn2.HeaderText = "開始時間";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 70;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.DataPropertyName = "EndTime";
            this.dataGridViewTextBoxColumn3.HeaderText = "結束時間";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // txtTStartTime
            // 
            // 
            // 
            // 
            this.txtTStartTime.Border.Class = "TextBoxBorder";
            this.txtTStartTime.Location = new System.Drawing.Point(136, 51);
            this.txtTStartTime.Name = "txtTStartTime";
            this.txtTStartTime.Size = new System.Drawing.Size(150, 25);
            this.txtTStartTime.TabIndex = 1;
            this.txtTStartTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtStartTime_Validating);
            // 
            // txtTEndTime
            // 
            // 
            // 
            // 
            this.txtTEndTime.Border.Class = "TextBoxBorder";
            this.txtTEndTime.Location = new System.Drawing.Point(316, 51);
            this.txtTEndTime.Name = "txtTEndTime";
            this.txtTEndTime.Size = new System.Drawing.Size(150, 25);
            this.txtTEndTime.TabIndex = 2;
            this.txtTEndTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtEndTime_Validating);
            // 
            // labelX3
            // 
            this.labelX3.Location = new System.Drawing.Point(16, 52);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(117, 23);
            this.labelX3.TabIndex = 3;
            this.labelX3.Text = "文字評量繳交時間";
            // 
            // labelX4
            // 
            this.labelX4.Location = new System.Drawing.Point(294, 52);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(16, 23);
            this.labelX4.TabIndex = 3;
            this.labelX4.Text = "~";
            // 
            // ExamID
            // 
            this.ExamID.DataPropertyName = "ExamID";
            this.ExamID.DisplayStyleForCurrentCellOnly = true;
            this.ExamID.HeaderText = "評量名稱";
            this.ExamID.Name = "ExamID";
            this.ExamID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ExamID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ExamID.Width = 120;
            // 
            // Weight
            // 
            this.Weight.DataPropertyName = "Weight";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Weight.DefaultCellStyle = dataGridViewCellStyle2;
            this.Weight.HeaderText = "比重";
            this.Weight.Name = "Weight";
            this.Weight.Width = 50;
            // 
            // UseScore
            // 
            this.UseScore.DataPropertyName = "UseScore";
            this.UseScore.FalseValue = "否";
            this.UseScore.HeaderText = "分數評量";
            this.UseScore.Name = "UseScore";
            this.UseScore.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.UseScore.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.UseScore.TrueValue = "是";
            this.UseScore.Width = 75;
            // 
            // UseEffort
            // 
            this.UseEffort.HeaderText = "努力程度";
            this.UseEffort.Name = "UseEffort";
            this.UseEffort.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.UseEffort.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.UseEffort.Width = 75;
            // 
            // UseText
            // 
            this.UseText.DataPropertyName = "UseText";
            this.UseText.FalseValue = "否";
            this.UseText.HeaderText = "文字描述";
            this.UseText.Name = "UseText";
            this.UseText.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.UseText.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.UseText.TrueValue = "是";
            this.UseText.Visible = false;
            this.UseText.Width = 75;
            // 
            // OpenTeacherAccess
            // 
            this.OpenTeacherAccess.DataPropertyName = "OpenTeacherAccess";
            this.OpenTeacherAccess.FalseValue = "否";
            this.OpenTeacherAccess.HeaderText = "開放繳交";
            this.OpenTeacherAccess.Name = "OpenTeacherAccess";
            this.OpenTeacherAccess.TrueValue = "是";
            this.OpenTeacherAccess.Visible = false;
            this.OpenTeacherAccess.Width = 75;
            // 
            // StartTime
            // 
            this.StartTime.DataPropertyName = "StartTime";
            this.StartTime.HeaderText = "開始時間";
            this.StartTime.Name = "StartTime";
            this.StartTime.Width = 110;
            // 
            // EndTime
            // 
            this.EndTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.EndTime.DataPropertyName = "EndTime";
            this.EndTime.HeaderText = "結束時間";
            this.EndTime.Name = "EndTime";
            // 
            // InputRequired
            // 
            this.InputRequired.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.InputRequired.DataPropertyName = "InputRequired";
            this.InputRequired.FalseValue = "是";
            this.InputRequired.HeaderText = "不強制繳交成績";
            this.InputRequired.Name = "InputRequired";
            this.InputRequired.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.InputRequired.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.InputRequired.TrueValue = "否";
            this.InputRequired.Visible = false;
            // 
            // AssessmentSetupManager
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(781, 414);
            this.Controls.Add(this.dataview);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.expandableSplitter1);
            this.Controls.Add(this.peTemplateName1);
            this.Controls.Add(this.npLeft);
            this.MinimumSize = new System.Drawing.Size(600, 296);
            this.Name = "AssessmentSetupManager";
            this.Text = "評量設定";
            this.Load += new System.EventHandler(this.AssessmentSetupManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataview)).EndInit();
            this.ipList.ResumeLayout(false);
            this.ipList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loading)).EndInit();
            this.peTemplateName1.ResumeLayout(false);
            this.navigationPanePanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.npLeft.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dataview;
        private DevComponents.DotNetBar.ExpandableSplitter expandableSplitter1;
        private DevComponents.DotNetBar.ButtonItem buttonItem3;
        private DevComponents.DotNetBar.ButtonItem buttonItem2;
        private DevComponents.DotNetBar.ButtonItem buttonItem1;
        private DevComponents.DotNetBar.ItemPanel ipList;
        private DevComponents.DotNetBar.ButtonX btnAddNew;
        private DevComponents.DotNetBar.ButtonX btnDelete;
        private DevComponents.DotNetBar.PanelEx peTemplateName1;
        private DevComponents.DotNetBar.ButtonItem buttonItem4;
        private DevComponents.DotNetBar.NavigationPanePanel navigationPanePanel1;
        private DevComponents.DotNetBar.NavigationPane npLeft;
        private System.Windows.Forms.PictureBox loading;
        private DevComponents.DotNetBar.PanelEx panel1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DevComponents.DotNetBar.LabelX lblIsDirty;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtOEndTime;
        private DevComponents.DotNetBar.Controls.TextBoxX txtOStartTime;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTEndTime;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTStartTime;
        private System.Windows.Forms.DataGridViewComboBoxColumn ExamID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Weight;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UseScore;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UseEffort;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UseText;
        private System.Windows.Forms.DataGridViewCheckBoxColumn OpenTeacherAccess;
        private System.Windows.Forms.DataGridViewTextBoxColumn StartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndTime;
        private System.Windows.Forms.DataGridViewCheckBoxColumn InputRequired;

    }
}