using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using Framework;

namespace JHSchool.Evaluation.Feature.Legacy
{
    internal class EditTemplate 
    {
        public static string Insert(string name)
        {
            DSXmlHelper req = new DSXmlHelper("InsertRequest");
            req.AddElement("ExamTemplate");
            req.AddElement("ExamTemplate", "TemplateName", name);
            req.AddElement("ExamTemplate", "AllowUpload", "是");

            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.Insert", new DSRequest(req));

            return rsp.GetContent().GetText("NewID");
        }

        public static void Delete(string identity)
        {
            DSXmlHelper req = new DSXmlHelper("DeleteRequest");
            req.AddElement("ExamTemplate");
            req.AddElement("ExamTemplate", "ID", identity);

            FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.Delete", new DSRequest(req));
        }

        public static void Rename(string identity, string newName)
        {
            DSXmlHelper req = new DSXmlHelper("UpdateRequest");
            req.AddElement("ExamTemplate");
            req.AddElement("ExamTemplate", "TemplateName", newName);
            req.AddElement("ExamTemplate", "Condition", "<ID>" + identity + "</ID>", true);

            FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.Update", new DSRequest(req));
        }

        public static void DeleteIncludeExam(XmlElement request)
        {
            FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.DeleteIncludeExam", new DSRequest(request));
        }

        public static void InsertIncludeExam(XmlElement request)
        {
            FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.InsertIncludeExam", new DSRequest(request));
        }

        public static void UpdateIncludeExam(XmlElement request)
        {
            FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.UpdateIncludeExam", new DSRequest(request));
        }

        public static void UpdateTemplate(string identity, string name, string allowUpload, string startTime, string endTime)
        {
            DSXmlHelper req = new DSXmlHelper("Request");
            req.AddElement("ExamTemplate");
            req.AddElement("ExamTemplate", "TemplateName", name);
            req.AddElement("ExamTemplate", "AllowUpload", allowUpload);
            req.AddElement("ExamTemplate", "StartTime", startTime);
            req.AddElement("ExamTemplate", "EndTime", endTime);
            req.AddElement("ExamTemplate", "Condition");
            req.AddElement("ExamTemplate/Condition", "ID", identity);

            FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.Update", new DSRequest(req));
        }
    }
}
