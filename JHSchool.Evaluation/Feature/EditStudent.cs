using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;
using System.Xml;

namespace JHSchool.Evaluation.Feature
{
    internal class EditStudent
    {
        public static void SaveSemesterScoreRecordEditor(IEnumerable<JHSchool.Evaluation.Editor.SemesterScoreRecordEditor> editors)
        {
            Dictionary<string, List<JHSchool.Evaluation.Editor.SemesterScoreRecordEditor>> grouped = new Dictionary<string, List<JHSchool.Evaluation.Editor.SemesterScoreRecordEditor>>();
            foreach (var editor in editors)
            {
                if (!grouped.ContainsKey(editor.RefStudentID))
                    grouped.Add(editor.RefStudentID, new List<JHSchool.Evaluation.Editor.SemesterScoreRecordEditor>());
                grouped[editor.RefStudentID].Add(editor);
            }
            SemesterScore.Instance.SyncData(grouped.Keys);

            DSXmlHelper insertHelper = new DSXmlHelper("InsertRequest");
            DSXmlHelper updateHelper = new DSXmlHelper("UpdateRequest");
            DSXmlHelper deleteHelper = new DSXmlHelper("DeleteRequest");

            bool do_insert = false, do_update = false, do_delete = false;
            List<string> synclist = new List<string>();

            foreach (var editor in editors)
            {
                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.NoChanged) continue;

                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                {
                    DSXmlHelper partialInsertHelper = new DSXmlHelper("SemesterSubjectScore");
                    partialInsertHelper.AddElement(".", "RefStudentId", editor.RefStudentID);
                    partialInsertHelper.AddElement(".", "SchoolYear", "" + editor.SchoolYear);
                    partialInsertHelper.AddElement(".", "Semester", "" + editor.Semester);
                    partialInsertHelper.AddElement(".", "GradeYear", "" + editor.GradeYear);
                    partialInsertHelper.AddElement(".", "ScoreInfo");

                    partialInsertHelper.AddElement("ScoreInfo", "SemesterSubjectScoreInfo");
                    foreach (var subjectKey in editor.Subjects.Keys)
                    {
                        SubjectScore subject = editor.Subjects[subjectKey];
                        XmlElement element = partialInsertHelper.AddElement("ScoreInfo/SemesterSubjectScoreInfo", "Subject");
                        element.SetAttribute("領域", subject.Domain);
                        element.SetAttribute("科目", subjectKey);
                        element.SetAttribute("節數", "" + subject.Period);
                        element.SetAttribute("權數", "" + subject.Credit);
                        element.SetAttribute("成績", "" + subject.Score);
                        element.SetAttribute("努力程度", "" + subject.Effort);
                        element.SetAttribute("文字描述", "" + subject.Text);
                        element.SetAttribute("註記", "" + subject.Comment);
                    }

                    partialInsertHelper.AddElement("ScoreInfo", "Domains");
                    foreach (var domainKey in editor.Domains.Keys)
                    {
                        DomainScore domain = editor.Domains[domainKey];
                        XmlElement element = partialInsertHelper.AddElement("ScoreInfo/Domains", "Domain");
                        element.SetAttribute("領域", domainKey);
                        element.SetAttribute("節數", "" + domain.Period);
                        element.SetAttribute("權數", "" + domain.Credit);
                        element.SetAttribute("成績", "" + domain.Score);
                        element.SetAttribute("努力程度", "" + domain.Effort);
                        element.SetAttribute("文字描述", "" + domain.Text);
                        element.SetAttribute("註記", "" + domain.Comment);
                    }

                    partialInsertHelper.AddElement("ScoreInfo", "LearnDomainScore", "" + editor.LearnDomainScore);
                    partialInsertHelper.AddElement("ScoreInfo", "CourseLearnScore", "" + editor.CourseLearnScore);

                    insertHelper.AddElement(".", partialInsertHelper.BaseElement);

                    do_insert = true;
                }
                else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                {
                    DSXmlHelper partialUpdateHelper = new DSXmlHelper("SemesterSubjectScore");
                    partialUpdateHelper.AddElement("Field");
                    partialUpdateHelper.AddElement("Field", "RefStudentId", editor.RefStudentID);
                    partialUpdateHelper.AddElement("Field", "SchoolYear", "" + editor.SchoolYear);
                    partialUpdateHelper.AddElement("Field", "Semester", "" + editor.Semester);
                    partialUpdateHelper.AddElement("Field", "GradeYear", "" + editor.GradeYear);
                    partialUpdateHelper.AddElement("Field", "ScoreInfo");

                    partialUpdateHelper.AddElement("Field/ScoreInfo", "SemesterSubjectScoreInfo");
                    foreach (var subjectKey in editor.Subjects.Keys)
                    {
                        SubjectScore subject = editor.Subjects[subjectKey];
                        XmlElement element = partialUpdateHelper.AddElement("Field/ScoreInfo/SemesterSubjectScoreInfo", "Subject");
                        element.SetAttribute("領域", subject.Domain);
                        element.SetAttribute("科目", subjectKey);
                        element.SetAttribute("節數", "" + subject.Period);
                        element.SetAttribute("權數", "" + subject.Credit);
                        element.SetAttribute("成績", "" + subject.Score);
                        element.SetAttribute("努力程度", "" + subject.Effort);
                        element.SetAttribute("文字描述", "" + subject.Text);
                        element.SetAttribute("註記", "" + subject.Comment);
                    }

                    partialUpdateHelper.AddElement("Field/ScoreInfo", "Domains");
                    foreach (var domainKey in editor.Domains.Keys)
                    {
                        DomainScore domain = editor.Domains[domainKey];
                        XmlElement element = partialUpdateHelper.AddElement("Field/ScoreInfo/Domains", "Domain");
                        element.SetAttribute("領域", domainKey);
                        element.SetAttribute("節數", "" + domain.Period);
                        element.SetAttribute("權數", "" + domain.Credit);
                        element.SetAttribute("成績", "" + domain.Score);
                        element.SetAttribute("努力程度", "" + domain.Effort);
                        element.SetAttribute("文字描述", "" + domain.Text);
                        element.SetAttribute("註記", "" + domain.Comment);
                    }

                    partialUpdateHelper.AddElement("Field/ScoreInfo", "LearnDomainScore", "" + editor.LearnDomainScore);
                    partialUpdateHelper.AddElement("Field/ScoreInfo", "CourseLearnScore", "" + editor.CourseLearnScore);

                    partialUpdateHelper.AddElement("Condition");
                    partialUpdateHelper.AddElement("Condition", "ID", editor.ID);

                    updateHelper.AddElement(".", partialUpdateHelper.BaseElement);

                    do_update = true;
                    synclist.Add(editor.ID);
                }
                else
                {
                    deleteHelper.AddElement("SemesterSubjectScore");
                    deleteHelper.AddElement("SemesterSubjectScore", "ID", editor.ID);

                    do_delete = true;
                    synclist.Add(editor.ID);
                }
            }

