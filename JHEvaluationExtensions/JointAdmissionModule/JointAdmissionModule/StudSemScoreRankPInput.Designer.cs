namespace JointAdmissionModule
{
    partial class StudSemScoreRankPInput
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
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.ucSocreRankItem1 = new JointAdmissionModule.UCSocreRankItem();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(388, 169);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(474, 169);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // ucSocreRankItem1
            // 
            this.ucSocreRankItem1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.ucSocreRankItem1.AutoSize = true;
            this.ucSocreRankItem1.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.ucSocreRankItem1.BackColor = System.Drawing.Color.Transparent;
            this.ucSocreRankItem1.CancelButtonVisible = false;
            this.ucSocreRankItem1.ContentValidated = true;
            this.ucSocreRankItem1.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ucSocreRankItem1.Group = "UCSocreRankItem";
            this.ucSocreRankItem1.Loading = false;
            this.ucSocreRankItem1.Location = new System.Drawing.Point(0, 12);
            this.ucSocreRankItem1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucSocreRankItem1.Name = "ucSocreRankItem1";
            this.ucSocreRankItem1.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.ucSocreRankItem1.SaveButtonVisible = false;
            this.ucSocreRankItem1.Size = new System.Drawing.Size(550, 150);
            this.ucSocreRankItem1.TabIndex = 2;
            // 
            // StudSemScoreRankPInput
            // 
            this.ClientSize = new System.Drawing.Size(553, 198);
            this.Controls.Add(this.ucSocreRankItem1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Name = "StudSemScoreRankPInput";
            this.Text = "學生學期成績排名與排名百分比輸入";
            this.Load += new System.EventHandler(this.StudSemScoreRankPInput_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private UCSocreRankItem ucSocreRankItem1;
    }
}