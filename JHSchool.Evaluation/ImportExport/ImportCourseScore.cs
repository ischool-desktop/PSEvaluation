using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using System.ComponentModel;
using SmartSchool.API.PlugIn;
using System.Threading;

namespace JHSchool.Evaluation.ImportExport
{
    class ImportCourseScore : SmartSchool.API.PlugIn.Import.Importer
    {
        public ImportCourseScore()
        {
            this.Image = null;
            this.Text = "匯入課程成績";
        }

        public override void InitializeImport(SmartSchool.API.PlugIn.Import.ImportWizard wizard)
        {
            //學生資訊
            Dictionary<string, Data.JHStudentRecord> students = new Dictionary<string, JHSchool.Data.JHStudentRecord>();
            //學生修課資訊 studentID -> List:SCAttendRecord
            Dictionary<string, List<Data.JHSCAttendRecord>> scattends = new Dictionary<string, List<JHSchool.Data.JHSCAttendRecord>>();
            //學生修習的課程 courseID -> CourseRecord
            Dictionary<string, Data.JHCourseRecord> courses = new Dictionary<string, JHSchool.Data.JHCourseRecord>();
            //所有課程(依學年度學期分開) schoolYear_semester -> (courseName -> CourseRecord)
            Dictionary<string, Dictionary<string, Data.JHCourseRecord>> allcourses = new Dictionary<string, Dictionary<string, JHSchool.Data.JHCourseRecord>>();
            //studentID_schoolYear_semester -> List:courseName
            Dictionary<string, List<string>> semesterCourseName = new Dictionary<string, List<string>>();
            //準備加入修課的資料 studentID -> (schoolYear_semester_courseName -> RowData)
            Dictionary<string, Dictionary<string, RowData>> prepareAttends = new Dictionary<string, Dictionary<string, RowData>>();

            wizard.PackageLimit = 3000;
            wizard.ImportableFields.AddRange("學年度", "學期", "課程名稱", "分數評量", "努力程度", "文字描述");
            wizard.RequiredFields.AddRange("學年度", "學期", "課程名稱");

            wizard.ValidateStart += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateStartEventArgs e)
            {
                #region 取得學生資訊
                foreach (Data.JHStudentRecord stu in Data.JHStudent.SelectByIDs(e.List))
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
                    foreach (Data.JHSCAttendRecord record in Data.JHSCAttend.SelectByStudentIDAndCourseID(e1.List, new string[] { }))
                    {
                        if (!scattends.ContainsKey(record.RefStudentID))
                            scattends.Add(record.RefStudentID, new List<JHSchool.Data.JHSCAttendRecord>());
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
                    foreach (Data.JHCourseRecord record in Data.JHCourse.SelectByIDs(new List<string>(e2.List)))
                    {
                        if (courses.ContainsKey(record.ID))
                            courses[record.ID] = record;
                    }
                };
                loader2.Run(courses.Keys);

                foreach (Data.JHCourseRecord course in Data.JHCourse.SelectAll())
                {
                    string key = course.SchoolYear + "_" + course.Semester;
                    if (!allcourses.ContainsKey(key))
                        allcourses.Add(key, new Dictionary<string, JHSchool.Data.JHCourseRecord>());
                    if (!allcourses[key].ContainsKey(course.Name))
                        allcourses[key].Add(course.Name, course);
                }
                #endregion
            };

