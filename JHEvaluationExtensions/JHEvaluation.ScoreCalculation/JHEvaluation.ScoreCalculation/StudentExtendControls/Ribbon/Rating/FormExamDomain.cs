using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Framework;
using JHSchool.Data;
using JHSchool.Evaluation.RatingFramework;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
{
    public partial class FormExamDomain : FormRating
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
        /// 在畫面上已選擇的科目或領域清單。
        /// </summary>
        private ItemWeightCollection SelectedDomains { get; set; }

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

        /// <summary>
        /// 用於領域名稱的對應，讓畫面上看到的與實際成績的名稱可以有些不同。
        /// Key：畫面上看到的，Value：實際的成績。
        /// </summary>
        private Dictionary<string, string> NameMapping = new Dictionary<string, string>();

        public FormExamDomain()
            : base()
        {
            InitializeComponent();
            Courses = new Dictionary<string, JHCourseRecord>();
            SCAttends = new Dictionary<string, JHSCAttendRecord>();

            // TODO: 特教改
            NameMapping.Add("語文", "語文");
            NameMapping.Add("數學", "數學");
            NameMapping.Add("社會", "社會");
            NameMapping.Add("自然與生活科技", "自然與生活科技");
            NameMapping.Add("藝術與人文", "藝術與人文");
            NameMapping.Add("健康與體育", "健康與體育");
            NameMapping.Add("綜合活動", "綜合活動");
            //NameMapping.Add("學習領域總成績", "學習領域總成績");
            //NameMapping.Add("課程學習", "課程學習");
        }

        private void FormExamSubject_Load(object sender, EventArgs e)
        {
            //if (Site.DesignMode) return;

            RatingUtils.DisableControls(this);
            RatingUtils.SetSemesterDefaultItems(cboSchoolYear, cboSemester);
            RatingUtils.SetExamDefaultItems(cboExam);
            Sems = new SemesterSelector(cboSchoolYear, cboSemester);
            Sems.SemesterChanged += new EventHandler(Sems_SemesterChanged);
            FillDomainOptions();
            FillCurrentSemesterData(); //準備目前學年度學期需要的資料。

            if (cboExam.Items.Count > 0) cboExam.SelectedIndex = 0;

            RatingUtils.EnableControls(this);
        }

        private void Sems_SemesterChanged(object sender, EventArgs e)
        {
            //不用 FillDomainOptions();
            FillCurrentSemesterData(); //準備目前學年度學期需要的資料。
        }

        #region 畫面驗證邏輯。
        private void cboExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateExamControl();
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
            return RefreshSelectedDomains();
        }

        #endregion

        private void btnRank_Click(object sender, EventArgs e)
        {
            if (Courses.Count <= 0)
            {
                MsgBox.Show("無任何成績資料可排名。");
                return; //沒有課程資料就不排名了。
            }
            if (!Sems.ValidateControlContent()) return;   //學期資訊不正確不進行排名。
            if (!ValidateExamControl()) return;   //試別資訊不正確不進行排名。
            if (!ValidateTopRankText()) return;
            if (!ValidatePercentageText()) return;
            if (!ValidateDataGrid()) return;

            RefreshSelectedDomains();
            if (SelectedDomains.Count <= 0) return;

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

            //記錄每一位學生的領域成績。
            Dictionary<string, DomainScoreCalculator> domainscores = new Dictionary<string, DomainScoreCalculator>();

            //計算領域成績。
            foreach (JHSCETakeRecord take in scetakes) //特別注意 take.Score 是 Extensions 裡面的 Score 不是實體欄位的 Score。
            {
                if (!take.Score.HasValue) continue; //沒有成績就不處理。

                JHSCAttendRecord attend;
                JHCourseRecord course;
                RatingStudent student;

                if (!SCAttends.TryGetValue(take.RefSCAttendID, out attend))
                    continue;  //找不到該修課記錄。

                if (!dicstuds.TryGetValue(attend.RefStudentID, out student))
                    continue; //找不到該學生。

                if (!Courses.TryGetValue(take.RefCourseID, out course))
                    continue;  //找不到該課程。

                if (!course.Credit.HasValue) continue; //如果課程沒有權重就不處理。

                string domain = course.Domain.Trim();
                string subject = course.Subject.Trim();
                decimal weight = course.Credit.Value;

                if (!SelectedDomains.Contains(domain))
                    continue; //不在處理的領域清單中。

                if (!domainscores.ContainsKey(attend.RefStudentID))
                    domainscores.Add(attend.RefStudentID, new DomainScoreCalculator());

                if (domainscores[attend.RefStudentID].Contains(course.Subject.Trim()))
                    throw new ArgumentException(string.Format("學生「{0}」在同一學期修習「{1}」科目一次以上。", student.Name, subject));

                domainscores[attend.RefStudentID].Add(domain, subject, take.Score.Value, weight);
            }

            foreach (KeyValuePair<string, DomainScoreCalculator> eachStudent in domainscores)
            {
                string studentId = eachStudent.Key; //學生編號。
                DomainScoreCalculator calculator = eachStudent.Value; //領域成績。

                foreach (DomainScore eachDomain in calculator.Domains)
                    dicstuds[studentId].Scores[ThisToken].Add(eachDomain.Name, eachDomain.GetScore());
            }
        }

        #region 計算領域成績輔助類別。
        private class DomainScoreCalculator
        {
            private Dictionary<string, DomainScore> domains = new Dictionary<string, DomainScore>();
            private Dictionary<string, string> subjects = new Dictionary<string, string>();

            /// <summary>
            /// 科目不管是哪個領域，但是在同一學期都不能重覆。
            /// </summary>
            public bool Contains(string subjectName)
            {
                return subjects.ContainsKey(subjectName);
            }

            public void Add(string domainName, string subjectName, decimal score, decimal weight)
            {
                if (!domains.ContainsKey(domainName))
                    domains.Add(domainName, new DomainScore(domainName));

                domains[domainName].Add(subjectName, score, weight);
                subjects.Add(subjectName, null);
            }

            public IEnumerable<DomainScore> Domains
            {
                get { return domains.Values; }
            }
        }

        /// <summary>
        /// 代表特定領域的科目成績，裡面包含了成績與權重。
        /// </summary>
        internal class DomainScore
        {
            private Dictionary<string, decimal> scores = new Dictionary<string, decimal>();
            private Dictionary<string, decimal> weights = new Dictionary<string, decimal>();

            public DomainScore(string name)
            {
                Name = name;
            }

            public string Name { get; private set; }

            public void Add(string subjectName, decimal score, decimal weight)
            {
                scores.Add(subjectName, score);
                weights.Add(subjectName, weight);
            }

            /// <summary>
            /// 算領域成績。
            /// </summary>
            public decimal GetScore()
            {
                decimal subjSum = 0;
                decimal weightSum = 0;

                foreach (string subject in scores.Keys)
                {
                    subjSum += (scores[subject] * weights[subject]);
                    weightSum += weights[subject];
                }

                return Math.Round(subjSum / weightSum, 2, MidpointRounding.AwayFromZero);
            }
        }
        #endregion

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
            List<ScoreParser> parsers = SelectedDomains.Keys.ToScoreParsers(ThisToken);
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

            return SelectedDomains.ToCalcScoreParser(name, method, round, ThisToken);
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

        private void FillDomainOptions()
        {
            if (!Sems.ValidateControlContent()) return;

            #region 列出領域名稱。
            dgv.SuspendLayout();
            dgv.Rows.Clear();
            foreach (string each in NameMapping.Keys)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, false, 1, each);
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
        private bool RefreshSelectedDomains()
        {
            bool result = true;
            SelectedDomains = new ItemWeightCollection();
            foreach (DataGridViewRow each in dgv.Rows)
            {
                if (each.IsNewRow) continue;
                string value = "" + each.Cells[chCheck.Index].Value;
                bool b;
                if (bool.TryParse(value, out b) && b == true)
                {
                    string domain = "" + each.Cells[chSubject.Index].Value;
                    string strPeriod = "" + each.Cells[chPeriod.Index].Value;
                    decimal period;

                    if (decimal.TryParse(strPeriod, out period) && period > 0)
                    {
                        each.Cells[chPeriod.Index].ErrorText = string.Empty;
                        SelectedDomains.Add(domain, period);
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
