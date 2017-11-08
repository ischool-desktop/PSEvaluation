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

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.StuinCourse
{
    //public partial class StudinCourse : FISCA.Presentation.Controls.BaseForm
    //{
    //    /*private int _a;
        
    //    public int A
    //    {
    //        get { return _a; }
    //    }

    //    public int B;
    //    */
    //    public StudinCourse()
    //    {
    //        /*將畫面裝載進來*/
    //        InitializeComponent();

    //        /*新增畫面在課程中*/
    //        //MenuButton button = Course.Instance.RibbonBarItems["統計報表"]["報表"]["自訂報表"];
    //        //button.Click += delegate { Framework.MsgBox.Show("按!"); };
    //        //button.Click += new EventHandler(CourseButton_Click);


    //    }
    //    private int CourseComparer(CourseRecord a, CourseRecord b)
    //    {
    //        return a.Name.CompareTo(b.Name); 
    //    }

    //    void CourseButton_Click(object sender, EventArgs e)
    //    {

            
    //        /*出現狀態Bar的訊息*/
    //        SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage("處理中，請稍候...", 0);

    //        Workbook book = new Workbook();
    //        book.Worksheets.Clear();
    //        book.Open(new MemoryStream(Resources.課程修課學生清單1));
    //        /*新增一個List接所選取的課程*/

    //        List<CourseRecord> _CourseList = new List<CourseRecord>();
            
    //        _CourseList = Course.Instance.SelectedList;
            
    //        //_CourseList.Sort(CourseComparer);

    //        _CourseList.Sort(delegate(CourseRecord a, CourseRecord b)
    //       {
    //           return a.Name.CompareTo(b.Name);
    //        });

    //       // _CourseList.Sort();
    //        Range _Range1 = book.Worksheets["Temp"].Cells.CreateRange(0, 4, false);
    //        Range _Range2 = book.Worksheets["Temp"].Cells.CreateRange(4, 1, false);
    //        Range _Range3 = book.Worksheets["Temp"].Cells.CreateRange(5, 1, false);


    //        int seq = 0;
    //        int Addpage = 0;
            
    //        /*將所選取的班級，資料取出*/
    //        foreach (CourseRecord cr in _CourseList)
    //        {

    //            book.Worksheets["Sheet1"].Cells.CreateRange(seq, 4, false).Copy(_Range1);
    //            //課程的標題檔
    //            book.Worksheets[0].Cells[seq, 0].PutValue(cr.SchoolYear + "學年度第" + cr.Semester + "學期 課程學生修課清單");
                
    //            seq++;
    //            //取得第一位的授課教師

    //            if (cr.GetFirstTeacher() !=null)
    //            {

    //                book.Worksheets[0].Cells[seq, 6].PutValue(cr.GetFirstTeacher().Name);
    //            }
    //            else {
    //                book.Worksheets[0].Cells[seq, 6].PutValue("");
    //            }
                
    //            //取得科目名稱
    //            book.Worksheets[0].Cells[seq, 1].PutValue(cr.Name);
    //            //取得節次
    //            book.Worksheets[0].Cells[seq, 4].PutValue(cr.Period);
                
    //            seq++;

    //            //取得科目
    //            book.Worksheets[0].Cells[seq, 1].PutValue(cr.Subject);
    //            //取得班級人數
    //            book.Worksheets[0].Cells[seq, 6].PutValue(cr.Class.Students.Count);

    //            /*取得課程的修課學生*/

                
                
    //            seq += 2;

    //            List<StudentRecord> scr = cr.GetAttendStudents();
               
    //            foreach (StudentRecord stdrec in scr)
    //            {

    //                book.Worksheets["Sheet1"].Cells.CreateRange(seq, 1, false).Copy(_Range2);
    //                //取得所屬班級 
    //                if (stdrec.Class != null)
    //                {
    //                    book.Worksheets[0].Cells[seq, 0].PutValue(stdrec.Class.Name);
    //                }
    //                else {
    //                    book.Worksheets[0].Cells[seq, 0].PutValue("");
    //                }
    //                //取得座號
    //                book.Worksheets[0].Cells[seq, 1].PutValue(stdrec.SeatNo);
    //                //取得學號
    //                book.Worksheets[0].Cells[seq, 2].PutValue(stdrec.StudentNumber);
    //                //取得姓名
    //                book.Worksheets[0].Cells[seq, 3].PutValue(stdrec.Name);
    //                //取得性別
    //                book.Worksheets[0].Cells[seq, 5].PutValue(stdrec.Gender);
                   
    //                seq++;
    //            }
                
    //            seq++;

    //            Addpage++;

    //            book.Worksheets["Sheet1"].Cells.CreateRange(seq, 1, false).Copy(_Range3);
    //            book.Worksheets[0].Cells[seq, 6].PutValue("第"+ Addpage +"頁/共"+_CourseList.Count +"頁");
                
    //            seq++;

    //            book.Worksheets[0].HPageBreaks.Add(seq, 0);
    //            //if (_CourseList.Count!= 0){
                                    
    //            //}
    //            //v
    //            book.Worksheets["Temp"].IsVisible = false;
                



    //        }
    //        #region
    //        try
    //        {

    //            SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage("請選擇儲存位置", 100);
    //            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();

    //            SaveFileDialog1.Filter = "Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
    //            SaveFileDialog1.FileName = "課程修課清單";

    //            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
    //            {
    //                book.Save(SaveFileDialog1.FileName);
    //                Process.Start(SaveFileDialog1.FileName);
    //            }
    //            else
    //            {
    //                MessageBox.Show("檔案未儲存");

    //            }
    //        }
    //        catch
    //        {
    //            MessageBox.Show("檔案儲存錯誤,請檢查檔案是否開啟中!!");
    //        }

    //        SmartSchool.Customization.PlugIn.Global.SetStatusBarMessage("已完成");

    //        #endregion

    //    }

      

    //}
}
