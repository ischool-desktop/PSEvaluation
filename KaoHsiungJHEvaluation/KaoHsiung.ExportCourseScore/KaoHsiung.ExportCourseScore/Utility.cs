using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using System.Data;
using System.Xml.Linq;

namespace KaoHsiung.ExportCourseScore
{
    public class Utility
    {
        /// <summary>
        /// 取得課程修課學生成績
        /// </summary>
        /// <param name="CousreIDList"></param>
        /// <returns></returns>
        public static List<CourseStudentsScore> GetCourseStudentList(List<string> CousreIDList)
        {
            List<CourseStudentsScore> retVal = new List<CourseStudentsScore>();
            Dictionary<string, List<ScoreItem>> scoreDict = new Dictionary<string, List<ScoreItem>>();
            if (CousreIDList.Count > 0)
            {
                scoreDict = GetScoreItems(CousreIDList);

                // 取得修課學
                QueryHelper qh = new QueryHelper();
                // 依課程名稱,班級名稱,座號 排序
                string strSQL = "select course.id as courseid,course.course_name,class.class_name,student.seat_no,student.student_number,student.name,student.id as studentid from course inner join sc_attend on course.id=sc_attend.ref_course_id inner join student on sc_attend.ref_student_id=student.id left join class on student.ref_class_id=class.id where course.id in(" + string.Join(",", CousreIDList.ToArray()) + ") order by course_name,class_name,seat_no;";
                DataTable dt = qh.Select(strSQL);
                int seatno;
                foreach (DataRow dr in dt.Rows)
                {
                    CourseStudentsScore css = new CourseStudentsScore();
                    css.CourseID = dr["courseid"].ToString();
                    css.CourseName = dr["course_name"].ToString();
                    css.ClassName = dr["class_name"].ToString();
                    if (int.TryParse(dr["seat_no"].ToString(), out seatno))
                        css.SeatNo = seatno;

                    css.StudentName = dr["name"].ToString();
                    css.StudentNumber = dr["student_number"].ToString();
                    css.StudentID = dr["studentid"].ToString();

                    // 加入成績
                    string key = css.CourseID + "_" + css.StudentID;
                    if (scoreDict.ContainsKey(key))
                    {
                        foreach (ScoreItem si in scoreDict[key])
                        {
                            if (!css.ScoreItems.ContainsKey(si.Name))
                                css.ScoreItems.Add(si.Name, si);
                        }
                    }
                    retVal.Add(css);
                }
            }
            return retVal;
        }


