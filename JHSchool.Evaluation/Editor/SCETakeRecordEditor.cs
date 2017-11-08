using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Editor;
using JHSchool.Evaluation.Feature;
using Framework;

namespace JHSchool.Evaluation.Editor
{
    public class SCETakeRecordEditor
    {
        public bool Remove { get; set; }
        internal string ID { get; private set; }
        public decimal? Score { get; set; }
        public int? Effort { get; set; }
        public string Text { get; set; }
        internal string RefSCAttendID { get; private set; }
        internal string RefExamID { get; private set; }
        internal string RefStudentID { get; private set; }
        internal string RefCourseID { get; private set; }

        internal SCETakeRecord SCETake { get; set; }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (SCETake == null)
                {
                    if (!Remove)
                        return EditorStatus.Insert;
                    else
                        return EditorStatus.NoChanged;
                }
                else
                {
                    if (Remove)
                        return EditorStatus.Delete;
                    if (
                        SCETake.RefSCAttendID != RefSCAttendID ||
                        SCETake.RefExamID != RefExamID ||
                        SCETake.Score != Score ||
                        SCETake.Effort != Effort ||
                        SCETake.Text != Text
                        )
                    {
                        return EditorStatus.Update;
                    }
                }
                return EditorStatus.NoChanged;
            }
        }

        public void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
                EditSCETake.SaveSCETakeRecordEditor(new SCETakeRecordEditor[] { this });
        }

        public SCETakeRecordEditor(SCETakeRecord record)
        {
            ID = record.ID;
            SCETake = record;

            Score = record.Score;
            Effort = record.Effort;
            Text = record.Text;

            RefSCAttendID = record.RefSCAttendID;
            RefExamID = record.RefExamID;
            RefStudentID = record.RefStudentID;
            RefCourseID = record.RefCourseID;
        }

        public SCETakeRecordEditor(SCAttendRecord scattend, ExamRecord exam)
        {
            Score = null;
            Effort = null;
            Text = string.Empty;

            RefSCAttendID = scattend.ID;
            RefExamID = exam.ID;
            RefStudentID = scattend.RefStudentID;
            RefCourseID = scattend.RefCourseID;
        }
    }

    public static class SCETakeRecordEditor_ExtendMethods
    {
        public static SCETakeRecordEditor GetEditor(this SCETakeRecord record)
        {
            return new SCETakeRecordEditor(record);
        }

        public static void SaveAllEditors(this IEnumerable<SCETakeRecordEditor> editors)
        {
            EditSCETake.SaveSCETakeRecordEditor(editors);
        }
    }
}
