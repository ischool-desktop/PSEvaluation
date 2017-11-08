using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.DAL;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.SemesterHistory;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.DailyLifeRecommend;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.Absence;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent
{
    public partial class Step3 : UserControl, IStep
    {
        private const string SCHOOL_YEAR_COLUMN = "學年度";
        private const string SEMESTER_COLUMN = "學期";

        private string _studentID;
        private List<string> _absenceList;
        private TransferStudentForm _parent;
        private List<string> _meritTypes;
        private List<string> _periodTypes;

        public Step3(TransferStudentForm parent)
        {
            InitializeComponent();

            _parent = parent;

            if (Student.Instance.SelectedList.Count == 0) return;
            _studentID = Student.Instance.SelectedList[0].ID;

            dgMeritStatistic.Columns.Add(SCHOOL_YEAR_COLUMN, SCHOOL_YEAR_COLUMN);
            dgMeritStatistic.Columns[SCHOOL_YEAR_COLUMN].Width = 75;
            dgMeritStatistic.Columns.Add(SEMESTER_COLUMN, SEMESTER_COLUMN);
            dgMeritStatistic.Columns[SEMESTER_COLUMN].Width = 60;

            _meritTypes = TransferStudentDAL.GetMeritTypes();

            foreach (string merit in _meritTypes)
            {
                int columnIndex = dgMeritStatistic.Columns.Add(merit, merit);
                dgMeritStatistic.Columns[columnIndex].Width = 60;
            }

            foreach (SemesterHistoryData sh in TransferStudentDAL.GetSemesterHistory(_studentID))
            {
                int rowIndex = dgMeritStatistic.Rows.Add();

                DataGridViewRow row = dgMeritStatistic.Rows[rowIndex];
                row.Tag = sh;

                DataGridViewCell cell = row.Cells[SCHOOL_YEAR_COLUMN];
                cell.Value = sh.SchoolYear;
                cell.ReadOnly = true;

                cell = row.Cells[SEMESTER_COLUMN];
                cell.Value = sh.Semester;
                cell.ReadOnly = true;

                //把值塞進來 .....
                foreach (string mt in _meritTypes)
                {
                    MeritStatisticItem item = _parent.TransferStudentData.MeritStatisticData.GetItem(sh.SchoolYear, sh.Semester, mt);

                    if (item != null)
                        row.Cells[mt].Value = item.Count;
                }
            }

            //處理缺曠輸入畫面            
            dgAbsence.Columns.Add(SCHOOL_YEAR_COLUMN, SCHOOL_YEAR_COLUMN);
            dgAbsence.Columns[SCHOOL_YEAR_COLUMN].Width = 75;
            dgAbsence.Columns.Add(SEMESTER_COLUMN, SEMESTER_COLUMN);
            dgAbsence.Columns[SEMESTER_COLUMN].Width = 60;

            _periodTypes = TransferStudentDAL.GetPeriodTypeItems();
            _absenceList = TransferStudentDAL.GetAbsenceItems();

            foreach (string periodType in _periodTypes)
            {
                foreach (string each in _absenceList)
                {
                    string columnName = periodType + each;

                    dgAbsence.Columns.Add(columnName, columnName);
                    dgAbsence.Columns[columnName].Width = 60;
                    dgAbsence.Columns[columnName].Tag = periodType + ":" + each;
                }
            }

            // 塞入缺曠之預設值
            foreach (SemesterHistoryData sh in TransferStudentDAL.GetSemesterHistory(_studentID))
            {
                int rowIndex = dgAbsence.Rows.Add();
                DataGridViewRow row = dgAbsence.Rows[rowIndex];

                row.Cells[SCHOOL_YEAR_COLUMN].Value = sh.SchoolYear;
                row.Cells[SEMESTER_COLUMN].Value = sh.Semester;

                foreach (string periodType in _periodTypes)
                {
                    foreach (string each in _absenceList)
                    {
                        string columnName = periodType + each;

                        AbsenceItem item = _parent.TransferStudentData.AbsenceData.GetItem(sh.SchoolYear, sh.Semester, periodType, each);

                        if (item != null)
                            row.Cells[columnName].Value = item.Count;
                    }
                }
            }
        }

        #region IStep 成員

        public bool Valid()
        {
            foreach (DataGridViewRow row in dgAbsence.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                    {
                        ErrorMessage = "缺曠統計中含有錯誤資料 ! \n欄:" + cell.ColumnIndex + "\n列:" + cell.RowIndex;
                        return false;
                    }
                }
            }
            ErrorMessage = string.Empty;
            return true;
        }

        public string ErrorMessage { get; set; }

        public void OnChangeStep()
        {
            foreach (DataGridViewRow row in dgMeritStatistic.Rows)
            {
                foreach (DataGridViewColumn column in dgMeritStatistic.Columns)
                {
                    if (column.Name == SCHOOL_YEAR_COLUMN) continue;
                    if (column.Name == SEMESTER_COLUMN) continue;

                    MeritStatisticItem item = new MeritStatisticItem();

                    item.SchoolYear = row.Cells[SCHOOL_YEAR_COLUMN].Value.ToString();
                    item.Semester = row.Cells[SEMESTER_COLUMN].Value.ToString();

                    int count = 0;
                    if (row.Cells[column.Name].Value != null)
                    {
                        string value = row.Cells[column.Name].Value.ToString();

                        if (!int.TryParse(value, out count))
                            count = 0;
                    }

                    item.MeritType = column.Name;
                    item.Count = count;

                    _parent.TransferStudentData.MeritStatisticData.SetItem(item);
                }
            }

            foreach (DataGridViewRow row in dgAbsence.Rows)
            {
                foreach (DataGridViewColumn column in dgAbsence.Columns)
                {
                    if (column.Name == SCHOOL_YEAR_COLUMN) continue;
                    if (column.Name == SEMESTER_COLUMN) continue;

                    AbsenceItem item = new AbsenceItem();

                    item.SchoolYear = row.Cells[SCHOOL_YEAR_COLUMN].Value.ToString();
                    item.Semester = row.Cells[SEMESTER_COLUMN].Value.ToString();

                    int count = 0;
                    if (row.Cells[column.Name].Value != null)
                    {
                        string value = row.Cells[column.Name].Value.ToString();

                        if (!int.TryParse(value, out count))
                            count = 0;
                    }

                    string[] tagSplit = column.Tag.ToString().Split(':');
                    string periodType = tagSplit[0];
                    string name = tagSplit[1];

                    item.PeriodType = periodType;
                    item.Name = name;
                    item.Count = count;

                    _parent.TransferStudentData.AbsenceData.SetItem(item);
                }
            }
        }

        #endregion

        private void dgAbsence_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgAbsence.Rows[e.RowIndex].Cells[e.ColumnIndex];
            DataGridViewColumn column = dgAbsence.Columns[e.ColumnIndex];

            cell.ErrorText = string.Empty;

            if (cell.Value == null) return;

            string value = cell.Value.ToString();

            if (value == string.Empty) return;

            int i;
            if (!int.TryParse(value, out i))
                cell.ErrorText = "必須為數字形態";
        }

        private void dgMeritStatistic_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgMeritStatistic.Rows[e.RowIndex].Cells[e.ColumnIndex];
            DataGridViewColumn column = dgMeritStatistic.Columns[e.ColumnIndex];

            cell.ErrorText = string.Empty;

            if (cell.Value == null) return;

            string value = cell.Value.ToString();

            if (value == string.Empty) return;

            int i;
            if (!int.TryParse(value, out i))
                cell.ErrorText = "必須為數字形態";
        }
    }
}
