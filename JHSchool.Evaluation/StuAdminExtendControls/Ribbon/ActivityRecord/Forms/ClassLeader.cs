using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.Data;
using JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.DAL;
using DevComponents.DotNetBar;

namespace JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.Forms
{
    public partial class ClassLeader : FISCA.Presentation.Controls.BaseForm
    {
        private List<MappingItem> _mappingItems;
        private bool _dirty;
        private bool _initialized = false;
        private string RecorverSchoolYear { get; set; }
        private string RecorverSemester { get; set; }
        private int RecorverIndex { get; set; }

        public ClassLeader()
        {
            InitializeComponent();

            _mappingItems = ActivityRecordDAL.GetActivityRecordMappingItems("班級幹部");

            int schoolYear;
            if (!int.TryParse(School.DefaultSchoolYear, out schoolYear))
                schoolYear = 97;

            for (int i = schoolYear; i > schoolYear - 3; i--)
            {
                cboSchoolYear.Items.Add(i);
            }
            cboSchoolYear.Text = schoolYear.ToString();
            RecorverSchoolYear = cboSchoolYear.Text;

            cboSemester.Items.Add(1);
            cboSemester.Items.Add(2);
            cboSemester.Text = School.DefaultSemester == "2" ? "2" : "1";
            RecorverSemester = cboSemester.Text;

            foreach (ClassRecord cr in Class.Instance.Items)
            {                
                cboClass.Items.Add(cr.Name);
            }

            if (cboClass.Items.Count > 0)
            {
                cboClass.SelectedIndex = 0;
                RecorverIndex = 0;
            }

            _initialized = true;
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

            if (e.ColumnIndex == colSeatNo.Index)
            {
                StudentRecord student = ActivityRecordDAL.GetStudent(cboClass.Text, value);
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

            List<ActivityRecordItem> arItems = ActivityRecordDAL.GetActivityRecordItems(cboSchoolYear.Text, cboSemester.Text, "班級幹部", cboClass.Text);
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
                Save(cboSchoolYear.Text, cboSemester.Text, cboClass.Text);
                MessageBoxEx.Show("儲存完畢");
            }
            else
            {
                MessageBoxEx.Show("資料內容有誤, 請修正後再行儲存!", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearStudentRow(DataGridViewRow row)
        {
            row.Cells[colName.Name].Value = string.Empty;
            row.Cells[colStudentNumber.Name].Value = string.Empty;
            row.Cells[colSeatNo.Name].Value = string.Empty;
            row.Tag = null;
            row.ErrorText = string.Empty;
        }

        private void FillStudentRow(DataGridViewRow row, StudentRecord student)
        {
            if (student != null)
            {
                row.Cells[colName.Name].Value = student.Name;
                row.Cells[colStudentNumber.Name].Value = student.StudentNumber;
                row.Cells[colSeatNo.Name].Value = student.SeatNo;
                row.Tag = student;
            }
            else
            {
                row.Cells[colName.Name].Value = string.Empty;
                row.Tag = null;
                row.ErrorText = "查無此學生";
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

        private void btnClearRow_Click(object sender, EventArgs e)
        {
            if (dgEditor.SelectedRows.Count == 0) return;
            
            _dirty = true;

            foreach (DataGridViewRow row in dgEditor.SelectedRows)
            {
                ClearStudentRow(row);
            }
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
                        string classname = cboClass.Items[RecorverIndex].ToString();
                        Save(RecorverSchoolYear, RecorverSemester, classname);
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
                        string classname = cboClass.Items[RecorverIndex].ToString();
                        Save(RecorverSchoolYear, RecorverSemester, classname);
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

        private void cboClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(errorProvider1.GetError(cboSemester)) || !string.IsNullOrEmpty(errorProvider1.GetError(cboSchoolYear)))
                return;

            if (cboClass.SelectedIndex == RecorverIndex)
                return;

            if (_dirty)
            {
                if (MessageBoxEx.Show("資料已變更且尚未儲存!\n是否儲存?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (ValidAll())
                    {
                        string classname = cboClass.Items[RecorverIndex].ToString();
                        Save(RecorverSemester, RecorverSemester, classname);
                    }
                    else
                    {
                        MessageBoxEx.Show("資料有誤, 請修正後再行儲存");
                        cboClass.SelectedIndex = RecorverIndex;
                        return;
                    }
                }
            }
            RecorverIndex = cboClass.SelectedIndex;
            errorProvider1.SetError(cboClass, string.Empty);
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

        private void Save(string schoolyear, string semester, string classname)
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
                    item.Type = "班級幹部";
                    item.Unit = classname;

                    list.Add(item);
                }
            }

            ActivityRecordDAL.SaveClassLeader(schoolyear, semester, classname, list.ToArray());
            _dirty = false;
        }
   
    }
}
