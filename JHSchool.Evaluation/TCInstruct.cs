using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using FISCA.DSAUtil;
using JHSchool.Evaluation.Editor;
using FISCA.Presentation;
using JHSchool.Data;
using K12.Data;

namespace JHSchool.Evaluation
{
    /// <summary>
    /// 教師教授課程(課程與教師之間的多對多關連)。
    /// </summary>
    public class TCInstruct : CacheManager<TCInstructRecord>
    {
        //public static void TestProgram()
        //{
        //    TCInstruct.Instance.ItemLoaded += delegate
        //    {
        //        TCInstructRecordEditor editor = Course.Instance.SelectedList[0].SetSecondTeacher(Teacher.Instance.TemporaList[0]);
        //        editor.Save();
        //    };

        //    if (!TCInstruct.Instance.Loaded)
        //        TCInstruct.Instance.SyncAllBackground();
        //}

        private static TCInstruct _Instance = null;
        public static TCInstruct Instance { get { if (_Instance == null) _Instance = new TCInstruct(); return _Instance; } }

        private TCInstruct()
        {
            ItemLoaded += delegate
            {
                #region 重建課程及學生修課反查表
                _course_teachers.Clear();
                _teacher_courses.Clear();
                foreach (var item in Items)
                {
                    if (!_course_teachers.ContainsKey(item.RefCourseID))
                        _course_teachers.Add(item.RefCourseID, new List<string>());

                    if (!_teacher_courses.ContainsKey(item.RefTeacherID))
                        _teacher_courses.Add(item.RefTeacherID, new List<string>());

                    _teacher_courses[item.RefTeacherID].Add(item.ID);
                    _course_teachers[item.RefCourseID].Add(item.ID);
                }
                #endregion

                teacherField.Reload();
            };

            ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                #region 更新(課成)及(教師)修課反查表
                List<string> keys = new List<string>(e.PrimaryKeys);
                keys.Sort(); //排序後的資料才可以進行 BinarySearch。

                #region 掃描每一個(課程)的(所有授課教師)是否有在(e.PrimaryKeys)中出現。
                foreach (var cid in _course_teachers.Keys)
                {
                    List<string> removeItems = new List<string>();
                    foreach (var eachInstruct in _course_teachers[cid])
                    {
                        if (keys.BinarySearch(eachInstruct) >= 0)
                            removeItems.Add(eachInstruct);
                    }

                    foreach (var eachInstruct in removeItems)
                        _course_teachers[cid].Remove(eachInstruct);
                }
                #endregion

                #region 掃描每一個(教師)的(所有課程)是否有在(e.PrimaryKeys)中出現。
                foreach (var cid in _teacher_courses.Keys)
                {
                    List<string> removeItems = new List<string>();
                    foreach (var eachInstruct in _teacher_courses[cid])
                    {
                        if (keys.BinarySearch(eachInstruct) >= 0)
                            removeItems.Add(eachInstruct);
                    }

                    foreach (var item in removeItems)
                        _teacher_courses[cid].Remove(item);
                }
                #endregion

                #region 將新資料加入到原有的集合中。
                foreach (var key in e.PrimaryKeys)
                {
                    var item = Items[key];
                    if (item != null)
                    {
                        if (!_course_teachers.ContainsKey(item.RefCourseID))
                            _course_teachers.Add(item.RefCourseID, new List<string>());

                        if (!_teacher_courses.ContainsKey(item.RefTeacherID))
                            _teacher_courses.Add(item.RefTeacherID, new List<string>());

                        _course_teachers[item.RefCourseID].Add(item.ID);
                        _teacher_courses[item.RefTeacherID].Add(item.ID);
                    }
                }
                #endregion

                teacherField.Reload();
                #endregion
            };
        }

        private bool _initialized = false;
        private ListPaneField teacherField;
        private RibbonBarButton assignTeacherButton;

