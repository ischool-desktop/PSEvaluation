using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Framework;
using JHSchool.Data;
using KaoHsiung.JHEvaluation.Data;
using System.Linq;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.CourseScoreItemControls
{
    public partial class ScoreInputForm : FISCA.Presentation.Controls.BaseForm
    {
        private ChangeListener _listener;
        private JHStudentRecord _student;
        private JHCourseRecord _course;
        private JHSCAttendRecord _scattend;
        private EffortMapper _effortMapper;
        // Log
        private PermRecLogProcess prlp;

        public ScoreInputForm(JHStudentRecord student, JHCourseRecord course)
        {
            InitializeComponent();
            InitializeListener();
            _effortMapper = new EffortMapper();
            prlp = new PermRecLogProcess();

            lblCourseName.Text = course.Name;
            lblStudent.Text = student.Name + " " + student.StudentNumber;

            _course = course;
            _student = student;

            List<JHSCAttendRecord> scattendList = JHSCAttend.SelectByStudentIDAndCourseID(new string[] { student.ID }, new string[] { course.ID });
            if (scattendList.Count > 0)
                _scattend = scattendList[0];

            #region 取得評量成績
            _listener.SuspendListen();

            // 取得所有試別
            Dictionary<string, JHExamRecord> exams = JHExam.SelectAll().ToDictionary(x => x.ID);
            List<string> examIDs = new List<string>(exams.Keys);

            List<KH.JHAEIncludeRecord> aeList = new List<KH.JHAEIncludeRecord>();

            // 當課程有評量成績時取得評量設定
            if (course.RefAssessmentSetupID != null)
            {
                JHAssessmentSetupRecord assessment = JHAssessmentSetup.SelectByID(course.RefAssessmentSetupID);
                if (assessment != null)
                    foreach (JHAEIncludeRecord ae in JHAEInclude.SelectByAssessmentSetupID(assessment.ID))
                        aeList.Add(new KH.JHAEIncludeRecord(ae));
            }

            // 排序需要再改寫
            aeList.Sort(delegate(KH.JHAEIncludeRecord x, KH.JHAEIncludeRecord y)
            {
                int ix = examIDs.IndexOf(x.RefExamID);
                int iy = examIDs.IndexOf(y.RefExamID);
                if (ix == -1) ix = int.MaxValue;
                if (iy == -1) iy = int.MaxValue;
                return ix.CompareTo(iy);
            });

            List<KH.JHSCETakeRecord> sceList = JHSCETake.SelectByStudentAndCourse(student.ID, course.ID).AsKHJHSCETakeRecords();

            Dictionary<string, DataGridViewRow> rows = new Dictionary<string, DataGridViewRow>();

            foreach (KH.JHAEIncludeRecord ae in aeList)
            {
                DataGridViewRow row = new DataGridViewRow();
                JHExamRecord exam = null;
                if (exams.ContainsKey(ae.RefExamID)) exam = exams[ae.RefExamID];
                row.CreateCells(dgv, (exam != null) ? exam.Name : "無此評量(" + ae.RefExamID + ")", "", "", "");
                dgv.Rows.Add(row);
                row.Cells[chExamName.Index].Tag = (exam != null) ? exam.ID : "";

                if (!ae.UseScore) DisableCell(row, chScore);
                if (!ae.UseEffort) DisableCell(row, chEffort);
                if (!ae.UseText) DisableCell(row, chText);

                if (!rows.ContainsKey(ae.RefExamID))
                    rows.Add(ae.RefExamID, row);
            }

            foreach (KH.JHSCETakeRecord sce in sceList)
            {
                if (rows.ContainsKey(sce.RefExamID))
                {
                    DataGridViewRow row = rows[sce.RefExamID];
                    row.Cells[chScore.Index].Value = sce.Score.HasValue ? "" + sce.Score.Value : "";
                    row.Cells[chEffort.Index].Value = sce.Effort.HasValue ? "" + sce.Effort.Value : "";
                    row.Cells[chText.Index].Value = sce.Text;
                    row.Tag = sce;
                }
                else
                {
                    DataGridViewRow row = new DataGridViewRow();
                    JHExamRecord exam = JHExam.SelectByID(sce.RefExamID);
                    row.CreateCells(dgv, (exam != null) ? exam.Name : "無此評量(" + sce.RefExamID + ")", sce.Score.HasValue ? "" + sce.Score.Value : "", sce.Effort.HasValue ? "" + sce.Effort.Value : "", sce.Text);
                    row.Tag = sce;
                    row.Cells[chExamName.Index].Tag = (exam!=null)? exam.ID:"";
                    dgv.Rows.Add(row);
                    DisableCell(row, chExamName);
                    DisableCell(row, chScore);
                    DisableCell(row, chEffort);
                    DisableCell(row, chText);
                }
            }


            if (_scattend != null)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, "平時評量", "" + _scattend.OrdinarilyScore, "" + _scattend.OrdinarilyEffort, string.Empty);
                row.Tag = "平時評量";
                dgv.Rows.Add(row);
                DisableCell(row, chText);

                DataGridViewRow row2 = new DataGridViewRow();
                row2.CreateCells(dgv, "課程總成績", "" + _scattend.Score, "" + _scattend.Effort, _scattend.Text);
                row2.Tag = "課程總成績";
                dgv.Rows.Add(row2);
            }

            foreach (DataGridViewRow dgv1 in dgv.Rows)
                foreach (DataGridViewCell cell in dgv1.Cells)
                {                    
                    cell.ErrorText = "";

                    if (cell.OwningColumn == chScore)
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
                    else if (cell.OwningColumn == chEffort)
                    {
                        if (!string.IsNullOrEmpty("" + cell.Value))
                        {
                            int i;
                            if (!int.TryParse("" + cell.Value, out i))
                                cell.ErrorText = "努力程度必須為整數";
                        }
                    }                
                }

            _listener.Reset();
            _listener.ResumeListen();
            #endregion
            SetLoadDataToLog();
        }

        private void InitializeListener()
        {
            _listener = new ChangeListener();
            _listener.Add(new DataGridViewSource(dgv));
            _listener.StatusChanged += new EventHandler<ChangeEventArgs>(Listener_StatusChanged);
        }

        private void Listener_StatusChanged(object sender, ChangeEventArgs e)
        {
            lblNotSave.Visible = (e.Status == ValueStatus.Dirty);
        }

        private void DisableCell(DataGridViewRow row, DataGridViewTextBoxColumn ch)
        {
            row.Cells[ch.Index].ReadOnly = true;
            row.Cells[ch.Index].Style.BackColor = Color.Lavender;
            row.Cells[ch.Index].Style.ForeColor = Color.Red;
        }

        // 載入 Log
        private void SetLoadDataToLog()
        {
            try
            {
                prlp.ClearCache();

                string sy = "", ss = "",key="";
                if (_course.SchoolYear.HasValue)
                    sy = _course.SchoolYear.Value.ToString();
                if (_course.Semester.HasValue)
                    ss = _course.Semester.Value.ToString();

                key = sy +"學年度第"+ ss +"學期"+ "學號:"+_student.StudentNumber +" 姓名:"+_student.Name+" "+_course.Name;


                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    if (dgvr.IsNewRow)
                        continue;

                    string strExamName = "", strScore = "", strEffort = "", strText = "";

                    if (dgvr.Cells[chExamName.Index].Value == null)
                        continue;

                    strExamName = dgvr.Cells[chExamName.Index].Value.ToString();

                    if (dgvr.Cells[chScore.Index].Value != null)
                        strScore = dgvr.Cells[chScore.Index].Value.ToString();

                    if (dgvr.Cells[chEffort.Index].Value != null)
                        strEffort = dgvr.Cells[chEffort.Index].Value.ToString();

                    if (dgvr.Cells[chText.Index].Value != null)
                        strText = dgvr.Cells[chText.Index].Value.ToString();

                    prlp.SetBeforeSaveText(key + strExamName+"分數評量",strScore);
                    prlp.SetBeforeSaveText(key + strExamName + "努力程度", strEffort);
                    prlp.SetBeforeSaveText(key + strExamName + "文字描述", strText);
                }
            }
            catch(Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("Log發生錯誤.");
            }
        }

        // 儲存 Log
        private void SetSaveDataToLog()
        {
            try
            {
                string sy = "", ss = "",key="";
                if (_course.SchoolYear.HasValue)
                    sy = _course.SchoolYear.Value.ToString();
                if (_course.Semester.HasValue)
                    ss = _course.Semester.Value.ToString();

                key = sy + "學年度第" + ss + "學期" + "學號:" + _student.StudentNumber + " 姓名:" + _student.Name + " " + _course.Name;
                                
                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    if (dgvr.IsNewRow)
                        continue;
                    
                    string strExamName = "", strScore = "", strEffort = "", strText = "";

                    if (dgvr.Cells[chExamName.Index].Value == null)
                        continue;
                        strExamName = dgvr.Cells[chExamName.Index].Value.ToString();

                    if (dgvr.Cells[chScore.Index].Value != null)
                        strScore = dgvr.Cells[chScore.Index].Value.ToString();

                    if (dgvr.Cells[chEffort.Index].Value != null)
                        strEffort = dgvr.Cells[chEffort.Index].Value.ToString();

                    if (dgvr.Cells[chText.Index].Value != null)
                        strText = dgvr.Cells[chText.Index].Value.ToString();

                    prlp.SetAfterSaveText(key + strExamName + "分數評量", strScore);
                    prlp.SetAfterSaveText(key + strExamName + "努力程度", strEffort);
                    prlp.SetAfterSaveText(key + strExamName + "文字描述", strText);                    
                }
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("Log發生錯誤.");
            }
        
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            if (!IsValid()) return;

            try
            {
                List<KH.JHSCETakeRecord> sceUpdateList = new List<KH.JHSCETakeRecord>();
                List<KH.JHSCETakeRecord> sceInsertList = new List<KH.JHSCETakeRecord>();
                List<KH.JHSCETakeRecord> sceDeleteList = new List<KH.JHSCETakeRecord>();

                bool scattendNeedSave = false;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    if ("" + row.Tag == "課程總成績")
                    {
                        #region 課程總成績
                        if ("" + row.Cells[chScore.Index].Value != "" + _scattend.Score)
                        {
                            scattendNeedSave = true;
                            decimal d;
                            if (decimal.TryParse("" + row.Cells[chScore.Index].Value, out d))
                                _scattend.Score = d;
                            else
                                _scattend.Score = null;
                        }
                        if ("" + row.Cells[chEffort.Index].Value != "" + _scattend.Effort)
                        {
                            scattendNeedSave = true;
                            int i;
                            if (int.TryParse("" + row.Cells[chEffort.Index].Value, out i))
                                _scattend.Effort = i;
                            else
                                _scattend.Effort = null;
                        }
                        if ("" + row.Cells[chText.Index].Value != _scattend.Text)
                        {
                            scattendNeedSave = true;
                            _scattend.Text = "" + row.Cells[chText.Index].Value;
                        }
                        #endregion
                    }
                    else if ("" + row.Tag == "平時評量")
                    {
                        #region 平時評量
                        if ("" + row.Cells[chScore.Index].Value != "" + _scattend.OrdinarilyScore )
                        {
                            scattendNeedSave = true;
                            decimal d;
                            if (decimal.TryParse("" + row.Cells[chScore.Index].Value, out d))
                                _scattend.OrdinarilyScore = d;
                            else
                                _scattend.OrdinarilyScore  = null;
                        }
                        if ("" + row.Cells[chEffort.Index].Value != "" + _scattend.OrdinarilyEffort)
                        {
                            scattendNeedSave = true;
                            int i;
                            if (int.TryParse("" + row.Cells[chEffort.Index].Value, out i))
                                _scattend.OrdinarilyEffort  = i;
                            else
                                _scattend.OrdinarilyEffort = null;
                        }
                        
                        #endregion

                    }
                    else if (row.Tag != null)
                    {
                        #region 有評量成績記錄的情況
                        KH.JHSCETakeRecord sce = row.Tag as KH.JHSCETakeRecord;

                        if (!string.IsNullOrEmpty("" + row.Cells[chScore.Index].Value))
                            sce.Score = decimal.Parse("" + row.Cells[chScore.Index].Value);
                        else
                            sce.Score = null;

                        if (!string.IsNullOrEmpty("" + row.Cells[chEffort.Index].Value))
                            sce.Effort = int.Parse("" + row.Cells[chEffort.Index].Value);
                        else
                            sce.Effort = null;

                        sce.Text = "" + row.Cells[chText.Index].Value;

                        if (!sce.Score.HasValue && !sce.Effort.HasValue && string.IsNullOrEmpty(sce.Text))
                            sceDeleteList.Add(sce);
                        else
                            sceUpdateList.Add(sce);
                        #endregion
                    }
                    else
                    {
                        #region 無評量成績記錄的情況
                        bool needsave = false;
                        if (!string.IsNullOrEmpty("" + row.Cells[chScore.Index].Value))
                            needsave = true;
                        if (!string.IsNullOrEmpty("" + row.Cells[chEffort.Index].Value))
                            needsave = true;
                        if (!string.IsNullOrEmpty("" + row.Cells[chText.Index].Value))
                            needsave = true;
                        if (needsave)
                        {
                            KH.JHSCETakeRecord sce = new KH.JHSCETakeRecord(new JHSCETakeRecord());
                            sce.RefCourseID = _course.ID;
                            sce.RefExamID = "" + row.Cells[chExamName.Index].Tag;
                            sce.RefSCAttendID = _scattend != null ? _scattend.ID : "";
                            sce.RefStudentID = _student.ID;
                            if (!string.IsNullOrEmpty("" + row.Cells[chScore.Index].Value))
                                sce.Score = decimal.Parse("" + row.Cells[chScore.Index].Value);
                            else
                                sce.Score = null;

                            if (!string.IsNullOrEmpty("" + row.Cells[chEffort.Index].Value))
                                sce.Effort = int.Parse("" + row.Cells[chEffort.Index].Value);
                            else
                                sce.Effort = null;

                            sce.Text = "" + row.Cells[chText.Index].Value;
                            sceInsertList.Add(sce);
                        }
                        #endregion
                    }
                }

                if (sceUpdateList.Count > 0) JHSCETake.Update(sceUpdateList.AsJHSCETakeRecords());
                if (sceInsertList.Count > 0) JHSCETake.Insert(sceInsertList.AsJHSCETakeRecords());
                if (sceDeleteList.Count > 0) JHSCETake.Delete(sceDeleteList.AsJHSCETakeRecords());

                if (scattendNeedSave) JHSCAttend.Update(_scattend);

                // log 處理
                SetSaveDataToLog();
                prlp.SetActionBy("學生", "評量成績輸入");
                prlp.SetAction("評量成績輸入");
                prlp.SetDescTitle("");
                prlp.SaveLog("", "", "Student",_student.ID);
                SetLoadDataToLog();


                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MsgBox.Show("儲存失敗。" + ex.Message);
            }
        }

        private bool IsValid()
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                        return false;
                }
            }

            return true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            cell.ErrorText = "";

            if (cell.OwningColumn == chScore)
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

                        dgv.Rows[e.RowIndex].Cells[chEffort.Index].Value = _effortMapper.GetCodeByScore(d);
                        dgv.NotifyCurrentCellDirty(true);
                    }
                }
            }
            else if (cell.OwningColumn == chEffort)
            {
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    int i;
                    if (!int.TryParse("" + cell.Value, out i))
                        cell.ErrorText = "努力程度必須為整數";
                }
            }
        }
    }
}
