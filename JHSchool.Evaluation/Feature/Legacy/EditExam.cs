using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using Framework;

namespace JHSchool.Evaluation.Feature.Legacy
{
    internal class EditExam
    {
        public static void Insert(DSXmlHelper helper)
        {
            FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.Insert", new DSRequest(helper));
        }

        public static void Update(DSXmlHelper helper)
        {

            FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.Update", new DSRequest(helper));
        }

        public static void Delete(List<string> idList)
        {
            DSXmlHelper helper = new DSXmlHelper("DeleteRequest");
            foreach (string id in idList)
            {
                helper.AddElement("Exam");
                helper.AddElement("Exam", "ID", id);                
            }
            FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.Delete", new DSRequest(helper));
        }
    }
}
