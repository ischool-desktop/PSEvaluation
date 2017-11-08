using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Framework;

namespace JHSchool.Evaluation.StudentExtendControls.CourseScoreItemControls
{
    public partial class ScoreInputForm : FISCA.Presentation.Controls.BaseForm
    {
        private ChangeListener _listener;
        private Data.JHStudentRecord _student;
        private Data.JHCourseRecord _course;
        private Data.JHSCAttendRecord _scattend;

        public ScoreInputForm(Data.JHStudentRecord student, Data.JHCourseRecord course)
        {
            InitializeComponent();
            InitializeListener();

            lblCourseName.Text = course.Name;
            lblStudent.Text = student.Name + " " + student.StudentNumber;

            _course = course;
            _student = student;

            List<Data.JHSCAttendRecord> scattendList = Data.JHSCAttend.SelectByStudentIDAndCourseID(new string[] { student.ID }, new string[] { course.ID });
            if (scattendList.Count > 0)
                _scattend = scattendList[0];

            #region 取得評量成績
            _listener.SuspendListen();

            List<Data.JHAEIncludeRecord> aeList = new List<JHSchool.Data.JHAEIncludeRecord>();

            if (course.RefAssessmentSetupID != null)
            {
                Data.JHAssessmentSetupRecord assessment = Data.JHAssessmentSetup.SelectByID(course.RefAssessmentSetupID);
                if (assessment != null)
                {
                    foreach (Data.JHAEIncludeRecord ae in Data.JHAEInclude.SelectAll())
                    {
                        if (ae.RefAssessmentSetupID == assessment.ID)
                            aeList.Add(ae);
                    }
                }
            }

            List<Data.JHSCETakeRecord> sceList = Data.JHSCETake.SelectByStudentAndCourse(student.ID, course.ID);

            aeList.Sort(delegate(Data.JHAEIncludeRecord x, Data.JHAEIncludeRecord y)
            {
                return x.RefExamID.CompareTo(y.RefExamID);
            });

            Dictionary<string, DataGridViewRow> rows = new Dictionary<string, DataGridViewRow>();
            foreach (Data.JHAEIncludeRecord ae in aeList)
            {
                DataGridViewRow row = new DataGridViewRow();
                Data.JHExamRecord exam = Data.JHExam.SelectByID(ae.RefExamID);
                row.CreateCells(dgv, (exam != null) ? exam.Name : "無此評量(" + ae.RefExamID + ")", "", "", "");
                dgv.Rows.Add(row);
                row.Cells[chExamName.Index].Tag = exam.ID;

                if (!ae.UseScore) DisableCell(row, chScore);
                if (!ae.UseEffort) DisableCell(row, chEffort);
                if (!ae.UseText) DisableCell(row, chText);

                if (!rows.ContainsKey(ae.RefExamID))
                    rows.Add(ae.RefExamID, row);
            }

            foreach (Data.JHSCETakeRecord sce in sceList)
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
                    Data.JHExamRecord exam = Data.JHExam.SelectByID(sce.RefExamID);
                    row.CreateCells(dgv, (exam != null) ? exam.Name : "無此評量(" + sce.RefExamID + ")", sce.Score.HasValue ? "" + sce.Score.Value : "", sce.Effort.HasValue ? "" + sce.Effort.Value : "", sce.Text);
                    row.Tag = sce;
                    row.Cells[chExamName.Index].Tag = exam.ID;
                    dgv.Rows.Add(row);
                    DisableCell(row, chExamName);
                    DisableCell(row, chScore);
                    DisableCell(row, chEffort);
                    DisableCell(row, chText);
                }
            }

            #region 填入修課總成績
            if (_scattend != null)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, "課程總成績", "" + _scattend.Score, "" + _scattend.Effort, _scattend.Text);
                row.Tag = "課程總成績";
                dgv.Rows.Add(row);
            }
            #endregion

            _listener.Reset();
            _listener.ResumeListen();
            #endregion
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            if (!IsValid()) return;

            try
            {
                List<Data.JHSCETakeRecord> sceUpdateList = new List<JHSchool.Data.JHSCETakeRecord>();
                List<Data.JHSCETakeRecord> sceInsertList = new List<JHSchool.Data.JHSCETakeRecord>();
                List<Data.JHSCETakeRecord> sceDeleteList = new List<JHSchool.Data.JHSCETakeRecord>();

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
                    else if (row.Tag != null)
                    {
                        #region 有評量成績記錄的情況
                        Data.JHSCETakeRecord sce = row.Tag as Data.JHSCETakeRecord;

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
                            Data.JHSCETakeRecord sce = new Data.JHSCETakeRecord();
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

                if (sceUpdateList.Count > 0) Data.JHSCETake.Update(sceUpdateList);
                if (sceInsertList.Count > 0) Data.JHSCETake.Insert(sceInsertList);
                if (sceDeleteList.Count > 0) Data.JHSCETake.Delete(sceDeleteList);

                if (scattendNeedSave) Data.JHSCAttend.Update(_scattend);

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
                if (!string.IsNullOrEmpty("" + cell.Value))
                {
                    decimal d;
                    if (!decimal.TryParse("" + cell.Value, out d))
                        cell.ErrorText = "分數必須為數字";
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
