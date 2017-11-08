using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRationality;
using System.Data;
using FISCA.Data;
using JHSchool.Data;


namespace JHSchool.Evaluation.StudentExtendControls.Ribbon
{
    public class CheckStudentSemHistoryScoreRATRec
    {
        public string 學生系統編號 {get;set;}
        public string 學號 { get; set; }
        public string 身分證號 { get; set; }
        public string 班級 { get; set; }
        public string 座號 { get; set; }
        public string 姓名 { get; set; }
        public string 狀態 { get; set; }
        public string 學年度 { get; set; }
        public string 學期 { get; set; }
        public string 說明 { get; set; }
    }

    public class CheckStudentSemHistoryScoreRAT :IDataRationality
    {
        List<CheckStudentSemHistoryScoreRATRec> RATRecs = new List<CheckStudentSemHistoryScoreRATRec>();

        #region IDataRationality 成員

        public void AddToTemp(IEnumerable<string> EntityIDs)
        {
            List<string> PrimaryKeys = new List<string>();

            if (EntityIDs == null)
            {
                // 全部
                List<string> ids = (from data in RATRecs select data.學生系統編號).Distinct().ToList ();
                PrimaryKeys.AddRange(ids);
            }
            else
                PrimaryKeys.AddRange(EntityIDs);

            PrimaryKeys.AddRange(K12.Presentation.NLDPanels.Student.TempSource);
            K12.Presentation.NLDPanels.Student.AddToTemp(PrimaryKeys.Distinct().ToList());
        }

        public void AddToTemp()
        {
            AddToTemp(null);
        }

        public string Category
        {
            get { return "成績"; }
        }

        public string Description
        {
            get
            {
                StringBuilder strBuilder = new System.Text.StringBuilder();
                strBuilder.AppendLine("檢查範圍：所有學生的學習歷程與學期成績。");
                strBuilder.AppendLine("檢查項目：檢查學生學習歷程：學年度、學期，學生學期成績：學年度、學期，是否有不一致。");
                strBuilder.AppendLine("檢查意義：找出學生有學期歷程沒有學期成績，或沒有學期歷程有學期成績。");
                return strBuilder.ToString();
            }
        }        

