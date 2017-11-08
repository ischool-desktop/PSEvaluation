using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Linq;
using Aspose.Cells;
using JHSchool;
using JHSchool.Data;
using JHSchool.Evaluation;
using KaoHsiung.JHEvaluation.Data;

namespace KaoHsiung.JHEvaluation.EduAdminExtendControls.Ribbon
{

    public partial class CourseScoreStatusForm : FISCA.Presentation.Controls.BaseForm
    {

        private List<CourseListViewItem> _courseListViewItemList;
        private Dictionary<string, KH.JHAEIncludeRecord> _aeCache;
        private static string Order = "asc";
       
        public class ListViewItemComparer : IComparer
        {
            private int col;

            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                if (Order.Equals("asc"))
                    return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                else
                    return String.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
            }
        }


        public CourseScoreStatusForm()
        {
            InitializeComponent();

            listView.Sorting = System.Windows.Forms.SortOrder.None;
            listView.ColumnClick += (sender, e) =>
            {
                this.listView.ListViewItemSorter = new ListViewItemComparer(e.Column);
                Order = Order.Equals("asc") ? "des" : "asc";
            };

            InitialSemester();
            _aeCache = new Dictionary<string, KH.JHAEIncludeRecord>();
        }

        /// <summary>
        /// 初始化學年度學期的值。
        /// </summary>
        private void InitialSemester()
        {
            int i;
            intSchoolYear.Value = int.TryParse(K12.Data.School.DefaultSchoolYear, out i) ? i : 97;
            intSemester.Value = int.TryParse(K12.Data.School.DefaultSemester, out i) ? i : 1;
        }

        /// <summary>
        /// FormLoad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CourseScoreStatusForm_Load(object sender, EventArgs e)
        {
            _courseListViewItemList = new List<CourseListViewItem>();
            SyncCacheManager();

            #region 取得所有試別
            Exam.Instance.SyncAllBackground();
            List<ExamRecord> list = new List<ExamRecord>(Exam.Instance.Items);
            list.Sort(delegate(ExamRecord a, ExamRecord b)
            {
                return a.DisplayOrder.CompareTo(b.DisplayOrder);
            });

            cboExam.Items.Clear();
            KeyValuePair<string, string> nullExam = new KeyValuePair<string, string>(string.Empty, string.Empty);
            cboExam.Items.Add(nullExam);
            foreach (var item in list)
            {
                KeyValuePair<string, string> pair = new KeyValuePair<string, string>(item.ID, item.Name);
                cboExam.Items.Add(pair);
            }

            cboExam.DisplayMember = "Value";
            cboExam.ValueMember = "Key";

            if (cboExam.Items.Count > 0)
                cboExam.SelectedIndex = 0;
            #endregion
        }

