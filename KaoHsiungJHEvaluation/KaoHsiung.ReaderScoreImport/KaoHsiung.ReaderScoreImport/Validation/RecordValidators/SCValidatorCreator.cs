using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using KaoHsiung.ReaderScoreImport.Model;

namespace KaoHsiung.ReaderScoreImport.Validation.RecordValidators
{
    internal class SCValidatorCreator
    {

        private StudentNumberDictionary _studentDict;
        private Dictionary<string, JHCourseRecord> _courseDict;
        private StudentCourseInfo _studentCourseInfo;
        private List<string> _attendCourseIDs;

        public StudentCourseInfo StudentCourseInfo { get { return _studentCourseInfo; } }
        public StudentNumberDictionary StudentNumberDictionary { get { return _studentDict; } }

        public List<string> AttendCourseIDs { get { return _attendCourseIDs; } }

        public SCValidatorCreator(List<JHStudentRecord> studentList, List<JHCourseRecord> courseList, List<JHSCAttendRecord> scaList)
        {
            _studentDict = new StudentNumberDictionary();
            _courseDict = new Dictionary<string, JHCourseRecord>();
            _studentCourseInfo = new StudentCourseInfo();
            _attendCourseIDs = new List<string>();

            foreach (JHStudentRecord student in studentList)
            {
                string studentNumber = student.StudentNumber;

                studentNumber = GetStudentNumberFormat(studentNumber);

                if (!_studentDict.ContainsKey(studentNumber))
                    _studentDict.Add(studentNumber, student);

                _studentCourseInfo.Add(student);
            }

            foreach (JHCourseRecord course in courseList)
            {
                if (!_courseDict.ContainsKey(course.ID))
                    _courseDict.Add(course.ID, course);
            }

            //Linq
            var student_ids = from student in studentList select student.ID;

            //foreach (JHSCAttendRecord sc in JHSCAttend.Select(student_ids.ToList<string>(), null, null, "" + schoolYear, "" + semester))
            foreach (JHSCAttendRecord sc in scaList)
            {
                if (!_studentCourseInfo.ContainsID(sc.RefStudentID)) continue;
                if (!_courseDict.ContainsKey(sc.RefCourseID)) continue;

                if (!_attendCourseIDs.Contains(sc.RefCourseID))
                    _attendCourseIDs.Add(sc.RefCourseID);

                _studentCourseInfo.AddCourse(sc.RefStudentID, _courseDict[sc.RefCourseID]);
            }
        }

        internal static string GetStudentNumberFormat(string studentNumber)
        {
            #region 學號不足位，左邊補0
            int StudentNumberLength =Global.StudentNumberLenght;
            int s = StudentNumberLength - studentNumber.Length;
            if (s > 0)
                return studentNumber.PadLeft(StudentNumberLength, '0');
            else
                return studentNumber;
            #endregion
        }

        internal IRecordValidator<DataRecord> CreateStudentValidator()
        {
            StudentValidator validator = new StudentValidator(_studentDict);
            return validator;
        }

        internal IRecordValidator<DataRecord> CreateSCAttendValidator()
        {
            SCAttendValidator validator = new SCAttendValidator(_studentCourseInfo);
            return validator;
        }
    }

    internal class StudentCourseInfo
    {
        private Dictionary<string, string> studentNumberTable; //StudentNumber -> ID
        private Dictionary<string, JHStudentRecord> studentTable; //ID -> Record
        private Dictionary<string, List<JHCourseRecord>> courseTable; //ID -> List of CourseRecord

        public StudentCourseInfo()
        {
            studentNumberTable = new Dictionary<string, string>();
            studentTable = new Dictionary<string, JHStudentRecord>();
            courseTable = new Dictionary<string, List<JHCourseRecord>>();
        }

        internal void Add(JHStudentRecord student)
        {
            if (string.IsNullOrEmpty(student.StudentNumber)) return;

            string studentNumber = SCValidatorCreator.GetStudentNumberFormat(student.StudentNumber);

            if (!studentNumberTable.ContainsKey(studentNumber))
                studentNumberTable.Add(studentNumber, student.ID);
            if (!studentTable.ContainsKey(student.ID))
                studentTable.Add(student.ID, student);
            if (!courseTable.ContainsKey(student.ID))
                courseTable.Add(student.ID, new List<JHCourseRecord>());
        }

        internal bool ContainsID(string id)
        {
            return studentTable.ContainsKey(id);
        }

        internal void AddCourse(string student_id, JHCourseRecord course)
        {
            if (!courseTable.ContainsKey(student_id)) return;
            courseTable[student_id].Add(course);
        }

        internal IEnumerable<JHCourseRecord> GetCourses(string sn)
        {
            if (!studentNumberTable.ContainsKey(sn)) return new List<JHCourseRecord>();
            string id = studentNumberTable[sn];
            if (!studentTable.ContainsKey(id)) return new List<JHCourseRecord>();

            return courseTable[id];
        }

        internal bool ContainsStudentNumber(string sn)
        {
            if (!studentNumberTable.ContainsKey(sn)) return false;
            else return true;
        }

        internal string GetStudentName(string sn)
        {
            if (!studentNumberTable.ContainsKey(sn)) return "<查無姓名>";
            string id = studentNumberTable[sn];
            if (!studentTable.ContainsKey(id)) return "<查無姓名>";

            return studentTable[id].Name;
        }
    }

    internal class StudentNumberDictionary : Dictionary<string, JHStudentRecord> { }
}
