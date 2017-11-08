using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Framework;
using JHSchool.Evaluation.Editor;
using JHSchool.Evaluation.Feature;
using JHSchool.Data;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon
{
    public partial class CourseScoreInputForm : FISCA.Presentation.Controls.BaseForm
    {
        private CourseRecord _course;
        private AssessmentSetupRecord _assessmentSetupRecord;
        private List<AEIncludeRecord> _aeIncludeRecordList;
        private List<JHSCAttendRecord> _scAttendRecordList;

        //記錄 StudentID 與 DataGridViewRow 的對應
        private Dictionary<string, DataGridViewRow> _studentRowDict;

        //努力程度對照表
        private Dictionary<decimal, int> _effortList = new Dictionary<decimal, int>();
        private List<decimal> _scoreList = new List<decimal>();

        //文字評量代碼表
        private Dictionary<string, string> _textMapping = new Dictionary<string, string>();

        private List<DataGridViewCell> _dirtyCellList;

        /// <summary>
        /// Constructor
        /// 傳入一個課程。
        /// </summary>
        /// <param name="course"></param>
        private CourseScoreInputForm(CourseRecord course)
        {
            InitializeComponent();

            _course = course;

            #region 設定小標題
            TeacherRecord first = course.GetFirstTeacher();
            TeacherRecord second = course.GetSecondTeacher();
            TeacherRecord third = course.GetThirdTeacher();

            StringBuilder builder = new StringBuilder("");
            if (first != null) builder.Append(first.Name + ",");
            if (second != null) builder.Append(second.Name + ",");
            if (third != null) builder.Append(third.Name + ",");

            string teachers = builder.ToString();
            if (!string.IsNullOrEmpty(teachers))
                teachers = teachers.Substring(0, teachers.Length - 1);

            lblCourseName.Text = course.Name + (!string.IsNullOrEmpty(teachers) ? " (" + teachers + ")" : "");
            #endregion

            #region 取得努力程度對照表
            ConfigData cd = School.Configuration["努力程度對照表"];
            if (!string.IsNullOrEmpty(cd["xml"]))
            {
                XmlElement element = XmlHelper.LoadXml(cd["xml"]);

                foreach (XmlElement each in element.SelectNodes("Effort"))
                {
                    int code = int.Parse(each.GetAttribute("Code"));
                    decimal score;
                    if (!decimal.TryParse(each.GetAttribute("Score"), out score))
                        score = 0;

                    if (!_effortList.ContainsKey(score))
                        _effortList.Add(score, code);
                }

                _scoreList = new List<decimal>();
                _scoreList.AddRange(_effortList.Keys);
                _scoreList.Sort(delegate(decimal a, decimal b)
                {
                    return b.CompareTo(a);
                });
            }
            #endregion

            #region 取得文字評量代碼表
            cd = School.Configuration["文字描述代碼表"];
            if (!string.IsNullOrEmpty(cd["xml"]))
            {
                XmlHelper helper = new XmlHelper(XmlHelper.LoadXml(cd["xml"]));
                foreach (XmlElement item in helper.GetElements("Item"))
                {
                    string code = item.GetAttribute("Code");
                    string content = item.GetAttribute("Content");

                    if (!_textMapping.ContainsKey(code))
                        _textMapping.Add(code, content);
                }
            }
            #endregion
        }

        /// <summary>
        /// Form_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CourseScoreInputForm_Load(object sender, EventArgs e)
        {
            _studentRowDict = new Dictionary<string, DataGridViewRow>();
            _dirtyCellList = new List<DataGridViewCell>();

            #region 取得修課學生

            //_scAttendRecordList = _course.GetAttends();
            _scAttendRecordList = JHSCAttend.SelectByCourseIDs(new string[] { _course.ID });

            FillStudentsToDataGridView();

            #endregion

            #region 取得評量設定

            _assessmentSetupRecord = _course.GetAssessmentSetup();
            if (!AEInclude.Instance.Loaded) AEInclude.Instance.SyncAllBackground();
            _aeIncludeRecordList = _assessmentSetupRecord.GetAEIncludes();

            FillToComboBox();

            #endregion
        }

        /// <summary>
        /// 將學生填入DataGridView。
        /// </summary>
        private void FillStudentsToDataGridView()
        {
            dgv.SuspendLayout();
            dgv.Rows.Clear();

            _scAttendRecordList.Sort(SCAttendComparer);

            foreach (var record in _scAttendRecordList)
            {
                JHStudentRecord student = record.Student;

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv,
                    (student.Class != null) ? student.Class.Name : "",
                    student.SeatNo,
                    student.Name,
                    student.StudentNumber
                );
                dgv.Rows.Add(row);

                SCAttendTag tag = new SCAttendTag();
                tag.SCAttend = record;
                row.Tag = tag;

                //加入 StudentID 與 Row 的對應
                if (!_studentRowDict.ContainsKey(student.ID))
                    _studentRowDict.Add(student.ID, row);
            }

            dgv.ResumeLayout();
        }

        /// <summary>
        /// Comparision
        /// 依班級、座號、學號排序修課學生。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int SCAttendComparer(JHSCAttendRecord a, JHSCAttendRecord b)
        {
            if (a.Student.Class != null && b.Student.Class != null)
            {
                if (a.Student.Class.ID == b.Student.Class.ID)
                {
                    int ia, ib;
                    //if (int.TryParse(a.Student.SeatNo, out ia) && int.TryParse(b.Student.SeatNo, out ib))
                    if (a.Student.SeatNo.HasValue && b.Student.SeatNo.HasValue)
                    {
                        if (a.Student.SeatNo.Value == b.Student.SeatNo.Value)
                            return a.Student.StudentNumber.CompareTo(b.Student.StudentNumber);
                        //return ia.CompareTo(ib);
                        return a.Student.SeatNo.Value.CompareTo(b.Student.SeatNo.Value);
                    }
                    //else if (int.TryParse(a.Student.SeatNo, out ia))
                    else if (a.Student.SeatNo.HasValue)
                        return -1;
                    //else if (int.TryParse(b.Student.SeatNo, out ib))
                    else if (b.Student.SeatNo.HasValue)
                        return 1;
                    else
                        return a.Student.StudentNumber.CompareTo(b.Student.StudentNumber);
                }
                else
                    return a.Student.Class.Name.CompareTo(b.Student.Class.Name);
            }
            else if (a.Student.Class != null && b.Student.Class == null)
                return -1;
            else if (a.Student.Class == null && b.Student.Class != null)
                return 1;
            else
                return a.Student.StudentNumber.CompareTo(b.Student.StudentNumber);
        }

        /// <summary>
        /// 將試別填入ComboBox。
        /// </summary>
        private void FillToComboBox()
        {
            cboExamList.Items.Clear();

            foreach (var record in _aeIncludeRecordList)
            {
                cboExamList.Items.Add(new ExamComboBoxItem(record));
            }
            if (cboExamList.Items.Count > 0)
                cboExamList.SelectedIndex = 0;
        }

        /// <summary>
        /// 選擇試別時觸發。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboExamList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboExamList.SelectedItem == null) return;
            ExamComboBoxItem item = cboExamList.SelectedItem as ExamComboBoxItem;
            AEIncludeRecord aeIncludeRecord = item.AEIncludeRecord;

            dgv.SuspendLayout();

            #region 依 AEIncludeRecord 決定哪些 Column 要顯示

            chInputScore.Visible = aeIncludeRecord.UseScore;
            chInputEffort.Visible = aeIncludeRecord.UseEffort;
            chInputText.Visible = aeIncludeRecord.UseText;

            #endregion

            GetScoresAndFill(aeIncludeRecord);

            dgv.ResumeLayout();
        }

        /// <summary>
        /// 取得成績並填入DataGridView
        /// </summary>
        private void GetScoresAndFill(AEIncludeRecord aeIncludeRecord)
        {
            _dirtyCellList.Clear();
            lblSave.Visible = false;

            #region 清空所有評量欄位及SCETakeRecord

            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.Cells[chInputScore.Index].Value = row.Cells[chInputEffort.Index].Value = row.Cells[chInputText.Index].Value = null;
                (row.Tag as SCAttendTag).SCETake = null;
            }

            #endregion

            #region 取得成績並填入

            foreach (var record in QuerySCETake.GetSCETakeRecords(_course.ID, aeIncludeRecord.RefExamID))
            {
                if (_studentRowDict.ContainsKey(record.RefStudentID))
                {
                    DataGridViewRow row = _studentRowDict[record.RefStudentID];
                    row.Cells[chInputScore.Index].Value = record.Score;
                    row.Cells[chInputEffort.Index].Value = record.Effort;
                    row.Cells[chInputText.Index].Value = record.Text;

                    row.Cells[chInputScore.Index].Tag = record.Score;
                    row.Cells[chInputEffort.Index].Tag = record.Effort;
                    row.Cells[chInputText.Index].Tag = record.Text;

                    if (record.Score < 60) row.Cells[chInputScore.Index].Style.ForeColor = Color.Red;
                    if (record.Score > 100 || record.Score < 0) row.Cells[chInputScore.Index].Style.ForeColor = Color.Green;
                    else row.Cells[chInputScore.Index].Style.ForeColor = Color.Black;

                    SCAttendTag tag = row.Tag as SCAttendTag;
                    tag.SCETake = record;
                }
                else
                {
                    #region 除錯用，別刪掉

                    //StudentRecord student = Student.Instance.Items[record.RefStudentID];
                    //if (student == null)
                    //    MsgBox.Show("系統編號「" + record.RefStudentID + "」的學生不存在…");
                    //else
                    //{
                    //    string className = (student.Class != null) ? student.Class.Name + "  " : "";
                    //    string seatNo = string.IsNullOrEmpty(className) ? "" : (string.IsNullOrEmpty(student.SeatNo) ? "" : student.SeatNo + "  ");
                    //    string studentNumber = string.IsNullOrEmpty(student.StudentNumber) ? "" : " (" + student.StudentNumber + ")";

                    //    MsgBox.Show(className + seatNo + student.Name + studentNumber, "這個學生有問題喔…");
                    //}

                    #endregion
                }
            }

            #endregion
        }

        /// <summary>
        /// 是否值有變更
        /// </summary>
        /// <returns></returns>
        private bool IsDirty()
        {
            return (_dirtyCellList.Count > 0);
        }

        /// <summary>
        /// 按下「儲存」時觸發。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            if (!IsValid())
            {
                MsgBox.Show("請先修正錯誤再儲存。");
                return;
            }

            try
            {
                List<SCETakeRecordEditor> editors = MakeSCETakeRecordEditors();
                if (editors.Count > 0) editors.SaveAllEditors();
                MsgBox.Show("儲存成功。");

                ExamComboBoxItem item = cboExamList.SelectedItem as ExamComboBoxItem;
                GetScoresAndFill(item.AEIncludeRecord);
            }
            catch (Exception ex)
            {
                MsgBox.Show("儲存失敗。\n" + ex.Message);
                //throw ex;
            }
        }

        /// <summary>
        /// 關閉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 從DataGridView上取得SCETakeRecordEditors
        /// </summary>
        /// <returns></returns>
        private List<SCETakeRecordEditor> MakeSCETakeRecordEditors()
        {
            if (cboExamList.SelectedItem == null) return new List<SCETakeRecordEditor>();

            List<SCETakeRecordEditor> list = new List<SCETakeRecordEditor>();
            ExamComboBoxItem item = cboExamList.SelectedItem as ExamComboBoxItem;
            ExamRecord exam = item.AEIncludeRecord.Exam;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                SCAttendTag tag = row.Tag as SCAttendTag;
                if (tag.SCETake != null)
                {
                    #region 修改or刪除
                    bool is_remove = true;

                    SCETakeRecordEditor editor = tag.SCETake.GetEditor();

                    if (chInputScore.Visible == true)
                    {
                        string value = "" + row.Cells[chInputScore.Index].Value;
                        is_remove &= string.IsNullOrEmpty(value);
                        if (!string.IsNullOrEmpty(value))
                            editor.Score = decimal.Parse(value);
                    }
                    if (chInputEffort.Visible == true)
                    {
                        string value = "" + row.Cells[chInputEffort.Index].Value;
                        is_remove &= string.IsNullOrEmpty(value);
                        if (!string.IsNullOrEmpty(value))
                            editor.Effort = int.Parse(value);
                    }
                    if (chInputText.Visible == true)
                    {
                        string value = "" + row.Cells[chInputText.Index].Value;
                        is_remove &= string.IsNullOrEmpty(value);
                        editor.Text = value;
                    }

                    editor.Remove = is_remove;
                    list.Add(editor);
                    #endregion
                }
                else
                {
                    #region 新增
                    bool is_add = false;

                    SCETakeRecordEditor editor = new SCETakeRecordEditor(new PrivateSCAttendRecord(tag.SCAttend), exam);

                    if (chInputScore.Visible == true)
                    {
                        string value = "" + row.Cells[chInputScore.Index].Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            editor.Score = decimal.Parse(value);
                            is_add = true;
                        }
                    }
                    if (chInputEffort.Visible == true)
                    {
                        string value = "" + row.Cells[chInputEffort.Index].Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            editor.Effort = int.Parse(value);
                            is_add = true;
                        }
                    }
                    if (chInputText.Visible == true)
                    {
                        string value = "" + row.Cells[chInputText.Index].Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            editor.Text = value;
                            is_add = true;
                        }
                    }

                    if (is_add) list.Add(editor);
                    #endregion
                }
            }

            return list;
        }

        /// <summary>
        /// 驗證每個欄位是否正確。
        /// 有錯誤訊息表示不正確。
        /// </summary>
        /// <returns></returns>
        private bool IsValid()
        {
            bool valid = true;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (chInputScore.Visible == true && !string.IsNullOrEmpty(row.Cells[chInputScore.Index].ErrorText))
                    valid = false;
                if (chInputEffort.Visible == true && !string.IsNullOrEmpty(row.Cells[chInputEffort.Index].ErrorText))
                    valid = false;
                if (chInputText.Visible == true && !string.IsNullOrEmpty(row.Cells[chInputText.Index].ErrorText))
                    valid = false;
            }

            return valid;
        }

        /// <summary>
        /// 點欄位立即進入編輯。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != chInputScore.Index && e.ColumnIndex != chInputEffort.Index && e.ColumnIndex != chInputText.Index) return;
            if (e.RowIndex < 0) return;

            dgv.BeginEdit(true);
        }

        /// <summary>
        /// 當欄位結束編輯，進行驗證。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != chInputScore.Index && e.ColumnIndex != chInputEffort.Index && e.ColumnIndex != chInputText.Index) return;
            if (e.RowIndex < 0) return;

            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.OwningColumn == chInputScore)
            {
                #region 驗證分數評量 & 低於60分變紅色
                cell.Style.ForeColor = Color.Black;

                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    decimal d;
                    if (!decimal.TryParse("" + cell.Value, out d))
                        cell.ErrorText = "分數必須是數字";
                    else
                    {
                        cell.ErrorText = "";
                        if (d < 60)
                            cell.Style.ForeColor = Color.Red;
                        if (d > 100 || d < 0)
                            cell.Style.ForeColor = Color.Green;

                        #region 自動加入對應的努力程度

                        if (_effortList.Count > 0)
                        {
                            dgv.Rows[cell.RowIndex].Cells[chInputEffort.Index].Value = GenerateEffortCode(d);
                        }

                        #endregion
                    }
                }
                else
                    cell.ErrorText = "";
                #endregion
            }
            else if (cell.OwningColumn == chInputEffort)
            {
                #region 驗證努力程度
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    int i;
                    if (!int.TryParse("" + cell.Value, out i))
                        cell.ErrorText = "努力程度必須是整數數字";
                }
                else
                    cell.ErrorText = "";
                #endregion
            }
            else if (cell.OwningColumn == chInputText)
            {
                #region 轉換文字描述 (文字評量代碼表)
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    string orig = "" + cell.Value;
                    foreach (string code in _textMapping.Keys)
                    {
                        if (orig.Contains(code))
                        {
                            int start = orig.IndexOf(code);
                            orig = orig.Substring(0, start) + _textMapping[code] + orig.Substring(start + code.Length);
                        }
                    }
                    cell.Value = orig;
                }
                #endregion
            }

            #region 檢查是否有變更過
            if ("" + cell.Tag != "" + cell.Value)
            {
                if (!_dirtyCellList.Contains(cell)) _dirtyCellList.Add(cell);
            }
            else
            {
                if (_dirtyCellList.Contains(cell)) _dirtyCellList.Remove(cell);
            }
            lblSave.Visible = IsDirty();
            #endregion
        }

        /// <summary>
        /// 產生努力程度的對應代碼
        /// </summary>
        /// <param name="d">分數</param>
        /// <returns>努力程度代碼</returns>
        private int GenerateEffortCode(decimal d)
        {
            foreach (decimal score in _scoreList)
                if (d >= score) return _effortList[score];

            return _effortList[_scoreList[_scoreList.Count - 1]];
        }

        /// <summary>
        /// Data Class
        /// 包含 AEIncludeRecord 的 ComboBoxItem
        /// </summary>
        private class ExamComboBoxItem
        {
            public string DisplayText
            {
                get
                {
                    if (_examRecord != null)
                        return _examRecord.Name;
                    return string.Empty;
                }
            }

            public AEIncludeRecord AEIncludeRecord
            {
                get { return _aeIncludeRecord; }
            }

            private AEIncludeRecord _aeIncludeRecord;
            private ExamRecord _examRecord;
            //private string _weight;

            public ExamComboBoxItem(AEIncludeRecord record)
            {
                _aeIncludeRecord = record;
                //_weight = record.Weight;
                _examRecord = record.Exam;
            }
        }

        /// <summary>
        /// 排序欄位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column == chSeatNo || e.Column == chStudentNumber)
            {
                int result = 0;

                int a, b;
                if (int.TryParse("" + e.CellValue1, out a) && int.TryParse("" + e.CellValue2, out b))
                    result = a.CompareTo(b);
                else if (int.TryParse("" + e.CellValue1, out a))
                    result = -1;
                else if (int.TryParse("" + e.CellValue2, out b))
                    result = 1;
                else
                    result = 0;

                e.SortResult = result;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Data Class
        /// 包含 SCAttendRecord 與 SCETakeRecord
        /// </summary>
        private class SCAttendTag
        {
            public JHSCAttendRecord SCAttend { get; set; }
            public SCETakeRecord SCETake { get; set; }
        }

        class PrivateSCAttendRecord : SCAttendRecord
        {
            public PrivateSCAttendRecord(JHSCAttendRecord record)
                : base(record.RefStudentID, record.RefCourseID, record.ID, "", "", "")
            {
            }
        }
    }
}