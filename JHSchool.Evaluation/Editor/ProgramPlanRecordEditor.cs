using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Editor;

namespace JHSchool.Evaluation.Editor
{
    public class ProgramPlanRecordEditor
    {
        public bool Remove { get; set; }
        public string ID { get; private set; }
        public string Name { get; set; }
        public List<ProgramSubject> Subjects { get; set; }

        internal ProgramPlanRecord ProgramPlan { get; private set; }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (ProgramPlan == null)
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

                    else if (ProgramPlan.Name != Name ||
                         ProgramPlan.Subjects != Subjects)
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
                Feature.EditProgramPlan.SaveProgramPlanRecordEditor(new ProgramPlanRecordEditor[] { this });
        }

        internal ProgramPlanRecordEditor()
        {
            ID = string.Empty;
            Name = string.Empty;
            Subjects = new List<ProgramSubject>();

            ProgramPlan = null;
        }

        internal ProgramPlanRecordEditor(ProgramPlanRecord record)
            : this()
        {
            ID = record.ID;
            Name = record.Name;
            List<ProgramSubject> list = new List<ProgramSubject>();
            foreach (var subject in record.Subjects)
                list.Add(subject.Clone() as ProgramSubject);
            Subjects = list;

            ProgramPlan = record;
        }
    }

    public static class ProgramPlanRecord_ExtendFunctions
    {
        public static ProgramPlanRecordEditor GetEditor(this ProgramPlanRecord record)
        {
            return new ProgramPlanRecordEditor(record);
        }
        public static void SaveAllEditors(this IEnumerable<ProgramPlanRecordEditor> editors)
        {
            Feature.EditProgramPlan.SaveProgramPlanRecordEditor(editors);
        }
        public static ProgramPlanRecordEditor AddProgramPlan(this ProgramPlan dataManager)
        {
            return new ProgramPlanRecordEditor();
        }
    }
}
