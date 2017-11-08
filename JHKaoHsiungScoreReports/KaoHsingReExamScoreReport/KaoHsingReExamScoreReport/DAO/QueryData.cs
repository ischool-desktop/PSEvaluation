using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using System.Data;

namespace KaoHsingReExamScoreReport.DAO
{
    public class QueryData
    {
        /// <summary>
        /// 透過班級ID 取得學生資料,依年級、班級、座號 排序
        /// </summary>
        /// <param name="ClassIDList"></param>
        /// <returns></returns>
        public static List<StudentData> GetStudentDataListByClassIDs(List<string> ClassIDList)
        {
            List<StudentData> retVal = new List<StudentData>();
            QueryHelper qh = new QueryHelper();
            string strSQL = "select student.id as sid,class.id as cid,student.name as sname,student_number,class.class_name,class.grade_year,student.seat_no from student inner join class on student.ref_class_id=class.id where student.status=1 and class.id in("+string.Join(",",ClassIDList.ToArray())+") order by grade_year,class_name,seat_no";
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                StudentData sd = new StudentData ();
                sd.StudentID= dr["sid"].ToString();
                sd.ClassID=dr["cid"].ToString();
                sd.Name=dr["sname"].ToString();
                sd.StudentNumber=dr["student_number"].ToString();
                sd.ClassName=dr["class_name"].ToString();
                sd.GradeYear=dr["grade_year"].ToString();
                sd.SeatNo=dr["seat_no"].ToString();
                retVal.Add(sd);

            }
            return retVal;
        }

        /// <summary>
        /// 透過班級ID取得班級資料
        /// </summary>
        /// <param name="ClassIDList"></param>
        /// <returns></returns>
        public static List<ClassData> GetClassDataByClassIDs(List<string> ClassIDList)
        {
            List<ClassData> retVal = new List<ClassData>();
            QueryHelper qh = new QueryHelper();
            string strSQL = "select class.id as cid,class_name ,teacher_name,grade_year from class inner join teacher on class.ref_teacher_id=teacher.id where class.id in("+string.Join(",",ClassIDList.ToArray())+") order by class.grade_year,class_name;";
            DataTable dt = qh.Select(strSQL);

            foreach (DataRow dr in dt.Rows)
            {
                ClassData cd = new ClassData();
                cd.ClassID = dr["cid"].ToString();
                cd.ClassName = dr["class_name"].ToString();
                cd.TeacherName = dr["teacher_name"].ToString();
                cd.GradeYear = dr["grade_year"].ToString();
                retVal.Add(cd);
            }
            return retVal;
        }

        /// <summary>
        /// 透過學生ID 取得學生資料,依年級、班級、座號 排序
        /// </summary>
        /// <param name="StudentIDList"></param>
        /// <returns></returns>
        public static List<StudentData> GetStudentDataListByStudentIDs(List<string> StudentIDList)
        {
            List<StudentData> retVal = new List<StudentData>();
            QueryHelper qh = new QueryHelper();
            string strSQL = "select student.id as sid,class.id as cid,student.name as sname,student_number,class.class_name,class.grade_year,student.seat_no from student inner join class on student.ref_class_id=class.id where student.status=1 and student.id in(" + string.Join(",", StudentIDList.ToArray()) + ") order by grade_year,class_name,seat_no";
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                StudentData sd = new StudentData();
                sd.StudentID = dr["sid"].ToString();
                sd.ClassID = dr["cid"].ToString();
                sd.Name = dr["sname"].ToString();
                sd.StudentNumber = dr["student_number"].ToString();
                sd.ClassName = dr["class_name"].ToString();
                sd.GradeYear = dr["grade_year"].ToString();
                sd.SeatNo = dr["seat_no"].ToString();
                retVal.Add(sd);

            }
            return retVal;
        }

    }
}
