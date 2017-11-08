using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.GroupActivity;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.DailyBehavior;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.SemesterHistory;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.DAL;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent
{
    public partial class Step1 : UserControl, IStep
    {
        private const string SCHOOL_YEAR_COLUMN = "學年度";
        private const string SEMESTER_COLUMN = "學期";
        private const string TEXT_ENDWITH = "-文字評量";
        private const string DEGREE_ENDWITH = "-努力程度";

        private string _studentID;
        private Dictionary<string, string> _dailyBehaviorDegrees;
        private Dictionary<string, string> _groupActivityDegrees;
        private TransferStudentForm _parent;
        List<string> _groupActivityItems;

        //public TransferStudentData TransferStudentData { get; set; }

        public Step1(TransferStudentForm parent)
        {
            InitializeComponent();

            _parent = parent;

            if (Student.Instance.SelectedList.Count == 0) return;
            _studentID = Student.Instance.SelectedList[0].ID;

            // 處理日常生活表現畫面
            dgDailyBehavior.Columns.Add(SCHOOL_YEAR_COLUMN, SCHOOL_YEAR_COLUMN);
            dgDailyBehavior.Columns[SCHOOL_YEAR_COLUMN].Width = 75;
            dgDailyBehavior.Columns.Add(SEMESTER_COLUMN, SEMESTER_COLUMN);
            dgDailyBehavior.Columns[SEMESTER_COLUMN].Width = 60;

            List<string> dailyBehaviorItems = TransferStudentDAL.GetDailyBehaviorItems();

            foreach (string dailyBehavior in dailyBehaviorItems)
            {
                dgDailyBehavior.Columns.Add(dailyBehavior, dailyBehavior);
            }

            foreach (SemesterHistoryData sh in TransferStudentDAL.GetSemesterHistory(_studentID))
            {
                int rowIndex = dgDailyBehavior.Rows.Add();

                DataGridViewRow row = dgDailyBehavior.Rows[rowIndex];
                row.Tag = sh;

                DataGridViewCell cell = row.Cells[SCHOOL_YEAR_COLUMN];
                cell.Value = sh.SchoolYear;
                cell.ReadOnly = true;

                cell = row.Cells[SEMESTER_COLUMN];
                cell.Value = sh.Semester;
                cell.ReadOnly = true;

                foreach (string each in dailyBehaviorItems)
                {
                    DailyBehaviorItem item = _parent.TransferStudentData.DailyBehaviorData.GetItem(each, sh.SchoolYear, sh.Semester);

                    if (item != null)
                        row.Cells[each].Value = item.Degree;
                }
            }

            _dailyBehaviorDegrees = TransferStudentDAL.GetDailyBehaviorDegrees();

            //處理團體活動輸入畫面
            dgTeam.Columns.Add(SCHOOL_YEAR_COLUMN, SCHOOL_YEAR_COLUMN);
            dgTeam.Columns[SCHOOL_YEAR_COLUMN].Width = 75;
            dgTeam.Columns.Add(SEMESTER_COLUMN, SEMESTER_COLUMN);
            dgTeam.Columns[SEMESTER_COLUMN].Width = 60;

            _groupActivityItems = TransferStudentDAL.GetGroupActivitieItems();
            foreach (string each in _groupActivityItems)
            {
                string each1 = GetGADegreeColName(each);
                string each2 = GetGATextColName(each);

                int columnIndex1 = dgTeam.Columns.Add(each1, each1);
                int columnIndex2 = dgTeam.Columns.Add(each2, each2);

                DataGridViewColumn column1 = dgTeam.Columns[columnIndex1];
                DataGridViewColumn column2 = dgTeam.Columns[columnIndex2];

                column1.Tag = each;
                column2.Tag = each;
            }

            foreach (SemesterHistoryData sh in TransferStudentDAL.GetSemesterHistory(_studentID))
            {
                int rowIndex = dgTeam.Rows.Add();

                DataGridViewRow row = dgTeam.Rows[rowIndex];
                row.Tag = sh;

                DataGridViewCell cell = row.Cells[SCHOOL_YEAR_COLUMN];
                cell.Value = sh.SchoolYear;
                cell.ReadOnly = true;

                cell = row.Cells[SEMESTER_COLUMN];
                cell.Value = sh.Semester;
                cell.ReadOnly = true;

                foreach (string each in _groupActivityItems)
                {
                    GroupActivityItem item = _parent.TransferStudentData.GroupActivityData.GetItem(each, sh.SchoolYear, sh.Semester);

                    if (item != null)
                    {
                        row.Cells[GetGADegreeColName(each)].Value = item.Degree;
                        row.Cells[GetGATextColName(each)].Value = item.Text;
                    }
                }
            }

            _groupActivityDegrees = TransferStudentDAL.GetGroupActivityDegrees();
        }

        private void dgDailyBehavior_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgDailyBehavior.Rows[e.RowIndex].Cells[e.ColumnIndex];

            string value = string.Empty;

            cell.ErrorText = string.Empty;

            if (cell.Value != null)
                value = cell.Value.ToString();

            if (string.IsNullOrEmpty(value)) return;

            if (_dailyBehaviorDegrees.ContainsKey(value))
            {
                value = _dailyBehaviorDegrees[value];
                cell.Value = value;
            }

            if (!_dailyBehaviorDegrees.ContainsValue(value))
                cell.ErrorText = "無此項程度, 請重新輸入";

        }

        private string GetGATextColName(string groupActivityName)
        {
            return groupActivityName + TEXT_ENDWITH;
        }

        private string GetGADegreeColName(string groupActivityName)
        {
            return groupActivityName + DEGREE_ENDWITH;
        }

        private void dgTeam_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dgTeam.Rows[e.RowIndex].Cells[e.ColumnIndex];

            DataGridViewColumn column = dgTeam.Columns[e.ColumnIndex];

            if (!column.HeaderText.EndsWith(DEGREE_ENDWITH)) return;

            cell.ErrorText = string.Empty;

            string value = string.Empty;

            if (cell.Value != null)
                value = cell.Value.ToString();

            if (string.IsNullOrEmpty(value)) return;

            if (_groupActivityDegrees.ContainsKey(value))
            {
                value = _groupActivityDegrees[value];
                cell.Value = value;
            }

            if (!_groupActivityDegrees.ContainsValue(value))
                cell.ErrorText = "無此項程度, 請重新輸入";
        }

        #region IStep 成員
        public string ErrorMessage { get; set; }

        public bool Valid()
        {
            //檢查日常生活表現有無錯誤
            foreach (DataGridViewRow row in dgDailyBehavior.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                    {
                        ErrorMessage = "日常生活表現中含有錯誤資料 ! \n欄:" + cell.ColumnIndex + "\n列:" + cell.RowIndex;
                        return false;
                    }
                }
            }

            //檢查團體活動表現有無錯誤
            foreach (DataGridViewRow row in dgTeam.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                    {
                        ErrorMessage = "團體活動表現中含有錯誤資料 ! \n欄:" + cell.ColumnIndex + "\n列:" + cell.RowIndex;
                        return false;
                    }
                }
            }

            ErrorMessage = string.Empty;
            return true;
        }

        public void OnChangeStep()
        {
            //將值塞入 _data 中
            foreach (DataGridViewRow row in dgDailyBehavior.Rows)
            {                
                SemesterHistoryData sh = row.Tag as SemesterHistoryData;

                foreach (DataGridViewColumn column in dgDailyBehavior.Columns)
                {
                    if (column.Name == SCHOOL_YEAR_COLUMN) continue;
                    if (column.Name == SEMESTER_COLUMN) continue;

                    string value = string.Empty;

                    if (row.Cells[column.Name].Value != null)
                        value = row.Cells[column.Name].Value.ToString();

                    DailyBehaviorItem item = new DailyBehaviorItem();
                    item.SchoolYear = sh.SchoolYear;
                    item.Semester = sh.Semester;
                    //item.Index = string.Empty;
                    item.Name = column.Name;
                    item.Degree = value;

                    _parent.TransferStudentData.DailyBehaviorData.SetItem(item);
                }
            }

            foreach (DataGridViewRow row in dgTeam.Rows)
            {                
                SemesterHistoryData sh = row.Tag as SemesterHistoryData;

                foreach (string groupActivityItem in _groupActivityItems)
                {
                    string degree = string.Empty;
                    DataGridViewCell degreeCell = row.Cells[GetGADegreeColName(groupActivityItem)];
                    if (degreeCell.Value != null)
                        degree = degreeCell.Value.ToString();

                    string text = string.Empty;
                    DataGridViewCell textCell = row.Cells[GetGATextColName(groupActivityItem)];
                    if (textCell.Value != null)
                        text = textCell.Value.ToString();

                    GroupActivityItem item = new GroupActivityItem();
                    item.SchoolYear = sh.SchoolYear;
                    item.Semester = sh.Semester;
                    item.Name = groupActivityItem;
                    item.Degree = degree;
                    item.Text = text;

                    _parent.TransferStudentData.GroupActivityData.SetItem(item);
                }
            }
        }

        #endregion
    }
}
