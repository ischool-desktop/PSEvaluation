namespace JHSchool.Evaluation.CourseExtendControls
{
    partial class ScoreCalcSetupItem
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
            this.labelX10 = new DevComponents.DotNetBar.LabelX();
            this.cboAssessmentSetup = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem18 = new DevComponents.Editors.ComboItem();
            this.comboItem19 = new DevComponents.Editors.ComboItem();
            this.labelX12 = new DevComponents.DotNetBar.LabelX();
            this.rdoCalcTrue = new System.Windows.Forms.RadioButton();
            this.rdoCalcFalse = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelX10
            // 
            this.labelX10.Location = new System.Drawing.Point(9, 46);
            this.labelX10.Name = "labelX10";
            this.labelX10.Size = new System.Drawing.Size(70, 23);
            this.labelX10.TabIndex = 20;
            this.labelX10.Text = "學期成績";
            this.labelX10.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // cboAssessmentSetup
            // 
            this.cboAssessmentSetup.DisplayMember = "Text";
            this.cboAssessmentSetup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboAssessmentSetup.FormattingEnabled = true;
            this.cboAssessmentSetup.ItemHeight = 19;
            this.cboAssessmentSetup.Items.AddRange(new object[] {
            this.comboItem18,
            this.comboItem19});
            this.cboAssessmentSetup.Location = new System.Drawing.Point(93, 8);
            this.cboAssessmentSetup.MaxDropDownItems = 6;
            this.cboAssessmentSetup.Name = "cboAssessmentSetup";
            this.cboAssessmentSetup.Size = new System.Drawing.Size(431, 25);
            this.cboAssessmentSetup.TabIndex = 23;
            this.cboAssessmentSetup.Tag = "ForceValidate";
            this.cboAssessmentSetup.SelectedIndexChanged += new System.EventHandler(this.cboAssessmentSetup_SelectedIndexChanged);
            // 
            // comboItem18
            // 
            this.comboItem18.Text = "是";
            // 
            // comboItem19
            // 
            this.comboItem19.Text = "否";
            // 
            // labelX12
            // 
            this.labelX12.Location = new System.Drawing.Point(9, 9);
            this.labelX12.Name = "labelX12";
            this.labelX12.Size = new System.Drawing.Size(70, 23);
            this.labelX12.TabIndex = 22;
            this.labelX12.Text = "評量設定";
            this.labelX12.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // rdoCalcTrue
            // 
            this.rdoCalcTrue.AutoSize = true;
            this.rdoCalcTrue.Location = new System.Drawing.Point(7, 2);
            this.rdoCalcTrue.Name = "rdoCalcTrue";
            this.rdoCalcTrue.Size = new System.Drawing.Size(52, 21);
            this.rdoCalcTrue.TabIndex = 24;
            this.rdoCalcTrue.Text = "列入";
            this.rdoCalcTrue.UseVisualStyleBackColor = true;
            this.rdoCalcTrue.CheckedChanged += new System.EventHandler(this.rdoCalcTrue_CheckedChanged);
            // 
            // rdoCalcFalse
            // 
            this.rdoCalcFalse.AutoSize = true;
            this.rdoCalcFalse.Checked = true;
            this.rdoCalcFalse.Location = new System.Drawing.Point(66, 2);
            this.rdoCalcFalse.Name = "rdoCalcFalse";
            this.rdoCalcFalse.Size = new System.Drawing.Size(65, 21);
            this.rdoCalcFalse.TabIndex = 24;
            this.rdoCalcFalse.TabStop = true;
            this.rdoCalcFalse.Text = "不列入";
            this.rdoCalcFalse.UseVisualStyleBackColor = true;
            this.rdoCalcFalse.CheckedChanged += new System.EventHandler(this.rdoCalcFalse_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdoCalcTrue);
            this.panel1.Controls.Add(this.rdoCalcFalse);
            this.panel1.Location = new System.Drawing.Point(93, 46);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(140, 25);
            this.panel1.TabIndex = 25;
            // 
            // ScoreCalcSetupItem
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.labelX12);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelX10);
            this.Controls.Add(this.cboAssessmentSetup);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(550, 0);
            this.Name = "ScoreCalcSetupItem";
            this.Size = new System.Drawing.Size(550, 85);
            this.Load += new System.EventHandler(this.ScoreCalcSetupItem_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX10;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboAssessmentSetup;
        private DevComponents.Editors.ComboItem comboItem18;
        private DevComponents.Editors.ComboItem comboItem19;
        private DevComponents.DotNetBar.LabelX labelX12;
        private System.Windows.Forms.RadioButton rdoCalcTrue;
        private System.Windows.Forms.RadioButton rdoCalcFalse;
        private System.Windows.Forms.Panel panel1;

    }
}
