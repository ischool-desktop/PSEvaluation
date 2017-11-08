using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.ScoreCalculation.ScoreStruct
{
    /// <summary>
    /// 代表數個學期的成績資料。
    /// </summary>
    public class SemesterScoreCollection : IEnumerable<SemesterScore>
    {
        private Dictionary<SemesterData, SemesterScore> SemsScore { get; set; }

        public SemesterScoreCollection()
        {
            SemsScore = new Dictionary<SemesterData, SemesterScore>();
        }

        public bool Contains(SemesterData semester)
        {
            return SemsScore.ContainsKey(semester);
        }

        public void Add(SemesterData semester, SemesterScore score)
        {
            SemsScore.Add(semester, score);
        }

        public SemesterScore this[SemesterData semester]
        {
            get { return SemsScore[semester]; }
            set { SemsScore[semester] = value; }
        }

        public void Remove(SemesterData semester)
        {
            SemsScore.Remove(semester);
        }

        public int Count { get { return SemsScore.Count; } }

        /// <summary>
        /// 計算指定科目的算術平均，不進位處理。
        /// </summary>
        /// <param name="semesters">要平均的學期。</param>
        /// <param name="subjName">科目名稱。</param>
        public decimal? AvgSubjectScore(IEnumerable<SemesterData> semesters, string subjName)
        {
            decimal sum = 0, count = 0;

            foreach (SemesterData sems in semesters)
            {
                if (Contains(sems)) //判斷是否包含此學期的成績。
                {
                    if (!this[sems].SubjectScoreExists(subjName)) continue;//判斷該學期是否有該科目。

                    sum += this[sems].GetSubjectScore(subjName);
                    count++;
                }
            }

            if (count <= 0) return null;

            return sum / count;
        }

        /// <summary>
        /// 計算領域算術平均。
        /// </summary>
        /// <param name="semesters">要平均的學期。</param>
        /// <param name="domainName">領域名稱。</param>
        public decimal? AvgDomainScore(IEnumerable<SemesterData> semesters, string domainName)
        {
            decimal sum = 0, count = 0;

            foreach (SemesterData sems in semesters)
            {
                if (Contains(sems)) //判斷是否包含此學期的成績。
                {
                    if (!this[sems].DomainScoreExists(domainName)) continue;//判斷該學期是否有該科目。

                    sum += this[sems].GetDomainScore(domainName);
                    count++;
                }
            }

            if (count <= 0) return null;

            return sum / count;
        }

        /// <summary>
        /// 計算學習領域算術平均。
        /// </summary>
        /// <param name="semesters">要平均的學期。</param>
        /// <returns></returns>
        public decimal? AvgLearningDomainScore(IEnumerable<SemesterData> semesters)
        {
            decimal sum = 0, count = 0;

            foreach (SemesterData sems in semesters)
            {
                if (Contains(sems)) //判斷是否包含此學期的成績。
                {
                    if (!this[sems].LearnDomainScore.HasValue) continue;//判斷該學期是否有該科目。

                    sum += this[sems].LearnDomainScore.Value;
                    count++;
                }
            }

            if (count <= 0) return null;

            return sum / count;
        }

        /// <summary>
        /// 計算學習領域算術平均,當沒有成績當0分算。
        /// </summary>
        /// <param name="semesters">要平均的學期1。</param>
        /// <returns></returns>
        public decimal? AvgLearningDomainScore1(IEnumerable<SemesterData> semesters)
        {
            decimal sum = 0, count = 0;

            foreach (SemesterData sems in semesters)
            {
                if (Contains(sems)) //判斷是否包含此學期的成績。
                {
                    //if (!this[sems].LearnDomainScore.HasValue) continue;//判斷該學期是否有該科目。

                    if(this[sems].LearnDomainScore.HasValue)
                        sum += this[sems].LearnDomainScore.Value;
                    
                    count++;
                }
            }

            if (count <= 0) return null;

            return sum / count;
        }


        public void Clear()
        {
            SemsScore.Clear();
        }

        #region IEnumerable<SemesterScore> 成員

        public IEnumerator<SemesterScore> GetEnumerator()
        {
            return SemsScore.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return SemsScore.Values.GetEnumerator();
        }

        #endregion
    }
}
