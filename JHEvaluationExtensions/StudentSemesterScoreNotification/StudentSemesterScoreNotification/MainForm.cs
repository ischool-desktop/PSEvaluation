using Aspose.Words;
using Campus.Report;
using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentSemesterScoreNotification
{
    public partial class MainForm : BaseForm
    {
        public enum PrintType { 學生, 班級 };

        private PrintType _PrintType;
        private int _schoolYear, _semester;
        private List<StudentRecord> _Students;

        private bool _chkPrintReScore=false;
        private BackgroundWorker _BW;
        private ReportConfiguration _Config;
        private int _DataRowCount = 0;

        private ReportConfiguration _config;

        string _ReExamMark = "*";

        string _SelScoreItem1="原始成績";
        string _SelScoreItem2 = "原始補考擇優";
        string _UserSelScoreItem = "";

        public MainForm(PrintType type)
        {
            InitializeComponent();

            _PrintType = type;
            _config = new ReportConfiguration(Global.TemplateConfigName);

            _BW = new BackgroundWorker();
            _BW.DoWork += new DoWorkEventHandler(BW_DoWork);
            _BW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BW_RunWorkerCompleted);
        }

        private void BW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetForm(true);

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }

            if(_DataRowCount==0)
            {
                MessageBox.Show("沒有資料");
                return;
            }

            Document doc = e.Result as Document;

            SaveFileDialog save = new SaveFileDialog();
            save.Title = "另存新檔";
            save.FileName = "新版學期成績通知單";
            save.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    doc.Save(save.FileName, SaveFormat.Doc);
                    System.Diagnostics.Process.Start(save.FileName);
                }
                catch
                {
                    MessageBox.Show("檔案儲存失敗");
                }
            }
        }

        private void BW_DoWork(object sender, DoWorkEventArgs e)
        {
            Document doc;

            //讀取報表設定
            _Config = new ReportConfiguration(Global.TemplateConfigName);

            //列印設定
            string setting = _Config.GetString("列印設定", "預設範本");
            string template_base64str = _Config.GetString(setting, string.Empty);

            if (string.IsNullOrWhiteSpace(template_base64str))
                doc = new Document(new MemoryStream(Properties.Resources.Template));
            else
                doc = new Document(new MemoryStream(Convert.FromBase64String(template_base64str)));

            if (_PrintType == PrintType.學生)
                _Students = K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);
            else
                _Students = K12.Data.Student.SelectByClassIDs(K12.Presentation.NLDPanels.Class.SelectedSource).FindAll(x => x.Status == StudentRecord.StudentStatus.一般 || x.Status == StudentRecord.StudentStatus.休學);

            DataTable dt = DataSource.GetDataTable(_Students, _schoolYear, _semester);

            _DataRowCount = dt.Rows.Count;
            // 只顯示補可成績另外處理
            if(_chkPrintReScore)
            {
                DataTable newDt = new DataTable();
                foreach (DataColumn dc in dt.Columns)
                    newDt.Columns.Add(dc.ColumnName);

                // 有補考成績 再加入
                foreach(DataRow dr in dt.Rows)
                {
                    bool add = false;

                    // 檢查是否有補考成績
                    for (int i = 1; i <= Global.SupportSubjectCount;i++ )
                    {
                        if (dr["S補考成績"+i] != null)
                        {
                            if (dr["S補考成績"+i].ToString().Replace(" ", "").Length > 0)
                            {
                                add = true;
                                break;
                            }
                        }
                    }

                    for (int i = 1; i <= Global.SupportDomainCount;i++)
                    {
                        if (dr["D補考成績"+i] != null)
                        {
                            if (dr["D補考成績"+i].ToString().Replace(" ", "").Length > 0)
                            {
                                add = true;
                                break;
                            }                               
                        }
                    }

                    if(add)
                    {
                        DataRow newDr = newDt.NewRow();

                        foreach (DataColumn dc in dt.Columns)
                            newDr[dc.ColumnName] = dr[dc.ColumnName];

                        newDt.Rows.Add(newDr);
                    }

                }
                doc.MailMerge.Execute(newDt);
                _DataRowCount = newDt.Rows.Count;
            }
            else
                doc.MailMerge.Execute(dt);

            doc.MailMerge.DeleteFields();

            e.Result = doc;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MinimumSize = this.Size;
            int schoolYear, semester;

            cbxScoreType.Items.Add(_SelScoreItem1);
            cbxScoreType.Items.Add(_SelScoreItem2);
            cbxScoreType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxScoreType.Text = _config.GetString("成績類型", _SelScoreItem1);

            chkReScore.Checked=_config.GetBoolean("只產生有補考成績學生", true);
            txtReExammark.Text = _config.GetString("補考成績加註", "*");
            _schoolYear = int.TryParse(K12.Data.School.DefaultSchoolYear, out schoolYear) ? schoolYear : 0;
            _semester = int.TryParse(K12.Data.School.DefaultSemester, out semester) ? semester : 0;

            for (int i = -2; i <= 2; i++)
                cboSchoolYear.Items.Add(_schoolYear + i);

            cboSemester.Items.Add(1);
            cboSemester.Items.Add(2);

            cboSchoolYear.Text = _schoolYear + "";
            cboSemester.Text = _semester + "";
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            int sy,sm;

            if (!int.TryParse(cboSchoolYear.Text, out sy))
            {
                MessageBox.Show("學年度必須為整數數字");
                return;
            }

            if (!int.TryParse(cboSemester.Text, out sm))
            {
                MessageBox.Show("學期必須為整數數字");
                return;
            }

            _schoolYear = sy;
            _semester = sm;
            _chkPrintReScore = chkReScore.Checked;
            _ReExamMark = txtReExammark.Text;
            _config.SetBoolean("只產生有補考成績學生", chkReScore.Checked);
            _config.SetString("補考成績加註", _ReExamMark);
            _config.SetString("成績類型", cbxScoreType.Text);
            _UserSelScoreItem = cbxScoreType.Text;
            if (_BW.IsBusy)
                MessageBox.Show("系統忙碌中,請稍後再試");
            else
            {
                SetForm(false);
                DataSource.SetReExamMark(_ReExamMark);
                DataSource.SetUserSelScoreType(_UserSelScoreItem);
                _BW.RunWorkerAsync();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new PrintConfigForm().ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new SelectTypeForm().ShowDialog();
        }

        private void SetForm(bool b)
        {
            cboSchoolYear.Enabled = b;
            cboSemester.Enabled = b;
            btnConfirm.Enabled = b;
            lnkPrintSetting.Enabled = b;
            lnkAbsentSetting.Enabled = b;
        }

        private void labelX4_Click(object sender, EventArgs e)
        {

        }

        private void labelX1_Click(object sender, EventArgs e)
        {

        }

        private void labelX2_Click(object sender, EventArgs e)
        {

        }

    }
}
