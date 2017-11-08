using System;
using System.Collections.Generic;
using System.Text;
using JHSchool;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard
{
    class CourseDataLoader
    {
        private IProgressUI _progress_ui;
        private CourseCollection _courses;
        private int _current_step;

        private void ReportProgress(string message, int progress)
        {
            _progress_ui.ReportProgress(message, progress);
        }

        public int CurrentStep
        {
            get { return _current_step; }
            private set { _current_step = value; }
        }

        public void LoadCalculationData(IProgressUI progressUI, IEnumerable<StudentRecord> students, int schoolYear, int semester)
        {
            _progress_ui = progressUI;
            _courses = new CourseCollection();

            List<string> courseIds = new List<string>();
            //foreach (CourseRecord each in JHSchool.Course.Instance.SelectedList)
            //    courseIds.Add(each.ID);
            foreach (StudentRecord each in students)
            {
                foreach (CourseRecord attend_course in GetAttendCourses(each))
                {
                    if (attend_course.SchoolYear != schoolYear) continue;
                    if (attend_course.Semester != semester) continue;

                    if (!courseIds.Contains(attend_course.ID))
                        courseIds.Add(attend_course.ID);
                }
            }

            CurrentStep = 1;
            //ReportProgress("下載課程相關資料...", 0);
            CourseCollection courses = Course.GetCourses(courseIds.ToArray());
            if (_progress_ui.Cancellation) return;

            CurrentStep++;
            //ReportProgress("下載評量相關資料...", 0);
            ExamTemplateCollection templates = ExamTemplate.GetExamTemplates();
            if (_progress_ui.Cancellation) return;

            TEIncludeCollection teincludes = TEInclude.GetTEIncludes();
            if (_progress_ui.Cancellation) return;

            CurrentStep++;
            SCAttendCollection scattends = SCAttend.GetSCAttends(_progress_ui, courseIds.ToArray());
            if (_progress_ui.Cancellation) return;

            CurrentStep++;
            SCETakeCollection scetakes = SCETake.GetSCETakes(_progress_ui, courseIds.ToArray());
            if (_progress_ui.Cancellation) return;

            //建立 ExamTemplate 的 TEInclude。
            CreateTemplateExamReference(templates, teincludes);

            //建立 Course 的 ExamTemplate。
            CreateCourseTemplateReference(courses, templates);

            //建立 Course 的 SCAttend。
            CreateCourseStudentTwoWayReference(courses, scattends);

            //建立 SCAttend  的 SCETake。
            CreateSCAttendSCETakeReference(scattends, scetakes);

            _courses = courses;
        }

        public CourseCollection Courses
        {
            get { return _courses; }
        }

        private static void CreateSCAttendSCETakeReference(SCAttendCollection scattends, SCETakeCollection scetakes)
        {
            foreach (SCETake each in scetakes.Values)
            {
                if (scattends.ContainsKey(each.SCAttendId))
                {
                    SCAttend scattend = scattends[each.SCAttendId];
                    scattend.SCETakes.Add(each);
                }
                else
                    Console.WriteLine("SCETake Error:" + each.Identity);
            }
        }

        private static void CreateCourseStudentTwoWayReference(CourseCollection courses, SCAttendCollection scattends)
        {
            foreach (SCAttend each in scattends.Values)
            {
                if (courses.ContainsKey(each.CourseIdentity))
                {
                    Course course = courses[each.CourseIdentity];
                    course.SCAttends.Add(each.Identity, each);
                    each.SetCourse(course);
                }
                else
                    Console.WriteLine("SCAttend Error:" + each.Identity);
            }
        }

        private static void CreateCourseTemplateReference(CourseCollection courses, ExamTemplateCollection templates)
        {
            foreach (Course each in courses.Values)
            {
                if (templates.ContainsKey(each.ExamTemplateId))
                    each.SetExamTemplate(templates[each.ExamTemplateId]);
                else
                    Console.WriteLine("Course Error:" + each.Identity);
            }
        }

        private static void CreateTemplateExamReference(ExamTemplateCollection templates, TEIncludeCollection teincludes)
        {
            foreach (TEInclude each in teincludes.Values)
            {
                if (templates.ContainsKey(each.ExamTemplateId))
                {
                    ExamTemplate template = templates[each.ExamTemplateId];
                    template.TEIncludes.Add(each.Identity, each);
                }
                else
                    Console.WriteLine("TEInclude Error:" + each.Identity);
            }
        }

        public List<CourseRecord> GetAttendCourses(StudentRecord studentRecord)
        {
            List<CourseRecord> courses = new List<CourseRecord>();
            foreach (var item in JHSchool.Evaluation.SCAttend.Instance.GetStudentAttend(studentRecord.ID))
            {
                courses.Add(item.Course);
            }
            return courses;
        }
    }
}
