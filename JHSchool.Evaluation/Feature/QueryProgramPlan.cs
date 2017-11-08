using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using Framework;

namespace JHSchool.Evaluation.Feature
{
    [FISCA.Authentication.AutoRetryOnWebException()]
    internal static class QueryProgramPlan
    {
        public static List<ProgramPlanRecord> GetAllProgramPlans()
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            List<ProgramPlanRecord> records = new List<ProgramPlanRecord>();

            request.AddElement(".", "Field", "<ID/><Name/><Content/>",true);
            request.AddElement(".", "Order", "<Name/>",true);

            string srvname = "SmartSchool.GraduationPlan.GetDetailList";

            foreach (XmlElement each in FISCA.Authentication.DSAServices.CallService(srvname, new DSRequest(request)).GetContent().GetElements("GraduationPlan"))
                records.Add(new ProgramPlanRecord(each));

            return records;
        }

        public static List<ProgramPlanRecord> GetProgramPlans(params string[] primaryKeys)
        {
            return GetProgramPlans((IEnumerable<string>)primaryKeys);
        }

        public static List<ProgramPlanRecord> GetProgramPlans(IEnumerable<string> primaryKeys)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            List<ProgramPlanRecord> records = new List<ProgramPlanRecord>();
            bool execute_required = false;

            request.AddElement(".", "Field", "<ID/><Name/><Content/>", true);
            request.AddElement("Condition");
            request.AddElement("Condition", "IDList");

            foreach (string each in primaryKeys)
            {
                request.AddElement("Condition/IDList", "ID", each);
                execute_required = true;
            }

            request.AddElement(".", "Order", "<Name/>", true);

            string srvname = "SmartSchool.GraduationPlan.GetDetailList";

            if (execute_required)
            {
                foreach (XmlElement each in FISCA.Authentication.DSAServices.CallService(srvname, new DSRequest(request)).GetContent().GetElements("GraduationPlan"))
                    records.Add(new ProgramPlanRecord(each));
            }

            return records;
        }
    }
}
