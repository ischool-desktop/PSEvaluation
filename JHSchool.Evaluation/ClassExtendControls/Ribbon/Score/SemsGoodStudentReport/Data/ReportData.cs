using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport.Data
{
    public class ReportData
    {
        public List<ClassData> Classes { get; set; }

        public ReportData()
        {
            Classes = new List<ClassData>();
        }

        public void SortClass()
        {
            Classes.Sort(new ClassComparer());
        }

        public void SetStudentScore(string studentid, decimal score)
        {
            foreach (ClassData cd in Classes)
            {
                foreach(StudentData sd in cd.Students)
                {
                    if (sd.Student.ID == studentid)
                    {
                        sd.Score = score;
                        return;
                    }
                }
            }
        }
    }
}
