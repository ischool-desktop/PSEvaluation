using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using SmartSchool.Feature.Course;
using JHSchool.Evaluation.Feature.Legacy;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.CalculationWizard
{
    class SCAttend
    {
        private string _identity, _student_identity, _course_id;
        private string _class_name, _student_number, _stu_name, _seat_number;
        private ExamScoreCollection _scetakes;
        private Course _course;
        private string _previous_score, _score;
        private bool _contains_lack = false;

        public SCAttend(XmlElement data)
        {
            DSXmlHelper objData = new DSXmlHelper(data);
            _identity = objData.GetText("@ID");
            _student_identity = objData.GetText("RefStudentID");
            _course_id = objData.GetText("RefCourseID");
            _class_name = objData.GetText("ClassName");
            _student_number = objData.GetText("StudentNumber");
            _seat_number = objData.GetText("SeatNumber");
            _stu_name = objData.GetText("Name");
            _previous_score = objData.GetText("Score");

            _scetakes = new ExamScoreCollection();
        }

        public string Identity
        {
            get { return _identity; }
        }

        public string StudentIdentity
        {
            get { return _student_identity; }
        }

        public string CourseIdentity
        {
            get { return _course_id; }
        }

        public string ClassName
        {
            get { return _class_name; }
        }

        public string StudentNumber
        {
            get { return _student_number; }
        }

        public string StudentName
        {
            get { return _stu_name; }
        }

        public string SeatNumber
        {
            get { return _seat_number; }
        }

        public Course Course
        {
            get { return _course; }
        }

        public ExamScoreCollection SCETakes
        {
            get { return _scetakes; }
        }

        public string PreviousScore
        {
            get { return _previous_score; }
            set { _previous_score = value; }
        }

        public string Score
        {
            get { return _score; }
        }

        public void SaveScore()
        {
            _previous_score = _score;
        }

        public void SetScore(string score)
        {
            _score = score;
        }

        public void SetCourse(Course course)
        {
            _course = course;
        }

        public bool ContainsLack
        {
            get { return _contains_lack; }
        }

        public void CalculateScore()
        {
            if (Course.ExamTemplate == null)
                return;

            if (!Course.ExamRequired)
                return;

            //AllowUpload 為 True 時，略過成績計算，因為成績是由老師提供。
            if (Course.ExamTemplate.AllowUpload)
                return;

            decimal total = 0;
            foreach (TEInclude exam in Course.RefExams)
            {
                decimal score = 0;
                if (SCETakes.ContainsKey(exam.ExamId))
                {
                    SCETake take = SCETakes[exam.ExamId];
                    if (!decimal.TryParse(take.Score, out score)) //如果缺考會 0 分處理。
                        _contains_lack = true;
                }
                else
                    _contains_lack = true;

                total += (score * ((decimal)exam.Weight / 100m));
            }

            //SetScore(Math.Round(total, 2).ToString());
            SetScore(total.ToString());
        }

        public static SCAttendCollection GetSCAttends(IProgressUI progress, params string[] courseIds)
        {
            SCAttendPackingLoader loader = new SCAttendPackingLoader(progress, courseIds);
            return loader.LoadData();
        }
    }

    class SCAttendCollection : Dictionary<string, SCAttend>
    {
    }

    class SCAttendPackingLoader
    {
        private const int PackingSize = 50;

        private IProgressUI _progress;
        private List<List<string>> _packings;

        public SCAttendPackingLoader(IProgressUI progress, string[] courseIds)
        {
            _progress = progress;

            _packings = new List<List<string>>();
            List<string> package = new List<string>();
            for (int i = 0; i < courseIds.Length; i++)
            {
                if (i % PackingSize == 0)
                {
                    package = new List<string>();
                    _packings.Add(package);
                }
                package.Add(courseIds[i]);
            }
        }

        public SCAttendCollection LoadData()
        {
            int current = 0;
            SCAttendCollection objSCAttends = new SCAttendCollection();
            foreach (List<string> each in _packings)
            {
                current++;

                if (_progress.Cancellation) break;

                _progress.ReportProgress(string.Format("下載修課相關資料({0}%)", Math.Round(((float)current / (float)_packings.Count) * 100, 0)), 0);
                XmlElement xmlSCAttends = QueryCourse.GetSCAttendBrief(each.ToArray()).GetContent().BaseElement;

                foreach (XmlElement attend in xmlSCAttends.SelectNodes("Student"))
                {
                    SCAttend scattend = new SCAttend(attend);
                    objSCAttends.Add(scattend.Identity, scattend);
                }
            }

            return objSCAttends;
        }
    }
}
