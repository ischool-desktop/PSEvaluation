using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JHSchool.Editor;
using JHSchool.Evaluation.Feature;

namespace JHSchool.Evaluation.Editor
{
    public class ScoreCalcRuleRecordEditor
    {
        public bool Remove { get; set; }
        public string ID { get; private set; }
        public string Name { get; set; }
        public XmlElement Content { get; set; }

        internal ScoreCalcRuleRecord ScoreCalcRule { get; private set; }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (ScoreCalcRule == null)
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

                    else if (ScoreCalcRule.Name != Name ||
                         ScoreCalcRule.Content != Content)
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
                EditScoreCalcRule.SaveScoreCalcRuleRecordEditor(new ScoreCalcRuleRecordEditor[] { this });
        }

        internal ScoreCalcRuleRecordEditor()
        {
            ID = string.Empty;
            Name = string.Empty;
            Content = null;
            ScoreCalcRule = null;
        }

        internal ScoreCalcRuleRecordEditor(ScoreCalcRuleRecord record)
            : this()
        {
            ID = record.ID;
            Name = record.Name;
            Content = record.Content;
            ScoreCalcRule = record;
        }
    }

    public static class ScoreCalcRuleRecordEditor_ExtendMethods
    {
        public static ScoreCalcRuleRecordEditor GetEditor(this ScoreCalcRuleRecord record)
        {
            return new ScoreCalcRuleRecordEditor(record);
        }

        public static void SaveAllEditors(this IEnumerable<ScoreCalcRuleRecordEditor> editors)
        {
            EditScoreCalcRule.SaveScoreCalcRuleRecordEditor(editors);
        }
    }
}
