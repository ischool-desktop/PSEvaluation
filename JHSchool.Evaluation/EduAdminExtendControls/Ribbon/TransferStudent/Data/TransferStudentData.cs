using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.DailyBehavior;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.GroupActivity;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.PublicService;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.SchoolSpecial;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.DailyLifeRecommend;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Absence;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.MeritStatistic;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.MeritDetail;
using System.Xml;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.DailyBehavior;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.DailyLifeRecommend;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.Absence;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data
{
    public class TransferStudentData
    {
        public List<SourceFlag> SourceFlags { get; set; }
        public DailyBehaviorData DailyBehaviorData { get; set; }
        public GroupActivityData GroupActivityData { get; set; }
        public PublicServiceData PublicServiceData { get; set; }
        public SchoolSpecialData SchoolSpecialData { get; set; }
        public DailyLifeRecommendData DailyLifeRecommendData { get; set; }
        public AbsenceData AbsenceData { get; set; }
        public MeritStatisticData MeritStatisticData { get; set; }

        public TransferStudentData()
            : this(null)
        {
        }

        public TransferStudentData(XmlElement response)
        {
            this.DailyBehaviorData = new DailyBehaviorData();
            this.GroupActivityData = new GroupActivityData();
            this.PublicServiceData = new PublicServiceData();
            this.SchoolSpecialData = new SchoolSpecialData();
            this.DailyLifeRecommendData = new DailyLifeRecommendData();
            this.AbsenceData = new AbsenceData();
            this.MeritStatisticData = new MeritStatisticData();
            this.SourceFlags = new List<SourceFlag>();
            if (response == null) return;

            foreach (XmlNode node in response.SelectNodes("SemesterMoralScore"))
            {
                string schoolyear = node.SelectSingleNode("SchoolYear").InnerText;
                string semester = node.SelectSingleNode("Semester").InnerText;

                SourceFlag sourceFlag = new SourceFlag();
                sourceFlag.SchoolYear = schoolyear;
                sourceFlag.Semester = semester;
                XmlElement sourceElement;

                if (node.SelectSingleNode("Summary").ChildNodes.Count > 0)
                {
                    sourceFlag.Flag = Flag.Summary;
                    sourceElement = (XmlElement)node.SelectSingleNode("Summary");
                    sourceFlag.SourceElement = sourceElement;
                }
                else
                {
                    sourceFlag.Flag = Flag.IntialSummary;
                    sourceElement = (XmlElement)node.SelectSingleNode("InitialSummary");
                    sourceFlag.SourceElement = sourceElement;
                }
                SourceFlags.Add(sourceFlag);

                //處理日常生活表現
                foreach (XmlNode itemNode in node.SelectNodes("TextScore/DailyBehavior/Item"))
                {
                    XmlElement itemElement = (XmlElement)itemNode;
                    DailyBehaviorItem item = new DailyBehaviorItem();

                    item.SchoolYear = schoolyear;
                    item.Semester = semester;
                    item.Degree = itemElement.GetAttribute("Degree");
                    item.Name = itemElement.GetAttribute("Name");
                    item.Index = itemElement.GetAttribute("Index");
                    this.DailyBehaviorData.Items.Add(item);
                }

                //處理團隊活動表現
                foreach (XmlNode itemNode in node.SelectNodes("TextScore/GroupActivity/Item"))
                {
                    XmlElement itemElement = (XmlElement)itemNode;
                    GroupActivityItem item = new GroupActivityItem();

                    item.SchoolYear = schoolyear;
                    item.Semester = semester;
                    item.Degree = itemElement.GetAttribute("Degree");
                    item.Name = itemElement.GetAttribute("Name");
                    item.Text = itemElement.GetAttribute("Description");
                    this.GroupActivityData.Items.Add(item);
                }

                //處理公共服務表現
                foreach (XmlNode itemNode in node.SelectNodes("TextScore/PublicService/Item"))
                {
                    XmlElement itemElement = (XmlElement)itemNode;
                    PublicServiceItem item = new PublicServiceItem();

                    item.SchoolYear = schoolyear;
                    item.Semester = semester;
                    item.Name = itemElement.GetAttribute("Name");
                    item.Description = itemElement.GetAttribute("Description");
                    this.PublicServiceData.Items.Add(item);
                }

                //處理校內外特殊表現
                foreach (XmlNode itemNode in node.SelectNodes("TextScore/SchoolSpecial/Item"))
                {
                    XmlElement itemElement = (XmlElement)itemNode;
                    SchoolSpecialItem item = new SchoolSpecialItem();

                    item.SchoolYear = schoolyear;
                    item.Semester = semester;
                    item.Name = itemElement.GetAttribute("Name");
                    item.Description = itemElement.GetAttribute("Description");
                    this.SchoolSpecialData.Items.Add(item);
                }

                //日常生活表現具體建議
                //foreach (XmlNode itemNode in node.SelectNodes("TextScore/DailyLifeRecommend/Item"))
                foreach (XmlNode itemNode in node.SelectNodes("TextScore/DailyLifeRecommend"))
                {
                    XmlElement itemElement = (XmlElement)itemNode;
                    DailyLifeRecommendItem item = new DailyLifeRecommendItem();

                    item.SchoolYear = schoolyear;
                    item.Semester = semester;
                    item.Name = itemElement.GetAttribute("Name");
                    item.Description = itemElement.GetAttribute("Description");
                    this.DailyLifeRecommendData.Items.Add(item);
                }

                //缺曠統計
                foreach (XmlNode itemNode in sourceElement.SelectNodes("AttendanceStatistics/Absence"))
                {
                    XmlElement itemElement = (XmlElement)itemNode;
                    AbsenceItem item = new AbsenceItem();

                                        item.SchoolYear = schoolyear;
                    item.Semester = semester;
                    item.Name = itemElement.GetAttribute("Name");
                    item.PeriodType = itemElement.GetAttribute("PeriodType");

                    int count;
                    if (!int.TryParse(itemElement.GetAttribute("Count"), out count))
                        count = 0;
                    item.Count = count;

                    this.AbsenceData.Items.Add(item);
                }

                //懲統計
                foreach (XmlNode itemNode in sourceElement.SelectNodes("DisciplineStatistics/Demerit"))
                {
                    XmlElement itemElement = (XmlElement)itemNode;
                    MeritStatisticItem itemA = new MeritStatisticItem();

                    itemA.SchoolYear = schoolyear;
                    itemA.Semester = semester;
                    itemA.MeritType = "大過";
                    int count;
                    if (!int.TryParse(itemElement.GetAttribute("A"), out count))
                        count = 0;
                    itemA.Count = count;
                    
                    this.MeritStatisticData.Items.Add(itemA);
                                       
                    MeritStatisticItem itemB = new MeritStatisticItem();

                    itemB.SchoolYear = schoolyear;
                    itemB.Semester = semester;
                    itemB.MeritType = "小過";
                
                    if (!int.TryParse(itemElement.GetAttribute("B"), out count))
                        count = 0;
                    itemB.Count = count;

                    this.MeritStatisticData.Items.Add(itemB);

                    MeritStatisticItem itemC = new MeritStatisticItem();

                    itemC.SchoolYear = schoolyear;
                    itemC.Semester = semester;
                    itemC.MeritType = "警告";
                
                    if (!int.TryParse(itemElement.GetAttribute("C"), out count))
                        count = 0;
                    itemC.Count = count;

                    this.MeritStatisticData.Items.Add(itemC);
                }

                //獎統計
                foreach (XmlNode itemNode in sourceElement.SelectNodes("DisciplineStatistics/Merit"))
                {
                    XmlElement itemElement = (XmlElement)itemNode;
                    MeritStatisticItem itemA = new MeritStatisticItem();

                    itemA.SchoolYear = schoolyear;
                    itemA.Semester = semester;
                    itemA.MeritType = "大功";
                    int count;
                    if (!int.TryParse(itemElement.GetAttribute("A"), out count))
                        count = 0;
                    itemA.Count = count;

                    this.MeritStatisticData.Items.Add(itemA);

                    MeritStatisticItem itemB = new MeritStatisticItem();

                    itemB.SchoolYear = schoolyear;
                    itemB.Semester = semester;
                    itemB.MeritType = "小功";

                    if (!int.TryParse(itemElement.GetAttribute("B"), out count))
                        count = 0;
                    itemB.Count = count;

                    this.MeritStatisticData.Items.Add(itemB);

                    MeritStatisticItem itemC = new MeritStatisticItem();

                    itemC.SchoolYear = schoolyear;
                    itemC.Semester = semester;
                    itemC.MeritType = "嘉獎";

                    if (!int.TryParse(itemElement.GetAttribute("C"), out count))
                        count = 0;
                    itemC.Count = count;

                    this.MeritStatisticData.Items.Add(itemC);
                }
            }
        }

        public XmlElement GetTextScoreElement(string schoolyear, string semester)
        {
            XmlDocument doc = new XmlDocument();            
            XmlElement textScoreElement = doc.CreateElement("TextScore");
            
            XmlElement dailyBehaviorElement = this.DailyBehaviorData.GetSemesterElement(schoolyear, semester);
            XmlNode dbe = doc.ImportNode(dailyBehaviorElement, true);
            textScoreElement.AppendChild(dbe);

            XmlElement groupActivityElement = this.GroupActivityData.GetSemesterElement(schoolyear, semester);
            XmlNode gae = doc.ImportNode(groupActivityElement, true);
            textScoreElement.AppendChild(gae);

            XmlElement publicServiceElement = this.PublicServiceData.GetSemesterElement(schoolyear, semester);
            XmlNode pse = doc.ImportNode(publicServiceElement, true);
            textScoreElement.AppendChild(pse);

            XmlElement schoolSpecialElement = this.SchoolSpecialData.GetSemesterElement(schoolyear, semester);
            XmlNode sse = doc.ImportNode(schoolSpecialElement, true);
            textScoreElement.AppendChild(sse);

            XmlElement dailyLifeRecommendElement = this.DailyLifeRecommendData.GetSemesterElement(schoolyear, semester);
            XmlNode dre = doc.ImportNode(dailyLifeRecommendElement, true);
            textScoreElement.AppendChild(dre);

            return textScoreElement;
        }

        public XmlElement GetStatisticsElement(string schoolyear, string semester)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("Summary");
           
            XmlElement attendanceElement = this.AbsenceData.GetSemesterElement(schoolyear, semester);
            XmlNode ate = doc.ImportNode(attendanceElement, true);
            element.AppendChild(ate);

            XmlElement disciplineElement = this.MeritStatisticData.GetSemesterElement(schoolyear, semester);
            XmlNode dpe = doc.ImportNode(disciplineElement, true);
            element.AppendChild(dpe);
                     
            return element;
        }
    }

    public enum Flag { IntialSummary, Summary }
    public class SourceFlag
    {
        public string SchoolYear { get; set; }
        public string Semester { get; set; }
        public XmlElement SourceElement { get; set; }
        public Flag Flag { get; set; }
    }
}
