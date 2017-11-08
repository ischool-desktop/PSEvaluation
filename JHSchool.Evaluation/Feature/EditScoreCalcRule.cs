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
    internal static class EditScoreCalcRule
    {
        internal static void SaveScoreCalcRuleRecordEditor(params ScoreCalcRuleRecordEditor[] editors)
        {
            SaveScoreCalcRuleRecordEditor((IEnumerable<ScoreCalcRuleRecordEditor>)editors);
        }

        internal static void SaveScoreCalcRuleRecordEditor(IEnumerable<ScoreCalcRuleRecordEditor> editors)
        {
            DSXmlHelper insertReq = new DSXmlHelper("Request");
            DSXmlHelper updateReq = new DSXmlHelper("Request");
            DSXmlHelper deleteReq = new DSXmlHelper("Request");

            bool do_insert = false, do_update = false, do_delete = false;

            List<string> synclist = new List<string>();

            foreach (ScoreCalcRuleRecordEditor editor in editors)
            {
                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                {
                    insertReq.AddElement("ScoreCalcRule");
                    insertReq.AddElement("ScoreCalcRule", "Name", editor.Name);
                    insertReq.AddElement("ScoreCalcRule", "Content");
                    if (editor.Content != null)
                        insertReq.AddElement("ScoreCalcRule/Content", editor.Content);

                    do_insert = true;
                }
                else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                {
                    updateReq.AddElement("ScoreCalcRule");
                    updateReq.AddElement("ScoreCalcRule", "Field");
                    updateReq.AddElement("ScoreCalcRule/Field", "Name", editor.Name);
                    updateReq.AddElement("ScoreCalcRule/Field", "Content");
                    if (editor.Content != null)
                        updateReq.AddElement("ScoreCalcRule/Field/Content", editor.Content);
                    updateReq.AddElement("ScoreCalcRule", "Condition");
                    updateReq.AddElement("ScoreCalcRule/Condition", "ID", editor.ID);

                    do_update = true;
                    synclist.Add(editor.ID);
                }
                else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                {
                    deleteReq.AddElement("ScoreCalcRule");
                    deleteReq.AddElement("ScoreCalcRule", "ID", editor.ID);

                    do_delete = true;
                    synclist.Add(editor.ID);
                }
            }

            if (do_insert)
            {
                DSXmlHelper response = FISCA.Authentication.DSAServices.CallService("SmartSchool.ScoreCalcRule.InsertScoreCalcRule", new DSRequest(insertReq)).GetContent();
                foreach (XmlElement each in response.GetElements("NewID"))
                    synclist.Add(each.InnerText);
            }

            if (do_update) FISCA.Authentication.DSAServices.CallService("SmartSchool.ScoreCalcRule.UpdateScoreCalcRule", new DSRequest(updateReq));
            if (do_delete) FISCA.Authentication.DSAServices.CallService("SmartSchool.ScoreCalcRule.DeleteScoreCalcRule", new DSRequest(deleteReq));

            if (synclist.Count > 0)
                ScoreCalcRule.Instance.SyncDataBackground(synclist);
        }
    }
}
