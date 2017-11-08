using System;
using System.Collections.Generic;
using System.Windows.Forms;
using JHSchool.Data;
using Campus.Rating;
using Campus.Windows;
using System.Drawing;
using Campus;
using JHSchool.Evaluation;

namespace JHEvaluation.Rating
{
    public partial class FormExamRating : FormRating
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
        /// 在畫面上已選擇的科目或領域清單。
        /// </summary>
        private ItemWeightCollection SelectedItems { get; set; }

        /// <summary>
        /// 提供學年度、學期的資訊。
        /// </summary>
        private SemesterSelector Sems;

        /// <summary>
        /// 預設的成績識別字串。
        /// </summary>
        private string Token = Tokens.Default;

        /// <summary>
        /// 用於顯示錯誤於畫面上。
        /// </summary>
        private ErrorProvider error = new ErrorProvider();

        /// <summary>
        /// 用於領域名稱的對應，讓畫面上看到的與實際成績的名稱可以有些不同。
        /// Key：畫面上看到的，Value：實際的成績。
        /// </summary>
        private Dictionary<string, string> NameMapping = new Dictionary<string, string>();

        public FormExamRating()
            : base()
        {
            InitializeComponent();
            Courses = new Dictionary<string, JHCourseRecord>();
            SCAttends = new Dictionary<string, JHSCAttendRecord>();

            // TODO: 特教改
            //NameMapping.Add("語文", "語文");
            //NameMapping.Add("數學", "數學");
            //NameMapping.Add("社會", "社會");
            //NameMapping.Add("自然與生活科技", "自然與生活科技");
            //NameMapping.Add("藝術與人文", "藝術與人文");
            //NameMapping.Add("健康與體育", "健康與體育");
            //NameMapping.Add("綜合活動", "綜合活動");
            foreach (string domain in Domain.SelectGeneral())
                NameMapping.Add(domain, domain);
            foreach (string domain in Domain.SelectSpecial())
                NameMapping.Add(domain, domain);
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

            int t1 = Environment.TickCount;
            IncludeQuery = new AEIncludeQuery();
            Console.WriteLine(string.Format("new AEIncludeQuery() Time：{0}", Environment.TickCount - t1));

            t1 = Environment.TickCount;
            if (cboExam.Items.Count > 0) cboExam.SelectedIndex = 0;
            Console.WriteLine(string.Format("cboExam.SelectedIndex Time：{0}", Environment.TickCount - t1));

            RatingUtils.EnableControls(this);
        }

        private void Sems_SemesterChanged(object sender, EventArgs e)
        {
            FillCurrentSemesterData(); //準備目前學年度學期需要的資料。

            if (ValidateExamControl()) RefreshDataAndOptions();
        }

