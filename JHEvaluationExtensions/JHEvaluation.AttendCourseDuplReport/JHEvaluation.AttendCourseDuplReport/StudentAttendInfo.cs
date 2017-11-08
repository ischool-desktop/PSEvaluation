using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace JHEvaluation.AttendCourseDuplReport
{
    internal class StudentAttendInfo
    {
        private Dictionary<string, SubjectCoursesCollection> _attend;

        /// <summary>
        /// 取得所有重覆修課資訊
        /// </summary>
        public Dictionary<string, SubjectCoursesCollection> DuplicateAttendInfo
        {
            get { return _attend; }
        }

        public StudentAttendInfo()
        {
            _attend = new Dictionary<string, SubjectCoursesCollection>();
        }

        /// <summary>
        /// 加入課程
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="course"></param>
        public void Add(string studentID, JHCourseRecord course)
        {
            if (!_attend.ContainsKey(studentID))
                _attend.Add(studentID, new SubjectCoursesCollection());

            SubjectCoursesCollection collection = _attend[studentID];
            if (!collection.ContainsKey(course.Subject))
                collection.Add(course.Subject, new List<string>());

            collection[course.Subject].Add(course.ID);
        }

        public class SubjectCoursesCollection : Dictionary<string, List<string>>
        {
        }

        /// <summary>
        /// 移除正規的修課資訊，只留下重覆修課
        /// </summary>
        public void RemoveRegular()
        {
            #region 移除正常的修課
            foreach (SubjectCoursesCollection collection in _attend.Values)
            {
                List<string> deleteSubjects = new List<string>();

                foreach (string subject in collection.Keys)
                {
                    if (collection[subject].Count == 1) //如果只科目只對應到一門課程
                        deleteSubjects.Add(subject);
                }

                foreach (string subject in deleteSubjects)
                    collection.Remove(subject);
            }
            #endregion

            #region 移除正常的學生
            List<string> deleteStudentIDs = new List<string>();
            foreach (string id in _attend.Keys)
            {
                if (_attend[id].Count <= 0)
                    deleteStudentIDs.Add(id);
            }
            foreach (string id in deleteStudentIDs)
                _attend.Remove(id);
            #endregion
        }

        /// <summary>
        /// 取得重覆修課學生編號
        /// </summary>
        /// <returns></returns>
        public List<string> GetDuplicateStudents()
        {
            return new List<string>(_attend.Keys);
        }

        /// <summary>
        /// 取得重覆修課課程編號
        /// </summary>
        /// <returns></returns>
        public List<string> GetDuplicateCourses()
        {
            List<string> courseIDs = new List<string>();

            foreach (SubjectCoursesCollection collection in _attend.Values)
            {
                foreach (List<string> ids in collection.Values)
                    courseIDs.AddRange(ids);
            }

            List<string> UniqueCourseIDs = new List<string>();
            foreach (string id in courseIDs)
            {
                if (!UniqueCourseIDs.Contains(id))
                    UniqueCourseIDs.Add(id);
            }

            return UniqueCourseIDs;
        }
    }
}
