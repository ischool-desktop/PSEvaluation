using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Xml;
using JHSchool.Editor;

namespace JHSchool.Evaluation.Editor
{
    public class MoralScoreRecordEditor
    {
        /// <summary>
        /// Constructor，為修改模式使用
        /// </summary>
        /// <param name="demeritRecord"></param>
        public MoralScoreRecordEditor(MoralScoreRecord updateRecord)
        {
            UpdateRecord = updateRecord;
            
            Remove = false;
            RefStudentID = UpdateRecord.RefStudentID;
            ID = UpdateRecord.ID;

            SchoolYear = updateRecord.SchoolYear;
            Semester = updateRecord.Semester;
            TextScore = updateRecord.TextScore;
        }

        /// <summary>
        /// Constructor ，為新增模式使用。
        /// </summary>
        /// <param name="studentRecord"></param>
        public MoralScoreRecordEditor(string PrimaryKey)
        {
            Remove = false;
            RefStudentID = PrimaryKey;

            TextScore = XmlHelper.LoadXml("<TextScore/>");
        }

        /// <summary>
        /// 取得編輯狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (UpdateRecord == null)
                {
                    if (!Remove)
                        return EditorStatus.Insert;
                    else
                        return EditorStatus.NoChanged;
                }
                else
                {
                    if (Remove)
                        return  EditorStatus.Delete;
                    else //if (UpdateRecord.TextScore.InnerXml != TextScore.InnerXml)
                        return EditorStatus.Update;
                }
            }
        }

        public void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
                Feature.EditMoralScore.SaveMoralScoreRecordEditor(new MoralScoreRecordEditor[] { this });
        }

        #region Fields

        public bool Remove { get; set; }

        public string RefStudentID { get; private set; }
        public string ID { get; private set; }

        public string SchoolYear { get; set; }
        public string Semester { get; set; }
        public XmlElement TextScore { get; set; }

        #endregion

        internal MoralScoreRecord UpdateRecord { get; private set; }
    }

    public static class AttendanceRecordEditor_ExtendMethods
    {
        public static MoralScoreRecordEditor GetEditor(this MoralScoreRecord record)
        {
            return new MoralScoreRecordEditor(record);
        }
        public static void SaveAllEditors(this IEnumerable<MoralScoreRecordEditor> editors)
        {
            Feature.EditMoralScore.SaveMoralScoreRecordEditor(editors);
        }
    }
}
