using System;
using System.Collections.Generic;
using System.Text;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard
{
    class CourseSummaryCalculate
    {
        private CourseCollection _courses;
        private StringBuilder _message;

        public CourseSummaryCalculate(CourseCollection courses)
        {
            _courses = courses;
        }

        public void Calculate()
        {
            _message = new StringBuilder();
            _message.AppendLine("已選擇課程總數：" + _courses.Count.ToString());

            int count = 0;
            count = CalcNotRequiredCount();
            _message.AppendLine("不列入學期成績課程數：" + count.ToString());

            count = ExamTemplateNullCount();
            _message.AppendLine("無評量課程數：" + count.ToString());

            count = CalcLackScoreCount();
            _message.AppendLine("含有「成績缺漏」之課程數：" + count.ToString());
        }

        private int CalcLackScoreCount()
        {
            int count = 0;
            bool breakCurrent = false;
            foreach (Course each in _courses.Values)
            {
                breakCurrent = false;
                foreach (SCAttend attend in each.SCAttends.Values)
                {
                    foreach (TEInclude exam in each.RefExams)
                    {
                        if (!attend.SCETakes.ContainsKey(exam.ExamId))
                        {
                            count++;
                            breakCurrent = true;
                            break;
                        }
                    }
                    if (breakCurrent) break;
                }
            }
            return count;
        }

        private int CalcNotRequiredCount()
        {
            int count = 0;
            foreach (Course each in _courses.Values)
            {
                if (!each.ExamRequired)
                    count++;
            }
            return count;
        }

        private int ExamTemplateNullCount()
        {
            int count = 0;
            foreach (Course each in _courses.Values)
            {
                if (each.ExamTemplate == null)
                    count++;
            }
            return count;
        }

        public string Message()
        {
            return _message.ToString();
        }
    }
}
