using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Semester
{
    /// <summary>
    /// 代表一群學生的所有學期成績集合。
    /// </summary>
    internal class RSemesterScoreCollection
    {
        /// <summary>
        /// 載入成績。
        /// </summary>
        public static RSemesterScoreCollection LoadScores(IEnumerable<string> studIds)
        {
            List<JHSchool.Data.JHSemesterScoreRecord> semsscores = JHSchool.Data.JHSemesterScore.SelectByStudentIDs(studIds);

            List<RSemesterScore> rscores = new List<RSemesterScore>();

            foreach (JHSchool.Data.JHSemesterScoreRecord each in semsscores)
                rscores.Add(new RSemesterScore(each));

            return new RSemesterScoreCollection(rscores);
        }

        private List<RSemesterScore> Scores { get; set; }
        /// <summary>
        /// 學生編號->所有學期成績(不分學期)。
        /// </summary>
        private Dictionary<string, List<RSemesterScore>> DicScores { get; set; }

        private RSemesterScoreCollection(List<RSemesterScore> scores)
        {
            Scores = scores;
            DicScores = new Dictionary<string, List<RSemesterScore>>();

            foreach (RSemesterScore each in Scores)
            {
                if (!DicScores.ContainsKey(each.RefStudentID))
                    DicScores.Add(each.RefStudentID, new List<RSemesterScore>());

                DicScores[each.RefStudentID].Add(each);
            }
        }

        public IEnumerable<RSemesterScore> ForBy(int schoolYear, int semester)
        {
            List<RSemesterScore> scores = new List<RSemesterScore>();

            foreach (RSemesterScore each in Scores)
            {
                if (each.SchoolYear != schoolYear) continue;
                if (each.Semester != semester) continue;

                scores.Add(each);
            }

            return scores.AsReadOnly();
        }

        internal IEnumerable<RSemesterScore> ForBy(string studentId, int schoolYear, int semester)
        {
            List<RSemesterScore> scores = new List<RSemesterScore>();

            if (DicScores.ContainsKey(studentId))
            {
                foreach (RSemesterScore each in DicScores[studentId])
                {
                    if (each.SchoolYear != schoolYear) continue;
                    if (each.Semester != semester) continue;

                    scores.Add(each);
                }
            }

            return scores;
        }
    }
}
