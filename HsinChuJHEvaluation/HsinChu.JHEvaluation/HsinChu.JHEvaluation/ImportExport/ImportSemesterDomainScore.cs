using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.API.PlugIn;
using Framework;
using System.Xml;
using System.Threading;
using JHSchool.Data;
using JHSchool;

namespace HsinChu.JHEvaluation.ImportExport
{
    class ImportSemesterDomainScore : SmartSchool.API.PlugIn.Import.Importer
    {
        public ImportSemesterDomainScore()
        {
            this.Image = null;
            this.Text = "匯入學期領域成績";
        }

        public override void InitializeImport(SmartSchool.API.PlugIn.Import.ImportWizard wizard)
        {
            Dictionary<string, int> _ID_SchoolYear_Semester_GradeYear = new Dictionary<string, int>();
            Dictionary<string, List<string>> _ID_SchoolYear_Semester_Subject = new Dictionary<string, List<string>>();
            Dictionary<string, JHStudentRecord> _StudentCollection = new Dictionary<string, JHStudentRecord>();
            Dictionary<JHStudentRecord, Dictionary<int, decimal>> _StudentPassScore = new Dictionary<JHStudentRecord, Dictionary<int, decimal>>();
            Dictionary<string, List<JHSemesterScoreRecord>> semsDict = new Dictionary<string, List<JHSemesterScoreRecord>>();

            wizard.PackageLimit = 3000;
            //wizard.ImportableFields.AddRange("領域", "學年度", "學期", "權數", "節數", "分數評量", "努力程度", "文字描述", "註記");
            wizard.ImportableFields.AddRange("領域", "學年度", "學期", "權數", "節數", "成績", "原始成績", "補考成績", "文字描述", "註記");
            wizard.RequiredFields.AddRange("領域", "學年度", "學期");

            wizard.ValidateStart += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateStartEventArgs e)
            {
                #region ValidateStart
                _ID_SchoolYear_Semester_GradeYear.Clear();
                _ID_SchoolYear_Semester_Subject.Clear();
                _StudentCollection.Clear();

                List<JHStudentRecord> list = JHStudent.SelectByIDs(e.List);

                MultiThreadWorker<JHStudentRecord> loader = new MultiThreadWorker<JHStudentRecord>();
                loader.MaxThreads = 3;
                loader.PackageSize = 250;
                loader.PackageWorker += delegate(object sender1, PackageWorkEventArgs<JHStudentRecord> e1)
                {
                    foreach (var item in JHSemesterScore.SelectByStudents(e1.List))
                    {
                        if (!semsDict.ContainsKey(item.RefStudentID))
                            semsDict.Add(item.RefStudentID, new List<JHSchool.Data.JHSemesterScoreRecord>());
                        semsDict[item.RefStudentID].Add(item);
                    }
                };
                loader.Run(list);

                foreach (JHStudentRecord stu in list)
                {
                    if (!_StudentCollection.ContainsKey(stu.ID))
                        _StudentCollection.Add(stu.ID, stu);
                }
                #endregion
            };
            wizard.ValidateRow += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateRowEventArgs e)
            {
                #region ValidateRow
                int t;
                decimal d;
                JHStudentRecord student;
                if (_StudentCollection.ContainsKey(e.Data.ID))
                {
                    student = _StudentCollection[e.Data.ID];
                }
                else
                {
                    e.ErrorMessage = "壓根就沒有這個學生" + e.Data.ID;
                    return;
                }
                bool inputFormatPass = true;
                #region 驗各欄位填寫格式
                foreach (string field in e.SelectFields)
                {
                    string value = e.Data[field];
                    switch (field)
                    {
                        default:
                            break;
                        case "領域":
                            break;
                        case "權數":
                        case "節數":
                            if (e.Data["領域"] == "學習領域" || e.Data["領域"] == "課程學習")
                            {

                            }
                            else if (value == "" || !decimal.TryParse(value, out d))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入數值");
                            }
                            break;
                        case "學年度":
                            if (value == "" || !int.TryParse(value, out t))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入學年度");
                            }
                            break;
                        case "學期":
                            if (value == "" || !int.TryParse(value, out t) || t > 2 || t < 1)
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入1或2");
                            }
                            break;
                        case "成績":
                            if (value != "" && !decimal.TryParse(value, out d))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入空白或數值");
                            }
                            break;
                        case "原始成績":
                            if (value != "" && !decimal.TryParse(value, out d))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入空白或數值");
                            }
                            break;
                        case "補考成績":
                            if (value != "" && !decimal.TryParse(value, out d))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入空白或數值");
                            }
                            break;
                        //case "努力程度":
                        //    if (value != "" && !int.TryParse(value, out t))
                        //    {
                        //        inputFormatPass &= false;
                        //        e.ErrorFields.Add(field, "必須填入空白或數值");
                        //    }
                        //    break;
                    }
                }
                #endregion
                //輸入格式正確才會針對情節做檢驗
                if (inputFormatPass)
                {
                    string errorMessage = "";

                    string domain = e.Data["領域"];
                    string schoolYear = e.Data["學年度"];
                    string semester = e.Data["學期"];
                    int? sy = null;
                    int? se = null;
                    if (int.TryParse(schoolYear, out t))
                        sy = t;
                    if (int.TryParse(semester, out t))
                        se = t;
                    if (sy != null && se != null)
                    {
                        string key = e.Data.ID + "_" + sy + "_" + se;
                        #region 驗證學習領域及課程學習
                        if (domain == "學習領域" || domain == "課程學習")
                        {
                            bool invalidColumn = false;

                            //foreach (string col in new string[] { "權數", "節數", "努力程度", "文字描述", "註記" })
                            foreach (string col in new string[] { "權數", "節數", "補考成績", "文字描述", "註記" })
                            {
                                if (e.Data.ContainsKey(col) && e.Data[col] != "")
                                    invalidColumn = true;
                            }

                            if (invalidColumn)
                            {
                                if (!e.WarningFields.ContainsKey("欄位沒有作用"))
                                    e.WarningFields.Add("欄位沒有作用", "學習領域及課程學習不需要以下欄位：權數、節數、補考成績、文字描述、註記");
                                //e.WarningFields.Add("欄位沒有作用", "學習領域及課程學習不需要以下欄位：權數、節數、努力程度、文字描述、註記");
                            }
                        }

                        #endregion
                        #region 驗證重複科目資料
                        //string skey = subject + "_" + le;
                        string skey = domain;
                        if (!_ID_SchoolYear_Semester_Subject.ContainsKey(key))
                            _ID_SchoolYear_Semester_Subject.Add(key, new List<string>());
                        if (_ID_SchoolYear_Semester_Subject[key].Contains(skey))
                        {
                            errorMessage += (errorMessage == "" ? "" : "\n") + "同一學期不允許多筆相同領域的資料";
                        }
                        else
                            _ID_SchoolYear_Semester_Subject[key].Add(skey);
                        #endregion
                    }
                    e.ErrorMessage = errorMessage;
                }
                #endregion
            };

            wizard.ImportPackage += delegate(object sender, SmartSchool.API.PlugIn.Import.ImportPackageEventArgs e)
            {
                #region ImportPackage
                Dictionary<string, List<RowData>> id_Rows = new Dictionary<string, List<RowData>>();
                #region 分包裝
                foreach (RowData data in e.Items)
                {
                    if (!id_Rows.ContainsKey(data.ID))
                        id_Rows.Add(data.ID, new List<RowData>());
                    id_Rows[data.ID].Add(data);
                }
                #endregion
                List<JHSemesterScoreRecord> insertList = new List<JHSemesterScoreRecord>();
                List<JHSemesterScoreRecord> updateList = new List<JHSemesterScoreRecord>();
                //交叉比對各學生資料
                #region 交叉比對各學生資料
                foreach (string id in id_Rows.Keys)
                {
                    XmlDocument doc = new XmlDocument();
                    JHStudentRecord studentRec = _StudentCollection[id];
                    //該學生的學期科目成績
                    Dictionary<SemesterInfo, JHSemesterScoreRecord> semesterScoreDictionary = new Dictionary<SemesterInfo, JHSemesterScoreRecord>();
                    #region 整理現有的成績資料
                    List<JHSchool.Data.JHSemesterScoreRecord> semsList;
                    if (semsDict.ContainsKey(studentRec.ID))
                        semsList = semsDict[studentRec.ID];
                    else
                        semsList = new List<JHSchool.Data.JHSemesterScoreRecord>();
                    foreach (JHSemesterScoreRecord var in semsList)
                    {
                        SemesterInfo info = new SemesterInfo();
                        info.SchoolYear = var.SchoolYear;
                        info.Semester = var.Semester;

                        if (!semesterScoreDictionary.ContainsKey(info))
                            semesterScoreDictionary.Add(info, var);
                    }
                    #endregion

                    //要匯入的學期科目成績
                    Dictionary<SemesterInfo, Dictionary<string, RowData>> semesterImportScoreDictionary = new Dictionary<SemesterInfo, Dictionary<string, RowData>>();

                    #region 整理要匯入的資料
                    foreach (RowData row in id_Rows[id])
                    {
                        string domain = row["領域"];
                        string schoolYear = row["學年度"];
                        string semester = row["學期"];
                        int sy = int.Parse(schoolYear);
                        int se = int.Parse(semester);

                        SemesterInfo info = new SemesterInfo();
                        info.SchoolYear = sy;
                        info.Semester = se;

                        if (!semesterImportScoreDictionary.ContainsKey(info))
                            semesterImportScoreDictionary.Add(info, new Dictionary<string, RowData>());
                        if (!semesterImportScoreDictionary[info].ContainsKey(domain))
                            semesterImportScoreDictionary[info].Add(domain, row);
                    }
                    #endregion

                    //學期年級重整
                    //Dictionary<SemesterInfo, int> semesterGradeYear = new Dictionary<SemesterInfo, int>();
                    //要變更成績的學期
                    List<SemesterInfo> updatedSemester = new List<SemesterInfo>();
                    //在變更學期中新增加的成績資料
                    Dictionary<SemesterInfo, List<RowData>> updatedNewSemesterScore = new Dictionary<SemesterInfo, List<RowData>>();
                    //要增加成績的學期
                    Dictionary<SemesterInfo, List<RowData>> insertNewSemesterScore = new Dictionary<SemesterInfo, List<RowData>>();
                    //開始處理ImportScore
                    #region 開始處理ImportScore
                    foreach (SemesterInfo info in semesterImportScoreDictionary.Keys)
                    {
                        foreach (string domain in semesterImportScoreDictionary[info].Keys)
                        {
                            RowData data = semesterImportScoreDictionary[info][domain];
                            //如果是本來沒有這筆學期的成績就加到insertNewSemesterScore
                            if (!semesterScoreDictionary.ContainsKey(info))
                            {
                                if (!insertNewSemesterScore.ContainsKey(info))
                                    insertNewSemesterScore.Add(info, new List<RowData>());
                                insertNewSemesterScore[info].Add(data);
                            }
                            else
                            {
                                bool hasChanged = false;
                                //修改已存在的資料
                                if (semesterScoreDictionary[info].Domains.ContainsKey(domain) || domain == "學習領域" || domain == "課程學習")
                                {
                                    JHSemesterScoreRecord record = semesterScoreDictionary[info];

                                    #region 直接修改已存在的成績資料的Detail
                                    if (domain == "學習領域" || domain == "課程學習")
                                    {
                                        foreach (string field in e.ImportFields)
                                        {
                                            string value = data[field];
                                            switch (field)
                                            {
                                                default:
                                                    break;
                                                case "成績":
                                                    if (domain == "學習領域" && "" + record.LearnDomainScore != value)
                                                    {
                                                        decimal d;
                                                        if (decimal.TryParse(value, out d))
                                                            record.LearnDomainScore = d;
                                                        else
                                                            record.LearnDomainScore = null;
                                                        hasChanged = true;
                                                    }
                                                    if (domain == "課程學習" && "" + record.CourseLearnScore != value)
                                                    {
                                                        decimal d;
                                                        if (decimal.TryParse(value, out d))
                                                            record.CourseLearnScore = d;
                                                        else
                                                            record.CourseLearnScore = null;
                                                        hasChanged = true;
                                                    }
                                                    break;
                                                case "原始成績":
                                                    if (domain == "學習領域" && "" + record.LearnDomainScoreOrigin != value)
                                                    {
                                                        decimal d;
                                                        if (decimal.TryParse(value, out d))
                                                            record.LearnDomainScoreOrigin = d;
                                                        else
                                                            record.LearnDomainScoreOrigin = null;
                                                        hasChanged = true;
                                                    }
                                                    if (domain == "課程學習" && "" + record.CourseLearnScoreOrigin != value)
                                                    {
                                                        decimal d;
                                                        if (decimal.TryParse(value, out d))
                                                            record.CourseLearnScoreOrigin = d;
                                                        else
                                                            record.CourseLearnScoreOrigin = null;
                                                        hasChanged = true;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string field in e.ImportFields)
                                        {
                                            K12.Data.DomainScore score = record.Domains[domain];
                                            string value = data[field];
                                            //"分數評量", "努力程度", "文字描述", "註記"
                                            switch (field)
                                            {
                                                default:
                                                    break;
                                                case "領域":
                                                    if (score.Domain != value)
                                                    {
                                                        score.Domain = value;
                                                        hasChanged = true;
                                                    }
                                                    break;
                                                case "權數":
                                                    if ("" + score.Credit != value)
                                                    {
                                                        score.Credit = decimal.Parse(value);
                                                        hasChanged = true;
                                                    }
                                                    break;
                                                case "節數":
                                                    if ("" + score.Period != value)
                                                    {
                                                        score.Period = decimal.Parse(value);
                                                        hasChanged = true;
                                                    }
                                                    break;
                                                //case "成績年級":
                                                //    int gy = int.Parse(data["成績年級"]);
                                                //    if (record.GradeYear != gy)
                                                //    {
                                                //        semesterGradeYear[info] = gy;
                                                //        hasChanged = true;
                                                //    }
                                                //    break;
                                                case "成績":
                                                    if ("" + score.Score != value)
                                                    {
                                                        decimal d;
                                                        if (decimal.TryParse(value, out d))
                                                            score.Score = d;
                                                        else
                                                            score.Score = null;
                                                        hasChanged = true;
                                                    }
                                                    break;
                                                case "原始成績":
                                                    if ("" + score.ScoreOrigin != value)
                                                    {
                                                        decimal d;
                                                        if (decimal.TryParse(value, out d))
                                                            score.ScoreOrigin = d;
                                                        else
                                                            score.ScoreOrigin = null;
                                                        hasChanged = true;
                                                    }
                                                    break;
                                                case "補考成績":
                                                    if ("" + score.ScoreMakeup != value)
                                                    {
                                                        decimal d;
                                                        if (decimal.TryParse(value, out d))
                                                            score.ScoreMakeup = d;
                                                        else
                                                            score.ScoreMakeup = null;
                                                        hasChanged = true;
                                                    }
                                                    break;
                                                //case "努力程度":
                                                //    if ("" + score.Effort != value)
                                                //    {
                                                //        int i;
                                                //        if (int.TryParse(value, out i))
                                                //            score.Effort = i;
                                                //        else
                                                //            score.Effort = null;
                                                //        hasChanged = true;
                                                //    }
                                                //    break;
                                                case "文字描述":
                                                    if ("" + score.Text != value)
                                                    {
                                                        score.Text = value;
                                                        hasChanged = true;
                                                    }
                                                    break;
                                                case "註記":
                                                    if (score.Comment != value)
                                                    {
                                                        score.Comment = value;
                                                        hasChanged = true;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    #endregion

                                }
                                else//加入新成績至已存在的學期
                                {
                                    //加入新成績至已存在的學期
                                    if (!updatedNewSemesterScore.ContainsKey(info))
                                        updatedNewSemesterScore.Add(info, new List<RowData>());
                                    updatedNewSemesterScore[info].Add(data);
                                    hasChanged = true;
                                }
                                //真的有變更
                                if (hasChanged)
                                {
                                    #region 登錄有變更的學期
                                    if (!updatedSemester.Contains(info))
                                        updatedSemester.Add(info);
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion
                    //處理已登錄要更新的學期成績
                    #region 處理已登錄要更新的學期成績
                    foreach (SemesterInfo info in updatedSemester)
                    {
                        //Dictionary<int, Dictionary<int, string>> semeScoreID = (Dictionary<int, Dictionary<int, string>>)studentRec.Fields["SemesterSubjectScoreID"];
                        //string semesterScoreID = semeScoreID[sy][se];//從學期抓ID
                        //int gradeyear = semesterGradeYear[info];//抓年級
                        //XmlElement subjectScoreInfo = doc.CreateElement("SemesterSubjectScoreInfo");
                        #region 產生該學期科目成績的XML
                        //foreach (SemesterSubjectScoreInfo scoreInfo in semesterScoreDictionary[sy][se].Values)
                        //{
                        //    subjectScoreInfo.AppendChild(doc.ImportNode(scoreInfo.Detail, true));
                        //}

                        updateList.Add(semesterScoreDictionary[info]);

                        //if (updatedNewSemesterScore.ContainsKey(sy) && updatedNewSemesterScore[sy].ContainsKey(se))
                        if (updatedNewSemesterScore.ContainsKey(info))
                        {
                            foreach (RowData row in updatedNewSemesterScore[info])
                            {
                                //XmlElement newScore = doc.CreateElement("Subject");
                                JHSemesterScoreRecord record = semesterScoreDictionary[info];
                                K12.Data.DomainScore domainScore = null;

                                string domain = row["領域"];
                                if (domain == "學習領域" || domain == "課程學習")
                                {
                                    if (e.ImportFields.Contains("成績"))
                                    {
                                        if (domain == "學習領域")
                                        {
                                            decimal d;
                                            if (decimal.TryParse(row["成績"], out d))
                                                record.LearnDomainScore = d;
                                            else
                                                record.LearnDomainScore = null;
                                        }
                                        else if (domain == "課程學習")
                                        {
                                            decimal d;
                                            if (decimal.TryParse(row["成績"], out d))
                                                record.CourseLearnScore = d;
                                            else
                                                record.CourseLearnScore = null;
                                        }
                                    }
                                    if (e.ImportFields.Contains("原始成績"))
                                    {
                                        if (domain == "學習領域")
                                        {
                                            decimal d;
                                            if (decimal.TryParse(row["原始成績"], out d))
                                                record.LearnDomainScoreOrigin = d;
                                            else
                                                record.LearnDomainScoreOrigin = null;
                                        }
                                        else if (domain == "課程學習")
                                        {
                                            decimal d;
                                            if (decimal.TryParse(row["原始成績"], out d))
                                                record.CourseLearnScoreOrigin = d;
                                            else
                                                record.CourseLearnScoreOrigin = null;
                                        }
                                    }
                                }
                                else
                                {
                                    domainScore = new K12.Data.DomainScore();
                                    #region 建立newScore
                                    //foreach (string field in new string[] { "領域", "科目", "權數", "節數", "分數評量", "努力程度", "文字描述", "註記" })
                                    foreach (string field in new string[] { "領域", "科目", "權數", "節數", "成績", "原始成績", "補考成績", "文字描述", "註記" })
                                    {
                                        if (e.ImportFields.Contains(field))
                                        {
                                            #region 填入領域資訊
                                            string value = row[field];
                                            switch (field)
                                            {
                                                default:
                                                    break;
                                                case "領域":
                                                    domainScore.Domain = value;
                                                    break;
                                                case "權數":
                                                    domainScore.Credit = decimal.Parse(value);
                                                    break;
                                                case "節數":
                                                    domainScore.Period = decimal.Parse(value);
                                                    break;
                                                case "成績":
                                                    decimal d;
                                                    if (decimal.TryParse(value, out d))
                                                        domainScore.Score = d;
                                                    else
                                                        domainScore.Score = null;
                                                    break;
                                                case "原始成績":
                                                    decimal d1;
                                                    if (decimal.TryParse(value, out d1))
                                                        domainScore.ScoreOrigin = d1;
                                                    else
                                                        domainScore.ScoreOrigin = null;
                                                    break;
                                                case "補考成績":
                                                    decimal d2;
                                                    if (decimal.TryParse(value, out d2))
                                                        domainScore.ScoreMakeup = d2;
                                                    else
                                                        domainScore.ScoreMakeup = null;
                                                    break;
                                                //case "努力程度":
                                                //    int i;
                                                //    if (int.TryParse(value, out i))
                                                //        domainScore.Effort = i;
                                                //    else
                                                //        domainScore.Effort = null;
                                                //    break;
                                                case "文字描述":
                                                    domainScore.Text = value;
                                                    break;
                                                case "註記":
                                                    domainScore.Comment = value;
                                                    break;
                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion
                                }
                                //subjectScoreInfo.AppendChild(newScore);

                                if (domainScore != null)
                                {
                                    if (!record.Domains.ContainsKey(domainScore.Domain))
                                        record.Domains.Add(domainScore.Domain, domainScore);
                                    else
                                        record.Domains[domainScore.Domain] = domainScore;
                                }
                                updateList.Add(record);
                            }
                        }
                        #endregion
                        //updateList.Add(new SmartSchool.Feature.Score.EditScore.UpdateInfo(semesterScoreID, gradeyear, subjectScoreInfo));
                    }
                    #endregion
                    //處理新增成績學期
                    #region 處理新增成績學期
                    foreach (SemesterInfo info in insertNewSemesterScore.Keys)
                    {
                        //int gradeyear = semesterGradeYear[info];//抓年級
                        foreach (RowData row in insertNewSemesterScore[info])
                        {
                            JHSemesterScoreRecord record = new JHSchool.Data.JHSemesterScoreRecord();
                            K12.Data.DomainScore domainScore = null;

                            string domain = row["領域"];
                            if (domain == "學習領域" || domain == "課程學習")
                            {
                                if (e.ImportFields.Contains("成績"))
                                {
                                    if (domain == "學習領域")
                                    {
                                        decimal d;
                                        if (decimal.TryParse(row["成績"], out d))
                                            record.LearnDomainScore = d;
                                        else
                                            record.LearnDomainScore = null;
                                    }
                                    else if (domain == "課程學習")
                                    {
                                        decimal d;
                                        if (decimal.TryParse(row["成績"], out d))
                                            record.CourseLearnScore = d;
                                        else
                                            record.CourseLearnScore = null;
                                    }
                                }
                                if (e.ImportFields.Contains("原始成績"))
                                {
                                    if (domain == "學習領域")
                                    {
                                        decimal d;
                                        if (decimal.TryParse(row["原始成績"], out d))
                                            record.LearnDomainScoreOrigin = d;
                                        else
                                            record.LearnDomainScoreOrigin = null;
                                    }
                                    else if (domain == "課程學習")
                                    {
                                        decimal d;
                                        if (decimal.TryParse(row["原始成績"], out d))
                                            record.CourseLearnScoreOrigin = d;
                                        else
                                            record.CourseLearnScoreOrigin = null;
                                    }
                                }
                            }
                            else
                            {
                                domainScore = new K12.Data.DomainScore();
                                //foreach (string field in new string[] { "領域", "科目", "權數", "節數", "分數評量", "努力程度", "文字描述", "註記" })
                                foreach (string field in new string[] { "領域", "科目", "權數", "節數", "成績", "原始成績", "補考成績", "文字描述", "註記" })
                                {
                                    if (e.ImportFields.Contains(field))
                                    {
                                        #region 填入領域資訊
                                        string value = row[field];
                                        switch (field)
                                        {
                                            default: break;
                                            case "領域":
                                                domainScore.Domain = value;
                                                break;
                                            case "權數":
                                                domainScore.Credit = decimal.Parse(value);
                                                break;
                                            case "節數":
                                                domainScore.Period = decimal.Parse(value);
                                                break;
                                            case "成績":
                                                decimal d;
                                                if (decimal.TryParse(value, out d))
                                                    domainScore.Score = d;
                                                else
                                                    domainScore.Score = null;
                                                break;
                                            case "原始成績":
                                                decimal d1;
                                                if (decimal.TryParse(value, out d1))
                                                    domainScore.ScoreOrigin = d1;
                                                else
                                                    domainScore.ScoreOrigin = null;
                                                break;
                                            case "補考成績":
                                                decimal d2;
                                                if (decimal.TryParse(value, out d2))
                                                    domainScore.ScoreMakeup = d2;
                                                else
                                                    domainScore.ScoreMakeup = null;
                                                break;
                                            //case "努力程度":
                                            //    int i;
                                            //    if (int.TryParse(value, out i))
                                            //        domainScore.Effort = i;
                                            //    else
                                            //        domainScore.Effort = null;
                                            //    break;
                                            case "文字描述":
                                                domainScore.Text = value;
                                                break;
                                            case "註記":
                                                domainScore.Comment = value;
                                                break;
                                        }
                                        #endregion
                                    }
                                }
                            }
                            //subjectScoreInfo.AppendChild(newScore);
                            record.SchoolYear = info.SchoolYear;
                            record.Semester = info.Semester;
                            record.RefStudentID = studentRec.ID;
                            //record.GradeYear = gradeyear;

                            if (domainScore != null)
                            {
                                if (!record.Domains.ContainsKey(domainScore.Domain))
                                    record.Domains.Add(domainScore.Domain, domainScore);
                                else
                                    record.Domains[domainScore.Domain] = domainScore;
                            }
                            insertList.Add(record);
                        }
                        //insertList.Add(new SmartSchool.Feature.Score.AddScore.InsertInfo(studentRec.StudentID, "" + sy, "" + se, gradeyear, "", subjectScoreInfo));
                    }
                    #endregion
                }
                #endregion

                Dictionary<string, JHSemesterScoreRecord> iList = new Dictionary<string, JHSemesterScoreRecord>();
                Dictionary<string, JHSemesterScoreRecord> uList = new Dictionary<string, JHSemesterScoreRecord>();

                foreach (var record in insertList)
                {
                    string key = record.RefStudentID + "_" + record.SchoolYear + "_" + record.Semester;
                    if (!iList.ContainsKey(key))
                        iList.Add(key, new JHSchool.Data.JHSemesterScoreRecord());
                    JHSchool.Data.JHSemesterScoreRecord newRecord = iList[key];
                    newRecord.RefStudentID = record.RefStudentID;
                    newRecord.SchoolYear = record.SchoolYear;
                    newRecord.Semester = record.Semester;

                    foreach (var domain in record.Domains.Keys)
                    {
                        if (!newRecord.Domains.ContainsKey(domain))
                            newRecord.Domains.Add(domain, record.Domains[domain]);
                    }
                    if (record.CourseLearnScore.HasValue)
                        newRecord.CourseLearnScore = record.CourseLearnScore;
                    if (record.LearnDomainScore.HasValue)
                        newRecord.LearnDomainScore = record.LearnDomainScore;

                    if (record.CourseLearnScoreOrigin.HasValue)
                        newRecord.CourseLearnScoreOrigin = record.CourseLearnScoreOrigin;
                    if (record.LearnDomainScoreOrigin.HasValue)
                        newRecord.LearnDomainScoreOrigin = record.LearnDomainScoreOrigin;
                }

                foreach (var record in updateList)
                {
                    string key = record.RefStudentID + "_" + record.SchoolYear + "_" + record.Semester;
                    if (!uList.ContainsKey(key))
                        uList.Add(key, new JHSemesterScoreRecord());
                    JHSemesterScoreRecord newRecord = uList[key];
                    newRecord.RefStudentID = record.RefStudentID;
                    newRecord.SchoolYear = record.SchoolYear;
                    newRecord.Semester = record.Semester;
                    newRecord.ID = record.ID;

                    foreach (var domain in record.Domains.Keys)
                    {
                        if (!newRecord.Domains.ContainsKey(domain))
                            newRecord.Domains.Add(domain, record.Domains[domain]);
                    }
                    if (record.CourseLearnScore.HasValue)
                        newRecord.CourseLearnScore = record.CourseLearnScore;
                    if (record.LearnDomainScore.HasValue)
                        newRecord.LearnDomainScore = record.LearnDomainScore;

                    if (record.CourseLearnScoreOrigin.HasValue)
                        newRecord.CourseLearnScoreOrigin = record.CourseLearnScoreOrigin;
                    if (record.LearnDomainScoreOrigin.HasValue)
                        newRecord.LearnDomainScoreOrigin = record.LearnDomainScoreOrigin;
                }

                List<string> ids = new List<string>(id_Rows.Keys);
                Dictionary<string, JHSemesterScoreRecord> origs = new Dictionary<string, JHSemesterScoreRecord>();
                foreach (var record in JHSemesterScore.SelectByStudentIDs(ids))
                {
                    if (!origs.ContainsKey(record.ID))
                        origs.Add(record.ID, record);
                }
                foreach (var record in uList.Values)
                {
                    if (origs.ContainsKey(record.ID))
                    {
                        foreach (var subject in origs[record.ID].Subjects.Keys)
                        {
                            if (!record.Subjects.ContainsKey(subject))
                                record.Subjects.Add(subject, origs[record.ID].Subjects[subject]);
                        }
                    }
                }

                JHSemesterScore.Insert(new List<JHSemesterScoreRecord>(iList.Values));

                JHSemesterScore.Update(new List<JHSemesterScoreRecord>(uList.Values));

                FISCA.LogAgent.ApplicationLog.Log("成績系統.匯入匯出", "匯入學期領域成績", "總共匯入" + (insertList.Count + updateList.Count) + "筆學期領域成績。");
                #endregion
            };
            wizard.ImportComplete += delegate
            {
                MsgBox.Show("匯入完成");
            };
        }

        private void updateSemesterSubjectScore(object item)
        {
            List<List<JHSemesterScoreRecord>> updatePackages = (List<List<JHSemesterScoreRecord>>)item;
            foreach (List<JHSemesterScoreRecord> package in updatePackages)
            {
                JHSemesterScore.Update(package);
            }
        }

        private void insertSemesterSubjectScore(object item)
        {
            List<List<JHSemesterScoreRecord>> insertPackages = (List<List<JHSemesterScoreRecord>>)item;
            foreach (List<JHSemesterScoreRecord> package in insertPackages)
            {
                JHSemesterScore.Insert(package);
            }
        }
    }
}
