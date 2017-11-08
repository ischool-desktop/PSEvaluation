using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HsinChu.ClassExamScoreAvgComparison.Model
{
    // 統計班級科目成績使用
    class ClassCourseAvg
    {
        public string ClassID { get; set; }
        public string ClassName { get; set; }

        // 科目人數
        private Dictionary<string, int?> _SubjectSudCount;

        // 科目成績加總
        private Dictionary<string, decimal?> _SubjectStudScoreSum;

        /// <summary>
        /// 取得科目成績平均
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, decimal?> GetSubjectStudScoreAvg()
        {
            Dictionary<string, decimal?> retValue = new Dictionary<string, decimal?>();

            foreach (KeyValuePair<string, decimal?> val in _SubjectStudScoreSum)
            {
                if (_SubjectSudCount.ContainsKey(val.Key))
                {
                    if (_SubjectSudCount[val.Key] > 0)
                    {
                        retValue.Add(val.Key, (val.Value / _SubjectSudCount[val.Key]));
                    }
                }
            }
            return retValue;
        }

        public ClassCourseAvg()
        {
            _SubjectSudCount = new Dictionary<string, int?>();
            _SubjectStudScoreSum = new Dictionary<string, decimal?>();
        }

        /// <summary>
        /// 加入科目成績
        /// </summary>
        /// <param name="CourseID"></param>
        /// <param name="Score"></param>
        public void AddSubjectScore(string subject, decimal Score)
        {
            if (_SubjectSudCount.ContainsKey(subject))
            {
                _SubjectSudCount[subject]++;
                _SubjectStudScoreSum[subject] += Score;
            }
            else
            {
                _SubjectSudCount.Add(subject, 1);
                _SubjectStudScoreSum.Add(subject, Score);
            }

        }

    }
}
