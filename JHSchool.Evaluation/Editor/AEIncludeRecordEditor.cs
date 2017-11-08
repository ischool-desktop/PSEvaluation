using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Editor;
using JHSchool.Evaluation.Feature;

namespace JHSchool.Evaluation.Editor
{
    public class AEIncludeRecordEditor
    {
        public bool Remove { get; set; }
        public string RefAssessmentSetupID { get; set; }
        public string RefExamID { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

        public bool UseText { get; set; }
        public bool UseScore { get; set; }
        public bool UseEffort { get; set; }

        public int Weight { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        internal AEIncludeRecord AEInclude { get; set; }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (AEInclude == null)
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
                    if (AEInclude.RefAssessmentSetupID != this.RefAssessmentSetupID ||
                        AEInclude.RefExamID != this.RefExamID ||
                        AEInclude.Name != this.Name ||
                        AEInclude.UseText != this.UseText ||
                        AEInclude.UseScore != this.UseScore ||
                        AEInclude.UseEffort != this.UseEffort ||
                        AEInclude.Weight != this.Weight ||
                        AEInclude.StartTime != this.StartTime ||
                        AEInclude.EndTime != this.EndTime
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
                EditAEInclude.SaveAEIncludeRecordEditor(new AEIncludeRecordEditor[] { this });
        }

        internal AEIncludeRecordEditor(AEIncludeRecord record)
        {
            AEInclude = record;

            ID = record.ID;
            RefAssessmentSetupID = record.RefAssessmentSetupID;
            RefExamID = record.RefExamID;
            Name = record.Name;
            UseText = record.UseText;
            UseScore = record.UseScore;
            UseEffort = record.UseEffort;
            Weight = record.Weight;
            StartTime = record.StartTime;
            EndTime = record.EndTime;
        }

        internal AEIncludeRecordEditor(AssessmentSetupRecord assessmentSetup, ExamRecord exam)
        {
            RefAssessmentSetupID = assessmentSetup.ID;
            RefExamID = exam.ID;
        }

        internal AEIncludeRecordEditor()
        {
        }
    }

    public static class AEIncludeRecordEditor_ExtendMethods
    {
        public static AEIncludeRecordEditor GetEditor(this AEIncludeRecord record)
        {
            return new AEIncludeRecordEditor(record);
        }

        public static void SaveAllEditors(this IEnumerable<AEIncludeRecordEditor> editors)
        {
            EditAEInclude.SaveAEIncludeRecordEditor(editors);
        }

        public static AEIncludeRecordEditor AddAEInclude(this AEInclude dataManager)
        {
            return new AEIncludeRecordEditor();
        }
    }
}
