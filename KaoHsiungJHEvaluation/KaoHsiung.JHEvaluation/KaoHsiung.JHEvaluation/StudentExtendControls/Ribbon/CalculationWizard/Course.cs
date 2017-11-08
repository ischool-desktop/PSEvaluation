using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using JHSchool.Evaluation.Feature.Legacy;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard
{
    class Course
    {
        private string _identity, _course_name, _exam_tempalte_id;
        private bool _exam_required;
        private ExamTemplate _exam_template;
        private SCAttendCollection _scattends;

        private Course(XmlElement data)
        {
            DSXmlHelper objData = new DSXmlHelper(data);

            _identity = objData.GetText("@ID");
            _course_name = objData.GetText("CourseName");
            _exam_tempalte_id = objData.GetText("RefExamTemplateID");
            //_exam_required = (objData.GetText("NotIncludedInCalc") == "是" ? false : true); //高中

            //1:列入學期成績 2:不列入學期成績。
            _exam_required = (objData.GetText("ScoreCalcFlag") == "1" ? true: false); //國中
            
            _scattends = new SCAttendCollection();
        }

        public string Identity
        {
            get { return _identity; }
        }

        public string CourseName
        {
            get { return _course_name; }
        }

        public bool ExamRequired
        {
            get { return _exam_required; }
        }

        public string ExamTemplateId
        {
            get { return _exam_tempalte_id; }
        }

        public ExamTemplate ExamTemplate
        {
            get { return _exam_template; }
        }

        public void SetExamTemplate(ExamTemplate template)
        {
            _exam_template = template;
        }

        public IEnumerable<TEInclude> RefExams
        {
            get
            {
                if (_exam_template == null)
                    return new List<TEInclude>();
                else
                    return _exam_template.TEIncludes.Values;
            }
        }

        public SCAttendCollection SCAttends
        {
            get { return _scattends; }
        }

        public void CalculateScores()
        {
            foreach (SCAttend each in _scattends.Values)
                each.CalculateScore();
        }

        public static CourseCollection GetCourses(params string[] courseList)
        {
            DSResponse rsp = QueryCourse.GetCourseById(courseList);

            CourseCollection courses = new CourseCollection();
            foreach (XmlElement each in rsp.GetContent().GetElements("Course"))
            {
                Course course = new Course(each);
                courses.Add(course.Identity, course);
            }

            return courses;
        }
    }

    class CourseCollection : Dictionary<string, Course>
    {
    }
}
