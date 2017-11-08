using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Framework;
using JHSchool.Data;
using JHSchool.Evaluation.RatingFramework;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
{
    public partial class FormSemesterDomain : FormRating
    {
        /// <summary>
        /// 已選擇的學期下所有學期成績。
        /// {學生編號,學期成績}
        /// </summary>
        private Dictionary<string, JHSemesterScoreRecord> SemesterScores { get; set; }

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

        public FormSemesterDomain()
            : base()
        {
            InitializeComponent();
            SemesterScores = new Dictionary<string, JHSemesterScoreRecord>();

            // TODO: 特教改
            NameMapping.Add("語文", "語文");
            NameMapping.Add("數學", "數學");
            NameMapping.Add("社會", "社會");
            NameMapping.Add("自然與生活科技", "自然與生活科技");
            NameMapping.Add("藝術與人文", "藝術與人文");
            NameMapping.Add("健康與體育", "健康與體育");
            NameMapping.Add("綜合活動", "綜合活動");
            NameMapping.Add("學習領域", "學習領域");
            NameMapping.Add("課程學習", "課程學習");
        }

        private void FormExamSubject_Load(object sender, EventArgs e)
        {
            //if (Site.DesignMode) return;

            RatingUtils.DisableControls(this);
            RatingUtils.SetSemesterDefaultItems(cboSchoolYear, cboSemester);
            Sems = new SemesterSelector(cboSchoolYear, cboSemester);
            Sems.SemesterChanged += new EventHandler(Sems_SemesterChanged);
            FillDomainOptions();
            FillCurrentSemesterData(); //準備目前學年度學期需要的資料。
            RatingUtils.EnableControls(this);
        }

        private void Sems_SemesterChanged(object sender, EventArgs e)
        {
            //不用 FillDomainOptions();
            FillCurrentSemesterData(); //準備目前學年度學期需要的資料。
        }

        #region 畫面驗證邏輯。
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
            if (SemesterScores.Count <= 0)
            {
                MsgBox.Show("無任何成績資料可排名。");
                return; //沒有任何學期成績不進行排名。
            }
            if (!Sems.ValidateControlContent()) return;   //學期資訊不正確不進行排名。
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
            JHClass.RemoveAll();
            RatingStudent.SetClassMapping(JHClass.SelectAll()); //快取班級對照資訊。
            Dictionary<string, RatingStudent> dicstuds = Students.ToDictionary();

            //將學生的成績清除，並新增一個 Token 用來放成績。
            foreach (RatingStudent each in Students)
            {
                each.Clear();
                each.Scores.Add(ThisToken, new ScoreCollection());
            }

            foreach (JHSemesterScoreRecord semsRecord in SemesterScores.Values)
            {
                RatingStudent student;

                if (!dicstuds.TryGetValue(semsRecord.RefStudentID, out student))
                    continue; //找不到該學生。

                ScoreCollection scores = student.Scores[ThisToken];

                foreach (K12.Data.DomainScore domainRecord in semsRecord.Domains.Values)
                {
                    if (!domainRecord.Score.HasValue) continue; //沒有成績就不處理。

                    string domain = domainRecord.Domain.Trim();

                    if (!SelectedDomains.Contains(domain))
                        continue; //不在處理的科目清單中。

                    if (!NameMapping.ContainsKey(domain))
                        throw new ArgumentException(string.Format("領域名稱「{0}」在此畫面沒有定議。", domain));

                    if (scores.Contains(domain))
                        throw new ArgumentException(string.Format("學生「{0}」在同一學期有「{1}」領域一次以上。", student.Name, domain));

                    scores.Add(domain, domainRecord.Score.Value);
                }

                if (semsRecord.LearnDomainScore.HasValue)
                    scores.Add("學習領域", semsRecord.LearnDomainScore.Value);

                if (semsRecord.CourseLearnScore.HasValue)
                    scores.Add("課程學習", semsRecord.CourseLearnScore.Value);
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
            SemesterScores = new Dictionary<string, JHSemesterScoreRecord>();
            List<JHSemesterScoreRecord> records = JHSemesterScore.SelectBySchoolYearAndSemester(Students.ToKeys(), Sems.SelectedSchoolYear, Sems.SelectedSemester);
            foreach (JHSemesterScoreRecord each in records)
            {
                if (SemesterScores.ContainsKey(each.RefStudentID))
                    throw new ArgumentException(string.Format("學生編號{0}，同一學期有兩筆成績？", each.RefStudentID));

                SemesterScores.Add(each.RefStudentID, each);
            }
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
