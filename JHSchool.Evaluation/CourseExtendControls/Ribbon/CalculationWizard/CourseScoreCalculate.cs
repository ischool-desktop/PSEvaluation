using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.CalculationWizard
{
    class CourseScoreCalculate
    {
        private CourseCollection _courses;

        public CourseScoreCalculate(CourseCollection courses)
        {
            _courses = courses;
        }

        public void Calculate()
        {
            foreach (Course course in _courses.Values)
            {
                foreach (SCAttend attend in course.SCAttends.Values)
                    attend.CalculateScore();
            }
        }
    }
}
