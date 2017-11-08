using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Semester
{
    class RSubjectScore
    {
        public RSubjectScore(K12.Data.SubjectScore score)
        {
            Name = score.Subject;
            Score = score.Score;
            //Period = score.Period;
        }

        /// <summary>
        /// 科目名稱。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 科目成績。
        /// </summary>
        public decimal? Score { get; private set; }

        public decimal GetScoreOrDefault(decimal defaultScore)
        {
            return Score.GetValueOrDefault(defaultScore);
        }

        //public decimal? Period { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
