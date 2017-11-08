using System;
using System.IO;
using System.Windows.Forms;
using Campus.Report;
using FISCA.Presentation.Controls;
using Aspose.Words;

namespace KaoHsiung.StudentExamScoreReport.ConfigForm
{
    public partial class PrintConfigForm : BaseForm
    {
        private ReportConfiguration Config { get; set; }

        public PrintConfigForm()
        {
            InitializeComponent();
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            Config = new ReportConfiguration(Global.ReportName);

            SetupDefaultTemplate();
            LoadConfig();
        }

        private void SetupDefaultTemplate()
        {
            //如果設定中沒有範本，使用預設範本。
            if (Config.Template == null)
            {
                ReportTemplate template = new ReportTemplate(Properties.Resources.評量成績通知單, TemplateType.Word);
                Config.Template = template;
            }
        }

        private void LoadConfig()
        {
            string print = Config.GetString("領域科目設定", string.Empty);
            if (print == "Domain")
                rbDomain.Checked = true;
            else if (print == "Subject")
                rbSubject.Checked = true;

            chkPeriod.Checked = Config.GetBoolean("列印節數", false);
            chkCredit.Checked = Config.GetBoolean("列印權數", false);

            //chkText.Checked = Config.GetBoolean("列印文字評語", true);
            chkText.Checked = false;
            Config.SetBoolean("列印文字評語", chkText.Checked);
            Config.Save();

            chkAssignment.Checked = Config.GetBoolean("列印平時評量", false);

            rtnPDF.Checked = Config.GetBoolean("輸出成PDF格式", false);
            chkEffort.Checked = Config.GetBoolean("列印努力程度", true);
            chkScore.Checked = Config.GetBoolean("列印分數評量", true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Config.SetString("領域科目設定", (rbDomain.Checked) ? "Domain" : "Subject");
            Config.SetBoolean("列印節數", chkPeriod.Checked);
            Config.SetBoolean("列印權數", chkCredit.Checked);            
            Config.SetBoolean("列印文字評語", chkText.Checked);
            Config.SetBoolean("列印平時評量", chkAssignment.Checked);
            Config.SetBoolean("輸出成PDF格式", rtnPDF.Checked);
            Config.SetBoolean("列印分數評量", chkScore.Checked);
            Config.SetBoolean("列印努力程度", chkEffort.Checked);
            Config.Save();

            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Config.Template == null)
            {
                MsgBox.Show("目前沒有任何範本，請重新上傳。");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Word (*.doc)|*.doc";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Config.Template.ToDocument().Save(saveDialog.FileName);
                    Document doc = new Document(new MemoryStream(Config.Template.ToBinary()));
                    doc.Save(saveDialog.FileName, Aspose.Words.SaveFormat.Doc);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗。" + ex.Message);
                    return;
                }
                try
                {
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("開啟失敗。" + ex.Message);
                    return;
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
                Config.Template = template;
            }
        }

        private void lnRemove_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MsgBox.Show("您確定要移除自訂範本嗎？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Config.Template = null;
                SetupDefaultTemplate();
            }
        }
    }
}
