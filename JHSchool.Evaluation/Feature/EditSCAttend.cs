using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;
using JHSchool.Evaluation.Editor;

namespace JHSchool.Evaluation.Feature
{
    //internal static class EditSCAttend
    //{
    //    public static void SaveStudentAttendCourseRecordEditor(IEnumerable<SCAttendRecordEditor> editors)
    //    {
    //        SaveStudentAttendCourseRecordEditor(editors, true);
    //    }

    //    internal static List<string> SaveStudentAttendCourseRecordEditor(IEnumerable<SCAttendRecordEditor> editors, bool sync)
    //    {
    //        List<string> sync_list = new List<string>();
    //        MultiThreadWorker<SCAttendRecordEditor> worker = new MultiThreadWorker<SCAttendRecordEditor>();
    //        worker.MaxThreads = 3;
    //        worker.PackageSize = 100;
    //        worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<SCAttendRecordEditor> e)
    //        {
    //            DSXmlHelper updateHelper = new DSXmlHelper("UpdateSCAttend");
    //            DSXmlHelper insertHelper = new DSXmlHelper("InsertSCAttend");
    //            DSXmlHelper deleteHelper = new DSXmlHelper("DeleteSCAttendRequest");
    //            //List<LogInfo> logs = new List<LogInfo>();
    //            List<string> reflashList = new List<string>();
    //            bool hasUpdate = false;
    //            bool hasInsert = false;
    //            bool hasRemove = false;
    //            foreach (var editor in e.List)
    //            {
    //                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
    //                {
    //                    #region 新增修課
    //                    //LogInfo logForStudent = new LogInfo() { Action = "新增修課", Entity = "Student", EntityID = editor.RefStudentID };
    //                    //LogInfo logForCourse = new LogInfo() { Action = "新增修課學生", Entity = "Course", EntityID = editor.RefCourseID };
    //                    //string description = "";
    //                    insertHelper.AddElement("Attend");
    //                    insertHelper.AddElement("Attend", "RefStudentID", editor.RefStudentID);
    //                    //description += "學生:" + editor.Student.GetDescription() + "\n";
    //                    insertHelper.AddElement("Attend", "RefCourseID", editor.RefCourseID);
    //                    //description += "修課:" + editor.Course.GetDescription() + "\n";
    //                    //if ( editor.OverriddenRequired )
    //                    //{
    //                    //    insertHelper.AddElement("Attend", "IsRequired", editor.OverriddenRequired ? "必" : "選");
    //                    //    description += "必選修:" + ( editor.OverriddenRequired ? "必選" : "選修" ) + "\n";
    //                    //}
    //                    //if ( editor.OverriddenRequiredBy )
    //                    //{
    //                    //    insertHelper.AddElement("Attend", "RequiredBy", "" + editor.OverriddenRequiredBy);
    //                    //    description += "校部定:" + editor.OverriddenRequiredBy + "\n";
    //                    //}
    //                    if (editor.Score != null)
    //                    {
    //                        insertHelper.AddElement("Attend", "Score");
    //                        //description += "修課總成績:" + editor.Score + "\n";
    //                    }
    //                    //logForStudent.Description = description;
    //                    //logForCourse.Description = description;
    //                    //logs.Add(logForStudent);
    //                    //logs.Add(logForCourse);
    //                    hasInsert = true;
    //                    #endregion
    //                }
    //                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
    //                {
    //                    #region 修改
    //                    //LogInfo logForStudent = new LogInfo() { Action = "變更修課內容", Entity = "Student", EntityID = editor.RefStudentID };
    //                    //string description = "";
    //                    updateHelper.AddElement("Attend");
    //                    updateHelper.AddElement("Attend", "FieldList");
    //                    //if ( editor.OverrideRequired != editor.StudentAttendCourseRecord.OverrideRequired )
    //                    //{
    //                    //    updateHelper.AddElement("Attend/FieldList", "IsRequired", editor.OverriddenRequired ? "必" : "選");
    //                    //    description += "必選修由\"" + ( ( editor.StudentAttendCourseRecord.OverriddenRequired != null ? ( editor.StudentAttendCourseRecord.OverriddenRequired ? "必選" : "選修" ) : "依課程" ) ) + "\"變更為\"" + ( editor.OverriddenRequired != null ? ( editor.OverriddenRequired ? "必選" : "選修" ) : "依課程" ) + "\"\n";
    //                    //}
    //                    //if ( editor.OverrideRequiredBy != editor.StudentAttendCourseRecord.OverrideRequiredBy )
    //                    //{
    //                    //    updateHelper.AddElement("Attend/FieldList", "RequiredBy", editor.OverrideRequiredBy);
    //                    //    description += "校部定由\"" + ( editor.StudentAttendCourseRecord.OverrideRequiredBy != null ? editor.StudentAttendCourseRecord.OverrideRequiredBy : "依課程" ) + "\"變更為\"" + ( editor.OverrideRequiredBy != null ? editor.OverrideRequiredBy : "依課程" ) + "\"\n";
    //                    //}
    //                    if (editor.Score != editor.SCAttendRecord.Score)
    //                    {
    //                        updateHelper.AddElement("Attend/FieldList", "Score", "" + editor.Score);
    //                        //description += "修課總成績由\"" + editor.SCAttendRecord.Score + "\"變更為\"" + editor.Score + "\"\n";
    //                    }
    //                    if (editor.RefCourseID != editor.SCAttendRecord.RefCourseID)
    //                    {
    //                        updateHelper.AddElement("Attend/FieldList", "RefCourseID", "" + editor.RefCourseID);
    //                        //logs.Add(new LogInfo() { Action = "移除修課學生", Entity = "Course", EntityID = editor.SCAttendRecord.RefCourseID, Description = ("移除修課學生:\"" + editor.Student.GetDescription() + "\"\n") });
    //                        //logs.Add(new LogInfo() { Action = "新增修課學生", Entity = "Course", EntityID = editor.RefCourseID, Description = ("新增修課學生:\"" + editor.Student.GetDescription() + "\"\n" + description) });
    //                        //logForStudent.Description = "學生:\"" + editor.Student.GetDescription() + "\"\n課程由\"" + editor.SCAttendRecord.Course.GetDescription() + "\"變更為\"" + editor.Course.GetDescription() + "\"\n" + description;
    //                        //logs.Add(logForStudent);
    //                    }
    //                    else
    //                    {
    //                        //description = "學生:\"" + editor.Student.GetDescription() + "\n課程\"" + editor.Course.GetDescription() + "\"\n" + description;
    //                        //logForStudent.Description = description;
    //                        //logs.Add(new LogInfo() { Action = "更改學生修課資料", Entity = "Course", EntityID = editor.RefCourseID, Description = description });
    //                        //logs.Add(logForStudent);
    //                    }
    //                    updateHelper.AddElement("Attend/FieldList", "Extension");
    //                    updateHelper.AddElement("Attend/FieldList/Extension", "Extension");
    //                    updateHelper.AddElement("Attend/FieldList/Extension/Extension", "Effort", "" + editor.Effort);
    //                    updateHelper.AddElement("Attend/FieldList/Extension/Extension", "Text", editor.Text);
    //                    updateHelper.AddElement("Attend", "Condition");
    //                    updateHelper.AddElement("Attend/Condition", "ID", editor.SCAttendRecord.ID);
    //                    reflashList.Add(editor.SCAttendRecord.ID);
    //                    hasUpdate = true;
    //                    #endregion
    //                }
    //                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
    //                {
    //                    #region 刪除
    //                    deleteHelper.AddElement("Attend");
    //                    deleteHelper.AddElement("Attend", "ID", editor.SCAttendRecord.ID);
    //                    //logs.Add(new LogInfo() { Action = "刪除修課記錄", Entity = "Student", EntityID = editor.SCAttendRecord.RefStudentID, Description = ("移除修課:\"" + editor.SCAttendRecord.Course.GetDescription() + "\"\n") });
    //                    //logs.Add(new LogInfo() { Action = "移除修課學生", Entity = "Course", EntityID = editor.SCAttendRecord.RefCourseID, Description = ("移除修課學生:\"" + editor.SCAttendRecord.Student.GetDescription() + "\"\n") });
    //                    reflashList.Add(editor.SCAttendRecord.ID);
    //                    hasRemove = true;
    //                    #endregion
    //                }
    //            }
    //            if (hasInsert)
    //            {
    //                DSResponse resp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.InsertSCAttend", new DSRequest(insertHelper.BaseElement));
    //                foreach (var item in resp.GetContent().GetElements("NewID"))
    //                {
    //                    reflashList.Add(item.InnerText);
    //                }
    //            }
    //            if (hasUpdate)
    //            {
    //                FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.UpdateSCAttend", new DSRequest(updateHelper.BaseElement));
    //            }
    //            if (hasRemove)
    //            {
    //                FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.DeleteSCAttend", new DSRequest(deleteHelper.BaseElement));
    //            }
    //            //if (logs.Count > 0)
    //            //    logs.SaveAll();

    //            sync_list.AddRange(reflashList);
    //        };
    //        List<PackageWorkEventArgs<SCAttendRecordEditor>> packages = worker.Run(editors);
    //        foreach (PackageWorkEventArgs<SCAttendRecordEditor> each in packages)
    //        {
    //            if (each.HasException)
    //                throw each.Exception;
    //        }

    //        if (sync_list.Count > 0 && sync)
    //            SCAttend.Instance.SyncDataBackground(sync_list);

    //        return sync_list;
    //    }
    //}
}
