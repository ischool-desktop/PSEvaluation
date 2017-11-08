using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using Campus.Rating;

namespace HsinChu.ClassExamScoreReportV21
{
    #region ReportStudent
    internal class ReportStudent : IStudent
    {
        /// <summary>
        /// 用於對照班級資訊。
        /// </summary>
        private static Dictionary<string, JHClassRecord> Classes { get; set; }

        public static void SetClassMapping(IEnumerable<JHClassRecord> classes)
        {
            Classes = new Dictionary<string, JHClassRecord>();
            foreach (JHClassRecord each in classes)
                Classes.Add(each.ID, each);
        }

        public ReportStudent(JHStudentRecord student)
        {
            Scores = new TokenScoreCollection();
            Places = new PlaceCollection();
            Id = student.ID;
            Name = student.Name;
            RefClassID = student.RefClassID;
            SeatNo = student.SeatNo + "";
            StudentNumber = student.StudentNumber;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public string RefClassID { get; private set; }

        public string ClassName
        {
            get
            {
                if (Classes.ContainsKey(RefClassID))
                    return Classes[RefClassID].Name;
                else
                    return string.Empty;
            }
        }

        public string ClassOrderString
        {
            get
            {
                if (Classes.ContainsKey(RefClassID))
                    return Classes[RefClassID].DisplayOrder.PadLeft(3, 'Z') + Classes[RefClassID].Name;
                else
                    return string.Empty;
            }
        }

        public string GradeYear
        {
            get
            {
                if (Classes.ContainsKey(RefClassID))
                    return Classes[RefClassID].GradeYear + "";
                else
                    return string.Empty;
            }
        }

        public string SeatNo { get; private set; }

        /// <summary>
        /// 用於排序的座號資料。
        /// </summary>
        public int OrderSeatNo
        {
            get
            {
                if (string.IsNullOrEmpty(SeatNo))
                    return int.MinValue;
                else
                    return int.Parse(SeatNo);
            }
        }

        public string StudentNumber { get; private set; }

        /// <summary>
        /// 清除排名相關資料，包含成績、排名結果。
        /// </summary>
        public void Clear()
        {
            Scores.Clear();
            Places.Clear();
        }

        public TokenScoreCollection Scores { get; private set; }

        #region IStudent 成員

        public PlaceCollection Places { get; private set; }

        #endregion
    }
    #endregion

    #region ScoreCollection
    /// <summary>
    /// 代表單一學期成績。
    /// </summary>
    internal class ScoreCollection
    {
        private Dictionary<string, decimal> _scores = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> _weight = new Dictionary<string, decimal>();

        public ScoreCollection()
        {
        }

        /// <summary>
        /// 加入科目成績。
        /// </summary>
        public void Add(string subject, decimal score, decimal weight)
        {
            _scores.Add(subject.Trim(), score);
            _weight.Add(subject, weight);
        }

        public bool Contains(string subject)
        {
            return _scores.ContainsKey(subject.Trim());
        }

        public void Clear()
        {
            _scores.Clear();
            _weight.Clear();
        }

        /// <summary>
        /// 取得科目成績。
        /// </summary>
        public decimal this[string subject]
        {
            get { return _scores[subject.Trim()]; }
        }

        /// <summary>
        /// 取得科目的權重。
        /// </summary>
        public Dictionary<string, decimal> Weights
        {
            get { return _weight; }
        }
    }
    #endregion

    #region TokenScoreCollection
    /// <summary>
    /// 代表多學期成績。
    /// </summary>
    internal class TokenScoreCollection
    {
        private Dictionary<string, ScoreCollection> _multi_scores = new Dictionary<string, ScoreCollection>();

        /// <summary>
        /// 加入指定的成績集合。
        /// </summary>
        /// <param name="token">可能類似學期的資訊，例：一上。</param>
        public void Add(string token, ScoreCollection scores)
        {
            _multi_scores.Add(token, scores);
        }

        public bool Contains(string token)
        {
            return _multi_scores.ContainsKey(token);
        }

        /// <summary>
        /// 取得指定 token 的成績集合。
        /// </summary>
        /// <param name="tokne">例：一上。</param>
        public ScoreCollection this[string tokne]
        {
            get { return _multi_scores[tokne]; }
        }

        public void Clear()
        {
            _multi_scores.Clear();
        }
    }
    #endregion
}
