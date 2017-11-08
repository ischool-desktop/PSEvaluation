using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaoHsiung.ReaderScoreImport.Model;
using JHSchool.Data;

namespace KaoHsiung.ReaderScoreImport.Validation.RecordValidators
{
    internal class SCAttendValidator : IRecordValidator<DataRecord>
    {
        private StudentCourseInfo _studentCourseInfo;
        public SCAttendValidator(StudentCourseInfo sudentCourseInfo)
        {
            _studentCourseInfo = sudentCourseInfo;
        }

        #region IRecordValidator<DataRecord> 成員

        public string Validate(DataRecord record)
        {
            string studentNumber = record.StudentNumber;
            List<string> subjects = new List<string>(record.Subjects);
            foreach (JHCourseRecord course in _studentCourseInfo.GetCourses(record.StudentNumber))
                subjects.Remove(course.Subject);

            if (subjects.Count > 0)
            {
                StringBuilder builder = new StringBuilder("");
                foreach (string s in subjects)
                    builder.Append(s + "、");
                string result = builder.ToString();
                if (result.EndsWith("、")) result = result.Substring(0, result.Length - 1);
                if (_studentCourseInfo.ContainsStudentNumber(record.StudentNumber))
                    return string.Format("學生「{0}」並沒有修習科目「{1}」。", _studentCourseInfo.GetStudentName(record.StudentNumber), result);
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        #endregion
    }
}
