using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace JHEvaluation.StudentSemesterScoreNotification
{
    internal class Options
    {
        public int SchoolYear { get; set; }
        public int Semester { get; set; }
        public List<JHStudentRecord> Students { get; set; }
    }
}
