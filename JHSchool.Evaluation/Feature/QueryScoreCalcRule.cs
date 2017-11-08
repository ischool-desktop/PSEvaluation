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
    internal static class QueryScoreCalcRule
    {
        public static List<ScoreCalcRuleRecord> GetAllScoreCalcRules()
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            List<ScoreCalcRuleRecord> records = new List<ScoreCalcRuleRecord>();

            request.AddElement(".", "Field", "<ID/><Name/><Content/>", true);
            request.AddElement(".", "Order", "<Name/>", true);
            string srvname = "SmartSchool.ScoreCalcRule.GetScoreCalcRule";

            foreach (XmlElement each in FISCA.Authentication.DSAServices.CallService(srvname, new DSRequest(request)).GetContent().GetElements("ScoreCalcRule"))
                records.Add(new ScoreCalcRuleRecord(each));

            return records;
        }

        public static List<ScoreCalcRuleRecord> GetScoreCalcRules(params string[] primaryKeys)
        {
            return GetScoreCalcRules((IEnumerable<string>)primaryKeys);
        }

        public static List<ScoreCalcRuleRecord> GetScoreCalcRules(IEnumerable<string> primaryKeys)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            List<ScoreCalcRuleRecord> records = new List<ScoreCalcRuleRecord>();
            bool execute_required = false;

            request.AddElement(".", "Field", "<ID/><Name/><Content/>", true);
            request.AddElement(".", "Order", "<Name/>", true);

            request.AddElement(".", "Condition", "<IDList/>", true);
            foreach (string each in primaryKeys)
            {
                request.AddElement("Condition/IDList", "ID", each);
                execute_required = true;
            }

            string srvname = "SmartSchool.ScoreCalcRule.GetScoreCalcRule";

            if (execute_required)
            {
                foreach (XmlElement each in FISCA.Authentication.DSAServices.CallService(srvname, new DSRequest(request)).GetContent().GetElements("ScoreCalcRule"))
                    records.Add(new ScoreCalcRuleRecord(each));
            }

            return records;
        }
    }
}
