using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.AssignmentScore
{
    public class CourseScoreCountRecord
    {
        /// <summary>
        /// 課程編號
        /// </summary>
        public string CourseID { get;set;}
        /// <summary>
        /// 課程名稱
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        /// 授課教師
        /// </summary>
        public string TeacherName { get; set; }
        /// <summary>
        /// 有平時評量成績人數
        /// </summary>
        public int OraHasScoreCount { get; set; }

        /// <summary>
        /// 總共有平時評量人數
        /// </summary>
        public int OraTotalScoreCount { get; set; }

        /// <summary>
        /// 有努力程度成績人數
        /// </summary>
        public int OraHasEffortCount { get; set; }

        /// <summary>
        /// 總共有努力程度人數
        /// </summary>
        public int OraTotalEffortCount { get; set; }

        /// <summary>
        /// 有文字描述人數
        /// </summary>
        public int OraHasTextCount { get; set; }

        /// <summary>
        /// 總共有文字描述人數
        /// </summary>
        public int OraTotalTextCount { get; set; }

        public CourseScoreCountRecord()
        { 
            // 初始化
            OraHasTextCount =OraTotalTextCount= OraHasScoreCount = OraHasEffortCount = OraTotalEffortCount = OraTotalScoreCount  = 0;
        
        }

        /// <summary>
        /// 平時或努力程度或文字描述沒有輸入完整
        /// </summary>
        /// <returns></returns>
        public bool NonAllInputScore()
        {
            bool pass = false;

            if(OraTotalScoreCount!= OraHasScoreCount )
                pass =true ;

            if (OraTotalEffortCount != OraHasEffortCount)
                pass = true;

            if (OraHasTextCount != OraTotalTextCount)
                pass = true;

            return pass;
        }

        /// <summary>
        /// 平時或努力程度
        /// </summary>
        /// <returns></returns>
        public bool NonInputScore()
        {
            bool pass = false;
            if (OraTotalScoreCount != OraHasScoreCount)
                pass = true;

            if (OraTotalEffortCount != OraHasEffortCount)
                pass = true;

            return pass;
        }


    }
}
