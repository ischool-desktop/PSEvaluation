using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.Absence
{
    public class MeritStatisticItem
    {
        public string SchoolYear { get; set; }
        public string Semester { get; set; }
        public string MeritType { get; set; }
        public int Count { get; set; }
        //public Dictionary<string,int> MeritMapping{ get; set; }           
    }
}
