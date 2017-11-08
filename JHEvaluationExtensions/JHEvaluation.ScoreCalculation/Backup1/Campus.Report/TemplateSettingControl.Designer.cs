namespace Campus.Report
{
    partial class TemplateSettingControl
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.lnkDownload = new System.Windows.Forms.LinkLabel();
            this.lnkUpload = new System.Windows.Forms.LinkLabel();
            this.lnkReset = new System.Windows.Forms.LinkLabel();
            this.lblLastSavedBy = new DevComponents.DotNetBar.LabelX();
            this.lblLastSaveTime = new DevComponents.DotNetBar.LabelX();
            this.lblSpace = new DevComponents.DotNetBar.LabelX();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lnkDownload
            // 
            this.lnkDownload.AutoSize = true;
            this.lnkDownload.Location = new System.Drawing.Point(6, 48);
            this.lnkDownload.Name = "lnkDownload";
            this.lnkDownload.Size = new System.Drawing.Size(77, 12);
            this.lnkDownload.TabIndex = 0;
            this.lnkDownload.TabStop = true;
            this.lnkDownload.Text = "下載目前樣版";
            this.lnkDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDownload_LinkClicked);
            // 
            // lnkUpload
            // 
            this.lnkUpload.AutoSize = true;
            this.lnkUpload.LinkArea = new System.Windows.Forms.LinkArea(0, 4);
            this.lnkUpload.Location = new System.Drawing.Point(6, 6);
            this.lnkUpload.Name = "lnkUpload";
            this.lnkUpload.Size = new System.Drawing.Size(53, 12);
            this.lnkUpload.TabIndex = 0;
            this.lnkUpload.TabStop = true;
            this.lnkUpload.Text = "上傳樣版";
            this.lnkUpload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUpload_LinkClicked);
            // 
            // lnkReset
            // 
            this.lnkReset.AutoSize = true;
            this.lnkReset.Location = new System.Drawing.Point(6, 90);
            this.lnkReset.Name = "lnkReset";
            this.lnkReset.Size = new System.Drawing.Size(53, 12);
            this.lnkReset.TabIndex = 0;
            this.lnkReset.TabStop = true;
            this.lnkReset.Text = "重設樣版";
            this.lnkReset.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkReset_LinkClicked);
            // 
            // lblLastSavedBy
            // 
            this.lblLastSavedBy.AutoSize = true;
            this.lblLastSavedBy.BackColor = System.Drawing.Color.Transparent;
            this.lblLastSavedBy.ForeColor = System.Drawing.Color.Black;
            this.lblLastSavedBy.Location = new System.Drawing.Point(4, 21);
            this.lblLastSavedBy.Name = "lblLastSavedBy";
            this.lblLastSavedBy.Size = new System.Drawing.Size(130, 18);
            this.lblLastSavedBy.TabIndex = 0;
            this.lblLastSavedBy.Text = "修改人：澔學有限公司";
            // 
            // lblLastSaveTime
            // 
            this.lblLastSaveTime.AutoSize = true;
            this.lblLastSaveTime.BackColor = System.Drawing.Color.Transparent;
            this.lblLastSaveTime.ForeColor = System.Drawing.Color.Black;
            this.lblLastSaveTime.Location = new System.Drawing.Point(4, 54);
            this.lblLastSaveTime.Name = "lblLastSaveTime";
            this.lblLastSaveTime.Size = new System.Drawing.Size(165, 18);
            this.lblLastSaveTime.TabIndex = 0;
            this.lblLastSaveTime.Text = "修改時間：2012/12/21 PM 1:55";
            // 
            // lblSpace
            // 
            this.lblSpace.AutoSize = true;
            this.lblSpace.BackColor = System.Drawing.Color.Transparent;
            this.lblSpace.ForeColor = System.Drawing.Color.Black;
            this.lblSpace.Location = new System.Drawing.Point(4, 88);
            this.lblSpace.Name = "lblSpace";
            this.lblSpace.Size = new System.Drawing.Size(102, 18);
            this.lblSpace.TabIndex = 0;
            this.lblSpace.Text = "樣版容量：540KB";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLastSavedBy);
            this.groupBox1.Controls.Add(this.lblSpace);
            this.groupBox1.Controls.Add(this.lblLastSaveTime);
            this.groupBox1.Location = new System.Drawing.Point(195, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 118);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "目前樣版資訊";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "上傳您自定的樣版。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "下載目前的樣版到本機電腦中。";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "將樣版回復到預設版面。";
            // 
            // TemplateSettingControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lnkReset);
            this.Controls.Add(this.lnkUpload);
            this.Controls.Add(this.lnkDownload);
            this.Name = "TemplateSettingControl";
            this.Size = new System.Drawing.Size(406, 129);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.LinkLabel lnkDownload;
        protected System.Windows.Forms.LinkLabel lnkUpload;
        protected System.Windows.Forms.LinkLabel lnkReset;
        protected DevComponents.DotNetBar.LabelX lblSpace;
        protected DevComponents.DotNetBar.LabelX lblLastSaveTime;
        protected DevComponents.DotNetBar.LabelX lblLastSavedBy;
        protected System.Windows.Forms.GroupBox groupBox1;
        protected System.Windows.Forms.Label label1;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.Label label3;

    }
}
