using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.Absence
{
    public class AbsenceItem
    {
        public string SchoolYear { get; set; }
        public string Semester { get; set; }
        public string Name { get; set; }
        public string PeriodType { get; set; }
        public int Count { get; set; }        
    }
}
