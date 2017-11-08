using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.Data;
using Framework;

namespace JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.DAL
{
    public static class ActivityRecordDAL
    {
        public const string TABLE_活動表現紀錄對照表 = "活動表現紀錄對照表";
        public const string TABLE_活動表現紀錄 = "活動表現紀錄";
        public const string PACKAGE_NAME = "活動表現";
        public const string SERIVCE_取得活動表現紀錄對照表 = "取得活動表現紀錄對照表";

        public static void PrepareMetadata()
        {
            if (!HasActivityRecordTable())            
                CreateActivityRecordTable(); 
            else
                UpdateSchema();

            ImportServicePackage();
        }

        public static bool HasActivityRecordTable()
        {
            string targetService = "UDTService.DDL.ListTables";
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest());

            foreach (XmlNode node in rsp.GetContent().BaseElement.SelectNodes("Table"))
            {
                XmlElement element = (XmlElement)node;

                if (element.GetAttribute("Name") == TABLE_活動表現紀錄對照表)
                    return true;
            }
            return false;
        }

        public static void UpdateSchema()
        {
            string targetService = "UDTService.DDL.ListTables";
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest());
            XmlNode node = rsp.GetContent().BaseElement.SelectSingleNode("Table[@Name='"+TABLE_活動表現紀錄對照表+"']/Field[@Name='數量']");

