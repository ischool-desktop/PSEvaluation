using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;

namespace JHSchool.Evaluation.Feature
{
    [FISCA.Authentication.AutoRetryOnWebException()]
    internal static class QueryAEInclude
    {
        internal static IEnumerable<AEIncludeRecord> GetAllAEIncludeRecords()
        {
            List<AEIncludeRecord> result = new List<AEIncludeRecord>();

            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");

            foreach (var item in FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.GetIncludeExamList", new DSRequest(helper)).GetContent().GetElements("IncludeExam"))
            {
                result.Add(new AEIncludeRecord(item));
            }

            return result;
        }

        internal static IEnumerable<AEIncludeRecord> GetAEIncludeRecords(IEnumerable<string> primaryKeys)
        {
            bool hasKey = false;
            List<AEIncludeRecord> result = new List<AEIncludeRecord>();

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
                foreach (var item in FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.GetIncludeExamList", new DSRequest(helper)).GetContent().GetElements("IncludeExam"))
                {
                    result.Add(new AEIncludeRecord(item));
                }
            }
            return result;
        }
    }
}
