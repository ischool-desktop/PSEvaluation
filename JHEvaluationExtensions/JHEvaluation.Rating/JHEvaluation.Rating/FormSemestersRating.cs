using System;
using System.Collections.Generic;
using System.Windows.Forms;
using JHSchool.Data;
using FISCA.Presentation.Controls;
using Campus.Rating;
using GTokens = JHEvaluation.Rating.Tokens;
using JHSchool.Evaluation;
using System.Drawing;

namespace JHEvaluation.Rating
{
    public partial class FormSemestersRating : FormRating
    {
        /// <summary>
        /// 所有的學期成績。
        /// {成績編號,學期成績}
        /// </summary>
        private List<JHSemesterScoreRecord> SemesterScores { get; set; }

        /// <summary>
        /// 所有的學期歷程。
        /// </summary>
        private HistoryQuery SemesterHistory { get; set; }

        /// <summary>
        /// 已選擇的科目清單。
        /// </summary>
        private ItemWeightCollection SelectedItems { get; set; }

        /// <summary>
        /// 預設的成績識別字串。
        /// </summary>
        private List<string> Tokens = new List<string>();

        /// <summary>
        /// 用於領域名稱的對應，讓畫面上看到的與實際成績的名稱可以有些不同。
        /// Key：畫面上看到的，Value：實際的成績。
        /// </summary>
        private Dictionary<string, string> NameMapping = new Dictionary<string, string>();

        /// <summary>
        /// 用於顯示錯誤於畫面上。
        /// </summary>
        private ErrorProvider error = new ErrorProvider();

        public FormSemestersRating()
            : base()
        {
            InitializeComponent();
            SemesterScores = new List<JHSemesterScoreRecord>();
            SemesterHistory = new HistoryQuery(new List<JHSemesterHistoryRecord>());

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
            NameMapping.Add("學習領域", "學習領域");
            NameMapping.Add("課程學習", "課程學習");
        }

        private void FormSemestersSubject_Load(object sender, EventArgs e)
        {
            //if (Site.DesignMode) return;

            RatingUtils.DisableControls(this);
            RefreshDomainOptions();
            RatingUtils.EnableControls(this);
        }

