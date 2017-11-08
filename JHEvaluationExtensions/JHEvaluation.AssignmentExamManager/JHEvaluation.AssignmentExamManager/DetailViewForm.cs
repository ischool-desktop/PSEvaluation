using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JHEvaluation.AssignmentExamManager
{
    internal partial class DetailViewForm : BaseForm
    {
        private AssignmentExamRow Row { get; set; }
        private string mCourseName;
        private string mSchoolYear;
        private string mSemester;

        public DetailViewForm(string CourseID)
        {
            InitializeComponent();

            List<AssignmentExamSCAttendRecord> records = AssignmentExamSCAttend.SelectByCourseID(CourseID);

            AssignmentExamCourseRecord course = AssignmentExamCourse.SelectByID(CourseID);

            mCourseName = course.Name;
            mSchoolYear = ""+course.SchoolYear;
            mSemester = "" + course.Semester;

            Dictionary<string, string> Scores = new Dictionary<string, string>();

            foreach (AssignmentExamSetupRecord setuprecord in course.ExamSetups)
            {
                if (!Scores.ContainsKey(setuprecord.SubExamID))
                {
                    Scores.Add(setuprecord.SubExamID, "");

                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.FillWeight = 33;
                    column.HeaderText = setuprecord.Name;
                    column.Name = setuprecord.SubExamID;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvDetail.Columns.Add(column);
                }
            }

            if (course != null)
            {
                foreach (AssignmentExamSCAttendRecord record in records)
                {
                    Dictionary<string, string> CurSocres = new Dictionary<string, string>();

                    foreach (string Key in Scores.Keys)
                        if (!CurSocres.ContainsKey(Key))
                        CurSocres.Add(Key, Scores[Key]);

                    foreach (AssignmentExamRecord aerecord in record.AssignmentExams)
                        if (CurSocres.ContainsKey(aerecord.SubExamID))
                            CurSocres[aerecord.SubExamID] = K12.Data.Decimal.GetString(aerecord.Score);

                    List<object> values = new List<object>();

                    values.Add(record.Student.Class.Name);
                    values.Add(record.Student.SeatNo);
                    values.Add(record.Student.StudentNumber);
                    values.Add(record.Student.Name);

                    foreach (string value in CurSocres.Values)
                        values.Add(value);

                    int RowIndex = dgvDetail.Rows.Add(values.ToArray());

                    //int RowIndex = dgvDetail.Rows.Add(record.Student.Class.Name , record.Student.SeatNo , record.Student.StudentNumber, record.Student.Name);
                   
                }
            }

            #region Sample
            //DataGridViewTextBoxColumn c1 = new DataGridViewTextBoxColumn();
            //c1.FillWeight = 33;
            //c1.HeaderText = "小考1";
            //c1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //DataGridViewTextBoxColumn c2 = new DataGridViewTextBoxColumn();
            //c2.FillWeight = 33;
            //c2.HeaderText = "小考2";
            //c2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //DataGridViewTextBoxColumn c3 = new DataGridViewTextBoxColumn();
            //c3.FillWeight = 34;
            //c3.HeaderText = "小考3";
            //c3.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //dgvDetail.Columns.Add(c1);
            //dgvDetail.Columns.Add(c2);
            //dgvDetail.Columns.Add(c3);

            //DataGridViewRow r1 = new DataGridViewRow();
            //r1.CreateCells(dgvDetail, "301", "1", "9602001", "張君雅", "98", "80", "95");
            //DataGridViewRow r2 = new DataGridViewRow();
            //r2.CreateCells(dgvDetail, "301", "2", "9602002", "林測試", "40", "52", "");
            //DataGridViewRow r3 = new DataGridViewRow();
            //r3.CreateCells(dgvDetail, "301", "3", "9602003", "許某人", "74", "", "");
            //DataGridViewRow r4 = new DataGridViewRow();
            //r4.CreateCells(dgvDetail, "301", "4", "9602004", "尤比爾", "78", "76", "81");
            //DataGridViewRow r5 = new DataGridViewRow();
            //r5.CreateCells(dgvDetail, "301", "5", "9602005", "王同學", "61", "69", "");

            //dgvDetail.Rows.Add(r1);
            //dgvDetail.Rows.Add(r2);
            //dgvDetail.Rows.Add(r3);
            //dgvDetail.Rows.Add(r4);
            //dgvDetail.Rows.Add(r5);
            #endregion
            //if (row == null) return;
            //Row = row;
            //Text += " - " + row.CourseName;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ExportUtil.Export(dgvDetail, string.Format("{0}-{1} {2}小考輸入細項", mSchoolYear , mSemester , mCourseName));
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
