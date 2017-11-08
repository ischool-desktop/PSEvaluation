using System;
using System.Collections.Generic;
using System.Text;

namespace Campus.Rating
{
    /// <summary>
    /// 提供同名次進階比較功能。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IScoresParser<T> : IScoreParser<T>
        where T : IStudent
    {
        /// <summary>
        /// 取得當成績相同時，額外的比較成績清單。
        /// </summary>
        /// <param name="student">原學生資料。</param>
        /// <returns></returns>
        List<decimal> GetSecondScores(T student);
    }
}
