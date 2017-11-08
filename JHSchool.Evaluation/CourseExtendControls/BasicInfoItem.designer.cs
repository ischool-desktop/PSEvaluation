using Framework;
using System;
namespace JHSchool.Evaluation.CourseExtendControls
{
    partial class BasicInfoItem
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
                //Teacher.Instance.TeacherDataChanged -= new EventHandler<TeacherDataChangedEventArgs>(Instance_TeacherDataChanged);
                //Teacher.Instance.TeacherInserted -= new EventHandler(Instance_TeacherInserted);
                //Teacher.Instance.TeacherDeleted -= new EventHandler<TeacherDeletedEventArgs>(Instance_TeacherDeleted);
                //ClassEntity.Instance.ClassInserted-= new EventHandler<InsertClassEventArgs>(Instance_ClassInserted);
                //ClassEntity.Instance.ClassUpdated -= new EventHandler<UpdateClassEventArgs>(Instance_ClassUpdated);
                //ClassEntity.Instance.ClassDeleted -= new EventHandler<DeleteClassEventArgs>(Instance_ClassDeleted);
                //CourseEntity.Instance.ForeignTableChanged -= new EventHandler(Instance_ForeignTableChanged);
                //CourseEntity.Instance.CourseChanged -= new EventHandler<CourseChangeEventArgs>(Instance_CourseChanged);
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtCourseName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtSubject = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.labelX7 = new DevComponents.DotNetBar.LabelX();
            this.cboClass = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem9 = new DevComponents.Editors.ComboItem();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem10 = new DevComponents.Editors.ComboItem();
            this.comboItem7 = new DevComponents.Editors.ComboItem();
            this.comboItem8 = new DevComponents.Editors.ComboItem();
            this.txtPeriodCredit = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.btnTeachers = new DevComponents.DotNetBar.ButtonX();
            this.btnTeacher1 = new DevComponents.DotNetBar.ButtonItem();
            this.btnTeacher2 = new DevComponents.DotNetBar.ButtonItem();
            this.btnTeacher3 = new DevComponents.DotNetBar.ButtonItem();
            this.cboMultiTeacher = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX14 = new DevComponents.DotNetBar.LabelX();
            this.cboDomain = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.picWaiting)).BeginInit();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(36, 20);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(70, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "課程名稱";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // txtCourseName
            // 
            // 
            // 
            // 
            this.txtCourseName.Border.Class = "TextBoxBorder";
            this.txtCourseName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtCourseName.Location = new System.Drawing.Point(113, 19);
            this.txtCourseName.MaxLength = 50;
            this.txtCourseName.Name = "txtCourseName";
            this.txtCourseName.Size = new System.Drawing.Size(151, 25);
            this.txtCourseName.TabIndex = 1;
            this.txtCourseName.TextChanged += new System.EventHandler(this.txtCourseName_TextChanged);
            // 
            // txtSubject
            // 
            // 
            // 
            // 
            this.txtSubject.Border.Class = "TextBoxBorder";
            this.txtSubject.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtSubject.Location = new System.Drawing.Point(113, 48);
            this.txtSubject.MaxLength = 50;
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(151, 25);
            this.txtSubject.TabIndex = 3;
            this.txtSubject.TextChanged += new System.EventHandler(this.txtSubject_TextChanged);
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(36, 49);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(70, 23);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "科目名稱";
            this.labelX2.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX4
            // 
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(290, 20);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(70, 23);
            this.labelX4.TabIndex = 12;
            this.labelX4.Text = "所屬班級";
            this.labelX4.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX5
            // 
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.Class = "";
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Location = new System.Drawing.Point(290, 49);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(70, 23);
            this.labelX5.TabIndex = 14;
            this.labelX5.Text = "學 年 度";
            this.labelX5.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX6
            // 
            // 
            // 
            // 
            this.labelX6.BackgroundStyle.Class = "";
            this.labelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX6.Location = new System.Drawing.Point(290, 78);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(70, 23);
            this.labelX6.TabIndex = 16;
            this.labelX6.Text = "學         期";
            this.labelX6.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX7
            // 
            // 
            // 
            // 
            this.labelX7.BackgroundStyle.Class = "";
            this.labelX7.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX7.Location = new System.Drawing.Point(36, 78);
            this.labelX7.Name = "labelX7";
            this.labelX7.Size = new System.Drawing.Size(70, 23);
            this.labelX7.TabIndex = 18;
            this.labelX7.Text = "節數/權數";
            this.labelX7.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // cboClass
            // 
            this.cboClass.DisplayMember = "Text";
            this.cboClass.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboClass.FormattingEnabled = true;
            this.cboClass.ItemHeight = 19;
            this.cboClass.Location = new System.Drawing.Point(364, 19);
            this.cboClass.MaxDropDownItems = 6;
            this.cboClass.Name = "cboClass";
            this.cboClass.Size = new System.Drawing.Size(151, 25);
            this.cboClass.TabIndex = 13;
            this.cboClass.Tag = "ForceValidate";
            this.cboClass.SelectedIndexChanged += new System.EventHandler(this.cboClass_SelectedIndexChanged);
            this.cboClass.TextChanged += new System.EventHandler(this.cboClass_TextChanged);
            this.cboClass.Validating += new System.ComponentModel.CancelEventHandler(this.ComboBoxItem_Validating);
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Items.AddRange(new object[] {
            this.comboItem9});
            this.cboSchoolYear.Location = new System.Drawing.Point(364, 48);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(151, 25);
            this.cboSchoolYear.TabIndex = 15;
            this.cboSchoolYear.Tag = "";
            this.cboSchoolYear.TextChanged += new System.EventHandler(this.cboSchoolYear_TextChanged);
            this.cboSchoolYear.Validating += new System.ComponentModel.CancelEventHandler(this.ComboBoxItem_Validating);
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Items.AddRange(new object[] {
            this.comboItem10,
            this.comboItem7,
            this.comboItem8});
            this.cboSemester.Location = new System.Drawing.Point(364, 77);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(151, 25);
            this.cboSemester.TabIndex = 17;
            this.cboSemester.Tag = "";
            this.cboSemester.TextChanged += new System.EventHandler(this.cboSemester_TextChanged);
            this.cboSemester.Validating += new System.ComponentModel.CancelEventHandler(this.ComboBoxItem_Validating);
            // 
            // comboItem7
            // 
            this.comboItem7.Text = "1";
            // 
            // comboItem8
            // 
            this.comboItem8.Text = "2";
            // 
            // txtPeriodCredit
            // 
            // 
            // 
            // 
            this.txtPeriodCredit.Border.Class = "TextBoxBorder";
            this.txtPeriodCredit.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtPeriodCredit.Location = new System.Drawing.Point(113, 77);
            this.txtPeriodCredit.Name = "txtPeriodCredit";
            this.txtPeriodCredit.Size = new System.Drawing.Size(151, 25);
            this.txtPeriodCredit.TabIndex = 19;
            this.txtPeriodCredit.TextChanged += new System.EventHandler(this.txtCredit_TextChanged);
            // 
            // btnTeachers
            // 
            this.btnTeachers.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnTeachers.AutoExpandOnClick = true;
            this.btnTeachers.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnTeachers.Location = new System.Drawing.Point(271, 109);
            this.btnTeachers.Margin = new System.Windows.Forms.Padding(4);
            this.btnTeachers.Name = "btnTeachers";
            this.btnTeachers.Size = new System.Drawing.Size(89, 22);
            this.btnTeachers.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.btnTeacher1,
            this.btnTeacher2,
            this.btnTeacher3});
            this.btnTeachers.TabIndex = 10;
            // 
            // btnTeacher1
            // 
            this.btnTeacher1.GlobalItem = false;
            this.btnTeacher1.Name = "btnTeacher1";
            this.btnTeacher1.Text = "教師一";
            // 
            // btnTeacher2
            // 
            this.btnTeacher2.GlobalItem = false;
            this.btnTeacher2.Name = "btnTeacher2";
            this.btnTeacher2.Text = "教師二";
            // 
            // btnTeacher3
            // 
            this.btnTeacher3.GlobalItem = false;
            this.btnTeacher3.Name = "btnTeacher3";
            this.btnTeacher3.Text = "教師三";
            // 
            // cboMultiTeacher
            // 
            this.cboMultiTeacher.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cboMultiTeacher.DisplayMember = "Text";
            this.cboMultiTeacher.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboMultiTeacher.FormattingEnabled = true;
            this.cboMultiTeacher.ItemHeight = 19;
            this.cboMultiTeacher.Location = new System.Drawing.Point(364, 108);
            this.cboMultiTeacher.MaxDropDownItems = 6;
            this.cboMultiTeacher.Name = "cboMultiTeacher";
            this.cboMultiTeacher.Size = new System.Drawing.Size(151, 25);
            this.cboMultiTeacher.TabIndex = 11;
            this.cboMultiTeacher.Tag = "ForceValidate";
            this.cboMultiTeacher.TextChanged += new System.EventHandler(this.cboMultiTeacher_TextChanged);
            this.cboMultiTeacher.Validating += new System.ComponentModel.CancelEventHandler(this.ComboBoxItem_Validating);
            // 
            // labelX14
            // 
            // 
            // 
            // 
            this.labelX14.BackgroundStyle.Class = "";
            this.labelX14.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX14.Location = new System.Drawing.Point(36, 108);
            this.labelX14.Name = "labelX14";
            this.labelX14.Size = new System.Drawing.Size(70, 23);
            this.labelX14.TabIndex = 18;
            this.labelX14.Text = "領         域";
            this.labelX14.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // cboDomain
            // 
            this.cboDomain.DisplayMember = "Text";
            this.cboDomain.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboDomain.FormattingEnabled = true;
            this.cboDomain.ItemHeight = 19;
            this.cboDomain.Location = new System.Drawing.Point(113, 108);
            this.cboDomain.Name = "cboDomain";
            this.cboDomain.Size = new System.Drawing.Size(151, 25);
            this.cboDomain.TabIndex = 26;
            this.cboDomain.Tag = "ForceValidate";
            this.cboDomain.TextChanged += new System.EventHandler(this.cboDomain_TextChanged);
            this.cboDomain.Validating += new System.ComponentModel.CancelEventHandler(this.ComboBoxItem_Validating);
            // 
            // BasicInfoItem
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.cboDomain);
            this.Controls.Add(this.btnTeachers);
            this.Controls.Add(this.cboClass);
            this.Controls.Add(this.cboSchoolYear);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.labelX7);
            this.Controls.Add(this.labelX14);
            this.Controls.Add(this.labelX6);
            this.Controls.Add(this.labelX5);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.txtCourseName);
            this.Controls.Add(this.cboMultiTeacher);
            this.Controls.Add(this.txtSubject);
            this.Controls.Add(this.txtPeriodCredit);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(550, 0);
            this.Name = "BasicInfoItem";
            this.Size = new System.Drawing.Size(550, 155);
            this.Load += new System.EventHandler(this.BasicInfoItem_Load);
            this.DoubleClick += new System.EventHandler(this.BasicInfo_DoubleClick);
            this.Controls.SetChildIndex(this.txtPeriodCredit, 0);
            this.Controls.SetChildIndex(this.txtSubject, 0);
            this.Controls.SetChildIndex(this.cboMultiTeacher, 0);
            this.Controls.SetChildIndex(this.txtCourseName, 0);
            this.Controls.SetChildIndex(this.labelX2, 0);
            this.Controls.SetChildIndex(this.labelX1, 0);
            this.Controls.SetChildIndex(this.labelX4, 0);
            this.Controls.SetChildIndex(this.labelX5, 0);
            this.Controls.SetChildIndex(this.labelX6, 0);
            this.Controls.SetChildIndex(this.labelX14, 0);
            this.Controls.SetChildIndex(this.labelX7, 0);
            this.Controls.SetChildIndex(this.cboSemester, 0);
            this.Controls.SetChildIndex(this.cboSchoolYear, 0);
            this.Controls.SetChildIndex(this.cboClass, 0);
            this.Controls.SetChildIndex(this.btnTeachers, 0);
            this.Controls.SetChildIndex(this.picWaiting, 0);
            this.Controls.SetChildIndex(this.cboDomain, 0);
            ((System.ComponentModel.ISupportInitialize)(this.picWaiting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        protected DevComponents.DotNetBar.Controls.TextBoxX txtCourseName;
        protected DevComponents.DotNetBar.Controls.TextBoxX txtSubject;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.LabelX labelX6;
        private DevComponents.DotNetBar.LabelX labelX7;
        protected DevComponents.DotNetBar.Controls.TextBoxX txtPeriodCredit;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboClass;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.Editors.ComboItem comboItem7;
        private DevComponents.Editors.ComboItem comboItem8;
        private DevComponents.Editors.ComboItem comboItem9;
        private DevComponents.Editors.ComboItem comboItem10;
        private DevComponents.DotNetBar.ButtonX btnTeachers;
        private DevComponents.DotNetBar.ButtonItem btnTeacher1;
        private DevComponents.DotNetBar.ButtonItem btnTeacher2;
        private DevComponents.DotNetBar.ButtonItem btnTeacher3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboMultiTeacher;
        private DevComponents.DotNetBar.LabelX labelX14;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboDomain;

    }
}
