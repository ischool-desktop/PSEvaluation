using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.Editor;
using Framework;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation.Feature
{
    internal static class EditAEInclude
    {
        public static void SaveAEIncludeRecordEditor(IEnumerable<AEIncludeRecordEditor> editors)
        {
            MultiThreadWorker<AEIncludeRecordEditor> worker = new MultiThreadWorker<AEIncludeRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<AEIncludeRecordEditor> e)
            {
                DSXmlHelper insertHelper = new DSXmlHelper("InsertRequest");
                DSXmlHelper updateHelper = new DSXmlHelper("UpdateRequest");
                DSXmlHelper deleteHelper = new DSXmlHelper("DeleteRequest");
                deleteHelper.AddElement("IncludeExam");
                List<LogInfo> logs = new List<LogInfo>();
                List<string> synclist = new List<string>();
                bool hasInsert = false;
                bool hasUpdate = false;
                bool hasRemove = false;
                foreach (var editor in e.List)
                {
                    if (editor.EditorStatus != JHSchool.Editor.EditorStatus.NoChanged)
                    {
                        if (!string.IsNullOrEmpty(editor.ID))
                            synclist.Add(editor.ID);
                    }

                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        insertHelper.AddElement("IncludeExam");
                        insertHelper.AddElement("IncludeExam", "RefExamTemplateID", editor.RefAssessmentSetupID);
                        insertHelper.AddElement("IncludeExam", "RefExamID", editor.RefExamID);
                        insertHelper.AddElement("IncludeExam", "UseScore", editor.UseScore ? "是" : "否");
                        
                        
                        insertHelper.AddElement("IncludeExam", "Weight", "" + editor.Weight);
                        insertHelper.AddElement("IncludeExam", "StartTime", editor.StartTime);
                        insertHelper.AddElement("IncludeExam", "EndTime", editor.EndTime);

                        insertHelper.AddElement("IncludeExam", "Extension");
                        insertHelper.AddElement("IncludeExam/Extension", "Extension");
                        insertHelper.AddElement("IncludeExam/Extension/Extension", "UseEffort", editor.UseEffort ? "是" : "否");
                        insertHelper.AddElement("IncludeExam/Extension/Extension", "UseText", editor.UseText ? "是" : "否");

                        #region 參考
                        //<InsertIncludeExamRequest>
                        //   <IncludeExam>
                        //      <RefExamTemplateID>integer</RefExamTemplateID>
                        //      <RefExamID>integer</RefExamID>
                        //      <UseScore>是否</UseScore>
                        //      <UseText>是否</UseText>
                        //      <Weight>integer</Weight>
                        //      <OpenTeacherAccess>是否</OpenTeacherAccess>
                        //      <StartTime>timestamp</StartTime>
                        //      <EndTime>timestamp</EndTime>
                        //      <InputRequired/>
                        //   </IncludeExam>
                        //</InsertIncludeExamRequest>
                        #endregion

                        hasInsert = true;
                    }
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                    {
                        updateHelper.AddElement("IncludeExam");
                        updateHelper.AddElement("IncludeExam", "RefExamTemplateID", editor.RefAssessmentSetupID);
                        updateHelper.AddElement("IncludeExam", "RefExamID", editor.RefExamID);
                        updateHelper.AddElement("IncludeExam", "UseScore", editor.UseScore ? "是" : "否");
                        updateHelper.AddElement("IncludeExam", "Weight", "" + editor.Weight);
                        updateHelper.AddElement("IncludeExam", "StartTime", editor.StartTime);
                        updateHelper.AddElement("IncludeExam", "EndTime", editor.EndTime);

                        updateHelper.AddElement("IncludeExam", "Extension");
                        updateHelper.AddElement("IncludeExam/Extension", "Extension");
                        updateHelper.AddElement("IncludeExam/Extension/Extension", "UseEffort", editor.UseEffort ? "是" : "否");
                        updateHelper.AddElement("IncludeExam/Extension/Extension", "UseText", editor.UseText ? "是" : "否");

                        updateHelper.AddElement("IncludeExam", "Condition");
                        updateHelper.AddElement("IncludeExam/Condition", "ID", editor.ID);

                        #region 參考
                        //<UpdateIncludeExamRequest>
                        //   <IncludeExam>
                        //      <RefExamID>integer</RefExamID>
                        //      <RefExamTemplateID></RefExamTemplateID>
                        //      <UseScore>是否</UseScore>
                        //      <UseText>是否</UseText>
                        //      <Weight>integer</Weight>
                        //      <OpenTeacherAccess>是否</OpenTeacherAccess>
                        //      <StartTime>timestamp</StartTime>
                        //      <EndTime>timestamp</EndTime>
                        //      <Condition>
                        //         <ID>integer</ID>
                        //      </Condition>
                        //      <InputRequired/>
                        //   </IncludeExam>
                        //</UpdateIncludeExamRequest>
                        #endregion

                        hasUpdate = true;
                    }
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                    {
                        deleteHelper.AddElement("IncludeExam", "ID", editor.ID);

                        hasRemove = true;
                    }
                }
                if (hasInsert)
                {
                    DSResponse resp = FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.InsertIncludeExam", new DSRequest(insertHelper.BaseElement));
                    foreach (var item in resp.GetContent().GetElements("NewID"))
                    {
                        synclist.Add(item.InnerText);
                    }
                }
                if (hasUpdate)
                {
                    FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.UpdateIncludeExam", new DSRequest(updateHelper.BaseElement));
                }
                if (hasRemove)
                {
                    FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.DeleteIncludeExam", new DSRequest(deleteHelper.BaseElement));
                }

                if (synclist.Count > 0)
                    AEInclude.Instance.SyncData(synclist);
            };
            List<PackageWorkEventArgs<AEIncludeRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<AEIncludeRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }
    }
}
