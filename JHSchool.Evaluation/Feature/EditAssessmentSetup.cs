using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.Editor;
using FISCA.DSAUtil;
using Framework;
using System.Xml;

namespace JHSchool.Evaluation.Feature
{
    internal static class EditAssessmentSetup
    {
        internal static void SaveAssessmentSetupRecordEditor(params AssessmentSetupRecordEditor[] editors)
        {
            SaveAssessmentSetupRecordEditor((IEnumerable<AssessmentSetupRecordEditor>)editors);
        }

        internal static void SaveAssessmentSetupRecordEditor(IEnumerable<AssessmentSetupRecordEditor> editors)
        {
            DSXmlHelper insertReq = new DSXmlHelper("Request");
            DSXmlHelper updateReq = new DSXmlHelper("Request");
            DSXmlHelper deleteReq = new DSXmlHelper("Request");

            bool do_insert = false, do_update = false, do_delete = false;

            //同步清單，裡面包含了 exam_template 的系統編號。
            List<string> synclist = new List<string>();

            foreach (AssessmentSetupRecordEditor editor in editors)
            {
                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                {
                    insertReq.AddElement("ExamTemplate");
                    insertReq.AddElement("ExamTemplate", "TemplateName", editor.Name);
                    insertReq.AddElement("ExamTemplate", "Description", editor.Description);
                    insertReq.AddElement("ExamTemplate", "AllowUpload", "否"); //國中預設為不可以由老師輸入課程成績。

                    do_insert = true;
                }
                else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                {
                    updateReq.AddElement("ExamTemplate");
                    updateReq.AddElement("ExamTemplate", "TemplateName", editor.Name);
                    updateReq.AddElement("ExamTemplate", "Description", editor.Description);
                    updateReq.AddElement("ExamTemplate", "Condition");
                    updateReq.AddElement("ExamTemplate/Condition", "ID", editor.ID);

                    do_update = true;
                    synclist.Add(editor.ID);
                }
                else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                {
                    deleteReq.AddElement("ExamTemplate");
                    deleteReq.AddElement("ExamTemplate", "ID", editor.ID);

                    do_delete = true;
                    synclist.Add(editor.ID);
                }
            }

            if (do_insert)
            {
                DSXmlHelper response = FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.Insert", new DSRequest(insertReq)).GetContent();
                foreach (XmlElement each in response.GetElements("NewID"))
                    synclist.Add(each.InnerText);
            }

            if (do_update) FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.Update", new DSRequest(updateReq));
            if (do_delete) FISCA.Authentication.DSAServices.CallService("SmartSchool.ExamTemplate.Delete", new DSRequest(deleteReq));

            if (synclist.Count > 0)
                AssessmentSetup.Instance.SyncDataBackground(synclist);
        }
    }
}
