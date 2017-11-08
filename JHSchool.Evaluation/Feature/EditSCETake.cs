using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.Editor;
using Framework;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation.Feature
{
    internal static class EditSCETake
    {
        public static void SaveSCETakeRecordEditor(IEnumerable<SCETakeRecordEditor> editors)
        {
            MultiThreadWorker<SCETakeRecordEditor> worker = new MultiThreadWorker<SCETakeRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<SCETakeRecordEditor> e)
            {
                DSXmlHelper insertHelper = new DSXmlHelper("Request");
                DSXmlHelper updateHelper = new DSXmlHelper("Request");
                DSXmlHelper deleteHelper = new DSXmlHelper("Request");
                List<string> synclist = new List<string>();

                bool do_insert = false, do_update = false, do_delete = false;

                foreach (var editor in editors)
                {
                    if (editor.EditorStatus != JHSchool.Editor.EditorStatus.NoChanged)
                        if(!string.IsNullOrEmpty(editor.ID)) synclist.Add(editor.ID);

                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        insertHelper.AddElement("ScoreSheetList");
                        insertHelper.AddElement("ScoreSheetList", "ScoreSheet");
                        insertHelper.AddElement("ScoreSheetList/ScoreSheet", "Score", "" + editor.Score.Value);
                        insertHelper.AddElement("ScoreSheetList/ScoreSheet", "ExamID", editor.RefExamID);
                        insertHelper.AddElement("ScoreSheetList/ScoreSheet", "AttendID", editor.RefSCAttendID);

                        insertHelper.AddElement("ScoreSheetList/ScoreSheet", "Extension");
                        insertHelper.AddElement("ScoreSheetList/ScoreSheet/Extension", "Extension");
                        insertHelper.AddElement("ScoreSheetList/ScoreSheet/Extension/Extension", "Effort", "" + editor.Effort);
                        insertHelper.AddElement("ScoreSheetList/ScoreSheet/Extension/Extension", "Text", editor.Text);

                        do_insert = true;
                    }
                    else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                    {
                        updateHelper.AddElement("ScoreSheetList");
                        updateHelper.AddElement("ScoreSheetList", "ScoreSheet");
                        updateHelper.AddElement("ScoreSheetList/ScoreSheet", "Score", "" + editor.Score.Value);
                        updateHelper.AddElement("ScoreSheetList/ScoreSheet", "ExamID", editor.RefExamID);
                        updateHelper.AddElement("ScoreSheetList/ScoreSheet", "AttendID", editor.RefSCAttendID);

                        updateHelper.AddElement("ScoreSheetList/ScoreSheet", "Extension");
                        updateHelper.AddElement("ScoreSheetList/ScoreSheet/Extension", "Extension");
                        updateHelper.AddElement("ScoreSheetList/ScoreSheet/Extension/Extension", "Effort", "" + editor.Effort);
                        updateHelper.AddElement("ScoreSheetList/ScoreSheet/Extension/Extension", "Text", editor.Text);

                        //這個 Element 是 Condition。這支 Service 寫法比較怪一點。
                        updateHelper.AddElement("ScoreSheetList/ScoreSheet", "ID", editor.ID);

                        do_update = true;
                    }
                    else if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                    {
                        deleteHelper.AddElement("ScoreSheet");
                        deleteHelper.AddElement("ScoreSheet", "ID", editor.ID);

                        do_delete = true;
                    }
                }

                //新增、修改、刪除的三個 Services 名字不太一致
                //Insert : SmartSchool.Course.InsertSECScore
                //Update : SmartSchool.Course.UpdateSCEScore
                //Delete : SmartSchool.Course.DeleteSCEScore
                if (do_insert)
                {
                    DSResponse resp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.InsertSECScore", new DSRequest(insertHelper.BaseElement));
                    foreach (var item in resp.GetContent().GetElements("NewID"))
                    {
                        synclist.Add(item.InnerText);
                    }
                }
                if (do_update)
                    FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.UpdateSCEScore", new DSRequest(updateHelper.BaseElement));
                if (do_delete)
                    FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.DeleteSCEScore", new DSRequest(deleteHelper.BaseElement));

                //SCETake 不做 CacheManager 了
                //SCETake.Instance.SyncDataBackground(synclist);
            };
            List<PackageWorkEventArgs<SCETakeRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<SCETakeRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }
    }
}
