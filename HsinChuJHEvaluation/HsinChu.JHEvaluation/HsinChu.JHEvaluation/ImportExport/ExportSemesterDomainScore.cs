using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.API.PlugIn;
using JHSchool.Data;

namespace HsinChu.JHEvaluation.ImportExport
{
    class ExportSemesterDomainScore : SmartSchool.API.PlugIn.Export.Exporter
    {
        public ExportSemesterDomainScore()
        {
            this.Image = null;
            this.Text = "匯出學期領域成績";
        }
        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            SmartSchool.API.PlugIn.VirtualCheckBox filterRepeat = new SmartSchool.API.PlugIn.VirtualCheckBox("自動略過重讀成績", true);
            wizard.Options.Add(filterRepeat);
            //wizard.ExportableFields.AddRange("領域", "學年度", "學期", "權數", "節數", "分數評量", "努力程度", "文字描述", "註記");
            wizard.ExportableFields.AddRange("領域", "學年度", "學期", "權數", "節數", "成績", "原始成績", "補考成績", "文字描述", "註記");
            wizard.ExportPackage += delegate(object sender, SmartSchool.API.PlugIn.Export.ExportPackageEventArgs e)
            {
                #region ExportPackage
                List<JHStudentRecord> students = JHStudent.SelectByIDs(e.List);

                Dictionary<string, List<JHSemesterScoreRecord>> semsDict = new Dictionary<string, List<JHSemesterScoreRecord>>();
                foreach (JHSemesterScoreRecord record in JHSemesterScore.SelectByStudentIDs(e.List))
                {
                    if (!semsDict.ContainsKey(record.RefStudentID))
                        semsDict.Add(record.RefStudentID, new List<JHSemesterScoreRecord>());
                    semsDict[record.RefStudentID].Add(record);
                }

                foreach (JHStudentRecord stu in students)
                {
                    if (!semsDict.ContainsKey(stu.ID)) continue;

                    foreach (JHSemesterScoreRecord record in semsDict[stu.ID])
                    {
                        foreach (K12.Data.DomainScore domain in record.Domains.Values)
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
                                        case "學年度": row.Add(field, "" + record.SchoolYear); break;
                                        case "學期": row.Add(field, "" + record.Semester); break;
                                        case "權數": row.Add(field, "" + domain.Credit); break;
                                        case "節數": row.Add(field, "" + domain.Period); break;
                                        case "成績": row.Add(field, "" + domain.Score); break;
                                        case "原始成績": row.Add(field, "" + domain.ScoreOrigin); break;
                                        case "補考成績": row.Add(field, "" + domain.ScoreMakeup); break;
                                        //case "努力程度": row.Add(field, "" + domain.Effort); break;
                                        case "文字描述": row.Add(field, domain.Text); break;
                                        case "註記": row.Add(field, domain.Comment); break;
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
                                        case "學年度": row.Add(field, "" + record.SchoolYear); break;
                                        case "學期": row.Add(field, "" + record.Semester); break;
                                        case "成績":
                                            if (item == "學習領域")
                                                row.Add("成績", "" + record.LearnDomainScore);
                                            else if (item == "課程學習")
                                                row.Add("成績", "" + record.CourseLearnScore);
                                            break;
                                        case "原始成績":
                                            if (item == "學習領域")
                                                row.Add("原始成績", "" + record.LearnDomainScoreOrigin);
                                            else if (item == "課程學習")
                                                row.Add("原始成績", "" + record.CourseLearnScoreOrigin);
                                            break;
                                    }
                                }
                            }
                            e.Items.Add(row);
                        }
                    }
                }
                #endregion

                FISCA.LogAgent.ApplicationLog.Log("成績系統.匯入匯出", "匯出學期領域成績", "總共匯出" + e.Items.Count + "筆學期領域成績。");
            };
        }
    }
}
