using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Framework;
using FISCA.DSAUtil;
using JHSchool.Editor;
using JHSchool.Evaluation.Editor;

namespace JHSchool.Evaluation.Feature
{
    /// <summary>
    /// 編輯日常生活表現成績
    /// </summary>
    internal static class EditMoralScore
    {
        private const string INSERT_MORAL_SCORE = "SmartSchool.Score.InsertSemesterMoralScore";
        private const string UPDATE_MORAL_SCORE = "SmartSchool.Score.UpdateSemesterMoralScore";
        private const string DELETE_MORAL_SCORE = "SmartSchool.Score.DeleteSemesterMoralScore";

        /// <summary>
        /// 批次儲存多個Editor
        /// </summary>
        /// <param name="editors"></param>
        internal static void SaveMoralScoreRecordEditor(IEnumerable<MoralScoreRecordEditor> editors)
        {
            MultiThreadWorker<MoralScoreRecordEditor> worker = new MultiThreadWorker<MoralScoreRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;

            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<MoralScoreRecordEditor> e)
            {
                List<string> primarykeys = new List<string>();

                DSXmlHelper insertHelper = new DSXmlHelper("Request");
                DSXmlHelper updateHelper = new DSXmlHelper("Request");
                DSXmlHelper deleteHelper = new DSXmlHelper("Request");

                bool hasInsert = false;
                bool hasUpdate = false;
                bool hasRemove = false;

                foreach (var editor in e.List)
                {
                     primarykeys.Add(editor.RefStudentID);

                    switch (editor.EditorStatus)
                    {
                        //<Request>
                        //   <SemesterMoralScore>
                        //      <RefStudentID>168708</RefStudentID>
                        //      <SchoolYear>97</SchoolYear>
                        //      <Semester>1</Semester>
                        //      <TextScore>....一大堆東西</TextScore>
                        //   </SemesterMoralScore>
                        //</Request>
                        #region 新增
                        case EditorStatus.Insert:
                            insertHelper.AddElement("SemesterMoralScore");
                            insertHelper.AddElement("SemesterMoralScore", "RefStudentID", editor.RefStudentID);
                            insertHelper.AddElement("SemesterMoralScore", "SchoolYear", editor.SchoolYear);
                            insertHelper.AddElement("SemesterMoralScore", "Semester", editor.Semester);
                            insertHelper.AddElement("SemesterMoralScore", "TextScore", "");
                            insertHelper.AddElement("SemesterMoralScore/TextScore", editor.TextScore);

                            hasInsert = true;
                            break;
                        #endregion

                        //<Request>
                        //   <SemesterMoralScore>
                        //     <TextScore>....一大堆東西</TextScore>
                        //      <Condition>
                        //         <ID>(德行學期成績編號)</ID>
                        //         <RefSchoolID/>
                        //         <Semester/>
                        //         <SchoolYear/>
                        //      </Condition>
                        //   </SemesterMoralScore>
                        //</Request>
                        #region 修改
                        case EditorStatus.Update:
                            updateHelper.AddElement("SemesterMoralScore");
                            updateHelper.AddElement("SemesterMoralScore", "TextScore", "");
                            updateHelper.AddElement("SemesterMoralScore/TextScore", editor.TextScore);

                            updateHelper.AddElement("SemesterMoralScore", "Condition");
                            updateHelper.AddElement("SemesterMoralScore/Condition", "ID", editor.ID);
                            updateHelper.AddElement("SemesterMoralScore/Condition", "RefStudentID", editor.RefStudentID);
                            updateHelper.AddElement("SemesterMoralScore/Condition", "SchoolYear", editor.SchoolYear);
                            updateHelper.AddElement("SemesterMoralScore/Condition", "Semester", editor.Semester);

                            hasUpdate = true;
                            break;
                        #endregion

                        //<Request>
                        //   <SemesterMoralScore>
                        //      <ID>(德行學期成績編號)</ID>
                        //   </SemesterMoralScore>
                        //</Request>
                        #region 刪除
                        case EditorStatus.Delete:
                            deleteHelper.AddElement("SemesterMoralScore");
                            deleteHelper.AddElement("SemesterMoralScore", "ID", editor.ID);

                            hasRemove = true;
                            break;
                        #endregion
                    }
                }

                if (hasInsert)
                    FISCA.Authentication.DSAServices.CallService(INSERT_MORAL_SCORE, new DSRequest(insertHelper.BaseElement));

                if (hasUpdate)
                    FISCA.Authentication.DSAServices.CallService(UPDATE_MORAL_SCORE, new DSRequest(updateHelper.BaseElement));

                if (hasRemove)
                    FISCA.Authentication.DSAServices.CallService(DELETE_MORAL_SCORE, new DSRequest(deleteHelper.BaseElement));

                if (primarykeys.Count > 0)
                   MoralScore.Instance.SyncDataBackground(primarykeys.ToArray());
            };

            List<PackageWorkEventArgs<MoralScoreRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<MoralScoreRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }
    }
}
