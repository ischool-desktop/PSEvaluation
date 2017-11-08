using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using Aspose.Cells;
using System.IO;
using System.Diagnostics;
using JHSchool.Data;


namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.StuinCourse
{
    public partial class StuinCourse
    {

        public StuinCourse()
        {


            /*出現狀態Bar的訊息*/
            SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage("處理中，請稍候...", 0);

            Workbook book = new Workbook();
            book.Worksheets.Clear();
            book.Open(new MemoryStream(Resources.課程修課學生清單1));
            /*新增一個List接所選取的課程*/

            List<CourseRecord> _CourseList = new List<CourseRecord>();

            _CourseList = Course.Instance.SelectedList;
            _CourseList.Sort();
            Range _Range1 = book.Worksheets["Temp"].Cells.CreateRange(0, 4, false);
            Range _Range2 = book.Worksheets["Temp"].Cells.CreateRange(4, 1, false);
            Range _Range3 = book.Worksheets["Temp"].Cells.CreateRange(5, 1, false);


            int seq = 0;
            int Addpage = 0;

            /*將所選取的班級，資料取出*/
            foreach (CourseRecord cr in _CourseList)
            {

                book.Worksheets["Sheet1"].Cells.CreateRange(seq, 4, false).Copy(_Range1);
                //課程的標題檔
                book.Worksheets[0].Cells[seq, 0].PutValue(cr.SchoolYear + "學年度第" + cr.Semester + "學期 課程學生修課清單");

                seq++;
                //取得第一位的授課教師

                if (cr.GetFirstTeacher() != null)
                {

                    book.Worksheets[0].Cells[seq, 6].PutValue(cr.GetFirstTeacher().Name);
                }

                if (cr.GetSecondTeacher() != null)
                {


                    book.Worksheets[0].Cells[seq, 6].PutValue(book.Worksheets[0].Cells[seq, 6].StringValue + "," + cr.GetSecondTeacher().Name);
                }
                if (cr.GetThirdTeacher() != null)
                {

                    book.Worksheets[0].Cells[seq, 6].PutValue(book.Worksheets[0].Cells[seq, 6].StringValue + "," + cr.GetThirdTeacher().Name);

                }

                //欄位重新定位

                book.Worksheets[0].AutoFitColumn(6, seq, seq);
                //取得課程名稱
                book.Worksheets[0].Cells[seq, 1].PutValue(cr.Name);
                //取得節次
                book.Worksheets[0].Cells[seq, 4].PutValue(cr.Period);
                seq++;
                //取得科目
                book.Worksheets[0].Cells[seq, 1].PutValue(cr.Subject);
                //判斷並取得修課人數

                IEnumerable<JHSchool.Data.JHSCAttendRecord> scr = JHSchool.Data.JHSCAttend.SelectByCourseIDs(new string[] { cr.ID });
                IEnumerable<string> studentids = from screc in scr select screc.RefStudentID;
                // 取得一般生
                List<JHSchool.Data.JHStudentRecord> students = JHStudent.SelectByIDs(studentids).Where(x => x.Status == K12.Data.StudentRecord.StudentStatus.一般).ToList();

                students.Sort(ParseStudent);


                if (cr.Class == null)
                {
                    book.Worksheets[0].Cells[seq, 6].PutValue(null);
                }
                else
                {
                    //book.Worksheets[0].Cells[seq, 6].PutValue(cr.Class.Students.Count);
                    // 修課學生人數
                    book.Worksheets[0].Cells[seq, 6].PutValue(students.Count);
                }
                /*取得課程的修課學生*/
                seq += 2;

                //List<StudentRecord> scr = cr.GetAttendStudents();


                foreach (JHStudentRecord stdrec in students)
                {

                    book.Worksheets["Sheet1"].Cells.CreateRange(seq, 1, false).Copy(_Range2);
                    //取得所屬班級 
                    if (stdrec.Class != null)
                    {
                        book.Worksheets[0].Cells[seq, 0].PutValue(stdrec.Class.Name);
                    }
                    else
                    {
                        book.Worksheets[0].Cells[seq, 0].PutValue("");
                    }
                    //取得座號
                    book.Worksheets[0].Cells[seq, 1].PutValue(stdrec.SeatNo);
                    //取得學號
                    book.Worksheets[0].Cells[seq, 2].PutValue(stdrec.StudentNumber);
                    //取得姓名
                    book.Worksheets[0].Cells[seq, 3].PutValue(stdrec.Name);
                    //取得性別
                    book.Worksheets[0].Cells[seq, 5].PutValue(stdrec.Gender);

                    seq++;
                }

                seq++;

                Addpage++;

                book.Worksheets["Sheet1"].Cells.CreateRange(seq, 1, false).Copy(_Range3);
                book.Worksheets[0].Cells[seq, 6].PutValue("第" + Addpage + "頁/共" + _CourseList.Count + "頁");

                seq++;

                book.Worksheets[0].HPageBreaks.Add(seq, 0);

                book.Worksheets["Temp"].IsVisible = false;
            }
            #region
            try
            {

                SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage("請選擇儲存位置", 100);
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();

                SaveFileDialog1.Filter = "Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                SaveFileDialog1.FileName = "課程修課清單";

                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    book.Save(SaveFileDialog1.FileName);
                    Process.Start(SaveFileDialog1.FileName);
                }
                else
                {
                    MessageBox.Show("檔案未儲存");

                }
            }
            catch
            {
                MessageBox.Show("檔案儲存錯誤,請檢查檔案是否開啟中!!");
            }

            SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage("已完成");

            #endregion

        }

        //排序功能
        private int ParseStudent(JHStudentRecord x, JHStudentRecord y)
        {
            //取得班級名稱
            string Xstring = x.Class != null ? x.Class.Name : "";
            string Ystring = y.Class != null ? y.Class.Name : "";

            //取得座號
            string Xint = x.SeatNo.HasValue ? x.SeatNo.ToString() : "";
            string Yint = y.SeatNo.HasValue ? y.SeatNo.ToString() : "";
            //班級名稱加:號加座號(靠右對齊補0)
            Xstring += ":" + Xint.PadLeft(2, '0');
            Ystring += ":" + Yint.PadLeft(2, '0');

            return Xstring.CompareTo(Ystring);

        }



    }
}
