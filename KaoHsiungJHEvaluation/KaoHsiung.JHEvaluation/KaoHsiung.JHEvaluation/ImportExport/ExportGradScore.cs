using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.API.PlugIn;
using JHSchool.Data;
using JHSchool.Evaluation;

namespace KaoHsiung.JHEvaluation.ImportExport
{
    class ExportGradScore : SmartSchool.API.PlugIn.Export.Exporter
    {
        public ExportGradScore()
        {
            this.Image = null;
            this.Text = "匯出畢業成績";
        }
        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            //SmartSchool.API.PlugIn.VirtualCheckBox filterRepeat = new SmartSchool.API.PlugIn.VirtualCheckBox("自動略過重讀成績", true);
            //wizard.Options.Add(filterRepeat);
            wizard.ExportableFields.AddRange("領域", "分數評量");
            wizard.ExportPackage += delegate(object sender, SmartSchool.API.PlugIn.Export.ExportPackageEventArgs e)
            {
                #region ExportPackage
                List<JHStudentRecord> students = JHStudent.SelectByIDs(e.List);

                GradScore.Instance.SyncDataBackground(e.List);

                foreach (JHStudentRecord stu in students)
                {
                    GradScoreRecord record = GradScore.Instance.Items[stu.ID];
                    if (record == null) continue;

                    foreach (GradDomainScore domain in record.Domains.Values)
                    {
                        RowData row = new RowData();
                        row.ID = stu.ID;
                        foreach (string field in e.ExportFields)
                        {
                            if (wizard.ExportableFields.Contains(field))
                            {
                                switch (field)
                                {
                                    case "領域": row.Add(field, "" + domain.Domain); break;
                                    case "分數評量": row.Add(field, "" + domain.Score); break;
                                }
                            }
                        }
                        e.Items.Add(row);
                    }

                    foreach (string item in new string[] { "學習領域", "課程學習" })
                    {
                        RowData row = new RowData();
                        row.ID = stu.ID;
                        foreach (string field in e.ExportFields)
                        {
                            if (wizard.ExportableFields.Contains(field))
                            {
                                switch (field)
                                {
                                    case "領域": row.Add(field, item); break;
                                    case "分數評量":
                                        if (item == "學習領域")
                                            row.Add("分數評量", "" + record.LearnDomainScore);
                                        else if (item == "課程學習")
                                            row.Add("分數評量", "" + record.CourseLearnScore);
                                        break;
                                }
                            }
                        }
                        e.Items.Add(row);
                    }
                }
                #endregion

                FISCA.LogAgent.ApplicationLog.Log("成績系統.匯入匯出", "匯出畢業成績", "總共匯出" + e.Items.Count + "筆畢業成績。");
            };
        }
    }
}
