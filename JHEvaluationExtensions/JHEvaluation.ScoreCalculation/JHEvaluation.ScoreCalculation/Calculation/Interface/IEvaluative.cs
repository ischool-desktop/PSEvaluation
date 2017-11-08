using System.Collections.Generic;

namespace JHSchool.Evaluation.Calculation
{
    /// <summary>
    /// 畢業判斷條件介面
    /// </summary>
    internal interface IEvaluative
    {
        /// <summary>
        /// 傳入學生列表，傳回每位學生通過或不通過
        /// </summary>
        /// <param name="list">學生記錄列表</param>
        /// <returns></returns>
        Dictionary<string, bool> Evaluate(IEnumerable<StudentRecord> list);

        /// <summary>
        /// 傳回不通過的原因
        /// </summary>
        EvaluationResult Result { get; }
    }
}