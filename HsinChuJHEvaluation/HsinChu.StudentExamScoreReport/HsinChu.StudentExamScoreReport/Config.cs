using System;
using System.Collections.Generic;
using System.Text;
using K12.Data.Configuration;
using System.Xml;
using JHSchool.Data;
using System.Linq;

namespace HsinChu.StudentExamScoreReport
{
    public class Config
    {
        //public bool PrintPeriod { get; private set; }
        //public bool PrintCredit { get; private set; }

        public JHExamRecord Exam { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public Dictionary<string, List<string>> PrintTypes { get; private set; }
        public Dictionary<string, List<string>> PrintSubjects { get; private set; }
        public Dictionary<string, bool> PrintDomains { get; private set; }
        public string DomainSubjectSetup { get; set; }
        public List<JHStudentRecord> Students { get; private set; }
        public int SchoolYear { get; set; }
        public int Semester { get; set; }

        private List<string> _asWithExam;
        Dictionary<string, List<string>> aeExamName;

        private string _name;

        public Config(string name)
        {
            _name = name;
            PrintTypes = new Dictionary<string, List<string>>();
            PrintSubjects = new Dictionary<string, List<string>>();
            PrintDomains = new Dictionary<string, bool>();

            _asWithExam = new List<string>();
            aeExamName = new Dictionary<string, List<string>>();
            
        }

        /// <summary>
        /// 學生單一學期學期歷程
        /// </summary>
        public static Dictionary<string, K12.Data.SemesterHistoryItem> _StudSemesterHistoryItemDict = new Dictionary<string, K12.Data.SemesterHistoryItem>();


        public void Load()
        {
            ConfigData cd = K12.Data.School.Configuration[_name];

            #region 假別設定
            if (cd.Contains("假別設定") && !string.IsNullOrEmpty(cd["假別設定"]))
            {
                XmlElement config = K12.Data.XmlHelper.LoadXml(cd["假別設定"]);
                foreach (XmlElement type in config.SelectNodes("Type"))
                {
                    string typeName = type.GetAttribute("Text");
                    if (!PrintTypes.ContainsKey(typeName))
                        PrintTypes.Add(typeName, new List<string>());

                    foreach (XmlElement absence in type.SelectNodes("Absence"))
                    {
                        string absenceName = absence.GetAttribute("Text");
                        if (!PrintTypes[typeName].Contains(absenceName))
                            PrintTypes[typeName].Add(absenceName);
                    }
                }
            }
            #endregion

            _asWithExam.Clear();
            foreach (JHAEIncludeRecord ae in JHAEInclude.SelectAll())
            {
                if (ae.RefExamID == Exam.ID)
                {
                    if (!_asWithExam.Contains(ae.RefAssessmentSetupID))
                        _asWithExam.Add(ae.RefAssessmentSetupID);
                }
            }

            // 評分樣板對照
            aeExamName.Clear();
            foreach (JHAEIncludeRecord ae in JHAEInclude.SelectAll())
            {
                if (aeExamName.ContainsKey(ae.RefAssessmentSetupID))
                {
                    if (!aeExamName[ae.RefAssessmentSetupID].Contains(ae.ExamName))
                        aeExamName[ae.RefAssessmentSetupID].Add(ae.ExamName);
                }
                else
                {
                    List<string> exList = new List<string>();
                    exList.Add(ae.ExamName);
                    aeExamName.Add(ae.RefAssessmentSetupID, exList);
                }
            } 
        }

        internal bool HasExam(JHCourseRecord course)
        {
            bool retVal = false;
            // 取得考試

            if (aeExamName.ContainsKey(course.RefAssessmentSetupID))
                if (aeExamName[course.RefAssessmentSetupID].Contains(Exam.Name))
                    retVal = true;

            return retVal;
            //if (string.IsNullOrEmpty(course.RefAssessmentSetupID)) return false;
            //if (_asWithExam.Contains(course.RefAssessmentSetupID)) return true;
            //else return false;
        }

        #region Set Methods
        internal void SetExam(JHExamRecord exam)
        {
            Exam = exam;
        }

        internal void SetDateRange(DateTime from, DateTime to)
        {
            StartDate = from;
            EndDate = to;
        }

        internal void SetPrintDomains(Dictionary<string, bool> domains)
        {
            PrintDomains = domains;
        }

        internal void SetPrintSubjects(Dictionary<string, List<string>> subjects)
        {
            PrintSubjects = subjects;
        }

        internal void SetStudents(List<JHStudentRecord> students)
        {
            Students = students;
        }
        #endregion
    }
}
