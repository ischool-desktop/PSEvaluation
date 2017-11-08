using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHEvaluation.ClassSemesterScoreAvgComparison.DAL
{
    class ClassEntity
    {
        /// <summary>
        /// 班級編號
        /// </summary>
        public string ClassID { get; set; }
        /// <summary>
        /// 班級名稱
        /// </summary>
        public string ClassName { get; set; }

        // 班級科目或領域成績
        private Dictionary<string, ClassSubjDomainScoreEntity> _ClassSubjScoreEntityDic;

        /// <summary>
        /// 班級人數
        /// </summary>
        public int StudCount { get; set; }

        public ClassEntity()
        {
            _ClassSubjScoreEntityDic = new Dictionary<string, ClassSubjDomainScoreEntity>();
        }

        /// <summary>
        /// 加入科目或領域成績
        /// </summary>
        /// <param name="SubjName"></param>
        /// <param name="Score"></param>
        public void AddClassSubjDomainScore(string SubjName, decimal Score)
        {
            if (_ClassSubjScoreEntityDic.ContainsKey(SubjName))
            {
                _ClassSubjScoreEntityDic[SubjName].AddScore(Score);
            }
            else
            {
                ClassSubjDomainScoreEntity csse = new ClassSubjDomainScoreEntity();
                csse.SubjName = SubjName;
                csse.AddScore(Score);
                _ClassSubjScoreEntityDic.Add(SubjName, csse);            
            }
        }

        /// <summary>
        /// 取得班級科目或領域平均        
        /// </summary>
        /// <param name="SubjName"></param>
        /// <returns></returns>
        public decimal GetClassSubjAvgScore(string SubjName)
        {
            if (_ClassSubjScoreEntityDic.ContainsKey(SubjName))
                return _ClassSubjScoreEntityDic[SubjName].GetSubjDomainAvgScore();
            else
                return 0;
        }
    }
}
