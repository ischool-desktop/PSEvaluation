using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Aspose.Cells;
using JHSchool.Data;

namespace JHEvaluation.AssignmentScore
{
    public partial class CourseScoreStatusForm : FISCA.Presentation.Controls.BaseForm 
    {
        // 學年度
        private int _SchoolYear;
        // 學期
        private int _Semester;

        private bool IsSetupReloadEvent = false;        

        private BackgroundWorker _bgWk= new BackgroundWorker ();

        private CourseScoreStatusFormOption Option
        {
            get
            {
                CourseScoreStatusFormOption Option = new CourseScoreStatusFormOption();
                Option.SchoolYear = _SchoolYear;
                Option.Semester = _Semester;
                Option.DisplayNotFinish = chkDisplayNotFinish.Checked;
                Option.CheckText = chkText.Checked;
                return Option;
            }
        }

        public CourseScoreStatusForm()
        {
            InitializeComponent();

            string VersionMessage = "『";

            foreach (Assembly Assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.Equals("JHEvaluation.AssignmentScore")))
                VersionMessage += Assembly.GetName().Version;

            VersionMessage += "』";

            this.Text = "課程平時評量輸入狀況" + VersionMessage;


            // 初始化學年度學期
            if (int.TryParse(K12.Data.School.DefaultSchoolYear, out _SchoolYear))
                intSchoolYear.Value = _SchoolYear;

            if (int.TryParse(K12.Data.School.DefaultSemester, out _Semester))
                intSemester.Value = _Semester;
            
            _bgWk.DoWork += new DoWorkEventHandler(_bgWk_DoWork);
            _bgWk.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWk_RunWorkerCompleted);

            intSchoolYear.Enabled = false;
            intSemester.Enabled = false;
            chkDisplayNotFinish.Enabled = false;
            chkText.Enabled = false;

