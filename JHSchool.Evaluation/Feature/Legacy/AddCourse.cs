using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation.Feature.Legacy
{
    internal class AddCourse
    {
        public static DSResponse AttendCourse(FISCA.DSAUtil.DSXmlHelper request)
        {
            return FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.InsertSCAttend", new DSRequest(request));
        }
    }
}
