using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FISCA.Presentation.Controls;
using JHSchool;
using JHSchool.Evaluation;
using HsinChu.JHEvaluation.Data;
using JHSchool.Data;
using K12.Data.Configuration;

namespace HsinChu.JHEvaluation.CourseExtendControls.Ribbon
{
    public partial class CourseScoreInputForm : BaseForm
    {
        private CourseRecord _course;
        private AssessmentSetupRecord _assessmentSetupRecord;
        private List<HC.JHAEIncludeRecord> _aeIncludeRecordList;
        private List<JHSCAttendRecord> _scAttendRecordList;
        private string _ExamName = "";
        // Log
        PermRecLogProcess prlp;        

        //記錄 StudentID 與 DataGridViewRow 的對應
        private Dictionary<string, DataGridViewRow> _studentRowDict;

        //文字評量代碼表
        private Dictionary<string, string> _textMapping = new Dictionary<string, string>();

        private List<DataGridViewCell> _dirtyCellList;

        /// <summary>
        /// Constructor
        /// 傳入一個課程。
        /// </summary>
        /// <param name="course"></param>
        public CourseScoreInputForm(CourseRecord course)
        {
            InitializeComponent();
            prlp = new PermRecLogProcess();
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

            #region 取得文字評量代碼表
            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["文字描述代碼表"];
            if (!string.IsNullOrEmpty(cd["xml"]))
            {
                K12.Data.XmlHelper helper = new K12.Data.XmlHelper(K12.Data.XmlHelper.LoadXml(cd["xml"]));
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
            _aeIncludeRecordList = JHAEInclude.SelectByAssessmentSetupID(_assessmentSetupRecord.ID).AsHCJHAEIncludeRecords();

            FillToComboBox();

            // 當沒有試別關閉
            if (cboExamList.Items.Count < 1)
                this.Close();

            // 載入分數顏色
            LoadDvScoreColor();
            _ExamName = cboExamList.Text;
            #endregion
        }

        /// <summary>
        /// 處理分數顏色變化
        /// </summary>
        private void LoadDvScoreColor()
        {
            // 處理初始分數變色
            foreach (DataGridViewRow dgv1 in dgv.Rows)
                foreach (DataGridViewCell cell in dgv1.Cells)
                {
                    cell.ErrorText = "";

                    if (cell.OwningColumn == chInputScore || cell.OwningColumn == chInputAssignmentScore)
                    {
                        cell.Style.ForeColor = Color.Black;
                        if (!string.IsNullOrEmpty("" + cell.Value))
                        {
                            decimal d;
                            if (!decimal.TryParse("" + cell.Value, out d))
                                cell.ErrorText = "分數必須為數字";
                            else
                            {
                                if (d < 60)
                                    cell.Style.ForeColor = Color.Red;
                                if (d > 100 || d < 0)
                                    cell.Style.ForeColor = Color.Green;
                            }
                        }
                    }
                }
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
                if (student.StatusStr != "一般") continue;

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
            {
                Campus.Configuration.ConfigData cd = Campus.Configuration.Config.User["新竹課程成績輸入考試別"];
                string str = cd["新竹課程成績輸入考試別"];
                int idx = cboExamList.FindString(str);
                if (idx < 0)
                    cboExamList.SelectedIndex = 0;
                else
                    cboExamList.SelectedIndex = idx;
            }
        }

        /// <summary>
        /// 載入資料 Log
        /// </summary>
        public void SetLoadDataToLog()
        {
            try
            {   // 將暫存清空
                prlp.ClearCache();
                _ExamName = cboExamList.Text;
                
                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    string strClassName = string.Empty, strSeatNo = string.Empty, strName = string.Empty, strStudentNumber = string.Empty;
                    if (dgvr.IsNewRow)
                        continue;

                    if (dgvr.Cells[chClassName.Index].Value != null)
                        strClassName = dgvr.Cells[chClassName.Index].Value.ToString();

                    if (dgvr.Cells[chSeatNo.Index].Value != null)
                        strSeatNo = dgvr.Cells[chSeatNo.Index].Value.ToString();

                    if (dgvr.Cells[chName.Index].Value != null)
                        strName = dgvr.Cells[chName.Index].Value.ToString();

                    if (dgvr.Cells[chStudentNumber.Index].Value != null)
                        strStudentNumber = dgvr.Cells[chStudentNumber.Index].Value.ToString();

                    string CoName = _course.SchoolYear + "學年度第" + _course.Semester + "學期" + _course.Name;
                    // key
                    string Key1 = CoName + ",試別:" + _ExamName + ",班級:" + strClassName + ",座號:" + strSeatNo + ",姓名:" + strName + ",學號:" + strStudentNumber + ",定期分數:";
                    string Key2 = CoName + ",試別:" + _ExamName + ",班級:" + strClassName + ",座號:" + strSeatNo + ",姓名:" + strName + ",學號:" + strStudentNumber + ",平時分數:";
                    string Key3 = CoName + ",試別:" + _ExamName + ",班級:" + strClassName + ",座號:" + strSeatNo + ",姓名:" + strName + ",學號:" + strStudentNumber + ",文字描述:";
                    string Value1 = string.Empty, Value2 = string.Empty,Value3=string.Empty;

                    if (dgvr.Cells[chInputScore.Index].Value != null)
                        Value1 = dgvr.Cells[chInputScore.Index].Value.ToString();

                    if (dgvr.Cells[chInputAssignmentScore.Index ].Value != null)
                        Value2 = dgvr.Cells[chInputAssignmentScore.Index].Value.ToString();

                    if (dgvr.Cells[chInputText.Index].Value != null)
                        Value3 = dgvr.Cells[chInputText.Index].Value.ToString();

                    // 定期分數
                    prlp.SetBeforeSaveText(Key1, Value1);
                    // 平時分數
                    prlp.SetBeforeSaveText(Key2, Value2);
                    // 文字描述
                    prlp.SetBeforeSaveText(Key3, Value3);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log 發生錯誤");
            }

        }

        /// <summary>
        /// 儲存資料 Log
        /// </summary>
        public void SetSaveDataToLog()
        {
            try
            {                
                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    string strClassName = string.Empty, strSeatNo = string.Empty, strName = string.Empty, strStudentNumber = string.Empty;
                    if (dgvr.IsNewRow)
                        continue;

                    if (dgvr.Cells[chClassName.Index].Value != null)
                        strClassName = dgvr.Cells[chClassName.Index].Value.ToString();

                    if (dgvr.Cells[chSeatNo.Index].Value != null)
                        strSeatNo = dgvr.Cells[chSeatNo.Index].Value.ToString();

                    if (dgvr.Cells[chName.Index].Value != null)
                        strName = dgvr.Cells[chName.Index].Value.ToString();

                    if (dgvr.Cells[chStudentNumber.Index].Value != null)
                        strStudentNumber = dgvr.Cells[chStudentNumber.Index].Value.ToString();

                    string CoName = _course.SchoolYear + "學年度第" + _course.Semester + "學期" + _course.Name;
                    // key
                    string Key1 = CoName + ",試別:" + _ExamName + ",班級:" + strClassName + ",座號:" + strSeatNo + ",姓名:" + strName + ",學號:" + strStudentNumber + ",定期分數:";
                    string Key2 = CoName + ",試別:" + _ExamName + ",班級:" + strClassName + ",座號:" + strSeatNo + ",姓名:" + strName + ",學號:" + strStudentNumber + ",平時分數:";
                    string Key3 = CoName + ",試別:" + _ExamName + ",班級:" + strClassName + ",座號:" + strSeatNo + ",姓名:" + strName + ",學號:" + strStudentNumber + ",文字描述:";
                    string Value1 = string.Empty, Value2 = string.Empty, Value3 = string.Empty;

                    if (dgvr.Cells[chInputScore.Index].Value != null)
                        Value1 = dgvr.Cells[chInputScore.Index].Value.ToString();

                    if (dgvr.Cells[chInputAssignmentScore.Index].Value != null)
                        Value2 = dgvr.Cells[chInputAssignmentScore.Index].Value.ToString();

                    if (dgvr.Cells[chInputText.Index].Value != null)
                        Value3 = dgvr.Cells[chInputText.Index].Value.ToString();

                    // 定期分數
                    prlp.SetAfterSaveText(Key1, Value1);
                    // 平時分數
                    prlp.SetAfterSaveText(Key2, Value2);
                    // 文字描述
                    prlp.SetAfterSaveText(Key3, Value3);                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log 發生錯誤");
            }

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
            HC.JHAEIncludeRecord aeIncludeRecord = item.AEIncludeRecord;

            dgv.SuspendLayout();

            #region 依 AEIncludeRecord 決定哪些 Column 要顯示

            chInputScore.Visible = aeIncludeRecord.UseScore;
            chInputAssignmentScore.Visible = aeIncludeRecord.UseAssignmentScore;
            chInputText.Visible = aeIncludeRecord.UseText;

            #endregion

            GetScoresAndFill(aeIncludeRecord);

            LoadDvScoreColor();
            dgv.ResumeLayout();
            SetLoadDataToLog();
        }

        /// <summary>
        /// 取得成績並填入DataGridView
        /// </summary>
        private void GetScoresAndFill(HC.JHAEIncludeRecord aeIncludeRecord)
        {
            _dirtyCellList.Clear();
            lblSave.Visible = false;

            #region 清空所有評量欄位及SCETakeRecord

            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.Cells[chInputScore.Index].Value = row.Cells[chInputAssignmentScore.Index].Value = row.Cells[chInputText.Index].Value = null;
                (row.Tag as SCAttendTag).SCETake = null;
            }

            #endregion

            #region 取得成績並填入

            //foreach (var record in QuerySCETake.GetSCETakeRecords(_course.ID, aeIncludeRecord.RefExamID))
            //{
            //}

            List<HC.JHSCETakeRecord> list = JHSchool.Data.JHSCETake.SelectByCourseAndExam(_course.ID, aeIncludeRecord.RefExamID).AsHCJHSCETakeRecords();
            foreach (var record in list)
            {
                if (_studentRowDict.ContainsKey(record.RefStudentID))
                {
                    DataGridViewRow row = _studentRowDict[record.RefStudentID];
                    row.Cells[chInputScore.Index].Value = record.Score;
                    row.Cells[chInputAssignmentScore.Index].Value = record.AssignmentScore;
                    row.Cells[chInputText.Index].Value = record.Text;

                    row.Cells[chInputScore.Index].Tag = record.Score;
                    row.Cells[chInputAssignmentScore.Index].Tag = record.AssignmentScore;
                    row.Cells[chInputText.Index].Tag = record.Text;

                    if (record.Score < 60) row.Cells[chInputScore.Index].Style.ForeColor = Color.Red;
                    if (record.Score > 100 || record.Score < 0) row.Cells[chInputScore.Index].Style.ForeColor = Color.Green;
                    else row.Cells[chInputScore.Index].Style.ForeColor = Color.Black;

                    if (record.AssignmentScore < 60) row.Cells[chInputAssignmentScore.Index].Style.ForeColor = Color.Red;
                    if (record.AssignmentScore > 100 || record.AssignmentScore < 0) row.Cells[chInputAssignmentScore.Index].Style.ForeColor = Color.Green;
                    else row.Cells[chInputAssignmentScore.Index].Style.ForeColor = Color.Black;

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
            if (string.IsNullOrEmpty(cboExamList.Text))
            {
                FISCA.Presentation.Controls.MsgBox.Show("沒有試別無法儲存.");
                return;
            }

            dgv.EndEdit();

            if (!IsValid())
            {
                MsgBox.Show("請先修正錯誤再儲存。");
                return;
            }

            try
            {
                //List<SCETakeRecordEditor> editors = MakeSCETakeRecordEditors();
                //if (editors.Count > 0) editors.SaveAll();
                bool checkLoadSave = false;
                RecordIUDLists lists = GetRecords();

                // 檢查超過 0~100
                if (lists.DeleteList.Count > 0)
                    foreach (HsinChu.JHEvaluation.Data.HC.JHSCETakeRecord sc in lists.DeleteList)
                    {
                        if (sc.AssignmentScore.HasValue)
                            if (sc.AssignmentScore < 0 || sc.AssignmentScore > 100)
                                checkLoadSave = true;
                        if (sc.Score.HasValue)
                            if (sc.Score < 0 || sc.Score > 100)
                                checkLoadSave = true;

                    }

                if (lists.InsertList.Count > 0)
                    foreach (HsinChu.JHEvaluation.Data.HC.JHSCETakeRecord sc in lists.InsertList)
                    {
                        if (sc.AssignmentScore.HasValue)
                            if (sc.AssignmentScore < 0 || sc.AssignmentScore > 100)
                                checkLoadSave = true;
                        if (sc.Score.HasValue)
                            if (sc.Score < 0 || sc.Score > 100)
                                checkLoadSave = true;

                    }

                if (lists.UpdateList.Count > 0)
                    foreach (HsinChu.JHEvaluation.Data.HC.JHSCETakeRecord sc in lists.UpdateList)
                    {
                        if (sc.AssignmentScore.HasValue)
                            if (sc.AssignmentScore < 0 || sc.AssignmentScore > 100)
                                checkLoadSave = true;

                        if (sc.Score.HasValue)
                            if (sc.Score < 0 || sc.Score > 100)
                                checkLoadSave = true;
                    }

                if (checkLoadSave)
                {
                    CheckSaveForm csf = new CheckSaveForm();
                    csf.ShowDialog();
                    if (csf.DialogResult == DialogResult.Cancel)
                        return;
                }

                if (lists.InsertList.Count > 0)
                    JHSchool.Data.JHSCETake.Insert(lists.InsertList.AsJHSCETakeRecords());
                if (lists.UpdateList.Count > 0)
                    JHSchool.Data.JHSCETake.Update(lists.UpdateList.AsJHSCETakeRecords());
                if (lists.DeleteList.Count > 0)
                    JHSchool.Data.JHSCETake.Delete(lists.DeleteList.AsJHSCETakeRecords());

                //記憶所選的試別
                Campus.Configuration.ConfigData cd = Campus.Configuration.Config.User["新竹課程成績輸入考試別"];
                cd["新竹課程成績輸入考試別"] = cboExamList.Text;
                cd.Save();

                MsgBox.Show("儲存成功。");

                // 記修改後 log
                SetSaveDataToLog();
                // 存 Log
                prlp.SetActionBy("課程", "課程成績輸入");
                prlp.SetAction("課程成績輸入");
                prlp.SetDescTitle("");
                prlp.SaveLog("", "", "Course", _course.ID);

                // 重新記修改前 Log
                SetLoadDataToLog();

                ExamComboBoxItem item = cboExamList.SelectedItem as ExamComboBoxItem;
                GetScoresAndFill(item.AEIncludeRecord);
            }
            catch (Exception ex)
            {
                MsgBox.Show("儲存失敗。\n" + ex.Message);
                //throw ex;
            }

            // // 載入分數顏色
            LoadDvScoreColor();
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
        //private List<SCETakeRecordEditor> MakeSCETakeRecordEditors()
        //{
        //    if (cboExamList.SelectedItem == null) return new List<SCETakeRecordEditor>();

        //    List<SCETakeRecordEditor> list = new List<SCETakeRecordEditor>();
        //    ExamComboBoxItem item = cboExamList.SelectedItem as ExamComboBoxItem;
        //    ExamRecord exam = item.AEIncludeRecord.Exam;

        //    foreach (DataGridViewRow row in dgv.Rows)
        //    {
        //        SCAttendTag tag = row.Tag as SCAttendTag;
        //        if (tag.SCETake != null)
        //        {
        //            #region 修改or刪除
        //            bool is_remove = true;

        //            SCETakeRecordEditor editor = tag.SCETake.GetEditor();

        //            if (chInputScore.Visible == true)
        //            {
        //                string value = "" + row.Cells[chInputScore.Index].Value;
        //                is_remove &= string.IsNullOrEmpty(value);
        //                if (!string.IsNullOrEmpty(value))
        //                    editor.Score = decimal.Parse(value);
        //            }
        //            if (chInputEffort.Visible == true)
        //            {
        //                string value = "" + row.Cells[chInputEffort.Index].Value;
        //                is_remove &= string.IsNullOrEmpty(value);
        //                if (!string.IsNullOrEmpty(value))
        //                    editor.Effort = int.Parse(value);
        //            }
        //            if (chInputText.Visible == true)
        //            {
        //                string value = "" + row.Cells[chInputText.Index].Value;
        //                is_remove &= string.IsNullOrEmpty(value);
        //                editor.Text = value;
        //            }

        //            editor.Remove = is_remove;
        //            list.Add(editor);
        //            #endregion
        //        }
        //        else
        //        {
        //            #region 新增
        //            bool is_add = false;

        //            SCETakeRecordEditor editor = new SCETakeRecordEditor(tag.SCAttend, exam);

        //            if (chInputScore.Visible == true)
        //            {
        //                string value = "" + row.Cells[chInputScore.Index].Value;
        //                if (!string.IsNullOrEmpty(value))
        //                {
        //                    editor.Score = decimal.Parse(value);
        //                    is_add = true;
        //                }
        //            }
        //            if (chInputEffort.Visible == true)
        //            {
        //                string value = "" + row.Cells[chInputEffort.Index].Value;
        //                if (!string.IsNullOrEmpty(value))
        //                {
        //                    editor.Effort = int.Parse(value);
        //                    is_add = true;
        //                }
        //            }
        //            if (chInputText.Visible == true)
        //            {
        //                string value = "" + row.Cells[chInputText.Index].Value;
        //                if (!string.IsNullOrEmpty(value))
        //                {
        //                    editor.Text = value;
        //                    is_add = true;
        //                }
        //            }

        //            if (is_add) list.Add(editor);
        //            #endregion
        //        }
        //    }

        //    return list;
        //}

        /// <summary>
        /// 從DataGridView上取得 HC.JHSCETakeRecord
        /// </summary>
        /// <returns></returns>
        private RecordIUDLists GetRecords()
        {
            if (cboExamList.SelectedItem == null) return new RecordIUDLists();

            RecordIUDLists lists = new RecordIUDLists();

            ExamComboBoxItem item = cboExamList.SelectedItem as ExamComboBoxItem;
            JHExamRecord exam = item.AEIncludeRecord.Exam;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                SCAttendTag tag = row.Tag as SCAttendTag;
                if (tag.SCETake != null)
                {
                    HC.JHSCETakeRecord record = tag.SCETake;

                    #region 修改or刪除
                    bool is_remove = true;

                    if (chInputScore.Visible == true)
                    {
                        string value = "" + row.Cells[chInputScore.Index].Value;
                        is_remove &= string.IsNullOrEmpty(value);
                        if (!string.IsNullOrEmpty(value))
                            record.Score = decimal.Parse(value);
                        // 小郭, 2013/12/30
                        else
                            record.Score = null;
                    }
                    if (chInputAssignmentScore.Visible == true)
                    {
                        string value = "" + row.Cells[chInputAssignmentScore.Index].Value;
                        is_remove &= string.IsNullOrEmpty(value);
                        if (!string.IsNullOrEmpty(value))
                            record.AssignmentScore = decimal.Parse(value);
                        // 小郭, 2013/12/30
                        else
                            record.AssignmentScore = null;
                    }
                    if (chInputText.Visible == true)
                    {
                        string value = "" + row.Cells[chInputText.Index].Value;
                        is_remove &= string.IsNullOrEmpty(value);
                        record.Text = value;
                    }

                    if (is_remove)
                        lists.DeleteList.Add(record);
                    else
                        lists.UpdateList.Add(record);
                    #endregion
                }
                else
                {
                    #region 新增
                    bool is_add = false;

                    JHSchool.Data.JHSCETakeRecord jh = new JHSchool.Data.JHSCETakeRecord();
                    HC.JHSCETakeRecord record = new HC.JHSCETakeRecord(jh);

                    record.RefCourseID = tag.SCAttend.Course.ID;
                    record.RefExamID = exam.ID;
                    record.RefSCAttendID = tag.SCAttend.ID;
                    record.RefStudentID = tag.SCAttend.Student.ID;

                    record.Score = null;
                    record.AssignmentScore = null;
                    record.Text = string.Empty;

                    if (chInputScore.Visible == true)
                    {
                        string value = "" + row.Cells[chInputScore.Index].Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            record.Score = decimal.Parse(value);
                            is_add = true;
                        }
                    }
                    if (chInputAssignmentScore.Visible == true)
                    {
                        string value = "" + row.Cells[chInputAssignmentScore.Index].Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            record.AssignmentScore = decimal.Parse(value);
                            is_add = true;
                        }
                    }
                    if (chInputText.Visible == true)
                    {
                        string value = "" + row.Cells[chInputText.Index].Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            record.Text = value;
                            is_add = true;
                        }
                    }

                    if (is_add) lists.InsertList.Add(record);
                    #endregion
                }
            }

