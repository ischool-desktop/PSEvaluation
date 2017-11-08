using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation.Feature
{
    [FISCA.Authentication.AutoRetryOnWebException()]
    internal static class QuerySCAttend
    {
        public static List<SCAttendRecord> GetAllSCAttendRecords()
        {
            var students = Student.Instance.Items;
            var courses = Course.Instance.Items;
            DSXmlHelper helper = new DSXmlHelper("SelectRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentID");
            helper.AddElement("Field", "RefCourseID");
            helper.AddElement("Field", "IsRequired");
            helper.AddElement("Field", "RequiredBy");
            helper.AddElement("Field", "Score");
            helper.AddElement("Field", "Extension");
            helper.AddElement("Condition");
            helper.AddElement("Order");
            DSRequest dsreq = new DSRequest(helper);
            List<SCAttendRecord> result = new List<SCAttendRecord>();
            foreach (var item in FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetSCAttend", dsreq).GetContent().GetElements("Student"))
            {
                helper = new DSXmlHelper(item);
                var studentid = helper.GetText("RefStudentID");
                var courseid = helper.GetText("RefCourseID");
                var score = helper.GetText("Score");
                var effort = helper.GetText("Extension/Extension/Effort");
                var text = helper.GetText("Extension/Extension/Text");
                var id = item.GetAttribute("ID");
                bool? required = null;
                string requiredby = null;
                switch (helper.GetText("IsRequired"))
                {
                    case "必":
                        required = true;
                        break;
                    case "選":
                        required = false;
                        break;
                    default:
                        required = null;
                        break;
                }
                switch (helper.GetText("RequiredBy"))
                {
                    case "部訂":
                    case "校訂":
                        requiredby = helper.GetText("RequiredBy");
                        break;
                    default:
                        requiredby = null;
                        break;
                }
                //if ( students.ContainsKey(studentid) && courses.ContainsKey(courseid) )
                result.Add(new SCAttendRecord(studentid, courseid, id, score, effort, text));
            }
            return result;
        }
        public static List<SCAttendRecord> GetSCAttendRecords(IEnumerable<string> primaryKeys)
        {
            bool hasKey = false;
            var students = Student.Instance.Items;
            var courses = Course.Instance.Items;
            DSXmlHelper helper = new DSXmlHelper("SelectRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentID");
            helper.AddElement("Field", "RefCourseID");
            helper.AddElement("Field", "IsRequired");
            helper.AddElement("Field", "RequiredBy");
            helper.AddElement("Field", "Score");
            helper.AddElement("Field", "Extension");
            helper.AddElement("Condition");
            foreach (var item in primaryKeys)
            {
                helper.AddElement("Condition", "ID", item);
                hasKey = true;
            }
            helper.AddElement("Order");
            List<SCAttendRecord> result = new List<SCAttendRecord>();
            if (hasKey)
            {
                DSRequest dsreq = new DSRequest(helper);
                foreach (var item in FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetSCAttend", dsreq).GetContent().GetElements("Student"))
                {
                    helper = new DSXmlHelper(item);
                    var studentid = helper.GetText("RefStudentID");
                    var courseid = helper.GetText("RefCourseID");
                    var score = helper.GetText("Score");
                    var effort = helper.GetText("Extension/Extension/Effort");
                    var text = helper.GetText("Extension/Extension/Text");
                    var id = item.GetAttribute("ID");
                    bool? required = null;
                    string requiredby = null;
                    switch (helper.GetText("IsRequired"))
                    {
                        case "必":
                            required = true;
                            break;
                        case "選":
                            required = false;
                            break;
                        default:
                            required = null;
                            break;
                    }
                    switch (helper.GetText("RequiredBy"))
                    {
                        case "部訂":
                        case "校訂":
                            requiredby = helper.GetText("RequiredBy");
                            break;
                        default:
                            requiredby = null;
                            break;
                    }
                    //if ( students.ContainsKey(studentid) && courses.ContainsKey(courseid) )
                    result.Add(new SCAttendRecord(studentid, courseid, id, score, effort, text));
                }
            }
            return result;
        }
        public static List<SCAttendRecord> GetSCAttendRecords(params string[] primaryKeys)
        { return GetSCAttendRecords((IEnumerable<string>)primaryKeys); }
    }
}