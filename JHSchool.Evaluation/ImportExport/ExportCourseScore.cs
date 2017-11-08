using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.API.PlugIn;

namespace JHSchool.Evaluation.ImportExport
{
    class ExportCourseScore : SmartSchool.API.PlugIn.Export.Exporter
    {
        public ExportCourseScore()
        {
            this.Image = null;
            this.Text = "匯出課程成績";
        }

        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            wizard.ExportableFields.AddRange("學年度", "學期", "課程名稱", "分數評量", "努力程度", "文字描述");
            wizard.ExportPackage += delegate(object sender, SmartSchool.API.PlugIn.Export.ExportPackageEventArgs e)
            {
                //學生資訊
                List<Data.JHStudentRecord> students = Data.JHStudent.SelectByIDs(e.List);
                //學生修課資訊
                Dictionary<string, List<Data.JHSCAttendRecord>> scattends = new Dictionary<string, List<JHSchool.Data.JHSCAttendRecord>>();
                //學生修習的課程
                Dictionary<string, Data.JHCourseRecord> courses = new Dictionary<string, JHSchool.Data.JHCourseRecord>();

                #region 取得修課記錄
                foreach (Data.JHSCAttendRecord record in Data.JHSCAttend.SelectByStudentIDAndCourseID(e.List, new string[] { }))
                {
                    if (!scattends.ContainsKey(record.RefStudentID))
                        scattends.Add(record.RefStudentID, new List<JHSchool.Data.JHSCAttendRecord>());
                    scattends[record.RefStudentID].Add(record);

                    if (!courses.ContainsKey(record.RefCourseID))
                        courses.Add(record.RefCourseID, null);
                }
                #endregion

                #region 取得課程資訊
                foreach (Data.JHCourseRecord record in Data.JHCourse.SelectByIDs(new List<string>(courses.Keys)))
                {
                    if (courses.ContainsKey(record.ID))
                        courses[record.ID] = record;
                }
                #endregion

                #region 產生 Row Data
                foreach (Data.JHStudentRecord stu in students)
                {
                    foreach (Data.JHSCAttendRecord record in scattends[stu.ID])
                    {
                        RowData row = new RowData();
                        row.ID = stu.ID;
                        foreach (string field in e.ExportFields)
                        {
                            if (wizard.ExportableFields.Contains(field))
                            {
                                switch (field)
                                {
                                    case "學年度": row.Add(field, "" + courses[record.RefCourseID].SchoolYear); break;
                                    case "學期": row.Add(field, "" + courses[record.RefCourseID].Semester); break;
                                    case "課程名稱": row.Add(field, courses[record.RefCourseID].Name); break;
                                    case "分數評量": row.Add(field, "" + record.Score); break;
                                    case "努力程度": row.Add(field, "" + record.Effort); break;
                                    case "文字描述": row.Add(field, record.Text); break;
                                }
                            }
                        }
                        e.Items.Add(row);
                    }
                }
                #endregion

                FISCA.LogAgent.ApplicationLog.Log("成績系統.匯入匯出", "匯出課程成績", "總共匯出" + e.Items.Count + "筆課程成績。");
            };
        }
    }
}
