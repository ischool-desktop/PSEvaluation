using System.Collections.Generic;

namespace JHSchool.Evaluation.Calculation.GraduationConditions
{
    /// <summary>
    /// 用來產生畢業判斷條件介面的介面
    /// </summary>
    internal interface IEvaluateFactory
    {
        /// <summary>
        /// 建立畢業判斷條件介面
        /// </summary>
        /// <returns></returns>
        Dictionary<string, IEvaluative> CreateEvaluativeEntities();
    }
}