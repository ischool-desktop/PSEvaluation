using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using System.Threading;
using K12.Data;
using Campus.Rating;

namespace HsinChu.ClassExamScoreReportV2
{
    /// <summary>
    /// 此表單不適合繼承。
    /// </summary>
    public partial class MainForm : BaseForm
    {
        /// <summary>
        /// 報表設定。
        /// </summary>
        private ReportPreference Perference = null;

        /// <summary>
        /// 可選擇的科目清單。
        /// 用於第二執行緒收集，主執行緒填入 ListView用途。
        /// </summary>
        private List<string> Subjects = new List<string>();

        /// <summary>
        /// 用於科目對照領域。
        /// </summary>
        private Dictionary<string, string> SubjectDomainMap = new Dictionary<string, string>();

        /// <summary>
        /// 代表使用者選擇的學期資訊。
        /// </summary>
        private SemesterSelector Semester;

        /// <summary>
        /// 要列印的班級清單。
        /// </summary>
        private List<JHClassRecord> SelectedClasses;

        /// <summary>
        /// 因為要算年排名，所以要全校的學生(一般狀態)。
        /// </summary>
        private List<ReportStudent> AllStudents = new List<ReportStudent>();

        /// <summary>
        /// 下載目前學期資料的 BackgroundWorker。
        /// </summary>
        private BackgroundWorker MasterWorker = new BackgroundWorker();
        private bool WorkPadding = false; //是否有工作 Padding。

        private UserOptions Options { get; set; }

        public static void Run(List<string> classes)
        {
            new MainForm(classes).ShowDialog();
        }

        public MainForm(List<string> classes)
        {
            InitializeComponent();

            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;

            cbExam.DisplayMember = "Name";
            cbScore.Items.Add("定期");
            cbScore.Items.Add("定期加平時");
            cbScore.SelectedIndex = 0;

            cbExam.Items.Add("");
            foreach (JHExamRecord exam in JHExam.SelectAll())
                cbExam.Items.Add(exam);
            cbExam.SelectedIndex = 0;

            cbExam.SelectedIndexChanged += new EventHandler(cbExam_SelectedIndexChanged);

            LoadingSubject.Visible = false;
            LoadingDomain.Visible = false;

            //準備相關學生、班級資料。
            ReportStudent.SetClassMapping(JHClass.SelectAll());
            SelectedClasses = JHClass.SelectByIDs(classes); //要列印成績單的班級清單。
            AllStudents = JHStudent.SelectAll().ToReportStudent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Utilities.SetSemesterDefaultItems(cboSchoolYear, cboSemester); //顯示學年度學期選項。

            Semester = new SemesterSelector(cboSchoolYear, cboSemester);
            Semester.SemesterChanged += new EventHandler(Semester_SemesterChanged);

            Options = new UserOptions();

            MasterWorker.DoWork += new DoWorkEventHandler(MasterWorker_DoWork);
            MasterWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MasterWorker_RunWorkerCompleted);

            //報表設定。
            Perference = new ReportPreference();

            FillCurrentSemesterData();
        }

        private void FillCurrentSemesterData()
        {
            if (cbExam.SelectedItem == null || !(cbExam.SelectedItem is JHExamRecord))
            {
                lvSubject.Items.Clear();
                lvDomain.Items.Clear();
                btnPrint.Enabled = false;
                return;
            }

            Loading = true;
            Utilities.DisableControls(this);
            lvSubject.Items.Clear();
            lvDomain.Items.Clear();
            Options.SchoolYear = Semester.SelectedSchoolYear;
            Options.Semester = Semester.SelectedSemester;
            Options.Exam = cbExam.SelectedItem as JHExamRecord;
            Options.ScoreSource = (string)cbScore.SelectedItem;

            if (MasterWorker.IsBusy)
                WorkPadding = true;
            else
                MasterWorker.RunWorkerAsync();
        }

        private void MasterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (WorkPadding)
            {
                MasterWorker.RunWorkerAsync();
                WorkPadding = false;
                return;
            }