        #region 畫面驗證邏輯。
        private bool ValidateDataGrid()
        {
            return RefreshSelectedItems();
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
        #endregion

        private void btnRank_Click(object sender, EventArgs e)
        {
            if (SemesterScores.Count <= 0)
            {
                MsgBox.Show("無任何成績資料可排名。");
                return; //沒有任何學期成績不進行排名。
            }
            if (!ValidateDataGrid()) return;
            if (!ValidateTopRankText()) return;
            if (!ValidateLastRankText()) return;
            if (!ValidatePercentageText()) return;
            if (!ValidateLastPercentageText()) return;

            RefreshSelectedItems();
            if (SelectedItems.Count <= 0) return;

            RefreshSelectedTokens();
            if (Tokens.Count <= 0)
            {
                MsgBox.Show("請選擇至少一個學期來進行排名。");
                return;
            }

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
            //學期歷程查詢類別。
            SemesterHistory = new HistoryQuery(JHSemesterHistory.SelectByStudentIDs(Students.ToKeys()));

            //將學生的成績清除。
            foreach (RatingStudent each in Students)
                each.Clear();

            foreach (JHSemesterScoreRecord semsRecord in SemesterScores)
            {
                RatingStudent student;

                if (!dicstuds.TryGetValue(semsRecord.RefStudentID, out student))
                    continue; //找不到該學生。

                ScoreCollection subjScores = null, domainScores = null;

                if (SemesterHistory.Contains(semsRecord.RefStudentID, semsRecord.SchoolYear, semsRecord.Semester))
                {
                    //tokne 意思是特定學期的識別字串，例如「一上」。
                    string token = SemesterHistory.GetToken(semsRecord.RefStudentID, semsRecord.SchoolYear, semsRecord.Semester);

                    //不在使用者選擇的學期中，就不處理。
                    if (!Tokens.Contains(token)) continue;

                    if (!student.Scores.Contains(ScoreType.Subject.Regulation(token))) //如果學生不包含該學期的成績，就加上該學期。
                        student.Scores.Add(ScoreType.Subject.Regulation(token), new ScoreCollection());

                    if (!student.Scores.Contains(ScoreType.Domain.Regulation(token))) //如果學生不包含該學期的成績，就加上該學期。
                        student.Scores.Add(ScoreType.Domain.Regulation(token), new ScoreCollection());

                    subjScores = student.Scores[ScoreType.Subject.Regulation(token)];
                    domainScores = student.Scores[ScoreType.Domain.Regulation(token)];
                }
                else
                    continue; //沒有該學期的學期歷程就不處理。

                #region 科目
                foreach (K12.Data.SubjectScore subjRecord in semsRecord.Subjects.Values)
                {
                    if (!subjRecord.Score.HasValue) continue; //沒有成績就不處理。

                    ScoreItem subject = new ScoreItem(subjRecord.Subject, ScoreType.Subject);

                    if (!SelectedItems.Contains(subject))
                        continue; //不在處理的科目清單中。

                    if (subjScores.Contains(subject.Name))
                        throw new ArgumentException(string.Format("學生「{0}」在同一學期修習「{1}」科目一次以上。", student.Name, subject.Name));

                    subjScores.Add(subject.Name, subjRecord.Score.Value);
                }
                #endregion

                #region 領域
                foreach (K12.Data.DomainScore domainRecord in semsRecord.Domains.Values)
                {
                    if (!domainRecord.Score.HasValue) continue; //沒有成績就不處理。

                    ScoreItem domain = new ScoreItem(domainRecord.Domain, ScoreType.Domain);

                    if (!SelectedItems.Contains(domain))
                        continue; //不在處理的領域清單中。

                    if (domainScores.Contains(domain.Name))
                        throw new ArgumentException(string.Format("學生「{0}」在{1}學年第{2}學期「{3}」領域成績出現一次以上。", student.Name, domainRecord.SchoolYear, domainRecord.Semester, domain.Name));

                    domainScores.Add(domain.Name, domainRecord.Score.Value);
                }

                if (semsRecord.LearnDomainScore.HasValue)
                    domainScores.Add("學習領域", semsRecord.LearnDomainScore.Value);

                if (semsRecord.CourseLearnScore.HasValue)
                    domainScores.Add("課程學習", semsRecord.CourseLearnScore.Value);

                #endregion
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

            List<MultiScoreParser> parsers = SelectedItems.Keys.ToMultiScoreParsers(Tokens);
            List<RatingScope<RatingStudent>> scopes = ToScopes();

            //分別排名。
            foreach (RatingScope<RatingStudent> eachScope in scopes)
            {
                foreach (MultiScoreParser eachParser in parsers)
                    eachScope.Rank(eachParser, PlaceOptions.Unsequence);
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
            else if (rbLastRank.Checked)
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

            if (rbAllRank.Checked) //如果是列出全部名次的話。
                new OutputSeparatePivot().Output(scopes, parsers.ToNameList()); //分別列出科目、名次(Pivot 列法)。
            else
                new OutputSeparate().Output(scopes, parsers.ToNameList(), param); //分別列出科目、名次(非 Pivot 列法)。
        }

        private void RefreshDomainOptions()
        {
            FillSemestersData();
            FillItemOptions();
        }

        /// <summary>
        /// (Network Access)將目前學期的資料填入到 SemesterScores 變數中。
        /// </summary>
        private void FillSemestersData()
        {
            SemesterScores = JHSemesterScore.SelectByStudentIDs(Students.ToKeys());
        }

        private void FillItemOptions()
        {
            dgv.SuspendLayout();
            dgv.Rows.Clear();

            #region 讀取學期成績，並群組出科目項目。
            List<string> studentIDs = Students.ToKeys();
            Dictionary<string, int> subjectCount = new Dictionary<string, int>();

            //群組出有哪些科目。
            foreach (JHSemesterScoreRecord semsscore in SemesterScores)
            {
                foreach (K12.Data.SubjectScore subject in semsscore.Subjects.Values)
                {
                    if (!subjectCount.ContainsKey(subject.Subject))
                        subjectCount.Add(subject.Subject, 0);
                    //subjectCount[subject.Subject]++;
                }
            }

            List<string> subjects = new List<string>(subjectCount.Keys);
            subjects.Sort(new Comparison<string>(SubjectSorter.Sort));

            foreach (string eachSubj in subjects)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, false, eachSubj);
                row.Tag = new ScoreItem(eachSubj, ScoreType.Subject);
                dgv.Rows.Add(row);
            }
            #endregion

            #region 列出領域名稱。
            foreach (string each in NameMapping.Keys)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, false, each);
                row.Tag = new ScoreItem(each, ScoreType.Domain);
                row.Cells[chScoreItem.Index].Style.ForeColor = Color.Blue;
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

        private void RefreshSelectedTokens()
        {
            this.Tokens = new List<string>();

            if (chk1Up.Checked)
            {
                this.Tokens.Add(GTokens.一上);
                this.Tokens.Add(GTokens.七上);
            }

            if (chk1Down.Checked)
            {
                this.Tokens.Add(GTokens.一下);
                this.Tokens.Add(GTokens.七下);
            }

            if (chk2Up.Checked)
            {
                this.Tokens.Add(GTokens.二上);
                this.Tokens.Add(GTokens.八上);
            }

            if (chk2Down.Checked)
            {
                this.Tokens.Add(GTokens.二下);
                this.Tokens.Add(GTokens.八下);
            }

            if (chk3Up.Checked)
            {
                this.Tokens.Add(GTokens.三上);
                this.Tokens.Add(GTokens.九上);
            }

            if (chk3Down.Checked)
            {
                this.Tokens.Add(GTokens.三下);
                this.Tokens.Add(GTokens.九下);
            }
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
                    string scoreName = "" + each.Cells[chScoreItem.Index].Value;
                    ScoreItem si = (ScoreItem)each.Tag;
                    SelectedItems.Add(si, 1);
                    result = true;
                }
            }

            return result;
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
