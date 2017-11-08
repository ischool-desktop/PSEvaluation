using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using Framework;

namespace JHSchool.Evaluation.Feature
{
    [FISCA.Authentication.AutoRetryOnWebException()]
    internal static class QueryExam
    {
        public static List<ExamRecord> GetAllExams()
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            List<ExamRecord> records = new List<ExamRecord>();

            request.AddElement(".", "Field", "<All/>", true);
            string srvname = "SmartSchool.Exam.GetAbstractList";

            foreach (XmlElement each in FISCA.Authentication.DSAServices.CallService(srvname, new DSRequest(request)).GetContent().GetElements("Exam"))
                records.Add(new ExamRecord(each));

            return records;
        }

        public static List<ExamRecord> GetExams(params string[] primaryKeys)
        {
            return GetExams((IEnumerable<string>)primaryKeys);
        }

        public static List<ExamRecord> GetExams(IEnumerable<string> primaryKeys)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            List<ExamRecord> records = new List<ExamRecord>();
            bool execute_required = false;

            request.AddElement(".", "Field", "<All/>", true);
            request.AddElement("Condition");

            foreach (string each in primaryKeys)
            {
                request.AddElement("Condition", "ID", each);
                execute_required = true;
            }

            string srvname = "SmartSchool.Exam.GetAbstractList";

            if (execute_required)
            {
                foreach (XmlElement each in FISCA.Authentication.DSAServices.CallService(srvname, new DSRequest(request)).GetContent().GetElements("Exam"))
                    records.Add(new ExamRecord(each));
            }

            return records;
        }
    }
}
