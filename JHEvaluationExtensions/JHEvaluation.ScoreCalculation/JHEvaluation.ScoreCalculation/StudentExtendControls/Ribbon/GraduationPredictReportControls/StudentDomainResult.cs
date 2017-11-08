using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReportControls
{
    public static class StudentDomainResult
    {
        public static Dictionary<string, Dictionary<string, DomainScore>> _DomainResult = new Dictionary<string, Dictionary<string, DomainScore>>();
        public static List<string> _DomainNameList = new List<string>();

        /// <summary>
        /// 新增領域成績
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="domainName"></param>
        /// <param name="domainScore"></param>
        /// <param name="isPass"></param>
        public static void AddDomain(string studentId, string domainName, decimal domainScore, bool isPass)
        {
            DomainScore domain = new DomainScore();
            
            if (!_DomainResult.ContainsKey(studentId))
                _DomainResult.Add(studentId, new Dictionary<string,DomainScore>());
            if (!_DomainResult[studentId].ContainsKey(domainName))
                _DomainResult[studentId].Add(domainName, new DomainScore());

            domain = _DomainResult[studentId][domainName];
            domain.domainName = domainName;
            domain.domainScore = domainScore;
            domain.isPass = isPass;

            if (!_DomainNameList.Contains(domainName))
                _DomainNameList.Add(domainName);
        }

        /// <summary>
        /// 清除資料
        /// </summary>
        public static void Clear()
        {
            _DomainNameList.Clear();
            _DomainResult.Clear();
        }
    }

    /// <summary>
    /// 小郭, 2013/12/30
    /// </summary>
    public class DomainScore
    {
        public string domainName { get; set; }
        public decimal domainScore { get; set; }
        public bool isPass { get; set; }
    }

}
