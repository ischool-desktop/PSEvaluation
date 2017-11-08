using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Editor;

namespace JHSchool.Evaluation.Editor
{
    public class ExamRecordEditor
    {
        public bool Remove { get; set; }
        public string ID { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }

        internal ExamRecord Exam { get; private set; }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (Exam == null)
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

                    else if (Exam.Name != Name ||
                        Exam.Description != Description ||
                        Exam.DisplayOrder != DisplayOrder)
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
                Feature.EditExam.SaveExamRecordEditor(new ExamRecordEditor[] { this });
        }

        internal ExamRecordEditor()
        {
            ID = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            DisplayOrder = 0;

            Exam = null;
        }

        internal ExamRecordEditor(ExamRecord record)
            : this()
        {
            ID = record.ID;
            Name = record.Name;
            Description = record.Description;
            DisplayOrder = record.DisplayOrder;

            Exam = record;
        }
    }
}
