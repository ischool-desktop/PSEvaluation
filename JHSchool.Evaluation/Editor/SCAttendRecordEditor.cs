using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Editor;
using JHSchool.Evaluation.Feature;

namespace JHSchool.Evaluation.Editor
{
    /// <summary>
    /// 編輯修課資料
    /// </summary>
    //public class SCAttendRecordEditor
    //{
    //    /// <summary>
    //    /// 取得或設定，指出是否從資料庫刪除這筆資料
    //    /// </summary>
    //    public bool Remove { get; set; }
    //    internal string RefStudentID { get; set; }
    //    internal string RefCourseID { get; set; }
    //    /// <summary>
    //    /// 取得或設定修課總成績
    //    /// </summary>
    //    public decimal? Score { get; set; }
    //    /// <summary>
    //    /// 取得或設定修課努力程度
    //    /// </summary>
    //    public int? Effort { get; set; }
    //    /// <summary>
    //    /// 取得或設定修課文字描述
    //    /// </summary>
    //    public string Text { get; set; }
        
    //    /// <summary>
    //    /// 取得修課學生
    //    /// </summary>
    //    public StudentRecord Student
    //    {
    //        get { return JHSchool.Student.Instance[RefStudentID]; }
    //    }
    //    /// <summary>
    //    /// 取得或設定修課課程
    //    /// </summary>
    //    public CourseRecord Course
    //    {
    //        get { return JHSchool.Course.Instance[RefCourseID]; }
    //        set { RefCourseID = value.ID; }
    //    }

    //    /// <summary>
    //    /// 取得修改狀態
    //    /// </summary>
    //    public EditorStatus EditorStatus
    //    {
    //        get
    //        {
    //            if (SCAttendRecord == null)
    //            {
    //                if (!Remove)
    //                    return EditorStatus.Insert;
    //                else
    //                    return EditorStatus.NoChanged;
    //            }
    //            else
    //            {
    //                if (Remove)
    //                    return EditorStatus.Delete;
    //                if (
    //                    SCAttendRecord.RefCourseID != this.RefCourseID ||
    //                    SCAttendRecord.RefStudentID != this.RefStudentID ||
    //                    SCAttendRecord.Score != this.Score ||
    //                    SCAttendRecord.Effort != this.Effort ||
    //                    SCAttendRecord.Text != this.Text
    //                    )
    //                {
    //                    return EditorStatus.Update;
    //                }
    //            }
    //            return EditorStatus.NoChanged;
    //        }
    //    }

    //    public void Save()
    //    {
    //        if (EditorStatus != EditorStatus.NoChanged)
    //            EditSCAttend.SaveStudentAttendCourseRecordEditor(new SCAttendRecordEditor[] { this });
    //    }

    //    internal SCAttendRecord SCAttendRecord { get; set; }

    //    public SCAttendRecordEditor(SCAttendRecord info)
    //    {
    //        SCAttendRecord = info;
    //        RefStudentID = info.RefStudentID;
    //        RefCourseID = info.RefCourseID;
    //        Score = info.Score;
    //        Effort = info.Effort;
    //        Text = info.Text;
    //    }

    //    public SCAttendRecordEditor(StudentRecord student, CourseRecord course)
    //    {
    //        RefStudentID = student.ID;
    //        RefCourseID = course.ID;
    //        Score = null;
    //        Effort = null;
    //        Text = string.Empty;
    //    }
    //}

    //public static class SCAttendRecordEditor_ExtendFunctions
    //{
    //    public static SCAttendRecordEditor GetEditor(this SCAttendRecord record)
    //    {
    //        return new SCAttendRecordEditor(record);
    //    }

    //    public static void SaveAllEditors(this IEnumerable<SCAttendRecordEditor> editors)
    //    {
    //        EditSCAttend.SaveStudentAttendCourseRecordEditor(editors);
    //    }

    //    /// <summary>
    //    /// 儲存全部資料，但不同步資料。
    //    /// </summary>
    //    internal static List<string> SaveAllNoSync(this IEnumerable<SCAttendRecordEditor> editors)
    //    {
    //        return EditSCAttend.SaveStudentAttendCourseRecordEditor(editors, false);
    //    }

    //    //public static SCAttendRecordEditor AddCourse(this SCAttend scattend)
    //    //{
    //    //    return new SCAttendRecordEditor();
    //    //}
    //}
}
