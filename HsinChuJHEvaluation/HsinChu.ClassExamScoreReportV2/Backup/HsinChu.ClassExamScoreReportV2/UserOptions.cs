using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace HsinChu.ClassExamScoreReportV2
{
    internal class UserOptions
    {
        public int SchoolYear { get; set; }
        public int Semester { get; set; }
        public JHExamRecord Exam { get; set; }
        public string ScoreSource { get; set; }
    }
}
