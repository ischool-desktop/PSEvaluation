using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Campus.Report;
using FISCA.Presentation.Controls;
using System.IO;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReportControls
{
    public partial class PrintConfigForm : BaseForm
    {
        private ReportConfiguration Config { get; set;}

        /// <summary>
        /// 收件人地址(設定儲存用)
        /// </summary>
        public static string setupAddresseeAddress = "收件人地址";
        /// <summary>
        /// 收件人姓名(設定儲存用)
        /// </summary>
        public static string setupAddresseeName = "收件人姓名";
        /// <summary>
        /// 發文日期(設定儲存用)
        /// </summary>
        public static string setupdtDocDate = "發文日期";

        /// <summary>
        /// 另存學生清單
        /// </summary>
        public static string setupExportStudentList = "另存學生清單";

        public PrintConfigForm()
        {
            InitializeComponent();
        }

        private void PrintConfigForm_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            Config = new ReportConfiguration("未達畢業標準通知單");
            SetupDefaultTemplate();
            LoadConfig();
        }

        /// <summary>
        /// 設定預設樣板
        /// </summary>
        private void SetupDefaultTemplate()
        {
            if (Config.Template == null)
            {
                ReportTemplate template = new ReportTemplate(JHEvaluation.ScoreCalculation.Properties.Resources.未達畢業標準通知單樣板, TemplateType.Word);
                Config.Template = template;            
            }        
        }

        /// <summary>
        /// 載入設定
        /// </summary>
        private void LoadConfig()
        {
            cboAddresseeAddress.Items.Add("聯絡地址");
            cboAddresseeAddress.Items.Add("戶籍地址");
            cboAddresseeAddress.Text = Config.GetString(setupAddresseeAddress, "聯絡地址");
            cboAddresseeName.Items.Add("父親");
            cboAddresseeName.Items.Add("母親");
            cboAddresseeName.Items.Add("監護人");
            cboAddresseeName.Text = Config.GetString(setupAddresseeName, "監護人");
            chkExportStuentList.Checked = Config.GetBoolean(setupExportStudentList, false);

            // 如果沒有設定日期就用當天
            DateTime dt;
            if (DateTime.TryParse(Config.GetString(setupdtDocDate, ""), out dt))
                dtDocDate.Value =dt;
            else
                dtDocDate.Value =DateTime.Now;


        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Config.SetString(setupAddresseeAddress, cboAddresseeAddress.Text);
            Config.SetString(setupAddresseeName, cboAddresseeName.Text);
            Config.SetString(setupdtDocDate, dtDocDate.Value.ToShortDateString());
            Config.SetBoolean(setupExportStudentList, chkExportStuentList.Checked);
            Config.Save();
            this.DialogResult = DialogResult.OK;
        }

        private void lnkRemove_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MsgBox.Show("您確定要移除自訂範本嗎?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Config.Template = null;
                SetupDefaultTemplate();
            }
        }

        private void lnkUpload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Word (*.doc)|*.doc";
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(openDialog.FileName);
                TemplateType type = TemplateType.Word;
                ReportTemplate template = new ReportTemplate(fileInfo, type);
                Config.Template = template;
            }
        }

        private void lnkDefault_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Config.Template == null)
            {
                MsgBox.Show("目前沒有任何範本，請重新上傳。");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = "Template";
            saveDialog.Filter = "Word (*.doc)|*.doc";
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Config.Template.ToDocument().Save(saveDialog.FileName);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗", ex.Message);
                    return;
                }

                try
                {
                    System.Diagnostics.Process.Start(saveDialog.FileName);

                }
                catch (Exception ex)
                {

                    MsgBox.Show("開啟失敗", ex.Message);
                    return;
                }
            }
        }

        private void lnkView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = "合併欄位總表";
            saveDialog.Filter = "Word (*.doc)|*.doc";
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Aspose.Words.Document doc = new Aspose.Words.Document(new MemoryStream(JHEvaluation.ScoreCalculation.Properties.Resources.功能變數));
                    doc.Save(saveDialog.FileName);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗", ex.Message);
                    return;
                }

                try
                {
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("開啟失敗", ex.Message);
                    return;
                }
            }
        }
    }
}
