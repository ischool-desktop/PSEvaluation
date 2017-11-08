using System;
using System.Collections.Generic;
using System.Text;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    class AttendScoreCalculator
    {
        private IStatusReporter Reporter { get; set; }

        private List<StudentScore> Students { get; set; }

        public AttendScoreCalculator(List<StudentScore> students, IStatusReporter reporter)
        {
            Students = students;
            Reporter = reporter;
        }

        public List<StudentScore> Calculate()
        {
            List<StudentScore> noneCalcs = new List<StudentScore>();
            foreach (StudentScore student in Students)
            {
                if (student.AttendScore.Count <= 0)
                {
                    noneCalcs.Add(student);
                    continue;
                }

                foreach (string subject in student.AttendScore)
                {
                    if (string.IsNullOrEmpty(subject)) continue; //沒有科目名稱，不計算成績。

                    AttendScore attend = student.AttendScore[subject];

                    if (Program.Mode == ModuleMode.HsinChu) //新竹課程文字描述是從評量文字描述加總。
                    {
                        //沒有任何定考成績的話，就維持原來的修課成績。
                        decimal? v = attend.Subscores.GetWeightAverageScore();
                        attend.Value = v.HasValue ? v : null;

                        attend.Text = attend.Subscores.GetJoinText();
                    }

                    if (Program.Mode == ModuleMode.KaoHsiung) //高雄的課程平時評量計算。
                    {
                        /** 以下兩段程式在 2010/6/28 號討論後修改算法，與會人：蔡主任、呂韻如、黃耀明、張騉翔*/

                        //if (attend.OrdinarilyScore.HasValue && attend.Value.HasValue)
                        //attend.Value = (attend.Value + attend.OrdinarilyScore.Value) / 2m;
                        CalculateKHAttendScore(attend);

                        //if (attend.OrdinarilyEffort.HasValue && attend.Effort.HasValue)
                        //{
                        //    decimal result = Math.Ceiling((attend.Effort.Value + attend.OrdinarilyEffort.Value) / 2m);
                        //    attend.Effort = (int)((result >= 5) ? 5 : result);
                        //}
                        CalculateKHAttendEffort(attend);
                    }
                }
            }

            return noneCalcs;
        }

        private static void CalculateKHAttendEffort(AttendScore attend)
        {
            decimal sum = 0, weight = 0;

            if (attend.OrdinarilyEffort.HasValue)
            {
                sum += attend.OrdinarilyEffort.Value;
                weight++;
            }

            decimal? avgEffort = attend.Subscores.GetAverageEffort();
            if (avgEffort.HasValue)
            {
                sum += avgEffort.Value;
                weight++;
            }

            //如果權重小於零，代表兩個成績都沒有，就維持原來成績。
            if (weight != 0)
            {
                decimal result = Math.Ceiling(sum / weight);
                attend.Effort = (int)((result >= 5) ? 5 : result);
            }
            else
                attend.Effort = null;
        }

        private static void CalculateKHAttendScore(AttendScore attend)
        {
            decimal sum = 0, weight = 0;

            if (attend.OrdinarilyScore.HasValue)
            {
                sum += attend.OrdinarilyScore.Value;
                weight++;
            }

            decimal? avgScore = attend.Subscores.GetWeightAverageScore();
            if (avgScore.HasValue)
            {
                sum += avgScore.Value;
                weight++;
            }

            if (weight != 0)
                attend.Value = (sum / weight);
            else
                attend.Value = null;
        }
    }
}
