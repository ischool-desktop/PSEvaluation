using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Framework;
using JHSchool.Editor;

namespace JHSchool.Evaluation.Editor
{
    public class AssessmentSetupRecordEditor
    {
        public bool Remove { get; set; }
        public string ID { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public string StartTime { get; private set; }
        //public string EndTime { get; private set; }
        //public string AllowUpload { get; private set; }

        internal AssessmentSetupRecord AssessmentSetup { get; private set; }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (AssessmentSetup == null)
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

                    else if (AssessmentSetup.Name != Name ||
                         AssessmentSetup.Description != Description)
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
                Feature.EditAssessmentSetup.SaveAssessmentSetupRecordEditor(new AssessmentSetupRecordEditor[] { this });
        }

        internal AssessmentSetupRecordEditor()
        {
            ID = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            AssessmentSetup = null;
        }

        internal AssessmentSetupRecordEditor(AssessmentSetupRecord record)
            : this()
        {
            ID = record.ID;
            Name = record.Name;
            Description = record.Description;
            AssessmentSetup = record;
        }
    }

    public static class AssessmentSetupRecord_ExtendFunctions
    {
        /// <summary>
        /// 取得評分設定的 Editor。
        /// </summary>
        /// <returns></returns>
        public static AssessmentSetupRecordEditor GetEditor(this AssessmentSetupRecord record)
        {
            return new AssessmentSetupRecordEditor(record);
        }

        /// <summary>
        /// 儲存所有集合中的評分設定資料。
        /// </summary>
        /// <param name="editors"></param>
        public static void SaveAllEditors(this IEnumerable<AssessmentSetupRecordEditor> editors)
        {
            Feature.EditAssessmentSetup.SaveAssessmentSetupRecordEditor(editors);
        }

        /// <summary>
        /// 新增評分設定。
        /// </summary>
        /// <param name="dataManager"></param>
        /// <returns></returns>
        public static AssessmentSetupRecordEditor AddAssessmentSetup(this AssessmentSetup dataManager)
        {
            return new AssessmentSetupRecordEditor();
        }
    }
}