        public DataRationalityMessage Execute()
        {
            RATRecs.Clear();            

            // 沒成績
            List<CheckStudentSemHistoryScoreRATRec> noScore = new List<CheckStudentSemHistoryScoreRATRec>();
            // 沒學期歷程
            List<CheckStudentSemHistoryScoreRATRec> noHistory = new List<CheckStudentSemHistoryScoreRATRec>();

            // Query: 取得學生有學期成績的學年度、學期
            QueryHelper Helper = new QueryHelper();
            string strSQL = "select ref_student_id,school_year,semester from sems_subj_score where score_info like '%成績%' order by ref_student_id,school_year,semester";
            DataTable dtScore =Helper.Select(strSQL);
            Dictionary<string, List<string>> studScoreScDict = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> studHistoryScDict = new Dictionary<string, List<string>>();
            foreach (DataRow d in dtScore.Rows)
            {
                string ID = d[0].ToString();
                string val = d[1].ToString()+","+d[2].ToString();

                if (studScoreScDict.ContainsKey(ID))
                {
                    studScoreScDict[ID].Add(val);
                }
                else
                {
                    List<string> strList = new List<string>();
                    strList.Add(val);
                    studScoreScDict.Add(ID, strList);
                }
            }

            // 取得所有學生
            List<JHStudentRecord> studRecList =JHStudent.SelectAll();

            // 取得學生學習歷程
            Dictionary<string, List<K12.Data.SemesterHistoryItem>> SHistoryDict = new Dictionary<string, List<K12.Data.SemesterHistoryItem>>();
            foreach (JHSemesterHistoryRecord rec in JHSemesterHistory.SelectAll())
            {
                foreach (K12.Data.SemesterHistoryItem shi in rec.SemesterHistoryItems)
                {
                    string val = shi.SchoolYear.ToString() + "," + shi.Semester.ToString();
                    if (studHistoryScDict.ContainsKey(shi.RefStudentID))
                    {
                        studHistoryScDict[shi.RefStudentID].Add(val);
                    }
                    else
                    {
                        List<string> strList = new List<string>();
                        strList.Add(val);
                        studHistoryScDict.Add(shi.RefStudentID, strList);
                    }
                }
            }
                        
            
            DataRationalityMessage retMsg = new DataRationalityMessage();
            try
            {
                foreach (JHStudentRecord studRec in studRecList)
                {
                    // 依學期歷程為主
                    if (studHistoryScDict.ContainsKey(studRec.ID))
                    {
                        if (studScoreScDict.ContainsKey(studRec.ID))
                        {
                            foreach (string str in studHistoryScDict[studRec.ID])
                            {
                                if (!studScoreScDict[studRec.ID].Contains(str))
                                {
                                    CheckStudentSemHistoryScoreRATRec rec1 = new CheckStudentSemHistoryScoreRATRec();
                                    rec1.身分證號 = studRec.IDNumber;
                                    rec1.姓名 = studRec.Name;
                                    rec1.狀態 = studRec.StatusStr;
                                    if (studRec.SeatNo.HasValue)
                                        rec1.座號 = studRec.SeatNo.Value.ToString();
                                    if (studRec.Class != null)
                                        rec1.班級 = studRec.Class.Name;
                                    rec1.說明 = "有學期歷程沒有成績";
                                    rec1.學生系統編號 = studRec.ID;
                                    rec1.學號 = studRec.StudentNumber;
                                    string[] ss = str.Split(',').ToArray();
                                    if (ss.Length > 1)
                                    {
                                        rec1.學年度 = ss[0];
                                        rec1.學期 = ss[1];
                                    }
                                    noScore.Add(rec1);
                                }
                            }
                        }
                    }


                    // 依學期成績為主
                    if (studScoreScDict.ContainsKey(studRec.ID))
                    {
                        if (studHistoryScDict.ContainsKey(studRec.ID))
                        {
                            foreach (string str in studScoreScDict[studRec.ID])
                            {
                                if (!studHistoryScDict[studRec.ID].Contains(str))
                                {
                                    CheckStudentSemHistoryScoreRATRec rec1 = new CheckStudentSemHistoryScoreRATRec();
                                    rec1.身分證號 = studRec.IDNumber;
                                    rec1.姓名 = studRec.Name;
                                    rec1.狀態 = studRec.StatusStr;
                                    if (studRec.SeatNo.HasValue)
                                        rec1.座號 = studRec.SeatNo.Value.ToString();
                                    if (studRec.Class != null)
                                        rec1.班級 = studRec.Class.Name;
                                    rec1.說明 = "沒有學期歷程有成績";
                                    rec1.學生系統編號 = studRec.ID;
                                    rec1.學號 = studRec.StudentNumber;
                                    string[] ss = str.Split(',').ToArray();
                                    if (ss.Length > 1)
                                    {
                                        rec1.學年度 = ss[0];
                                        rec1.學期 = ss[1];
                                    }
                                    noHistory.Add(rec1);
                                }
                            }
                        }
                    }
                }
                    RATRecs.AddRange(noHistory);
                    RATRecs.AddRange(noScore);
                    int no;
                    var sortedRATRecs = from rec in RATRecs orderby rec.狀態, rec.班級, int.TryParse(rec.座號,out no), rec.學年度, rec.學期 select rec;
                    retMsg.Message = "有學期歷程沒有成績共"+noScore.Count+"筆,沒有學期歷程有成績共"+noHistory.Count+"筆。" ;
                    retMsg.Data = sortedRATRecs.ToList ();                    
            }
            catch (Exception ex)
            {
                retMsg.Message = ex.Message;
                return retMsg;
            }

            return retMsg;
        }

        public string Name
        {
            get { return "學期歷程與學期成績(學年度、學期)檢查"; }
        }

        #endregion
    }
}
