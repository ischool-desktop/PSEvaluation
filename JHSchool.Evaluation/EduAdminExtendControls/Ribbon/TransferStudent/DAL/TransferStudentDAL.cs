using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data;
using FISCA.DSAUtil;
using Framework;
using Framework.Feature;
using System.Xml;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.SemesterHistory;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.DAL
{
    public static class TransferStudentDAL
    {
        /// <summary>
        /// 取得日常生活表現子項
        /// </summary>
        /// <returns>日常生活表現子項名稱</returns>
        public static List<string> GetDailyBehaviorItems()
        {
            List<string> list = new List<string>();
            ConfigData cd = School.Configuration["DLBehaviorConfig"];

            foreach (string each in cd)
            {
                if (each == "DailyBehavior")
                {
                    XmlElement db = XmlHelper.LoadXml(cd[each]);

                    foreach (XmlNode node in db.SelectNodes("Item"))
                    {
                        XmlElement element = (XmlElement)node;
                        string name = element.GetAttribute("Name");
                        list.Add(name);
                    }
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// 取得學習歷程
        /// </summary>
        /// <param name="studentid">學生編號</param>
        /// <returns>學習歷程</returns>
        public static List<SemesterHistoryData> GetSemesterHistory(string studentid)
        {
            bool checkAddDefault = true;
            List<SemesterHistoryData> list = new List<SemesterHistoryData>();
            foreach (SemesterHistoryRecord each in SemesterHistory.Instance[studentid])
            {
                SemesterHistoryData sh = new SemesterHistoryData();
                sh.GradeYear = each.GradeYear.ToString();
                sh.SchoolYear = each.SchoolYear.ToString();
                sh.Semester = each.Semester.ToString();
                list.Add(sh);
            }

            // 檢查是否已有預設學年度學期資料
            foreach (SemesterHistoryData shd in list)
                if (shd.SchoolYear == School.DefaultSchoolYear && shd.Semester == School.DefaultSemester)
                {
                    checkAddDefault = false;
                    break;
                }

            if(checkAddDefault ==true )
            {
                SemesterHistoryData current = new SemesterHistoryData();
                current.SchoolYear = School.DefaultSchoolYear;
                current.Semester = School.DefaultSemester;
                list.Add(current);
            }

            //假造資料
            //SemesterHistoryData sh11 = new SemesterHistoryData();
            //sh11.GradeYear = "1";
            //sh11.SchoolYear = "96";
            //sh11.Semester = "1";
            //list.Add(sh11);

            //SemesterHistoryData sh12 = new SemesterHistoryData();
            //sh12.GradeYear = "1";
            //sh12.SchoolYear = "96";
            //sh12.Semester = "2";
            //list.Add(sh12);

            //SemesterHistoryData sh21 = new SemesterHistoryData();
            //sh21.GradeYear = "2";
            //sh21.SchoolYear = "97";
            //sh21.Semester = "1";
            //list.Add(sh21);

            //SemesterHistoryData sh22 = new SemesterHistoryData();
            //sh22.GradeYear = "2";
            //sh22.SchoolYear = "97";
            //sh22.Semester = "2";
            //list.Add(sh22);

            return list;
        }

        /// <summary>
        /// 取得日常生活表現程度對照表
        /// </summary>
        /// <returns>Key:代碼, Value:文字 的陣列</returns>
        public static Dictionary<string, string> GetDailyBehaviorDegrees()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            ConfigData cd = School.Configuration["DLBehaviorConfig"];

            foreach (string each in cd)
            {
                if (each == "DailyBehavior")
                {
                    XmlElement db = XmlHelper.LoadXml(cd[each]);

                    foreach (XmlNode node in db.SelectNodes("PerformanceDegree/Mapping"))
                    {
                        XmlElement element = (XmlElement)node;
                        string degree = element.GetAttribute("Degree");
                        string desc = element.GetAttribute("Desc");
                        list.Add(degree, desc);
                    }
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// 取得團體活動子項目
        /// </summary>
        /// <returns>團體活動子項目</returns>
        public static List<string> GetGroupActivitieItems()
        {
            List<string> list = new List<string>();
            ConfigData cd = School.Configuration["DLBehaviorConfig"];

            foreach (string each in cd)
            {
                if (each == "GroupActivity")
                {
                    XmlElement db = XmlHelper.LoadXml(cd[each]);

                    foreach (XmlNode node in db.SelectNodes("Item"))
                    {
                        XmlElement element = (XmlElement)node;
                        string name = element.GetAttribute("Name");
                        list.Add(name);
                    }
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// 取得團體活動表現的努力程度對照表
        /// </summary>
        /// <returns>Key:代碼, Value:文字 的陣列</returns>
        public static Dictionary<string, string> GetGroupActivityDegrees()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            ConfigData cd = School.Configuration["努力程度對照表"];

            foreach (string each in cd)
            {
                if (each == "xml")
                {
                    XmlElement db = XmlHelper.LoadXml(cd[each]);

                    foreach (XmlNode node in db.SelectNodes("Effort"))
                    {
                        XmlElement element = (XmlElement)node;
                        string code = element.GetAttribute("Code");
                        string name = element.GetAttribute("Name");
                        list.Add(code, name);
                    }
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// 取得公共服務表現項目
        /// </summary>
        /// <returns>公共服務表現項目</returns>
        public static List<string> GetPublicServicesItems()
        {
            List<string> list = new List<string>();
            ConfigData cd = School.Configuration["DLBehaviorConfig"];

            foreach (string each in cd)
            {
                if (each == "PublicService")
                {
                    XmlElement db = XmlHelper.LoadXml(cd[each]);

                    foreach (XmlNode node in db.SelectNodes("Item"))
                    {
                        XmlElement element = (XmlElement)node;
                        string name = element.GetAttribute("Name");
                        list.Add(name);
                    }
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// 取得校內外特殊表現
        /// </summary>
        /// <returns>校內外特殊表現</returns>
        public static List<string> GetSchoolSpecialItems()
        {
            List<string> list = new List<string>();

            ConfigData cd = School.Configuration["DLBehaviorConfig"];

            foreach (string each in cd)
            {
                if (each == "SchoolSpecial")
                {
                    XmlElement db = XmlHelper.LoadXml(cd[each]);

                    foreach (XmlNode node in db.SelectNodes("Item"))
                    {
                        XmlElement element = (XmlElement)node;
                        string name = element.GetAttribute("Name");
                        list.Add(name);
                    }
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// 取得日常生活表現具體建議子項目
        /// </summary>
        /// <returns>日常生活表現具體建議子項目清單</returns>
        public static List<string> GetDailyLifeRecommendItems()
        {
            List<string> list = new List<string>();
            list.Add("日常生活表現具體建議");

            return list;
        }

        /// <summary>
        /// 日常生活表現輸入對照表
        /// </summary>
        /// <returns>對照表</returns>
        public static Dictionary<string, string> GetDailyLifeRecommendMapping()
        {            
            //德行評語代碼表
            string targetService = "SmartSchool.Config.GetList";

            Dictionary<string, string> list = new Dictionary<string, string>();

            DSXmlHelper helper = new DSXmlHelper("GetListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "Content", "");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "Name", "德行評語代碼表");

            DSRequest req = new DSRequest(helper.BaseElement);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, req);

            foreach (XmlElement element in rsp.GetContent().GetElements("List/Content/Morality"))
            {
                list.Add(element.GetAttribute("Code"),element.GetAttribute("Comment"));
            }
          
            return list;
        }

        /// <summary>
        /// 取得假別項目
        /// </summary>
        /// <returns>假別項目列表</returns>
        public static List<string> GetAbsenceItems()
        {
            string targetService = "SmartSchool.Config.GetList";

            List<string> list = new List<string>();

            DSXmlHelper helper = new DSXmlHelper("GetListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "Content", "");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "Name", "假別對照表");

            DSRequest req = new DSRequest(helper.BaseElement);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, req);

            foreach (XmlElement element in rsp.GetContent().GetElements("List/AbsenceList/Absence"))
            {
                list.Add(element.GetAttribute("Name"));
            }
            return list;
        }

        /// <summary>
        /// 取得節次類型
        /// </summary>
        /// <returns>節次類型清單</returns>
        public static List<string> GetPeriodTypeItems()
        {
            string targetService = "SmartSchool.Config.GetList";

            List<string> list = new List<string>();

            DSXmlHelper helper = new DSXmlHelper("GetListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "Content", "");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "Name", "節次對照表");

            DSRequest req = new DSRequest(helper.BaseElement);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, req);

            foreach (XmlElement element in rsp.GetContent().GetElements("List/Periods/Period"))
            {
                string type = element.GetAttribute("Type");

                if (!list.Contains(type))
                    list.Add(type);
            }
            return list;
        }

        /// <summary>
        /// 取得獎懲清單
        /// </summary>
        /// <returns>獎懲清單</returns>
        public static List<string> GetMeritTypes()
        {
            string targetService = "SmartSchool.Config.GetList";

            List<string> list = new List<string>();

            DSXmlHelper helper = new DSXmlHelper("GetListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "Content");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "Name", "鍵盤化獎懲資料管理_獎懲熱鍵表");

            DSRequest req = new DSRequest(helper.BaseElement);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, req);

            foreach (XmlElement element in rsp.GetContent().GetElements("List/Configurations/Configuration"))
            {
                list.Add(element.GetAttribute("Name"));
            }
            return list;
        }

        public static TransferStudentData GetStudentTransferData(string studentid)
        {
            string targetService = "SmartSchool.Score.GetSemesterMoralScore";

            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "TextScore");
            helper.AddElement("Field", "InitialSummary");
            helper.AddElement("Field", "Summary");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "RefStudentID", studentid);

            DSRequest req = new DSRequest(helper.BaseElement);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, req);

            TransferStudentData data = new TransferStudentData(rsp.GetContent().BaseElement);

            return data;
        }

        public static void Save(TransferStudentData transferStudentData, string studentid)
        {
            List<SemesterHistoryData> history = GetSemesterHistory(studentid);

            DSXmlHelper insertHelper = new DSXmlHelper("Request");
            DSXmlHelper updateHelper = new DSXmlHelper("Request");
            int insertIndex = 1, updateIndex = 1;

            foreach (SemesterHistoryData sh in history)
            {
                //預設用 insert 來儲存此學期資料
                string modifyType = "insert";

                //預設存在 Summary 欄位中
                Flag target = Flag.Summary;

                foreach (SourceFlag flag in transferStudentData.SourceFlags)
                {
                    // 如果學習歷程中的學年度學期包含在此學生的資料當中, 表示要用 Update 來儲存此學期資料
                    if (flag.SchoolYear == sh.SchoolYear && flag.Semester == sh.Semester)
                    {
                        modifyType = "update";
                        target = flag.Flag;
                        break;
                    }
                }

                // 如果為本學年度學期的話, 則一律存在 InitialSummary 欄位中
                if (sh.SchoolYear == School.DefaultSchoolYear && sh.Semester == School.DefaultSemester)
                    target = Flag.IntialSummary;

                XmlElement statisticElement = transferStudentData.GetStatisticsElement(sh.SchoolYear, sh.Semester);
                XmlElement textScoreElement = transferStudentData.GetTextScoreElement(sh.SchoolYear, sh.Semester);

                if (modifyType == "insert")
                {
                    insertHelper.AddElement("SemesterMoralScore");
                    insertHelper.AddElement("SemesterMoralScore[" + insertIndex + "]", "RefStudentID", studentid);
                    insertHelper.AddElement("SemesterMoralScore[" + insertIndex + "]", "SchoolYear", sh.SchoolYear);
                    insertHelper.AddElement("SemesterMoralScore[" + insertIndex + "]", "Semester", sh.Semester);
                    insertHelper.AddElement("SemesterMoralScore[" + insertIndex + "]", "TextScore", textScoreElement.OuterXml,true);

                    if (target == Flag.IntialSummary)
                        insertHelper.AddElement("SemesterMoralScore[" + insertIndex + "]", "InitialSummary", statisticElement.OuterXml, true);
                    else
                        insertHelper.AddElement("SemesterMoralScore[" + insertIndex + "]", "Summary", statisticElement.OuterXml, true);

                    insertIndex++;
                }
                else
                {
                    updateHelper.AddElement("SemesterMoralScore");
                    updateHelper.AddElement("SemesterMoralScore[" + updateIndex + "]", "Condition");
                    updateHelper.AddElement("SemesterMoralScore[" + updateIndex + "]/Condition", "RefStudentID", studentid);
                    updateHelper.AddElement("SemesterMoralScore[" + updateIndex + "]/Condition", "SchoolYear", sh.SchoolYear);
                    updateHelper.AddElement("SemesterMoralScore[" + updateIndex + "]/Condition", "Semester", sh.Semester);
                    updateHelper.AddElement("SemesterMoralScore[" + updateIndex + "]", "TextScore", textScoreElement.OuterXml,true);

                    if (target == Flag.IntialSummary)
                        updateHelper.AddElement("SemesterMoralScore[" + updateIndex + "]", "InitialSummary", statisticElement.OuterXml, true);
                    else
                        updateHelper.AddElement("SemesterMoralScore[" + updateIndex + "]", "Summary", statisticElement.OuterXml, true);

                    updateIndex++;
                }
            }

            if (insertIndex > 1)
            {
                FISCA.Authentication.DSAServices.CallService("SmartSchool.Score.InsertSemesterMoralScore", new DSRequest(insertHelper.BaseElement));
            }
            if (updateIndex > 1)
            {
                FISCA.Authentication.DSAServices.CallService("SmartSchool.Score.UpdateSemesterMoralScore", new DSRequest(updateHelper.BaseElement));
            }
        }
    }
}
