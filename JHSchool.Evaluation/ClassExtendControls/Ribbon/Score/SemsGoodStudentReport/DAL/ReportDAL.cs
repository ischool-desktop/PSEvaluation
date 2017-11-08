using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport.Data;
using FISCA.DSAUtil;
using System.Xml;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport.DAL
{
    public static class ReportDAL
    {
        public static ReportData GetReportData(string schoolyear, string semester, bool exclude_abnormal)
        {
            string targetService = "SmartSchool.Score.GetSemesterSubjectScore";
            ReportData reportData = new ReportData();

            DSXmlHelper helper = new DSXmlHelper("GetSemesterSubjectScore");
            helper.AddElement("Field");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SchoolYear", schoolyear);
            helper.AddElement("Condition", "Semester", semester);
            helper.AddElement("Condition", "StudentIDList");

            foreach (K12.Data.ClassRecord classRecord in K12.Data.Class.SelectByIDs(Class.Instance.SelectedKeys))
            {
                ClassData cd = new ClassData(classRecord, exclude_abnormal);
                reportData.Classes.Add(cd);

                foreach (K12.Data.StudentRecord studentRecord in classRecord.Students)
                    helper.AddElement("Condition/StudentIDList", "ID", studentRecord.ID);
            }

            DSRequest req = new DSRequest(helper.BaseElement);
            DSResponse rsp = FISCA.Authentication.DSAServices.CallService(targetService, req);

            foreach (XmlNode node in rsp.GetContent().BaseElement.SelectNodes("SemesterSubjectScore"))
            {
                string studentid = node.SelectSingleNode("RefStudentId").InnerText;
                string scoreText = node.SelectSingleNode("ScoreInfo/LearnDomainScore").InnerText;
                decimal score;

                if (!decimal.TryParse(scoreText, out score))
                    score = 0;
                reportData.SetStudentScore(studentid, score);
            }

            reportData.SortClass();
            foreach (ClassData cd in reportData.Classes)
            {
                cd.Sort();
            }
            return reportData;
        }
    }
}
