using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FISCA.Data;
using JHSchool.Data;
using K12.Data;

namespace JHEvaluation.AssignmentExamManager
{
    public class AssignmentExamCourse : JHSchool.Data.JHCourse
    {
        /// <summary>
        /// 取得課程所有領域列表，並去除重覆的
        /// </summary>
        /// <returns></returns>
        public static List<string> SelectDomains()
        {
            List<string> Domains = new List<string>();

            QueryHelper Helper = new QueryHelper();

            DataTable Table = Helper.Select("select distinct domain from course order by domain desc");

            foreach (DataRow Row in Table.Rows)
            {
                string Domain = Row.Field<string>("domain");
                Domains.Add(Domain);
            }

            return Domains;
        }

        /// <summary>
        /// 根據多筆課程系統編號取得課程
        /// </summary>
        /// <param name="CourseIDs"></param>
        /// <returns></returns>
        public static List<AssignmentExamCourseRecord> SelectByIDs(IEnumerable<string> CourseIDs)
        {
            JHCourse.RemoveByIDs(CourseIDs);

            return SelectByIDs<AssignmentExamCourseRecord>(CourseIDs);
        }

        /// <summary>
        /// 根據單筆課程系統編號取得課程
        /// </summary>
        /// <param name="CourseID"></param>
        /// <returns></returns>
        public static AssignmentExamCourseRecord SelectByID(string CourseID)
        {
            JHCourse.RemoveByIDs(new List<string>() { CourseID });

            return SelectByID<AssignmentExamCourseRecord>(CourseID);
        }

        /// <summary>
        /// 根據條件取得課程
        /// </summary>
        /// <param name="SchoolYear">學年度</param>
        /// <param name="Semester">學期</param>
        /// <param name="Domain">領域</param>
        /// <param name="CompleteInputCount">完整小考輸入次數</param>
        /// <param name="ProgressChange">進度函式</param>
        /// <returns></returns>
        public static List<AssignmentExamCourseRecord> Select(int? SchoolYear, int? Semester, string Domain, int? CompleteInputCount,Action<int,object> ProgressChange)
        {
            ProgressChange.Invoke(0, "取得課程");

            #region Step1:根據學年度學期取得課程
            //1.根據學年度學期取得課程

            List<string> CourseIDs = new List<string>();

            QueryHelper Helper = new QueryHelper();

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine("select id from course where subject!='' ");

            if (SchoolYear.HasValue)
                strBuilder.Append(" and school_year=" + SchoolYear.Value);
            if (Semester.HasValue)
                strBuilder.Append(" and semester=" + Semester.Value);

            if (!Domain.Equals("顯示全部領域"))
            {
                if(Domain.Equals(""))
                    strBuilder.Append(" and domain is null");
                else
                    strBuilder.Append(" and domain='" + Domain + "'");
            }
            DataTable Table = Helper.Select(strBuilder.ToString());

            foreach (DataRow Row in Table.Rows)
            {
                string CourseID = Row.Field<string>("id");
                CourseIDs.Add(CourseID);
            }

            FunctionSpliter<string, AssignmentExamCourseRecord> CourseSpliter = new FunctionSpliter<string, AssignmentExamCourseRecord>(1000, 1);

            CourseSpliter.ProgressChange = (x) => ProgressChange(x, "分批取得課程");
            CourseSpliter.Function = x => AssignmentExamCourse.SelectByIDs(x);

            List<AssignmentExamCourseRecord> records = CourseSpliter
                .Execute(CourseIDs);
            #endregion

            #region 已使用Native Query來篩選出科目不為空白的課程
            //1.1 取得課程標籤為社團或聯課活動的課程編號
            //List<string> AssociationCourseIDs = CourseTag
            //    .SelectByCourseIDs(records.Select(x => x.ID))
            //    .Where(x => x.FullName.Contains("聯課活動") || x.FullName.Contains("社團"))
            //    .Select(x => x.RefEntityID).ToList();

            //records = records.Where(x => !AssociationCourseIDs.Contains(x.ID)).ToList();
            #endregion

            #region 已使用Native Query來篩選領域
            //2.根據領域篩選出課程
            //if (!Domain.Equals("顯示全部領域"))
            //    records = records.Where(x=>x.Domain.Equals(Domain)).ToList();
            #endregion

            //3.根據課程編號篩選出學生修課

            ProgressChange(100, "已取得課程");

            FunctionSpliter<string, AssignmentExamSCAttendRecord> SCAttendSpliter = new FunctionSpliter<string, AssignmentExamSCAttendRecord>(100,3 );

            SCAttendSpliter.ProgressChange = (x) => ProgressChange(x,"分批取得學生修課");
            SCAttendSpliter.Function = x => AssignmentExamSCAttend.SelectByCourseIDs(x);

            List<AssignmentExamSCAttendRecord> screcords = SCAttendSpliter
                .Execute(CourseIDs);

            ProgressChange.Invoke(100, "已取得學生修課");

            //4.計算每個課程的小考完整輸入次數
            ProgressChange.Invoke(0, "計算小考輸入次數");

            double Progress = 0;

            foreach(AssignmentExamCourseRecord courserecord in records)
            {
                Progress++;
                int Percentage = (int)((Progress / records.Count) * 100);

                ProgressChange.Invoke(Percentage, "計算小考輸入次數");

                //4.1 選出課程的學生修課紀錄
                List<AssignmentExamSCAttendRecord> curscrecords = screcords.Where(x => x.RefCourseID == courserecord.ID && (x.Student.Status == K12.Data.StudentRecord.StudentStatus.一般)).ToList();

                //4.2 紀錄每個小考已輸入小考成績的結構
                Dictionary<string, int> InputCount = new Dictionary<string, int>();

                //4.3 循訪課程學生修課
                foreach (AssignmentExamSCAttendRecord screcord in curscrecords)
                {
                    // 非一般生不列入
                    if (screcord.Student.Status != StudentRecord.StudentStatus.一般)
                        continue;

                    //4.4 循訪每個學生修課的小考紀錄
                    foreach (AssignmentExamRecord aerecord in screcord.AssignmentExams)
                    {
                        //4.5 假設小考編號不存在列表中
                        if (!InputCount.ContainsKey(aerecord.SubExamID))
                            InputCount.Add(aerecord.SubExamID, 0);


                        //4.6 假設有輸入小考成績則加入統計中
                        if (aerecord.Score.HasValue)
                            InputCount[aerecord.SubExamID]++;
                    }
                }

                //5 判斷小考統計是否等於修課人數，是的話將完整輸入次數加1
                foreach (string Key in InputCount.Keys)
                    if (InputCount[Key] >= curscrecords.Count)
                        courserecord.FinishedCount++;
            }

            ProgressChange.Invoke(100, "計算小考輸入次數");

            return CompleteInputCount!=null?records.Where(x=>x.FinishedCount<CompleteInputCount).ToList():records;
        }
    }
}