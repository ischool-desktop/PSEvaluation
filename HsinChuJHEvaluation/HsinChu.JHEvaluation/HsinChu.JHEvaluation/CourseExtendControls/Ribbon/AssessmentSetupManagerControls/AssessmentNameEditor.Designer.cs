namespace HsinChu.JHEvaluation.CourseExtendControls.Ribbon.AssessmentSetupManagerControls
{
    partial class AssessmentNameEditor
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtTemplateName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.btnConfirm = new DevComponents.DotNetBar.ButtonX();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.lblCopyExist = new DevComponents.DotNetBar.LabelX();
            this.cboExistTemplates = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem1 = new DevComponents.Editors.ComboItem();
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
            this.labelX1.Location = new System.Drawing.Point(9, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(60, 21);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "樣版名稱";
            // 
            // txtTemplateName
            // 
            // 
            // 
            // 
            this.txtTemplateName.Border.Class = "TextBoxBorder";
            this.txtTemplateName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtTemplateName.Location = new System.Drawing.Point(101, 9);
            this.txtTemplateName.Name = "txtTemplateName";
            this.txtTemplateName.Size = new System.Drawing.Size(256, 25);
            this.txtTemplateName.TabIndex = 1;
            // 
            // btnConfirm
            // 
            this.btnConfirm.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.btnConfirm.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(201, 73);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "確定";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(282, 73);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "離開";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblCopyExist
            // 
            this.lblCopyExist.AutoSize = true;
            this.lblCopyExist.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblCopyExist.BackgroundStyle.Class = "";
            this.lblCopyExist.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblCopyExist.Location = new System.Drawing.Point(9, 43);
            this.lblCopyExist.Name = "lblCopyExist";
            this.lblCopyExist.Size = new System.Drawing.Size(87, 21);
            this.lblCopyExist.TabIndex = 4;
            this.lblCopyExist.Text = "複製現有樣版";
            // 
            // cboExistTemplates
            // 
            this.cboExistTemplates.DisplayMember = "Name";
            this.cboExistTemplates.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboExistTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExistTemplates.FormattingEnabled = true;
            this.cboExistTemplates.ItemHeight = 19;
            this.cboExistTemplates.Items.AddRange(new object[] {
            this.comboItem1});
            this.cboExistTemplates.Location = new System.Drawing.Point(101, 40);
            this.cboExistTemplates.Name = "cboExistTemplates";
            this.cboExistTemplates.Size = new System.Drawing.Size(256, 25);
            this.cboExistTemplates.TabIndex = 5;
            this.cboExistTemplates.ValueMember = "ID";
            // 
            // comboItem1
            // 
            this.comboItem1.Text = "<不複製>";
            // 
            // AssessmentNameEditor
            // 
            this.AcceptButton = this.btnConfirm;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(367, 105);
            this.ControlBox = false;
            this.Controls.Add(this.cboExistTemplates);
            this.Controls.Add(this.lblCopyExist);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.txtTemplateName);
            this.Controls.Add(this.labelX1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AssessmentNameEditor";
            this.Text = "新增評分樣版";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTemplateName;
        private DevComponents.DotNetBar.ButtonX btnConfirm;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.DotNetBar.LabelX lblCopyExist;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboExistTemplates;
        private DevComponents.Editors.ComboItem comboItem1;
    }
}