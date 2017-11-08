using System;
using System.Collections.Generic;
using System.Text;
using K12.Data;

namespace JHEvaluation.ScoreCalculation.ScoreStruct
{
    /// <summary>
    /// 每一個評量的成績，以 ExamID 為 Key 的成績集合。
    /// </summary>
    public class TakeScoreCollection : ScoreCollection<TakeScore>
    {
        /// <summary>
        /// 取得加權平均成績。
        /// </summary>
        /// <returns></returns>
        public decimal? GetWeightAverageScore()
        {
            decimal total = 0, weight = 0;
            foreach (string examId in this)
            {
                TakeScore score = this[examId];

                if (score.Value.HasValue && score.Weight.HasValue)
                {
                    total += (score.Value.Value * score.Weight.Value);
                    weight += score.Weight.Value;
                }
            }

            if (weight <= 0) return null;

            return (total / weight);
        }

        /// <summary>
        /// 取得努力程度算術平均(無條件進位)。
        /// </summary>
        /// <returns></returns>
        public int? GetAverageEffort()
        {
            decimal total = 0, count = 0;
            foreach (string examId in this)
            {
                TakeScore score = this[examId];

                if (score.Effort.HasValue)
                {
                    total += score.Effort.Value;
                    count++;
                }
            }

            if (count <= 0) return null;
            decimal result = Math.Ceiling(total / count);

            return (int)((result >= 5) ? 5 : result);
        }

        /// <summary>
        /// 取得文字評量串接字串。
        /// </summary>
        /// <returns></returns>
        public string GetJoinText()
        {
            StringBuilder builder = new StringBuilder();

            foreach (string examId in this)
            {
                TakeScore score = this[examId];

                if (!string.IsNullOrEmpty(score.Text))
                    builder.Append(score.Text + ",");
            }

            string[] texts = builder.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(",", texts);
        }
    }

    /// <summary>
    /// 每一個修習科目的成績，以科目名稱為 Key。
    /// </summary>
    public class AttendScoreCollection : ScoreCollection<AttendScore>
    {
    }

    /// <summary>
    /// 學期科目成績。
    /// </summary>
    public class SemesterSubjectScoreCollection : ScoreCollection<SemesterSubjectScore>
    {
        public SemesterSubjectScoreCollection()
        {
            Log = new LogData(string.Empty);
        }

        public LogData Log { get; private set; }
    }

    /// <summary>
    /// 學期領域成績。
    /// </summary>
    public class SemesterDomainScoreCollection : ScoreCollection<SemesterDomainScore>
    {
        public SemesterDomainScoreCollection()
        {
            Log = new LogData(string.Empty);
        }

        public LogData Log { get; private set; }
    }

    /// <summary>
    /// 畢業成績。
    /// </summary>
    public class GraduateScoreCollection : ScoreCollection<GraduateScore>
    {
        public GraduateScoreCollection()
        {
            Log = new LogData(string.Empty);
            LearningLog = new LogData("學習領或");
            CourseLog = new LogData("課程學習");
        }

        public override void Clear()
        {
            base.Clear();
            RawScore = null;
        }

        /// <summary>
        /// 原始的畢業成績資料。
        /// </summary>
        public GradScoreRecord RawScore { get; set; }

        /// <summary>
        /// 學習領域成績。
        /// </summary>
        public decimal? LearnDomainScore { get; set; }

        /// <summary>
        /// 課程學期成績。 
        /// </summary>
        public decimal? CourseLearnScore { get; set; }

        public LogData Log { get; private set; }

        public LogData LearningLog { get; private set; }

        public LogData CourseLog { get; private set; }
    }

    #region ScoreCollection
    /// <summary>
    /// 代表一組成績。
    /// </summary>
    public class ScoreCollection<T> : IEnumerable<string> where T : IScore
    {
        private Dictionary<string, T> _scores = new Dictionary<string, T>();

        public ScoreCollection()
        {
        }

        /// <summary>
        /// 加入成績。
        /// </summary>
        public virtual void Add(string item, T score)
        {
            _scores.Add(item.Trim(), score);
        }

        /// <summary>
        /// 取得成績是否在集合中。
        /// </summary>
        public bool Contains(string item)
        {
            return _scores.ContainsKey(item.Trim());
        }

        public virtual void Clear()
        {
            _scores.Clear();
        }

        public int Count { get { return _scores.Count; } }

        public virtual void Remove(string item)
        {
            _scores.Remove(item);
        }

        /// <summary>
        /// 取得科目成績。
        /// </summary>
        public T this[string item]
        {
            get { return _scores[item.Trim()]; }
        }

        #region IEnumerable<string> 成員

        public IEnumerator<string> GetEnumerator()
        {
            return _scores.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _scores.Keys.GetEnumerator();
        }

        #endregion
    }
    #endregion
}
