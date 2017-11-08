using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.Editor;
using FISCA.DSAUtil;
using System.Xml;
using Framework;

namespace JHSchool.Evaluation.Feature
{
    internal static class EditProgramPlan
    {
        internal static void SaveProgramPlanRecordEditor(params ProgramPlanRecordEditor[] editors)
        {
            SaveProgramPlanRecordEditor((IEnumerable<ProgramPlanRecordEditor>)editors);
        }

        internal static void SaveProgramPlanRecordEditor(IEnumerable<ProgramPlanRecordEditor> editors)
        {
            DSXmlHelper insertReq = new DSXmlHelper("Request");
            DSXmlHelper updateReq = new DSXmlHelper("Request");
            DSXmlHelper deleteReq = new DSXmlHelper("Request");
            deleteReq.AddElement(".", "GraduationPlan");

            //同步清單，裡面包含了 graduation plan 的系統編號。
            List<string> synclist = new List<string>();

            bool do_insert = false, do_update = false, do_delete = false;

            foreach (ProgramPlanRecordEditor editor in editors)
            {
                if (editor.EditorStatus != JHSchool.Editor.EditorStatus.NoChanged)
                    synclist.Add(editor.ID);

                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                {
                    insertReq.AddElement("GraduationPlan");
                    insertReq.AddElement("GraduationPlan", "Name", editor.Name);
                    insertReq.AddElement("GraduationPlan", "Content", "<GraduationPlan></GraduationPlan>", true);
                    foreach (var subject in editor.Subjects)
                    {
                        DSXmlHelper helper = new DSXmlHelper("Subject");
                        helper.SetAttribute(".", "GradeYear", subject.GradeYear);
                        helper.SetAttribute(".", "Semester", subject.Semester);
                        helper.SetAttribute(".", "Credit", subject.Credit);
                        helper.SetAttribute(".", "Period", subject.Period);
                        helper.SetAttribute(".", "Domain", subject.Domain);
                        helper.SetAttribute(".", "FullName", subject.FullName);
                        helper.SetAttribute(".", "Level", subject.Level);
                        helper.SetAttribute(".", "CalcFlag", "" + subject.CalcFlag);
                        helper.SetAttribute(".", "SubjectName", subject.SubjectName);
                        helper.AddElement("Grouping");
                        helper.SetAttribute("Grouping", "RowIndex", "" + subject.RowIndex);
                        insertReq.AddElement("GraduationPlan/Content/GraduationPlan", helper.BaseElement);
                    }

                    do_insert = true;
                    //<GraduationPlan>
                    //<Subject Category="一般科目" Credit="2" Domain="外國語文" 
                    //Entry="學業" FullName="ESL" GradeYear="2" Level="" 
                    //NotIncludedInCalc="False" NotIncludedInCredit="False" 
                    //Required="必修" RequiredBy="校訂" Semester="2" SubjectName="ESL">
                    //<Grouping RowIndex="1" startLevel=""/>
                    //</Subject>
                    //</GraduationPlan>
                }
                else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                {
                    updateReq.AddElement("GraduationPlan");
                    updateReq.AddElement("GraduationPlan", "Field");
                    updateReq.AddElement("GraduationPlan/Field", "Name", editor.Name);
                    updateReq.AddElement("GraduationPlan/Field", "Content", "<GraduationPlan></GraduationPlan>", true);
                    foreach (var subject in editor.Subjects)
                    {
                        DSXmlHelper helper = new DSXmlHelper("Subject");
                        helper.SetAttribute(".", "GradeYear", subject.GradeYear);
                        helper.SetAttribute(".", "Semester", subject.Semester);
                        helper.SetAttribute(".", "Credit", subject.Credit);
                        helper.SetAttribute(".", "Period", subject.Period);
                        helper.SetAttribute(".", "Domain", subject.Domain);
                        helper.SetAttribute(".", "FullName", subject.FullName);
                        helper.SetAttribute(".", "Level", subject.Level);
                        helper.SetAttribute(".", "CalcFlag", "" + subject.CalcFlag);
                        helper.SetAttribute(".", "SubjectName", subject.SubjectName);
                        helper.AddElement("Grouping");
                        helper.SetAttribute("Grouping", "RowIndex", "" + subject.RowIndex);
                        updateReq.AddElement("GraduationPlan/Field/Content/GraduationPlan", helper.BaseElement);
                    }
                    updateReq.AddElement("GraduationPlan", "Condition");
                    updateReq.AddElement("GraduationPlan/Condition", "ID", editor.ID);

                    do_update = true;
                }
                else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                {
                    deleteReq.AddElement("GraduationPlan", "ID", editor.ID);

                    do_delete = true;
                }
            }

            if (do_insert)
            {
                DSXmlHelper response = FISCA.Authentication.DSAServices.CallService("SmartSchool.GraduationPlan.Insert", new DSRequest(insertReq)).GetContent();
                foreach (XmlElement each in response.GetElements("NewID"))
                    synclist.Add(each.InnerText);
            }
            if (do_update) FISCA.Authentication.DSAServices.CallService("SmartSchool.GraduationPlan.Update", new DSRequest(updateReq));
            if (do_delete) FISCA.Authentication.DSAServices.CallService("SmartSchool.GraduationPlan.Delete", new DSRequest(deleteReq));

            if (synclist.Count > 0)
                ProgramPlan.Instance.SyncDataBackground(synclist);
        }
    }
}