        /// <summary>
        /// 取得成績 ,key: courseid_student_id
        /// </summary>
        /// <param name="CourseIDList"></param>
        /// <returns></returns>
        public static Dictionary<string, List<ScoreItem>> GetScoreItems(List<string> CourseIDList)
        {
            Dictionary<string, List<ScoreItem>> retVal = new Dictionary<string, List<ScoreItem>>();

            // 定期評量
            QueryHelper qh1 = new QueryHelper();
            string strSQL1 = "select ref_course_id||'_'||ref_student_id as key,sce_take.extension,exam_name from sc_attend inner join sce_take on sc_attend.id=sce_take.ref_sc_attend_id inner join exam on sce_take.ref_exam_id=exam.id where ref_course_id in(" + string.Join(",", CourseIDList.ToArray()) + ")  order by ref_course_id,ref_student_id";
            DataTable dt1 = qh1.Select(strSQL1);
            foreach (DataRow dr in dt1.Rows)
            {
                string key = dr["key"].ToString();
                string strScore = dr["extension"].ToString();
                ScoreItem si = new ScoreItem();
                si.Name = dr["exam_name"].ToString();

                if (!string.IsNullOrWhiteSpace(strScore))
                {
                    XElement elm = XElement.Parse(strScore);
                    if (elm.Element("Score") != null)
                    {
                        decimal sc;
                        if (decimal.TryParse(elm.Element("Score").Value, out sc))
                            si.Score = sc;
                    }
                }

                if (!retVal.ContainsKey(key))
                {
                    List<ScoreItem> items = new List<ScoreItem>();
                    retVal.Add(key, items);
                }

                retVal[key].Add(si);

            }

            // 課程成績+小考成績
            QueryHelper qh2 = new QueryHelper();
            string strSQL2 = "select ref_course_id||'_'||ref_student_id as key,score,extension,extensions from sc_attend where ref_course_id in(" + string.Join(",", CourseIDList.ToArray()) + ") order by ref_course_id,ref_student_id";
            DataTable dt2 = qh2.Select(strSQL2);
            foreach (DataRow dr in dt2.Rows)
            {
                string key = dr["key"].ToString();
                if (!retVal.ContainsKey(key))
                {
                    List<ScoreItem> items = new List<ScoreItem>();
                    retVal.Add(key, items);
                }

                // 課程總成績
                ScoreItem si1 = new ScoreItem();
                si1.Name = Global.CousreScoreAvgKey;
                string strSc = dr["score"].ToString();
                decimal dc1;
                if (decimal.TryParse(strSc, out dc1))
                    si1.Score = dc1;
                retVal[key].Add(si1);


                //// 平時評量
                ScoreItem si2 = new ScoreItem();
                si2.Name = Global.CourseOrdinarilyScoreKey;
                string strXml1 = dr["extension"].ToString();
                if (!string.IsNullOrWhiteSpace(strXml1))
                {
                    XElement elm1 = XElement.Parse(strXml1);
                    if (elm1.Element("OrdinarilyScore") != null)
                    {
                        decimal d1;
                        if (decimal.TryParse(elm1.Element("OrdinarilyScore").Value, out d1))
                        {
                            si2.Score = d1;
                            retVal[key].Add(si2);
                        }
                    }
                }

                // 小考成績
                string strXml2 = dr["extensions"].ToString();
                if (!string.IsNullOrWhiteSpace(strXml2))
                {
                    XElement elm2 = XElement.Parse(strXml2);
                    if (elm2.Element("Extension") != null)
                    {
                        if (elm2.Element("Extension").Element("Exam") != null)
                        {
                            //// 平時評量
                            //if (elm2.Element("Extension").Element("Exam").Attribute("Score") != null)
                            //{
                            //    ScoreItem si2 = new ScoreItem();
                            //    si2.Name = Global.CourseOrdinarilyScoreKey;
                            //    decimal sid2;
                            //    if (decimal.TryParse(elm2.Element("Extension").Element("Exam").Attribute("Score").Value, out sid2))
                            //        si2.Score = sid2;

                            //    retVal[key].Add(si2);

                            //}

                            foreach (XElement eem in elm2.Element("Extension").Element("Exam").Elements("Item"))
                            {
                                ScoreItem sii = new ScoreItem();
                                sii.Name = eem.Attribute("SubExamID").Value;
                                decimal d2;
                                if (decimal.TryParse(eem.Attribute("Score").Value, out d2))
                                    sii.Score = d2;
                                retVal[key].Add(sii);
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// 取得課程所有定期考試名稱,courseID,ExamName,ExamID
        /// </summary>
        /// <param name="CourseIDList"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> GetCourseExamNameDict(List<string> CourseIDList)
        {
            Dictionary<string, Dictionary<string, string>> retVal = new Dictionary<string, Dictionary<string, string>>();
            if (CourseIDList.Count > 0)
            {
                QueryHelper qh = new QueryHelper();
                string strSQL = "select course.id as courseid,exam.exam_name,exam.id as examid from course inner join te_include on course.ref_exam_template_id=te_include.ref_exam_template_id inner join exam on te_include.ref_exam_id=exam.id where course.id in(" + string.Join(",", CourseIDList.ToArray()) + ") order by course.id";
                DataTable dt = qh.Select(strSQL);

                foreach (DataRow dr in dt.Rows)
                {
                    string key = dr["courseid"].ToString();

                    if (!retVal.ContainsKey(key))
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        retVal.Add(key, dict);
                    }

                    string key2 = dr["exam_name"].ToString();

                    if (!retVal[key].ContainsKey(key2))
                        retVal[key].Add(key2, dr["examid"].ToString());
                }
            }

            return retVal;
        }

        /// <summary>
        /// 取得所有課程小考名稱
        /// </summary>
        /// <param name="CourseIDList"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, Dictionary<string, decimal>>> GetCourseOrdinarilyNameDict(List<string> CourseIDList)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, decimal>>> retVal = new Dictionary<string, Dictionary<string, Dictionary<string, decimal>>>();
            if (CourseIDList.Count > 0)
            {
                QueryHelper qh = new QueryHelper();
                string strSQL = "select id,course.extensions from course where id in(" + string.Join(",", CourseIDList.ToArray()) + ") order by course.id";
                DataTable dt = qh.Select(strSQL);
                foreach (DataRow dr in dt.Rows)
                {
                    string key = dr["id"].ToString();

                    if (!retVal.ContainsKey(key))
                    {
                        Dictionary<string, Dictionary<string, decimal>> dict = new Dictionary<string, Dictionary<string, decimal>>();
                        retVal.Add(key, dict);
                    }
                    // 放入小考名稱
                    string strXML = dr["extensions"].ToString();
                    if (!string.IsNullOrWhiteSpace(strXML))
                    {
                        XElement elmRoot = XElement.Parse(strXML);
                        #region 找出小考名稱
                        if (elmRoot.Element("Extension") != null)
                        {
                            foreach (XElement extension in elmRoot.Elements("Extension"))   // 小郭, 2013/12/25
                            {
                                if (extension.Attribute("Name").Value != "GradeItem") continue; // <Extension Name="GradeItem">, 小郭, 2013/12/25
                                if (extension.Element("GradeItem") != null) // 小郭, 2013/12/25
                                {
                                    foreach (XElement elmName in extension.Element("GradeItem").Elements("Item"))   // 小郭, 2013/12/25
                                    {
                                        var eName = elmName.Attribute("Name").Value;
                                        var eID = elmName.Attribute("SubExamID").Value;
                                        decimal eWeight = 0m;
                                        decimal.TryParse(elmName.Attribute("Weight").Value, out eWeight);
                                        if (!retVal[key].ContainsKey(eName))
                                        {
                                            retVal[key].Add(eName, new Dictionary<string, decimal>());
                                        }
                                        if (!retVal[key][eName].ContainsKey(eID))
                                            retVal[key][eName].Add(eID, eWeight);
                                    }
                                }
                            }
                        }
                        #endregion 找出小考名稱
                    }
                }
            }

            return retVal;
        }


        /// <summary>
        /// 依照學年度,學期,課程名稱排序
        /// </summary>
        /// <param name="courseIDList"></param>
        /// <returns></returns>
        public static List<string> sortCourseIDList(List<string> courseIDList)
        {
            List<string> retVal = new List<string>();
            if (courseIDList.Count > 0)
            {
                QueryHelper qh = new QueryHelper();
                string strSQL = "select id from course where id in(" + string.Join(",", courseIDList.ToArray()) + ") order by school_year,semester,course_name";
                DataTable dt = qh.Select(strSQL);
                foreach (DataRow dr in dt.Rows)
                    retVal.Add(dr[0].ToString());
            }
            return retVal;
        }
    }
}
