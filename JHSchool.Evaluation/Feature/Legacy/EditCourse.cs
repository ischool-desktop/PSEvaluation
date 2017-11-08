using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;

namespace JHSchool.Evaluation.Feature.Legacy
{
    internal static class EditCourse
    {
        public static void UpdateAttend(DSXmlHelper request)
        {
            FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.UpdateSCAttend", new DSRequest(request));
        }

        public static void DeleteAttend(DSXmlHelper request)
        {
            FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.DeleteSCAttend", new DSRequest(request));
        }

        public static void InsertSCEScore(DSRequest request)
        {
            FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.InsertSECScore", request);
        }

        public static void UpdateSCEScore(DSRequest request)
        {
            FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.UpdateSCEScore", request);
        }

        public static void DeleteSCEScore(DSRequest request)
        {
            FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.DeleteSCEScore", request);
        }
    }
}
