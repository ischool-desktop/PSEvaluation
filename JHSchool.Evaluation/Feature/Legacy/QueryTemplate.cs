using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using Framework;

namespace JHSchool.Evaluation.Feature.Legacy
{
    [FISCA.Authentication.AutoRetryOnWebException()]
    internal class QueryTemplate
    {
        public static XmlElement GetAbstractList()
        {
            DSXmlHelper req = new DSXmlHelper("GetAbstractListRequest");
            req.AddElement("Field");
            req.AddElement("Field", "All");

            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.GetAbstractList", new DSRequest(req));

            return rsp.GetContent().BaseElement;
        }

        public static XmlElement GetTempalteInfo(string id)
        {
            DSXmlHelper req = new DSXmlHelper("GetAbstractListRequest");
            req.AddElement("Field");
            req.AddElement("Field", "All");
            req.AddElement("Condition");
            req.AddElement("Condition", "ID", id);

            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.GetAbstractList", new DSRequest(req));

            return rsp.GetContent().BaseElement;
        }

        public static XmlElement GetIncludeExamList(string templateId)
        {
            DSXmlHelper req = new DSXmlHelper("GetTemplateExamListRequest");
            req.AddElement("Field");
            req.AddElement("Field", "All");
            req.AddElement("Condition");
            req.AddElement("Condition", "ExamTemplateID", templateId);

            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.GetIncludeExamList", new DSRequest(req));

            return rsp.GetContent().BaseElement;
        }

        public static XmlElement GetIncludeExamList()
        {
            DSXmlHelper req = new DSXmlHelper("GetTemplateExamListRequest");
            req.AddElement("Field");
            req.AddElement("Field", "All");

            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.GetIncludeExamList", new DSRequest(req));

            return rsp.GetContent().BaseElement;
        }
    }
}
