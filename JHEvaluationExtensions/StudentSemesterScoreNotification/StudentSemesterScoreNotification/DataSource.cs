using Campus.Report;
using FISCA.Data;
using FISCA.UDT;
using JHSchool.Behavior.BusinessLogic;
using JHSchool.Data;
using JHSchool.Evaluation.Mapping;
using K12.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace StudentSemesterScoreNotification
{
    class DataSource
    {
        private static AccessHelper _A;
        private static QueryHelper _Q;
        private static DegreeMapper _degreeMapper;

        private static List<string> _文字描述 = new List<string>();
        private static List<StudentRecord> _students;
        private static int _schoolYear, _semester;

        /// <summary>
        /// 補考成績加註
        /// </summary>
        private static string _ReExamMark = "";

        /// <summary>
        /// 使用者選擇成績類別
        /// </summary>
        private static string _UserSelScoreType = "";

        //DataRow catch
        private static Dictionary<string, DataRow> _RowCatchs = new Dictionary<string, DataRow>();

        /// <summary>
        /// 日常生活表現名稱對照使用
        /// </summary>
        private static Dictionary<string, string> _DLBehaviorConfigNameDict = new Dictionary<string, string>();

        /// <summary>
        /// 日常生活表現子項目名稱,呼叫GetDLBehaviorConfigNameDict 一同取得
        /// </summary>
        private static Dictionary<string, List<string>> _DLBehaviorConfigItemNameDict = new Dictionary<string, List<string>>();

        /// <summary>
        /// XML 內解析子項目名稱
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        private static List<string> ParseItems(XElement elm)
        {
            List<string> retVal = new List<string>();

            foreach (XElement subElm in elm.Elements("Item"))
            {
                // 因為社團功能，所以要將"社團活動" 字不放入
                string name = subElm.Attribute("Name").Value;
                if (name != "社團活動")
                    retVal.Add(name);
            }
            return retVal;
        }

        public static void SetReExamMark(string str)
        {
            _ReExamMark = str;
        }

        public static void SetUserSelScoreType(string str)
        {
            _UserSelScoreType = str;
        }


        /// <summary>
        /// 取得日常生活表現設定名稱
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> GetDLBehaviorConfigNameDict()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            try
            {
                _DLBehaviorConfigItemNameDict.Clear();

                // 包含新竹與高雄
                K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["DLBehaviorConfig"];
                if (!string.IsNullOrEmpty(cd["DailyBehavior"]))
                {
                    string key = "日常行為表現";
                    //日常行為表現
                    XElement e1 = XElement.Parse(cd["DailyBehavior"]);
                    string name = e1.Attribute("Name").Value;
                    retVal.Add(key, name);

                    // 日常生活表現子項目
                    List<string> items = ParseItems(e1);
                    if (items.Count > 0)
                        _DLBehaviorConfigItemNameDict.Add(key, items);
                }

                if (!string.IsNullOrEmpty(cd["GroupActivity"]))
                {
                    string key = "團體活動表現";
                    //團體活動表現
                    XElement e4 = XElement.Parse(cd["GroupActivity"]);
                    string name = e4.Attribute("Name").Value;
                    retVal.Add(key, name);

                    // 團體活動表現
                    List<string> items = ParseItems(e4);
                    if (items.Count > 0)
                        _DLBehaviorConfigItemNameDict.Add(key, items);

                }

                if (!string.IsNullOrEmpty(cd["PublicService"]))
                {
                    string key = "公共服務表現";
                    //公共服務表現
                    XElement e5 = XElement.Parse(cd["PublicService"]);
                    string name = e5.Attribute("Name").Value;
                    retVal.Add(key, name);
                    List<string> items = ParseItems(e5);
                    if (items.Count > 0)
                        _DLBehaviorConfigItemNameDict.Add(key, items);

                }

                if (!string.IsNullOrEmpty(cd["SchoolSpecial"]))
                {
                    string key = "校內外特殊表現";
                    //校內外特殊表現,新竹沒有子項目，高雄有子項目
                    XElement e6 = XElement.Parse(cd["SchoolSpecial"]);
                    string name = e6.Attribute("Name").Value;
                    retVal.Add(key, name);
                    List<string> items = ParseItems(e6);
                    if (items.Count > 0)
                        _DLBehaviorConfigItemNameDict.Add(key, items);
                }

                if (!string.IsNullOrEmpty(cd["OtherRecommend"]))
                {
                    //其他表現
                    XElement e2 = XElement.Parse(cd["OtherRecommend"]);
                    string name = e2.Attribute("Name").Value;
                    retVal.Add("其他表現", name);
                }

                if (!string.IsNullOrEmpty(cd["DailyLifeRecommend"]))
                {
                    //日常生活表現具體建議
                    XElement e3 = XElement.Parse(cd["DailyLifeRecommend"]);
                    string name = e3.Attribute("Name").Value;
                    retVal.Add("具體建議", name);  // 高雄
                    retVal.Add("綜合評語", name);  // 新竹
                }
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("日常生活表現設定檔解析失敗!" + ex.Message);
            }

            return retVal;
        }

        /// <summary>
        /// 取得一張初始化的資料表
        /// </summary>
        /// <returns></returns>
        private static DataTable GetEmptyDataTable()
        {
            _RowCatchs.Clear();
            _DLBehaviorConfigNameDict = GetDLBehaviorConfigNameDict();

            List<string> plist = K12.Data.PeriodMapping.SelectAll().Select(x => x.Type).Distinct().ToList();
            List<string> alist = K12.Data.AbsenceMapping.SelectAll().Select(x => x.Name).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("列印日期");
            dt.Columns.Add("學校名稱");
            dt.Columns.Add("學年度");
            dt.Columns.Add("學期");
            dt.Columns.Add("姓名");
            dt.Columns.Add("班級");
            dt.Columns.Add("座號");
            dt.Columns.Add("學號");
            dt.Columns.Add("大功");
            dt.Columns.Add("小功");
            dt.Columns.Add("嘉獎");
            dt.Columns.Add("大過");
            dt.Columns.Add("小過");
            dt.Columns.Add("警告");
            dt.Columns.Add("上課天數");
            dt.Columns.Add("學習領域成績");
            dt.Columns.Add("學習領域原始成績");
            dt.Columns.Add("課程學習成績");
            dt.Columns.Add("課程學習原始成績");
            dt.Columns.Add("班導師");
            dt.Columns.Add("教務主任");
            dt.Columns.Add("校長");
            dt.Columns.Add("服務學習時數");
            dt.Columns.Add("文字描述");

            //科目欄位
            for (int i = 1; i <= Global.SupportSubjectCount; i++)
            {
                dt.Columns.Add("S科目" + i);
                dt.Columns.Add("S領域" + i);
                dt.Columns.Add("S節數" + i);
                dt.Columns.Add("S權數" + i);
                dt.Columns.Add("S等第" + i);
                dt.Columns.Add("S成績" + i);
                dt.Columns.Add("S原始成績" + i);
                dt.Columns.Add("S補考成績" + i);
            }

            //領域欄位
            for (int i = 1; i <= Global.SupportDomainCount; i++)
            {
                dt.Columns.Add("D領域" + i);
                dt.Columns.Add("D節數" + i);
                dt.Columns.Add("D權數" + i);
                dt.Columns.Add("D等第" + i);
                dt.Columns.Add("D成績" + i);
                dt.Columns.Add("D原始成績" + i);
                dt.Columns.Add("D補考成績" + i);
            }

            // 指定領域合併
            foreach (string dName in Global.PriDomainNameList())
            {
                dt.Columns.Add(dName + "科目");
                dt.Columns.Add(dName + "領域");
                dt.Columns.Add(dName + "節數");
                dt.Columns.Add(dName + "權數");
                dt.Columns.Add(dName + "等第");
                dt.Columns.Add(dName + "成績");
                dt.Columns.Add(dName + "原始成績");
                dt.Columns.Add(dName + "補考成績");
            }

            // 只領領域科目合併
            foreach (string dName in Global.PriDomainNameList())
            {
                for (int j = 1; j <= 7; j++)
                {
                    dt.Columns.Add(dName + "科目" + j);
                    dt.Columns.Add(dName + "領域" + j);
                    dt.Columns.Add(dName + "節數" + j);
                    dt.Columns.Add(dName + "權數" + j);
                    dt.Columns.Add(dName + "等第" + j);
                    dt.Columns.Add(dName + "成績" + j);
                    dt.Columns.Add(dName + "原始成績" + j);
                    dt.Columns.Add(dName + "補考成績" + j);
                }
            }

            //假別欄位
            for (int i = 1; i <= Global.SupportAbsentCount; i++)
                dt.Columns.Add("列印假別" + i);

            //日常生活表現欄位
            foreach (string key in Global.DLBehaviorRef.Keys)
            {
                dt.Columns.Add(key + "_Name");
                dt.Columns.Add(key + "_Description");
            }

            //日常生活表現子項目欄位
            foreach (string key in _DLBehaviorConfigNameDict.Keys)
            {
                int itemIndex = 0;

                if (_DLBehaviorConfigItemNameDict.ContainsKey(key))
                {
                    foreach (string item in _DLBehaviorConfigItemNameDict[key])
                    {
                        itemIndex++;
                        dt.Columns.Add(key + "_Item_Name" + itemIndex);
                        dt.Columns.Add(key + "_Item_Degree" + itemIndex);
                        dt.Columns.Add(key + "_Item_Description" + itemIndex);
                    }
                }
            }

            //社團欄位
            for (int i = 1; i <= Global.SupportClubCount; i++)
            {
                dt.Columns.Add("社團Name" + i);
                dt.Columns.Add("社團Score" + i);
                dt.Columns.Add("社團Effort" + i);
                dt.Columns.Add("社團Text" + i);
            }

            //看ColumnName用的
            //List<string> dcs = new List<string>();

            //foreach (DataColumn dc in dt.Columns)
            //    dcs.Add(dc.ColumnName);

            // 加入體適能欄位
            for (int i = 1; i <= 8; i++)
            {
                dt.Columns.Add("身高" + i);
                dt.Columns.Add("體重" + i);
                dt.Columns.Add("坐姿體前彎" + i);
                dt.Columns.Add("坐姿體前彎常模" + i);
                dt.Columns.Add("立定跳遠" + i);
                dt.Columns.Add("立定跳遠常模" + i);
                dt.Columns.Add("仰臥起坐" + i);
                dt.Columns.Add("仰臥起坐常模" + i);
                dt.Columns.Add("心肺適能" + i);
                dt.Columns.Add("心肺適能常模" + i);
            }

            return dt;
        }

        private static string SelectTime() //取得Server的時間
        {
            DataTable dtable = _Q.Select("select now()"); //取得時間
            DateTime dt = DateTime.Now;
            DateTime.TryParse("" + dtable.Rows[0][0], out dt); //Parse資料
            string ComputerSendTime = dt.ToString("yyyy/MM/dd"); //最後時間

            return ComputerSendTime;
        }

        /// <summary>
        /// 填寫DataTable的資料
        /// </summary>
        /// <param name="dt"></param>
        private static void FillData(DataTable dt)
        {
            string printDateTime = SelectTime();
            string schoolName = K12.Data.School.ChineseName;
            string 校長 = K12.Data.School.Configuration["學校資訊"].PreviousData.SelectSingleNode("ChancellorChineseName").InnerText;
            string 教務主任 = K12.Data.School.Configuration["學校資訊"].PreviousData.SelectSingleNode("EduDirectorName").InnerText;

            //假別設定
            Dictionary<string, List<string>> allowAbsentDic = new Dictionary<string, List<string>>();
            foreach (AbsentSetting abs in _A.Select<AbsentSetting>())
            {
                string target = abs.Target;
                string source = abs.Source;

                if (!allowAbsentDic.ContainsKey(target))
                    allowAbsentDic.Add(target, new List<string>());

                allowAbsentDic[target].Add(source);
            }

            List<string> classIDs = _students.Select(x => x.RefClassID).Distinct().ToList();
            List<string> studentIDs = _students.Select(x => x.ID).ToList();

            //學生ID字串
            string id_str = string.Join("','", studentIDs);
            id_str = "'" + id_str + "'";

            //班級 catch
            Dictionary<string, ClassRecord> classDic = new Dictionary<string, ClassRecord>();
            foreach (ClassRecord cr in K12.Data.Class.SelectByIDs(classIDs))
            {
                if (!classDic.ContainsKey(cr.ID))
                    classDic.Add(cr.ID, cr);
            }

            //基本資料
            foreach (StudentRecord student in _students)
            {
                DataRow row = dt.NewRow();
                ClassRecord myClass = classDic.ContainsKey(student.RefClassID) ? classDic[student.RefClassID] : new ClassRecord();
                TeacherRecord myTeacher = myClass.Teacher != null ? myClass.Teacher : new TeacherRecord();

                row["列印日期"] = printDateTime;
                row["學校名稱"] = schoolName;
                row["學年度"] = _schoolYear;
                row["學期"] = _semester;
                row["姓名"] = student.Name;
                row["班級"] = myClass.Name + "";
                row["班導師"] = myTeacher.Name + "";
                row["座號"] = student.SeatNo + "";
                row["學號"] = student.StudentNumber;

                row["校長"] = 校長;
                row["教務主任"] = 教務主任;

                //filedName是 "列印假別1~20"
                foreach (string filedName in allowAbsentDic.Keys)
                {
                    row[filedName] = 0;
                }

                dt.Rows.Add(row);

                _RowCatchs.Add(student.ID, row);
            }

            //上課天數
            foreach (SemesterHistoryRecord shr in K12.Data.SemesterHistory.SelectByStudents(_students))
            {
                DataRow row = _RowCatchs[shr.RefStudentID];

                foreach (SemesterHistoryItem shi in shr.SemesterHistoryItems)
                {
                    if (shi.SchoolYear == _schoolYear && shi.Semester == _semester)
                        row["上課天數"] = shi.SchoolDayCount + "";
                }
            }

            //學期科目及領域成績
            foreach (JHSemesterScoreRecord jsr in JHSchool.Data.JHSemesterScore.SelectBySchoolYearAndSemester(studentIDs, _schoolYear, _semester))
            {
                DataRow row = _RowCatchs[jsr.RefStudentID];
                _文字描述.Clear();

                //學習領域成績
                if (_UserSelScoreType == "原始成績")
                {
                    row["學習領域成績"] = jsr.LearnDomainScoreOrigin.HasValue ? jsr.LearnDomainScoreOrigin.Value + "" : string.Empty;
                    row["課程學習成績"] = jsr.CourseLearnScoreOrigin.HasValue ? jsr.CourseLearnScoreOrigin.Value + "" : string.Empty;
                }
                else
                {
                    row["學習領域成績"] = jsr.LearnDomainScore.HasValue ? jsr.LearnDomainScore.Value + "" : string.Empty;
                    row["課程學習成績"] = jsr.CourseLearnScore.HasValue ? jsr.CourseLearnScore.Value + "" : string.Empty;
                }


                row["學習領域原始成績"] = jsr.LearnDomainScoreOrigin.HasValue ? jsr.LearnDomainScoreOrigin.Value + "" : string.Empty;
                row["課程學習原始成績"] = jsr.CourseLearnScoreOrigin.HasValue ? jsr.CourseLearnScoreOrigin.Value + "" : string.Empty;

                // 收集領域科目成績給領域科目對照時使用
                Dictionary<string, DomainScore> DomainScoreDict = new Dictionary<string, DomainScore>();
                Dictionary<string, List<SubjectScore>> DomainSubjScoreDict = new Dictionary<string, List<SubjectScore>>();




                #region 科目成績照領域排序
                var jsSubjects = new List<SubjectScore>(jsr.Subjects.Values);
                var domainList = new Dictionary<string, int>();
                domainList.Add("語文", 9000);
                domainList.Add("國語文", 8000);
                domainList.Add("英語", 7000);
                domainList.Add("數學", 6000);
                domainList.Add("社會", 5000);
                domainList.Add("自然與生活科技", 4000);
                domainList.Add("藝術與人文", 3000);
                domainList.Add("健康與體育", 2000);
                domainList.Add("綜合活動", 1000);
                domainList.Add("彈性課程", 0900);
                jsSubjects.Sort(delegate (SubjectScore r1, SubjectScore r2)
                {
                    decimal rank1 = 0;
                    decimal rank2 = 0;

                    if (r1.Credit != null)
                        rank1 += r1.Credit.Value;
                    if (r2.Credit != null)
                        rank2 += r2.Credit.Value;

                    if (domainList.ContainsKey(r1.Domain))
                        rank1 += domainList[r1.Domain];
                    if (domainList.ContainsKey(r2.Domain))
                        rank2 += domainList[r2.Domain];

                    if (rank1 == rank2)
                        return r2.Subject.CompareTo(r1.Subject);
                    else
                        return rank2.CompareTo(rank1);
                });
                #endregion
                //科目成績
                int count = 0;
                foreach (SubjectScore subj in jsSubjects)
                {
                    string ssNmae = subj.Domain;
                    if (string.IsNullOrEmpty(ssNmae))
                        ssNmae = "彈性課程";
                    if (!DomainSubjScoreDict.ContainsKey(ssNmae))
                        DomainSubjScoreDict.Add(ssNmae, new List<SubjectScore>());

                    DomainSubjScoreDict[ssNmae].Add(subj);

                    count++;

                    //超過就讓它爆炸
                    if (count > Global.SupportSubjectCount)
                        throw new Exception("超過支援列印科目數量: " + Global.SupportSubjectCount);

                    row["S科目" + count] = subj.Subject;
                    row["S領域" + count] = string.IsNullOrWhiteSpace(subj.Domain) ? "彈性課程" : subj.Domain;
                    row["S節數" + count] = subj.Period + "";
                    row["S權數" + count] = subj.Credit + "";
                    row["S成績" + count] = GetScoreString(subj.Score, subj.ScoreOrigin, subj.ScoreMakeup);
                    row["S等第" + count] = GetScoreDegreeString(subj.Score, subj.ScoreOrigin);//subj.Score.HasValue ? _degreeMapper.GetDegreeByScore(subj.Score.Value) : string.Empty;
                    row["S原始成績" + count] = subj.ScoreOrigin.HasValue ? subj.ScoreOrigin.Value + "" : string.Empty;
                    row["S補考成績" + count] = subj.ScoreMakeup.HasValue ? subj.ScoreMakeup.Value + "" : string.Empty;
                }


                // 處理領域科目並列               
                foreach (string dName in Global.PriDomainNameList())
                {
                    if (DomainSubjScoreDict.ContainsKey(dName))
                    {
                        int si = 1;
                        foreach (SubjectScore ss in DomainSubjScoreDict[dName])
                        {
                            row[dName + "科目" + si] = ss.Subject;
                            row[dName + "領域" + si] = ss.Domain;
                            row[dName + "節數" + si] = ss.Period + "";
                            row[dName + "權數" + si] = ss.Credit + "";

                            row[dName + "等第" + si] = GetScoreDegreeString(ss.Score, ss.ScoreOrigin);//ss.Score.HasValue ? _degreeMapper.GetDegreeByScore(ss.Score.Value) : string.Empty;
                            row[dName + "成績" + si] = GetScoreString(ss.Score, ss.ScoreOrigin, ss.ScoreMakeup);
                            row[dName + "原始成績" + si] = ss.ScoreOrigin.HasValue ? ss.ScoreOrigin.Value + "" : string.Empty;
                            row[dName + "補考成績" + si] = ss.ScoreMakeup.HasValue ? ss.ScoreMakeup.Value + "" : string.Empty;
                            si++;
                        }
                    }
                }


                count = 0;
                foreach (DomainScore domain in jsr.Domains.Values)
                {
                    if (!DomainScoreDict.ContainsKey(domain.Domain))
                        DomainScoreDict.Add(domain.Domain, domain);

                    count++;

                    //超過就讓它爆炸
                    if (count > Global.SupportDomainCount)
                        throw new Exception("超過支援列印領域數量: " + Global.SupportDomainCount);

                    row["D領域" + count] = domain.Domain;
                    row["D節數" + count] = domain.Period + "";
                    row["D權數" + count] = domain.Credit + "";

                    //row["D成績" + count] = domain.Score.HasValue ? domain.Score.Value + "" : string.Empty;
                    row["D成績" + count] = GetScoreString(domain.Score, domain.ScoreOrigin, domain.ScoreMakeup);

                    row["D等第" + count] = GetScoreDegreeString(domain.Score, domain.ScoreOrigin);//domain.Score.HasValue ? _degreeMapper.GetDegreeByScore(domain.Score.Value) : string.Empty;
                    row["D原始成績" + count] = domain.ScoreOrigin.HasValue ? domain.ScoreOrigin.Value + "" : string.Empty;
                    row["D補考成績" + count] = domain.ScoreMakeup.HasValue ? domain.ScoreMakeup.Value + "" : string.Empty;

                    if (!string.IsNullOrWhiteSpace(domain.Text))
                        _文字描述.Add(domain.Domain + " : " + domain.Text);
                }

                // 處理指定領域
                foreach (string dName in Global.PriDomainNameList())
                {
                    if (DomainScoreDict.ContainsKey(dName))
                    {
                        DomainScore domain = DomainScoreDict[dName];
                        row[dName + "領域"] = domain.Domain;
                        row[dName + "節數"] = domain.Period + "";
                        row[dName + "權數"] = domain.Credit + "";
                        row[dName + "成績"] = GetScoreString(domain.Score, domain.ScoreOrigin, domain.ScoreMakeup);
                        row[dName + "等第"] = GetScoreDegreeString(domain.Score, domain.ScoreOrigin);//domain.Score.HasValue ? _degreeMapper.GetDegreeByScore(domain.Score.Value) : string.Empty;
                        row[dName + "原始成績"] = domain.ScoreOrigin.HasValue ? domain.ScoreOrigin.Value + "" : string.Empty;
                        row[dName + "補考成績"] = domain.ScoreMakeup.HasValue ? domain.ScoreMakeup.Value + "" : string.Empty;
                    }
                }


                row["文字描述"] = string.Join(Environment.NewLine, _文字描述);
            }

            //預設學年度學期物件
            JHSchool.Behavior.BusinessLogic.SchoolYearSemester sysm = new JHSchool.Behavior.BusinessLogic.SchoolYearSemester(_schoolYear, _semester);

            //AutoSummary
            foreach (AutoSummaryRecord asr in AutoSummary.Select(_students.Select(x => x.ID), new JHSchool.Behavior.BusinessLogic.SchoolYearSemester[] { sysm }))
            {
                DataRow row = _RowCatchs[asr.RefStudentID];

                //缺曠
                foreach (AbsenceCountRecord acr in asr.AbsenceCounts)
                {
                    string key = Global.GetKey(acr.PeriodType, acr.Name);

                    //filedName是 "列印假別1~20"
                    foreach (string filedName in allowAbsentDic.Keys)
                    {
                        foreach (string item in allowAbsentDic[filedName])
                        {
                            if (key == item)
                            {
                                int count = 0;
                                int.TryParse(row[filedName] + "", out count);

                                count += acr.Count;
                                row[filedName] = count;
                            }
                        }
                    }
                }

                //獎懲
                row["大功"] = asr.MeritA;
                row["小功"] = asr.MeritB;
                row["嘉獎"] = asr.MeritC;
                row["大過"] = asr.DemeritA;
                row["小過"] = asr.DemeritB;
                row["警告"] = asr.DemeritC;

                //日常生活表現
                JHMoralScoreRecord msr = asr.MoralScore;
                XmlElement textScore = (msr != null && msr.TextScore != null) ? msr.TextScore : K12.Data.XmlHelper.LoadXml("<TextScore/>");

                foreach (string key in Global.DLBehaviorRef.Keys)
                    SetDLBehaviorData(key, Global.DLBehaviorRef[key], textScore, row);
            }

            //社團成績
            string condition = string.Format("SchoolYear='{0}' and Semester='{1}' and studentid in ({2})", _schoolYear, _semester, id_str);
            List<AssnCode> list = _A.Select<AssnCode>(condition);

            foreach (string id in studentIDs)
            {
                int count = 0;
                DataRow row = _RowCatchs[id];

                foreach (AssnCode ac in list.FindAll(x => x.StudentID == id))
                {
                    XmlElement scores = K12.Data.XmlHelper.LoadXml(ac.Scores);

                    foreach (XmlElement item in scores.SelectNodes("Item"))
                    {
                        count++;

                        //超過就讓它爆炸
                        if (count > Global.SupportClubCount)
                            throw new Exception("超過支援列印社團數量: " + Global.SupportClubCount);

                        string name = item.GetAttribute("AssociationName");
                        string score = item.GetAttribute("Score");
                        string effort = item.GetAttribute("Effort");
                        string text = item.GetAttribute("Text");

                        row["社團Name" + count] = name;
                        row["社團Score" + count] = score;
                        row["社團Effort" + count] = effort;
                        row["社團Text" + count] = text;
                    }
                }
            }

            //服務學習時數
            string query = string.Format("select ref_student_id,occur_date,reason,hours from $k12.service.learning.record where school_year={0} and semester={1} and ref_student_id in ({2})", _schoolYear, _semester, id_str);
            DataTable table = _Q.Select(query);
            foreach (DataRow dr in table.Rows)
            {
                string sid = dr["ref_student_id"] + "";

                DataRow row = _RowCatchs[sid];

                decimal new_hr = 0;
                decimal.TryParse(dr["hours"] + "", out new_hr);

                decimal old_hr = 0;
                decimal.TryParse(row["服務學習時數"] + "", out old_hr);

                decimal hr = old_hr + new_hr;
                row["服務學習時數"] = hr;
            }

            // 取得體適能資料
            Dictionary<string, List<StudentFitnessRecord_C>> StudentFitnessRecord_CDict = new Dictionary<string, List<StudentFitnessRecord_C>>();

            string qry = "ref_student_id in('" + string.Join("','", studentIDs.ToArray()) + "') and school_year=" + _schoolYear;
            AccessHelper accHelper = new AccessHelper();
            List<StudentFitnessRecord_C> StudentFitnessRecord_CList = accHelper.Select<StudentFitnessRecord_C>(qry);

            // 依測驗日期排序
            StudentFitnessRecord_CList = (from data in StudentFitnessRecord_CList orderby data.TestDate ascending select data).ToList();

            foreach (StudentFitnessRecord_C rec in StudentFitnessRecord_CList)
            {
                if (!StudentFitnessRecord_CDict.ContainsKey(rec.StudentID))
                    StudentFitnessRecord_CDict.Add(rec.StudentID, new List<StudentFitnessRecord_C>());

                StudentFitnessRecord_CDict[rec.StudentID].Add(rec);
            }

            foreach (string sid in StudentFitnessRecord_CDict.Keys)
            {
                if (_RowCatchs.ContainsKey(sid))
                {
                    DataRow row = _RowCatchs[sid];
                    int cot = 1;
                    foreach (StudentFitnessRecord_C rec in StudentFitnessRecord_CDict[sid])
                    {
                        row["身高" + cot] = rec.Height;
                        row["體重" + cot] = rec.Weight;
                        row["坐姿體前彎" + cot] = rec.SitAndReach;
                        row["坐姿體前彎常模" + cot] = rec.SitAndReachDegree;
                        row["立定跳遠" + cot] = rec.StandingLongJump;
                        row["立定跳遠常模" + cot] = rec.StandingLongJumpDegree;
                        row["仰臥起坐" + cot] = rec.SitUp;
                        row["仰臥起坐常模" + cot] = rec.SitUpDegree;
                        row["心肺適能" + cot] = rec.Cardiorespiratory;
                        row["心肺適能常模" + cot] = rec.CardiorespiratoryDegree;
                        cot++;
                    }
                }
            }
        }


        /// <summary>
        /// 取得成績文字型態
        /// </summary>
        /// <param name="score"></param>
        /// <param name="scoreO"></param>
        /// <param name="scoreM"></param>
        /// <returns></returns>
        private static string GetScoreString(decimal? score, decimal? scoreO, decimal? scoreM)
        {
            string value = "";

            if (_UserSelScoreType == "原始成績")
            {
                if (scoreO.HasValue)
                    value = scoreO.Value.ToString();
            }

            if (_UserSelScoreType == "原始補考擇優")
            {
                decimal ss = 0;
                // 成績
                if (score.HasValue)
                    ss = score.Value;

                if (scoreM.HasValue && scoreM.Value >= ss)
                    value = _ReExamMark + ss;
                else
                    value = ss.ToString();
            }

            return value;
        }

        /// <summary>
        /// 取得成績等第
        /// </summary>
        /// <param name="score"></param>
        /// <param name="scoreO"></param>
        /// <param name="scoreM"></param>
        /// <returns></returns>
        private static string GetScoreDegreeString(decimal? score, decimal? scoreO)
        {
            string value = "";

            if (_UserSelScoreType == "原始成績")
            {
                if (scoreO.HasValue)
                {
                    value = _degreeMapper.GetDegreeByScore(scoreO.Value);
                }
            }

            if (_UserSelScoreType == "原始補考擇優")
            {
                decimal ss = 0;
                if (score.HasValue)
                    ss = score.Value;

                value = _degreeMapper.GetDegreeByScore(ss);
            }

            return value;
        }


        /// <summary>
        /// 填寫日常生活表現資料
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="textScore"></param>
        /// <param name="row"></param>

        private static void SetDLBehaviorData(string name, string path, XmlElement textScore, DataRow row)
        {
            row[name + "_Name"] = _DLBehaviorConfigNameDict.ContainsKey(name) ? _DLBehaviorConfigNameDict[name] : string.Empty;

            if (_DLBehaviorConfigItemNameDict.ContainsKey(name))
            {
                int index = 0;
                foreach (string itemName in _DLBehaviorConfigItemNameDict[name])
                {
                    foreach (XmlElement item in textScore.SelectNodes(path))
                    {
                        if (itemName == item.GetAttribute("Name"))
                        {
                            index++;
                            row[name + "_Item_Name" + index] = itemName;
                            row[name + "_Item_Degree" + index] = item.GetAttribute("Degree");
                            row[name + "_Item_Description" + index] = item.GetAttribute("Description");
                        }
                    }
                }
            }
            else if (_DLBehaviorConfigNameDict.ContainsKey(name))
            {
                string value = _DLBehaviorConfigNameDict[name];

                foreach (XmlElement item in textScore.SelectNodes(path))
                {
                    if (value == item.GetAttribute("Name"))
                        row[name + "_Description"] = item.GetAttribute("Description");
                }
            }
        }

        /// <summary>
        /// 取得列印資料表
        /// </summary>
        /// <param name="students"></param>
        /// <param name="schoolYear"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(List<StudentRecord> students, int schoolYear, int semester)
        {
            Initialize();

            _students = students;
            _schoolYear = schoolYear;
            _semester = semester;

            DataTable dt = GetEmptyDataTable();

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //有學生才填資料
            if (_students.Count > 0)
                FillData(dt);

            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);

            return dt;
        }

        /// <summary>
        /// 物件初始化
        /// </summary>
        private static void Initialize()
        {
            if (_A == null)
                _A = new AccessHelper();
            if (_Q == null)
                _Q = new QueryHelper();
            if (_degreeMapper == null)
                _degreeMapper = new DegreeMapper();
        }
    }
}