            wizard.ValidateRow += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateRowEventArgs e)
            {
                int i;
                decimal d;

                #region 檢查學生是否存在
                Data.JHStudentRecord student = null;
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
                        case "分數評量":
                            if (value != "" && !decimal.TryParse(value, out d))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入空白或數值");
                            }
                            break;
                        case "努力程度":
                            if (value != "" && !int.TryParse(value, out i))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入空白或整數");
                            }
                            break;
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

                    //int schoolyear = Framework.Int.ParseInt(sy);
                    //int semester = Framework.Int.ParseInt(se);

                    #region 同一個學年度學期不能有重覆的課程名稱
                    if (!semesterCourseName.ContainsKey(key))
                        semesterCourseName.Add(key, new List<string>());
                    if (semesterCourseName[key].Contains(courseName))
                    {
                        errorMessage += (errorMessage == "" ? "" : "\n") + " 同一學年度學期不允許多筆相同課程名稱的資料";
                    }
                    else
                    {
                        semesterCourseName[key].Add(courseName);
                    }
                    #endregion

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

                    #region 檢查學生是否有修此課程
                    bool attended = false;

                    if (scattends.ContainsKey(e.Data.ID))
                    {
                        foreach (Data.JHSCAttendRecord record in scattends[e.Data.ID])
                        {
                            if (courses[record.RefCourseID].Name == courseName)
                                attended = true;
                        }
                    }
                    else //學生沒修半堂課
                    {
                    }

                    if (!attended && !noCourse)
                    {
                        if (!e.WarningFields.ContainsKey("無修課記錄"))
                            e.WarningFields.Add("無修課記錄", "學生在此學期並無修習此課程，將會新增修課記錄");

                        if (!prepareAttends.ContainsKey(e.Data.ID))
                            prepareAttends.Add(e.Data.ID, new Dictionary<string, RowData>());
                        if (!prepareAttends[e.Data.ID].ContainsKey(semsKey + "_" + courseName))
                            prepareAttends[e.Data.ID].Add(semsKey + "_" + courseName, e.Data);
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

                List<Data.JHSCAttendRecord> insertList = new List<JHSchool.Data.JHSCAttendRecord>();
                List<Data.JHSCAttendRecord> updateList = new List<JHSchool.Data.JHSCAttendRecord>();

                //交叉比對各學生資料
                #region 交叉比對各學生資料
                foreach (string id in id_Rows.Keys)
                {
                    Data.JHStudentRecord studentRec = students[id];

                    #region 處理要新增的修課記錄
                    if (prepareAttends.ContainsKey(id))
                    {
                        foreach (RowData data in prepareAttends[id].Values)
                        {
                            string sy = data["學年度"];
                            string se = data["學期"];
                            string semsKey = sy + "_" + se;
                            string courseName = data["課程名稱"];

                            if (allcourses.ContainsKey(semsKey) && allcourses[semsKey].ContainsKey(courseName))
                            {
                                Data.JHSCAttendRecord record = new JHSchool.Data.JHSCAttendRecord();
                                record.RefStudentID = id;
                                record.RefCourseID = allcourses[semsKey][courseName].ID;

                                #region 填入資料
                                foreach (string field in e.ImportFields)
                                {
                                    string value = data[field];
                                    switch (field)
                                    {
                                        case "分數評量":
                                            if (value != "")
                                            {
                                                decimal d;
                                                if (decimal.TryParse(value, out d))
                                                    record.Score = d;
                                                else
                                                    record.Score = null;
                                            }
                                            break;
                                        case "努力程度":
                                            if (value != "")
                                            {
                                                int i;
                                                if (int.TryParse(value, out i))
                                                    record.Effort = i;
                                                else
                                                    record.Effort = null;
                                            }
                                            break;
                                        case "文字描述":
                                            if (value != "")
                                            {
                                                record.Text = value;
                                            }
                                            break;
                                    }
                                }
                                #endregion

                                insertList.Add(record);
                            }
                        }
                    }
                    #endregion

                    #region 處理要修改的修課記錄
                    foreach (RowData data in id_Rows[id])
                    {
                        string sy = data["學年度"];
                        string se = data["學期"];
                        string semsKey = sy + "_" + se;
                        string courseName = data["課程名稱"];

                        //如果是新增修課就略過
                        if (prepareAttends.ContainsKey(id) && prepareAttends[id].ContainsKey(semsKey + "_" + courseName))
                            continue;

                        if (scattends.ContainsKey(id))
                        {
                            foreach (Data.JHSCAttendRecord record in scattends[id])
                            {
                                Data.JHCourseRecord course = courses[record.RefCourseID];
                                if (course.Name == courseName && "" + course.SchoolYear == sy && "" + course.Semester == se)
                                {
                                    #region 填入資料
                                    foreach (string field in e.ImportFields)
                                    {
                                        string value = data[field];
                                        switch (field)
                                        {
                                            case "分數評量":
                                                if (value != "")
                                                {
                                                    decimal d;
                                                    if (decimal.TryParse(value, out d))
                                                        record.Score = d;
                                                    else
                                                        record.Score = null;
                                                }
                                                break;
                                            case "努力程度":
                                                if (value != "")
                                                {
                                                    int i;
                                                    if (int.TryParse(value, out i))
                                                        record.Effort = i;
                                                    else
                                                        record.Effort = null;
                                                }
                                                break;
                                            case "文字描述":
                                                if (value != "")
                                                {
                                                    record.Text = value;
                                                }
                                                break;
                                        }
                                    }
                                    #endregion

                                    updateList.Add(record);
                                }
                            }
                        }
                    }
                    #endregion
                }

                try
                {
                    if (updateList.Count > 0)
                    {
                        #region 分批次兩路上傳
                        List<List<Data.JHSCAttendRecord>> updatePackages = new List<List<JHSchool.Data.JHSCAttendRecord>>();
                        List<List<Data.JHSCAttendRecord>> updatePackages2 = new List<List<JHSchool.Data.JHSCAttendRecord>>();
                        {
                            List<Data.JHSCAttendRecord> package = null;
                            int count = 0;
                            foreach (Data.JHSCAttendRecord var in updateList)
                            {
                                if (count == 0)
                                {
                                    package = new List<Data.JHSCAttendRecord>(30);
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

                    List<List<Data.JHSCAttendRecord>> insertPackages = new List<List<Data.JHSCAttendRecord>>();
                    List<List<Data.JHSCAttendRecord>> insertPackages2 = new List<List<Data.JHSCAttendRecord>>();
                    {
                        List<Data.JHSCAttendRecord> package = null;
                        int count = 0;
                        foreach (Data.JHSCAttendRecord var in insertList)
                        {
                            if (count == 0)
                            {
                                package = new List<Data.JHSCAttendRecord>(30);
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

                FISCA.LogAgent.ApplicationLog.Log("成績系統.匯入匯出", "匯入課程成績", "總共匯入" + (insertList.Count + updateList.Count) + "筆課程成績。");
                #endregion
            };
        }

        private void Update(object item)
        {
            try
            {
                List<List<Data.JHSCAttendRecord>> updatePackages = (List<List<Data.JHSCAttendRecord>>)item;
                foreach (List<Data.JHSCAttendRecord> package in updatePackages)
                {
                    Data.JHSCAttend.Update(package);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Insert(object item)
        {
            List<List<Data.JHSCAttendRecord>> insertPackages = (List<List<Data.JHSCAttendRecord>>)item;
            foreach (List<Data.JHSCAttendRecord> package in insertPackages)
            {
                Data.JHSCAttend.Insert(package);
            }
        }
    }
}
