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
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.PublicService;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.SchoolSpecial;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.DailyLifeRecommend;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent
{
    public partial class Step2 : UserControl, IStep
    {
        private TransferStudentForm _parent;
        private const string SCHOOL_YEAR_COLUMN = "學年度";
        private const string SEMESTER_COLUMN = "學期";        
        private string _studentID;
        private Dictionary<string, string> _dailyLifeRecommendDegrees;

        public Step2(TransferStudentForm parent)
        {
            _parent = parent;

            InitializeComponent();

            if (Student.Instance.SelectedList.Count == 0) return;
            _studentID = Student.Instance.SelectedList[0].ID;

            // 處理日常生活表現畫面
            dgDailyLifeRecommend.Columns.Add(SCHOOL_YEAR_COLUMN, SCHOOL_YEAR_COLUMN);
            dgDailyLifeRecommend.Columns[SCHOOL_YEAR_COLUMN].Width = 75;
            dgDailyLifeRecommend.Columns.Add(SEMESTER_COLUMN, SEMESTER_COLUMN);
            dgDailyLifeRecommend.Columns[SEMESTER_COLUMN].Width = 65;

            List<string> dailyLifeRecommendItems = TransferStudentDAL.GetDailyLifeRecommendItems();

            foreach (string dailyLifeRecommend in dailyLifeRecommendItems)
            {
                int columnIndex = dgDailyLifeRecommend.Columns.Add(dailyLifeRecommend, dailyLifeRecommend);
                DataGridViewColumn column = dgDailyLifeRecommend.Columns[columnIndex];
                column.Width = 400;
            }

            foreach (SemesterHistoryData sh in TransferStudentDAL.GetSemesterHistory(_studentID))
            {
                int rowIndex = dgDailyLifeRecommend.Rows.Add();

                DataGridViewRow row = dgDailyLifeRecommend.Rows[rowIndex];
                row.Tag = sh;

                DataGridViewCell cell = row.Cells[SCHOOL_YEAR_COLUMN];
                cell.Value = sh.SchoolYear;
                cell.ReadOnly = true;

                cell = row.Cells[SEMESTER_COLUMN];
                cell.Value = sh.Semester;
                cell.ReadOnly = true;

                //把值塞進來 .....
                foreach (string dailyLifeRecommend in dailyLifeRecommendItems)
                {
                    DailyLifeRecommendItem item = _parent.TransferStudentData.DailyLifeRecommendData.GetItem(dailyLifeRecommend, sh.SchoolYear, sh.Semester);

                    if (item != null)
                        row.Cells[dailyLifeRecommend].Value = item.Description;
                }
            }

            _dailyLifeRecommendDegrees = TransferStudentDAL.GetDailyLifeRecommendMapping();

            // 公共服務表現畫面
            dgPublicService.Columns.Add(SCHOOL_YEAR_COLUMN, SCHOOL_YEAR_COLUMN);
            dgPublicService.Columns[SCHOOL_YEAR_COLUMN].Width = 75;
            dgPublicService.Columns.Add(SEMESTER_COLUMN, SEMESTER_COLUMN);
            dgPublicService.Columns[SEMESTER_COLUMN].Width = 65;

            List<string> PublicServiceServiceItems = TransferStudentDAL.GetPublicServicesItems();
            foreach (string dailyBehavior in TransferStudentDAL.GetPublicServicesItems())
            {
                dgPublicService.Columns.Add(dailyBehavior, dailyBehavior);
            }

            foreach (SemesterHistoryData sh in TransferStudentDAL.GetSemesterHistory(_studentID))
            {
                int rowIndex = dgPublicService.Rows.Add();

                DataGridViewRow row = dgPublicService.Rows[rowIndex];
                row.Tag = sh;

                DataGridViewCell cell = row.Cells[SCHOOL_YEAR_COLUMN];
                cell.Value = sh.SchoolYear;
                cell.ReadOnly = true;

                cell = row.Cells[SEMESTER_COLUMN];
                cell.Value = sh.Semester;
                cell.ReadOnly = true;

                foreach (string each in PublicServiceServiceItems)
                {
                    PublicServiceItem item = _parent.TransferStudentData.PublicServiceData.GetItem(each, sh.SchoolYear, sh.Semester);

                    if (item != null)
                        row.Cells[each].Value = item.Description;
                }      
                
            }

            //校內外特殊表現輸入畫面
            dgSchoolSpecial.Columns.Add(SCHOOL_YEAR_COLUMN, SCHOOL_YEAR_COLUMN);
            dgSchoolSpecial.Columns[SCHOOL_YEAR_COLUMN].Width = 75;
            dgSchoolSpecial.Columns.Add(SEMESTER_COLUMN, SEMESTER_COLUMN);
            dgSchoolSpecial.Columns[SEMESTER_COLUMN].Width = 65;

            List<string> schoolSpecialItems = TransferStudentDAL.GetSchoolSpecialItems();
            foreach (string each in schoolSpecialItems)
            {
                dgSchoolSpecial.Columns.Add(each, each);
            }

            foreach (SemesterHistoryData sh in TransferStudentDAL.GetSemesterHistory(_studentID))
            {
                int rowIndex = dgSchoolSpecial.Rows.Add();

                DataGridViewRow row = dgSchoolSpecial.Rows[rowIndex];
                row.Tag = sh;

                DataGridViewCell cell = row.Cells[SCHOOL_YEAR_COLUMN];
                cell.Value = sh.SchoolYear;
                cell.ReadOnly = true;

                cell = row.Cells[SEMESTER_COLUMN];
                cell.Value = sh.Semester;
                cell.ReadOnly = true;   
             
                foreach(string each in schoolSpecialItems)
                {
                    SchoolSpecialItem item = _parent.TransferStudentData.SchoolSpecialData.GetItem(each, sh.SchoolYear, sh.Semester);

                    if(item != null)
                        row.Cells[each].Value = item.Description;
                }                
            }          
        }

        #region IStep 成員

        public bool Valid()
        {            
            return true;
        }

        public void OnChangeStep()
        {
            foreach (DataGridViewRow row in dgDailyLifeRecommend.Rows)
            {                
                SemesterHistoryData sh = row.Tag as SemesterHistoryData;

                foreach (DataGridViewColumn column in dgDailyLifeRecommend.Columns)
                {
                    if (column.Name == SCHOOL_YEAR_COLUMN) continue;
                    if (column.Name == SEMESTER_COLUMN) continue;

                    string value = string.Empty;

                    if (row.Cells[column.Name].Value != null)
                        value = row.Cells[column.Name].Value.ToString();

                    DailyLifeRecommendItem item = new DailyLifeRecommendItem();
                    item.SchoolYear = sh.SchoolYear;
                    item.Semester = sh.Semester;
                    item.Name = column.Name;
                    item.Description = value;

                    _parent.TransferStudentData.DailyLifeRecommendData.SetItem(item);
                }
            }

            //將值塞入 _data 中
            foreach (DataGridViewRow row in dgPublicService.Rows)
            {                
                SemesterHistoryData sh = row.Tag as SemesterHistoryData;

                foreach (DataGridViewColumn column in dgPublicService.Columns)
                {
                    if (column.Name == SCHOOL_YEAR_COLUMN) continue;
                    if (column.Name == SEMESTER_COLUMN) continue;

                    string value = string.Empty;

                    if (row.Cells[column.Name].Value != null)
                        value = row.Cells[column.Name].Value.ToString();

                    PublicServiceItem item = new PublicServiceItem();
                    item.SchoolYear = sh.SchoolYear;
                    item.Semester = sh.Semester;                  
                    item.Name = column.Name;
                    item.Description = value;

                    _parent.TransferStudentData.PublicServiceData.SetItem(item);
                }
            }

            foreach (DataGridViewRow row in dgSchoolSpecial.Rows)
            {               
                SemesterHistoryData sh = row.Tag as SemesterHistoryData;

                foreach (DataGridViewColumn column in dgSchoolSpecial.Columns)
                {
                    if (column.Name == SCHOOL_YEAR_COLUMN) continue;
                    if (column.Name == SEMESTER_COLUMN) continue;

                    string value = string.Empty;

                    if (row.Cells[column.Name].Value != null)
                        value = row.Cells[column.Name].Value.ToString();

                    SchoolSpecialItem item = new SchoolSpecialItem();
                    item.SchoolYear = sh.SchoolYear;
                    item.Semester = sh.Semester;
                    item.Name = column.Name;
                    item.Description = value;

                    _parent.TransferStudentData.SchoolSpecialData.SetItem(item);
                }
            }
        }

        public string ErrorMessage { get; set; }
        #endregion

        private void dgDailyLifeRecommend_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            char sep = ',';

            DataGridViewCell cell = dgDailyLifeRecommend.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.Value == null) return;

            string[] split = cell.Value.ToString().Split(sep);
            StringBuilder sb = new StringBuilder();

            foreach (string s in split)
            {
                string str = s.Trim();

                if (_dailyLifeRecommendDegrees.ContainsKey(str))
                    str = _dailyLifeRecommendDegrees[s];
                sb.Append(str).Append(sep);
            }
            cell.Value = sb.ToString();
        }
    }
}
