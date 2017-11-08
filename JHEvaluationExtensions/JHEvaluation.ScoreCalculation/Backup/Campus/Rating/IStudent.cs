using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Campus.Rating
{
    /// <summary>
    /// 代表要排名的學生資訊。
    /// </summary>
    public interface IStudent
    {
        /// <summary>
        /// 唯一識別編號。
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 取得排名資料集合。
        /// </summary>
        PlaceCollection Places { get; }
    }
}