        #region 畫面驗證邏輯。
        private void cboExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateExamControl()) RefreshDataAndOptions();
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

        private string CurrentSemester = string.Empty;
        private void RefreshDataAndOptions()
        {
            if (Sems.ValidateControlContent())
            {
                if (ValidateExamControl())
                {
                    if (CurrentSemester != Sems.SelectedSchoolYear + ":" + Sems.SelectedSemester)
                    {
                        int t1 = Environment.TickCount;
                        FillCurrentSemesterData();
                        Console.WriteLine(string.Format("FillCurrentSemesterData Time：{0}", Environment.TickCount - t1));
                    }

                    CurrentSemester = Sems.SelectedSchoolYear + ":" + Sems.SelectedSemester;

                    int t2 = Environment.TickCount;
                    FillItemOptions();
                    Console.WriteLine(string.Format("FillItemOptions Time：{0}", Environment.TickCount - t2));
                }
            }
        }

        private void txtTopRank_TextChanged(object sender, EventArgs e)
        {
            rbTopRank.Checked = true;
            ValidateTopRankText();
        }

        private bool ValidateLastRankText()
        {
            error.SetError(txtLastRank, "");
            int v;
            if (rbLastRank.Checked && !int.TryParse(txtLastRank.Text, out v) && v < 1)
            {
                error.SetError(txtLastRank, "名次請輸入大於 1 的數字。");
                return false;
            }
            return true;
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

        private bool ValidateLastPercentageText()
        {
            error.SetError(txtLastPercentage, "");
            decimal v;
            if (rbLastPercentage.Checked && !decimal.TryParse(txtLastPercentage.Text, out v) && (v <= 0 || v > 100))
            {
                error.SetError(txtLastPercentage, "百分比請輸入 0 到 100 之間的數字。");
                return false;
            }

            return true;
        }


        private bool ValidateDataGrid()
        {
            return RefreshSelectedItems();
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
            if (!ValidateLastRankText()) return;
            if (!ValidatePercentageText()) return;
            if (!ValidateLastPercentageText()) return;
            if (!ValidateDataGrid()) return;

            RefreshSelectedItems();
            if (SelectedItems.Count <= 0) return;

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
            FunctionSpliter<string, JHSCETakeRecord> selectData = new FunctionSpliter<string, JHSCETakeRecord>(500, 5);
            selectData.Function = delegate(List<string> ps)
            {
                return JHSCETake.SelectByCourseAndExam(ps, ExamId);
            };
            List<JHSCETakeRecord> scetakes = selectData.Execute(Courses.Values.ToKeys());

            JHClass.RemoveAll();
            RatingStudent.SetClassMapping(JHClass.SelectAll()); //快取班級對照資訊。
            Dictionary<string, RatingStudent> dicstuds = Students.ToDictionary();

            //將學生的成績清除，並新增一個 Token 用來放成績。
            foreach (RatingStudent each in Students)
            {
                each.Clear();
                each.Scores.Add(ScoreType.Domain.Regulation(Token), new ScoreCollection());
                each.Scores.Add(ScoreType.Subject.Regulation(Token), new ScoreCollection());
                each.Scores.Add(ScoreType.SummaryDomain.Regulation(Token), new ScoreCollection());
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

                ScoreItem subject = new ScoreItem(course.Subject.Trim(), ScoreType.Subject);
                ScoreItem domain = new ScoreItem(course.Domain.Trim(), ScoreType.Domain);

                //--- Subject
                ProcessSubject(subject, student, take);

                //--- Domain
                ProcessDomain(domainscores, take, course, student, subject, domain);
            }

            foreach (KeyValuePair<string, DomainScoreCalculator> eachStudent in domainscores)
            {
                string studentId = eachStudent.Key; //學生編號。
                DomainScoreCalculator calculator = eachStudent.Value; //領域成績。

                foreach (DomainScore eachDomain in calculator.Domains)
                    dicstuds[studentId].Scores[ScoreType.Domain.Regulation(Token)].Add(eachDomain.Name, eachDomain.GetScore());
            }
        }

        private void ProcessDomain(Dictionary<string, DomainScoreCalculator> domainscores,
            JHSCETakeRecord take,
            JHCourseRecord course,
            RatingStudent student,
            ScoreItem subject,
            ScoreItem domain)
        {
            if (!course.Credit.HasValue) return; //如果課程沒有權重就不處理。

            decimal weight = course.Credit.Value;

            if (!SelectedItems.Contains(domain))
                return; //不在處理的領域清單中。

            if (!domainscores.ContainsKey(student.Id))
                domainscores.Add(student.Id, new DomainScoreCalculator());

            if (domainscores[student.Id].Contains(subject.Name))
                throw new ArgumentException(string.Format("學生「{0}」在同一學期修習「{1}」科目一次以上。", student.Name, subject.Name));

            domainscores[student.Id].Add(domain.Name, subject.Name, take.Score.Value, weight);
        }

        private void ProcessSubject(ScoreItem subject, RatingStudent student, JHSCETakeRecord take)
        {
            ScoreCollection subjScores = student.Scores[subject.Regulation(Token)];

            if (!SelectedItems.Contains(subject))
                return; //不在處理的科目清單中。

            if (subjScores.Contains(subject.Name))
                throw new ArgumentException(string.Format("學生「{0}」在同一學期修習「{1}」科目一次以上。", student.Name, subject.Name));

            //take.Score  一定有值，因為在很前面就處理掉了。
            subjScores.Add(subject.Name, take.Score.Value);
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
            List<ScoreParser> parsers = SelectedItems.Keys.ToScoreParsers(Token);
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
                // 排名
                param.Mode = FilterMode.Place;                
                    param.Top = int.Parse(txtTopRank.Text);                
            }
            else if(rbLastRank.Checked)
            {
                // 後排名
                param.Mode = FilterMode.PlaceL;
                param.Last = int.Parse(txtLastRank.Text);
            }
            else if (rbPercentage.Checked)
            {
                // 排名百分比
                param.Mode = FilterMode.Percentage;
                param.Top = int.Parse(txtPercentage.Text);
            }
            else if (rbLastPercentage.Checked)
            {
                // 後排名百分比
                param.Mode = FilterMode.PercentageL;
                param.Last = int.Parse(txtLastPercentage.Text);
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

            return SelectedItems.ToCalcScoreParser(name, method, round, Token);
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
            int t1 = Environment.TickCount;
            List<JHCourseRecord> listCourses = JHCourse.SelectBySchoolYearAndSemester(Sems.SelectedSchoolYear, Sems.SelectedSemester);
            Console.WriteLine(string.Format("JHCourse.SelectBySchoolYearAndSemester Time：{0}", Environment.TickCount - t1));

            if (listCourses.Count <= 0) return; //沒有任何課程也不會有成績資料了。

            #region 建立 Course Dictionary
            foreach (JHCourseRecord each in listCourses)
                Courses.Add(each.ID, each);
            #endregion

            //Network Access
            t1 = Environment.TickCount;
            List<JHSCAttendRecord> listSCAttends = JHSCAttend.SelectByStudentIDAndCourseID(Students.ToKeys(), Courses.Values.ToKeys());

            Console.WriteLine(string.Format("JHSCAttend.SelectByStudentIDAndCourseID Time：{0}", Environment.TickCount - t1));

            t1 = Environment.TickCount;
            #region 建立 SCAttend Dictionary
            int selSchoolYear = Sems.SelectedSchoolYear;
            int selSemester = Sems.SelectedSemester;
            foreach (JHSCAttendRecord each in listSCAttends)
            {
                JHCourseRecord course = Courses[each.RefCourseID];
                if (course.SchoolYear != selSchoolYear || course.Semester != selSemester)
                    continue;

                SCAttends.Add(each.ID, each);
            }
            #endregion
            Console.WriteLine(string.Format("建立 SCAttend Dictionary Time：{0}", Environment.TickCount - t1));
        }

        private void FillItemOptions()
        {
            if (!Sems.ValidateControlContent()) return;

            dgv.SuspendLayout();
            dgv.Rows.Clear();

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

            foreach (string eachSubj in subjects)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, false, subjectWeight[eachSubj], eachSubj, subjectCount[eachSubj]);
                row.Tag = new ScoreItem(eachSubj, ScoreType.Subject);
                dgv.Rows.Add(row);
            }
            #endregion

            #region 列出領域名稱。
            foreach (string each in NameMapping.Keys)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, false, 1, each);
                row.Cells[chScoreItem.Index].Style.ForeColor = Color.Blue;
                row.Tag = new ScoreItem(each, ScoreType.Domain);
                dgv.Rows.Add(row);
            }
            #endregion

            dgv.ResumeLayout();
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
        private bool RefreshSelectedItems()
        {
            bool result = true;
            SelectedItems = new ItemWeightCollection();
            foreach (DataGridViewRow each in dgv.Rows)
            {
                if (each.IsNewRow) continue;
                string value = "" + each.Cells[chCheck.Index].Value;
                bool b;
                if (bool.TryParse(value, out b) && b == true)
                {
                    string domain = "" + each.Cells[chScoreItem.Index].Value;
                    string strPeriod = "" + each.Cells[chPeriod.Index].Value;
                    decimal period;

                    if (decimal.TryParse(strPeriod, out period) && period > 0)
                    {
                        each.Cells[chPeriod.Index].ErrorText = string.Empty;
                        ScoreItem item = (ScoreItem)each.Tag;
                        SelectedItems.Add(item, period);
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

        private void txtLastRank_TextChanged(object sender, EventArgs e)
        {
            rbLastRank.Checked = true;
            ValidateLastRankText();
        }

        private void txtLastPercentage_TextChanged(object sender, EventArgs e)
        {
            rbLastPercentage.Checked = true;
            ValidateLastPercentageText();
        }
    }
}
