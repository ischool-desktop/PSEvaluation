using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Framework;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking;
using JHSchool.Evaluation.Calculation;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Basic;
using JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Semester;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon
{
    public partial class RankingSemesterForm : FISCA.Presentation.Controls.BaseForm
    {
        private List<RStudentRecord> Students { get; set; }
        private RSemesterScoreCollection SemesterScores { get; set; }

        private bool _initialized = false;
        private RankDataCollector _collector;
        private Rank _ranker;
        private CombineRankType CombineRankType { get; set; }

        private RankingSemesterOptionForm _form;

        private string _reportName;

        private int _carry;
        private int Carry { get { return _carry; } }

        private string _type;
        private string Type { get { return _type; } }

        private string _category;
        private string Category { get { return _category; } }

        public RankingSemesterForm(string type, string category)
        {
            InitializeComponent();

            Students = RStudentRecord.LoadStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
            SemesterScores = RSemesterScoreCollection.LoadScores(Students.AsKeyList());

            _ranker = new Rank();
            _collector = new RankDataCollector(Students, _ranker);
            _collector.RankType = RankType.Class;
            _reportName = string.Empty;
            _form = new RankingSemesterOptionForm();
            _carry = _form.Carry;
            _ranker.Sequence = _form.Sequence;

            lblSelectedCount.Text = string.Format("已選擇人數：{0}", Students.Count);

            _type = type;
            _category = category;

            _reportName = "學期成績";
            this.Text = "學期成績排名";

            if (Category.Equals("Subject"))
                chCategory.HeaderText = "科目";
            else if (Category.Equals("Domain"))
                chCategory.HeaderText = "領域";

            _reportName += chCategory.HeaderText + "排名";
            _collector.Category = chCategory.HeaderText;

            InitializeSemester();
            InitializeCombineType();

            Color c = (DevComponents.DotNetBar.Rendering.GlobalManager.Renderer as DevComponents.DotNetBar.Rendering.Office2007Renderer).ColorTable.CheckBoxItem.Default.Text;
            SetColor(c);
        }

        private int SelectedSchoolYear
        {
            get
            {
                int sy;
                if (int.TryParse(cboSchoolYear.Text, out sy))
                    return sy;
                else
                    return 0;
            }
        }

        private int SelectedSemester
        {
            get
            {
                int se;
                if (int.TryParse(cboSemester.Text, out se))
                    return se;
                else
                    return 0;
            }
        }

        private void InitializeCombineType()
        {
            foreach (Control ctrl in panelEx1.Controls)
            {
                if (ctrl is RadioButton) (ctrl as RadioButton).CheckedChanged += new EventHandler(CombineRadioButton_CheckedChanged);
            }
            CombineRankType = CombineRankType.加權總分;
        }

        private void CombineRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if ("" + rb.Tag == "加權總分") CombineRankType = CombineRankType.加權總分;
            else if ("" + rb.Tag == "加權平均") CombineRankType = CombineRankType.加權平均;
            else if ("" + rb.Tag == "合計總分") CombineRankType = CombineRankType.合計總分;
            else if ("" + rb.Tag == "算數平均") CombineRankType = CombineRankType.算數平均;
        }

        private void InitializeSemester()
        {
            try
            {
                int schoolyear = int.Parse(School.DefaultSchoolYear);

                for (int i = -3; i < 3; i++)
                    cboSchoolYear.Items.Add(schoolyear + i);

                cboSemester.Items.Add("1");
                cboSemester.Items.Add("2");

                cboSchoolYear.Text = School.DefaultSchoolYear;
                cboSemester.Text = School.DefaultSemester;
            }
            catch (Exception)
            {
                MsgBox.Show("學年度學期初始化錯誤。");
            }
        }

        private void SetColor(Color c)
        {
            foreach (Control ctrl in panel3.Controls)
                if (ctrl is Label || ctrl is RadioButton || ctrl is CheckBox)
                    ctrl.ForeColor = c;
            foreach (Control ctrl in panel4.Controls)
                if (ctrl is Label || ctrl is RadioButton || ctrl is CheckBox)
                    ctrl.ForeColor = c;
            foreach (Control ctrl in panelEx1.Controls)
                if (ctrl is Label || ctrl is RadioButton || ctrl is CheckBox)
                    ctrl.ForeColor = c;
            foreach (Control ctrl in panelEx2.Controls)
                if (ctrl is Label || ctrl is RadioButton || ctrl is CheckBox)
                    ctrl.ForeColor = c;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRank_Click(object sender, EventArgs e)
        {
            if (!IsValid())
            {
                MsgBox.Show("輸入資料不正確，請先修正。");
                return;
            }

            _collector.Clear();
            if (Type.Equals("Semester"))
            {
                #region 學期成績排名
                if (Category.Equals("Subject"))
                {
                    #region 科目
                    Dictionary<string, decimal> selectedItems = GetSelectedItems();
                    List<string> studentIDs = Students.AsKeyList();
                    Dictionary<string, RankData> ranks = ArrangeBySubject(selectedItems);

                    if (rbSeparate.Checked)
                    {
                        #region 分科排名
                        foreach (RankData rankData in ranks.Values)
                            _collector.AddRankData(rankData);
                        #endregion
                    }
                    else
                    {
                        #region 運算排名
                        RankData combinRank = new RankData();
                        combinRank.Name = CombineRankType.ToString();
                        //StudentID->Subject->Score
                        Dictionary<string, Dictionary<string, decimal>> combineScore = new Dictionary<string, Dictionary<string, decimal>>();
                        decimal periodCount = 0;
                        foreach (RankData eachRank in ranks.Values)
                        {
                            foreach (string studId in eachRank.Keys) //學生編號
                            {
                                if (!combineScore.ContainsKey(studId))
                                    combineScore.Add(studId, new Dictionary<string, decimal>());

                                if (!combineScore[studId].ContainsKey(eachRank.Name))
                                {
                                    if (CombineRankType == CombineRankType.加權平均 || CombineRankType == CombineRankType.加權總分)
                                        combineScore[studId].Add(eachRank.Name, eachRank[studId].Score * selectedItems[eachRank.Name]);
                                    else
                                        combineScore[studId].Add(eachRank.Name, eachRank[studId].Score);
                                }
                            }

                            if (CombineRankType == CombineRankType.加權平均)
                                periodCount += selectedItems[eachRank.Name];
                        }

                        foreach (string id in combineScore.Keys)
                        {
                            decimal total = 0;
                            foreach (decimal score in combineScore[id].Values)
                                total += score;

                            if (CombineRankType == CombineRankType.加權平均)
                                total = total / periodCount;
                            else if (CombineRankType == CombineRankType.算數平均)
                                total = total / combineScore[id].Count;

                            if (!combinRank.ContainsKey(id))
                            {
                                total = decimal.Round(total, Carry, MidpointRounding.AwayFromZero);
                                combinRank.Add(id, new RankScore(total, null));
                            }
                        }
                        combinRank.Tag = ranks;
                        _collector.AddRankData(combinRank);
                        #endregion
                    }
                    _collector.Perform();
                    _collector.Export(rbCombine.Checked, rbAllRank.Checked, txtTopRank.Text, (chkPrintByClass.Enabled) ? chkPrintByClass.Checked : false, _reportName);
                    #endregion
                }
                else if (Category.Equals("Domain"))
                {
                    #region 領域
                    //Dictionary<string, decimal> domains = new Dictionary<string, decimal>();
                    //foreach (DataGridViewRow row in dgv.Rows)
                    //{
                    //    string value = "" + row.Cells[chCheck.Index].Value;
                    //    bool b;
                    //    if (bool.TryParse(value, out b) && b == true)
                    //    {
                    //        string domain = "" + row.Cells[chCategory.Index].Value;
                    //        decimal period = decimal.Parse("" + row.Cells[chPeriod.Index].Value);
                    //        domains.Add(domain, period);
                    //    }
                    //}

                    //if (rbSeparate.Checked)
                    //{
                    //    #region 分科排名
                    //    List<string> studentIDs = Students.AsKeyList();
                    //    Dictionary<string, RankData> data = new Dictionary<string, RankData>();
                    //    foreach (Data.JHSemesterScoreRecord record in _semsScores)
                    //    {
                    //        if (!studentIDs.Contains(record.RefStudentID)) continue;

                    //        foreach (K12.Data.DomainScore domain in record.Domains.Values)
                    //        {
                    //            if (!domains.ContainsKey(domain.Domain)) continue;

                    //            if (!data.ContainsKey(domain.Domain))
                    //                data.Add(domain.Domain, new RankData());
                    //            RankData rankData = data[domain.Domain];
                    //            rankData.Name = domain.Domain;
                    //            if (!rankData.ContainsKey(record.RefStudentID))
                    //            {
                    //                if (domain.Score.HasValue)
                    //                    rankData.Add(record.RefStudentID, new RankScore(domain.Score.Value, null));
                    //                //else if (UseZero)
                    //                //    rankData.Add(record.RefStudentID, new RankScore(0, null));
                    //            }
                    //        }

                    //        string learnDomain = "學習領域";
                    //        if (domains.ContainsKey(learnDomain))
                    //        {
                    //            if (!data.ContainsKey(learnDomain))
                    //                data.Add(learnDomain, new RankData());
                    //            RankData rankData = data[learnDomain];
                    //            rankData.Name = learnDomain;
                    //            if (!rankData.ContainsKey(record.RefStudentID))
                    //            {
                    //                if (record.LearnDomainScore.HasValue)
                    //                    rankData.Add(record.RefStudentID, new RankScore(record.LearnDomainScore.Value, null));
                    //                //else if (UseZero)
                    //                //    rankData.Add(record.RefStudentID, new RankScore(0, null));
                    //            }
                    //        }

                    //        string courseLearn = "課程學習";
                    //        if (domains.ContainsKey(courseLearn))
                    //        {
                    //            if (!data.ContainsKey(courseLearn))
                    //                data.Add(courseLearn, new RankData());
                    //            RankData rankData = data[courseLearn];
                    //            rankData.Name = courseLearn;
                    //            if (!rankData.ContainsKey(record.RefStudentID))
                    //            {
                    //                if (record.CourseLearnScore.HasValue)
                    //                    rankData.Add(record.RefStudentID, new RankScore(record.CourseLearnScore.Value, null));
                    //                //else if (UseZero)
                    //                //    rankData.Add(record.RefStudentID, new RankScore(0, null));
                    //            }
                    //        }
                    //    }
                    //    foreach (RankData rankData in data.Values)
                    //        _collector.AddRankData(rankData);
                    //    #endregion
                    //}
                    //else
                    //{
                    //    #region 運算排名
                    //    List<string> studentIDs = Students.AsKeyList();
                    //    Dictionary<string, RankData> data = new Dictionary<string, RankData>();
                    //    foreach (Data.JHSemesterScoreRecord record in _semsScores)
                    //    {
                    //        if (!studentIDs.Contains(record.RefStudentID)) continue;

                    //        foreach (K12.Data.DomainScore domain in record.Domains.Values)
                    //        {
                    //            if (!domains.ContainsKey(domain.Domain)) continue;

                    //            if (!data.ContainsKey(domain.Domain))
                    //                data.Add(domain.Domain, new RankData());
                    //            RankData rankData = data[domain.Domain];
                    //            rankData.Name = domain.Domain;
                    //            if (!rankData.ContainsKey(record.RefStudentID))
                    //            {
                    //                if (domain.Score.HasValue)
                    //                    rankData.Add(record.RefStudentID, new RankScore(domain.Score.Value, null));
                    //                //else if (UseZero)
                    //                //    rankData.Add(record.RefStudentID, new RankScore(0, null));
                    //            }
                    //        }
                    //    }
                    //    RankData combineData = new RankData();
                    //    combineData.Name = CombineRankType.ToString();
                    //    Dictionary<string, Dictionary<string, decimal>> combineScore = new Dictionary<string, Dictionary<string, decimal>>();
                    //    decimal periodCount = 0;
                    //    foreach (RankData rankData in data.Values)
                    //    {
                    //        foreach (string id in rankData.Keys)
                    //        {
                    //            if (!combineScore.ContainsKey(id))
                    //                combineScore.Add(id, new Dictionary<string, decimal>());

                    //            if (!combineScore[id].ContainsKey(rankData.Name))
                    //            {
                    //                if (CombineRankType == CombineRankType.加權平均 || CombineRankType == CombineRankType.加權總分)
                    //                    combineScore[id].Add(rankData.Name, rankData[id].Score * domains[rankData.Name]);
                    //                else
                    //                    combineScore[id].Add(rankData.Name, rankData[id].Score);
                    //            }
                    //        }

                    //        if (CombineRankType == CombineRankType.加權平均)
                    //            periodCount += domains[rankData.Name];
                    //    }
                    //    foreach (string id in combineScore.Keys)
                    //    {
                    //        decimal total = 0;
                    //        foreach (decimal score in combineScore[id].Values)
                    //            total += score;

                    //        if (CombineRankType == CombineRankType.加權平均)
                    //            total = total / periodCount;
                    //        else if (CombineRankType == CombineRankType.算數平均)
                    //            total = total / combineScore[id].Count;

                    //        if (!combineData.ContainsKey(id))
                    //        {
                    //            total = decimal.Round(total, Carry, MidpointRounding.AwayFromZero);
                    //            combineData.Add(id, new RankScore(total, null));
                    //        }
                    //    }
                    //    combineData.Tag = data;
                    //    _collector.AddRankData(combineData);
                    //    #endregion
                    //}
                    //_collector.Perform();
                    //_collector.Export(rbCombine.Checked, rbAllRank.Checked, txtTopRank.Text, (chkPrintByClass.Enabled) ? chkPrintByClass.Checked : false, _reportName);
                    #endregion
                }
                #endregion
            }
        }

        /// <summary>
        /// 取得畫面上選擇的科目(領域)與相對應的權重。
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, decimal> GetSelectedItems()
        {
            Dictionary<string, decimal> selectedsubjs = new Dictionary<string, decimal>();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                string value = "" + row.Cells[chCheck.Index].Value;
                bool b;
                if (bool.TryParse(value, out b) && b == true)
                {
                    string subject = "" + row.Cells[chCategory.Index].Value;
                    decimal period = decimal.Parse("" + row.Cells[chPeriod.Index].Value);
                    selectedsubjs.Add(subject, period);
                }
            }
            return selectedsubjs;
        }

        /// <summary>
        /// 按科目組織排名資料。
        /// </summary>
        /// <param name="selectedsubjs">科目名稱->RankData</param>
        /// <returns></returns>
        private Dictionary<string, RankData> ArrangeBySubject(Dictionary<string, decimal> selectedsubjs)
        {
            Dictionary<string, RankData> ranks = new Dictionary<string, RankData>();
            foreach (RSemesterScore semsscore in SemesterScores.ForBy(SelectedSchoolYear, SelectedSemester))
            {
                foreach (RSubjectScore subjscore in semsscore.Subjects)
                {
                    //不在科目清單中就跳過。
                    if (!selectedsubjs.ContainsKey(subjscore.Name)) continue;

                    //「排名」不存在就新增。
                    if (!ranks.ContainsKey(subjscore.Name))
                    {
                        ranks.Add(subjscore.Name, new RankData());
                        ranks[subjscore.Name].Name = subjscore.Name;
                    }

                    RankData rank = ranks[subjscore.Name];

                    //將學生成績加入到排名資料中。
                    if (!rank.ContainsKey(semsscore.RefStudentID))
                        rank.Add(semsscore.RefStudentID, new RankScore(subjscore.GetScoreOrDefault(0), null));
                    else
                        throw new ArgumentException("學生有重覆的學期成績(資料異常!)。");
                }
            }
            return ranks;
        }

        private void btnRankingOption_Click(object sender, EventArgs e)
        {
            if (_form.ShowDialog() == DialogResult.OK)
            {
                _ranker.Sequence = _form.Sequence;
                _carry = _form.Carry;
            }
        }

        private bool IsValid()
        {
            if (errorProvider.HasError) return false;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                bool b = false;
                if (bool.TryParse("" + row.Cells[chCheck.Index].Value, out b) &&
                    b == true &&
                    !string.IsNullOrEmpty(row.Cells[chPeriod.Index].ErrorText))
                    return false;
            }

            return true;
        }

        private void ValidPeriod(DataGridViewRow row)
        {
            string value = "" + row.Cells[chPeriod.Index].Value;
            decimal d;

            if (value.Contains(","))
            {
                if (!_initialized)
                    row.Cells[chPeriod.Index].ErrorText = "有兩個以上的權數";
                else
                    row.Cells[chPeriod.Index].ErrorText = "權數必須為數字";
            }
            else if (!decimal.TryParse(value, out d))
                row.Cells[chPeriod.Index].ErrorText = "權數必須為數字";
            else
                row.Cells[chPeriod.Index].ErrorText = "";
        }

        private bool ValidSemester()
        {
            errorProvider.SetError(cboSchoolYear, "");
            errorProvider.SetError(cboSemester, "");

            bool valid = true;
            int i;
            if (!int.TryParse(cboSchoolYear.Text, out i))
            {
                errorProvider.SetError(cboSchoolYear, "無效的學年度");
                valid = false;
            }
            if (!int.TryParse(cboSemester.Text, out i))
            {
                errorProvider.SetError(cboSemester, "無效的學期");
                valid = false;
            }
            return valid;
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ValidPeriod(dgv.Rows[e.RowIndex]);
        }

        private void dgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ValidPeriod(dgv.Rows[e.RowIndex]);
        }

        private void LoadItems()
        {
            dgv.Rows.Clear();

            _initialized = false;

            if (Type.Equals("Semester"))
            {
                if (Category.Equals("Subject"))
                {
                    #region 讀取學期成績科目

                    Dictionary<string, int> subjects = new Dictionary<string, int>();
                    foreach (RStudentRecord student in Students)
                    {
                        foreach (RSemesterScore semsscore in SemesterScores.ForBy(student.ID, SelectedSchoolYear, SelectedSemester))
                        {
                            foreach (RSubjectScore subjscore in semsscore.Subjects)
                            {
                                if (!subjects.ContainsKey(subjscore.Name))
                                    subjects.Add(subjscore.Name, 0);

                                subjects[subjscore.Name]++;
                            }
                        }
                    }

                    foreach (string subject in subjects.Keys)
                    {
                        DataGridViewRow row = new DataGridViewRow();

                        row.CreateCells(dgv, false, 1, subject, subjects[subject]);
                        dgv.Rows.Add(row);
                    }

                    #endregion
                }
                else if (Category.Equals("Domain"))
                {
                    #region 讀取學期成績領域

                    Dictionary<string, int> domains = new Dictionary<string, int>();
                    Dictionary<string, List<decimal>> periods = new Dictionary<string, List<decimal>>();
                    foreach (RStudentRecord student in Students)
                    {
                        foreach (RSemesterScore record in SemesterScores.ForBy(student.ID, SelectedSchoolYear, SelectedSemester))
                        {
                            foreach (RDomainScore domain in record.Domains)
                            {
                                if (!domains.ContainsKey(domain.Name))
                                    domains.Add(domain.Name, 0);
                                domains[domain.Name]++;
                            }
                        }
                    }

                    foreach (string domain in domains.Keys)
                    {
                        DataGridViewRow row = new DataGridViewRow();

                        row.CreateCells(dgv, false, 1, domain, domains[domain]);
                        dgv.Rows.Add(row);
                    }

                    #endregion
                }
            }

            _initialized = true;
        }

        private void cboSchoolYear_TextChanged(object sender, EventArgs e)
        {
            if (!ValidSemester()) return;

            LoadItems();
        }

        private void cboSemester_TextChanged(object sender, EventArgs e)
        {
            if (!ValidSemester()) return;

            LoadItems();
        }

        private void rbCombine_CheckedChanged(object sender, EventArgs e)
        {
            panelEx1.Enabled = rbCombine.Checked;
            if (rbCombine.Checked)
                chkPrintByClass.Enabled = true;
            else
                chkPrintByClass.Enabled = rbAllRank.Checked;
        }

        private void rbClass_CheckedChanged(object sender, EventArgs e)
        {
            if (rbClass.Checked)
                _collector.RankType = RankType.Class;
            else
                _collector.RankType = RankType.GradeYear;
        }

        private void rbAllRank_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCombine.Checked)
                chkPrintByClass.Enabled = true;
            else
                chkPrintByClass.Enabled = rbAllRank.Checked;
        }

        private void txtTopRank_TextChanged(object sender, EventArgs e)
        {
            errorProvider.SetError(txtTopRank, "");

            string text = txtTopRank.Text;
            if (string.IsNullOrEmpty(text)) return;

            if (text.EndsWith("%"))
            {
                string percent = text.Substring(0, text.Length - 1);
                int i;
                if (!int.TryParse(percent, out i))
                    errorProvider.SetError(txtTopRank, "百分比必須為整數");
                else if (i < 1 || i > 100)
                    errorProvider.SetError(txtTopRank, "百分比必須為1到100");
            }
            else
            {
                int i;
                if (!int.TryParse(text, out i))
                    errorProvider.SetError(txtTopRank, "必須為整數或百分比");
            }
        }
    }
}
