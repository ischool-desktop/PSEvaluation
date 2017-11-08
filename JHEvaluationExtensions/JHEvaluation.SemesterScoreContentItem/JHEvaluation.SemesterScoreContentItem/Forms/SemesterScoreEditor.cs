using DevComponents.DotNetBar.Controls;
using FISCA.Presentation.Controls;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using JHSchool.Data;
using JHSchool.Evaluation;
using K12.Data.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace JHEvaluation.SemesterScoreContentItem.Forms
{
    public partial class SemesterScoreEditor : BaseForm
    {
        //可編輯狀態
        bool _Editable = true;

        /// <summary>
        /// 學生所有學期成績記錄
        /// </summary>
        private List<JHSemesterScoreRecord> _semesterScoreRecordList;

        /// <summary>
        /// 學生
        /// </summary>
        private JHStudentRecord _student;

        /// <summary>
        /// 選擇的學期成績記錄
        /// </summary>
        private JHSemesterScoreRecord _record;

        /// <summary>
        /// 成績進位
        /// </summary>
        private ScoreCalculator _calculator;

        /// <summary>
        /// 學期歷程
        /// </summary>
        private SemesterHistoryUtility _util;

        /// <summary>
        ///監看資料改變
        /// </summary>
        private Framework.ChangeListener _listener;

        private int _selectedRowIndex;
        private bool _isModifyMode = false;
        private bool _semesterIsValid = false;
        private bool _filled = false;

        //努力程度對照表
        private Dictionary<decimal, int> _effortDict;
        private List<decimal> _effortScoreList;

        /// <summary>
        /// Constructor
        /// </summary>
        private SemesterScoreEditor()
        {
            InitializeComponent();
            InitializeComboBoxes();
            InitializeDomainsAndSubjects();
            InitializeListener();
            InitializeEffortDegreeMapping();

            btnPreSubjScore.Enabled = false;
            btnPreCalcScore.Enabled = false;

            //如果是新竹市，將努力程度藏起來
            if (Global.Params["Mode"] == "HsinChu")
            {
                chsEffort.Visible = false;
                chdEffort.Visible = false;
            }
        }

        /// <summary>
        /// Constructor
        /// 新增模式
        /// </summary>
        public SemesterScoreEditor(JHStudentRecord student)
            : this()
        {
            _student = student;
            _calculator = new ScoreCalculator(student.ScoreCalcRule);

            _util = new SemesterHistoryUtility(JHSemesterHistory.SelectByStudentID(student.ID));
            _semesterScoreRecordList = JHSemesterScore.SelectByStudentID(student.ID);

            #region 填入學生資訊
            string classname = (student.Class != null) ? student.Class.Name + " " : "";
            string seatno = (!string.IsNullOrEmpty(classname) && !string.IsNullOrEmpty("" + student.SeatNo)) ? student.SeatNo + " " : "";
            string studentnumber = (!string.IsNullOrEmpty(student.StudentNumber)) ? " (" + student.StudentNumber + ")" : "";
            lblStudentInfo.Text = classname + seatno + student.Name + studentnumber;
            #endregion

            errorProvider.SetError(cboSchoolYear, "無效的學年度");
            errorProvider.SetError(cboSemester, "無效的學期");

            _listener.SuspendListen();
            _listener.Reset();
            _listener.ResumeListen();
        }

        /// <summary>
        /// Constructor
        /// 修改模式
        /// </summary>
        public SemesterScoreEditor(JHStudentRecord student, JHSemesterScoreRecord record)
            : this(student)
        {
            _isModifyMode = true;

            _record = record;
            cboSchoolYear.Text = "" + record.SchoolYear;
            cboSemester.Text = "" + record.Semester;
            cboSchoolYear.Enabled = cboSemester.Enabled = false;

            FillScore(record);
        }

        /// <summary>
        /// Constructor
        /// 檢視模式
        /// </summary>
        public SemesterScoreEditor(JHStudentRecord student, JHSemesterScoreRecord record, bool editable)
            : this(student)
        {
            _Editable = editable;
            _isModifyMode = true;

            _record = record;
            cboSchoolYear.Text = "" + record.SchoolYear;
            cboSemester.Text = "" + record.Semester;
            cboSchoolYear.Enabled = cboSemester.Enabled = false;

            FillScore(record);
        }

        /// <summary>
        /// 初始化努力程度對照表
        /// </summary>
        private void InitializeEffortDegreeMapping()
        {
            Global.Params = FISCA.ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");

            if (Global.Params["Mode"] == "HsinChu") return;

            _effortDict = new Dictionary<decimal, int>();
            _effortScoreList = new List<decimal>();

            ConfigData cd = K12.Data.School.Configuration["努力程度對照表"];
            if (!string.IsNullOrEmpty(cd["xml"]))
            {
                XmlElement element = K12.Data.XmlHelper.LoadXml(cd["xml"]);

                foreach (XmlElement each in element.SelectNodes("Effort"))
                {
                    int code = int.Parse(each.GetAttribute("Code"));
                    decimal score;
                    if (!decimal.TryParse(each.GetAttribute("Score"), out score))
                        score = 0;

                    if (!_effortDict.ContainsKey(score))
                        _effortDict.Add(score, code);
                }

                _effortScoreList.AddRange(_effortDict.Keys);
                _effortScoreList.Sort(delegate(decimal a, decimal b)
                {
                    return b.CompareTo(a);
                });
            }
        }

        /// <summary>
        /// 初始化 ChangeListener
        /// </summary>
        private void InitializeListener()
        {
            _listener = new Framework.ChangeListener();

            //_listener.Add(new ComboBoxSource(cboGradeYear, ComboBoxSource.ListenAttribute.Text));
            _listener.Add(new Framework.DataGridViewSource(dgvSubject));
            _listener.Add(new Framework.DataGridViewSource(dgvDomain));
            _listener.Add(new Framework.TextBoxSource(txtLearnDomainScore));
            _listener.Add(new Framework.TextBoxSource(txtCourseLearnScore));

            _listener.StatusChanged += delegate(object sender, Framework.ChangeEventArgs e)
            {
                lblSaveWarning.Visible = (e.Status == Framework.ValueStatus.Dirty);
            };
        }

        /// <summary>
        /// 初始化領域及科目
        /// </summary>
        private void InitializeDomainsAndSubjects()
        {
            List<string> domainList = new List<string>(Subject.Domains);
            List<string> subjectList = new List<string>(Subject.Subjects);

            chsDomain.Items.Clear();
            chsDomain.Items.AddRange(domainList);
            chsSubject.Items.Clear();
            chsSubject.Items.AddRange(subjectList);

            chdDomain.Items.Clear();
            chdDomain.Items.AddRange(domainList);
        }

        /// <summary>
        /// 初始化學年度、學期，這兩個ComboBox
        /// </summary>
        private void InitializeComboBoxes()
        {
            try
            {
                //cboSchoolYear.Enabled = cboSemester.Enabled = cboGradeYear.Enabled = false;
                cboSchoolYear.Enabled = cboSemester.Enabled = false;
                int schoolYear;
                if (!int.TryParse(K12.Data.School.DefaultSchoolYear, out schoolYear))
                    schoolYear = 97;

                for (int i = -2; i <= 2; i++)
                    cboSchoolYear.Items.Add(schoolYear + i);

                foreach (int item in new int[] { 1, 2 })
                    cboSemester.Items.Add(item);

                //cboSchoolYear.Enabled = cboSemester.Enabled = cboGradeYear.Enabled = true;
                cboSchoolYear.Enabled = cboSemester.Enabled = true;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 將成績填入畫面
        /// </summary>
        /// <param name="record"></param>
        private void FillScore(JHSemesterScoreRecord record)
        {
            _listener.SuspendListen();

            //填入科目成績
            FillSubjectScore(record);
            //排序科目
            SortSubjectGrid();

            //填入領域成績
            FillDomainScore(record);
            //排序領域
            SortDomainGrid();

            //填入學習領域成績
            txtLearnDomainScore.Text = "" + record.LearnDomainScore;
            //填入課程學習成績
            txtCourseLearnScore.Text = "" + record.CourseLearnScore;
            //填入原始學習領域成績
            txtLearnDomainScoreOrigin.Text = "" + record.LearnDomainScoreOrigin;
            //填入原始課程學習成績
            txtCourseLearnScoreOrigin.Text = "" + record.CourseLearnScoreOrigin;

            _listener.Reset();
            _listener.ResumeListen();
        }

        /// <summary>
        /// 排序科目
        /// </summary>
        private void SortSubjectGrid()
        {
            List<DataGridViewRow> list = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvSubject.Rows)
            {
                if (row.IsNewRow) continue;
                list.Add(row);
            }
            list.Sort(delegate(DataGridViewRow x, DataGridViewRow y)
            {
                Subject subjectX = new Subject("" + x.Cells[chsSubject.Index].Value, "" + x.Cells[chsDomain.Index].Value);
                Subject subjectY = new Subject("" + y.Cells[chsSubject.Index].Value, "" + y.Cells[chsDomain.Index].Value);
                return Subject.CompareOrdinal(subjectX, subjectY);
            });
            dgvSubject.SuspendLayout();
            dgvSubject.Rows.Clear();
            foreach (DataGridViewRow row in list)
                dgvSubject.Rows.Add(row);
            dgvSubject.ResumeLayout();
        }

        /// <summary>
        /// 排序領域
        /// </summary>
        private void SortDomainGrid()
        {
            List<DataGridViewRow> list = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvDomain.Rows)
            {
                if (row.IsNewRow) continue;
                list.Add(row);
            }
            list.Sort(delegate(DataGridViewRow x, DataGridViewRow y)
            {
                string domainX = "" + x.Cells[chdDomain.Index].Value;
                string domainY = "" + y.Cells[chdDomain.Index].Value;
                return Subject.CompareDomainOrdinal(domainX, domainY);
            });
            dgvDomain.SuspendLayout();
            dgvDomain.Rows.Clear();
            foreach (DataGridViewRow row in list)
                dgvDomain.Rows.Add(row);
            dgvDomain.ResumeLayout();
        }

        /// <summary>
        /// 填入科目成績
        /// </summary>
        /// <param name="record"></param>
        private void FillSubjectScore(JHSemesterScoreRecord record)
        {
            dgvSubject.SuspendLayout();
            dgvSubject.Rows.Clear();
            foreach (K12.Data.SubjectScore subject in record.Subjects.Values)
            {
                string periodCredit = "";
                PeriodCredit pc = new PeriodCredit();
                if (pc.Parse(subject.Period + "/" + subject.Credit))
                    periodCredit = pc.ToString();

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvSubject,
                    subject.Domain,
                    subject.Subject,
                    periodCredit,
                    "" + subject.Score,
                    "" + subject.ScoreOrigin,
                    "" + subject.ScoreMakeup,
                    "" + subject.Effort,
                    subject.Text,
                    subject.Comment);
                dgvSubject.Rows.Add(row);

                if (subject.Score < 60)
                    row.Cells[chsScore.Index].Style.ForeColor = Color.Red;
                if (subject.Score > 100 || subject.Score < 0)
                    row.Cells[chsScore.Index].Style.ForeColor = Color.Green;

                if (subject.ScoreOrigin < 60)
                    row.Cells[chsScoreOrigin.Index].Style.ForeColor = Color.Red;
                if (subject.ScoreOrigin > 100 || subject.ScoreOrigin < 0)
                    row.Cells[chsScoreOrigin.Index].Style.ForeColor = Color.Green;

                if (subject.ScoreMakeup < 60)
                    row.Cells[chsScoreMakeup.Index].Style.ForeColor = Color.Red;
                if (subject.ScoreMakeup > 100 || subject.ScoreMakeup < 0)
                    row.Cells[chsScoreMakeup.Index].Style.ForeColor = Color.Green;
            }
            dgvSubject.ResumeLayout();
        }

        /// <summary>
        /// 填入領域成績
        /// </summary>
        /// <param name="domains"></param>
        private void FillDomainScore(JHSemesterScoreRecord record)
        {
            dgvDomain.SuspendLayout();
            dgvDomain.Rows.Clear();
            foreach (K12.Data.DomainScore domain in record.Domains.Values)
            {
                string periodCredit = "";
                PeriodCredit pc = new PeriodCredit();
                if (pc.Parse(domain.Period + "/" + domain.Credit))
                    periodCredit = pc.ToString();

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvDomain,
                    domain.Domain,
                    periodCredit,
                    "" + domain.Score,
                    "" + domain.ScoreOrigin,
                    "" + domain.ScoreMakeup,
                    "" + domain.Effort,
                    domain.Text,
                    domain.Comment);
                dgvDomain.Rows.Add(row);

                if (domain.Score < 60)
                    row.Cells[chdScore.Index].Style.ForeColor = Color.Red;
                if (domain.Score > 100 || domain.Score < 0)
                    row.Cells[chdScore.Index].Style.ForeColor = Color.Green;

                if (domain.ScoreOrigin < 60)
                    row.Cells[chScoreOrigin.Index].Style.ForeColor = Color.Red;
                if (domain.ScoreOrigin > 100 || domain.Score < 0)
                    row.Cells[chScoreOrigin.Index].Style.ForeColor = Color.Green;

                if (domain.ScoreMakeup < 60)
                    row.Cells[chScoreMakeup.Index].Style.ForeColor = Color.Red;
                if (domain.ScoreMakeup > 100 || domain.Score < 0)
                    row.Cells[chScoreMakeup.Index].Style.ForeColor = Color.Green;
            }
            dgvDomain.ResumeLayout();
        }

        /// <summary>
        /// 變更 DataGridView Enable
        /// </summary>
        /// <param name="dgv"></param>
        private void EvaluateEnabled()
        {
            if (_Editable)
            {
                btnPreSubjScore.Enabled = btnPreCalcScore.Enabled = btnSave.Enabled = gpSubject.Enabled = gpDomain.Enabled = _semesterIsValid;
            }
            else
            {
                btnSave.Enabled = false;
                gpSubject.Enabled = true;
                gpDomain.Enabled = true;
                _semesterIsValid = false;

                btnPreSubjScore.Enabled = false;
                btnPreCalcScore.Enabled = false;
            }
        }

        /// <summary>
        /// 依努力程度對照表，產生對應的努力程度代碼
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private int GenerateEffortCode(decimal d)
        {
            foreach (decimal score in _effortScoreList)
                if (d >= score) return _effortDict[score];

            return _effortDict[_effortScoreList[_effortScoreList.Count - 1]];
        }

        /// <summary>
        /// 依課程規劃，自動填入科目
        /// </summary>
        private void AutoFillSubjects()
        {
            _filled = true;

            JHProgramPlanRecord record = _student.ProgramPlan;
            if (record == null) return;

            dgvSubject.SuspendLayout();

            foreach (K12.Data.ProgramSubject subject in record.Subjects)
            {
                if ("" + subject.Semester == cboSemester.Text &&
                    subject.GradeYear == _util.GetGradeYear(cboSchoolYear.Text, cboSemester.Text))
                {
                    string pc = string.Empty;
                    PeriodCredit pcobj = new PeriodCredit();
                    pcobj.Parse(subject.Period + "/" + subject.Credit);
                    pc = pcobj.ToString();

                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvSubject,
                        subject.Domain,
                        subject.SubjectName,
                        pc);
                    dgvSubject.Rows.Add(row);
                }
            }

            SortSubjectGrid();
            dgvSubject.ResumeLayout();
        }

        /// <summary>
        /// 按下「儲存」
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValid()) return;
            Save();
            this.DialogResult = DialogResult.OK;
        }

        /// <summar>
        /// 按下「關閉」
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 關閉前，提示使用者儲存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SemesterScoreEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK) return;
            if (!lblSaveWarning.Visible) return;

            if (MsgBox.Show("資料尚未儲存，您確定要離開嗎？", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                this.DialogResult = DialogResult.None;
            }
        }

        /// <summary>
        /// 儲存
        /// </summary>
        private void Save()
        {
            if (_record == null)
            {
                int schoolYear = int.Parse(cboSchoolYear.Text);
                int semester = int.Parse(cboSemester.Text);
                _record = new JHSchool.Data.JHSemesterScoreRecord();
                _record.RefStudentID = _student.ID;
                _record.SchoolYear = schoolYear;
                _record.Semester = semester;
            }

            #region 從畫面上取得科目成績內容
            _record.Subjects.Clear();
            foreach (DataGridViewRow row in dgvSubject.Rows)
            {
                if (row.IsNewRow) continue;
                if (IsEmptyRow(row)) continue;

                K12.Data.SubjectScore subject = new K12.Data.SubjectScore();
                subject.Domain = "" + row.Cells[chsDomain.Index].Value;
                subject.Subject = "" + row.Cells[chsSubject.Index].Value;

                PeriodCredit pc = new PeriodCredit();
                string periodCredit = "" + row.Cells[chsPeriodCredit.Index].Value;
                pc.Parse(periodCredit);

                subject.Period = pc.Period;
                subject.Credit = pc.Credit;

                subject.Score = GetDecimalValue("" + row.Cells[chsScore.Index].Value);

                subject.ScoreOrigin = GetDecimalValue("" + row.Cells[chsScoreOrigin.Index].Value);
                subject.ScoreMakeup = GetDecimalValue("" + row.Cells[chsScoreMakeup.Index].Value);

                subject.Effort = string.IsNullOrEmpty("" + row.Cells[chsEffort.Index].Value) ? null : (int?)int.Parse("" + row.Cells[chsEffort.Index].Value);
                subject.Text = "" + row.Cells[chsText.Index].Value;
                subject.Comment = "" + row.Cells[chsComment.Index].Value;

                _record.Subjects.Add(subject.Subject, subject);
            }
            #endregion

            #region 從畫面上取得領域成績內容
            _record.Domains.Clear();
            foreach (DataGridViewRow row in dgvDomain.Rows)
            {
                if (row.IsNewRow) continue;
                if (IsEmptyRow(row)) continue;

                K12.Data.DomainScore domain = new K12.Data.DomainScore();
                domain.Domain = "" + row.Cells[chdDomain.Index].Value;

                PeriodCredit pc = new PeriodCredit();
                string periodCredit = "" + row.Cells[chdPeriodCredit.Index].Value;
                pc.Parse(periodCredit);

                domain.Period = pc.Period;
                domain.Credit = pc.Credit;

                domain.Score = GetDecimalValue("" + row.Cells[chdScore.Index].Value);
                domain.ScoreOrigin = GetDecimalValue("" + row.Cells[chScoreOrigin.Index].Value);
                domain.ScoreMakeup = GetDecimalValue("" + row.Cells[chScoreMakeup.Index].Value);

                domain.Effort = string.IsNullOrEmpty("" + row.Cells[chdEffort.Index].Value) ? null : (int?)int.Parse("" + row.Cells[chdEffort.Index].Value);
                domain.Text = "" + row.Cells[chdText.Index].Value;
                //domain.Comment = "" + row.Cells[chdComment.Index].Value;
                domain.Comment = "" + row.Cells[chMemo.Index].Value;

                _record.Domains.Add(domain.Domain, domain);
            }
            #endregion

            decimal d;
            _record.LearnDomainScore = decimal.TryParse(txtLearnDomainScore.Text, out d) ? (decimal?)d : null;
            _record.CourseLearnScore = decimal.TryParse(txtCourseLearnScore.Text, out d) ? (decimal?)d : null;
            _record.LearnDomainScoreOrigin = decimal.TryParse(txtLearnDomainScoreOrigin.Text, out d) ? (decimal?)d : null;
            _record.CourseLearnScoreOrigin = decimal.TryParse(txtCourseLearnScoreOrigin.Text, out d) ? (decimal?)d : null;

            if (string.IsNullOrEmpty(_record.ID))
                JHSemesterScore.Insert(_record);
            else
                JHSemesterScore.Update(_record);
        }

        /// <summary>
        /// 檢查畫面是否有錯誤訊息
        /// </summary>
        /// <returns></returns>
        private bool IsValid()
        {
            bool valid = true;

            //科目
            dgvSubject.EndEdit();
            foreach (DataGridViewRow row in dgvSubject.Rows)
            {
                if (row.IsNewRow) continue;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                        valid = false;
                }
            }

            //領域
            dgvDomain.EndEdit();
            foreach (DataGridViewRow row in dgvDomain.Rows)
            {
                if (row.IsNewRow) continue;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                        valid = false;
                }
            }

            //學習領域成績
            if (!string.IsNullOrEmpty(errorProvider.GetError(txtLearnDomainScore)))
                valid = false;

            //課程學習成績
            if (!string.IsNullOrEmpty(errorProvider.GetError(txtCourseLearnScore)))
                valid = false;

            return valid;
        }

        /// <summary>
        /// 當學年度、學期改變時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SchoolYearAndSemester_TextChanged(object sender, EventArgs e)
        {
            if (ValidSemester())
            {
                _semesterIsValid = true;
                SemesterChanged(sender);

                if (!_isModifyMode && _semesterIsValid && !_filled)
                    AutoFillSubjects();
            }
            else
                _semesterIsValid = false;
            EvaluateEnabled();
        }

        /// <summary>
        /// 驗證學年度學期
        /// </summary>
        /// <returns></returns>
        private bool ValidSemester()
        {
            bool valid = true;

            errorProvider.SetError(cboSchoolYear, "");
            errorProvider.SetError(cboSemester, "");

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

        /// <summary>
        /// 在新增模式時，當學年度學期改變，檢查是否該學年度學期已有資料，如果有資料，則將畫面 Disable。
        /// </summary>
        private void SemesterChanged(object sender)
        {
            if (_record != null) return;

            int schoolYear = int.Parse(cboSchoolYear.Text);
            int semester = int.Parse(cboSemester.Text);

            bool found = false;

            foreach (JHSemesterScoreRecord record in _semesterScoreRecordList)
            {
                if (record.SchoolYear == schoolYear && record.Semester == semester)
                {
                    found = true;
                    break;
                }
            }

            _semesterIsValid = !found;
            EvaluateEnabled();

            if (found)
                toolTip.Show("此學年度學期已有資料", sender as Control, 10, 25);
            else
                toolTip.RemoveAll();
        }

        /// <summary>
        /// 當「學習領域成績」及「課程學習成績」改變時，進行驗證
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Score_TextChanged(object sender, EventArgs e)
        {
            DevComponents.DotNetBar.Controls.TextBoxX score = sender as DevComponents.DotNetBar.Controls.TextBoxX;

            errorProvider.SetError(score, "");
            if (!string.IsNullOrEmpty(score.Text))
            {
                decimal d;
                if (!decimal.TryParse(score.Text, out d))
                    errorProvider.SetError(score, "成績必須為數字");
            }
        }

        #region DataGridView Event
        /// <summary>
        /// 結束編輯時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewX dgv = sender as DataGridViewX;
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (dgv.Name == "dgvSubject")
                ValidSubjectRequiredField(cell.OwningRow);
            else if (dgv.Name == "dgvDomain")
                ValidDomainRequiredField(cell.OwningRow);

            ValidCell(cell);
        }

        /// <summary>
        /// 加入新的 Row 時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridViewX dgv = sender as DataGridViewX;
            if (dgv.Name == "dgvSubject")
                ValidSubjectRequiredField(dgv.Rows[e.RowIndex]);
            else if (dgv.Name == "dgvDomain")
                ValidDomainRequiredField(dgv.Rows[e.RowIndex]);
        }

        /// <summary>
        /// 在 RowHeader 按下右鍵，觸發 ContextMenuStrip
        /// 在 Cell 按下左鍵，進入編輯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewX dgv = sender as DataGridViewX;
            if (e.ColumnIndex < 0 && e.Button == MouseButtons.Right)
            {
                dgv.EndEdit();
                _selectedRowIndex = e.RowIndex;
                foreach (DataGridViewRow row in dgv.SelectedRows)
                {
                    if (row.Index != _selectedRowIndex)
                        row.Selected = false;
                }
                dgv.Rows[_selectedRowIndex].Selected = true;
                if (dgv.Name == "dgvSubject")
                    contextMenuStripSubject.Show(dgv, dgv.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
                else if (dgv.Name == "dgvDomain")
                    contextMenuStripDomain.Show(dgv, dgv.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
            }
            else if (e.ColumnIndex >= 0 && e.Button == MouseButtons.Left)
            {
                dgv.BeginEdit(true);
            }
            else
                dgv.EndEdit();
        }

        /// <summary>
        /// RowHeader MouseClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewX dgv = sender as DataGridViewX;
            if (e.Button == MouseButtons.Left)
                dgv.EndEdit();
        }

        #endregion

        #region Validation
        /// <summary>
        /// 驗證 Cell
        /// </summary>
        /// <param name="cell"></param>
        private void ValidCell(DataGridViewCell cell)
        {
            if (cell.OwningRow.IsNewRow) return;
            if (IsEmptyRow(cell.OwningRow)) return;

            #region 驗證科目成績相關欄位

            #region 驗證努力程度欄位
            if (cell.OwningColumn == chsEffort)
            {
                if (string.IsNullOrEmpty("" + cell.Value))
                    cell.ErrorText = string.Empty;
                else
                {
                    int i;
                    if (!int.TryParse("" + cell.Value, out i))
                        cell.ErrorText = "努力程度必須為整數";
                    else
                        cell.ErrorText = string.Empty;
                }
            }
            #endregion

            #region 驗證成績欄位
            if (cell.OwningColumn == chsScore)
            {
                if (string.IsNullOrEmpty("" + cell.Value))
                    cell.ErrorText = "成績不能為空白";
                else
                {
                    decimal d;
                    if (!decimal.TryParse("" + cell.Value, out d))
                        cell.ErrorText = "成績必須為數字";
                    else
                    {
                        cell.ErrorText = string.Empty;

                        #region 不及格變紅色
                        if (d < 60)
                            cell.Style.ForeColor = Color.Red;
                        else
                            cell.Style.ForeColor = Color.Black;
                        #endregion

                        if (d > 100 || d < 0)
                            cell.Style.ForeColor = Color.Green;

                        #region 自動換算出努力程度
                        if (_effortScoreList != null && _effortScoreList.Count > 0)
                        {
                            DataGridViewCell effortCell = cell.OwningRow.Cells[chsEffort.Index];
                            string orig = "" + effortCell.Value;
                            effortCell.Value = GenerateEffortCode(d);
                            effortCell.ErrorText = string.Empty;
                            if (orig != "" + effortCell.Value) dgvSubject.NotifyCurrentCellDirty(true);
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #region 驗證原始成績欄位
            if (cell.OwningColumn == chsScoreOrigin)
            {
                cell.ErrorText = string.Empty;
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    decimal d;
                    if (!decimal.TryParse("" + cell.Value, out d))
                        cell.ErrorText = "原始成績必須為數字";
                    else
                    {
                        #region 不及格變紅色
                        if (d < 60)
                            cell.Style.ForeColor = Color.Red;
                        else
                            cell.Style.ForeColor = Color.Black;
                        #endregion

                        if (d > 100 || d < 0)
                            cell.Style.ForeColor = Color.Green;
                    }
                }
            }
            #endregion

            #region 驗證補考成績欄位
            if (cell.OwningColumn == chsScoreMakeup)
            {
                cell.ErrorText = string.Empty;
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    decimal d;
                    if (!decimal.TryParse("" + cell.Value, out d))
                        cell.ErrorText = "補考成績必須為數字";
                    else
                    {
                        #region 不及格變紅色
                        if (d < 60)
                            cell.Style.ForeColor = Color.Red;
                        else
                            cell.Style.ForeColor = Color.Black;
                        #endregion

                        if (d > 100 || d < 0)
                            cell.Style.ForeColor = Color.Green;
                    }
                }
            }
            #endregion

            #region 驗證節數/權數欄位
            if (cell.OwningColumn == chsPeriodCredit)
            {
                PeriodCredit pc = new PeriodCredit();
                cell.ErrorText = string.Empty;
                if (!pc.Parse("" + cell.Value))
                    cell.ErrorText = pc.Error;
                else
                {
                    if (pc.Credit.CompareTo(pc.Period) == 0)
                    {
                        cell.Value = pc.ToString();
                        dgvSubject.NotifyCurrentCellDirty(true);
                    }
                }
            }
            #endregion

            #region 驗證科目欄位
            if (cell.OwningColumn == chsSubject)
            {
                cell.ErrorText = string.Empty;
                if (string.IsNullOrEmpty("" + cell.Value))
                    cell.ErrorText = "科目不能為空白";
                else
                {
                    List<string> subjectList = new List<string>();
                    foreach (DataGridViewRow row in dgvSubject.Rows)
                    {
                        if (row.IsNewRow) continue;
                        DataGridViewCell subjectCell = row.Cells[chsSubject.Index];

                        subjectCell.ErrorText = string.Empty;
                        if (!subjectList.Contains("" + subjectCell.Value))
                            subjectList.Add("" + subjectCell.Value);
                        else
                            subjectCell.ErrorText = "科目名稱不能重覆";
                    }
                }
            }
            #endregion

            #endregion

            #region 驗證領域成績相關欄位

            #region 驗證努力程度欄位
            if (cell.OwningColumn == chdEffort)
            {
                cell.ErrorText = string.Empty;
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    int i;
                    if (!int.TryParse("" + cell.Value, out i))
                        cell.ErrorText = "努力程度必須為整數";
                }
            }
            #endregion

            #region 驗證成績欄位
            if (cell.OwningColumn == chdScore)
            {
                cell.ErrorText = string.Empty;
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    decimal d;
                    if (!decimal.TryParse("" + cell.Value, out d))
                        cell.ErrorText = "成績必須為數字";
                    else
                    {
                        #region 不及格變紅色
                        if (d < 60)
                            cell.Style.ForeColor = Color.Red;
                        else
                            cell.Style.ForeColor = Color.Black;
                        #endregion

                        if (d > 100 || d < 0)
                            cell.Style.ForeColor = Color.Green;

                        #region 自動換算出努力程度
                        if (_effortScoreList != null && _effortScoreList.Count > 0)
                        {
                            cell.OwningRow.Cells[chdEffort.Index].Value = GenerateEffortCode(d);
                            cell.OwningRow.Cells[chdEffort.Index].ErrorText = "";
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #region 驗證原始成績欄位
            if (cell.OwningColumn == chScoreOrigin)
            {
                cell.ErrorText = string.Empty;
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    decimal d;
                    if (!decimal.TryParse("" + cell.Value, out d))
                        cell.ErrorText = "原始成績必須為數字";
                    else
                    {
                        #region 不及格變紅色
                        if (d < 60)
                            cell.Style.ForeColor = Color.Red;
                        else
                            cell.Style.ForeColor = Color.Black;
                        #endregion

                        if (d > 100 || d < 0)
                            cell.Style.ForeColor = Color.Green;
                    }
                }
            }
            #endregion

            #region 驗證補考成績欄位
            if (cell.OwningColumn == chScoreMakeup)
            {
                cell.ErrorText = string.Empty;
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    decimal d;
                    if (!decimal.TryParse("" + cell.Value, out d))
                        cell.ErrorText = "補考成績必須為數字";
                    else
                    {
                        #region 不及格變紅色
                        if (d < 60)
                            cell.Style.ForeColor = Color.Red;
                        else
                            cell.Style.ForeColor = Color.Black;
                        #endregion

                        if (d > 100 || d < 0)
                            cell.Style.ForeColor = Color.Green;
                    }
                }
            }
            #endregion

            #region 驗證節數/權數欄位
            if (cell.OwningColumn == chdPeriodCredit)
            {
                PeriodCredit pc = new PeriodCredit();
                cell.ErrorText = string.Empty;
                if (!pc.Parse("" + cell.Value))
                    cell.ErrorText = pc.Error;
                else
                {
                    if (pc.Credit.CompareTo(pc.Period) == 0)
                    {
                        cell.Value = pc.ToString();
                        dgvDomain.NotifyCurrentCellDirty(true);
                    }
                }
            }
            #endregion

            #region 驗證領域欄位
            if (cell.OwningColumn == chdDomain)
            {
                cell.ErrorText = string.Empty;
                if (string.IsNullOrEmpty("" + cell.Value))
                    cell.ErrorText = "領域不能為空白";
                else
                {
                    List<string> domainList = new List<string>();
                    foreach (DataGridViewRow row in dgvDomain.Rows)
                    {
                        if (row.IsNewRow) continue;
                        DataGridViewCell domainCell = row.Cells[chdDomain.Index];

                        domainCell.ErrorText = string.Empty;
                        if (!domainList.Contains("" + domainCell.Value))
                            domainList.Add("" + domainCell.Value);
                        else
                            domainCell.ErrorText = "科目名稱不能重覆";
                    }
                }
            }
            #endregion

            #endregion
        }

        /// <summary>
        /// 驗證科目必要欄位是否為空白
        /// </summary>
        /// <param name="row"></param>
        private void ValidSubjectRequiredField(DataGridViewRow row)
        {
            if (row.IsNewRow) return;

            if (IsEmptyRow(row))
            {
                row.Cells[chsSubject.Index].ErrorText = "";
                row.Cells[chsPeriodCredit.Index].ErrorText = "";
                row.Cells[chsScore.Index].ErrorText = "";
                return;
            }

            if (string.IsNullOrEmpty("" + row.Cells[chsSubject.Index].Value))
                row.Cells[chsSubject.Index].ErrorText = "科目不能為空白";
            if (string.IsNullOrEmpty("" + row.Cells[chsPeriodCredit.Index].Value))
                row.Cells[chsPeriodCredit.Index].ErrorText = "節數/權數不能為空白";
            if (string.IsNullOrEmpty("" + row.Cells[chsScore.Index].Value))
                row.Cells[chsScore.Index].ErrorText = "成績不能為空白";
        }

        private bool IsEmptyRow(DataGridViewRow row)
        {
            if (Global.Params["Mode"].ToUpper() == "HsinChu".ToUpper())
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.OwningColumn == chsEffort || cell.OwningColumn == chdEffort) continue;
                    if (!string.IsNullOrEmpty("" + cell.Value)) return false;
                }
            }
            else
            {
                foreach (DataGridViewCell cell in row.Cells)
                    if (!string.IsNullOrEmpty("" + cell.Value)) return false;
            }
            return true;
        }

        /// <summary>
        /// 驗證領域必要欄位是否為空白
        /// </summary>
        /// <param name="row"></param>
        private void ValidDomainRequiredField(DataGridViewRow row)
        {
            if (row.IsNewRow) return;

            if (IsEmptyRow(row))
            {
                row.Cells[chdDomain.Index].ErrorText = "";
                row.Cells[chdPeriodCredit.Index].ErrorText = "";
                row.Cells[chdScore.Index].ErrorText = "";
                return;
            }

            //if (string.IsNullOrEmpty("" + row.Cells[chdDomain.Index].Value))
            //    row.Cells[chdDomain.Index].ErrorText = "領域不能為空白";
            if (string.IsNullOrEmpty("" + row.Cells[chdPeriodCredit.Index].Value))
                row.Cells[chdPeriodCredit.Index].ErrorText = "節數/權數不能為空白";
            if (string.IsNullOrEmpty("" + row.Cells[chdScore.Index].Value))
                row.Cells[chdScore.Index].ErrorText = "成績不能為空白";
        }
        #endregion

        #region ContextMenuStrip Events
        /// <summary>
        /// Subject ContextMenuStrip，按下「插入」
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubjectInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuInsertRow(dgvSubject);
        }

        /// <summary>
        /// Subject ContextMenuStrip，按下「刪除」
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubjectDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuDeleteRow(dgvSubject);
        }

        /// <summary>
        /// Domain ContextMenuStrip，按下「插入」
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DomainInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuInsertRow(dgvDomain);
        }

        /// <summary>
        /// Domain ContextMenuStrip，按下「刪除」
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DomainDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuDeleteRow(dgvDomain);
        }

        /// <summary>
        /// ContextStripMenu Insert Row
        /// </summary>
        /// <param name="dgv"></param>
        private void ToolStripMenuInsertRow(DataGridView dgv)
        {
            dgv.Rows.Insert(_selectedRowIndex, new DataGridViewRow());
        }

        /// <summary>
        /// ContextStripMenu Delete Row
        /// </summary>
        /// <param name="dgv"></param>
        private void ToolStripMenuDeleteRow(DataGridView dgv)
        {
            if (_selectedRowIndex >= 0 && dgv.Rows.Count - 1 > _selectedRowIndex)
                dgv.Rows.RemoveAt(_selectedRowIndex);
        }
        #endregion

        /// <summary>
        /// 領域成績試算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreCalcScore_Click(object sender, EventArgs e)
        {
            #region 檢查成績年級及科目是否有錯誤
            bool valid = true;

            int SubjHasDomainCount = 0;

            dgvSubject.EndEdit();
            foreach (DataGridViewRow row in dgvSubject.Rows)
            {
                // 檢查畫面上是否有科目的領域
                if (row.Cells[chsDomain.Index].Value != null)
                    if (row.Cells[chsDomain.Index].Value.ToString().Trim() != "")
                        SubjHasDomainCount++;

                if (row.IsNewRow) continue;
                if (IsEmptyRow(row)) continue;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                        valid = false;
                }
            }

            if (!valid) return;
            #endregion

            //2015.06.10 by Cloud
            DomainScoreSetting setting = new DomainScoreSetting(false, chkScoreLimit.Checked);

            // 是否只計算領域
            bool OnlyCalcDomain = false;
            if (dgvSubject.Rows.Count == 0 || SubjHasDomainCount == 0)
                OnlyCalcDomain = true;
            else
                OnlyCalcDomain = false;

            #region 計算領域成績
            StudentScore.SetClassMapping();
            StudentScore studentScore = new StudentScore(_student);

            JHSemesterScoreRecord tempRecord;

            if (_record == null)
                tempRecord = new JHSemesterScoreRecord();
            else
                tempRecord = JHSemesterScore.SelectBySchoolYearAndSemester(_student.ID, _record.SchoolYear, _record.Semester);

            if (OnlyCalcDomain)
                tempRecord = GetSemesterScoreRecordFromDomainGrid(tempRecord);
            else
            {
                tempRecord = GetSemesterScoreRecordFromDomainGrid(tempRecord);
                tempRecord = GetSemesterScoreRecordFromSubjectGrid(tempRecord);
            }

            SemesterScore semesterScore = new SemesterScore(tempRecord);
            studentScore.SemestersScore.Add(SemesterData.Empty, semesterScore);

            List<StudentScore> students = new List<StudentScore>();
            students.Add(studentScore);

            //讀取計算規則
            students.ReadCalculationRule(null);
            //計算領域成績
            //students.CalcuateDomainSemesterScore(new string[] { });
            students.CalcuateDomainSemesterScore(new string[] { },setting);

            dgvDomain.SuspendLayout();
            dgvDomain.Rows.Clear();
            foreach (string domain in semesterScore.Domain)
            {
                if (domain == "__彈性課程" || domain == "") continue;

                SemesterDomainScore domainScore = semesterScore.Domain[domain];

                string periodCredit = "";
                PeriodCredit pc = new PeriodCredit();
                if (pc.Parse(domainScore.Period + "/" + domainScore.Weight))
                    periodCredit = pc.ToString();

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvDomain,
                    domain,
                    periodCredit,
                    "" + domainScore.Value,
                    "" + domainScore.ScoreOrigin,
                    "" + domainScore.ScoreMakeup,
                    "" + domainScore.Effort,
                    domainScore.Text,
                    string.Empty);
                dgvDomain.Rows.Add(row);

                if (domainScore.Value < 60)
                    row.Cells[chdScore.Index].Style.ForeColor = Color.Red;
                if (domainScore.Value > 100 || domainScore.Value < 0)
                    row.Cells[chdScore.Index].Style.ForeColor = Color.Green;
            }
            dgvDomain.ResumeLayout();

            //排序領域
            SortDomainGrid();

            txtLearnDomainScore.Text = "" + semesterScore.LearnDomainScore;
            txtCourseLearnScore.Text = "" + semesterScore.CourseLearnScore;
            txtLearnDomainScoreOrigin.Text = "" + semesterScore.LearnDomainScoreOrigin;
            txtCourseLearnScoreOrigin.Text = "" + semesterScore.CourseLearnScoreOrigin;

            #endregion
        }


        /// <summary>
        /// 從畫面上讀取領域成績
        /// </summary>
        /// <returns></returns>
        private JHSemesterScoreRecord GetSemesterScoreRecordFromDomainGrid(JHSemesterScoreRecord record)
        {
            JHSemesterScoreRecord temp = record;
            temp.RefStudentID = _student.ID;
            temp.SchoolYear = int.Parse(cboSchoolYear.Text);
            temp.Semester = int.Parse(cboSemester.Text);

            foreach (DataGridViewRow row in dgvDomain.Rows)
            {
                if (row.IsNewRow) continue;
                if (IsEmptyRow(row)) continue;

                string dn = row.Cells[chdDomain.Index].Value.ToString();

                //這樣寫不知道會不會爆掉。
                K12.Data.DomainScore dDomainScore = new K12.Data.DomainScore();
                if (temp.Domains.ContainsKey(dn))
                    dDomainScore = temp.Domains[dn];
                else
                    temp.Domains.Add(dn, dDomainScore);

                PeriodCredit pc = new PeriodCredit();

                pc.Parse("" + row.Cells[chdPeriodCredit.Index].Value);

                dDomainScore.Domain = row.Cells[chdDomain.Index].Value.ToString();
                dDomainScore.Period = pc.Period;
                dDomainScore.Credit = pc.Credit;
                dDomainScore.Score = GetDecimalValue("" + row.Cells[chdScore.Index].Value);
                dDomainScore.ScoreOrigin = GetDecimalValue("" + row.Cells[chScoreOrigin.Index].Value);
                dDomainScore.ScoreMakeup = GetDecimalValue("" + row.Cells[chScoreMakeup.Index].Value);
                int i;
                dDomainScore.Effort = int.TryParse("" + row.Cells[chdEffort.Index].Value, out i) ? (int?)i : null;
                dDomainScore.Text = "" + row.Cells[chdText.Index].Value;
                dDomainScore.Comment = "" + row.Cells[chdText.Index].Value;
            }
            return temp;
        }

        private decimal? GetDecimalValue(string strScore)
        {
            decimal score;

            if (Decimal.TryParse(strScore, out score))
                return score;
            else
                return null;
        }
        /// <summary>
        /// 從畫面上的科目成績轉成JHSemesterScoreRecord
        /// </summary>
        /// <returns></returns>
        private JHSemesterScoreRecord GetSemesterScoreRecordFromSubjectGrid(JHSemesterScoreRecord record)
        {
            JHSemesterScoreRecord temp = record;
            temp.RefStudentID = _student.ID;
            temp.SchoolYear = int.Parse(cboSchoolYear.Text);
            temp.Semester = int.Parse(cboSemester.Text);

            temp.Subjects.Clear();
            foreach (DataGridViewRow row in dgvSubject.Rows)
            {
                if (row.IsNewRow) continue;
                if (IsEmptyRow(row)) continue;

                PeriodCredit pc = new PeriodCredit();
                pc.Parse("" + row.Cells[chsPeriodCredit.Index].Value);
                int i;

                K12.Data.SubjectScore subject = new K12.Data.SubjectScore();
                subject.Domain = "" + row.Cells[chsDomain.Index].Value;
                subject.Subject = "" + row.Cells[chsSubject.Index].Value;
                subject.Period = pc.Period;
                subject.Credit = pc.Credit;
                subject.Score = GetDecimalValue("" + row.Cells[chsScore.Index].Value);

                subject.ScoreOrigin = GetDecimalValue("" + row.Cells[chsScoreOrigin.Index].Value);
                subject.ScoreMakeup = GetDecimalValue("" + row.Cells[chsScoreMakeup.Index].Value);

                subject.Effort = int.TryParse("" + row.Cells[chsEffort.Index].Value, out i) ? (int?)i : null;
                subject.Text = "" + row.Cells[chsText.Index].Value;
                subject.Comment = "" + row.Cells[chsComment.Index].Value;

                temp.Subjects.Add(subject.Subject, subject);
            }
            return temp;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new SemesterScoreItemHelp().ShowDialog();
        }

        private void btnPreSubjScore_Click(object sender, EventArgs e)
        {
            //科目名稱重覆算下去會爆炸,所以這邊先檢查一次
            List<string> subjs = new List<string>();
            foreach (DataGridViewRow row in dgvSubject.Rows)
            {
                if (row.IsNewRow) continue;
                if(IsEmptyRow((row))) continue;

                string subj = row.Cells[chsSubject.Index].Value + "";

                if (subjs.Contains(subj))
                {
                    MessageBox.Show("科目名稱重覆,無法進行試算");
                    return;
                }
                else
                    subjs.Add(subj);
            }

            StudentScore.SetClassMapping();
            StudentScore studentScore = new StudentScore(_student);

            JHSemesterScoreRecord tempRecord;

            if (_record == null)
                tempRecord = new JHSemesterScoreRecord();
            else 
                tempRecord = JHSemesterScore.SelectBySchoolYearAndSemester(_student.ID, _record.SchoolYear, _record.Semester);
            
            tempRecord = GetSemesterScoreRecordFromSubjectGrid(tempRecord);

            SemesterScore semesterScore = new SemesterScore(tempRecord);
            studentScore.SemestersScore.Add(SemesterData.Empty, semesterScore);

            List<StudentScore> students = new List<StudentScore>();
            students.Add(studentScore);



            // 2016/ 6/7 穎驊製作，恩正說從這邊先於Students.ReadAttendScore 攔科目名稱重覆，否則進去攔的資料太複雜，超級崩潰
            #region 檢查科目重複問題
           
            List<JHSCAttendRecord> scAttendList = JHSCAttend.SelectByStudentID(_student.ID);

            Dictionary<string, List<JHSCAttendRecord>> scAttendCheck = new Dictionary<string, List<JHSchool.Data.JHSCAttendRecord>>();
            
            // 檢查重覆修習科目用字典
            Dictionary<string, List<JHSCAttendRecord>> duplicateErrorCourse = new Dictionary<string, List<JHSCAttendRecord>>();

            //把課程讀下來快取，穎驊筆記: 這一行超級重要，如果沒有的話至少多100倍的時間下面Foreach 才能算完，不誇張
            JHSchool.Data.JHCourse.SelectAll();

            List<string> ErrorMessages = new List<string>();
           
            foreach (JHSCAttendRecord scAttendRec in scAttendList)
            {

                if (scAttendRec.Course.SchoolYear == int.Parse(cboSchoolYear.Text) && scAttendRec.Course.Semester == int.Parse(cboSemester.Text) && scAttendRec.Course.CalculationFlag == "1")
                {
                    String duplicateErrorCourseKey = scAttendRec.Student.ID + "_" + scAttendRec.Course.Subject;

                    if (!scAttendCheck.ContainsKey(duplicateErrorCourseKey))
                    {
                        scAttendCheck.Add(duplicateErrorCourseKey, new List<JHSchool.Data.JHSCAttendRecord>());
                    }
                    else
                    {
                        ErrorMessages.Add(scAttendRec.Course.Name);
                    }                    
                }
            }
            try {
              if (ErrorMessages.Count>0)
            {
                String msg = "";

                foreach(String CourseName in ErrorMessages){

                if(msg ==""){
                msg += CourseName;                
                }
                else{
                msg+= "、";
                msg += CourseName;                
                }
                }                
                throw new System.Exception(msg);
            }                         
            }
            catch(Exception ee){
                if (ee.Source != null)
                {
                    MsgBox.Show(ee.Message + "科目於本學期重覆，請檢查");
                    //意外發現原來void 的函式 也可以用return 來終止，總之如果抓到Exception 後面的東西也不需要做了
                    return;
                }
            
            }    
            #endregion
            //2016/6/7，穎驊筆記，就是下面這一條，去找CalculationHelper的ReadAttendScore，恩正大大說，要在這之前先驗證
            //此學生有沒有修習重覆科目名稱的問題，要不然ReadAttendScore裡面的結構複雜到爆牽一髮動全身般地拆炸彈讓人戰戰兢兢無法下手

            //計算課程成績
            students.ReadAttendScore(int.Parse(cboSchoolYear.Text), int.Parse(cboSemester.Text), new string[] { }, null);
            students.SaveAttendScore(new string[] { }, null);
            //讀取計算規則
            students.ReadCalculationRule(null);
            //計算科目成績
            students.CalcuateSubjectSemesterScore(new string[] { });

            dgvSubject.SuspendLayout();
            dgvSubject.Rows.Clear();
            foreach (string subject in semesterScore.Subject)
            {
                SemesterSubjectScore sss = semesterScore.Subject[subject];

                string periodCredit = "";
                PeriodCredit pc = new PeriodCredit();
                if (pc.Parse(sss.Period + "/" + sss.Weight))
                    periodCredit = pc.ToString();

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvSubject,
                    sss.Domain,
                    subject,
                    periodCredit,
                    "" + sss.Value,
                    "" + sss.ScoreOrigin,
                    "" + sss.ScoreMakeup,
                    "" + sss.Effort,
                    sss.Text,
                    string.Empty);

                dgvSubject.Rows.Add(row);

                if (sss.Value < 60)
                    row.Cells[chsScore.Index].Style.ForeColor = Color.Red;
                if (sss.Value > 100 || sss.Value < 0)
                    row.Cells[chsScore.Index].Style.ForeColor = Color.Green;

                if (sss.ScoreOrigin < 60)
                    row.Cells[chsScoreOrigin.Index].Style.ForeColor = Color.Red;
                if (sss.ScoreOrigin > 100 || sss.ScoreOrigin < 0)
                    row.Cells[chsScoreOrigin.Index].Style.ForeColor = Color.Green;

                if (sss.ScoreMakeup < 60)
                    row.Cells[chsScoreMakeup.Index].Style.ForeColor = Color.Red;
                if (sss.ScoreMakeup > 100 || sss.ScoreMakeup < 0)
                    row.Cells[chsScoreMakeup.Index].Style.ForeColor = Color.Green;
            }
            dgvSubject.ResumeLayout();
        }

        //private void btnCalSubjScore_Click(object sender, EventArgs e)
        //{
        //    #region 檢查成績年級及科目是否有錯誤
        //    bool valid = true;

        //    //bool hasSubject = false;
        //    //科目
        //    dgvSubject.EndEdit();
        //    foreach (DataGridViewRow row in dgvSubject.Rows)
        //    {
        //        if (row.IsNewRow) continue;
        //        if (IsEmptyRow(row)) continue;

        //        //hasSubject = true;
        //        foreach (DataGridViewCell cell in row.Cells)
        //        {
        //            if (!string.IsNullOrEmpty(cell.ErrorText))
        //                valid = false;
        //        }
        //    }

        //    if (!valid) return;
        //    //if (!hasSubject) return;
        //    #endregion

        //    StudentScore.SetClassMapping();
        //    StudentScore studentScore = new StudentScore(_student);

        //    JHSemesterScoreRecord tempRecord = GetSemesterScoreRecordFromSubjectGrid();
        //    SemesterScore semesterScore = new SemesterScore(tempRecord);
        //    studentScore.SemestersScore.Add(SemesterData.Empty, semesterScore);

        //    List<StudentScore> students = new List<StudentScore>();
        //    students.Add(studentScore);

        //    //讀取計算規則
        //    students.ReadCalculationRule(null);
        //    //計算領域成績
        //    students.CalcuateDomainSemesterScore(new string[] { });

        //    dgvDomain.SuspendLayout();
        //    dgvDomain.Rows.Clear();
        //    foreach (string domain in semesterScore.Domain)
        //    {
        //        if (domain == "__彈性課程") continue;

        //        SemesterDomainScore domainScore = semesterScore.Domain[domain];

        //        string periodCredit = "";
        //        PeriodCredit pc = new PeriodCredit();
        //        if (pc.Parse(domainScore.Period + "/" + domainScore.Weight))
        //            periodCredit = pc.ToString();

        //        DataGridViewRow row = new DataGridViewRow();
        //        row.CreateCells(dgvDomain,
        //            domain,
        //            periodCredit,
        //            "" + domainScore.Value,
        //            "" + domainScore.ScoreOrigin,
        //            "" + domainScore.ScoreMakeup,
        //            "" + domainScore.Effort,
        //            domainScore.Text,
        //            string.Empty);
        //        dgvDomain.Rows.Add(row);

        //        if (domainScore.Value < 60)
        //            row.Cells[chdScore.Index].Style.ForeColor = Color.Red;
        //        if (domainScore.Value > 100 || domainScore.Value < 0)
        //            row.Cells[chdScore.Index].Style.ForeColor = Color.Green;
        //    }
        //    dgvDomain.ResumeLayout();

        //    //排序領域
        //    SortDomainGrid();
        //}
    }
}