            return lists;
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
                if (chInputAssignmentScore.Visible == true && !string.IsNullOrEmpty(row.Cells[chInputAssignmentScore.Index].ErrorText))
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
            if (e.ColumnIndex != chInputScore.Index && e.ColumnIndex != chInputAssignmentScore.Index && e.ColumnIndex != chInputText.Index) return;
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
            if (e.ColumnIndex != chInputScore.Index && e.ColumnIndex != chInputAssignmentScore.Index && e.ColumnIndex != chInputText.Index) return;
            if (e.RowIndex < 0) return;

            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.OwningColumn == chInputScore || cell.OwningColumn == chInputAssignmentScore)
            {
                #region 驗證分數 & 低於60分變紅色
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
                    }
                }
                else
                    cell.ErrorText = "";
                #endregion
            }
            //else if (cell.OwningColumn == chInputEffort)
            //{
            //    #region 驗證努力程度
            //    if (!string.IsNullOrEmpty("" + cell.Value))
            //    {
            //        int i;
            //        if (!int.TryParse("" + cell.Value, out i))
            //            cell.ErrorText = "努力程度必須是整數數字";
            //    }
            //    else
            //        cell.ErrorText = "";
            //    #endregion
            //}
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

            public HC.JHAEIncludeRecord AEIncludeRecord
            {
                get { return _aeIncludeRecord; }
            }

            private HC.JHAEIncludeRecord _aeIncludeRecord;
            private JHExamRecord _examRecord;
            //private string _weight;

            public ExamComboBoxItem(HC.JHAEIncludeRecord record)
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
            public HC.JHSCETakeRecord SCETake { get; set; }
        }

        private class RecordIUDLists
        {
            public List<HC.JHSCETakeRecord> InsertList { get; set; }
            public List<HC.JHSCETakeRecord> UpdateList { get; set; }
            public List<HC.JHSCETakeRecord> DeleteList { get; set; }

            public RecordIUDLists()
            {
                InsertList = new List<HC.JHSCETakeRecord>();
                UpdateList = new List<HC.JHSCETakeRecord>();
                DeleteList = new List<HC.JHSCETakeRecord>();
            }
        }
    }
}
