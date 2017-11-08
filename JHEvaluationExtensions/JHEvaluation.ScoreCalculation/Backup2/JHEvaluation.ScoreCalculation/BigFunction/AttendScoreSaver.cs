using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    internal class AttendScoreSaver
    {
        private IStatusReporter Reporter { get; set; }

        private List<StudentScore> Students { get; set; }

        private UniqueSet<string> FilterSubject { get; set; }

        public AttendScoreSaver(List<StudentScore> students,
            IEnumerable<string> filterSubject,
            IStatusReporter reporter)
        {
            Reporter = reporter;
            Students = students;
            FilterSubject = new UniqueSet<string>(filterSubject);
        }

        public void Save()
        {
            #region 儲存課程成績。
            List<JHSCAttendRecord> attends = new List<JHSCAttendRecord>();

            foreach (StudentScore student in Students)
            {
                foreach (string subject in student.AttendScore)
                {
                    if (FilterSubject.Contains(subject) || FilterSubject.Count <= 0)
                    {
                        AttendScore attend = student.AttendScore[subject];
                        JHSCAttendRecord DALAttend = attend.RawAttend;

                        DALAttend.Score = attend.Value;
                        DALAttend.Effort = attend.Effort;
                        DALAttend.Text = attend.Text;

                        attends.Add(DALAttend);
                    }
                }
            }

            if (attends.Count <= 0) return;

            FunctionSpliter<JHSCAttendRecord, JHSCAttendRecord> spliter = new FunctionSpliter<JHSCAttendRecord, JHSCAttendRecord>(300 * 10, Util.MaxThread);
            spliter.Function = delegate(List<JHSCAttendRecord> attendPart)
            {
                JHSCAttend.Update(attendPart);
                return new List<JHSCAttendRecord>();
            };
            spliter.ProgressChange = delegate(int progress)
            {
                Reporter.Feedback("儲存計算結果...", Util.CalculatePercentage(attends.Count, progress));
            };
            spliter.Execute(attends);
            #endregion

        }
    }
}
