using System.Xml;
using System.Collections.Generic;

namespace JHEvaluation.AssignmentExamManager
{
    public class AssignmentExamCourseRecord : JHSchool.Data.JHCourseRecord
    {
        public List<AssignmentExamSetupRecord> ExamSetups
        {
            get
            {
                List<AssignmentExamSetupRecord> records = new List<AssignmentExamSetupRecord>();

                if (base.Extensions != null && !string.IsNullOrEmpty(base.Extensions.InnerXml))
                {
                    foreach (XmlNode Node in base.Extensions.SelectNodes("Extension[@Name='GradeItem']/GradeItem/Item"))
                    {
                        AssignmentExamSetupRecord record = new AssignmentExamSetupRecord();

                        record.Load(Node as XmlElement);

                        records.Add(record);
                    }
                }
                
                return records;
            }
        }

        /// <summary>
        /// 小考次數
        /// </summary>
        public int ExamCount 
        {
            get
            {
                if (base.Extensions != null && !string.IsNullOrEmpty(base.Extensions.InnerXml))
                {
                    XmlNodeList Nodes = base.Extensions.SelectNodes("Extension[@Name='GradeItem']/GradeItem/Item");

                    return Nodes==null?0:Nodes.Count;
                }

                return 0;
            }
        }
        /// <summary>
        /// 完整輸入次數
        /// </summary>
        public int FinishedCount { get; set; }
    }
}