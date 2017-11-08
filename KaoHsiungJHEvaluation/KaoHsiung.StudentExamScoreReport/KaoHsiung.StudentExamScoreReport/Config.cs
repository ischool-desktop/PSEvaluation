using System;
using System.Collections.Generic;
using System.Xml;
using JHSchool.Data;
using K12.Data.Configuration;
using System.Linq;

namespace KaoHsiung.StudentExamScoreReport
{
    /// <summary>
    /// 報表設定資訊
    /// </summary>
    public class Config
    {
        //public bool PrintPeriod { get; private set; }
        //public bool PrintCredit { get; private set; }

        /// <summary>
        /// 使用者所選取的試別
        /// </summary>
        public JHExamRecord Exam { get; private set; }
        /// <summary>
        /// 使用者設定的缺曠獎懲統計開始時間
        /// </summary>
        public DateTime StartDate { get; private set; }
        /// <summary>
        /// 使用者設定的缺曠獎懲統計結束時間
        /// </summary>
        public DateTime EndDate { get; private set; }
        /// <summary>
        /// 使用者設定缺曠類別及假別對照表
        /// </summary>
        public Dictionary<string, List<string>> PrintTypes { get; private set; }
        /// <summary>
        /// 使用者設定要列印是領域還是科目，其值一般為Domain或是Subject
        /// </summary>
        public string DomainSubjectSetup { get; set; }
        /// <summary>
        /// 使用者所選擇的學生清單列表
        /// </summary>
        public List<JHStudentRecord> Students { get; private set; }
        /// <summary>
        /// 使用者設定的學年度
        /// </summary>
        public int SchoolYear { get; set; }
        /// <summary>
        /// 使用者設定的學期
        /// </summary>
        public int Semester { get; set; }
        //public Dictionary<string, List<string>> PrintSubjects { get; private set; }
        /// <summary>
        /// 哪些領域是要展開科目，哪些領域不用展開；false是要展開，true是不用展開。
        /// </summary>
        public Dictionary<string, bool> PrintDomains { get; private set; }

        Dictionary<string, List<string>> aeExamName;

        //public bool PrintAssignment { get; private set; }
        //public string AssignmentExamID { get; private set; }
        private List<string> _asWithExam;

        private string _name;

        public Config(string name)
        {
            _name = name;
            PrintTypes = new Dictionary<string, List<string>>();
            //PrintSubjects = new Dictionary<string, List<string>>();
            PrintDomains = new Dictionary<string, bool>();
            aeExamName = new Dictionary<string, List<string>>();
            _asWithExam = new List<string>();
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
            bool retVal=false ;
            // 取得考試

            if(aeExamName.ContainsKey(course.RefAssessmentSetupID ))
                if(aeExamName[course.RefAssessmentSetupID].Contains(Exam.Name ))
                    retVal =true;

            return retVal ;
            
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

        //internal void SetPrintSubjects(Dictionary<string, List<string>> subjects)
        //{
        //    PrintSubjects = subjects;
        //}

        internal void SetStudents(List<JHStudentRecord> students)
        {
            Students = students;
        }
        #endregion
    }
}
