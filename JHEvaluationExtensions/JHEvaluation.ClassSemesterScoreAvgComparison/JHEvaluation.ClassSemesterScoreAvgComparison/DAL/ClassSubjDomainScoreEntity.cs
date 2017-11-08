using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHEvaluation.ClassSemesterScoreAvgComparison.DAL
{
    // 計算科目或領域班級平均
    class ClassSubjDomainScoreEntity
    {
        /// <summary>
        /// 科目或領域名稱
        /// </summary>
        public string SubjName { get; set; }
        private decimal _SubjSumScore=0;
        private  int _SubjStudCount=0;

        public ClassSubjDomainScoreEntity()
        {

        }

        /// <summary>
        /// 加入科目或領域成績
        /// </summary>
        /// <param name="Score"></param>
        public void AddScore(decimal Score)
        {
            _SubjSumScore += Score;
            _SubjStudCount++;
        }

        /// <summary>
        /// 清空值
        /// </summary>
        public void Clear()
        {
            _SubjSumScore = 0;
            _SubjStudCount = 0;
        }


        /// <summary>
        /// 取得科目或領域平均
        /// </summary>
        /// <returns></returns>
        public decimal GetSubjDomainAvgScore()
        {
            if (_SubjStudCount == 0)
                return 0;
            else
                return Math.Round(_SubjSumScore / _SubjStudCount, 2, MidpointRounding.AwayFromZero);
        }
    }
}
