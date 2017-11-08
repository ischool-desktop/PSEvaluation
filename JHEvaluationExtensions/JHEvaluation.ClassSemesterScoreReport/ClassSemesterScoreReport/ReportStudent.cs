using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using Campus.Rating;
using JHSchool.Evaluation.Calculation;

namespace JHEvaluation.ClassSemesterScoreReport
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

            string RuleID = "";
            // 先使用學生
            if (student.OverrideScoreCalcRuleID != null)
            {
                RuleID = student.OverrideScoreCalcRuleID;
            }
            else
            {
                if (Utility.tmpClassRuleIDDict.ContainsKey(RefClassID))
                    RuleID = Utility.tmpClassRuleIDDict[RefClassID];
            }

            // 成績計算規則進位方式
            if(RuleID!="")
            if (Utility.tmpScoreCalculatorDict.ContainsKey(RuleID))
                StudScoreCalculator = Utility.tmpScoreCalculatorDict[RuleID];
          
        }

        public ScoreCalculator StudScoreCalculator { get; private set; }

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

        private Dictionary<string, decimal> _ReExamScores = new Dictionary<string, decimal>();

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

        /// <summary>
        /// 加入科目補考成績
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="score"></param>
        public void AddReExam(string subject,decimal score)
        {

            // 2016/10/7  穎驊修正，避免Key 重覆的問題(當學校同時具有科目補考成績、領域補考成績，且領域、科目名稱相同EX: 數學，此狀況基本上為誤植)
            // 就會有此問題，雖然回去看前面的MainForm.cs 的Code 我認為有點奇怪，但恩正說 目前如果KEY 值重覆先行覆蓋即可
            if (!_ReExamScores.ContainsKey(subject.Trim()))
            {
                _ReExamScores.Add(subject.Trim(), score);
            }
            else 
            {
                _ReExamScores[subject.Trim()] = score;
            
            }
            
        }

        public bool Contains(string subject)
        {
            return _scores.ContainsKey(subject.Trim());
        }

        public void Clear()
        {
            _scores.Clear();
            _weight.Clear();
            _ReExamScores.Clear();
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

        /// <summary>
        /// 取得補考成績，沒有回傳-1
        /// </summary>
        /// <param name="subj"></param>
        /// <returns></returns>
        public decimal GetReExamScore(string subj)
        {
            string key = subj.Trim();
            decimal value = -1;
            if (_ReExamScores.ContainsKey(key))
                value = _ReExamScores[key];
            return value;
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