        /// <summary>
        /// 當學年度改變時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void intSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            RefreshListView();
        }

        /// <summary>
        /// 當學期改變時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void intSemester_ValueChanged(object sender, EventArgs e)
        {
            RefreshListView();
        }

        /// <summary>
        /// 當試別改變時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshListView();
        }

        /// <summary>
        /// 更新ListView
        /// </summary>
        private void RefreshListView()
        {
            if (cboExam.SelectedItem == null) return;
            string exam_id = ((KeyValuePair<string, string>)cboExam.SelectedItem).Key;
            if (string.IsNullOrEmpty(exam_id)) return;

            listView.Items.Clear();

            LoadCourses(exam_id);
            SortItemList();
            FillCourses(GetDisplayList());
        }

        /// <summary>
        /// 如果CacheManager沒有還沒載入資料，那就載入吧
        /// </summary>
        private void SyncCacheManager()
        {
            if (!Course.Instance.Loaded) Course.Instance.SyncAllBackground();
            if (!AssessmentSetup.Instance.Loaded) AssessmentSetup.Instance.SyncAllBackground();
            if (!TCInstruct.Instance.Loaded) TCInstruct.Instance.SyncAllBackground();
            //if (!SCAttend.Instance.Loaded) SCAttend.Instance.SyncAllBackground();
            if (!AEInclude.Instance.Loaded) AEInclude.Instance.SyncAllBackground();
            _aeCache.Clear();
            foreach (JHAEIncludeRecord ae in JHAEInclude.SelectAll())
            {
                if (!_aeCache.ContainsKey(ae.ID))
                    _aeCache.Add(ae.ID, new KH.JHAEIncludeRecord(ae));
            }
        }

        /// <summary>
        /// 依試別取得所有關聯課程
        /// </summary>
        /// <param name="exam_id"></param>
        private void LoadCourses(string exam_id)
        {
            Dictionary<CourseRecord, KH.JHAEIncludeRecord> courseAEDict = new Dictionary<CourseRecord, KH.JHAEIncludeRecord>();

            List<string> CourseIDs = new List<string>();
            foreach (var course in Course.Instance.Items)
            {
                if (course.SchoolYear != intSchoolYear.Value) continue;
                if (course.Semester != intSemester.Value) continue;

                CourseIDs.Add(course.ID);
                AssessmentSetupRecord asRecord = course.GetAssessmentSetup();
                if (asRecord == null) continue;

                foreach (KH.JHAEIncludeRecord ae in _aeCache.Values)
                {
                    if (ae.RefAssessmentSetupID == asRecord.ID &&
                        ae.RefExamID == exam_id)
                    {
                        if (!courseAEDict.ContainsKey(course))
                            courseAEDict.Add(course, ae);
                    }
                }

                //foreach (var ae in asRecord.GetAEIncludes())
                //{
                //    if (ae.RefExamID == exam_id)
                //    {
                //        KH.JHAEIncludeRecord ae2 = _aeCache[ae.ID];
                //        if (!courseAEDict.ContainsKey(course))
                //            courseAEDict.Add(course, ae2);
                //    }
                //}
            }

            Dictionary<string, List<KH.JHSCETakeRecord>> courseScoreDict = GetCourseScores(exam_id,CourseIDs);

            _courseListViewItemList.Clear();
            foreach (CourseRecord course in courseAEDict.Keys)
            {
                List<KH.JHSCETakeRecord> sceList = null;
                if (courseScoreDict.ContainsKey(course.ID))
                    sceList = courseScoreDict[course.ID];
                else
                    sceList = new List<KH.JHSCETakeRecord>();

                CourseListViewItem item = new CourseListViewItem(course, courseAEDict[course], sceList);
                _courseListViewItemList.Add(item);
            }
        }

        /// <summary>
        /// 取得某試別的所有成績記錄
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<KH.JHSCETakeRecord>> GetCourseScores(string exam_id,List<string> CourseIDs)
        {
            #region 過濾非在校生

            //先把成績依 StudentID 區分，為了過濾掉非在校生。
            Dictionary<string, List<KH.JHSCETakeRecord>> studentScoreDict = new Dictionary<string, List<KH.JHSCETakeRecord>>();

            //foreach (JHSchool.Data.JHSCETakeRecord jhsce in JHSchool.Data.JHSCETake.SelectByCourseAndExam("", exam_id))
            foreach (JHSchool.Data.JHSCETakeRecord jhsce in JHSchool.Data.JHSCETake.SelectByCourseAndExam(CourseIDs, exam_id))
            {
                KH.JHSCETakeRecord sce = new KH.JHSCETakeRecord(jhsce);
                if (!studentScoreDict.ContainsKey(sce.RefStudentID))
                    studentScoreDict.Add(sce.RefStudentID, new List<KH.JHSCETakeRecord>());
                studentScoreDict[sce.RefStudentID].Add(sce);
            }
            //foreach (var sce in JHSchool.Evaluation.Feature.QuerySCETake.GetKH.JHSCETakeRecords("", exam_id))
            //{
            //    if (!studentScoreDict.ContainsKey(sce.RefStudentID))
            //        studentScoreDict.Add(sce.RefStudentID, new List<KH.JHSCETakeRecord>());
            //    studentScoreDict[sce.RefStudentID].Add(sce);
            //}

            List<string> studentIDList = new List<string>(studentScoreDict.Keys);
            List<StudentRecord> validStudentList = Student.Instance.GetStudents(studentIDList.ToArray()).Where(x=>x.Status == "一般").ToList();
            List<KH.JHSCETakeRecord> sceList = new List<KH.JHSCETakeRecord>();
            foreach (var student in validStudentList)
                sceList.AddRange(studentScoreDict[student.ID]);

            #endregion

            #region 依 CourseID，將成績分開成不同 List

            Dictionary<string, List<KH.JHSCETakeRecord>> courseScoreDict = new Dictionary<string, List<KH.JHSCETakeRecord>>();

            foreach (var sce in sceList)
            {
                if (!courseScoreDict.ContainsKey(sce.RefCourseID))
                    courseScoreDict.Add(sce.RefCourseID, new List<KH.JHSCETakeRecord>());
                courseScoreDict[sce.RefCourseID].Add(sce);
            }

            #endregion

            return courseScoreDict;
        }

        /// <summary>
        /// 將課程填入ListView
        /// </summary>
        private void FillCourses(List<CourseListViewItem> list)
        {
            if (list.Count <= 0) return;

            listView.SuspendLayout();
            listView.Items.Clear();
            listView.Items.AddRange(list.ToArray());
            listView.ResumeLayout();
        }

        /// <summary>
        /// 將 CourseListViewItemList 排序
        /// </summary>
        private void SortItemList()
        {
            _courseListViewItemList.Sort(delegate(CourseListViewItem a, CourseListViewItem b) { return a.Text.CompareTo(b.Text); });
        }

        /// <summary>
        /// 按下「關閉」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 改變「僅顯示未完成輸入之課程」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDisplayNotFinish_CheckedChanged(object sender, EventArgs e)
        {
            FillCourses(GetDisplayList());
        }

        /// <summary>
        /// 取得要顯示的 CourseListViewItemList
        /// </summary>
        /// <returns></returns>
        private List<CourseListViewItem> GetDisplayList()
        {
            if (chkDisplayNotFinish.Checked == true)
            {
                List<CourseListViewItem> list = new List<CourseListViewItem>();
                foreach (CourseListViewItem item in _courseListViewItemList)
                {
                    if (item.IsFinish) continue;
                    list.Add(item);
                }
                return list;
            }
            else
            {
                return _courseListViewItemList;
            }
        }

        /// <summary>
        /// 按下「匯出到 Excel」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (listView.Items.Count <= 0) return;

            saveFileDialog1.FileName = string.Format("{0}學年度{1}學期{2}課程成績輸入狀況", intSchoolYear.Value, (intSemester.Value == 1) ? "上" : "下", ((KeyValuePair<string, string>)cboExam.SelectedItem).Value);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Workbook book = new Workbook();
                book.Worksheets.Clear();
                Worksheet ws = book.Worksheets[book.Worksheets.Add()];
                ws.Name = string.Format("{0}學年度 {1}學期 {2}", intSchoolYear.Value, (intSemester.Value == 1) ? "上" : "下", ((KeyValuePair<string, string>)cboExam.SelectedItem).Value);

                #region 加入 Header

                int row = 0;
                ws.Cells[row, chCourseName.Index].PutValue(chCourseName.Text);
                ws.Cells[row, chTeacher.Index].PutValue(chTeacher.Text);
                ws.Cells[row, chScore.Index].PutValue(chScore.Text);
                ws.Cells[row, chEffort.Index].PutValue(chEffort.Text);
                ws.Cells[row, chText.Index].PutValue(chText.Text);

                #endregion

                #region 加入每一筆課程輸入狀況

                listView.SuspendLayout();
                foreach (CourseListViewItem item in listView.Items)
                {
                    row++;
                    ws.Cells[row, chCourseName.Index].PutValue(item.Text);
                    ws.Cells[row, chTeacher.Index].PutValue(item.SubItems[chTeacher.Index].Text);
                    ws.Cells[row, chScore.Index].PutValue(item.SubItems[chScore.Index].Text);
                    ws.Cells[row, chEffort.Index].PutValue(item.SubItems[chEffort.Index].Text);
                    ws.Cells[row, chText.Index].PutValue(item.SubItems[chText.Index].Text);
                }
                listView.ResumeLayout();

                #endregion

                ws.AutoFitColumns();

                try
                {
                    book.Save(saveFileDialog1.FileName, FileFormatType.Excel2003);
                    Framework.MsgBox.Show("匯出完成。");
                }
                catch (Exception ex)
                {
                    Framework.MsgBox.Show("匯出失敗。" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 按下「重新整理」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                Course.Instance.SyncAllBackground();
                AssessmentSetup.Instance.SyncAllBackground();
                TCInstruct.Instance.SyncAllBackground();
                //SCAttend.Instance.SyncAllBackground();
                //AEInclude.Instance.SyncAllBackground();
                int count = Course.Instance.Items.Count +
                    AssessmentSetup.Instance.Items.Count +
                    TCInstruct.Instance.Items.Count;
                    //SCAttend.Instance.Items.Count;
                    //AEInclude.Instance.Items.Count;
            }
            RefreshListView();
        }

        /// <summary>
        /// 每一筆課程的評量狀況
        /// </summary>
        private class CourseListViewItem : ListViewItem
        {
            private const string Format = "{0}/{1}";
            private int _total;
            private int _scoreCount, _effortCount, _textCount;

            private string ScoreField { get { return string.Format(Format, _scoreCount, _total); } }
            private string EffortField { get { return string.Format(Format, _effortCount, _total); } }
            private string TextField { get { return string.Format(Format, _textCount, _total); } }

            private bool _is_finish;
            public bool IsFinish { get { return _is_finish; } }

            public CourseListViewItem(JHSchool.CourseRecord course, KH.JHAEIncludeRecord aei, List<KH.JHSCETakeRecord> sceList)
            {
                _is_finish = true;

                TeacherRecord teacher = course.GetFirstTeacher();

                //_total = course.GetAttends().Count;
                _total = SCAttend.GetCourseStudentCount(course.ID);
                Calculate(sceList);

                if (aei.UseScore) _is_finish &= (_scoreCount == _total);
                if (aei.UseEffort) _is_finish &= (_effortCount == _total);
                if (aei.UseText) _is_finish &= (_textCount == _total);

                this.Text = course.Name;
                this.SubItems.Add((teacher != null) ? teacher.Name : "");
                this.SubItems.Add(aei.UseScore ? ScoreField : "").ForeColor = (_scoreCount == _total) ? Color.Black : Color.Red;
                this.SubItems.Add(aei.UseEffort ? EffortField : "").ForeColor = (_effortCount == _total) ? Color.Black : Color.Red;
                this.SubItems.Add(aei.UseText ? TextField : "").ForeColor = (_textCount == _total) ? Color.Black : Color.Red;
            }

            private void Calculate(List<KH.JHSCETakeRecord> sceList)
            {
                _scoreCount = _effortCount = _textCount = 0;

                foreach (var sce in sceList)
                {
                    if (sce.Score.HasValue) _scoreCount++;
                    if (sce.Effort.HasValue) _effortCount++;
                    if (!string.IsNullOrEmpty(sce.Text)) _textCount++;
                }
            }
        }


    }
}
