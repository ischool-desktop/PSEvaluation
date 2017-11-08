using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    interface IStudentCell : ICell
    {
    }

    class StudentCell : IStudentCell
    {
        private string _value;
        
        public StudentCell(string value)
        {
            _value = value;
        }

        #region ICell жин√

        public string GetValue()
        {
            return _value;    
        }

        #endregion
    }
}
