using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using FISCA.Presentation;
using FISCA.LogAgent;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using System.Linq;

namespace JHEvaluation.ScoreCalculation
{
    public partial class GraduateScoreCalculate : BaseForm, IStatusReporter
    {
        private List<string> StudentIDs;

        public GraduateScoreCalculate(List<string> studentIds)
        {
            InitializeComponent();
            StudentIDs = studentIds;
        }

        private void DomainScoreCalculate_Load(object sender, EventArgs e)
        {
            StudentScore.SetClassMapping();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            DialogResult dr = MsgBox.Show("您確定要計算學生畢業成績？", MessageBoxButtons.YesNo);

            if (dr == DialogResult.No) return;

            btnCalc.Enabled = false;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<StudentScore> Students = StudentIDs.ToStudentScore();

            List<StudentScore> noValid = Students.ReadCalculationRule(this); //沒有計算規則的學生。

            if (noValid.Count > 0)
                throw new CalculationException(noValid, "下列學生沒有計算規則，無法計算畢業成績。");

            Students.ReadSemesterScore(this); //讀取所有學期的成績。
            Students.ReadSemesterHistory(this); //讀取學期歷程。
            noValid = Students.VaidSixSemesterHistory(); //驗證學期歷程是否完整。

            if (noValid.Count > 0)
                throw new CalculationException(noValid, "下列學生學期歷程不完整，無法計算畢業成績。");

            noValid = Students.ValidSixSemesterDomainScore(); //驗證學期成績是否完整。

            if (noValid.Count > 0)
                throw new CalculationException(noValid, "下列學生並沒有完整的六學期成績，無法計算畢業成績。");

            Students.ReadGraduateScore(this); //讀取畢業成績。
            Students.CalculateGraduateScore(); //計算畢業成績。

            // 取得學生 ID
            List<string> StudIDs = (from Stud in Students select Stud.Id ).ToList ();
            // 學生科目成績(彈性課程)
            Dictionary<string, List<K12.Data.SubjectScore>> StudSubjScore = new Dictionary<string, List<K12.Data.SubjectScore>>();
            Dictionary<string, decimal> StudDomainNullScore = new Dictionary<string, decimal>();

            foreach (JHSemesterScoreRecord Scr in JHSemesterScore.SelectByStudentIDs(StudentIDs))
            {
                // 取得領域名稱空白
                List<K12.Data.SubjectScore> ss = (from ss1 in Scr.Subjects where ss1.Value.Domain == "" select ss1.Value).ToList();

                if (ss.Count > 0)
                {

                    if (StudSubjScore.ContainsKey(Scr.RefStudentID))
                        StudSubjScore[Scr.RefStudentID].AddRange(ss);
                    else
                        StudSubjScore.Add(Scr.RefStudentID, ss);
                }
            }

            // 計算彈性課程成績

            foreach (KeyValuePair<string,List<K12.Data.SubjectScore>> data in StudSubjScore)
            {
                decimal sum=0, Credit=0;
                foreach (K12.Data.SubjectScore ss in data.Value)
                {
                    if (ss.Credit.HasValue)
                        Credit += ss.Credit.Value;
                    if (ss.Score.HasValue)
                        sum += ss.Score.Value;                
                }
                if (!StudDomainNullScore.ContainsKey(data.Key))
                {
                    if (Credit > 0)
                    {
                        decimal score = sum / Credit;
                        StudDomainNullScore.Add(data.Key, score);
                    }
                }
            }

            foreach (StudentScore ss in Students)
            { 
                if(StudDomainNullScore.ContainsKey(ss.Id ))
                {
                    //2017/5/9 穎驊修正 ，因應 高雄 [08-05][03] 畢業資格判斷成績及格標準調整 項目，
                    // 領域 分數超過60分 ，以 四捨五入取到小數第二位 ， 低於60分 採用 無條件進位至整數 (EX : 59.01 =60)
                    //decimal score = ss.CalculationRule.ParseDomainScore(StudDomainNullScore[ss.Id]);

                    decimal score = 0;

                    if (JHEvaluation.ScoreCalculation.Program.Mode == JHEvaluation.ScoreCalculation.ModuleMode.HsinChu)
                    {
                        score = ss.CalculationRule.ParseDomainScore(StudDomainNullScore[ss.Id]);
                    }
                    
                    if (JHEvaluation.ScoreCalculation.Program.Mode == JHEvaluation.ScoreCalculation.ModuleMode.KaoHsiung)
                    {
                        if (StudDomainNullScore[ss.Id] >= 60)
                        {
                            score = ss.CalculationRule.ParseDomainScore(StudDomainNullScore[ss.Id]);
                        }
                        else
                        {
                            score = Math.Ceiling(StudDomainNullScore[ss.Id]);
                        }
                    }

                    
                    if (ss.GraduateScore.Contains("彈性課程"))
                    {

                        ss.GraduateScore["彈性課程"].Value = score;
                    }
                    else
                    {
                        GraduateScore gs = new GraduateScore ();
                        gs.Value = score;
                        ss.GraduateScore.Add("彈性課程", gs);
                    }
                }
            
            }


            


            Students.SaveGraduateScore(this);//儲存畢業成績。

            //儲存 Log。
            try
            {
                LogSaver logger = FISCA.LogAgent.ApplicationLog.CreateLogSaverInstance();
                logger.Log("學生畢業成績計算(學生)", "計算學生畢業成績", "計算學生數：" + Students.Count);
                //logger.BatchLogCompleted += delegate(object sender1, EventArgs e1) { };
                //logger.BatchLogFailure += delegate(object sender1, LogErrorEventArgs e1) { };
                foreach (StudentScore each in Students)
                {
                    StringBuilder description = new StringBuilder();
                    description.AppendLine(string.Format("學生：{0}({1})", each.Name, each.StudentNumber));

                    foreach (LogData log in each.GraduateScore.Log)
                        description.Append(log.ToString());

                    description.Append(each.GraduateScore.LearningLog.ToString());
                    description.Append(each.GraduateScore.CourseLog.ToString());

                    logger.AddBatch("學生畢業成績計算", "計算學生畢業成績", "student", each.Id, description.ToString());
                }
                logger.LogBatch();
            }
            catch { }

            Feedback("", 0);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnCalc.Enabled = true;
            MotherForm.SetStatusBarMessage("", 0);

            if (e.Error != null)
            {
                if (e.Error is CalculationException)
                {
                    CalculationException ce = e.Error as CalculationException;
                    new StudentsView<StudentScore>(ce.Students, ce.Message).ShowDialog();
                }
                else
                    MsgBox.Show(e.Error.Message);

                return;
            }

            MsgBox.Show("成績計算完成。");
            Close();
        }

        #region IStatusReporter 成員

        public void Feedback(string message, int percentage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, int>(Feedback), new object[] { message, percentage });
            }
            else
            {
                MotherForm.SetStatusBarMessage(message, percentage);
                Application.DoEvents();
            }
        }

        #endregion
    }
}
