using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.API.PlugIn;

namespace JHSchool.Evaluation.ImportExport.Course
{
    public class ExportCourseStudents : SmartSchool.API.PlugIn.Export.Exporter
    {
        private string Item { get; set; }
        public ExportCourseStudents(string item)
        {
            this.Image = null;
            Item = item;
            if (Item == "社團")
                this.Text = "匯出社團參與學生";
            else
                this.Text = "匯出課程修課學生";
        }

        private List<string> InternalExportableFields = new List<string>();

        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            wizard.ExportableFields.AddRange("姓名", "學號", "班級", "座號");
            wizard.ExportPackage += delegate(object sender, SmartSchool.API.PlugIn.Export.ExportPackageEventArgs e)
            {
                //課程資訊
                List<Data.JHCourseRecord> courses = Data.JHCourse.SelectByIDs(e.List);
                //學生修課資訊
                Dictionary<string, List<Data.JHSCAttendRecord>> scattends = new Dictionary<string, List<JHSchool.Data.JHSCAttendRecord>>();
                //課程修課學生
                Dictionary<string, Data.JHStudentRecord> students = new Dictionary<string, JHSchool.Data.JHStudentRecord>();
                
                #region 取得修課記錄
                foreach (Data.JHSCAttendRecord record in Data.JHSCAttend.SelectByStudentIDAndCourseID(new string[] { }, e.List))
                {
                    if (!scattends.ContainsKey(record.RefCourseID))
                        scattends.Add(record.RefCourseID, new List<JHSchool.Data.JHSCAttendRecord>());
                    scattends[record.RefCourseID].Add(record);

                    if (!students.ContainsKey(record.RefStudentID))
                        students.Add(record.RefStudentID, null);
                }
                #endregion

                #region 取得學生資訊
                JHSchool.Data.JHStudent.RemoveAll();
                foreach (Data.JHStudentRecord record in Data.JHStudent.SelectByIDs(new List<string>(students.Keys)))
                {
                    if (students.ContainsKey(record.ID))
                        students[record.ID] = record;
                }
                #endregion

                #region 產生 Row Data
                foreach (Data.JHCourseRecord course in courses)
                {
                    //Debug
                    if (!scattends.ContainsKey(course.ID)) continue;

                    foreach (Data.JHSCAttendRecord record in scattends[course.ID])
                    {   //2017.09.29 羿均修改，僅匯出在校生 。
                        if (record.Student.Status.ToString() == "一般")
                        {
                            RowData row = new RowData();
                            row.ID = course.ID;
                            foreach (string field in e.ExportFields)
                            {
                                if (wizard.ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "姓名": row.Add(field, students[record.RefStudentID].Name); break;
                                        case "學號": row.Add(field, students[record.RefStudentID].StudentNumber); break;
                                        case "班級": row.Add(field, (students[record.RefStudentID].Class != null ? students[record.RefStudentID].Class.Name : "")); break;
                                        case "座號": row.Add(field, "" + students[record.RefStudentID].SeatNo); break;
                                    }
                                }
                            }
                            e.Items.Add(row);
                        }
                    }
                }
                #endregion

                if (Item != "社團")
                    FISCA.LogAgent.ApplicationLog.Log("成績系統.匯入匯出", "匯出課程修課學生", "總共匯出" + e.Items.Count + "筆課程修課學生。");
            };
        }
    }
}