            _bgWk.RunWorkerAsync(Option);
        }

        void _bgWk_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<CourseScoreCountRecord> vCourseScoreCountEntityList = e.Result as List<CourseScoreCountRecord>;

            // 檢查畫面值，如果不同在執行一次
            if (_SchoolYear == intSchoolYear.Value && _Semester == intSemester.Value)
            {
                lblMsg.Visible = false;
                if(Option.CheckText)
                    listView.Columns[chText.Index].Text = "文字描述";
                else
                    listView.Columns[chText.Index].Text="";
                //2017/10/05，羿均，避免vCourseScoreCountEntityList = null 例外狀況發生。
                if (vCourseScoreCountEntityList != null)
                {
                    foreach (CourseScoreCountRecord csce in vCourseScoreCountEntityList)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.SubItems[0].Text = csce.CourseID;
                        lvi.SubItems.Add(csce.CourseName);
                        lvi.SubItems.Add(csce.TeacherName);
                        lvi.SubItems.Add(csce.OraHasScoreCount + "/" + csce.OraTotalScoreCount);
                        if (csce.OraHasScoreCount == csce.OraTotalScoreCount)
                            lvi.SubItems[3].ForeColor = Color.Black;
                        else
                            lvi.SubItems[3].ForeColor = Color.Red;

                        lvi.SubItems.Add(csce.OraHasEffortCount + "/" + csce.OraTotalEffortCount);
                        if (csce.OraHasEffortCount == csce.OraTotalEffortCount)
                            lvi.SubItems[4].ForeColor = Color.Black;
                        else
                            lvi.SubItems[4].ForeColor = Color.Red;

                        if (Option.CheckText)
                        {
                            // 文字描述
                            lvi.SubItems.Add(csce.OraHasTextCount + "/" + csce.OraTotalScoreCount);
                            if (csce.OraHasTextCount == csce.OraTotalTextCount)
                                lvi.SubItems[5].ForeColor = Color.Black;
                            else
                                lvi.SubItems[5].ForeColor = Color.Red;
                        }

                        listView.Items.Add(lvi);
                    }
                }
                intSchoolYear.Enabled = true;
                intSemester.Enabled = true;
                chkDisplayNotFinish.Enabled = true;
                chkText.Enabled = true;
                lblCourseCount.Text = "共" + listView.Items.Count + "筆";
            }
            else
            {
                _bgWk.RunWorkerAsync(Option);
            }

            if (!IsSetupReloadEvent)
            {
                intSchoolYear.ValueChanged += (vsender, ve) => ReloadListViewData();

                intSemester.ValueChanged += (vsender, ve) => ReloadListViewData();

                chkDisplayNotFinish.CheckedChanged += (vsender, ve) => ReloadListViewData();

                chkText.CheckedChanged += (vsender, ve) => ReloadListViewData();

                btnRefresh.Click += (vsender, ve) => ReloadListViewData();

                IsSetupReloadEvent = true;
            }
        }

        void _bgWk_DoWork(object sender, DoWorkEventArgs e)
        {
            CourseScoreStatusFormOption Option = e.Argument as CourseScoreStatusFormOption;

            List<CourseScoreCountRecord> CourseScoreCountEntityList = new List<CourseScoreCountRecord>();
            List<CourseScoreCountRecord> CourseScoreCountEntityListNonInput = new List<CourseScoreCountRecord>();
            List<CourseScoreCountRecord> CourseScoreCountEntityListNonInputScore = new List<CourseScoreCountRecord>();
         
            // 處理取得這所選學年學期有課程id,name,修課id
            DAO.QueryData.ProcesstCourseNameStudentBySchoolYearSemester(Option.SchoolYear, Option.Semester);
            Dictionary<string, string> CourseIDNameDict = DAO.QueryData._CourseIDNameDict;
            List<JHSCAttendRecord> SCAttendList = JHSCAttend.SelectByIDs(DAO.QueryData._SCAttenndIDList);
            Dictionary<string, List<JHSCAttendRecord>> CourseSCAttendDict = new Dictionary<string, List<JHSCAttendRecord>>();
            foreach (JHSCAttendRecord rec in SCAttendList)
            {
                if (!CourseSCAttendDict.ContainsKey(rec.RefCourseID))
                    CourseSCAttendDict.Add(rec.RefCourseID, new List<JHSCAttendRecord>());

                CourseSCAttendDict[rec.RefCourseID].Add(rec);
            }
            
            // 取得課程授課教師
            Dictionary<string, string> CourseTeacherNameDict = DAO.QueryData.GetCourseTeacher(Option.SchoolYear, Option.Semester);

            #region 計算人數並組成回傳格式
           foreach(string CourseID in CourseIDNameDict.Keys)
           {
                CourseScoreCountRecord vCourseScoreCountRecord = new CourseScoreCountRecord();

                vCourseScoreCountRecord.CourseID = CourseID;
                vCourseScoreCountRecord.CourseName = CourseIDNameDict[CourseID];
                if (CourseTeacherNameDict.ContainsKey(CourseID))
                    vCourseScoreCountRecord.TeacherName = CourseTeacherNameDict[CourseID];

                // 計算人數與有成績人數
               if(CourseSCAttendDict.ContainsKey(CourseID))
                foreach (JHSCAttendRecord rec in CourseSCAttendDict[CourseID])
                {
                        vCourseScoreCountRecord.OraTotalEffortCount++;
                        vCourseScoreCountRecord.OraTotalScoreCount++;
                        vCourseScoreCountRecord.OraTotalTextCount++;

                        if (rec.OrdinarilyScore.HasValue)
                            vCourseScoreCountRecord.OraHasScoreCount++;
                        if (rec.OrdinarilyEffort.HasValue)
                            vCourseScoreCountRecord.OraHasEffortCount++;

                    if(Option.CheckText)
                        if (!string.IsNullOrEmpty(rec.Text))
                            vCourseScoreCountRecord.OraHasTextCount++;                 
                }

               
                if (Option.DisplayNotFinish == true && Option.CheckText == true)
                {
                    // 當勾選未輸入與需要檢查文字描述
                    if (vCourseScoreCountRecord.NonAllInputScore())
                        CourseScoreCountEntityListNonInput.Add(vCourseScoreCountRecord);

                }
                else if (Option.DisplayNotFinish == true && Option.CheckText == false)
                {
                    // 當勾選未輸入，不檢查文字描述
                    if (vCourseScoreCountRecord.NonInputScore())
                        CourseScoreCountEntityListNonInputScore.Add(vCourseScoreCountRecord);
                }
                else
                {
                    CourseScoreCountEntityList.Add(vCourseScoreCountRecord);
                }               
                             
            }

           if (CourseScoreCountEntityListNonInput.Count > 0)
               e.Result = CourseScoreCountEntityListNonInput.OrderBy(x=>x.CourseName).ToList();

            if(CourseScoreCountEntityListNonInputScore.Count>0)
                e.Result = CourseScoreCountEntityListNonInputScore.OrderBy(x => x.CourseName).ToList();

            if(CourseScoreCountEntityList.Count>0)
                e.Result = CourseScoreCountEntityList.OrderBy(x => x.CourseName).ToList();

      
            #endregion
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
                
        // 課程名稱排序
        private int SortCourseScoreName(CourseScoreCountRecord x, CourseScoreCountRecord y)
        {
            return x.CourseName.CompareTo(y.CourseName);        
        }

        /// <summary>
        /// 匯出到 Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
 
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (listView.Items.Count <= 0) return;

            saveFileDialog1.FileName = string.Format("{0}學年度{1}學期課程平時評量成績輸入狀況", intSchoolYear.Value, (intSemester.Value == 1) ? "上" : "下");
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Workbook book = new Workbook();
                book.Worksheets.Clear();
                Worksheet ws = book.Worksheets[book.Worksheets.Add()];
                ws.Name = string.Format("{0}學年度 {1}學期", intSchoolYear.Value, (intSemester.Value == 1) ? "上" : "下");

                #region 加入 Header

                int row = 0;
                ws.Cells[row, chCourseName.Index-1].PutValue(chCourseName.Text);
                ws.Cells[row, chTeacher.Index-1].PutValue(chTeacher.Text);
                ws.Cells[row, chScore.Index-1].PutValue(chScore.Text);
                ws.Cells[row, chEffort.Index-1].PutValue(chEffort.Text);
                ws.Cells[row, chText.Index - 1].PutValue(chText.Text);


                #endregion

                listView.SuspendLayout();
                foreach (ListViewItem item in listView.Items)
                {
                    row++;                    
                    ws.Cells[row, chCourseName.Index-1].PutValue(item.SubItems[chCourseName.Index].Text);
                    ws.Cells[row, chTeacher.Index-1].PutValue(item.SubItems[chTeacher.Index].Text);
                    ws.Cells[row, chScore.Index-1].PutValue(item.SubItems[chScore.Index].Text);
                    ws.Cells[row, chEffort.Index-1].PutValue(item.SubItems[chEffort.Index].Text);
                    if(Option.CheckText)
                        ws.Cells[row, chText.Index - 1].PutValue(item.SubItems[chText.Index].Text);
                    
                }
                listView.ResumeLayout();

                ws.AutoFitColumns();

                try
                {
                    book.Save(saveFileDialog1.FileName+".xls", FileFormatType.Excel2003);
                    FISCA.Presentation.Controls.MsgBox.Show("匯出完成。");
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("匯出失敗。" + ex.Message);
                }
            }
        }

        private void CourseScoreStatusForm_Load(object sender, EventArgs e)
        {
            chkDisplayNotFinish.Checked = false;            
        }

        private void ReloadListViewData()
        {
            //2017/10/05，羿均，Reload後控制會項反紅，造成滑鼠移到控制項上就開始點擊的問題，所以註解掉。
            _SchoolYear = intSchoolYear.Value;
            _Semester = intSemester.Value;
            //intSchoolYear.Enabled = false;
            //intSemester.Enabled = false;
            chkDisplayNotFinish.Enabled = false;
            chkText.Enabled = false;

            lblCourseCount.Text = "";
            lblMsg.Visible = true;
            listView.Items.Clear();
            if (!_bgWk.IsBusy)
                _bgWk.RunWorkerAsync(Option);
        }

    }

    /// <summary>
    /// 使用者選項
    /// </summary>
    internal class CourseScoreStatusFormOption
    {
        /// <summary>
        /// 學年度
        /// </summary>
        public int SchoolYear { get; set; }
        /// <summary>
        /// 學期
        /// </summary>
        public int Semester { get; set; }
        /// <summary>
        /// 只顯示未輸入完成
        /// </summary>
        public bool DisplayNotFinish { get; set; }

        /// <summary>
        /// 是否檢查文字評量
        /// </summary>
        public bool CheckText { get; set; }
    }
}
