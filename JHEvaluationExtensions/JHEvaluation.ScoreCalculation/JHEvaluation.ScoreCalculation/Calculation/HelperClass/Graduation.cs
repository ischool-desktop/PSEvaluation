using System.Collections.Generic;
using JHSchool.Data;
using JHSchool.Evaluation.Calculation.GraduationConditions;

namespace JHSchool.Evaluation.Calculation
{
    /// <summary>
    /// 畢業判斷主要進入點
    /// </summary>
    public class Graduation
    {
        private IEvaluateFactory _factory;
        private Dictionary<string, IEvaluative> _evals;
        private EvaluationResult _result;

        private static Graduation _instance;

        /// <summary>
        /// 取得畢業判斷商業邏輯實體，只會有一個
        /// </summary>
        public static Graduation Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Graduation();
                return _instance;
            }
        }

        /// <summary>
        /// 無參數建構式
        /// </summary>
        private Graduation()
        {
            //建立畢業判斷條件介面
            _factory = new EvaluateFactory();
            //建立畢業判斷規則介斷列表
            _evals = _factory.CreateEvaluativeEntities();
            //畢業判斷回傳結果
            _result = new EvaluationResult();
        }

        /// <summary>
        /// 檢查學生學期歷程
        /// 改寫當學年度、學期、年級加入判斷
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        public List<StudentRecord> CheckSemesterHistories(IEnumerable<StudentRecord> students)
        {
            // 檢查是否新增學期歷程
            bool chkInsertShi = false;

            // 取得目前學生班級年級
            Dictionary<string,int> studGrYearDic = new Dictionary<string,int> ();
            foreach (JHStudentRecord stud in JHStudent.SelectByIDs(students.AsKeyList()))
                if(stud.Class !=null )
                    if(stud.Class.GradeYear.HasValue)
                        studGrYearDic.Add(stud.ID,stud.Class.GradeYear.Value );


            // 取得學生學期歷程
            Dictionary<string, JHSemesterHistoryRecord> studentSemesterHistoryRecordDict = new Dictionary<string, JHSemesterHistoryRecord>();
            foreach (JHSemesterHistoryRecord record in JHSemesterHistory.SelectByStudentIDs(students.AsKeyList()))
            {
                chkInsertShi = true;
                K12.Data.SemesterHistoryItem shi = new K12.Data.SemesterHistoryItem ();
                shi.SchoolYear = UIConfig._UserSetSHSchoolYear;
                shi.Semester = UIConfig._UserSetSHSemester;
                
                if (studGrYearDic.ContainsKey(record.RefStudentID))
                    shi.GradeYear = studGrYearDic[record.RefStudentID];

                // 檢查是否已經有同學年度學期學期歷程
                foreach (K12.Data.SemesterHistoryItem shiItem in record.SemesterHistoryItems)
                {
                    if (shiItem.SchoolYear == UIConfig._UserSetSHSchoolYear && shiItem.Semester == UIConfig._UserSetSHSemester)
                    {
                        chkInsertShi = false;
                        break;
                    }
                }

                // 加入當學年度學期判斷用
                if (chkInsertShi)
                    record.SemesterHistoryItems.Add(shi);

                studentSemesterHistoryRecordDict.Add(record.RefStudentID, record);
               
            }

            List<StudentRecord> errorList = new List<StudentRecord>();

            // 主要修改成檢查是否有學期歷程與不重複，不推測學期歷程是否合理
            foreach (StudentRecord student in students)
            {
                // 當學生沒有學期歷程紀錄
                if (!studentSemesterHistoryRecordDict.ContainsKey(student.ID))
                {
                    errorList.Add(student);
                    continue;
                }

                List<string> checkSame = new List<string>();

                // 有學期歷程
                if (studentSemesterHistoryRecordDict.ContainsKey(student.ID))
                {
                    // 當學生有學期歷程，但是筆數是0
                    if (studentSemesterHistoryRecordDict[student.ID].SemesterHistoryItems.Count == 0)
                    {
                        errorList.Add(student);
                        continue;
                    }

                    checkSame.Clear();
                    foreach (K12.Data.SemesterHistoryItem shi in studentSemesterHistoryRecordDict[student.ID].SemesterHistoryItems)
                    {                        
                        // 當資料有疑問
                        if (shi.SchoolYear < 1 || shi.Semester < 1 || shi.GradeYear < 1)
                        {
                            errorList.Add(student);
                            continue;
                        }

                        // 檢查學期歷程是否有重複(學年度+學期+年級)
                        string key = shi.SchoolYear.ToString() + shi.Semester.ToString() + shi.GradeYear;                        
                        if (checkSame.Contains(key))
                        {
                            errorList.Add(student);
                            continue;                        
                        }
                        checkSame.Add(key);
                    }
                }
            }

            return errorList;
        }

        //private List<SemesterHistoryRecord> ReadSemesterHistory(List<string> studentIDs)
        //{
        //    FunctionSpliter<string, SemesterHistoryRecord> selectData = new FunctionSpliter<string, SemesterHistoryRecord>(500, Util.MaxThread);
        //    selectData.Function = delegate(List<string> p)
        //    {
        //        return K12.Data.SemesterHistory.SelectByStudentIDs(p);
        //    };
        //    return selectData.Execute(studentIDs);
        //}

        /// <summary>
        /// 進行畢業判斷
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        public Dictionary<string, bool> Evaluate(IEnumerable<StudentRecord> students)
        {
            //每位學生過與不過
            Dictionary<string, bool> passList = new Dictionary<string, bool>();            
            List<EvaluationResult> resultList = new List<EvaluationResult>();
            //鍵值為成績計算規則系統編號，值為這些成績計算規則的學生列表
            Dictionary<string, List<StudentRecord>> valid_students = new Dictionary<string, List<StudentRecord>>();

            //針對每位學生
            foreach (StudentRecord student in students)
            {
                //先假設學生通過
                passList.Add(student.ID, true);
                //取得學生成績計算規則
                ScoreCalcRuleRecord rule = student.GetScoreCalcRuleRecord();
                if (rule != null)
                {
                    if (!valid_students.ContainsKey(rule.ID))
                        valid_students.Add(rule.ID, new List<StudentRecord>());
                    valid_students[rule.ID].Add(student);
                }
            }

            //針對每項成績計算規則
            foreach (string rule_id in valid_students.Keys)
            {
                if (_evals.ContainsKey(rule_id))
                {
                    Dictionary<string, bool> evalPassList = _evals[rule_id].Evaluate(valid_students[rule_id]);
                    foreach (string id in evalPassList.Keys)
                        passList[id] = evalPassList[id];
                    resultList.Add(_evals[rule_id].Result);
                }
            }

            if (resultList.Count > 0) MergeResults(passList, resultList);
            return passList;
        }

        private void MergeResults(Dictionary<string, bool> passList, IEnumerable<EvaluationResult> resultList)
        {
            EvaluationResult merged = new EvaluationResult();

            foreach (EvaluationResult result in resultList)
            {
                foreach (string student_id in result.Keys)
                {
                    if (passList[student_id]) continue;
                    merged.MergeResults(student_id, result[student_id]);
                }
            }

            _result = merged;
        }

        public EvaluationResult Result
        {
            get { return _result; }
        }

        public void Refresh()
        {
            _evals = _factory.CreateEvaluativeEntities();
            //_result.Clear();
        }

        internal void SetFactory(IEvaluateFactory factory)
        {
            _factory = factory;
        }

        //清除_instance
        public void Reset()
        {
            _instance = null;
        }

        public void TestDrive()
        {
            //List<StudentRecord> list = Student.Instance.SelectedList.GetInSchoolStudents();

            //List<StudentRecord> error = CheckSemesterHistories(list);
            //if (error.Count > 0)
            //{
            //    JHSchool.Evaluation.Calculation.GraduationForm.ErrorViewer viewer = new JHSchool.Evaluation.Calculation.GraduationForm.ErrorViewer();
            //    viewer.SetHeader("學生");
            //    foreach (StudentRecord student in error)
            //        viewer.SetMessage(student, new List<string>(new string[] { "學期歷程不完整" }));
            //    viewer.ShowDialog();
            //}
            //else
            //{
            //    Dictionary<string, bool> passList = Evaluate(list);
            //    EvaluationResult result = Result;

            //    int a = 0;
            //}
        }
    }
}