        /// <summary>
        /// 設定使用者介面
        /// </summary>
        public void SetupPresentation()
        {
            if (_initialized) return;

            #region 課程加入授課教師
            Course.Instance.SelectedListChanged += delegate { assignTeacherButton.Enable = (Course.Instance.SelectedList.Count > 0 && Teacher.Instance.TemporaList.Count > 0); };
            Teacher.Instance.TemporaListChanged += delegate { assignTeacherButton.Enable = (Course.Instance.SelectedList.Count > 0 && Teacher.Instance.TemporaList.Count > 0); };

            assignTeacherButton = Course.Instance.RibbonBarItems["指定"]["評分教師"];
            assignTeacherButton.Enable = false;
            assignTeacherButton.Image = Properties.Resources.teacher_64;
            MenuButton loadingMenuButton = assignTeacherButton["載入中…"];
            assignTeacherButton.PopupOpen += delegate(object sender, PopupOpenEventArgs e)
            {
                if (Teacher.Instance.TemporaList.Count <= 0) return;
                if (Course.Instance.SelectedList.Count <= 0) return;

                loadingMenuButton.Visible = false;

                foreach (var item in Teacher.Instance.TemporaList)
                {
                    MenuButton mb = e.VirtualButtons[item.Name];
                    mb.Tag = item;
                    mb.Click += new EventHandler(MenuButton_Click);
                }
            };
            #endregion

            #region 授課教師 ListPanelField
            teacherField = new ListPaneField("授課教師");
            teacherField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                string teacherName = "";
                if (TCInstruct.Instance.Loaded)
                {
                    TeacherRecord teacher1 = Course.Instance[e.Key].GetFirstTeacher();
                    if (teacher1 != null)
                        teacherName += teacher1.FullName;

                    TeacherRecord teacher2 = Course.Instance[e.Key].GetSecondTeacher();
                    if (teacher2 != null)
                    {
                        if (teacherName.Length > 0)
                            teacherName += "," + teacher2.FullName;
                        else
                            teacherName += teacher2.FullName;
                    }
                    TeacherRecord teacher3 = Course.Instance[e.Key].GetThirdTeacher();
                    if (teacher3 != null)
                    {
                        if (teacherName.Length > 0)
                            teacherName += "," + teacher3.FullName;
                        else
                            teacherName += teacher3.FullName;
                    }
                    if (teacherName.Length > 0)
                        e.Value = teacherName;
                    else
                        e.Value = string.Empty;
                }
                else
                    e.Value = "Loading...";
            };

            Course.Instance.AddListPaneField(teacherField);
            #endregion

            _initialized = true;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            TeacherRecord teacher = (sender as MenuButton).Tag as TeacherRecord;

            //List<string> courseIDs = new List<string>(K12.Presentation.NLDPanels.Course.SelectedSource);

            //List<JHTCInstructRecord> insertList = new List<JHTCInstructRecord>();
            //List<JHTCInstructRecord> updateList = new List<JHTCInstructRecord>();

            //foreach (JHTCInstructRecord tc in JHTCInstruct.SelectByTeacherIDAndCourseID(new string[] { }, K12.Presentation.NLDPanels.Course.SelectedSource))
            //{
            //    if (tc.Sequence == 1) //評分教師
            //    {
            //        courseIDs.Remove(tc.RefCourseID);
            //        tc.RefTeacherID = teacher.ID;
            //        updateList.Add(tc);
            //    }
            //}
            //foreach (string courseID in courseIDs)
            //{
            //    JHTCInstructRecord newTCInstruct = new JHTCInstructRecord();
            //    newTCInstruct.RefCourseID = courseID;
            //    newTCInstruct.RefTeacherID = teacher.ID;
            //    newTCInstruct.Sequence = 1;
            //    insertList.Add(newTCInstruct);
            //}

            //if (insertList.Count > 0)
            //    JHTCInstruct.Insert(insertList);
            //if (updateList.Count > 0)
            //    JHTCInstruct.Update(updateList);
            //MsgBox.Show("指定評分教師完成");

            List<TCInstructRecordEditor> editors = new List<TCInstructRecordEditor>();
            foreach (var item in Course.Instance.SelectedList)
                editors.Add(item.SetFirstTeacher(teacher));

