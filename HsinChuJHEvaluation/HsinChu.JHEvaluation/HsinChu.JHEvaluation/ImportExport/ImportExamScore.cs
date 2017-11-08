using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartSchool.API.PlugIn;
using System.Threading;
using Framework;
using JHSchool.Data;
using HsinChu.JHEvaluation.Data;

namespace HsinChu.JHEvaluation.ImportExport
{
    class ImportExamScore : SmartSchool.API.PlugIn.Import.Importer
    {
        public ImportExamScore()
        {
            this.Image = null;
            this.Text = "匯入評量成績";
        }

        public override void InitializeImport(SmartSchool.API.PlugIn.Import.ImportWizard wizard)
        {
            //學生資訊 key: studentID
            Dictionary<string, JHStudentRecord> students = new Dictionary<string, JHSchool.Data.JHStudentRecord>();
            //學生修課資訊 studentID -> List:SCAttendRecord
            Dictionary<string, List<JHSCAttendRecord>> scattends = new Dictionary<string, List<JHSchool.Data.JHSCAttendRecord>>();
            //學生修習的課程 courseID -> CourseRecord
            Dictionary<string, JHCourseRecord> courses = new Dictionary<string, JHSchool.Data.JHCourseRecord>();
            //所有課程(依學年度學期分開) schoolYear_semester -> (courseName -> CourseRecord)
            Dictionary<string, Dictionary<string, JHCourseRecord>> allcourses = new Dictionary<string, Dictionary<string, JHSchool.Data.JHCourseRecord>>();
            //學生修習的課程對應的評量設定細節
            Dictionary<string, List<HC.JHAEIncludeRecord>> courseAe = new Dictionary<string, List<HC.JHAEIncludeRecord>>();
            //學生的評量成績記錄
            Dictionary<string, List<HC.JHSCETakeRecord>> existSces = new Dictionary<string, List<HC.JHSCETakeRecord>>();
            //所有試別
            Dictionary<string, JHExamRecord> exams = new Dictionary<string, JHSchool.Data.JHExamRecord>();

            wizard.PackageLimit = 3000;
            wizard.ImportableFields.AddRange("學年度", "學期", "課程名稱", "評量名稱", "定期分數", "平時分數", "文字描述");
            wizard.RequiredFields.AddRange("學年度", "學期", "課程名稱", "評量名稱");

            wizard.ValidateStart += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateStartEventArgs e)
            {
                #region 取得學生資訊
                foreach (JHStudentRecord stu in JHStudent.SelectByIDs(e.List))
                {
                    if (!students.ContainsKey(stu.ID))
                        students.Add(stu.ID, stu);
                }
                #endregion

                #region 取得修課記錄
                MultiThreadWorker<string> loader1 = new MultiThreadWorker<string>();
                loader1.MaxThreads = 3;
                loader1.PackageSize = 250;
                loader1.PackageWorker += delegate(object sender1, PackageWorkEventArgs<string> e1)
                {
                    foreach (JHSCAttendRecord record in JHSCAttend.SelectByStudentIDAndCourseID(e1.List, new string[] { }))
                    {
                        if (!scattends.ContainsKey(record.RefStudentID))
                            scattends.Add(record.RefStudentID, new List<JHSCAttendRecord>());
                        scattends[record.RefStudentID].Add(record);

                        if (!courses.ContainsKey(record.RefCourseID))
                            courses.Add(record.RefCourseID, null);
                    }
                };
                loader1.Run(e.List);
                #endregion

                #region 取得課程資訊
                MultiThreadWorker<string> loader2 = new MultiThreadWorker<string>();
                loader2.MaxThreads = 3;
                loader2.PackageSize = 250;
                loader2.PackageWorker += delegate(object sender2, PackageWorkEventArgs<string> e2)
                {
                    foreach (JHCourseRecord record in JHCourse.SelectByIDs(new List<string>(e2.List)))
                    {
                        if (courses.ContainsKey(record.ID))
                            courses[record.ID] = record;
                    }
                };
                loader2.Run(courses.Keys);

                foreach (JHCourseRecord course in JHCourse.SelectAll())
                {
                    string key = course.SchoolYear + "_" + course.Semester;
                    if (!allcourses.ContainsKey(key))
                        allcourses.Add(key, new Dictionary<string, JHCourseRecord>());
                    if (!allcourses[key].ContainsKey(course.Name))
                        allcourses[key].Add(course.Name, course);
                }
                #endregion

                #region 取得目前評量成績記錄

                MultiThreadWorker<string> loader3 = new MultiThreadWorker<string>();
                loader3.MaxThreads = 3;
                loader3.PackageSize = 250;
                loader3.PackageWorker += delegate(object sender3, PackageWorkEventArgs<string> e3)
                {
                    foreach (HC.JHSCETakeRecord sce in JHSCETake.SelectByStudentIDs(e3.List).AsHCJHSCETakeRecords())
                    {
                        if (!existSces.ContainsKey(sce.RefSCAttendID))
                            existSces.Add(sce.RefSCAttendID, new List<HC.JHSCETakeRecord>());
                        existSces[sce.RefSCAttendID].Add(sce);
                    }
                };
                loader3.Run(e.List);
                #endregion

                #region 取得評量設定
                foreach (HC.JHAEIncludeRecord ae in JHAEInclude.SelectAll().AsHCJHAEIncludeRecords())
                {
                    if (!courseAe.ContainsKey(ae.RefAssessmentSetupID))
                        courseAe.Add(ae.RefAssessmentSetupID, new List<HC.JHAEIncludeRecord>());
                    courseAe[ae.RefAssessmentSetupID].Add(ae);
                }
                #endregion

                #region 取得試別
                foreach (JHExamRecord exam in JHExam.SelectAll())
                {
                    if (!exams.ContainsKey(exam.ID))
                        exams.Add(exam.ID, exam);
                }
                #endregion
            };

