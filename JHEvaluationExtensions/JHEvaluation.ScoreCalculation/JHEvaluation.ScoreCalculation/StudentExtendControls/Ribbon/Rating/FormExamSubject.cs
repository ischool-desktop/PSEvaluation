using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Framework;
using JHSchool.Data;
using JHSchool.Evaluation.RatingFramework;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
{
    public partial class FormExamSubject : FormRating
    {
        /// <summary>
        /// 已選擇的學期下所有課程。
        /// </summary>
        private Dictionary<string, JHCourseRecord> Courses { get; set; }

        /// <summary>
        /// 已選擇的學期下指定學生的所有修課記錄。
        /// </summary>
        private Dictionary<string, JHSCAttendRecord> SCAttends { get; set; }

        /// <summary>
        /// 用於查詢某個評量設定是否包含了特定的試別。
        /// </summary>
        private AEIncludeQuery IncludeQuery { get; set; }

        /// <summary>
        /// 已選擇的科目清單。
        /// </summary>
        private ItemWeightCollection SelectedSubjects { get; set; }

        /// <summary>
        /// 提供學年度、學期的資訊。
        /// </summary>
        private SemesterSelector Sems;

        /// <summary>
        /// 預設的成績識別字串。
        /// </summary>
        private string ThisToken = Token.Default;

        /// <summary>
        /// 用於顯示錯誤於畫面上。
        /// </summary>
        private ErrorProvider error = new ErrorProvider();

        public FormExamSubject()
            : base()
        {
            InitializeComponent();
            Courses = new Dictionary<string, JHCourseRecord>();
            SCAttends = new Dictionary<string, JHSCAttendRecord>();
        }

        private void FormExamSubject_Load(object sender, EventArgs e)
        {
            //if (Site.DesignMode) return;

            RatingUtils.DisableControls(this);
            RatingUtils.SetSemesterDefaultItems(cboSchoolYear, cboSemester);
            RatingUtils.SetExamDefaultItems(cboExam);
            Sems = new SemesterSelector(cboSchoolYear, cboSemester);
            Sems.SemesterChanged += new EventHandler(Sems_SemesterChanged);
            IncludeQuery = new AEIncludeQuery();

            if (cboExam.Items.Count > 0) cboExam.SelectedIndex = 0;

            RatingUtils.EnableControls(this);
        }

        private void Sems_SemesterChanged(object sender, EventArgs e)
        {
            RefreshSubjectOptions();
        }

        #region 畫面驗證邏輯。
        private void cboExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateExamControl())
                RefreshSubjectOptions();
        }

        private bool ValidateExamControl()
        {
            error.SetError(cboExam, "");

            if (string.IsNullOrEmpty(ExamId))
            {
                error.SetError(cboExam, "請選擇評量。");
                return false;
            }
            return true;
        }

        private void txtTopRank_TextChanged(object sender, EventArgs e)
        {
            rbTopRank.Checked = true;
            ValidateTopRankText();
        }

        private bool ValidateTopRankText()
        {
            error.SetError(txtTopRank, "");
            int v;
            if (rbTopRank.Checked && !int.TryParse(txtTopRank.Text, out v) && v < 1)
            {
                error.SetError(txtTopRank, "名次請輸入大於 1 的數字。");
                return false;
            }
            return true;
        }

        private void txtPercentage_TextChanged(object sender, EventArgs e)
        {
            rbPercentage.Checked = true;
            ValidatePercentageText();
        }

        private bool ValidatePercentageText()
        {
            error.SetError(txtPercentage, "");
            decimal v;
            if (rbPercentage.Checked && !decimal.TryParse(txtPercentage.Text, out v) && (v <= 0 || v > 100))
            {
                error.SetError(txtPercentage, "百分比請輸入 0 到 100 之間的數字。");
                return false;
            }

            return true;
        }

        private bool ValidateDataGrid()
        {
            return RefreshSelectedSubjects();
        }

        #endregion

        private void btnRank_Click(object sender, EventArgs e)
        {
            if (Courses.Count <= 0)
            {
                MsgBox.Show("無任何成績資料可排名。");
                return; //沒有課程不進行排名。
            }
            if (!Sems.ValidateControlContent()) return;   //學期資訊不正確不進行排名。
            if (!ValidateExamControl()) return;   //試別資訊不正確不進行排名。
            if (!ValidateTopRankText()) return;
            if (!ValidatePercentageText()) return;
            if (!ValidateDataGrid()) return;

            RefreshSelectedSubjects();
            if (SelectedSubjects.Count <= 0) return;

            RatingUtils.DisableControls(this);
            PrepareData();
        }

        protected override void DisplaySelectedStudentCount()
        {
            lblSelectedCount.Text = string.Format("已選擇人數：{0}", Students.Count);
        }

        #region 準備成績資料。
        protected override void PrepareDataBackground()
        {
            List<JHSCETakeRecord> scetakes = JHSCETake.SelectByCourseAndExam(Courses.Values.ToKeys(), ExamId);
            JHClass.RemoveAll();
            RatingStudent.SetClassMapping(JHClass.SelectAll()); //快取班級對照資訊。
            Dictionary<string, RatingStudent> dicstuds = Students.ToDictionary();

            //將學生的成績清除，並新增一個 Token 用來放成績。
            foreach (RatingStudent each in Students)
            {
                each.Clear();
                each.Scores.Add(ThisToken, new ScoreCollection());
            }

            foreach (JHSCETakeRecord take in scetakes) //特別注意 take.Score 是 Extensions 裡面的 Score 不是實體欄位的 Score。
            {
                if (!take.Score.HasValue) continue; //沒有成績就不處理。

                JHSCAttendRecord attend;
                JHCourseRecord course;
                RatingStudent student;

                if (!SCAttends.TryGetValue(take.RefSCAttendID, out attend))
                    continue;  //找不到該修課記錄。

                if (!Courses.TryGetValue(take.RefCourseID, out course))
                    continue;  //找不到該課程。

                if (!dicstuds.TryGetValue(attend.RefStudentID, out student))
                    continue; //找不到該學生。

                ScoreCollection scores = student.Scores[ThisToken];
                string subject = course.Subject;

                if (!SelectedSubjects.Contains(subject))
                    continue; //不在處理的科目清單中。

                if (scores.Contains(subject))
                    throw new ArgumentException(string.Format("學生「{0}」在同一學期修習「{1}」科目一次以上。", student.Name, subject));

                scores.Add(subject, take.Score.Value);
            }
        }
        #endregion

        protected override void PrepareDataComplete(Exception error)
        {
            RatingUtils.EnableControls(this);

            if (error != null)
            {
                MsgBox.Show(error.Message);
                return;
            }

            CalculationScoreParser calcparser = rbCombine.Checked ? GetCalculationScoreParser() : null;
            List<ScoreParser> parsers = SelectedSubjects.Keys.ToScoreParsers(ThisToken);
            List<RatingScope<RatingStudent>> scopes = ToScopes();

            //分別排名。
            foreach (RatingScope<RatingStudent> eachScope in scopes)
            {
                foreach (ScoreParser eachParser in parsers)
                    eachScope.Rank(eachParser, PlaceOptions.Unsequence);
            }

            if (calcparser != null)
            {
                //運算排名。
                foreach (RatingScope<RatingStudent> eachScope in scopes)
                    eachScope.Rank(calcparser, PlaceOptions.Unsequence);
            }

            FilterParameter param = new FilterParameter();
            if (rbAllRank.Checked)
                param.Mode = FilterMode.None;
            else if (rbTopRank.Checked)
            {
                param.Mode = FilterMode.Place;
                param.Top = int.Parse(txtTopRank.Text);
            }
            else if (rbPercentage.Checked)
            {
                param.Mode = FilterMode.Percentage;
                param.Top = int.Parse(txtPercentage.Text);
            }
            else throw new ArgumentException("無此種過慮排名的方式。");

            if (rbSeparate.Checked) //分別排名。
            {
                if (rbAllRank.Checked) //如果是列出全部名次的話。
                    new OutputSeparatePivot().Output(scopes, parsers.ToNameList()); //分別列出科目、名次(Pivot 列法)。
                else
                    new OutputSeparate().Output(scopes, parsers.ToNameList(), param); //分別列出科目、名次(非 Pivot 列法)。
            }
            else
                new OutputCalculationPivot().Output(scopes, parsers, calcparser.Name, param); //列出運算名次、運算前成績。
        }

        /// <summary>
        /// 依據畫面設定，建立運算成績解析類別。
        /// </summary>
        /// <returns></returns>
        private CalculationScoreParser GetCalculationScoreParser()
        {
            string name = "";
            int round = integerInput1.Value;
            CalculationScoreParser.CalcMethod method;

            if (rbWeightAverage.Checked)
            {
                name = "加權平均";
                method = CalculationScoreParser.CalcMethod.加權平均;
            }
            else if (rbWeightTotal.Checked)
            {
                name = "加權總分";
                method = CalculationScoreParser.CalcMethod.加權總分;
            }
            else if (rbAverage.Checked)
            {
                name = "算術平均";
                method = CalculationScoreParser.CalcMethod.算術平均;
            }
            else
            {
                name = "合計總分";
                method = CalculationScoreParser.CalcMethod.合計總分;
            }

            return SelectedSubjects.ToCalcScoreParser(name, method, round, ThisToken);
        }

        private string CurrentSemester = string.Empty;
        private void RefreshSubjectOptions()
        {
            if (Sems.ValidateControlContent())
            {
                if (ValidateExamControl())
                {
                    if (CurrentSemester != Sems.SelectedSchoolYear + ":" + Sems.SelectedSemester)
                        FillCurrentSemesterData();

                    CurrentSemester = Sems.SelectedSchoolYear + ":" + Sems.SelectedSemester;

                    FillSubjectOptions();
                }
            }
        }

        /// <summary>
        /// (Network Access)將目前學期的資料填入到 Courses、SCAttends 變數中。
        /// </summary>
        private void FillCurrentSemesterData()
        {
            Courses = new Dictionary<string, JHCourseRecord>();
            SCAttends = new Dictionary<string, JHSCAttendRecord>();

            if (Students.Count <= 0) return; //沒有學生當然就沒有任何資料了。

            //Network Access
            List<JHCourseRecord> listCourses = JHCourse.SelectBySchoolYearAndSemester(Sems.SelectedSchoolYear, Sems.SelectedSemester);

            if (listCourses.Count <= 0) return; //沒有任何課程也不會有成績資料了。

            #region 建立 Course Dictionary
            foreach (JHCourseRecord each in listCourses)
                Courses.Add(each.ID, each);
            #endregion

            //Network Access
            List<JHSCAttendRecord> listSCAttends = JHSCAttend.SelectByStudentIDAndCourseID(Students.ToKeys(), Courses.Values.ToKeys());

            #region 建立 SCAttend Dictionary
            foreach (JHSCAttendRecord each in listSCAttends)
            {
                JHCourseRecord course = Courses[each.RefCourseID];
                if (course.SchoolYear != Sems.SelectedSchoolYear || course.Semester != Sems.SelectedSemester)
                    continue;

                SCAttends.Add(each.ID, each);
            }
            #endregion
        }

        private void FillSubjectOptions()
        {
            if (!Sems.ValidateControlContent()) return;

            #region 讀取修課記錄，並群組出科目項目。
            string examid = ExamId;
            List<string> studentIDs = Students.ToKeys();
            Dictionary<string, int> subjectCount = new Dictionary<string, int>();
            Dictionary<string, decimal> subjectWeight = new Dictionary<string, decimal>();

            foreach (JHSCAttendRecord record in SCAttends.Values)
            {
                JHCourseRecord course = Courses[record.RefCourseID];

                if (!IncludeQuery.Contains(course.RefAssessmentSetupID, examid)) continue;

                string subject = course.Subject.Trim();

                //course.RefAssessmentSetupID
                //JHAssessmentSetup.SelectByID("").ID
                //JHAEInclude.SelectByAssessmentSetupID("")[0].ExamName

                //隨機取得一個課程的 Credit 當作是 Subject 的比重。
                if (!subjectWeight.ContainsKey(subject))
                {
                    subjectWeight.Add(subject, 0);
                    subjectWeight[subject] = course.Credit.HasValue ? course.Credit.Value : 0;
                }

                //計算科目的修習人數。
                if (!subjectCount.ContainsKey(subject))
                    subjectCount.Add(subject, 0);
                subjectCount[subject]++;
            }

            List<string> subjects = new List<string>(subjectCount.Keys);
            subjects.Sort(new Comparison<string>(SubjectSorter.Sort));

            dgv.SuspendLayout();
            dgv.Rows.Clear();
            foreach (string eachSubj in subjects)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, false, subjectWeight[eachSubj], eachSubj, subjectCount[eachSubj]);
                dgv.Rows.Add(row);
            }
            dgv.ResumeLayout();
            #endregion
        }

        /// <summary>
        /// 將資料群組成特定範圍，以方便排名。
        /// </summary>
        /// <returns></returns>
        private List<RatingScope<RatingStudent>> ToScopes()
        {
            if (rbClass.Checked)
                return Students.ToClassScopes();
            else
                return Students.ToGradeYearScopes();
        }

        private string ExamId
        {
            get { return RatingUtils.GetExamId(cboExam); }
        }

        /// <summary>
        /// 把使用者已選擇的科目資訊(權重)更新到 SelectedSubjects 變數。
        /// </summary>
        /// <returns>true 為資料正確沒有發生任何錯誤。</returns>
        private bool RefreshSelectedSubjects()
        {
            bool result = true;
            SelectedSubjects = new ItemWeightCollection();
            foreach (DataGridViewRow each in dgv.Rows)
            {
                if (each.IsNewRow) continue;
                string value = "" + each.Cells[chCheck.Index].Value;
                bool b;
                if (bool.TryParse(value, out b) && b == true)
                {
                    string subject = "" + each.Cells[chSubject.Index].Value;
                    string strPeriod = "" + each.Cells[chPeriod.Index].Value;
                    decimal period;

                    if (decimal.TryParse(strPeriod, out period) && period > 0)
                    {
                        each.Cells[chPeriod.Index].ErrorText = string.Empty;
                        SelectedSubjects.Add(subject, period);
                    }
                    else
                    {
                        each.Cells[chPeriod.Index].ErrorText = "計算比例必須是大於 0 的數字。";
                        result = false;
                    }
                }
            }

            return result;
        }

        private string CurrentCalculation
        {
            get
            {
                if (rbWeightAverage.Checked)
                    return "加權平均";
                else if (rbWeightTotal.Checked)
                    return "加權總分";
                else if (rbAverage.Checked)
                    return "算術平均";
                else //(rbTotal.Checked)
                    return "合計總分";
            }
        }

        private void rbSeparate_CheckedChanged(object sender, EventArgs e)
        {
            peCalcMethod.Enabled = rbCombine.Checked;
        }
    }
}
