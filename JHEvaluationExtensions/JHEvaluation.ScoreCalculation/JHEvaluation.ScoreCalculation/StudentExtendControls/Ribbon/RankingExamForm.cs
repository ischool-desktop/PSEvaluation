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
using JHSchool.Data;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon
{
    public partial class RankingExamForm : FISCA.Presentation.Controls.BaseForm
    {
        /// <summary>
        /// 使用者所選擇的學生。
        /// </summary>
        private List<RStudentRecord> _students;
        /// <summary>
        /// 已選擇學生的修課記錄。
        /// </summary>
        private List<JHSCAttendRecord> _scattends;

        /// <summary>
        /// 已選擇學生所修課程，以使用者選擇的「學年度、學期」過濾。
        /// </summary>
        private List<CourseRecord> _courses;
        private List<JHSemesterScoreRecord> _semsScores;
        private Dictionary<string, List<JHSemesterScoreRecord>> _studentSemesterScoreRecordCache;

        private bool _initialized = false;
        private RankDataCollector _collector;
        private Rank _ranker;
        private CombineRankType CombineRankType { get; set; }

        private RankingExamOptionForm _form;

        private string _reportName;

        private bool _use_zero;
        private bool UseZero { get { return _use_zero; } }
        private int _carry;
        private int Carry { get { return _carry; } }

        private string _type;
        private string Type { get { return _type; } }

        private string _category;
        private string Category { get { return _category; } }

        public RankingExamForm(string type, string category)
        {
            InitializeComponent();

            _students = RStudentRecord.LoadStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
            _ranker = new Rank();
            _collector = new RankDataCollector(_students, _ranker);
            _collector.RankType = RankType.Class;
            _reportName = string.Empty;
            _form = new RankingExamOptionForm();
            _use_zero = _form.UseZero;
            _carry = _form.Carry;
            _ranker.Sequence = _form.Sequence;

            lblSelectedCount.Text = string.Format("已選擇人數：{0}", _students.Count);

            _type = type;
            _category = category;

            if (Type.Equals("Exam"))
            {
                InitializeExam();
                cboExam.Enabled = true;
                cboExam.Visible = true;
                panel1.Size = new Size(panel1.Width, 60);
                _reportName = "評量成績";
                this.Text = "評量成績排名";
            }

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

        private void InitializeExam()
        {
            List<string> studentIDs = _students.AsKeyList();

            _scattends = Data.JHSCAttend.SelectByStudentIDs(studentIDs);
            _courses = new List<CourseRecord>();
            cboExam.DisplayMember = "Name";
            cboExam.ValueMember = "ID";

            foreach (Data.JHExamRecord exam in Data.JHExam.SelectAll())
            {
                cboExam.Items.Add(exam);
            }
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
            if (Type.Equals("Exam"))
            {
                try
                {
                    #region 評量成績排名
                    if (Category.Equals("Subject"))
                    {
                        #region 科目
                        //將使用者選擇的「科目名稱、權重」集中於 subjects 集合。
                        Dictionary<string, decimal> subjects = new Dictionary<string, decimal>();
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            string value = "" + row.Cells[chCheck.Index].Value;
                            bool b;
                            if (bool.TryParse(value, out b) && b == true)
                            {
                                string subject = "" + row.Cells[chCategory.Index].Value;
                                decimal period = decimal.Parse("" + row.Cells[chPeriod.Index].Value);
                                subjects.Add(subject, period);
                            }
                        }

                        //_courses 轉換成 ID 清單，如果該課程的科目使用者並沒有選擇就過慮掉。
                        List<string> courseIDs = new List<string>();
                        foreach (CourseRecord record in _courses)
                        {
                            if (subjects.ContainsKey(record.Subject))
                                courseIDs.Add(record.ID);
                        }

                        //取得課程的特定評量成績。
                        List<Data.JHSCETakeRecord> sce_records = Data.JHSCETake.SelectByCourseAndExam(courseIDs, (cboExam.SelectedItem as Data.JHExamRecord).ID);
                        Dictionary<string, Data.JHSCETakeRecord> dic_sce = AsDictionary(sce_records);

                        if (rbSeparate.Checked)
                        {
                            #region 分科排名
                            List<string> studentIDs = _students.AsKeyList();
                            Dictionary<string, RankData> data = new Dictionary<string, RankData>();

                            foreach (string each in subjects.Keys)
                            {
                                data.Add(each, new RankData());
                                data[each].Name = each;
                            }

                            foreach (Data.JHSCAttendRecord record in _scattends)
                            {
                                CourseRecord course = Course.Instance.Items[record.RefCourseID];

                                if ("" + course.SchoolYear != cboSchoolYear.Text) continue;
                                if ("" + course.Semester != cboSemester.Text) continue;
                                if (!subjects.ContainsKey(course.Subject)) continue;

                                RankData rankData = data[course.Subject];
                                decimal? score = null;

                                if (dic_sce.ContainsKey(record.ID))
                                {
                                    Data.JHSCETakeRecord jr = dic_sce[record.ID];
                                    score = jr.Score;
                                }

                                if (rankData.ContainsKey(record.RefStudentID))
                                    throw new ArgumentException(string.Format("學生「{0}」在同一學期修習「{1}」科目一次以上。", Student.Instance[record.RefStudentID].Name, course.Subject));

                                if (score.HasValue)
                                    rankData.Add(record.RefStudentID, new RankScore(score.Value, null));
                                else if (UseZero)
                                    rankData.Add(record.RefStudentID, new RankScore(0, null));
                            }

                            foreach (RankData rankData in data.Values)
                                _collector.AddRankData(rankData);

                            #endregion
                        }
                        else
                        {
                            #region 運算排名
                            List<string> studentIDs = _students.AsKeyList();
                            Dictionary<string, RankData> data = new Dictionary<string, RankData>();

                            foreach (string each in subjects.Keys)
                            {
                                data.Add(each, new RankData());
                                data[each].Name = each;
                            }

                            foreach (Data.JHSCAttendRecord record in _scattends)
                            {
                                CourseRecord course = Course.Instance.Items[record.RefCourseID];

                                if ("" + course.SchoolYear != cboSchoolYear.Text) continue;
                                if ("" + course.Semester != cboSemester.Text) continue;
                                if (!subjects.ContainsKey(course.Subject)) continue;

                                RankData rankData = data[course.Subject];
                                decimal? score = null;

                                if (dic_sce.ContainsKey(record.ID))
                                {
                                    Data.JHSCETakeRecord jr = dic_sce[record.ID];
                                    score = jr.Score;
                                }

                                if (rankData.ContainsKey(record.RefStudentID))
                                    throw new ArgumentException(string.Format("學生「{0}」在同一學期修習「{1}」科目一次以上。", Student.Instance[record.RefStudentID].Name, course.Subject));

                                if (score.HasValue)
                                    rankData.Add(record.RefStudentID, new RankScore(score.Value, null));
                                else if (UseZero)
                                    rankData.Add(record.RefStudentID, new RankScore(0, null));
                            }

                            RankData combineData = new RankData();
                            combineData.Name = CombineRankType.ToString();
                            //<學生編號,<科目名稱,成績>>
                            Dictionary<string, Dictionary<string, decimal>> combineScore = new Dictionary<string, Dictionary<string, decimal>>();
                            decimal periodCount = 0;
                            foreach (RankData rankData in data.Values) //每一個 rankData 是特定科目的所有成績。
                            {
                                foreach (string stuId in rankData.Keys) //id 是學生編號。
                                {
                                    if (!combineScore.ContainsKey(stuId))
                                        combineScore.Add(stuId, new Dictionary<string, decimal>());

                                    if (!combineScore[stuId].ContainsKey(rankData.Name)) //rankData.Name 是科目名稱。
                                    {
                                        if (CombineRankType == CombineRankType.加權平均 || CombineRankType == CombineRankType.加權總分)
                                            combineScore[stuId].Add(rankData.Name, rankData[stuId].Score * subjects[rankData.Name]);
                                        else
                                            combineScore[stuId].Add(rankData.Name, rankData[stuId].Score);
                                    }
                                }

                                //加總科目權重。
                                if (CombineRankType == CombineRankType.加權平均)
                                    periodCount += subjects[rankData.Name];
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

                                if (!combineData.ContainsKey(id))
                                {
                                    total = decimal.Round(total, Carry, MidpointRounding.AwayFromZero);
                                    combineData.Add(id, new RankScore(total, null));
                                }
                            }
                            combineData.Tag = data;
                            _collector.AddRankData(combineData);
                            #endregion
                        }
                        _collector.Perform();
                        _collector.Export(rbCombine.Checked, rbAllRank.Checked, txtTopRank.Text, (chkPrintByClass.Enabled) ? chkPrintByClass.Checked : false, _reportName);
                        #endregion
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private static Dictionary<string, Data.JHSCETakeRecord> AsDictionary(List<Data.JHSCETakeRecord> sce_records)
        {
            Dictionary<string, Data.JHSCETakeRecord> dic_sce = new Dictionary<string, JHSCETakeRecord>();
            foreach (Data.JHSCETakeRecord each in sce_records)
                dic_sce.Add(each.RefSCAttendID, each);
            return dic_sce;
        }

        private void btnRankingOption_Click(object sender, EventArgs e)
        {
            if (_form.ShowDialog() == DialogResult.OK)
            {
                _use_zero = _form.UseZero;
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

            if (Type.Equals("Exam"))
            {
                if (Category.Equals("Subject"))
                {
                    #region 讀取評量成績科目
                    List<string> studentIDs = _students.AsKeyList();
                    Dictionary<string, int> courseCount = new Dictionary<string, int>();
                    Dictionary<string, int> subjectCount = new Dictionary<string, int>();
                    Dictionary<string, int> courseCredit = new Dictionary<string, int>();

                    foreach (Data.JHSCAttendRecord record in _scattends)
                    {
                        CourseRecord course = Course.Instance.Items[record.RefCourseID];

                        if ("" + course.SchoolYear != cboSchoolYear.Text) continue;
                        if ("" + course.Semester != cboSemester.Text) continue;

                        if (!courseCount.ContainsKey(record.RefCourseID))
                            courseCount.Add(record.RefCourseID, 0);
                        courseCount[record.RefCourseID]++;

                        if (!courseCredit.ContainsKey(course.Subject))
                        {
                            int credit;

                            courseCredit.Add(course.Subject, 1);

                            if (int.TryParse(course.Credit, out credit))
                                courseCredit[course.Subject] = credit;
                        }

                        if (!subjectCount.ContainsKey(course.Subject))
                            subjectCount.Add(course.Subject, 0);
                        subjectCount[course.Subject]++;
                    }

                    Dictionary<string, DataGridViewRow> rows = new Dictionary<string, DataGridViewRow>();
                    foreach (string course_id in courseCount.Keys)
                    {
                        CourseRecord course = Course.Instance.Items[course_id];

                        if ("" + course.SchoolYear != cboSchoolYear.Text) continue;
                        if ("" + course.Semester != cboSemester.Text) continue;

                        _courses.Add(course);

                        string subject = course.Subject;
                        if (!rows.ContainsKey(subject))
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(dgv, false, courseCredit[subject], subject, subjectCount[subject]);
                            dgv.Rows.Add(row);
                            rows.Add(subject, row);
                        }
                    }
                    #endregion
                }
            }

            _initialized = true;
        }

        private void cboExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider.SetError(cboExam, "");
            if (!ValidSemester()) return;

            LoadItems();
        }

        private void cboSchoolYear_TextChanged(object sender, EventArgs e)
        {
            if (!ValidSemester()) return;

            if (Type.Equals("Exam") && cboExam.SelectedItem == null)
            {
                errorProvider.SetError(cboExam, "請選擇評量");
                return;
            }

            LoadItems();
        }

        private void cboSemester_TextChanged(object sender, EventArgs e)
        {
            if (!ValidSemester()) return;

            if (Type.Equals("Exam") && cboExam.SelectedItem == null)
            {
                errorProvider.SetError(cboExam, "請選擇評量");
                return;
            }

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
