using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;

namespace JHSchool.Evaluation.Feature.Legacy
{
    [FISCA.Authentication.AutoRetryOnWebException()]
    internal static class QueryCourse
    {
        public static DSResponse GetSCAttend(params string[] courseid)
        {
            DSXmlHelper helper = new DSXmlHelper("SelectRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string var in courseid)
            {
                helper.AddElement("Condition", "CourseID", var);
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "ClassName", "ASC");
            helper.AddElement("Order", "SeatNumber", "ASC");
            DSRequest dsreq = new DSRequest(helper);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetSCAttend", dsreq);
            return rsp;
        }

        public static DSResponse GetSECTake(params string[] courseid)
        {
            DSXmlHelper helper = new DSXmlHelper("ScoreRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string id in courseid)
            {
                helper.AddElement("Condition", "CourseID", id);
            }
            DSRequest dsreq = new DSRequest(helper);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetSCETake", dsreq);
            return rsp;
        }

        public static DSResponse GetCourseDetail(params int[] courseid)
        {
            DSXmlHelper helper = new DSXmlHelper("GetDetailListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");

            foreach (int each in courseid)
                helper.AddElement("Condition", "ID", each.ToString());

            helper.AddElement("Order");
            helper.AddElement("Order", "CourseName");
            helper.AddElement("Order", "ID");
            helper.AddElement("Order", "Sequence");

            DSRequest dsreq = new DSRequest(helper);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetDetailList", dsreq);
            return rsp;
        }

        public static DSResponse GetExamList()
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            DSRequest dsreq = new DSRequest(helper);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.GetAbstractList", dsreq);
            return rsp;
        }

        public static DSResponse GetCourseExam(params string[] courseid)
        {
            if (courseid.Length == 0) throw new Exception("必須傳入至少一筆課程");
            DSXmlHelper helper = new DSXmlHelper("GetScoreTypeRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string id in courseid)
            {
                helper.AddElement("Condition", "ID", id);
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "DisplayOrder");
            DSRequest dsreq = new DSRequest(helper);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetScoreType", dsreq);
            return rsp;
        }

        public static DSResponse GetCourseById(params string[] courseId)
        {
            DSXmlHelper helper = new DSXmlHelper("GetDetailListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string id in courseId)
            {
                helper.AddElement("Condition", "ID", id);
            }
            DSRequest dsreq = new DSRequest(helper);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetDetailList", dsreq);
            return rsp;
        }

        public static DSResponse GetSCAttendBrief(params string[] courseid)
        {
            DSXmlHelper helper = new DSXmlHelper("SelectRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentID");
            helper.AddElement("Field", "RefCourseID");
            helper.AddElement("Field", "ClassName");
            helper.AddElement("Field", "StudentNumber");
            helper.AddElement("Field", "SeatNumber");
            helper.AddElement("Field", "Name");
            helper.AddElement("Field", "Score");
            helper.AddElement("Field", "Extension");
            helper.AddElement("Condition");
            foreach (string var in courseid)
            {
                helper.AddElement("Condition", "CourseID", var);
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "ClassName", "ASC");
            helper.AddElement("Order", "SeatNumber", "ASC");
            DSRequest dsreq = new DSRequest(helper);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.GetSCAttend", dsreq);
            return rsp;
        }
    }
}