            Loading = false;
            Utilities.EnableControls(this);

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }

            lvDomain.FillItems(Utilities.DomainNames, "領域");
            lvSubject.FillItems(Subjects, "科目");
        }

        private void MasterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //1.Goup By 可選擇的科目清單。
            //2.寫入成績資料到 ReportStudent 上。

            // TODO: 改這裡
            //List<JHSemesterScoreRecord> semsScores = JHSemesterScore.SelectBySchoolYearAndSemester(AllStudents.ToKeys(), Semester.SelectedSchoolYear, Semester.SelectedSemester);
            List<InternalExamScoreRecord> examScores = InternalExamScore.Select(AllStudents.ToKeys(), Options);

            //GroupBySubjects(semsScores);
            GroupBySubjects(examScores);

            //先把學生身上的成績、排名相關資料清掉。
            foreach (ReportStudent each in AllStudents)
            {
                each.Clear();
                each.Scores.Add(Utilities.SubjectToken, new ScoreCollection()); //科目成績。
                each.Scores.Add(Utilities.DomainToken, new ScoreCollection());  //領域成績。
                each.Scores.Add(Utilities.SummaryToken, new ScoreCollection());   //運算後的成績。
            }

            //將成績填到學生身上。
            Dictionary<string, ReportStudent> dicAllStudent = AllStudents.ToDictionary();
            // TODO: 這裡
            //foreach (JHSemesterScoreRecord eachScore in semsScores)
            foreach (InternalExamScoreRecord eachScore in examScores)
            {
                //如果找不到該學生，跳到下一筆。
                if (!dicAllStudent.ContainsKey(eachScore.RefStudentID)) continue;

                ReportStudent student = dicAllStudent[eachScore.RefStudentID];

                //科目成績。
                foreach (SubjectScore each in eachScore.Subjects.Values)
                {
                    if (!each.Score.HasValue) continue; //沒有分數不處理。
                    if (!each.Credit.HasValue || each.Credit.Value <= 0) continue;  //沒有節數不處理。

                    if (!student.Scores[Utilities.SubjectToken].Contains(each.Subject))
                        student.Scores[Utilities.SubjectToken].Add(each.Subject, each.Score.Value, each.Credit.Value);
                }

                //領域成績。
                foreach (DomainScore each in eachScore.Domains.Values)
                {
                    if (!each.Score.HasValue) continue;
                    if (!each.Credit.HasValue || each.Credit.Value <= 0) continue;

                    student.Scores[Utilities.DomainToken].Add(each.Domain, each.Score.Value, each.Credit.Value);
                }

                //運算後成績是在使用者按下列印時才計算。
                //因為需要依據使用者選擇的科目進行計算。
            }
        }

        /// <summary>
        /// Group By 科目清單，儲存於 Subjects  變數中。
        /// (限制在已選擇的學生有修的才算)。
        /// </summary>
        /// <param name="semsScores"></param>
        //private void GroupBySubjects(List<JHSemesterScoreRecord> semsScores)
        private void GroupBySubjects(List<InternalExamScoreRecord> examScores)
        {
            SubjectDomainMap = new Dictionary<string, string>();
            Dictionary<string, string> groupBySubject = new Dictionary<string, string>();

            //Group By 可選擇的科目清單。
            Dictionary<string, ReportStudent> dicSelectedStudents = JHStudent.SelectByClassIDs(
                SelectedClasses.ToKeys()).ToReportStudent().ToDictionary();

            // TODO: 這裡
            //foreach (JHSemesterScoreRecord eachScore in semsScores)
            foreach (InternalExamScoreRecord eachScore in examScores)
            {
                //如果成績不屬於已選擇的學生之中，就跳到下一筆。
                if (!dicSelectedStudents.ContainsKey(eachScore.RefStudentID)) continue;

                foreach (SubjectScore each in eachScore.Subjects.Values)
                {
                    if (!groupBySubject.ContainsKey(each.Subject))
                    {
                        groupBySubject.Add(each.Subject, null);
                        SubjectDomainMap.Add(each.Subject, each.Domain);
                    }
                }
            }

            //所有可選擇的科目。
            Subjects = new List<string>(groupBySubject.Keys);
            Subjects.Sort(new Comparison<string>(SubjectSorter.Sort));
        }

        /// <summary>
        /// 當使用者選的學年度學期變更時。
        /// </summary>
        private void Semester_SemesterChanged(object sender, EventArgs e)
        {
            FillCurrentSemesterData();
        }

        private void lnConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (new ConfigForm().ShowDialog() == DialogResult.OK)
                Perference = new ReportPreference(); //重新 New 一次就會重新讀取設定。
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            List<string> selectedSubjects = lvSubject.GetSelectedItems();
            List<string> selectedDomains = lvDomain.GetSelectedItems();

            #region 檢查選擇的科目、領域是否合理。
            //if ((selectedSubjects.Count + selectedDomains.Count + Perference.PrintItems.Count) > Report.ScoreHeaderCount)
            int ScoreHeaderCount = (Perference.PaperSize == "B4") ? 32 : 18;
            if ((selectedSubjects.Count + selectedDomains.Count + Perference.PrintItems.Count) > ScoreHeaderCount)
            {
                MsgBox.Show("選擇的成績項目超過，無法列印報表。");
                return;
            }
            #endregion

            #region 重設計算資料
            foreach (ReportStudent each in AllStudents)
            {
                each.Scores[Utilities.SummaryToken].Clear();
                each.Places.Clear();
            }

            #endregion

            #region 計算各類成績。
            ScoreCalculator calculator = new ScoreCalculator(2, Utilities.SummaryToken);
            calculator.Subjects = selectedSubjects;
            calculator.Domains = selectedDomains;

            foreach (ReportStudent each in AllStudents)
                calculator.CalculateScore(each);
            #endregion

            #region 計算排名。
            List<RatingScope<ReportStudent>> classScopes = AllStudents.ToClassScopes();
            List<RatingScope<ReportStudent>> gyScopes = AllStudents.ToGradeYearScopes();

            List<ScoreParser> parsers = new List<ScoreParser>(new ScoreParser[]{
                new ScoreParser("加權平均", Utilities.SummaryToken),
                new ScoreParser("加權總分", Utilities.SummaryToken),
                new ScoreParser("合計總分", Utilities.SummaryToken),
                new ScoreParser("算術平均", Utilities.SummaryToken),
            });

            foreach (ScoreParser parser in parsers)
            {
                foreach (RatingScope<ReportStudent> Scope in classScopes)
                    Scope.Rank(parser, PlaceOptions.Unsequence);

                foreach (RatingScope<ReportStudent> Scope in gyScopes)
                    Scope.Rank(parser, PlaceOptions.Unsequence);
            }

            #endregion

            #region 輸出到 Excel
            Report report = new Report();
            report.Perference = Perference;
            report.SchoolName = School.ChineseName;
            report.SchoolYear = Semester.SelectedSchoolYear.ToString();
            report.Semester = Semester.SelectedSemester.ToString();
            report.Classes = SelectedClasses;
            report.AllStudents = AllStudents.ToDictionary();
            report.SelectedSubject = selectedSubjects;
            report.SelectedDomain = selectedDomains;
            report.SubjectDomainMap = SubjectDomainMap;
            report.ExamName = Options.Exam.Name;

            Utilities.DisableControls(this);
            BackgroundWorker OutputWork = new BackgroundWorker();
            OutputWork.DoWork += delegate(object sender1, DoWorkEventArgs e1)
            {
                e1.Result = report.Output();
            };
            OutputWork.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
            {
                if (e1.Error != null)
                    MsgBox.Show(e1.Error.Message);
                else
                    Utilities.Save(e1.Result as Aspose.Cells.Workbook, "班級評量成績單.xls");

                Utilities.EnableControls(this);
            };
            OutputWork.RunWorkerAsync(report);

            #endregion
        }

        private bool Loading
        {
            set
            {
                LoadingSubject.Visible = value;
                LoadingDomain.Visible = value;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillCurrentSemesterData();
        }

        private void cbScore_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillCurrentSemesterData();
        }
    }
}
