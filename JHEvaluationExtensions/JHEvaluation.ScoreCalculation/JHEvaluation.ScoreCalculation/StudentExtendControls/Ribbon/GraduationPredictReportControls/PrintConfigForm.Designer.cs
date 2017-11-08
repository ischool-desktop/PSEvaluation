namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReportControls
{
    partial class PrintConfigForm
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
            this.gp1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.lnkView = new System.Windows.Forms.LinkLabel();
            this.lnkRemove = new System.Windows.Forms.LinkLabel();
            this.lnkUpload = new System.Windows.Forms.LinkLabel();
            this.lnkDefault = new System.Windows.Forms.LinkLabel();
            this.gp2 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.dtDocDate = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.cboAddresseeName = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cboAddresseeAddress = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.chkExportStuentList = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.gp1.SuspendLayout();
            this.gp2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtDocDate)).BeginInit();
            this.SuspendLayout();
            // 
            // gp1
            // 
            this.gp1.BackColor = System.Drawing.Color.Transparent;
            this.gp1.CanvasColor = System.Drawing.SystemColors.Control;
            this.gp1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gp1.Controls.Add(this.lnkView);
            this.gp1.Controls.Add(this.lnkRemove);
            this.gp1.Controls.Add(this.lnkUpload);
            this.gp1.Controls.Add(this.lnkDefault);
            this.gp1.Location = new System.Drawing.Point(12, 12);
            this.gp1.Name = "gp1";
            this.gp1.Size = new System.Drawing.Size(370, 70);
            // 
            // 
            // 
            this.gp1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gp1.Style.BackColorGradientAngle = 90;
            this.gp1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gp1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gp1.Style.BorderBottomWidth = 1;
            this.gp1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gp1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gp1.Style.BorderLeftWidth = 1;
            this.gp1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gp1.Style.BorderRightWidth = 1;
            this.gp1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gp1.Style.BorderTopWidth = 1;
            this.gp1.Style.Class = "";
            this.gp1.Style.CornerDiameter = 4;
            this.gp1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gp1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gp1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gp1.StyleMouseDown.Class = "";
            this.gp1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gp1.StyleMouseOver.Class = "";
            this.gp1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gp1.TabIndex = 0;
            this.gp1.Text = "自訂範本";
            // 
            // lnkView
            // 
            this.lnkView.AutoSize = true;
            this.lnkView.Location = new System.Drawing.Point(262, 15);
            this.lnkView.Name = "lnkView";
            this.lnkView.Size = new System.Drawing.Size(86, 17);
            this.lnkView.TabIndex = 3;
            this.lnkView.TabStop = true;
            this.lnkView.Text = "合併欄位總表";
            this.lnkView.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkView_LinkClicked);
            // 
            // lnkRemove
            // 
            this.lnkRemove.AutoSize = true;
            this.lnkRemove.Location = new System.Drawing.Point(170, 15);
            this.lnkRemove.Name = "lnkRemove";
            this.lnkRemove.Size = new System.Drawing.Size(86, 17);
            this.lnkRemove.TabIndex = 2;
            this.lnkRemove.TabStop = true;
            this.lnkRemove.Text = "移除自訂範本";
            this.lnkRemove.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRemove_LinkClicked);
            // 
            // lnkUpload
            // 
            this.lnkUpload.AutoSize = true;
            this.lnkUpload.Location = new System.Drawing.Point(104, 15);
            this.lnkUpload.Name = "lnkUpload";
            this.lnkUpload.Size = new System.Drawing.Size(60, 17);
            this.lnkUpload.TabIndex = 1;
            this.lnkUpload.TabStop = true;
            this.lnkUpload.Text = "上傳範本";
            this.lnkUpload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUpload_LinkClicked);
            // 
            // lnkDefault
            // 
            this.lnkDefault.AutoSize = true;
            this.lnkDefault.Location = new System.Drawing.Point(12, 15);
            this.lnkDefault.Name = "lnkDefault";
            this.lnkDefault.Size = new System.Drawing.Size(86, 17);
            this.lnkDefault.TabIndex = 0;
            this.lnkDefault.TabStop = true;
            this.lnkDefault.Text = "檢視目前範本";
            this.lnkDefault.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDefault_LinkClicked);
            // 
            // gp2
            // 
            this.gp2.BackColor = System.Drawing.Color.Transparent;
            this.gp2.CanvasColor = System.Drawing.SystemColors.Control;
            this.gp2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gp2.Controls.Add(this.dtDocDate);
            this.gp2.Controls.Add(this.labelX3);
            this.gp2.Controls.Add(this.cboAddresseeName);
            this.gp2.Controls.Add(this.labelX2);
            this.gp2.Controls.Add(this.cboAddresseeAddress);
            this.gp2.Controls.Add(this.labelX1);
            this.gp2.Location = new System.Drawing.Point(12, 97);
            this.gp2.Name = "gp2";
            this.gp2.Size = new System.Drawing.Size(370, 149);
            // 
            // 
            // 
            this.gp2.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gp2.Style.BackColorGradientAngle = 90;
            this.gp2.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gp2.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gp2.Style.BorderBottomWidth = 1;
            this.gp2.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gp2.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gp2.Style.BorderLeftWidth = 1;
            this.gp2.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gp2.Style.BorderRightWidth = 1;
            this.gp2.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gp2.Style.BorderTopWidth = 1;
            this.gp2.Style.Class = "";
            this.gp2.Style.CornerDiameter = 4;
            this.gp2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gp2.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gp2.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gp2.StyleMouseDown.Class = "";
            this.gp2.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gp2.StyleMouseOver.Class = "";
            this.gp2.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gp2.TabIndex = 1;
            this.gp2.Text = "項目";
            // 
            // dtDocDate
            // 
            // 
            // 
            // 
            this.dtDocDate.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dtDocDate.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtDocDate.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dtDocDate.ButtonDropDown.Visible = true;
            this.dtDocDate.IsPopupCalendarOpen = false;
            this.dtDocDate.Location = new System.Drawing.Point(100, 85);
            // 
            // 
            // 
            this.dtDocDate.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtDocDate.MonthCalendar.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dtDocDate.MonthCalendar.BackgroundStyle.Class = "";
            this.dtDocDate.MonthCalendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtDocDate.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dtDocDate.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dtDocDate.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dtDocDate.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dtDocDate.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dtDocDate.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dtDocDate.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dtDocDate.MonthCalendar.CommandsBackgroundStyle.Class = "";
            this.dtDocDate.MonthCalendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtDocDate.MonthCalendar.DayNames = new string[] {
        "日",
        "一",
        "二",
        "三",
        "四",
        "五",
        "六"};
            this.dtDocDate.MonthCalendar.DisplayMonth = new System.DateTime(2011, 3, 1, 0, 0, 0, 0);
            this.dtDocDate.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.dtDocDate.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtDocDate.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dtDocDate.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dtDocDate.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dtDocDate.MonthCalendar.NavigationBackgroundStyle.Class = "";
            this.dtDocDate.MonthCalendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dtDocDate.MonthCalendar.TodayButtonVisible = true;
            this.dtDocDate.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.dtDocDate.Name = "dtDocDate";
            this.dtDocDate.Size = new System.Drawing.Size(112, 25);
            this.dtDocDate.TabIndex = 5;
            // 
            // labelX3
            // 
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(14, 85);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(75, 23);
            this.labelX3.TabIndex = 4;
            this.labelX3.Text = "發文日期";
            this.labelX3.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // cboAddresseeName
            // 
            this.cboAddresseeName.DisplayMember = "Text";
            this.cboAddresseeName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboAddresseeName.FormattingEnabled = true;
            this.cboAddresseeName.ItemHeight = 19;
            this.cboAddresseeName.Location = new System.Drawing.Point(100, 50);
            this.cboAddresseeName.Name = "cboAddresseeName";
            this.cboAddresseeName.Size = new System.Drawing.Size(113, 25);
            this.cboAddresseeName.TabIndex = 3;
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(14, 50);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "收件人姓名";
            this.labelX2.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // cboAddresseeAddress
            // 
            this.cboAddresseeAddress.DisplayMember = "Text";
            this.cboAddresseeAddress.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboAddresseeAddress.FormattingEnabled = true;
            this.cboAddresseeAddress.ItemHeight = 19;
            this.cboAddresseeAddress.Location = new System.Drawing.Point(100, 14);
            this.cboAddresseeAddress.Name = "cboAddresseeAddress";
            this.cboAddresseeAddress.Size = new System.Drawing.Size(113, 25);
            this.cboAddresseeAddress.TabIndex = 1;
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(14, 15);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "收件人地址";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(242, 258);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(67, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(315, 258);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(67, 23);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // chkExportStuentList
            // 
            this.chkExportStuentList.AutoSize = true;
            this.chkExportStuentList.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkExportStuentList.BackgroundStyle.Class = "";
            this.chkExportStuentList.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkExportStuentList.Location = new System.Drawing.Point(12, 260);
            this.chkExportStuentList.Name = "chkExportStuentList";
            this.chkExportStuentList.Size = new System.Drawing.Size(147, 21);
            this.chkExportStuentList.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkExportStuentList.TabIndex = 4;
            this.chkExportStuentList.Text = "另存學生清單(Excel)";
            // 
            // PrintConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 290);
            this.Controls.Add(this.chkExportStuentList);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gp2);
            this.Controls.Add(this.gp1);
            this.DoubleBuffered = true;
            this.Name = "PrintConfigForm";
            this.Text = "列印設定";
            this.Load += new System.EventHandler(this.PrintConfigForm_Load);
            this.gp1.ResumeLayout(false);
            this.gp1.PerformLayout();
            this.gp2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtDocDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.GroupPanel gp1;
        private System.Windows.Forms.LinkLabel lnkRemove;
        private System.Windows.Forms.LinkLabel lnkUpload;
        private System.Windows.Forms.LinkLabel lnkDefault;
        private DevComponents.DotNetBar.Controls.GroupPanel gp2;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dtDocDate;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboAddresseeName;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboAddresseeAddress;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkExportStuentList;
        private System.Windows.Forms.LinkLabel lnkView;
    }
}