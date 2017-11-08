using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Campus.Rating
{
    /// <summary>
    /// 代表成績資料的取得方法。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IScoreParser<T> where T : IStudent
    {
        /// <summary>
        /// 成績資料的名稱，例如：國文。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 取得成績資料，如果回傳 Null 代表該學生沒有該項成績。
        /// </summary>
        /// <param name="student">要取得成績資料的學生。</param>
        /// <returns></returns>
        decimal? GetScore(T student);
    }
}
