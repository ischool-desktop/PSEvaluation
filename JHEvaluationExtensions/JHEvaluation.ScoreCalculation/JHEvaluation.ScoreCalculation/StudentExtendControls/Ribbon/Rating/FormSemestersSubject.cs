using System;
using System.Collections.Generic;
using System.Windows.Forms;
using JHSchool.Data;
using JHSchool.Evaluation.RatingFramework;
using FISCA.Presentation.Controls;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating
{
    public partial class FormSemestersSubject : FormRating
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
        private ItemWeightCollection SelectedSubjects { get; set; }

        /// <summary>
        /// 用於顯示錯誤於畫面上。
        /// </summary>
        private ErrorProvider error = new ErrorProvider();

        /// <summary>
        /// 預設的成績識別字串。
        /// </summary>
        private List<string> Tokens = new List<string>();

        public FormSemestersSubject()
            : base()
        {
            InitializeComponent();
            SemesterScores = new List<JHSemesterScoreRecord>();
            SemesterHistory = new HistoryQuery(new List<JHSemesterHistoryRecord>());
        }

        private void FormSemestersSubject_Load(object sender, EventArgs e)
        {
            //if (Site.DesignMode) return;

            RatingUtils.DisableControls(this);
            RefreshSubjectOptions();
            RatingUtils.EnableControls(this);
        }

        #region 畫面驗證邏輯。
        private bool ValidateDataGrid()
        {
            return RefreshSelectedSubjects();
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
        #endregion

        private void btnRank_Click(object sender, EventArgs e)
        {
            if (SemesterScores.Count <= 0)
            {
                MsgBox.Show("無任何成績資料可排名。");
                return; //沒有任何學期成績不進行排名。
            }
            if (!ValidateDataGrid()) return;

            RefreshSelectedSubjects();
            if (SelectedSubjects.Count <= 0) return;

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

                ScoreCollection scores = null;

                if (SemesterHistory.Contains(semsRecord.RefStudentID, semsRecord.SchoolYear, semsRecord.Semester))
                {
                    //tokne 意思是特定學期的識別字串，例如「一上」。
                    string token = SemesterHistory.GetToken(semsRecord.RefStudentID, semsRecord.SchoolYear, semsRecord.Semester);

                    //不在使用者選擇的學期中，就不處理。
                    if (!Tokens.Contains(token)) continue;

                    if (!student.Scores.Contains(token)) //如果學生不包含該學期的成績，就加上該學期。
                        student.Scores.Add(token, new ScoreCollection());

                    scores = student.Scores[token];
                }
                else
                    continue; //沒有該學期的學期歷程就不處理。

                foreach (K12.Data.SubjectScore subjRecord in semsRecord.Subjects.Values)
                {
                    if (!subjRecord.Score.HasValue) continue; //沒有成績就不處理。

                    string subject = subjRecord.Subject;

                    if (!SelectedSubjects.Contains(subject))
                        continue; //不在處理的科目清單中。

                    if (scores.Contains(subject))
                        throw new ArgumentException(string.Format("學生「{0}」在同一學期修習「{1}」科目一次以上。", student.Name, subject));

                    scores.Add(subject, subjRecord.Score.Value);
                }
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

            List<MultiScoreParser> parsers = SelectedSubjects.Keys.ToMultiScoreParsers(Tokens);
            List<RatingScope<RatingStudent>> scopes = ToScopes();
            List<RatingStudent> sortedStudents;

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
                param.Mode = FilterMode.Place;
                param.Top = int.Parse(txtTopRank.Text);
            }
            else if (rbPercentage.Checked)
            {
                param.Mode = FilterMode.Percentage;
                param.Top = int.Parse(txtPercentage.Text);
            }
            else throw new ArgumentException("無此種過慮排名的方式。");

            if (rbAllRank.Checked) //如果是列出全部名次的話。
                new OutputSeparatePivot().Output(scopes, parsers.ToNameList()); //分別列出科目、名次(Pivot 列法)。
            else
                new OutputSeparate().Output(scopes, parsers.ToNameList(), param); //分別列出科目、名次(非 Pivot 列法)。
        }

        private void RefreshSubjectOptions()
        {
            FillSemestersData();
            FillSubjectOptions();
        }

        /// <summary>
        /// (Network Access)將目前學期的資料填入到 SemesterScores 變數中。
        /// </summary>
        private void FillSemestersData()
        {
            SemesterScores = JHSemesterScore.SelectByStudentIDs(Students.ToKeys());
        }

        private void FillSubjectOptions()
        {
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

            dgv.SuspendLayout();
            dgv.Rows.Clear();
            foreach (string eachSubj in subjects)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, false, eachSubj);
                dgv.Rows.Add(row);
            }
            dgv.ResumeLayout();
            //dgv.Refresh();
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

        private void RefreshSelectedTokens()
        {
            Tokens = new List<string>();

            if (chk1Up.Checked) Tokens.Add(Token.一上);
            if (chk1Down.Checked) Tokens.Add(Token.一下);
            if (chk2Up.Checked) Tokens.Add(Token.二上);
            if (chk2Down.Checked) Tokens.Add(Token.二下);
            if (chk3Up.Checked) Tokens.Add(Token.三上);
            if (chk3Down.Checked) Tokens.Add(Token.三下);
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
                    SelectedSubjects.Add(subject, 1);
                    result = true;
                }
            }

            return result;
        }
    }
}
