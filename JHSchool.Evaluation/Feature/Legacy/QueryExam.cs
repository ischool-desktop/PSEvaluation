using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using Framework;

namespace JHSchool.Evaluation.Feature.Legacy
{
    [FISCA.Authentication.AutoRetryOnWebException()]
    internal class QueryExam
    {
        public static XmlElement GetAbstractList()
        {
            DSXmlHelper req = new DSXmlHelper("GetAbstractListRequest");
            req.AddElement("Field");
            req.AddElement("Field", "All");
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.GetAbstractList", new DSRequest(req));

            return rsp.GetContent().BaseElement;
        }

        public static DSResponse GetCourseBelong(string examid, int schoolYear, int semester)
        {
            DSXmlHelper req = new DSXmlHelper("Request");
            req.AddElement("Field");
            req.AddElement("Field", "All");
            req.AddElement("Condition");
            req.AddElement("Condition", "RefExamID", examid);
            req.AddElement("Condition", "SchoolYear", schoolYear.ToString());
            req.AddElement("Condition", "Semester", semester.ToString());
            return FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.GetCourseBelong", new DSRequest(req));
        }

        public static int GetExamTemplateUseCount(string examid)
        {
            DSXmlHelper req = new DSXmlHelper("Request");
            req.AddElement(".", "ID", examid);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.GetExamTemplateUseCount", new DSRequest(req));

            return int.Parse(rsp.GetContent().GetElement("Count").InnerText);
        }

        public static int GetTextScoreUseCount(string examid)
        {
            DSXmlHelper req = new DSXmlHelper("Request");
            req.AddElement(".", "ID", examid);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.GetTextScoreUseCount", new DSRequest(req));

            return int.Parse(rsp.GetContent().GetElement("Count").InnerText);
        }

        public static int GetNumberScoreUseCount(string examid)
        {
            DSXmlHelper req = new DSXmlHelper("Request");
            req.AddElement(".", "ID", examid);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.GetNumberScoreUseCount", new DSRequest(req));

            return int.Parse(rsp.GetContent().GetElement("Count").InnerText);
        }

    }
}