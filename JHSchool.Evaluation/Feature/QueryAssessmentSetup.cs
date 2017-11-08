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
    internal static class QueryAssessmentSetup
    {
        public static List<AssessmentSetupRecord> GetAllAssessmentSetup()
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            List<AssessmentSetupRecord> records = new List<AssessmentSetupRecord>();

            request.AddElement(".", "Field", "<ID/><TemplateName/><Description/><AllowUpload/><StartTime/><EndTime/>", true);
            string srvname = "SmartSchool.ExamTemplate.GetAbstractList";

            foreach (XmlElement each in FISCA.Authentication.DSAServices.CallService(srvname, new DSRequest(request)).GetContent().GetElements("ExamTemplate"))
                records.Add(new AssessmentSetupRecord(each));

            return records;
        }

        public static List<AssessmentSetupRecord> GetAssessmentSetups(params string[] primaryKeys)
        {
            return GetAssessmentSetups((IEnumerable<string>)primaryKeys);
        }

        public static List<AssessmentSetupRecord> GetAssessmentSetups(IEnumerable<string> primaryKeys)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            List<AssessmentSetupRecord> records = new List<AssessmentSetupRecord>();
            bool execute_required = false;

            request.AddElement(".", "Field", "<ID/><TemplateName/><Description/><AllowUpload/><StartTime/><EndTime/>", true);
            request.AddElement("Condition");

            foreach (string each in primaryKeys)
            {
                request.AddElement("Condition", "ID", each);
                execute_required = true;
            }

            string srvname = "SmartSchool.ExamTemplate.GetAbstractList";

            if (execute_required)
            {
                foreach (XmlElement each in FISCA.Authentication.DSAServices.CallService(srvname, new DSRequest(request)).GetContent().GetElements("ExamTemplate"))
                    records.Add(new AssessmentSetupRecord(each));
            }

            return records;
        }
    }
}
