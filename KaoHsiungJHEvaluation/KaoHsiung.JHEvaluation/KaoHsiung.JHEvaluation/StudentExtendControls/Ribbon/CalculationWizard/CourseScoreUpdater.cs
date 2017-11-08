using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.Feature.Course;
using FISCA.DSAUtil;
using System.Xml;
using JHSchool.Evaluation.Feature.Legacy;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard
{
    class CourseScoreUpdater
    {
        private const int PackingSize = 15;
        private List<List<Course>> _packings;

        private CourseCollection _courses;
        private IProgressUI _progress;
        private bool _clear_data = false;

        public CourseScoreUpdater(CourseCollection courses, IProgressUI progress, bool clearData)
        {
            _courses = courses;
            _progress = progress;
            _clear_data = clearData;

            _packings = new List<List<Course>>();
            List<Course> package = new List<Course>();
            int currentIndex = 0;
            foreach (Course each in courses.Values)
            {
                if (currentIndex % PackingSize == 0)
                {
                    package = new List<Course>();
                    _packings.Add(package);
                }

                package.Add(each);
                currentIndex++;
            }
        }

        private IProgressUI Progress
        {
            get { return _progress; }
        }

        public void UpdateToServer()
        {
            int progress = 0;
            bool updateRequired = false;
            foreach (List<Course> each in _packings)
            {
                DSXmlHelper request = new DSXmlHelper("Request");

                foreach (Course course in each)
                {
                    if (course.ExamTemplate == null)
                        continue;

                    if (!course.ExamRequired)
                        continue;

                    //AllowUpload 為 True 時，略過成績計算，因為成績是由老師提供。
                    //if (course.ExamTemplate.AllowUpload)
                    //    continue;

                    foreach (SCAttend attend in course.SCAttends.Values)
                    {
                        XmlElement xmlAttend = request.AddElement("Attend");
                        DSXmlHelper hlpAttend = new DSXmlHelper(xmlAttend);
                        hlpAttend.AddElement(".", "ID", attend.Identity);

                        if (_clear_data)
                        {
                            hlpAttend.AddElement(".", "Score", "");
                            hlpAttend.AddElement(".", "Extension");
                            hlpAttend.AddElement("Extension", "Extension");
                            hlpAttend.AddElement("Extension/Extension", "Effort", "");
                            attend.PreviousScore = string.Empty;
                            attend.PreviousEffort = string.Empty;
                        }
                        else
                        {
                            hlpAttend.AddElement(".", "Score", attend.Score);
                            hlpAttend.AddElement(".", "Extension");
                            hlpAttend.AddElement("Extension", "Extension");
                            hlpAttend.AddElement("Extension/Extension", "Effort", attend.Effort);
                            attend.SaveScore();
                            attend.SaveEffort();
                        }

                        updateRequired = true; //指示是否要執行呼叫 Service 的動作。

                        progress++;
                    }
                }

                if (updateRequired && request.PathExist("Attend"))
                    EditCourse.UpdateAttend(request);

                //Progress.ReportProgress("", progress);
            }
        }
    }
}
