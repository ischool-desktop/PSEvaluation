namespace JHEvaluation.ScoreCalculation
{
    partial class DomainScoreCalculateByGradeyear
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
            this.intGradeyear = new DevComponents.Editors.IntegerInput();
            this.intSemester = new DevComponents.Editors.IntegerInput();
            this.intSchoolYear = new DevComponents.Editors.IntegerInput();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.btnCalc = new DevComponents.DotNetBar.ButtonX();
            this.chkClearDomainScore = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.chkScoreLimite = new DevComponents.DotNetBar.Controls.CheckBoxX();
            ((System.ComponentModel.ISupportInitialize)(this.intGradeyear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSemester)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).BeginInit();
            this.SuspendLayout();
            // 
            // intGradeyear
            // 
            this.intGradeyear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intGradeyear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intGradeyear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intGradeyear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intGradeyear.Location = new System.Drawing.Point(75, 47);
            this.intGradeyear.Name = "intGradeyear";
            this.intGradeyear.ShowUpDown = true;
            this.intGradeyear.Size = new System.Drawing.Size(224, 25);
            this.intGradeyear.TabIndex = 22;
            // 
            // intSemester
            // 
            this.intSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intSemester.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intSemester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intSemester.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intSemester.Location = new System.Drawing.Point(219, 16);
            this.intSemester.Name = "intSemester";
            this.intSemester.ShowUpDown = true;
            this.intSemester.Size = new System.Drawing.Size(80, 25);
            this.intSemester.TabIndex = 21;
            // 
            // intSchoolYear
            // 
            this.intSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intSchoolYear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intSchoolYear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intSchoolYear.Location = new System.Drawing.Point(75, 16);
            this.intSchoolYear.Name = "intSchoolYear";
            this.intSchoolYear.ShowUpDown = true;
            this.intSchoolYear.Size = new System.Drawing.Size(80, 25);
            this.intSchoolYear.TabIndex = 20;
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelX3.Location = new System.Drawing.Point(179, 17);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(34, 23);
            this.labelX3.TabIndex = 19;
            this.labelX3.Text = "學期";
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelX1.Location = new System.Drawing.Point(22, 46);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(47, 23);
            this.labelX1.TabIndex = 17;
            this.labelX1.Text = "年級";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelX2.Location = new System.Drawing.Point(22, 17);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(47, 23);
            this.labelX2.TabIndex = 18;
            this.labelX2.Text = "學年度";
            // 
            // btnCalc
            // 
            this.btnCalc.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCalc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalc.BackColor = System.Drawing.Color.Transparent;
            this.btnCalc.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCalc.Location = new System.Drawing.Point(224, 78);
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.Size = new System.Drawing.Size(75, 23);
            this.btnCalc.TabIndex = 16;
            this.btnCalc.Text = "計算";
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // chkClearDomainScore
            // 
            this.chkClearDomainScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkClearDomainScore.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkClearDomainScore.BackgroundStyle.Class = "";
            this.chkClearDomainScore.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkClearDomainScore.Location = new System.Drawing.Point(-2, -3);
            this.chkClearDomainScore.Name = "chkClearDomainScore";
            this.chkClearDomainScore.Size = new System.Drawing.Size(18, 23);
            this.chkClearDomainScore.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkClearDomainScore.TabIndex = 23;
            this.chkClearDomainScore.Text = "刪除全部領域成績並重算";
            this.chkClearDomainScore.Visible = false;
            // 
            // chkScoreLimite
            // 
            this.chkScoreLimite.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkScoreLimite.BackgroundStyle.Class = "";
            this.chkScoreLimite.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkScoreLimite.Location = new System.Drawing.Point(17, 78);
            this.chkScoreLimite.Name = "chkScoreLimite";
            this.chkScoreLimite.Size = new System.Drawing.Size(201, 23);
            this.chkScoreLimite.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkScoreLimite.TabIndex = 24;
            this.chkScoreLimite.Text = "補考科目該領域不得超過60分";
            // 
            // DomainScoreCalculateByGradeyear
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 108);
            this.Controls.Add(this.chkScoreLimite);
            this.Controls.Add(this.chkClearDomainScore);
            this.Controls.Add(this.intGradeyear);
            this.Controls.Add(this.intSemester);
            this.Controls.Add(this.intSchoolYear);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.btnCalc);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(336, 147);
            this.MinimumSize = new System.Drawing.Size(336, 147);
            this.Name = "DomainScoreCalculateByGradeyear";
            this.Text = "批次學期領域成績計算";
            this.Load += new System.EventHandler(this.DomainScoreCalculateByGradeyear_Load);
            ((System.ComponentModel.ISupportInitialize)(this.intGradeyear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSemester)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.Editors.IntegerInput intGradeyear;
        private DevComponents.Editors.IntegerInput intSemester;
        private DevComponents.Editors.IntegerInput intSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.ButtonX btnCalc;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkClearDomainScore;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkScoreLimite;
    }
}