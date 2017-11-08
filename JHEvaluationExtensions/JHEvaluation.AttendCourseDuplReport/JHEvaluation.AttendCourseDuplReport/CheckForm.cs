using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Aspose.Cells;
using JHSchool.Data;
using System.IO;

namespace JHEvaluation.AttendCourseDuplReport
{
    internal partial class CheckForm : BaseForm
    {
        private StudentAttendInfo Info { get; set; }
        private int SchoolYear { get; set; }
        private int Semester { get; set; }

        private List<string> StudentIDs { get; set; }
        private List<string> CourseIDs { get; set; }

        private BackgroundWorker Worker { get; set; }

        internal CheckForm(StudentAttendInfo info, int schoolYear, int semester)
        {
            InitializeComponent();

            Info = info;
            SchoolYear = schoolYear;
            Semester = semester;

            SetDisplayCount();
        }

        private void SetDisplayCount()
        {
            StudentIDs = Info.GetDuplicateStudents();
            CourseIDs = Info.GetDuplicateCourses();

            if (StudentIDs.Count > 0)
            {
                lblStudentCount.ForeColor = Color.Red;
                lblStudentCount.Text = "" + StudentIDs.Count;
            }
            else
                btnStudentTemporary.Enabled = false;

            if (CourseIDs.Count > 0)
            {
                lblCourseCount.ForeColor = Color.Red;
                lblCourseCount.Text = "" + CourseIDs.Count;
            }
            else
                btnCourseTemporary.Enabled = false;

            btnExport.Enabled = (StudentIDs.Count > 0 || CourseIDs.Count > 0);
        }

        private void btnStudentTemporary_Click(object sender, EventArgs e)
        {
            K12.Presentation.NLDPanels.Student.AddToTemp(StudentIDs);
            btnStudentTemporary.Enabled = false;
        }

        private void btnCourseTemporary_Click(object sender, EventArgs e)
        {
            K12.Presentation.NLDPanels.Course.AddToTemp(CourseIDs);
            btnCourseTemporary.Enabled = false;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Worker = new BackgroundWorker();
            Worker.WorkerReportsProgress = true;
            Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            Worker.RunWorkerCompleted += delegate(object sender1, RunWorkerCompletedEventArgs e1)
            {
                if (e1.Error != null)
                {
                    MsgBox.Show("匯出失敗。" + e1.Error.Message);
                    return;
                }

                FISCA.Presentation.MotherForm.SetStatusBarMessage("匯出完成");

                Workbook book = e1.Result as Workbook;

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel(*.xls)|*.xls";
                sfd.FileName = "學生學期修課檢查表";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        book.Save(sfd.FileName, FileFormatType.Excel2003);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show(ex.Message);
                        return;
                    }

                    try
                    {
                        if (MsgBox.Show("是否立即開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            System.Diagnostics.Process.Start(sfd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show(ex.Message);
                    }
                }
            };
            Worker.ProgressChanged += delegate(object sender2, ProgressChangedEventArgs e2)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("匯出學生學期修課檢查表...", e2.ProgressPercentage);
            };

            //執行
            Worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<ExcelRow> rows = new List<ExcelRow>();

            #region 整理成一行一行
            foreach (string studentID in Info.DuplicateAttendInfo.Keys)
            {
                JHStudentRecord student = JHStudent.SelectByID(studentID);
                foreach (string subject in Info.DuplicateAttendInfo[studentID].Keys)
                {
                    ExcelRow row = new ExcelRow(student);
                    row.SetSubject(subject);
                    row.SetCourceIDs(Info.DuplicateAttendInfo[studentID][subject]);

                    rows.Add(row);
                }
            }
            #endregion

            rows.Sort(delegate(ExcelRow x, ExcelRow y)
            {
                if (x.ClassName == y.ClassName)
                {
                    int seatNoX, seatNoY;
                    if (!int.TryParse(x.SeatNo, out seatNoX)) seatNoX = int.MaxValue;
                    if (!int.TryParse(y.SeatNo, out seatNoY)) seatNoY = int.MaxValue;

                    if (seatNoX == seatNoY)
                        return JHSchool.Evaluation.Subject.CompareSubjectOrdinal(x.Subject, y.Subject);
                    else
                        return seatNoX.CompareTo(seatNoY);
                }
                else
                    return x.ClassName.CompareTo(y.ClassName);
            });

            Workbook template = new Workbook();
            template.Open(new MemoryStream(Properties.Resources.重覆修課學生清單));
            Range tempRow = template.Worksheets[0].Cells.CreateRange(2, 1, false);

            Workbook book = new Workbook();
            book.Open(new MemoryStream(Properties.Resources.重覆修課學生清單));

            Worksheet ws = book.Worksheets[0];

            #region 填入 Excel
            double total = rows.Count;
            double count = 0;

            ws.Cells[0, 0].PutValue(string.Format("{0} 學年度 第 {1} 學期 學生學期修課檢查表", SchoolYear, Semester));

            int rowIndex = 2;
            foreach (ExcelRow row in rows)
            {
                count++;

                int colIndex = 0;
                ws.Cells.CreateRange(rowIndex, 1, false).Copy(tempRow);

                ws.Cells[rowIndex, colIndex++].PutValue(row.ClassName);
                ws.Cells[rowIndex, colIndex++].PutValue(row.SeatNo);
                ws.Cells[rowIndex, colIndex++].PutValue(row.StudentNumber);
                ws.Cells[rowIndex, colIndex++].PutValue(row.StudentName);
                ws.Cells[rowIndex, colIndex++].PutValue(row.Subject);
                ws.Cells[rowIndex, colIndex++].PutValue(row.CourseNames);

                rowIndex++;

                Worker.ReportProgress((int)(count * 100 / total));
            }
            #endregion

            e.Result = book;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private class ExcelRow
        {
            public string ClassName { get; private set; }
            public string SeatNo { get; private set; }
            public string StudentNumber { get; private set; }
            public string StudentName { get; private set; }
            public string Subject { get; private set; }
            public string CourseNames
            {
                get
                {
                    StringBuilder builder = new StringBuilder("");
                    _courseIDs.Sort();
                    foreach (string id in _courseIDs)
                    {
                        JHCourseRecord course = JHCourse.SelectByID(id);
                        builder.Append(course.Name + "、");
                    }
                    string result = builder.ToString();
                    if (result.EndsWith("、"))
                        result = result.Substring(0, result.Length - 1);
                    return result;
                }
            }

            private List<string> _courseIDs = new List<string>();

            public void SetSubject(string subject)
            {
                Subject = subject;
            }

            public void SetCourceIDs(List<string> courseIDs)
            {
                _courseIDs = courseIDs;
            }

            public ExcelRow(JHStudentRecord student)
            {
                ClassName = (student.Class != null) ? student.Class.Name : "";
                SeatNo = "" + student.SeatNo;
                StudentNumber = student.StudentNumber;
                StudentName = student.Name;
            }
        }
    }
}
