//using System;
//using System.Collections.Generic;
//using System.Text;
//using DevComponents.DotNetBar;
//using System.Collections.ObjectModel;

//namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.AssessmentSetupManagerControls
//{
//    internal class Template : ButtonItem
//    {
//        private string _identity, _name, _allow_upload;
//        private string _start_time, _end_time;
//        private ExamCollection _exams;

//        public Template(string identity, string name)
//        {
//            TemplateName = name;
//            _identity = identity;
//            _exams = new ExamCollection();
//        }

//        public string TemplateName
//        {
//            get { return _name; }
//            set
//            {
//                _name = value;
//                Text = value;
//                Refresh();
//            }
//        }

//        public string Identity
//        {
//            get { return _identity; }
//        }

//        public string AllowUpload
//        {
//            get { return _allow_upload; }
//            set { _allow_upload = value; }
//        }

//        public string StartTime
//        {
//            get { return _start_time; }
//            set { _start_time = value; }
//        }

//        public string EndTime
//        {
//            get { return _end_time; }
//            set { _end_time = value; }
//        }

//        public ExamCollection Exams
//        {
//            get { return _exams; }
//        }
//    }
//}
