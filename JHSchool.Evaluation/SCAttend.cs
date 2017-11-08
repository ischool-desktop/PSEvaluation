using System;
using System.Collections.Generic;
using System.Text;
using FISCA.Presentation;
using Framework;
using FISCA.Authentication;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation
{
    /// <summary>
    /// 修課記錄資料管理
    /// </summary>
    public class SCAttend //: CacheManager<SCAttendRecord>
    {
        private static SCAttend _Instance = null;
        /// <summary>
        /// 取得唯一實體
        /// </summary>
        public static SCAttend Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new SCAttend();
                return _Instance;
            }
        }

        ////用於執行緒同步 _course_attends、_student_attends 的物件。
        //private object sync_attend_object = new object();
        ////課程修課學生反查表
        //private Dictionary<string, List<string>> _course_attends = new Dictionary<string, List<string>>();
        ////學生修課反查表
        //private Dictionary<string, List<string>> _student_attends = new Dictionary<string, List<string>>();
        //private SCAttend()
        //{
        //    this.ItemLoaded += delegate
        //    {
        //        #region 重建課程及學生修課反查表
        //        lock (sync_attend_object)
        //        {
        //            _course_attends.Clear();
        //            _student_attends.Clear();
        //            foreach (var item in Items)
        //            {
        //                if (!_course_attends.ContainsKey(item.RefCourseID))
        //                    _course_attends.Add(item.RefCourseID, new List<string>());
        //                _course_attends[item.RefCourseID].Add(item.ID);
        //                if (!_student_attends.ContainsKey(item.RefStudentID))
        //                    _student_attends.Add(item.RefStudentID, new List<string>());
        //                _student_attends[item.RefStudentID].Add(item.ID);
        //            }
        //        }
        //        #endregion
        //        //重載畫面
        //        //_Field.Reload();
        //    };
        //    this.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
        //    {
        //        #region 更新課成及學生修課反查表
        //        lock (sync_attend_object)
        //        {
        //            List<string> keys = new List<string>(e.PrimaryKeys);
        //            keys.Sort();
        //            #region 掃描修課學生中有被更新者先移除
        //            foreach (var cid in _course_attends.Keys)
        //            {
        //                List<string> removeItems = new List<string>();
        //                foreach (var item in _course_attends[cid])
        //                {
        //                    if (keys.BinarySearch(item) >= 0)
        //                    {
        //                        removeItems.Add(item);
        //                    }
        //                }
        //                foreach (var item in removeItems)
        //                {
        //                    _course_attends[cid].Remove(item);
        //                }
        //            }
        //            #endregion
        //            #region 掃描學生修課中有被更新者先移除
        //            foreach (var cid in _student_attends.Keys)
        //            {
        //                List<string> removeItems = new List<string>();
        //                foreach (var item in _student_attends[cid])
        //                {
        //                    if (keys.BinarySearch(item) >= 0)
        //                    {
        //                        removeItems.Add(item);
        //                    }
        //                }
        //                foreach (var item in removeItems)
        //                {
        //                    _student_attends[cid].Remove(item);
        //                }
        //            }
        //            #endregion
        //            #region 將更新後資料重新加入至課程及學生修課反查表
        //            foreach (var key in e.PrimaryKeys)
        //            {
        //                var item = Items[key];
        //                if (item != null)
        //                {
        //                    if (!_course_attends.ContainsKey(item.RefCourseID))
        //                        _course_attends.Add(item.RefCourseID, new List<string>());
        //                    _course_attends[item.RefCourseID].Add(item.ID);
        //                    if (!_student_attends.ContainsKey(item.RefStudentID))
        //                        _student_attends.Add(item.RefStudentID, new List<string>());
        //                    _student_attends[item.RefStudentID].Add(item.ID);
        //                }
        //            }
        //            #endregion
        //        }
        //        #endregion
        //        //重載畫面
        //        // _Field.Reload();
        //    };
        //}
        //protected override Dictionary<string, SCAttendRecord> GetAllData()
        //{
        //    Dictionary<string, SCAttendRecord> results = new Dictionary<string, SCAttendRecord>();
        //    foreach (var item in JHSchool.Evaluation.Feature.QuerySCAttend.GetAllSCAttendRecords())
        //    {
        //        results.Add(item.ID, item);
        //    }
        //    return results;
        //}
        //protected override Dictionary<string, SCAttendRecord> GetData(IEnumerable<string> primaryKeys)
        //{
        //    Dictionary<string, SCAttendRecord> results = new Dictionary<string, SCAttendRecord>();
        //    foreach (var item in JHSchool.Evaluation.Feature.QuerySCAttend.GetSCAttendRecords(primaryKeys))
        //    {
        //        results.Add(item.ID, item);
        //    }
        //    return results;
        //}

        /// <summary>
        /// 記錄課程的修課學生數。
        /// </summary>
        private static Dictionary<string, int> CSCount = new Dictionary<string, int>();
        private static object CSCount_SyncRoot = new object();

        public static int GetCourseStudentCount(string courseID)
        {
            lock (CSCount_SyncRoot)
            {
                int result;

                if (CSCount.TryGetValue(courseID, out result))
                    return result;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 重新載入修課人數資訊。
        /// </summary>
        public static void ReloadCourseStudentCount()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(x =>
            {
                try
                {
                    FISCA.DSAUtil.DSXmlHelper req = new FISCA.DSAUtil.DSXmlHelper();
                    req.AddElement("Field");
                    req.AddElement("Field", "ID");
                    req.AddElement("Field", "StudentCount");
                    req.AddElement("Condition");
                    req.AddElement("Condition", "Status", "1");
                    req.AddElement("Condition", "Status", "2");

                    DSResponse rsp = DSAServices.CallService("SmartSchool.Course.GetCourseAttendCount", new DSRequest(req));

                    lock (CSCount_SyncRoot)
                    {
                        CSCount = new Dictionary<string, int>();
                        foreach (XmlElement each in rsp.GetContent().GetElements("Course"))
                            CSCount.Add(each.GetAttribute("ID"), int.Parse(each.SelectSingleNode("StudentCount").InnerText));
                    }
                }
                catch (FISCA.DSAUtil.DSAServerException ex)
                {
                    if (ex.ServerStatus == FISCA.DSAUtil.DSAServerStatus.ServiceNotFound)
                        System.IO.File.AppendAllText("缺少Service.txt", "SmartSchool.Course.GetCourseAttendCount");
                    else
                        throw;
                }
                finally
                {
                }
            });
        }

        private bool _Initialed = false;
        //private ListPaneField _Field = new ListPaneField("學生數");
        private RibbonBarButton courseAssignButton;
        private RibbonBarButton studentAssignButton;

        /// <summary>
        /// 設定使用者介面
        /// </summary>
        public void SetupPresentation()
        {
            if (_Initialed) return;

            ReloadCourseStudentCount();
            JHSchool.Data.JHSCAttend.AfterInsert += delegate(object sender, K12.Data.DataChangedEventArgs e)
            {
                ReloadCourseStudentCount();
            };

            JHSchool.Data.JHSCAttend.AfterUpdate += delegate(object sender, K12.Data.DataChangedEventArgs e)
            {
                ReloadCourseStudentCount();
            };

            JHSchool.Data.JHSCAttend.AfterDelete += delegate(object sender, K12.Data.DataChangedEventArgs e)
            {
                ReloadCourseStudentCount();
            };

            #region 課程欄位「學生數」
            //_Field.GetVariable += delegate(object sender, GetVariableEventArgs e)
            //{
            //    if (Loaded)
            //        e.Value = GetCourseAttended(e.Key).Count;
            //    else
            //        e.Value = "Loading";
            //};

            //_Field.CompareValue += delegate(object sender, CompareValueEventArgs e)
            //{
            //    int x, y;

            //    if (!int.TryParse(e.Value1.ToString(), out x))
            //        x = int.MaxValue;

            //    if (!int.TryParse(e.Value2.ToString(), out y))
            //        y = int.MaxValue;

            //    e.Result = x.CompareTo(y);
            //};
            //Course.Instance.AddListPaneField(_Field);
            #endregion

            #region 課程指定修課學生
            Course.Instance.SelectedListChanged += delegate { courseAssignButton.Enable = (Course.Instance.SelectedList.Count > 0 && Student.Instance.TemporaList.Count > 0); };
            Student.Instance.TemporaListChanged += delegate { courseAssignButton.Enable = (Course.Instance.SelectedList.Count > 0 && Student.Instance.TemporaList.Count > 0); };

            courseAssignButton = Course.Instance.RibbonBarItems["指定"]["修課學生"];
            courseAssignButton.Image = Properties.Resources.college_64;
            courseAssignButton.Enable = false;
            courseAssignButton.PopupOpen += delegate(object sender, PopupOpenEventArgs e)
            {
                if (Student.Instance.TemporaList.Count <= 0) return;
                if (Course.Instance.SelectedList.Count <= 0) return;

                bool first = true;
                foreach (var item in Student.Instance.TemporaList)
                {
                    string name = string.Format("【{0}】{1}", (item.Class != null) ? item.Class.Name : "", item.Name);
                    MenuButton mb = e.VirtualButtons[name];
                    if (first)
                    {
                        mb.BeginGroup = true;
                        first = false;
                    }
                    mb.Tag = item;
                    mb.Click += new EventHandler(CourseMenuButton_Click);
                }
            };
            courseAssignButton["加入所有待處理學生"].Click += delegate
            {
                AddSCAttends(Student.Instance.TemporaList, Course.Instance.SelectedList);
            };

            #endregion

            #region 學生指定修課課程
            Student.Instance.SelectedListChanged += delegate { studentAssignButton.Enable = (Student.Instance.SelectedList.Count > 0 && Course.Instance.TemporaList.Count > 0); };
            Course.Instance.TemporaListChanged += delegate { studentAssignButton.Enable = (Student.Instance.SelectedList.Count > 0 && Course.Instance.TemporaList.Count > 0); };

            Student.Instance.RibbonBarItems["指定"]["修課"].Image = Properties.Resources.CourseIcon;
            studentAssignButton = Student.Instance.RibbonBarItems["指定"]["修課"];
            studentAssignButton.Enable = false;
            studentAssignButton.PopupOpen += delegate(object sender, PopupOpenEventArgs e)
            {
                if (Course.Instance.TemporaList.Count <= 0) return;
                if (Student.Instance.SelectedList.Count <= 0) return;

                bool first = true;
                foreach (var item in Course.Instance.TemporaList)
                {
                    MenuButton mb = e.VirtualButtons[item.Name];
                    if (first)
                    {
                        mb.BeginGroup = true;
                        first = false;
                    }
                    mb.Tag = item;
                    mb.Click += new EventHandler(StudentMenuButton_Click);
                }
            };
            studentAssignButton["加入所有待處理課程"].Click += delegate
            {
                AddSCAttends(Student.Instance.SelectedList, Course.Instance.TemporaList);
            };
            #endregion

            _Initialed = true;
        }

        private void StudentMenuButton_Click(object sender, EventArgs e)
        {
            CourseRecord course = (sender as MenuButton).Tag as CourseRecord;
            List<CourseRecord> courses = new List<CourseRecord>();
            courses.Add(course);
            AddSCAttends(Student.Instance.SelectedList, courses);
        }

        private void CourseMenuButton_Click(object sender, EventArgs e)
        {
            StudentRecord student = (sender as MenuButton).Tag as StudentRecord;
            List<StudentRecord> students = new List<StudentRecord>();
            students.Add(student);
            AddSCAttends(students, Course.Instance.SelectedList);
        }

        private void AddSCAttends(List<StudentRecord> students, List<CourseRecord> courses)
        {
            if (students.Count <= 0) return;
            if (courses.Count <= 0) return;

            FISCA.LogAgent.LogSaver logSaver = FISCA.LogAgent.ApplicationLog.CreateLogSaverInstance();

            Dictionary<string, List<string>> studentScattends = new Dictionary<string, List<string>>();
            List<string> courseIDs = courses.AsKeyList();
            StringBuilder coursesBuilder = new StringBuilder("");
            string coursesString = string.Empty;
            foreach (CourseRecord courseRecord in courses)
                coursesBuilder.Append(courseRecord.Name + "、");
            coursesString = coursesBuilder.ToString();
            if (coursesString.EndsWith("、")) coursesString = coursesString.Substring(0, coursesString.Length - 1);

            foreach (var student in students)
            {
                if (!studentScattends.ContainsKey(student.ID))
                {
                    studentScattends.Add(student.ID, courseIDs);

                    string s = string.Empty;
                    if (student.Class != null)
                    {
                        s += student.Class.Name;
                        if (student.SeatNo != "")
                            s += "(" + student.SeatNo + "號)";
                        s += " ";
                    }
                    if (student.StudentNumber != "")
                        s += student.StudentNumber + " ";
                    if (s == "")
                        s += "學生：";
                    s += student.Name;

                    string desc = string.Format("學生「{0}」加入修課：{1}", s, coursesString);
                    logSaver.AddBatch("成績系統.修課", "學生指定修課", desc);
                }
            }

            foreach (Data.JHSCAttendRecord record in Data.JHSCAttend.SelectByStudentIDAndCourseID(students.AsKeyList(), courses.AsKeyList()))
            {
                if (studentScattends.ContainsKey(record.RefStudentID))
                    if (studentScattends[record.RefStudentID].Contains(record.RefCourseID))
                        studentScattends[record.RefStudentID].Remove(record.RefCourseID);
            }

            List<Data.JHSCAttendRecord> list = new List<JHSchool.Data.JHSCAttendRecord>();
            foreach (string studentID in studentScattends.Keys)
            {
                foreach (string courseID in studentScattends[studentID])
                {
                    Data.JHSCAttendRecord record = new JHSchool.Data.JHSCAttendRecord();
                    record.RefCourseID = courseID;
                    record.RefStudentID = studentID;
                    list.Add(record);
                }
            }

            if (list.Count > 0)
            {
                MultiThreadBackgroundWorker<Data.JHSCAttendRecord> worker = new MultiThreadBackgroundWorker<JHSchool.Data.JHSCAttendRecord>();
                worker.PackageSize = 50;
                worker.Loading = MultiThreadLoading.Light;
                worker.DoWork += delegate(object sender, PackageDoWorkEventArgs<Data.JHSCAttendRecord> e)
                {
                    Data.JHSCAttend.Insert(e.Items);
                };
                worker.RunWorkerCompleted += delegate
                {
                    logSaver.LogBatch();
                    MsgBox.Show("指定修課完成");
                };
                worker.RunWorkerAsync(list);
            }
        }

        ///// <summary>
        ///// 取得課程的所有修課記錄
        ///// </summary>
        ///// <param name="courseID">課程編號</param>
        ///// <returns>修課記錄清單</returns>
        //public List<SCAttendRecord> GetCourseAttended(string courseID)
        //{
        //    List<SCAttendRecord> result = new List<SCAttendRecord>();
        //    lock (sync_attend_object)
        //    {
        //        if (_course_attends.ContainsKey(courseID))
        //        {
        //            foreach (var item in _course_attends[courseID])
        //            {
        //                var scattend = Items[item];
        //                if (scattend.Student != null && scattend.Course != null)
        //                    result.Add(Items[item]);
        //            }
        //        }
        //    }
        //    return result;
        //}
        ///// <summary>
        ///// 取得學生的所有修課記錄
        ///// </summary>
        ///// <param name="studentID">學生編號</param>
        ///// <returns>修課記錄清單</returns>
        //public List<SCAttendRecord> GetStudentAttend(string studentID)
        //{
        //    List<SCAttendRecord> result = new List<SCAttendRecord>();
        //    lock (sync_attend_object)
        //    {
        //        if (_student_attends.ContainsKey(studentID))
        //        {
        //            foreach (var item in _student_attends[studentID])
        //            {
        //                var scattend = Items[item];
        //                if (scattend.Student != null && scattend.Course != null)
        //                    result.Add(Items[item]);
        //            }
        //        }
        //    }
        //    return result;
        //}
    }

    //public static class StudentAttendCourseExtendFunctions
    //{
    //    /// <summary>
    //    /// 取得課程的所有修課記錄
    //    /// </summary>
    //    /// <param name="courseRecord">課程</param>
    //    /// <returns>修課記錄清單</returns>
    //    public static List<SCAttendRecord> GetAttends(this CourseRecord courseRecord)
    //    {
    //        return SCAttend.Instance.GetCourseAttended(courseRecord.ID);
    //    }
    //    /// <summary>
    //    /// 取得學生的所有修課記錄
    //    /// </summary>
    //    /// <param name="courseRecord">學生</param>
    //    /// <returns>修課記錄清單</returns>
    //    public static List<SCAttendRecord> GetAttends(this StudentRecord studentRecord)
    //    {
    //        return SCAttend.Instance.GetCourseAttended(studentRecord.ID);
    //    }
    //    /// <summary>
    //    /// 取得課程的所有修課學生
    //    /// </summary>
    //    /// <param name="courseRecord">課程</param>
    //    /// <returns>修課學生清單</returns>
    //    public static List<StudentRecord> GetAttendStudents(this CourseRecord courseRecord)
    //    {
    //        List<StudentRecord> students = new List<StudentRecord>();
    //        foreach (var item in SCAttend.Instance.GetCourseAttended(courseRecord.ID))
    //        {
    //            students.Add(item.Student);
    //        }
    //        return students;
    //    }
    //    /// <summary>
    //    /// 取得學生的所有修課
    //    /// </summary>
    //    /// <param name="courseRecord">學生</param>
    //    /// <returns>課程清單</returns>
    //    public static List<CourseRecord> GetAttendCourses(this StudentRecord studentRecord)
    //    {
    //        List<CourseRecord> courses = new List<CourseRecord>();
    //        foreach (var item in SCAttend.Instance.GetStudentAttend(studentRecord.ID))
    //        {
    //            courses.Add(item.Course);
    //        }
    //        return courses;
    //    }
    //}
}
