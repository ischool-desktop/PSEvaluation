using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;

namespace JHEvaluation.ClassSemesterScoreAvgComparison.DAL
{
    // 負責與 DAL 層存取資料，組成相對應 Class   

    class DALTransfer
    {
        private static Dictionary<string, int> _ClassStudCount = new Dictionary<string, int>();
        private static List<JHSemesterScoreRecord> _SemesterScoreRecordList;
        private static Dictionary<string, string> _domainMapping = new Dictionary<string, string>();

        /// <summary>
        /// 載入多筆學生學期成績(依學年度學期篩選)(一般生)
        /// </summary>
        /// <param name="SchoolYear"></param>
        /// <param name="Semester"></param>
        /// <param name="StudentIDList"></param>
        public static void LoadSemesterScoreRecord(int SchoolYear, int Semester, List<string> ClassIDList, List<string> notRankStudentIDList)
        {
            _ClassStudCount.Clear();
            List<JHClassRecord> ClassRecList = JHClass.SelectByIDs(ClassIDList);
            List<string> StudentIDList = new List<string>();
            foreach (JHClassRecord ClassRec in ClassRecList)
                foreach (JHStudentRecord studRec in ClassRec.Students)
                    if (studRec.Status == K12.Data.StudentRecord.StudentStatus.一般 && !notRankStudentIDList.Contains(studRec.ID))//剔除不排名學生
                    {
                        StudentIDList.Add(studRec.ID);

                        if (_ClassStudCount.ContainsKey(studRec.Class.ID))
                            _ClassStudCount[studRec.Class.ID]++;
                        else
                            _ClassStudCount.Add(studRec.Class.ID, 1);
                    }

            // 取得多筆學生學年度學期成績
            _SemesterScoreRecordList = JHSemesterScore.SelectBySchoolYearAndSemester(StudentIDList, SchoolYear, Semester);            
        }


        /// <summary>
        /// 取得班級符合條件成績
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, ClassEntity> GetClassEntityDic(List<string> SubjList, List<string> DomainList, List<string> NotRankStudentIDList)
        {
            Dictionary<string, ClassEntity> ClassEntityDic = new Dictionary<string, ClassEntity>();

            foreach (JHSemesterScoreRecord SemsRec in _SemesterScoreRecordList)
            {
                string ClassName=string.Empty;

                //剔除 不排名學生
                if (NotRankStudentIDList.Contains(SemsRec.RefStudentID))
                {
                    continue;                                
                }


                // 取得學生班名
                if(SemsRec.Student.Class !=null )
                    ClassName =SemsRec.Student.Class.Name;

                // 如果沒有班級名稱跳過
                if(string.IsNullOrEmpty(ClassName ))
                    continue;

                // 檢查班級
                if (ClassEntityDic.ContainsKey(ClassName))
                {
                    // 科目成績
                    foreach (KeyValuePair<string, K12.Data.SubjectScore> val in SemsRec.Subjects)
                    {
                        if (SubjList.Contains(val.Key))
                            if (val.Value.Score.HasValue)
                                ClassEntityDic[ClassName].AddClassSubjDomainScore(val.Key, val.Value.Score.Value);
                        if (!_domainMapping.ContainsKey(val.Value.Subject))
                            _domainMapping.Add(val.Value.Subject, val.Value.Domain);
                    }

                    // 領域成績
                    foreach (KeyValuePair<string, K12.Data.DomainScore> val in SemsRec.Domains)
                        if(DomainList.Contains(val.Key))
                            if (val.Value.Score.HasValue)
                                ClassEntityDic[ClassName].AddClassSubjDomainScore(val.Key+"_", val.Value.Score.Value);
                }
                else
                { 
                    // 新增
                    ClassEntity ce = new ClassEntity();
                    ce.ClassID = SemsRec.Student.Class.ID;
                    ce.ClassName = ClassName;

                    // 科目成績
                    foreach (KeyValuePair<string, K12.Data.SubjectScore> val in SemsRec.Subjects)
                        if(SubjList.Contains(val.Key ))
                            if (val.Value.Score.HasValue)
                                ce.AddClassSubjDomainScore(val.Key, val.Value.Score.Value);

                    // 領域成績
                    foreach (KeyValuePair<string, K12.Data.DomainScore> val in SemsRec.Domains)
                        if(DomainList.Contains(val.Key ))
                            if (val.Value.Score.HasValue)
                                ce.AddClassSubjDomainScore(val.Key+"_", val.Value.Score.Value);

                    ClassEntityDic.Add(ClassName, ce);
                }
            }

            // 填入班級一般生人數
            foreach (ClassEntity ce in ClassEntityDic.Values)
                if (_ClassStudCount.ContainsKey(ce.ClassID))
                    ce.StudCount = _ClassStudCount[ce.ClassID];

            return ClassEntityDic;

        }

        /// <summary>
        /// 取得科目領域的對照表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetDomainMapping()
        {
            return _domainMapping;
        }

        /// <summary>
        /// 取得科目名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSubjectName()
        { 
            List<string> subjNameList = new List<string>();

            if(_SemesterScoreRecordList !=null )
            foreach (JHSemesterScoreRecord SSRec in _SemesterScoreRecordList)
            {
                foreach (string subjName in SSRec.Subjects.Keys)
                    if (!subjNameList.Contains(subjName))
                        subjNameList.Add(subjName);
            }
            return subjNameList;
        }

        public static List<string> GetDoaminName()
        {
            List<string> DomainNameList = new List<string>();
            if(_SemesterScoreRecordList !=null )
            foreach (JHSemesterScoreRecord SSRec in _SemesterScoreRecordList)
            {
                foreach (string DomainName in SSRec.Domains.Keys)
                    if (!DomainNameList.Contains(DomainName))
                        DomainNameList.Add(DomainName);
            }
            return DomainNameList;        
        }

    }
}
