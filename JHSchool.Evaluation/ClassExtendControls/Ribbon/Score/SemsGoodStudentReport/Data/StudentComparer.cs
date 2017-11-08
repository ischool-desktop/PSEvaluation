using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport.Data
{
    public class StudentComparer : IComparer<StudentData>
    {
        #region IComparer<StudentData> 成員

        public int Compare(StudentData x, StudentData y)
        {
            return x.Score.CompareTo(y.Score) * -1;
        }

        #endregion
    }
}
