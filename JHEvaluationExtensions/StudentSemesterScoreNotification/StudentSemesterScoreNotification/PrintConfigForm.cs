using Aspose.Words;
using Campus.Report;
using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StudentSemesterScoreNotification
{
    public partial class PrintConfigForm : BaseForm
    {

        private ReportConfiguration _config;

        public PrintConfigForm()
        {
            InitializeComponent();
            _config = new ReportConfiguration(Global.TemplateConfigName);

            string setting = _config.GetString("列印設定", "預設範本");

            if (setting == "自訂範本1")
                rbCustomize1.Checked = true;
            else if (setting == "自訂範本2")
                rbCustomize2.Checked = true;
            else
                rbDefault.Checked = true;
        }

        /// <summary>
        /// 檢視預設範本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkView1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Word (*.doc)|*.doc";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                //_config.Template.ToDocument().Save(saveDialog.FileName);
                Document doc = new Document(new MemoryStream(Properties.Resources.Template));

                try
                {
                    doc.Save(saveDialog.FileName, Aspose.Words.SaveFormat.Doc);
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
                catch
                {
                    MsgBox.Show("路徑無法存取，請確認檔案是否未正確關閉。");
                }
            }
        }

        /// <summary>
        /// 檢視自訂範本1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkView2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ViewTemplate("自訂範本1");
        }

        /// <summary>
        /// 檢視自訂範本2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkView3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ViewTemplate("自訂範本2");
        }

        private void lnkUpload1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UploadTemplate("自訂範本1");
        }

        private void lnkUpload2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UploadTemplate("自訂範本2");
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            string value;

            if (rbCustomize1.Checked)
                value = "自訂範本1";
            else if (rbCustomize2.Checked)
                value = "自訂範本2";
            else
                value = "預設範本";

            _config.SetString("列印設定", value);
            _config.Save();

            this.Close();
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewTemplate(string value)
        {
            string base64str = _config.GetString(value, string.Empty);

            if (string.IsNullOrWhiteSpace(base64str))
            {
                MsgBox.Show("目前沒有任何範本，請重新上傳。");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Word (*.doc)|*.doc";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                //_config.Template.ToDocument().Save(saveDialog.FileName);
                Document doc = new Document(new MemoryStream(Convert.FromBase64String(base64str)));

                try
                {
                    doc.Save(saveDialog.FileName, Aspose.Words.SaveFormat.Doc);
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
                catch
                {
                    MsgBox.Show("路徑無法存取，請確認檔案是否未正確關閉。");
                }
            }
        }

        private void UploadTemplate(string value)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Word (*.doc)|*.doc";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(openDialog.FileName);
                TemplateType type = TemplateType.Word;

                ReportTemplate template = new ReportTemplate(fileInfo, type);

                _config.SetString(value, template.ToBase64());

                _config.Save();

                MessageBox.Show("上傳完成!");
            }
        }

        private void lnkAllFields_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = "合併欄位總表.doc";
            saveDialog.Filter = "Word (*.doc)|*.doc";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                Document doc = new Document(new MemoryStream(Properties.Resources.MergeFields));

                try
                {
                    doc.Save(saveDialog.FileName, Aspose.Words.SaveFormat.Doc);
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
                catch
                {
                    MsgBox.Show("路徑無法存取，請確認檔案是否未正確關閉。");
                }
            }
        }
    }
}
