using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.DAL;
using JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.Data;
using DevComponents.DotNetBar;

namespace JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.Forms
{
    public partial class SchoolLeader : FISCA.Presentation.Controls.BaseForm
    {
        private List<MappingItem> _mappingItems;
        private bool _dirty;
        private bool _initialized = false;
        private string RecorverSchoolYear { get; set; }
        private string RecorverSemester { get; set; }

        public SchoolLeader()
        {
            InitializeComponent();

            int schoolYear;
            if (!int.TryParse(School.DefaultSchoolYear, out schoolYear))
                schoolYear = 97;

            for (int i = schoolYear; i > schoolYear - 3; i--)
            {
                cboSchoolYear.Items.Add(i);
            }
            cboSchoolYear.Text = schoolYear.ToString();

            cboSemester.Items.Add(1);
            cboSemester.Items.Add(2);
            cboSemester.Text = School.DefaultSemester;

            _mappingItems = ActivityRecordDAL.GetActivityRecordMappingItems("學校幹部");

            _initialized = true;
            RecorverSchoolYear = cboSchoolYear.Text;
            RecorverSemester = cboSemester.Text;
            ChangeSelection();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgEditor_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _dirty = true;
            DataGridViewRow row = dgEditor.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            string value = string.Empty;

            if (cell.Value != null)
                value = cell.Value.ToString();

            row.Tag = null;
            row.ErrorText = string.Empty;

            if (e.ColumnIndex == colSeatNo.Index || e.ColumnIndex == colClass.Index)
            {
                string className = string.Empty;
                if (row.Cells[colClass.Name].Value != null)
                    className = row.Cells[colClass.Name].Value.ToString();

                string seatNo = string.Empty;
                if (row.Cells[colSeatNo.Name].Value != null)
                    seatNo = row.Cells[colSeatNo.Name].Value.ToString();

                if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(seatNo))
                {
                    row.ErrorText = "請輸入完整的班級與座號";
                    return;
                }

                StudentRecord student = ActivityRecordDAL.GetStudent(className, seatNo);
                FillStudentRow(row, student);
            }
            else if (e.ColumnIndex == colStudentNumber.Index)
            {
                StudentRecord student = ActivityRecordDAL.GetStudent(value);
                FillStudentRow(row, student);
            }
        }