            if (editors.Count > 0)
            {
                MultiThreadBackgroundWorker<TCInstructRecordEditor> worker = new MultiThreadBackgroundWorker<TCInstructRecordEditor>();
                worker.PackageSize = 50;
                worker.Loading = MultiThreadLoading.Light;
                worker.DoWork += delegate(object worker_sender, PackageDoWorkEventArgs<TCInstructRecordEditor> worker_e)
                {
                    worker_e.Items.SaveAllEditors();
                };
                worker.RunWorkerCompleted += delegate
                {
                    MsgBox.Show("指定評分教師完成");
                };
                worker.RunWorkerAsync(editors);
            }
        }

        protected override Dictionary<string, TCInstructRecord> GetAllData()
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("All");

            DSRequest dsreq = new DSRequest(helper);
            Dictionary<string, TCInstructRecord> result = new Dictionary<string, TCInstructRecord>();
            string srvname = "SmartSchool.Course.GetTCInstruct";
            foreach (var item in FISCA.Authentication.DSAServices.CallService(srvname, dsreq).GetContent().GetElements("TCInstruct"))
            {
                helper = new DSXmlHelper(item);
                var teacherid = helper.GetText("RefTeacherID");
                var courseid = helper.GetText("RefCourseID");
                var id = item.GetAttribute("ID");
                var sequence = helper.GetText("Sequence");

                TCInstructRecord record = new TCInstructRecord(teacherid, courseid, id, sequence);
                result.Add(record.ID, record);
            }
            return result;
        }

        protected override Dictionary<string, TCInstructRecord> GetData(IEnumerable<string> primaryKeys)
        {
            // 指示是否需要呼叫 Service。
            bool execute_require = false;

            //建立 Request。
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("All");
            helper.AddElement("Condition");
            foreach (string id in primaryKeys)
            {
                helper.AddElement("Condition", "ID", id);
                execute_require = true;
            }

            //儲存最後結果的集合。
            Dictionary<string, TCInstructRecord> result = new Dictionary<string, TCInstructRecord>();

            if (execute_require)
            {
                string srvname = "SmartSchool.Course.GetTCInstruct";
                DSRequest dsreq = new DSRequest(helper);

                foreach (var item in FISCA.Authentication.DSAServices.CallService(srvname, dsreq).GetContent().GetElements("TCInstruct"))
                {
                    helper = new DSXmlHelper(item);
                    var teacherid = helper.GetText("RefTeacherID");
                    var courseid = helper.GetText("RefCourseID");
                    var id = item.GetAttribute("ID");
                    var sequence = helper.GetText("Sequence");

                    TCInstructRecord record = new TCInstructRecord(teacherid, courseid, id, sequence);
                    result.Add(record.ID, record);
                }
            }

            return result;
        }

        /// <summary>
        /// 從(課程)查詢授課教師。
        /// </summary>
        private Dictionary<string, List<string>> _course_teachers = new Dictionary<string, List<string>>();

        /// <summary>
        /// 取得課程的所有授課教師。
        /// </summary>
        /// <param name="courseID">課程編號。</param>
        /// <returns>授課教師清單。</returns>
        public List<TCInstructRecord> GetCourseTeachers(string courseID)
        {
            List<TCInstructRecord> result = new List<TCInstructRecord>();
            if (_course_teachers.ContainsKey(courseID))
            {
                foreach (var eachInstructID in _course_teachers[courseID])
                {
                    var objInstruct = Items[eachInstructID];
                    if (objInstruct.Course != null && objInstruct.Teacher != null)
                        result.Add(Items[eachInstructID]);
                }
            }
            return result;
        }

        /// <summary>
        /// 從(教師)查詢教授課程。
        /// </summary>
        private Dictionary<string, List<string>> _teacher_courses = new Dictionary<string, List<string>>();

        /// <summary>
        /// 取得教師所授教的課程。
        /// </summary>
        /// <param name="studentID">學生編號</param>
        /// <returns>修課記錄清單</returns>
        public List<TCInstructRecord> GetTeacherCourses(string studentID)
        {
            List<TCInstructRecord> result = new List<TCInstructRecord>();
            if (_teacher_courses.ContainsKey(studentID))
            {
                foreach (var eachInstructID in _teacher_courses[studentID])
                {
                    var objInstruct = Items[eachInstructID];
                    if (objInstruct.Course != null && objInstruct.Teacher != null)
                        result.Add(Items[eachInstructID]);
                }
            }
            return result;
        }
    }

    public static class TCInstruct_ExtendMethods
    {
        /// <summary>
        /// 取得課程的第一位授課教師。
        /// </summary>
        public static TeacherRecord GetFirstTeacher(this CourseRecord course)
        {
            if (course != null)
            {
                TCInstructRecord tc = course.GetFirstInstruct();
                if (tc != null) return tc.Teacher;
            }
            return null;
        }

        /// <summary>
        /// 取得課程的第二位授課教師。
        /// </summary>
        public static TeacherRecord GetSecondTeacher(this CourseRecord course)
        {
            if (course != null)
            {
                TCInstructRecord tc = course.GetSecondInstruct();
                if (tc != null) return tc.Teacher;
            }
            return null;
        }

        /// <summary>
        /// 取得課程的第三位授課教師。
        /// </summary>
        public static TeacherRecord GetThirdTeacher(this CourseRecord course)
        {
            if (course != null)
            {
                TCInstructRecord tc = course.GetThirdInstruct();
                if (tc != null) return tc.Teacher;
            }
            return null;
        }

        /// <summary>
        /// 取得課程的第一位授課教師。
        /// </summary>
        internal static TCInstructRecord GetFirstInstruct(this CourseRecord course)
        {
            if (course != null)
            {
                foreach (TCInstructRecord each in course.GetInstructs())
                    if (each.Sequence == "1") return each;
            }
            return null;
        }

        /// <summary>
        /// 取得課程的第二位授課教師。
        /// </summary>
        internal static TCInstructRecord GetSecondInstruct(this CourseRecord course)
        {
            if (course != null)
            {
                foreach (TCInstructRecord each in course.GetInstructs())
                    if (each.Sequence == "2") return each;
            }
            return null;
        }

        /// <summary>
        /// 取得課程的第三位授課教師。
        /// </summary>
        internal static TCInstructRecord GetThirdInstruct(this CourseRecord course)
        {
            if (course != null)
            {
                foreach (TCInstructRecord each in course.GetInstructs())
                    if (each.Sequence == "3") return each;
            }
            return null;
        }

        /// <summary>
        /// 取得課程的所有上課教師關聯資料。
        /// </summary>
        public static List<TCInstructRecord> GetInstructs(this CourseRecord course)
        {
            if (course != null)
                return TCInstruct.Instance.GetCourseTeachers(course.ID);
            else
                return null;
        }

        /// <summary>
        /// 取得教師上的所有課程關聯資料。 
        /// </summary>
        public static List<TCInstructRecord> GetInstructs(this TeacherRecord teacher)
        {

            return TCInstruct.Instance.GetTeacherCourses(teacher.ID);
        }

        /// <summary>
        /// 取得課程的所有教師資料。
        /// </summary>
        public static List<TeacherRecord> GetInstructTeachers(this CourseRecord course)
        {
            if (course != null)
            {
                List<TeacherRecord> teachers = new List<TeacherRecord>();
                foreach (TCInstructRecord each in GetInstructs(course))
                    teachers.Add(each.Teacher);

                return teachers;
            }
            else
                return null;
        }

        /// <summary>
        /// 取得教師上的所有課程資料。
        /// </summary>
        public static List<CourseRecord> GetInstructCoruses(this TeacherRecord teacher)
        {
            List<CourseRecord> courses = new List<CourseRecord>();
            foreach (TCInstructRecord each in GetInstructs(teacher))
                courses.Add(each.Course);

            return courses;
        }
    }
}
