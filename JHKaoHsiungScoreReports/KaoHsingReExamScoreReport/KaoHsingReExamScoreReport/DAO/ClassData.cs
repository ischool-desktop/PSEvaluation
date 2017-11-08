using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaoHsingReExamScoreReport.DAO
{
    public class ClassData
    {
        public string ClassID { get; set; }

        public string ClassName { get; set; }

        public string TeacherName { get; set; }

        public string GradeYear { get; set; }

        /// <summary>
        /// 領域補考人數統計
        /// </summary>
        public Dictionary<string, int> DomainReExamCount = new Dictionary<string, int>();
    }
}
