using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data.Configuration;
using System.Xml;
using Campus.Report;
using System.IO;
using Aspose.Words;

namespace JHEvaluation.StudentSemesterScoreNotification.Forms
{
    public partial class PrintConfigForm : BaseForm
    {
        private ReportConfiguration _config;

        public PrintConfigForm()
        {
            InitializeComponent();
            _config = new ReportConfiguration(Global.ReportName);

            SetupDefaultTemplate();
            LoadConfig();
        }

        private void SetupDefaultTemplate()
        {
            //如果設定中沒有範本，使用預設範本。
            if (_config.Template == null)
            {
                byte[] tplBytes = (Global.Params["Mode"] == "KaoHsiung") ? Properties.Resources.高雄學期成績通知單樣板 : Properties.Resources.新竹學期成績通知單樣板;
                ReportTemplate template = new ReportTemplate(tplBytes, TemplateType.Word);
                _config.Template = template;
            }
        }

        private void LoadConfig()
        {
            string print = _config.GetString("領域科目設定", string.Empty);
            if (print == "Domain")
                rbDomain.Checked = true;
            else if (print == "Subject")
                rbSubject.Checked = true;

            chkLearnDomain.Checked = _config.GetBoolean("列印學習領域總成績", true);

            chkPeriod.Checked = _config.GetBoolean("列印節數", true);
            chkCredit.Checked = _config.GetBoolean("列印權數", false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _config.SetString("領域科目設定", (rbDomain.Checked) ? "Domain" : "Subject");
            _config.SetBoolean("列印學習領域總成績", chkLearnDomain.Checked);
            _config.SetBoolean("列印節數", chkPeriod.Checked);
            _config.SetBoolean("列印權數", chkCredit.Checked);
            _config.Save();

            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_config.Template == null)
            {
                MsgBox.Show("目前沒有任何範本，請重新上傳。");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Word (*.doc)|*.doc";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                //_config.Template.ToDocument().Save(saveDialog.FileName);
                Document doc = new Document(new MemoryStream(_config.Template.ToBinary()));
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

        private void lnUpload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Word (*.doc)|*.doc";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(openDialog.FileName);
                TemplateType type = TemplateType.Word;

                ReportTemplate template = new ReportTemplate(fileInfo, type);
                _config.Template = template;
            }
        }

        private void lnClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MsgBox.Show("您確定要移除自訂範本嗎？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _config.Template = null;
                SetupDefaultTemplate();
            }
        }
    }
}