        private void ChangeSelection()
        {
            _dirty = false;
            if (!_initialized) return;

            List<ActivityRecordItem> arItems = ActivityRecordDAL.GetActivityRecordItems(cboSchoolYear.Text, cboSemester.Text, "學校幹部", string.Empty);

            dgEditor.Rows.Clear();
            foreach (MappingItem item in _mappingItems)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    int rowIndex = dgEditor.Rows.Add();
                    DataGridViewRow row = dgEditor.Rows[rowIndex];
                    row.Cells[colItem.Index].Value = item.Item;
                }
            }

            foreach (ActivityRecordItem ari in arItems)
            {
                StudentRecord student = Student.Instance[ari.StudentID];

                foreach (DataGridViewRow row in dgEditor.Rows)
                {
                    if (row.Cells[colItem.Name].Value.ToString() == ari.Item && row.Tag == null && student != null)
                    {
                        FillStudentRow(row, student);
                        break;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidAll())
            {
                Save(cboSchoolYear.Text, cboSemester.Text);
                MessageBoxEx.Show("儲存完畢");
            }
            else
            {
                MessageBoxEx.Show("資料內容有誤, 請修正後再行儲存!", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Save()
        {
            List<ActivityRecordItem> list = new List<ActivityRecordItem>();

            if (!string.IsNullOrEmpty(errorProvider1.GetError(cboSemester)) || !string.IsNullOrEmpty(errorProvider1.GetError(cboSchoolYear)))
            {
                MessageBoxEx.Show("資料有誤, 請修正後再行儲存！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (DataGridViewRow row in dgEditor.Rows)
            {
                if (!string.IsNullOrEmpty(row.ErrorText))
                {
                    MessageBoxEx.Show("資料有誤, 請修正後再行儲存！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                StudentRecord student = row.Tag as StudentRecord;

                if (student != null)
                {
                    ActivityRecordItem item = new ActivityRecordItem();
                    item.StudentID = student.ID;
                    item.SchoolYear = cboSchoolYear.Text;
                    item.Semester = cboSemester.Text;
                    item.Item = row.Cells[colItem.Name].Value.ToString();
                    item.Type = "學校幹部";
                    item.Unit = string.Empty;

                    list.Add(item);
                }
            }

            ActivityRecordDAL.SaveSchoolLeader(cboSchoolYear.Text, cboSemester.Text, list.ToArray());
            _dirty = false;
        }

        private void ClearStudentRow(DataGridViewRow row)
        {
            row.Cells[colName.Name].Value = string.Empty;
            row.Cells[colClass.Name].Value = string.Empty;
            row.Cells[colStudentNumber.Name].Value = string.Empty;
            row.Cells[colSeatNo.Name].Value = string.Empty;
            row.ErrorText = string.Empty;
            row.Tag = null;
        }

        private void FillStudentRow(DataGridViewRow row, StudentRecord student)
        {
            ClearStudentRow(row);

            if (student != null)
            {
                row.Cells[colName.Name].Value = student.Name;
                row.Cells[colClass.Name].Value = (student.Class != null ? student.Class.Name : string.Empty);
                row.Cells[colStudentNumber.Name].Value = student.StudentNumber;
                row.Cells[colSeatNo.Name].Value = student.SeatNo;
                row.Tag = student;
            }
            else
            {
                row.ErrorText = "查無此學生";
                row.Cells[colName.Name].Value = string.Empty;
                row.Tag = null;
            }
        }

        private void dgEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (dgEditor.SelectedRows.Count == 0) return;
                _dirty = true;

                foreach (DataGridViewRow row in dgEditor.SelectedRows)
                    ClearStudentRow(row);
            }
        }


        private void cboSchoolYear_Validating(object sender, CancelEventArgs e)
        {
            if (cboSchoolYear.Text == RecorverSchoolYear)
                return;

            int s;
            if (!int.TryParse(cboSchoolYear.Text, out s))
            {
                errorProvider1.SetError(cboSchoolYear, "必須為數字");
                e.Cancel = true;
            }
            else
            {
                if (_dirty)
                {
                    if (MessageBoxEx.Show("資料已變更且尚未儲存!\n是否要儲存?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        cboSchoolYear.Text = RecorverSchoolYear;
                        e.Cancel = true;
                    }
                    else
                    {
                        RecorverSchoolYear = cboSchoolYear.Text;
                        errorProvider1.SetError(cboSchoolYear, string.Empty);
                        ChangeSelection();
                    }
                }
                else
                {
                    RecorverSchoolYear = cboSchoolYear.Text;
                    errorProvider1.SetError(cboSchoolYear, string.Empty);
                    ChangeSelection();
                }
            }
        }

        private void cboSemester_Validating(object sender, CancelEventArgs e)
        {
            if (cboSemester.Text == RecorverSemester)
                return;

            if (cboSemester.Text != "1" && cboSemester.Text != "2")
            {
                errorProvider1.SetError(cboSemester, "必須為數字1或2");
                e.Cancel = true;
            }
            else
            {
                if (_dirty)
                {
                    if (MessageBoxEx.Show("資料已變更且尚未儲存!\n是否繼續?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    {
                        Save();
                    }
                    else
                    {
                        RecorverSemester = cboSemester.Text;
                        errorProvider1.SetError(cboSemester, string.Empty);
                        ChangeSelection();
                    }
                }
            }
        }

        private void btnClearRow_Click(object sender, EventArgs e)
        {
            if (dgEditor.SelectedRows.Count == 0) return;
            _dirty = true;
            foreach (DataGridViewRow row in dgEditor.SelectedRows)
                ClearStudentRow(row);
        }

        private void cboSchoolYear_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(cboSchoolYear, string.Empty);
            int s;
            if (!int.TryParse(cboSchoolYear.Text, out s))
            {
                errorProvider1.SetError(cboSchoolYear, "必須為數字");
                return;
            }

            if (cboSchoolYear.Text == RecorverSchoolYear)
                return;

            if (_dirty)
            {
                if (MessageBoxEx.Show("資料已變更且尚未儲存!\n是否儲存?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (ValidAll())
                    {
                        Save(RecorverSchoolYear, RecorverSemester);
                    }
                    else
                    {
                        MessageBoxEx.Show("資料有誤, 請修正後再行儲存");
                        cboSchoolYear.Text = RecorverSchoolYear;
                        return;
                    }
                }
            }
            RecorverSchoolYear = cboSchoolYear.Text;
            ChangeSelection();
        }

        private void cboSemester_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(cboSemester, string.Empty);
            int s;
            if (!int.TryParse(cboSemester.Text, out s))
            {
                errorProvider1.SetError(cboSemester, "必須為數字");
                return;
            }

            if (cboSemester.Text == RecorverSemester)
                return;

            if (_dirty)
            {
                if (MessageBoxEx.Show("資料已變更且尚未儲存!\n是否儲存?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (ValidAll())
                    {
                        Save(RecorverSchoolYear, RecorverSemester);
                    }
                    else
                    {
                        MessageBoxEx.Show("資料有誤, 請修正後再行儲存");
                        cboSemester.Text = RecorverSemester;
                        return;
                    }
                }
            }
            RecorverSemester = cboSemester.Text;
            ChangeSelection();
        }

        private bool ValidAll()
        {
            if (!string.IsNullOrEmpty(errorProvider1.GetError(cboSchoolYear))) return false;
            if (!string.IsNullOrEmpty(errorProvider1.GetError(cboSemester))) return false;

            foreach (DataGridViewRow row in dgEditor.Rows)
            {
                if (!string.IsNullOrEmpty(row.ErrorText)) return false;
                foreach (DataGridViewCell cell in row.Cells)
                    if (!string.IsNullOrEmpty(cell.ErrorText)) return false;
            }
            return true;
        }

        private void Save(string schoolyear, string semester)
        {
            List<ActivityRecordItem> list = new List<ActivityRecordItem>();

            foreach (DataGridViewRow row in dgEditor.Rows)
            {
                StudentRecord student = row.Tag as StudentRecord;

                if (student != null)
                {
                    ActivityRecordItem item = new ActivityRecordItem();
                    item.StudentID = student.ID;
                    item.SchoolYear = schoolyear;
                    item.Semester = semester;
                    item.Item = row.Cells[colItem.Name].Value.ToString();
                    item.Type = "學校幹部";

                    list.Add(item);
                }
            }

            ActivityRecordDAL.SaveSchoolLeader(schoolyear, semester, list.ToArray());
            _dirty = false;
        }

    }
}
