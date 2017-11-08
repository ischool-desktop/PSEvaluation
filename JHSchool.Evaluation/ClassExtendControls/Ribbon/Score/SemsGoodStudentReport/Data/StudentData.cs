using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport.Data
{
    public class StudentData
    {        
        public K12.Data.StudentRecord Student { get; set; }
        public decimal Score { get; set; }
        public int Rank { get; set; }
    }
}
