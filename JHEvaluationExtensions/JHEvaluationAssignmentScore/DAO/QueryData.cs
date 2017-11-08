using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FISCA.Data;

namespace JHEvaluation.AssignmentScore.DAO
{
    public class QueryData
    {
        /// <summary>
        /// 透過學年度學期處理課程修課學生
        /// </summary>
        /// <param name="SchoolYear"></param>
        /// <param name="Semester"></param>
        /// <returns></returns>
        public static void ProcesstCourseNameStudentBySchoolYearSemester(int SchoolYear, int Semester)
        {            
            QueryHelper qh = new QueryHelper();
            string strQuery = "select course.id as courseid,course_name,sc_attend.id as scattendid from sc_attend inner join  student on sc_attend.ref_student_id=student.id inner join course on sc_attend.ref_course_id=course.id where course.subject <>'' and student.status=1 and course.school_year="+SchoolYear+" and course.semester="+Semester+" order by course_name;";
            DataTable dt = qh.Select(strQuery);
            
            _CourseIDNameDict.Clear();
            _SCAttenndIDList.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                string id = dr[0].ToString();

                if (!_CourseIDNameDict.ContainsKey(id))
                    _CourseIDNameDict.Add(id, dr[1].ToString());

                _SCAttenndIDList.Add(dr[2].ToString());         
            }
         
        }

        /// <summary>
        /// 課程ID,Name
        /// </summary>
        public static Dictionary<string, string> _CourseIDNameDict = new Dictionary<string, string>();

        /// <summary>
        ///  學生修課編號
        /// </summary>
        public static List<string> _SCAttenndIDList = new List<string>();

        /// <summary>
        /// 透過學年度學期去得授課教師1
        /// </summary>
        /// <param name="SchoolYear"></param>
        /// <param name="Semester"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetCourseTeacher(int SchoolYear,int Semester)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            // 課程授課教師資料
            QueryHelper qh2 = new QueryHelper();
            string query2 = "select ref_course_id,teacher.teacher_name from  tc_instruct inner join teacher on tc_instruct.ref_teacher_id=teacher.id inner join course on tc_instruct.ref_course_id=course.id where tc_instruct.sequence=1 and course.school_year=" + SchoolYear + " and course.semester=" + Semester;
            DataTable dt2 = qh2.Select(query2);
            
            foreach (DataRow dr in dt2.Rows)
            {
                string id = dr["ref_course_id"].ToString();
                if (!retVal.ContainsKey(id))
                    retVal.Add(id, dr["teacher_name"].ToString());
            }
            return retVal;        
        }

    }
}
