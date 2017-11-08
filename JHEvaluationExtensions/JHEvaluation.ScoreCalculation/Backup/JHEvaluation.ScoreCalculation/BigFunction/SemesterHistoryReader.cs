using System;
using System.Collections.Generic;
using System.Text;
using K12.Data;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    internal class SemesterHistoryReader
    {
        private IStatusReporter Reporter { get; set; }

        private Dictionary<string, StudentScore> Students { get; set; }

        public SemesterHistoryReader(List<StudentScore> students, IStatusReporter reporter)
        {
            Students = students.ToDictionary();
            Reporter = reporter;
        }

        public void Read()
        {
            List<SemesterHistoryRecord> histories = ReadSemesterHistory();

            foreach (SemesterHistoryRecord each in histories)
            {
                if (Students.ContainsKey(each.RefStudentID))
                {
                    StudentScore student = Students[each.RefStudentID];
                    foreach (SemesterHistoryItem item in each.SemesterHistoryItems)
                        student.SHistory.Add(new SemesterData(item.GradeYear, item.SchoolYear, item.Semester));
                }
            }
        }

        private List<SemesterHistoryRecord> ReadSemesterHistory()
        {
            FunctionSpliter<string, SemesterHistoryRecord> selectData = new FunctionSpliter<string, SemesterHistoryRecord>(500, Util.MaxThread);
            selectData.Function = delegate(List<string> p)
            {
                return K12.Data.SemesterHistory.SelectByStudentIDs(p);
            };
            selectData.ProgressChange = delegate(int progress)
            {
                Reporter.Feedback("讀取學期歷程...", Util.CalculatePercentage(Students.Count, progress));
            };
            return selectData.Execute(Students.Values.ToKeys());
        }
    }
}