            wizard.ValidateRow += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateRowEventArgs e)
            {
                int i;
                decimal d;

                #region 檢查學生是否存在
                JHStudentRecord student = null;
                if (students.ContainsKey(e.Data.ID))
                {
                    student = students[e.Data.ID];
                }
                else
                {
                    e.ErrorMessage = "壓根就沒有這個學生" + e.Data.ID;
                    return;
                }
                #endregion

                #region 驗證各個欄位格式
                bool inputFormatPass = true;
                foreach (string field in e.SelectFields)
                {
                    string value = e.Data[field];
                    switch (field)
                    {
                        default:
                            break;
                        case "學年度":
                        case "學期":
                            if (value == "" || !int.TryParse(value, out i))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入整數");
                            }
                            break;
                        case "課程名稱":
                            if (value == "")
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入課程名稱");
                            }
                            break;
                        case "評量名稱":
                            if (value == "")
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入評量名稱");
                            }
                            break;
                        case "定期分數":
                        case "平時分數":
                            if (value != "" && !decimal.TryParse(value, out d))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入空白或數值");
                            }
                            break;
                        //case "努力程度":
                        //    if (value != "" && !int.TryParse(value, out i))
                        //    {
                        //        inputFormatPass &= false;
                        //        e.ErrorFields.Add(field, "必須填入空白或整數");
                        //    }
                        //    break;
                        case "文字描述":
                            break;
                    }
                }
                #endregion

                //輸入格式正確才會針對情節做檢驗
                #region 驗證各種情節
                if (inputFormatPass)
                {
                    string errorMessage = "";

                    string sy = e.Data["學年度"];
                    string se = e.Data["學期"];
                    string key = e.Data.ID + "_" + sy + "_" + se;
                    string courseName = e.Data["課程名稱"];
                    string semsKey = sy + "_" + se;
                    string examName = e.Data["評量名稱"];

                    //int schoolyear = Framework.Int.ParseInt(sy);
                    //int semester = Framework.Int.ParseInt(se);

                    #region 檢查課程是否存在系統中
                    bool noCourse = false;
                    if (!allcourses.ContainsKey(semsKey))
                    {
                        noCourse = true;
                        errorMessage += (errorMessage == "" ? "" : "\n") + " 系統中找不到該課程";
                    }
                    else if (!allcourses[semsKey].ContainsKey(courseName))
                    {
                        noCourse = true;
                        errorMessage += (errorMessage == "" ? "" : "\n") + " 系統中找不到該課程";
                    }
                    else
                    {
                    }
                    #endregion

                    #region 檢查學生是否有修此課程 & 評量是否存在
                    bool attended = false;

                    JHCourseRecord attendCourse = null;
                    if (scattends.ContainsKey(e.Data.ID))
                    {
                        foreach (JHSCAttendRecord record in scattends[e.Data.ID])
                        {
                            //if (courses[record.RefCourseID].Name == courseName)
                            //    attendCourse = courses[record.RefCourseID];
                            bool HasRec = false;

                            // 當有學年度學期課程名稱相同
                            if (courses[record.RefCourseID].Name == courseName && courses[record.RefCourseID].SchoolYear.HasValue && courses[record.RefCourseID].Semester.HasValue)
                            {
                                if ((courses[record.RefCourseID].SchoolYear.Value.ToString().Trim() == sy.Trim()) && courses[record.RefCourseID].Semester.Value.ToString().Trim() == se.Trim())
                                    HasRec = true;
                            }
                            if (HasRec && courses.ContainsKey(record.RefCourseID))
                                attendCourse = courses[record.RefCourseID];

                        }
                    }
                    else //學生沒修半堂課
                    {
                    }

                    if (attendCourse == null && !noCourse)
                    {
                        if (!e.ErrorFields.ContainsKey("無修課記錄"))
                            e.ErrorFields.Add("無修課記錄", "學生在此學期並無修習此課程");
                    }
                    else if (attendCourse != null)
                    {
                        #region 驗證評量是否存在
                        if (string.IsNullOrEmpty(attendCourse.RefAssessmentSetupID))
                        {
                            if (!e.ErrorFields.ContainsKey("無評量設定"))
                                e.ErrorFields.Add("無評量設定", "課程(" + attendCourse.Name + ")無評量設定");
                        }
                        else
                        {
                            if (!courseAe.ContainsKey(attendCourse.RefAssessmentSetupID))
                            {
                                if (!e.ErrorFields.ContainsKey("無評量設定"))
                                    e.ErrorFields.Add("無評量設定", "課程(" + attendCourse.Name + ")無評量設定");
                            }
                            else
                            {
                                bool examValid = false;
                                foreach (HC.JHAEIncludeRecord ae in courseAe[attendCourse.RefAssessmentSetupID])
                                {
                                    if (!exams.ContainsKey(ae.RefExamID)) continue;

                                    if (exams[ae.RefExamID].Name == examName)
                                        examValid = true;
                                }

                                if (!examValid)
                                {
                                    if (!e.ErrorFields.ContainsKey("評量名稱無效"))
                                        e.ErrorFields.Add("評量名稱無效", "評量名稱(" + examName + ")不存在系統中");
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                    e.ErrorMessage = errorMessage;
                }
                #endregion
            };

            wizard.ImportPackage += delegate(object sender, SmartSchool.API.PlugIn.Import.ImportPackageEventArgs e)
            {
                Dictionary<string, List<RowData>> id_Rows = new Dictionary<string, List<RowData>>();

                #region 分包裝
                foreach (RowData data in e.Items)
                {
                    if (!id_Rows.ContainsKey(data.ID))
                        id_Rows.Add(data.ID, new List<RowData>());
                    id_Rows[data.ID].Add(data);
                }
                #endregion

                List<HC.JHSCETakeRecord> insertList = new List<HC.JHSCETakeRecord>();
                List<HC.JHSCETakeRecord> updateList = new List<HC.JHSCETakeRecord>();

                //交叉比對各學生資料
                #region 交叉比對各學生資料
                foreach (string id in id_Rows.Keys)
                {
                    JHStudentRecord studentRec = students[id];

                    foreach (RowData data in id_Rows[id])
                    {
                        string examName = data["評量名稱"];
                        string courseName = data["課程名稱"];
                        string SchoolYear = data["學年度"];
                        string Semester = data["學期"];


                        if (!scattends.ContainsKey(id)) continue;

                        foreach (JHSCAttendRecord record in scattends[id])
                        {
                            if (!courses.ContainsKey(record.RefCourseID)) continue;
                            JHCourseRecord course = courses[record.RefCourseID];
                            //if (course.Name != courseName) continue;

                            HC.JHSCETakeRecord currentSCE = null;


                            string sy = "", ss = "";
                            if (course.SchoolYear.HasValue)
                                sy = course.SchoolYear.Value.ToString();
                            if (course.Semester.HasValue)
                                ss = course.Semester.Value.ToString();

                            if (SchoolYear != sy || Semester != ss || courseName != course.Name)
                                continue;


                            if (SchoolYear == sy && Semester == ss && course.Name == courseName)
                            {
                                if (existSces.ContainsKey(record.ID))
                                {
                                    foreach (HC.JHSCETakeRecord sce in existSces[record.ID])
                                    {
                                        if (!exams.ContainsKey(sce.RefExamID)) continue;

                                        if (exams[sce.RefExamID].Name == examName)
                                            currentSCE = sce;
                                    }
                                }
                            }

                            if (currentSCE != null)
                            {
                                bool changed = false;

                                #region 填入資料
                                foreach (string field in e.ImportFields)
                                {
                                    string value = data[field];
                                    switch (field)
                                    {
                                        case "定期分數":
                                            if ("" + currentSCE.Score != value)
                                            {
                                                decimal d;
                                                if (decimal.TryParse(value, out d))
                                                    currentSCE.Score = d;
                                                else
                                                    currentSCE.Score = null;
                                                changed = true;
                                            }
                                            break;
                                        case "平時分數":
                                            if ("" + currentSCE.AssignmentScore != value)
                                            {
                                                decimal d;
                                                if (decimal.TryParse(value, out d))
                                                    currentSCE.AssignmentScore = d;
                                                else
                                                    currentSCE.AssignmentScore = null;
                                                changed = true;
                                            }
                                            break;
                                        //case "努力程度":
                                        //    if ("" + currentSCE.Effort != value)
                                        //    {
                                        //        int i;
                                        //        if (int.TryParse(value, out i))
                                        //            currentSCE.Effort = i;
                                        //        else
                                        //            currentSCE.Effort = null;
                                        //        changed = true;
                                        //    }
                                        //    break;
                                        case "文字描述":
                                            if (currentSCE.Text != value)
                                            {
                                                currentSCE.Text = value;
                                                changed = true;
                                            }
                                            break;
                                    }
                                }
                                #endregion

                                if (changed)
                                    updateList.Add(currentSCE);
                            }
                            else
                            {
                                HC.JHSCETakeRecord newSCE = new HC.JHSCETakeRecord(new JHSCETakeRecord());
                                newSCE.RefStudentID = id;
                                newSCE.RefSCAttendID = record.ID;
                                newSCE.RefCourseID = record.RefCourseID;

                                foreach (JHExamRecord exam in exams.Values)
                                {
                                    if (exam.Name == examName)
                                        newSCE.RefExamID = exam.ID;
                                }

                                #region 填入資料
                                foreach (string field in e.ImportFields)
                                {
                                    string value = data[field];
                                    switch (field)
                                    {
                                        case "定期分數":
                                            if (value != "")
                                            {
                                                decimal d;
                                                if (decimal.TryParse(value, out d))
                                                    newSCE.Score = d;
                                                else
                                                    newSCE.Score = null;
                                            }
                                            else
                                                newSCE.Score = null;
                                            break;
                                        case "平時分數":
                                            if (value != "")
                                            {
                                                decimal d;
                                                if (decimal.TryParse(value, out d))
                                                    newSCE.AssignmentScore = d;
                                                else
                                                    newSCE.AssignmentScore = null;
                                            }
                                            else
                                                newSCE.AssignmentScore = null;
                                            break;
                                        case "文字描述":
                                            newSCE.Text = value;
                                            break;
                                    }
                                }
                                #endregion

                                if (newSCE.RefExamID != "")
                                    insertList.Add(newSCE);
                            }
                        }
                    }
                }

                try
                {
                    if (updateList.Count > 0)
                    {
                        #region 分批次兩路上傳
                        List<List<HC.JHSCETakeRecord>> updatePackages = new List<List<HC.JHSCETakeRecord>>();
                        List<List<HC.JHSCETakeRecord>> updatePackages2 = new List<List<HC.JHSCETakeRecord>>();
                        {
                            List<HC.JHSCETakeRecord> package = null;
                            int count = 0;
                            foreach (HC.JHSCETakeRecord var in updateList)
                            {
                                if (count == 0)
                                {
                                    package = new List<HC.JHSCETakeRecord>(30);
                                    count = 30;
                                    if ((updatePackages.Count & 1) == 0)
                                        updatePackages.Add(package);
                                    else
                                        updatePackages2.Add(package);
                                }
                                package.Add(var);
                                count--;
                            }
                        }
                        Thread threadUpdateSemesterSubjectScore = new Thread(new ParameterizedThreadStart(Update));
                        threadUpdateSemesterSubjectScore.IsBackground = true;
                        threadUpdateSemesterSubjectScore.Start(updatePackages);
                        Thread threadUpdateSemesterSubjectScore2 = new Thread(new ParameterizedThreadStart(Update));
                        threadUpdateSemesterSubjectScore2.IsBackground = true;
                        threadUpdateSemesterSubjectScore2.Start(updatePackages2);

                        threadUpdateSemesterSubjectScore.Join();
                        threadUpdateSemesterSubjectScore2.Join();
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                }

                if (insertList.Count > 0)
                {
                    #region 分批次兩路上傳

                    List<List<HC.JHSCETakeRecord>> insertPackages = new List<List<HC.JHSCETakeRecord>>();
                    List<List<HC.JHSCETakeRecord>> insertPackages2 = new List<List<HC.JHSCETakeRecord>>();
                    {
                        List<HC.JHSCETakeRecord> package = null;
                        int count = 0;
                        foreach (HC.JHSCETakeRecord var in insertList)
                        {
                            if (count == 0)
                            {
                                package = new List<HC.JHSCETakeRecord>(30);
                                count = 30;
                                if ((insertPackages.Count & 1) == 0)
                                    insertPackages.Add(package);
                                else
                                    insertPackages2.Add(package);
                            }
                            package.Add(var);
                            count--;
                        }
                    }
                    Thread threadInsertSemesterSubjectScore = new Thread(new ParameterizedThreadStart(Insert));
                    threadInsertSemesterSubjectScore.IsBackground = true;
                    threadInsertSemesterSubjectScore.Start(insertPackages);
                    Thread threadInsertSemesterSubjectScore2 = new Thread(new ParameterizedThreadStart(Insert));
                    threadInsertSemesterSubjectScore2.IsBackground = true;
                    threadInsertSemesterSubjectScore2.Start(insertPackages2);

                    threadInsertSemesterSubjectScore.Join();
                    threadInsertSemesterSubjectScore2.Join();
                    #endregion
                }

                FISCA.LogAgent.ApplicationLog.Log("成績系統.匯入匯出", "匯入評量成績", "總共匯入" + (insertList.Count + updateList.Count) + "筆評量成績。");
                #endregion
            };
        }

        private void Update(object item)
        {
            try
            {
                List<List<HC.JHSCETakeRecord>> updatePackages = (List<List<HC.JHSCETakeRecord>>)item;
                foreach (List<HC.JHSCETakeRecord> package in updatePackages)
                {
                    JHSCETake.Update(package.AsJHSCETakeRecords());
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Insert(object item)
        {
            List<List<HC.JHSCETakeRecord>> insertPackages = (List<List<HC.JHSCETakeRecord>>)item;
            foreach (List<HC.JHSCETakeRecord> package in insertPackages)
            {
                JHSCETake.Insert(package.AsJHSCETakeRecords());
            }
        }
    }
}
