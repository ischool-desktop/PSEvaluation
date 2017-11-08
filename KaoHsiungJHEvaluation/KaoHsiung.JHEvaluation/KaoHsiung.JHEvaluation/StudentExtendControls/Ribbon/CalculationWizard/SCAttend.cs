using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using SmartSchool.Feature.Course;
using JHSchool.Evaluation.Feature.Legacy;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard
{
    class SCAttend
    {
        private string _identity, _student_identity, _course_id;
        private string _class_name, _student_number, _stu_name, _seat_number;
        private ExamScoreCollection _scetakes;
        private Course _course;
        private string _previous_score, _score;
        private string _previous_effort, _effort;
        private bool _contains_lack = false;
        private bool _no_exam = false;
        private List<string> _score_lack;
        private List<string> _effort_lack;

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
            _previous_effort = objData.GetText("Extension/Extension/Effort");

            _scetakes = new ExamScoreCollection();
            _score_lack = new List<string>();
            _effort_lack = new List<string>();
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

        public string PreviousEffort
        {
            get { return _previous_effort; }
            set { _previous_effort = value; }
        }

        public string Effort
        {
            get { return _effort; }
        }

        public void SaveEffort()
        {
            _previous_effort = _effort;
        }

        public void SetEffort(string effort)
        {
            _effort = effort;
        }

        public void SetCourse(Course course)
        {
            _course = course;
        }

        public bool NoExam
        {
            get { return _no_exam; }
        }

        public bool ContainsLack
        {
            get { return _contains_lack; }
        }

        public List<string> ScoreLack
        {
            get { return _score_lack; }
        }

        public List<string> EffortLack
        {
            get { return _effort_lack; }
        }

        private void SetScoreLack(string exam)
        {
            if (!_score_lack.Contains(exam))
                _score_lack.Add(exam);
        }

        private void SetEffortLack(string exam)
        {
            if (!_effort_lack.Contains(exam))
                _effort_lack.Add(exam);
        }

        public void CalculateScore()
        {
            if (Course.ExamTemplate == null)
                return;

            if (!Course.ExamRequired)
                return;

            //AllowUpload 為 True 時，略過成績計算，因為成績是由老師提供。
            //if (Course.ExamTemplate.AllowUpload)
            //    return;

            decimal total = 0;
            int effort_total = 0;
            int effort_count = 0;
            bool hasScore = false;
            bool hasEffort = false;

            if (new List<TEInclude>(Course.RefExams).Count == 0)
            {
                _no_exam = true;
                return;
            }

            foreach (TEInclude exam in Course.RefExams)
            {
                decimal score = 0;
                int effort = 0;
                if (SCETakes.ContainsKey(exam.ExamId))
                {
                    SCETake take = SCETakes[exam.ExamId];

                    if (exam.UseScore)
                    {
                        if (!decimal.TryParse(take.Score, out score))
                        {
                            _contains_lack = true;
                            SetScoreLack(exam.ExamName);
                        }
                        else
                        {
                            total += (score * ((decimal)exam.Weight / 100m));
                            hasScore = true;
                        }
                    }

                    if (exam.UseEffort)
                    {
                        if (!int.TryParse(take.Effort, out effort))
                        {
                            _contains_lack = true;
                            SetEffortLack(exam.ExamName);
                        }
                        else
                        {
                            effort_count++;
                            effort_total += effort;
                            hasEffort = true;
                        }
                    }
                }
                else
                {
                    if (exam.UseScore)
                    {
                        _contains_lack = true;
                        SetScoreLack(exam.ExamName);
                    }
                    if (exam.UseEffort)
                    {
                        _contains_lack = true;
                        SetEffortLack(exam.ExamName);
                    }
                }
            }

            //SetScore(Math.Round(total, 2).ToString());
            if (hasScore)
            {
                SetScore(total.ToString());
            }
            if (hasEffort)
            {
                SetEffort("");
                if (effort_count > 0)
                    SetEffort("" + (effort_total / effort_count));
            }
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

                //_progress.ReportProgress(string.Format("下載修課相關資料({0}%)", Math.Round(((float)current / (float)_packings.Count) * 100, 0)), 0);
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
