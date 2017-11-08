using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;

namespace JHSchool.Evaluation.Feature
{
    [FISCA.Authentication.AutoRetryOnWebException()]
    internal static class QuerySCETake
    {
        internal static IEnumerable<SCETakeRecord> GetAllSCETakeRecords()
        {
            List<SCETakeRecord> result = new List<SCETakeRecord>();

            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");

            foreach (var item in FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetSCETake", new DSRequest(helper)).GetContent().GetElements("Score"))
            {
                result.Add(new SCETakeRecord(item));
            }

            return result;
        }

        internal static IEnumerable<SCETakeRecord> GetSCETakeRecords(IEnumerable<string> primaryKeys)
        {
            bool hasKey = false;
            List<SCETakeRecord> result = new List<SCETakeRecord>();

            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (var item in primaryKeys)
            {
                helper.AddElement("Condition", "ID", item);
                hasKey = true;
            }

            if (hasKey)
            {
                foreach (var item in FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetSCETake", new DSRequest(helper)).GetContent().GetElements("Score"))
                {
                    result.Add(new SCETakeRecord(item));
                }
            }

            return result;
        }

        public static IEnumerable<SCETakeRecord> GetSCETakeRecords(string courseID, string examID)
        {
            List<SCETakeRecord> result = new List<SCETakeRecord>();

            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            if (!string.IsNullOrEmpty(courseID))
                helper.AddElement("Condition", "CourseID", courseID);
            if (!string.IsNullOrEmpty(examID))
                helper.AddElement("Condition", "ExamID", examID);

            foreach (var item in FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetSCETake", new DSRequest(helper)).GetContent().GetElements("Score"))
            {
                result.Add(new SCETakeRecord(item));
            }

            return result;
        }
    }
}