            if (node == null)
            {
                DSXmlHelper helper = new DSXmlHelper("Request");
                XmlElement e = helper.AddElement("TableName");
                e.InnerText = TABLE_活動表現紀錄對照表;

                XmlElement e1 = helper.AddElement("Field");
                e1.SetAttribute("Name", "數量");
                e1.SetAttribute("DataType", "Number");
                e1.SetAttribute("Indexed", "false");

                FISCA.Authentication.DSAServices.CallService("UDTService.DDL.AddField", new DSRequest(helper.BaseElement));
            }

        }


        /// <summary>
        /// 建立活動紀錄所需UDT Table
        /// </summary>
        public static void CreateActivityRecordTable()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Resource1.ImportTables);

            string targetService = "UDTService.DDL.ImportTables";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(doc.DocumentElement));
        }

        /// <summary>
        /// 刪除活動紀錄所用UDT Table
        /// </summary>
        public static void DropActivityRecordTables()
        {
            string targetService = "UDTService.DDL.DropTable";

            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("TableName", TABLE_活動表現紀錄);
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            helper = new DSXmlHelper("Request");
            helper.AddElement("TableName", TABLE_活動表現紀錄對照表);
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));
        }

        public static void ImportServicePackage()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Resource1.Package);

            DSRequest req = new DSRequest(doc.DocumentElement);
            FISCA.Authentication.DSAServices.CallService("UDSManagerService.ImportPackage", req);
        }

        /// <summary>
        /// 設定活動紀錄對照表
        /// </summary>
        /// <param name="typeName">類型</param>
        /// <param name="items">細項陣列</param>
        public static void SetActivityRecordMapping(string typeName, MappingItem[] items)
        {
            //先刪除所有目前的資料
            DSXmlHelper helper = new DSXmlHelper("DeleteRequest");
            helper.AddElement(TABLE_活動表現紀錄對照表);
            helper.AddElement(TABLE_活動表現紀錄對照表, "Condition");

            XmlElement element = helper.AddElement(TABLE_活動表現紀錄對照表 + "/Condition", "Equals", typeName);
            element.SetAttribute("FieldName", "類別");

            string targetService = "UDTService.DML.Delete";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            //把新的資料加上去 .....
            helper = new DSXmlHelper("InsertRequest");

            for (int i = 0; i < items.Length; i++)
            {
                helper.AddElement(TABLE_活動表現紀錄對照表);
                helper.AddElement(TABLE_活動表現紀錄對照表 + "[" + (i + 1) + "]", "類別", typeName);
                helper.AddElement(TABLE_活動表現紀錄對照表 + "[" + (i + 1) + "]", "細項", items[i].Item);
                helper.AddElement(TABLE_活動表現紀錄對照表 + "[" + (i + 1) + "]", "數量", items[i].Count.ToString());
            }

            targetService = "UDTService.DML.Insert";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));
        }

        /// <summary>
        /// 取得活動對照表所有項目
        /// </summary>
        /// <returns>活動對照表所有項目</returns>
        public static List<MappingItem> GetActivityRecordMappingItems()
        {
            string targetService = "UDSInvokeService.Invoke";
            List<MappingItem> items = new List<MappingItem>();

            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.BaseElement.SetAttribute("Package", PACKAGE_NAME);
            helper.BaseElement.SetAttribute("Service", SERIVCE_取得活動表現紀錄對照表);

            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            foreach (XmlNode node in rsp.GetContent().BaseElement.SelectNodes("項目"))
            {
                XmlElement element = (XmlElement)node;

                int count;
                if (!int.TryParse(element.GetAttribute("數量"), out count))
                    count = 1;

                MappingItem item = new MappingItem();
                item.Item = element.GetAttribute("細項");
                item.Type = element.GetAttribute("類別");
                item.Count = count;
            }

            return items;
        }

        /// <summary>
        /// 取得活動對照表種類
        /// </summary>
        /// <returns>活動對照表種類</returns>
        public static List<string> GetActivityRecordTypes()
        {
            List<string> list = new List<string>();
            list.Add("班級幹部");
            list.Add("社團幹部");
            list.Add("學校幹部");

            return list;
        }

        public static StudentRecord GetStudent(string className, string seatNo)
        {
            ClassRecord classRecord = Class.Instance.GetClassByName(className);
            if (classRecord == null) return null;

            foreach (StudentRecord student in classRecord.Students)
            {
                if (student.SeatNo == seatNo)
                    return student;
            }
            return null;
        }

        /// <summary>
        /// 取得學生
        /// </summary>
        /// <param name="idnumber">學號</param>
        /// <returns>學生紀錄</returns>
        public static StudentRecord GetStudent(string studentNumber)
        {
            if (string.IsNullOrEmpty(studentNumber))
                return null;
            foreach (StudentRecord student in Student.Instance.Items)
            {
                if (student.StudentNumber == studentNumber)
                    return student;
            }
            return null;
        }

        /// <summary>
        /// 取得對照表
        /// </summary>
        /// <param name="type">類別</param>
        /// <returns>項目列表</returns>
        public static List<MappingItem> GetActivityRecordMappingItems(string type)
        {
            string targetService = "UDSInvokeService.Invoke";
            List<MappingItem> items = new List<MappingItem>();

            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "類別", type);

            helper.BaseElement.SetAttribute("Package", PACKAGE_NAME);
            helper.BaseElement.SetAttribute("Service", SERIVCE_取得活動表現紀錄對照表);

            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            foreach (XmlNode node in rsp.GetContent().BaseElement.SelectNodes("項目"))
            {
                XmlElement element = (XmlElement)node;

                int count;
                if (!int.TryParse(element.GetAttribute("數量"), out count))
                    count = 1;

                MappingItem item = new MappingItem();
                item.Item = element.GetAttribute("細項");
                item.Type = element.GetAttribute("類別");
                item.Count = count;
                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// 儲存班級幹部名單
        /// </summary>
        /// <param name="schoolyear">學年度</param>
        /// <param name="semester">學期</param>
        /// <param name="unit">班級名稱</param>
        /// <param name="items">項目</param>
        public static void SaveClassLeader(string schoolyear, string semester, string unit, ActivityRecordItem[] items)
        {
            DSXmlHelper helper = new DSXmlHelper("DeleteRequest");
            helper.AddElement(TABLE_活動表現紀錄);
            helper.AddElement(TABLE_活動表現紀錄, "Condition");

            XmlElement conditionElement1 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", unit);
            conditionElement1.SetAttribute("FieldName", "單位");

            XmlElement conditionElement2 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", "班級幹部");
            conditionElement2.SetAttribute("FieldName", "類別");

            XmlElement conditionElement3 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", schoolyear);
            conditionElement3.SetAttribute("FieldName", "學年度");

            XmlElement conditionElement4 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", semester);
            conditionElement4.SetAttribute("FieldName", "學期");

            string targetService = "UDTService.DML.Delete";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            helper = new DSXmlHelper("InsertRequest");

            for (int i = 0; i < items.Length; i++)
            {
                ActivityRecordItem item = items[i];

                helper.AddElement("活動表現紀錄");
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "單位", unit);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學年度", item.SchoolYear);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學期", item.Semester);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學生編號", item.StudentID);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "細項", item.Item);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "類別", "班級幹部");
            }

            targetService = "UDTService.DML.Insert";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));
        }

        /// <summary>
        /// 儲存社團幹部名單
        /// </summary>
        /// <param name="schoolyear">學年度</param>
        /// <param name="semester">學期</param>
        /// <param name="unit">社團名稱</param>
        /// <param name="items">項目</param>
        public static void SaveGroupLeader(string schoolyear, string semester, string unit, ActivityRecordItem[] items)
        {
            DSXmlHelper helper = new DSXmlHelper("DeleteRequest");
            helper.AddElement(TABLE_活動表現紀錄);
            helper.AddElement(TABLE_活動表現紀錄, "Condition");

            XmlElement conditionElement1 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", unit);
            conditionElement1.SetAttribute("FieldName", "單位");

            XmlElement conditionElement2 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", "社團幹部");
            conditionElement2.SetAttribute("FieldName", "類別");

            XmlElement conditionElement3 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", schoolyear);
            conditionElement3.SetAttribute("FieldName", "學年度");

            XmlElement conditionElement4 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", semester);
            conditionElement4.SetAttribute("FieldName", "學期");

            string targetService = "UDTService.DML.Delete";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            helper = new DSXmlHelper("InsertRequest");

            for (int i = 0; i < items.Length; i++)
            {
                ActivityRecordItem item = items[i];

                helper.AddElement("活動表現紀錄");
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "單位", unit);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學年度", item.SchoolYear);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學期", item.Semester);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學生編號", item.StudentID);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "細項", item.Item);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "類別", "社團幹部");
            }

            targetService = "UDTService.DML.Insert";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));
        }

        /// <summary>
        /// 儲存競賽項目
        /// </summary>
        /// <param name="schoolyear">學年度</param>
        /// <param name="semester">學期</param>
        /// <param name="items">學校競賽名單</param>
        public static void SaveRacing(string schoolyear, string semester, ActivityRecordItem[] items)
        {
            DSXmlHelper helper = new DSXmlHelper("DeleteRequest");
            helper.AddElement(TABLE_活動表現紀錄);
            helper.AddElement(TABLE_活動表現紀錄, "Condition");

            XmlElement conditionElement2 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", "競賽項目");
            conditionElement2.SetAttribute("FieldName", "類別");

            XmlElement conditionElement3 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", schoolyear);
            conditionElement3.SetAttribute("FieldName", "學年度");

            XmlElement conditionElement4 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", semester);
            conditionElement4.SetAttribute("FieldName", "學期");

            string targetService = "UDTService.DML.Delete";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            helper = new DSXmlHelper("InsertRequest");

            for (int i = 0; i < items.Length; i++)
            {
                ActivityRecordItem item = items[i];

                helper.AddElement("活動表現紀錄");
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學年度", item.SchoolYear);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學期", item.Semester);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學生編號", item.StudentID);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "細項", item.Item);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "類別", "競賽項目");
            }

            targetService = "UDTService.DML.Insert";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));
        }

        /// <summary>
        /// 儲存學校幹部名單
        /// </summary>
        /// <param name="schoolyear">學年度</param>
        /// <param name="semester">學期</param>
        /// <param name="items">項目</param>
        public static void SaveSchoolLeader(string schoolyear, string semester, ActivityRecordItem[] items)
        {
            DSXmlHelper helper = new DSXmlHelper("DeleteRequest");
            helper.AddElement(TABLE_活動表現紀錄);
            helper.AddElement(TABLE_活動表現紀錄, "Condition");

            XmlElement conditionElement2 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", "學校幹部");
            conditionElement2.SetAttribute("FieldName", "類別");

            XmlElement conditionElement3 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", schoolyear);
            conditionElement3.SetAttribute("FieldName", "學年度");

            XmlElement conditionElement4 = helper.AddElement(TABLE_活動表現紀錄 + "/Condition", "Equals", semester);
            conditionElement4.SetAttribute("FieldName", "學期");

            string targetService = "UDTService.DML.Delete";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            helper = new DSXmlHelper("InsertRequest");

            for (int i = 0; i < items.Length; i++)
            {
                ActivityRecordItem item = items[i];

                helper.AddElement("活動表現紀錄");
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學年度", item.SchoolYear);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學期", item.Semester);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "學生編號", item.StudentID);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "細項", item.Item);
                helper.AddElement("活動表現紀錄[" + (i + 1) + "]", "類別", "學校幹部");
            }

            targetService = "UDTService.DML.Insert";
            FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));
        }

        public static List<ActivityRecordItem> GetActivityRecordItems(string schoolyear, string semester, string type, string unit)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.BaseElement.SetAttribute("TableName", TABLE_活動表現紀錄);
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            XmlElement element = helper.AddElement("Condition", "Equals", schoolyear);
            element.SetAttribute("FieldName", "學年度");

            XmlElement element1 = helper.AddElement("Condition", "Equals", semester);
            element1.SetAttribute("FieldName", "學期");

            XmlElement element2 = helper.AddElement("Condition", "Equals", type);
            element2.SetAttribute("FieldName", "類別");

            if (!string.IsNullOrEmpty(unit))
            {
                XmlElement element3 = helper.AddElement("Condition", "Equals", unit);
                element3.SetAttribute("FieldName", "單位");
            }

            string targetService = "UDTService.DML.Select";
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            List<ActivityRecordItem> list = new List<ActivityRecordItem>();
            foreach (XmlNode node in rsp.GetContent().BaseElement.SelectNodes(TABLE_活動表現紀錄))
            {
                ActivityRecordItem item = new ActivityRecordItem();
                item.Item = node.SelectSingleNode("細項").InnerText;
                item.SchoolYear = node.SelectSingleNode("學年度").InnerText;
                item.Semester = node.SelectSingleNode("學期").InnerText;
                item.StudentID = node.SelectSingleNode("學生編號").InnerText;
                item.Type = node.SelectSingleNode("類別").InnerText;

                if (string.IsNullOrEmpty(unit))
                    item.Unit = node.SelectSingleNode("單位").InnerText;

                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 取得社團
        /// </summary>
        /// <returns>社團</returns>
        public static List<string> GetGroupList(string schoolyear,string semester)
        {
            List<string> list = new List<string>();
            Course.Instance.Items.SyncTagCache();
            foreach (CourseRecord course in Course.Instance.Items)
            {
                if (course.SchoolYear.ToString() != schoolyear) continue;
                if (course.Semester.ToString() != semester) continue;

                foreach (CourseTagRecord tag in course.GetTags())
                {                 
                    if (tag.Prefix == "聯課活動")
                        list.Add(course.Name);
                }
            }
            return list;
        }

        /// <summary>
        /// 取得學生活動紀錄
        /// </summary>
        /// <param name="studentid"></param>
        /// <returns></returns>
        public static List<ActivityRecordItem> GetActivityRecordHistory(string studentid)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.BaseElement.SetAttribute("TableName", TABLE_活動表現紀錄);
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");

            XmlElement element = helper.AddElement("Condition", "Equals", studentid);
            element.SetAttribute("FieldName", "學生編號");

            //helper.AddElement("Order");
            //helper.AddElement("Order", "學年度", "desc");
            //helper.AddElement("Order", "學期", "asc");

            string targetService = "UDTService.DML.Select";
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, new DSRequest(helper.BaseElement));

            List<ActivityRecordItem> list = new List<ActivityRecordItem>();
            foreach (XmlNode node in rsp.GetContent().BaseElement.SelectNodes(TABLE_活動表現紀錄))
            {
                ActivityRecordItem item = new ActivityRecordItem();
                item.Item = node.SelectSingleNode("細項").InnerText;
                item.SchoolYear = node.SelectSingleNode("學年度").InnerText;
                item.Semester = node.SelectSingleNode("學期").InnerText;
                item.StudentID = node.SelectSingleNode("學生編號").InnerText;
                item.Type = node.SelectSingleNode("類別").InnerText;
                item.Unit = node.SelectSingleNode("單位").InnerText;

                list.Add(item);
            }
            return list;
        }
    }
}
