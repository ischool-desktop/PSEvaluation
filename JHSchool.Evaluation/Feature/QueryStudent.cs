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
    internal static class QueryStudent
    {
        public static List<SemesterScoreRecord> GetSemesterScores(params string[] primaryKeys)
        {
            return GetSemesterScores((IEnumerable<string>)primaryKeys);
        }

        public static List<SemesterScoreRecord> GetSemesterScores(IEnumerable<string> primaryKeys)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterSubjectScore");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "StudentIDList");
            foreach (var each in primaryKeys)
                helper.AddElement("Condition/StudentIDList", "ID", each);

            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Score.GetSemesterSubjectScore", new DSRequest(helper));

            List<SemesterScoreRecord> result = new List<SemesterScoreRecord>();
            foreach (XmlElement element in rsp.GetContent().GetElements("SemesterSubjectScore"))
            {
                SemesterScoreRecord record = new SemesterScoreRecord(element);
                result.Add(record);
            }
            return result;
        }

        public static List<GradScoreRecord> GetGradScores(params string[] primaryKeys)
        {
            return GetGradScores((IEnumerable<string>)primaryKeys);
        }

        internal static List<GradScoreRecord> GetGradScores(IEnumerable<string> primaryKeys)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "GradScore");
            helper.AddElement("Condition");
            foreach (var each in primaryKeys)
                helper.AddElement("Condition", "ID", each);

            DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Student.GetDetailList", new DSRequest(helper));

            List<GradScoreRecord> result = new List<GradScoreRecord>();
            foreach (XmlElement element in rsp.GetContent().GetElements("Student"))
            {
                GradScoreRecord record = new GradScoreRecord(element);
                result.Add(record);
            }
            return result;
        }
    }
}
