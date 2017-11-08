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
    internal static class EditExam
    {
        internal static void SaveExamRecordEditor(params ExamRecordEditor[] editors)
        {
            SaveExamRecordEditor((IEnumerable<ExamRecordEditor>)editors);
        }

        internal static void SaveExamRecordEditor(IEnumerable<ExamRecordEditor> editors)
        {
            DSXmlHelper insertReq = new DSXmlHelper("Request");
            DSXmlHelper updateReq = new DSXmlHelper("Request");
            DSXmlHelper deleteReq = new DSXmlHelper("Request");
            deleteReq.AddElement("Exam");

            //同步清單，裡面包含了 graduation plan 的系統編號。
            List<string> synclist = new List<string>();

            bool do_insert = false, do_update = false, do_delete = false;

            foreach (ExamRecordEditor editor in editors)
            {
                if (editor.EditorStatus != JHSchool.Editor.EditorStatus.NoChanged)
                    synclist.Add(editor.ID);

                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                {
                    insertReq.AddElement("Exam");
                    insertReq.AddElement("Exam", "ExamName", editor.Name);
                    insertReq.AddElement("Exam", "Description", editor.Description);
                    insertReq.AddElement("Exam", "DisplayOrder", "" + editor.DisplayOrder);

                    do_insert = true;
                }
                else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                {
                    updateReq.AddElement("Exam");
                    updateReq.AddElement("Exam", "ExamName", editor.Name);
                    updateReq.AddElement("Exam", "Description", editor.Description);
                    updateReq.AddElement("Exam", "DisplayOrder", "" + editor.DisplayOrder);

                    updateReq.AddElement("Exam", "Condition");
                    updateReq.AddElement("Exam/Condition", "ID", editor.ID);

                    do_update = true;
                }
                else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                {
                    deleteReq.AddElement("Exam", "ID", editor.ID);

                    do_delete = true;
                }
            }

            if (do_insert)
            {
                DSXmlHelper response = FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.Insert", new DSRequest(insertReq)).GetContent();
                foreach (XmlElement each in response.GetElements("NewID"))
                    synclist.Add(each.InnerText);
            }
            if (do_update) FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.Update", new DSRequest(updateReq));
            if (do_delete) FISCA.Authentication.DSAServices.CallService("SmartSchool.Exam.Delete", new DSRequest(deleteReq));

            if (synclist.Count > 0)
                Exam.Instance.SyncDataBackground(synclist);
        }
    }
}
