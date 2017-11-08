using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FISCA.Presentation;
using FISCA.Presentation.Controls;

namespace JHEvaluation.AssignmentExamManager
{
    public partial class MainForm : BaseForm
    {
        private string InitialTitleText = string.Empty;

        /// <summary>
        /// Static call...
        /// </summary>
        internal static void Run()
        {
            new MainForm().ShowDialog();
        }

        private List<AssignmentExamRow> Rows { get; set; }

        public MainForm()
        {
            InitializeComponent();

            string VersionMessage = "『";

            foreach (Assembly Assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.Equals("JHEvaluation.AssignmentExamManager")))
                VersionMessage += Assembly.GetName().Version;

            VersionMessage += "』";


            InitialTitleText = "小考輸入狀況檢視" + VersionMessage;
            this.Text = InitialTitleText;

            RefreshUI();
            Rows = new List<AssignmentExamRow>();

            btnQuery.Click += (sender, e) => SelectAssignmentExam();
            dgvInfo.DoubleClick += (sender, e) => btnDetailView_Click(null, null);
        }

        /// <summary>
        /// 初始化介面
        /// </summary>
        private void RefreshUI()
        {
            #region 將所有控制項失效
            this.TitleText = InitialTitleText + " 資料準備中..";
            foreach (Control vControl in Controls)
                vControl.Enabled = false;
            #endregion

            #region 初始化學年度學期
            try
            {
                int schoolYear = int.Parse(K12.Data.School.DefaultSchoolYear);
                int semester = int.Parse(K12.Data.School.DefaultSemester);

                for (int i = -2; i <= 2; i++)
                    cboSchoolYear.Items.Add(i + schoolYear);
                cboSemester.Items.Add(1);
                cboSemester.Items.Add(2);

                cboSchoolYear.SelectedItem = schoolYear;
                cboSemester.SelectedItem = semester;
            }
            catch (Exception)
            {
            }
            #endregion

            #region 初始化領域下拉選單
            cboDomain.Items.Clear();
            KeyValuePair<string, string> displayAllDomains = new KeyValuePair<string, string>("顯示全部領域", "顯示全部領域");
            cboDomain.DisplayMember = "Key";
            cboDomain.ValueMember = "Value";
            cboDomain.Items.Add(displayAllDomains);

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (sender, e) =>
            {
                List<string> Domains = AssignmentExamCourse.SelectDomains(); 

                e.Result = Domains;
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                List<string> Domains = e.Result as List<string>;

                foreach (string Domain in Domains)
                    cboDomain.Items.Add(new KeyValuePair<string, string>(Domain.Equals("") ? "(空白)or未設定領域" : Domain, Domain));

                foreach (Control vControl in Controls)
                    vControl.Enabled = true;

                this.TitleText = InitialTitleText;
            };

            worker.RunWorkerAsync();
            #endregion
        }

        private void cboSchoolYearSemester_TextChanged(object sender, EventArgs e)
        {
            if (ValidateSchoolYearSemester() == false) return;

            //SelectAssignmentExam();

            //FillAssignmentExamRows(ConditionFilter(Rows));
        }

        private void SelectAssignmentExam()
        {
            string Domain = string.Empty;
            int? CompleteInputCount = null;
            int? SchoolYear = (int?)cboSchoolYear.SelectedItem;
            int? Semester = (int?)cboSemester.SelectedItem;

            if (chkCondition.Checked)
                CompleteInputCount = intTimes.Value;

            if (cboDomain.SelectedItem != null)
            {
                KeyValuePair<string, string> KeyValue = (KeyValuePair<string, string>)cboDomain.SelectedItem;
                Domain = KeyValue.Value;
            }

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (sender, e) =>
            {
                List<AssignmentExamCourseRecord> records = AssignmentExamCourse
                    .Select(SchoolYear
                    , Semester
                    , Domain
                    , CompleteInputCount
                    , worker.ReportProgress);

                worker.ReportProgress(0, "資料呈現");

                e.Result = records;
            };

            worker.ProgressChanged += (sender, e) =>
            {
                MotherForm.SetStatusBarMessage(""+e.UserState, e.ProgressPercentage);
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                List<AssignmentExamCourseRecord> records = e.Result as List<AssignmentExamCourseRecord>;

                foreach (AssignmentExamCourseRecord record in records)
                    dgvInfo.Rows.Add(record.ID, 
                        record.Domain, 
                        record.Name, 
                        record.MajorTeacherName, 
                        record.ExamCount, 
                        record.FinishedCount);

                dgvInfo.ResumeLayout();

                this.TitleText = InitialTitleText;
                foreach (Control vControl in Controls)
                    vControl.Enabled = true;

                MotherForm.SetStatusBarMessage("資料呈現", 100);
            };

            dgvInfo.SuspendLayout();
            dgvInfo.Rows.Clear();

            this.TitleText = InitialTitleText + " 資料查詢中..";

            foreach (Control vControl in Controls)
                vControl.Enabled = false;

            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// 填入 rows
        /// </summary>
        /// <param name="rows"></param>
        private void FillAssignmentExamRows(List<AssignmentExamRow> rows)
        {
            dgvInfo.SuspendLayout();
            dgvInfo.Rows.Clear();
            foreach (AssignmentExamRow row in rows)
            {
                dgvInfo.Rows.Add(row);
                row.UpdateDisplayValues();
            }
            dgvInfo.ResumeLayout();
        }

        /// <summary>
        /// 依條件過濾 rows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private List<AssignmentExamRow> ConditionFilter(List<AssignmentExamRow> rows)
        {
            if (chkCondition.Checked == false) return rows;

            List<AssignmentExamRow> filteredRows = new List<AssignmentExamRow>();
            foreach (AssignmentExamRow row in rows)
                if (row.FinishedCount < intTimes.Value)
                    filteredRows.Add(row);
            return filteredRows;
        }

        /// <summary>
        /// 驗證學年度學期
        /// </summary>
        /// <returns></returns>
        private bool ValidateSchoolYearSemester()
        {
            bool valid = true;
            errorProvider.Clear();

            int i;
            if (!int.TryParse(cboSchoolYear.Text, out i))
            {
                errorProvider.SetError(cboSchoolYear, "學年度應為整數");
                valid &= false;
            }
            if (!int.TryParse(cboSemester.Text, out i))
            {
                errorProvider.SetError(cboSemester, "學期應為 1 或 2");
                valid &= false;
            }
            return valid;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ExportUtil.Export(dgvInfo, string.Format("{0}-{1} 小考輸入狀況", cboSchoolYear.Text, cboSemester.Text));
        }

        private void btnDetailView_Click(object sender, EventArgs e)
        {
            if (dgvInfo.SelectedRows.Count != 1)
            {
                MessageBox.Show("一次只能選擇一個課程檢視明細!");
                return;
            }
            //AssignmentExamRow row = dgvInfo.SelectedRows[0] as AssignmentExamRow;
            //DetailViewForm detailViewForm = new DetailViewForm(row);

            DetailViewForm detailViewForm = new DetailViewForm(dgvInfo.SelectedRows[0].Cells[0].Value.ToString());
            detailViewForm.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
