//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.DAL;
//using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.Absence;
//using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.MeritDetail;

//namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent
//{
//    public partial class Step4 : UserControl, IStep
//    {
//        private const string SCHOOLYEAR_COLUMN = "學年度";
//        private const string SEMESTER_COLUMN = "學期";
//        private const string INPUT_DATE_COLUMN = "補登日期";
//        private const string MERIT_DATE_COLUMN = "日期";
//        private const string MERIT_TYPE_COLUMN = "獎懲別";
//        private const string COUNT_COLUMN = "次數";
//        private const string REASON_COLUMN = "事由";

//        private string _studentID;
//        private List<string> _meritTypes;
//        private TransferStudentForm _parent;

//        public Step4(TransferStudentForm parent)
//        {
//            InitializeComponent();

//            _parent = parent;

//            if (Student.Instance.SelectedList.Count == 0) return;
//            _studentID = Student.Instance.SelectedList[0].ID;
                      
//            dgMeritStatistic.Columns.Add(SCHOOLYEAR_COLUMN, SCHOOLYEAR_COLUMN);
//            dgMeritStatistic.Columns.Add(SEMESTER_COLUMN, SEMESTER_COLUMN);
//            dgMeritStatistic.Columns.Add(INPUT_DATE_COLUMN, INPUT_DATE_COLUMN);

//            _meritTypes = TransferStudentDAL.GetMeritTypes();

//            foreach (string merit in _meritTypes)
//            {
//                int columnIndex = dgMeritStatistic.Columns.Add(merit, merit);
//                dgMeritStatistic.Columns[columnIndex].Width = 60;
//            }
                        
//             塞入獎懲統計之預設值
//            foreach (MeritStatisticItem item in _parent.TransferStudentData.MeritStatisticData.Items)
//            {
//                int rowIndex = dgMeritStatistic.Rows.Add();
//                DataGridViewRow row = dgMeritStatistic.Rows[rowIndex];

//                row.Cells[SCHOOLYEAR_COLUMN].Value = item.SchoolYear;
//                row.Cells[SEMESTER_COLUMN].Value = item.Semester;
//                row.Cells[INPUT_DATE_COLUMN].Value = item.InputDate;
           
//                foreach (string mt in _meritTypes)
//                {
//                    if (item.MeritMapping.ContainsKey(mt))
//                        row.Cells[mt].Value = item.MeritMapping[mt].ToString();
//                }
//            }

//            獎懲明細處理
//            dgMeritDetail.Columns.Add(MERIT_DATE_COLUMN, MERIT_DATE_COLUMN);

//            加入下拉方塊欄位
//            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
//            comboBoxColumn.Items.AddRange(_meritTypes.ToArray());
//            comboBoxColumn.ValueType = typeof(string);
//            comboBoxColumn.HeaderText = MERIT_TYPE_COLUMN;
//            comboBoxColumn.Name = MERIT_TYPE_COLUMN;
//            comboBoxColumn.Width = 60;
//            dgMeritDetail.Columns.Add(comboBoxColumn);

//            dgMeritDetail.Columns.Add(COUNT_COLUMN, COUNT_COLUMN);
//            dgMeritDetail.Columns[COUNT_COLUMN].Width = 60;
//            dgMeritDetail.Columns.Add(REASON_COLUMN, REASON_COLUMN);
//            dgMeritDetail.Columns[REASON_COLUMN].Width = 200;

//            foreach (MeritDetailItem item in _parent.TransferStudentData.MeritDetailData.Items)
//            {
//                int rowIndex = dgMeritDetail.Rows.Add();
//                DataGridViewRow row = dgMeritDetail.Rows[rowIndex];

//                row.Cells[MERIT_DATE_COLUMN].Value = item.OccurDate.ToShortDateString();
//                row.Cells[MERIT_TYPE_COLUMN].Value = item.MeritType;
//                row.Cells[COUNT_COLUMN].Value = item.Count;
//                row.Cells[REASON_COLUMN].Value = item.Reason;
//            }
//        }

//        #region IStep 成員

//        public bool Valid()
//        {
//            return true;
//        }

//        public string ErrorMessage { get; set; }

//        public void OnChangeStep()
//        {
//            _parent.TransferStudentData.MeritStatisticData.Items.Clear();
//            foreach (DataGridViewRow row in dgMeritStatistic.Rows)
//            {
//                先把所有空欄位當空值處理
//                foreach (DataGridViewCell cell in row.Cells)
//                {
//                    if (cell.Value == null)
//                        cell.Value = string.Empty;
//                }

//                MeritStatisticItem item = new MeritStatisticItem();

//                item.SchoolYear = row.Cells[SCHOOLYEAR_COLUMN].Value.ToString();
//                item.Semester = row.Cells[SEMESTER_COLUMN].Value.ToString();
//                item.InputDate = DateTime.Parse(row.Cells[INPUT_DATE_COLUMN].Value.ToString());
            
//                foreach (string mt in _meritTypes)
//                {
//                    int count = 0;
//                    string value = row.Cells[mt].Value.ToString();

//                    if (!int.TryParse(value, out count))
//                        count = 0;

//                    item.MeritMapping.Add(mt, count);
//                }

//                _parent.TransferStudentData.MeritStatisticData.Items.Add(item);
//            }

//            _parent.TransferStudentData.MeritDetailData.Items.Clear();
//            foreach (DataGridViewRow row in dgMeritDetail.Rows)
//            {
//                先把所有空欄位當空值處理
//                foreach (DataGridViewCell cell in row.Cells)
//                {
//                    if (cell.Value == null)
//                        cell.Value = string.Empty;
//                }

//                MeritDetailItem item = new MeritDetailItem();

//                item.OccurDate = DateTime.Parse(row.Cells[MERIT_DATE_COLUMN].Value.ToString());

//                string value = row.Cells[COUNT_COLUMN].Value.ToString();
//                int count;
//                if (!int.TryParse(value, out count))
//                    count = 0;
//                item.Count = count;
//                item.MeritType = row.Cells[MERIT_TYPE_COLUMN].Value.ToString();
//                item.Reason = row.Cells[REASON_COLUMN].Value.ToString();

//                _parent.TransferStudentData.MeritDetailData.Items.Add(item);
//            }
//        }

//        #endregion
//    }
//}
