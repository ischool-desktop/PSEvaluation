using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartSchool.API.PlugIn;
using JHSchool.Data;
using KaoHsiung.JHEvaluation.Data;
using FISCA.Data;
using System.Data;
using System.Xml.Linq;

namespace KaoHsiung.JHEvaluation.ImportExport
{
    class ExportExamScore : SmartSchool.API.PlugIn.Export.Exporter
    {
        public ExportExamScore()
        {
            this.Image = null;
            this.Text = "匯出評量成績";
        }

        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            wizard.ExportableFields.AddRange("學年度", "學期", "課程名稱", "評量名稱", "分數評量", "努力程度", "文字描述");
            wizard.ExportPackage += delegate(object sender, SmartSchool.API.PlugIn.Export.ExportPackageEventArgs e)
            {
                //學生資訊
                List<JHStudentRecord> students = JHStudent.SelectByIDs(e.List);
                //學生修課資訊
                Dictionary<string, List<JHSCAttendRecord>> scattends = new Dictionary<string, List<JHSchool.Data.JHSCAttendRecord>>();
                //學生修習的課程
                Dictionary<string, JHCourseRecord> courses = new Dictionary<string, JHSchool.Data.JHCourseRecord>();
                //評量成績 key: SCAttendID
                Dictionary<string, List<KH.JHSCETakeRecord>> sces = new Dictionary<string, List<KH.JHSCETakeRecord>>();
                //試別資訊
                Dictionary<string, JHExamRecord> exams = new Dictionary<string, JHSchool.Data.JHExamRecord>();

                #region 取得修課記錄
                foreach (JHSCAttendRecord record in JHSCAttend.SelectByStudentIDAndCourseID(e.List, new string[] { }))
                {
                    if (!scattends.ContainsKey(record.RefStudentID))
                        scattends.Add(record.RefStudentID, new List<JHSchool.Data.JHSCAttendRecord>());
                    scattends[record.RefStudentID].Add(record);

                    if (!courses.ContainsKey(record.RefCourseID))
                        courses.Add(record.RefCourseID, null);
                }
                #endregion

                #region 取得課程資訊
                foreach (JHCourseRecord record in JHCourse.SelectByIDs(new List<string>(courses.Keys)))
                {
                    if (courses.ContainsKey(record.ID))
                        courses[record.ID] = record;
                }
                #endregion

                #region 取得試別資訊
                foreach (JHExamRecord exam in JHExam.SelectAll())
                {
                    if (!exams.ContainsKey(exam.ID))
                        exams.Add(exam.ID, exam);
                }
                #endregion

                #region 取得評量成績
                foreach (KH.JHSCETakeRecord record in JHSCETake.SelectByStudentAndCourse(new List<string>(scattends.Keys), new List<string>(courses.Keys)).AsKHJHSCETakeRecords())
                {
                    if (!sces.ContainsKey(record.RefSCAttendID))
                        sces.Add(record.RefSCAttendID, new List<KH.JHSCETakeRecord>());
                    sces[record.RefSCAttendID].Add(record);    
                }
                #endregion

               


                #region 產生 Row Data
                foreach (JHStudentRecord stu in students)
                {
                    if (!scattends.ContainsKey(stu.ID))
                    {
                        //學生沒有任何修課記錄。
                        continue;
                    }

                    foreach (JHSCAttendRecord record in scattends[stu.ID])
                    {
                        if (!sces.ContainsKey(record.ID)) continue;

                        sces[record.ID].Sort(delegate(KH.JHSCETakeRecord x, KH.JHSCETakeRecord y)
                        {
                            return x.RefExamID.CompareTo(y.RefExamID);
                        });

                        foreach (KH.JHSCETakeRecord sce in sces[record.ID])
                        {
                            string examName = sce.RefExamID;
                            if (exams.ContainsKey(sce.RefExamID))
                                examName = exams[sce.RefExamID].Name;

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
                                        case "評量名稱": row.Add(field, examName); break;
                                        case "分數評量": row.Add(field, "" + sce.Score); break;
                                        case "努力程度": row.Add(field, "" + sce.Effort); break;
                                        case "文字描述": row.Add(field, sce.Text); break;
                                    }
                                }
                            }
                            e.Items.Add(row);
                        }

                        //2016/7/26 穎驊新增，因應高雄國中希望能加入匯出匯入"平時成績"的功能，因此在原本的匯出評量成績報表Excel中增加平時成績的項目，
                        // 基本邏輯跟 SCEtake 的定期評量一樣，另外意外發現平時成績、努力程度、Text在 JHSCAttendRecord裏頭就有了，不必在另外下SQL 找UDT 在用XElement 去解析Xml 檔填成績
                            string examName2 = "平時評量";                           
                            RowData row2 = new RowData();
                            row2.ID = stu.ID;
                            foreach (string field in e.ExportFields)
                            {
                                if (wizard.ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "學年度": row2.Add(field, "" + courses[record.RefCourseID].SchoolYear); break;
                                        case "學期": row2.Add(field, "" + courses[record.RefCourseID].Semester); break;
                                        case "課程名稱": row2.Add(field, courses[record.RefCourseID].Name); break;
                                        case "評量名稱": row2.Add(field, examName2); break;
                                        case "分數評量": row2.Add(field, "" + record.OrdinarilyScore); break;
                                        case "努力程度": row2.Add(field, "" + record.OrdinarilyEffort); break;
                                        case "文字描述": row2.Add(field, record.Text); break;
                                    }
                                }
                            }
                            e.Items.Add(row2);                        
                    }
                }
                #endregion

                FISCA.LogAgent.ApplicationLog.Log("成績系統.匯入匯出", "匯出評量成績", "總共匯出" + e.Items.Count + "筆評量成績。");
            };
        }
    }
}
