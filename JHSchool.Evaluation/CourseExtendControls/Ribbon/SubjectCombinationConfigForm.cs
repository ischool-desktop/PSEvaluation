using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon
{
    public partial class SubjectCombinationConfigForm : BaseForm
    {
        private bool CanClose { get; set; }
        private List<string> SubjectList { get; set; }
        private List<JHExamRecord> ExamList { get; set; }
        private const string spliter = ", ";

        public SubjectCombinationConfigForm(List<JHCourseRecord> courseList)
        {
            InitializeComponent();
            CanClose = false;

            #region 找出這些課程科目的聯集
            List<string> subjectList = new List<string>();
            foreach (JHCourseRecord course in courseList)
            {
                string subject = course.Subject;
                if (string.IsNullOrEmpty(subject)) continue;

                if (!subjectList.Contains(subject))
                    subjectList.Add(subject);
            }
            subjectList.Sort(SubjectSorter.Sort);
            SubjectList = subjectList;
            #endregion

            #region 找出這些課程試別的聯集
            string asID = courseList[0].RefAssessmentSetupID; //每個課程的 AssessmentSetupID 都一樣
            List<string> examIDList = new List<string>();
            foreach (JHAEIncludeRecord ae in JHAEInclude.SelectByAssessmentSetupID(asID))
            {
                string examID = ae.RefExamID;
                if (string.IsNullOrEmpty(examID)) continue;

                if (!examIDList.Contains(examID))
                    examIDList.Add(examID);
            }
            ExamList = JHExam.SelectByIDs(examIDList);
            #endregion

            InitializeSourceComboBox();
            InitializeExamComboBox();
        }

        private void InitializeSourceComboBox()
        {
            foreach (string subject in SubjectList)
                chSource.Items.Add(subject);
        }

        private void InitializeExamComboBox()
        {
            chExam.DisplayMember = "Name";
            foreach (JHExamRecord exam in ExamList)
                chExam.Items.Add(exam);
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex < 0) return;

            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.OwningRow.IsNewRow) return;
            if (cell.OwningColumn != chTargets) return;

            cmsSubject.Cell = cell;
            cmsSubject.SetSubjects(GetSourceSubject(e.RowIndex), SubjectList, GetSubjects(cell));

            cmsSubject.Show(GetLocation(e.ColumnIndex, e.RowIndex));
            cmsSubject.Refresh();
        }

        private string GetSourceSubject(int rowIndex)
        {
            return "" + dgv.Rows[rowIndex].Cells[chSource.Index].Value;
        }

        private List<string> GetSubjects(DataGridViewCell cell)
        {
            List<string> checkList = new List<string>();
            string value = "" + cell.Value;
            foreach (string s in value.Split(new string[] { spliter }, StringSplitOptions.RemoveEmptyEntries))
                checkList.Add(s);
            return checkList;
        }

        private void RemoveSubject(DataGridViewCell cell, string subject)
        {
            string oldValue = "" + cell.Value;
            string newValue = string.Empty;
            foreach (string s in oldValue.Split(new string[] { spliter }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (s != subject)
                    newValue += s + spliter;
            }
            if (newValue.EndsWith(spliter)) newValue = newValue.Substring(0, newValue.Length - spliter.Length);
            cell.Value = newValue;
        }

        #region 取得 Cell 在畫面上的座標
        private Point GetLocation(int colIndex, int rowIndex)
        {
            return dgv.PointToScreen(new Point(GetX(colIndex), GetY(rowIndex)));
        }

        private int GetX(int index)
        {
            int width = 0;
            for (int i = 0; i < index; i++)
                width += dgv.Columns[i].Width;
            width += dgv.RowHeadersWidth;
            return width;
        }

        private int GetY(int index)
        {
            int width = 0;
            for (int i = 0; i < index; i++)
                width += dgv.Rows[i].Height;
            width += dgv.ColumnHeadersHeight;
            return width;
        }
        #endregion

        private void cmsSubject_MouseEnter(object sender, EventArgs e)
        {
            CanClose = true;
        }

        private void cmsSubject_MouseLeave(object sender, EventArgs e)
        {
            if (CanClose == true)
                cmsSubject.Close();

            CanClose = false;
        }

        private void cmsSubject_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (CanClose == false)
                e.Cancel = true;
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgv.EndEdit();

            if (dgv.SelectedCells.Count <= 0) return;
            DataGridViewCell cell = dgv.SelectedCells[0];
            if (cell.OwningColumn == chTargets) return;

            if (cell.OwningColumn == chSource)
            {
                string subject = "" + cell.Value;
                RemoveSubject(cell.OwningRow.Cells[chTargets.Index], subject);
            }

            CheckComboBoxDuplication(cell.OwningColumn);
        }

        private void CheckComboBoxDuplication(DataGridViewColumn column)
        {
            List<string> unique = new List<string>();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                DataGridViewCell cell = row.Cells[column.Index];
                string value = "" + cell.Value;
                if (!unique.Contains(value))
                {
                    unique.Add(value);
                    cell.ErrorText = string.Empty;
                }
                else
                    cell.ErrorText = column.HeaderText + "不可重覆";
            }
        }

        private bool IsValid()
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                foreach (DataGridViewCell cell in row.Cells)
                    if (!string.IsNullOrEmpty(cell.ErrorText)) return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValid())
            {
                MsgBox.Show("請先修正錯誤。");
                return;
            }
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        internal static bool CheckAssessmentSetup(List<JHCourseRecord> courseList)
        {
            List<string> asIDs = new List<string>();
            foreach (JHCourseRecord course in courseList)
            {
                if (!asIDs.Contains(course.RefAssessmentSetupID))
                    asIDs.Add(course.RefAssessmentSetupID);
            }

            if (asIDs.Count > 1) //選擇的課程中，有超過一種以上的評量設定(AssessmentSetup)。
            {
                // UNDONE: 這邊的訊息需要調整一下
                MsgBox.Show("評量設定不一致");
                return false;
            }
            else
                return true;
        }
    }
}
