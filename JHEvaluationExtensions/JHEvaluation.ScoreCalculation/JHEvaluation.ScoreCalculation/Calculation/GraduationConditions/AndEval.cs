using System.Collections.Generic;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 條件為And
    /// </summary>
    internal class AndEval : IEvaluative
    {
        private List<IEvaluative> _evals;
        private EvaluationResult _result;

        /// <summary>
        /// 無參數建構式
        /// </summary>
        public AndEval()
        {
            _evals = new List<IEvaluative>();
            _result = new EvaluationResult();
        }

        public void Add(IEvaluative eval)
        {
            _evals.Add(eval);
        }

        #region IEvaluative 成員

        public Dictionary<string, bool> Evaluate(IEnumerable<StudentRecord> list)
        {
            _result.Clear();

            Dictionary<string, bool> passList = new Dictionary<string, bool>();
            List<EvaluationResult> resultList = new List<EvaluationResult>();

            foreach (StudentRecord student in list)
                passList.Add(student.ID, true);

            foreach (IEvaluative eval in _evals)
            {
                Dictionary<string, bool> evalPassList = eval.Evaluate(list);
                foreach (string id in evalPassList.Keys)
                    passList[id] &= evalPassList[id];
                resultList.Add(eval.Result);
            }

            if (resultList.Count > 0) MergeResults(passList, resultList);
            return passList;
        }

        private void MergeResults(Dictionary<string, bool> passList, IEnumerable<EvaluationResult> resultList)
        {
            EvaluationResult merged = new EvaluationResult();

            foreach (EvaluationResult result in resultList)
            {
                foreach (string student_id in result.Keys)
                {
                    if (!passList.ContainsKey(student_id)) continue;
                    if (passList[student_id]) continue;
                    merged.MergeResults(student_id, result[student_id]);
                }
            }

            _result = merged;
        }

        public EvaluationResult Result
        {
            get { return _result; }
        }

        #endregion
    }
}