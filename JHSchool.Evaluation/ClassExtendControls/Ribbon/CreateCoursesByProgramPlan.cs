using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Framework;
using JHSchool.Evaluation.ClassExtendControls.Ribbon.CreateCoursesRelated;
using JHSchool.Data;
using JHSchool.Evaluation.Legacy;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon
{
    public class CreateCoursesByProgramPlan
    {
        internal static void Run()
        {
            new CreateCoursesByProgramPlan();
        }

        private BackgroundWorker Worker { get; set; }
        private BackgroundWorker Preloader { get; set; }
        private ManualResetEvent Event { get; set; }

        private ProgramPlanCollection PPCollection { get; set; }

        /// <summary>
        /// Constructor
        /// 依課程規劃表開課。
        /// </summary>
        public CreateCoursesByProgramPlan()
        {
            if (EachClassHasProgramPlan() == false) return; //如果有班級沒有課程規劃，停止開課。

            InitializePreloaderAndResetEvent();
            Preloader.RunWorkerAsync();

            SelectSemesterForm form = new SelectSemesterForm();
            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            Event.WaitOne();

            InitializeWorker();
            Worker.RunWorkerAsync(form);
        }

        /// <summary>
        /// 檢查每個班級是否有課程規劃表
        /// </summary>
        /// <returns></returns>
        private bool EachClassHasProgramPlan()
        {
            List<JHClassRecord> noProgramPlanClasses = new List<JHClassRecord>();
            foreach (JHClassRecord cla in JHClass.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource))
                if (string.IsNullOrEmpty(cla.RefProgramPlanID))
                    noProgramPlanClasses.Add(cla);

            if (noProgramPlanClasses.Count > 0)
            {
                ErrorViewer viewer = new ErrorViewer();
                viewer.SetColumns("班級", "錯誤訊息");
                foreach (var cla in noProgramPlanClasses)
                    viewer.AddRow(cla.Name, "沒有設定課程規劃表。");
                viewer.Show();
                return false;
            }
            return true;
        }

        private void InitializePreloaderAndResetEvent()
        {
            Preloader = new BackgroundWorker();
            Preloader.DoWork += new DoWorkEventHandler(Preloader_DoWork);
            Preloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Preloader_RunWorkerCompleted);

            Event = new ManualResetEvent(false);
        }

        private void InitializeWorker()
        {
            Worker = new BackgroundWorker();
            Worker.WorkerReportsProgress = true;
            Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
        }

        #region Preloader Events
        private void Preloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw e.Error;

            Event.Set();
        }

        private void Preloader_DoWork(object sender, DoWorkEventArgs e)
        {
            PPCollection = new ProgramPlanCollection();
            foreach (JHProgramPlanRecord record in JHProgramPlan.SelectAll())
                PPCollection.Add(record.ID, record);
        }
        #endregion

        #region Worker Events
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //MsgBox.Show(Diagnostic.GetXmlString(e.Error));
                MsgBox.Show("開課發生錯誤。" + e.Error.Message);
                //SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
            }
            FISCA.Presentation.MotherForm.SetStatusBarMessage("開課完成。");
            MsgBox.Show("開課完成。");

            //TODO 這個之後應該拿掉...
            //SCAttend.Instance.SyncAllBackground();
            TCInstruct.Instance.SyncAllBackground();
            Course.Instance.SyncAllBackground();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            #region DoWork

            SelectSemesterForm opt = e.Argument as SelectSemesterForm;

            Worker.ReportProgress(0, "正在進行開課...");

            double totalClass = K12.Presentation.NLDPanels.Class.SelectedSource.Count;
            if (totalClass <= 0) totalClass = 0;
            double classCount = 0;

            Dictionary<string, JHCourseRecord> subjectCourseDict = new Dictionary<string, JHCourseRecord>();

            foreach (JHClassRecord cla in JHClass.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource))
            {
                #region 班級開課

                classCount++;

                int gradeYear = 0; //取得班級年級
                if (!cla.GradeYear.HasValue) continue;
                gradeYear = cla.GradeYear.Value;

                // TODO: 先寫著，之後再討論…
                if (gradeYear >= 7) gradeYear -= 6;

                #region 取得班級內每個學生的課程規劃表
                List<JHProgramPlanRecord> programPlanList = new List<JHProgramPlanRecord>();

                List<JHStudentRecord> studentsInSchool = GetInSchoolStudents(cla.Students);
                if (studentsInSchool.Count > 0) //班級有學生，抓學生課程規劃表
                {
                    foreach (JHStudentRecord stu in studentsInSchool)
                    {
                        //取得學生的課程規劃表
                        JHProgramPlanRecord record = PPCollection.GetProgramPlanRecord(stu);
                        if (record != null)
                        {
                            if (!programPlanList.Contains(record))
                                programPlanList.Add(record);
                        }
                    }
                }
                else //班級沒有學生，也是要開課的！
                {
                    JHProgramPlanRecord record = PPCollection.GetProgramPlanRecord(cla);
                    if (record != null)
                    {
                        if (!programPlanList.Contains(record))
                            programPlanList.Add(record);
                    }
                }
                #endregion

                #region 所有課程規劃表中要開的課程
                Dictionary<string, K12.Data.ProgramSubject> courseDict = new Dictionary<string, K12.Data.ProgramSubject>();

                foreach (JHProgramPlanRecord record in programPlanList)
                {
                    foreach (K12.Data.ProgramSubject subject in record.Subjects)
                    {
                        if (subject.GradeYear == gradeYear &&
                            subject.Semester == opt.Semester &&
                            subject.Period.HasValue &&
                            subject.Credit.HasValue)
                        {
                            string key = subject.SubjectName.Trim();
                            if (!courseDict.ContainsKey(key))
                                courseDict.Add(key, subject);
                        }
                    }
                }
                #endregion

                //快取來源學期課程先放這
                Dictionary<string, JHCourseRecord> copySourceCourses = new Dictionary<string, JHCourseRecord>();

                #region 取得本學期已開的課程
                Dictionary<string, JHCourseRecord> existSubjects = new Dictionary<string, JHCourseRecord>();

                foreach (JHCourseRecord course in JHCourse.SelectAll())
                {
                    #region 取得複製來源學期的課程，智慧型開課需要
                    if (opt.CopyOptionEnabled && (opt.CopyTeacher || opt.CopyAssessmentSetup))
                    {
                        if (course.SchoolYear == opt.CopySchoolYear && course.Semester == opt.CopySemester && course.RefClassID == cla.ID)
                        {
                            if (!copySourceCourses.ContainsKey(course.Subject))
                                copySourceCourses.Add(course.Subject, course);
                        }
                    }
                    #endregion

                    //取得本學期的課程
                    if (course.SchoolYear == opt.SchoolYear && course.Semester == opt.Semester && course.RefClassID == cla.ID)
                    {
                        if (!existSubjects.ContainsKey(course.Subject))
                            existSubjects.Add(course.Subject, course);
                    }
                }
                #endregion

                #region 開課(加入新課程)
                List<JHCourseRecord> insertCourseList = new List<JHCourseRecord>();
                foreach (string key in courseDict.Keys)
                {
                    //是原來沒有的課程
                    if (!existSubjects.ContainsKey(key))
                    {
                        K12.Data.ProgramSubject subject = courseDict[key];
                        JHCourseRecord newCourse = new JHCourseRecord();

                        newCourse.Credit = subject.Credit;
                        newCourse.Period = subject.Period;
                        newCourse.Domain = subject.Domain;
                        newCourse.Name = cla.Name + " " + subject.SubjectName;
                        newCourse.SchoolYear = opt.SchoolYear;
                        newCourse.Semester = opt.Semester;
                        newCourse.Subject = subject.SubjectName.Trim();
                        newCourse.RefClassID = cla.ID;
                        newCourse.CalculationFlag = subject.CalcFlag ? "1" : "2";

                        insertCourseList.Add(newCourse);
                    }
                }
                if (insertCourseList.Count > 0)
                    JHCourse.Insert(insertCourseList);
                #endregion

                #region 重新取得本學期已開的課程(包含這次新開的課程)
                JHCourse.RemoveAll();
                Dictionary<string, JHCourseRecord> reloadedExistSubjects = new Dictionary<string, JHCourseRecord>();
                List<string> courseIDs = new List<string>();
                foreach (JHCourseRecord course in JHCourse.SelectAll())
                {
                    if (course.SchoolYear == opt.SchoolYear &&
                        course.Semester == opt.Semester &&
                        course.RefClassID == cla.ID)
                    {
                        string key = course.Subject;
                        if (!reloadedExistSubjects.ContainsKey(key))
                            reloadedExistSubjects.Add(key, course);

                        courseIDs.Add(course.ID);
                    }
                }
                #endregion

                #region 加入學生修課
                CourseAttendCollection caCollection = new CourseAttendCollection();
                foreach (JHSCAttendRecord sca in JHSCAttend.SelectByCourseIDs(courseIDs))
                {
                    if (!caCollection.ContainsKey(sca.RefCourseID))
                        caCollection.Add(sca.RefCourseID, new List<JHSCAttendRecord>());
                    caCollection[sca.RefCourseID].Add(sca);
                }

                List<JHSCAttendRecord> insertSCAttendList = new List<JHSCAttendRecord>();

                foreach (JHStudentRecord student in GetInSchoolStudents(cla.Students))
                {
                    if (PPCollection.GetProgramPlanRecord(student) == null) continue;

                    foreach (K12.Data.ProgramSubject subject in PPCollection.GetProgramPlanRecord(student).Subjects)
                    {
                        string key = subject.SubjectName.Trim();

                        if (subject.GradeYear == gradeYear &&
                            subject.Semester == opt.Semester &&
                            reloadedExistSubjects.ContainsKey(key))
                        {
                            bool found = false;
                            foreach (JHStudentRecord attendStudent in caCollection.GetAttendStudents(reloadedExistSubjects[key]))
                            {
                                if (attendStudent.ID == student.ID)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found == false)
                            {
                                JHSCAttendRecord newSCAttend = new JHSCAttendRecord();
                                newSCAttend.RefStudentID = student.ID;
                                newSCAttend.RefCourseID = reloadedExistSubjects[key].ID;
                                insertSCAttendList.Add(newSCAttend);
                            }
                        }
                    }
                }
                if (insertSCAttendList.Count > 0)
                {
                    int t1 = Environment.TickCount;

                    JHSCAttend.Insert(insertSCAttendList);

                    Trace.WriteLine("寫入修課記錄時間：" + (Environment.TickCount - t1).ToString());
                }
                #endregion

                #region 判斷是否進行智慧型開課
                if (opt.CopyOptionEnabled)
                {
                    CourseInstructCollection ciCollection = new CourseInstructCollection();
                    CourseInstructCollection currentCICollection = new CourseInstructCollection();
                    List<string> copyCourseIDs = new List<string>();
                    foreach (JHCourseRecord course in copySourceCourses.Values)
                        copyCourseIDs.Add(course.ID);
                    foreach (JHTCInstructRecord tc in JHTCInstruct.SelectByTeacherIDAndCourseID(new string[] { }, copyCourseIDs))
                    {
                        if (!ciCollection.ContainsKey(tc.RefCourseID))
                            ciCollection.Add(tc.RefCourseID, new List<JHTCInstructRecord>());
                        ciCollection[tc.RefCourseID].Add(tc);
                    }
                    foreach (JHTCInstructRecord tc in JHTCInstruct.SelectByTeacherIDAndCourseID(new string[] { }, courseIDs))
                    {
                        if (!currentCICollection.ContainsKey(tc.RefCourseID))
                            currentCICollection.Add(tc.RefCourseID, new List<JHTCInstructRecord>());
                        currentCICollection[tc.RefCourseID].Add(tc);
                    }

                    List<JHTCInstructRecord> insertTCList = new List<JHTCInstructRecord>();
                    List<JHTCInstructRecord> updateTCList = new List<JHTCInstructRecord>();

                    List<JHCourseRecord> updateCourseList = new List<JHCourseRecord>();

                    //針對目前這個班級在開課學年度學期的所有課程
                    foreach (JHCourseRecord course in reloadedExistSubjects.Values)
                    {
                        //如果課程不存在課程規劃表中，則不處理
                        if (!courseDict.ContainsKey(course.Subject)) continue;

                        //複製來源課程中，如果有相同科目名稱，則進行複製
                        if (copySourceCourses.ContainsKey(course.Subject))
                        {
                            JHCourseRecord copyCourseRecord = copySourceCourses[course.Subject];

                            #region 自動加入授課教師
                            if (opt.CopyTeacher == true)
                            {
                                for (int i = 1; i <= 3; i++)
                                {
                                    //取得來源課程的授課教師
                                    JHTeacherRecord teacherRecord = ciCollection.GetTeacher(copyCourseRecord, i);
                                    if (teacherRecord != null)
                                    {
                                        //取得開課課程的授課記錄
                                        JHTCInstructRecord tc = currentCICollection.GetInstruct(course, i);
                                        if (tc == null)
                                        {
                                            tc = new JHTCInstructRecord();
                                            tc.RefCourseID = course.ID;
                                            tc.RefTeacherID = teacherRecord.ID;
                                            tc.Sequence = i;
                                            insertTCList.Add(tc);
                                        }
                                        else
                                        {
                                            tc.RefTeacherID = teacherRecord.ID;
                                            updateTCList.Add(tc);
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region 自動加入評量設定
                            if (opt.CopyAssessmentSetup == true)
                            {
                                course.RefAssessmentSetupID = copyCourseRecord.RefAssessmentSetupID;
                                updateCourseList.Add(course);
                            }
                            #endregion
                        }
                    }

                    if (insertTCList.Count > 0)
                        JHTCInstruct.Insert(insertTCList);
                    if (updateTCList.Count > 0)
                        JHTCInstruct.Update(updateTCList);
                    if (updateCourseList.Count > 0)
                        JHCourse.Update(updateCourseList);
                }
                #endregion

                #endregion

                //回報進度
                Worker.ReportProgress((int)(classCount * 100d / totalClass), "正在進行開課...");
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// 取得在校生
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        private List<JHStudentRecord> GetInSchoolStudents(List<JHStudentRecord> students)
        {
            List<JHStudentRecord> in_students = new List<JHStudentRecord>();
            foreach (JHStudentRecord student in students)
            {
                if (student.Status == K12.Data.StudentRecord.StudentStatus.一般 ||
                    student.Status == K12.Data.StudentRecord.StudentStatus.輟學)
                    in_students.Add(student);
            }
            return in_students;
        }

        class ProgramPlanCollection : Dictionary<string, JHProgramPlanRecord>
        {
            public JHProgramPlanRecord GetProgramPlanRecord(JHStudentRecord student)
            {
                if (student == null) return null;

                if (string.IsNullOrEmpty(student.OverrideProgramPlanID))
                {
                    if (student.Class != null)
                        return GetProgramPlanRecord(student.Class);
                }
                else if (ContainsKey(student.OverrideProgramPlanID))
                    return this[student.OverrideProgramPlanID];

                return null;
            }

            public JHProgramPlanRecord GetProgramPlanRecord(JHClassRecord cla)
            {
                if (cla == null) return null;

                if (!string.IsNullOrEmpty(cla.RefProgramPlanID) && ContainsKey(cla.RefProgramPlanID))
                    return this[cla.RefProgramPlanID];
                else
                    return null;
            }
        }

        class CourseAttendCollection : Dictionary<string, List<JHSCAttendRecord>>
        {
            public List<JHStudentRecord> GetAttendStudents(JHCourseRecord course)
            {
                List<JHStudentRecord> list = new List<JHStudentRecord>();
                if (ContainsKey(course.ID))
                {
                    foreach (JHSCAttendRecord sca in this[course.ID])
                        list.Add(JHStudent.SelectByID(sca.RefStudentID));
                }
                return list;
            }
        }

        class CourseInstructCollection : Dictionary<string, List<JHTCInstructRecord>>
        {
            public JHTeacherRecord GetTeacher(JHCourseRecord course, int sequence)
            {
                if (!ContainsKey(course.ID)) return null;
                foreach (JHTCInstructRecord record in this[course.ID])
                    if (record.Sequence == sequence)
                        return JHTeacher.SelectByID(record.RefTeacherID);
                return null;
            }

            public JHTCInstructRecord GetInstruct(JHCourseRecord course, int sequence)
            {
                if (!ContainsKey(course.ID)) return null;
                foreach (JHTCInstructRecord record in this[course.ID])
                    if (record.Sequence == sequence)
                        return record;
                return null;
            }
        }
    }
}