            if (do_insert)
            {
                DSXmlHelper response = FISCA.Authentication.DSAServices.CallService("SmartSchool.Score.InsertSemesterSubjectScore", new DSRequest(insertHelper)).GetContent();
                foreach (XmlElement each in response.GetElements("NewID"))
                    synclist.Add(each.InnerText);
            }

            if (do_update)
                FISCA.Authentication.DSAServices.CallService("SmartSchool.Score.UpdateSemesterSubjectScore", new DSRequest(updateHelper));

            if (do_delete)
                FISCA.Authentication.DSAServices.CallService("SmartSchool.Score.DeleteSemesterSubjectScore", new DSRequest(deleteHelper));

            SemesterScore.Instance.SyncDataBackground(synclist.ToArray());
        }

        internal static void SaveGradScoreRecordEditor(IEnumerable<JHSchool.Evaluation.Editor.GradScoreRecordEditor> editors)
        {
            DSXmlHelper updateHelper = new DSXmlHelper("UpdateStudentList");

            bool do_update = false;
            List<string> synclist = new List<string>();

            foreach (var editor in editors)
            {
                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.NoChanged) continue;

                DSXmlHelper partialUpdateHelper = new DSXmlHelper("Student");
                partialUpdateHelper.AddElement("Field");
                partialUpdateHelper.AddElement("Field", "GradScore");
                partialUpdateHelper.AddElement("Field/GradScore", "GradScore");
                partialUpdateHelper.AddElement("Field/GradScore/GradScore", "LearnDomainScore", "" + editor.LearnDomainScore);
                partialUpdateHelper.AddElement("Field/GradScore/GradScore", "CourseLearnScore", "" + editor.CourseLearnScore);
                foreach (var domain in editor.Domains.Values)
                {
                    XmlElement domainElement = partialUpdateHelper.AddElement("Field/GradScore/GradScore", "Domain");
                    domainElement.SetAttribute("Name", domain.Domain);
                    domainElement.SetAttribute("Score", "" + domain.Score);
                }

                //<GradScore>
                //<Domain Name="語文" Score="90"/>
                //<Domain Name="數學" Score="90"/>
                //<LearnDomainScore>90</LearnDomainScore>
                //<CourseLearnScore>80</CourseLearnScore>
                //</GradScore>

                partialUpdateHelper.AddElement("Condition");
                partialUpdateHelper.AddElement("Condition", "ID", editor.RefStudentID);

                updateHelper.AddElement(".", partialUpdateHelper.BaseElement);

                do_update = true;
                synclist.Add(editor.RefStudentID);
            }

            if (do_update)
                FISCA.Authentication.DSAServices.CallService("SmartSchool.Student.Update", new DSRequest(updateHelper));

            GradScore.Instance.SyncDataBackground(synclist.ToArray());
        }
    }
}
