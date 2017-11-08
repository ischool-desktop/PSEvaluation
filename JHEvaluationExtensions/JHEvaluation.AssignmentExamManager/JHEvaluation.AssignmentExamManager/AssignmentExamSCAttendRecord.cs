using System.Collections.Generic;
using System.Xml;

namespace JHEvaluation.AssignmentExamManager
{
    public class AssignmentExamSCAttendRecord : JHSchool.Data.JHSCAttendRecord
    {
        public List<AssignmentExamRecord> AssignmentExams
        {
            get
            {
                List<AssignmentExamRecord> records = new List<AssignmentExamRecord>();

                if (base.Extensions != null)
                {
                    foreach (XmlNode Node in base.Extensions.SelectNodes("Extension[@Name='GradeBook']/Exam/Item"))
                    {
                        AssignmentExamRecord record = new AssignmentExamRecord();
                        record.SubExamID = (Node as XmlElement).GetAttribute("SubExamID");
                        record.Score = K12.Data.Decimal.ParseAllowNull((Node as XmlElement).GetAttribute("Score"));
                        records.Add(record);
                    }
                }
                return records;
            }   
        }
    }
}