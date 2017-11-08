using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Presentation;
using JHSchool.Data;
using FISCA.LogAgent;

namespace JHEvaluation.ScoreCalculation
{
    public partial class DomainScoreCalculateByGradeyear : BaseForm, IStatusReporter
    {
        private List<string> StudentIDs;
        private int SchoolYear { get; set; }
        private int Semester { get; set; }

        //private bool _ClearDomainScore;
        private DomainScoreSetting _domainScoreSetting;

        public DomainScoreCalculateByGradeyear()
        {
            InitializeComponent();
            _domainScoreSetting = new DomainScoreSetting(false, false);
        }

        private void DomainScoreCalculateByGradeyear_Load(object sender, EventArgs e)
        {
            Util.SetSemesterDefaultItems(intSchoolYear, intSemester);

            StudentScore.SetClassMapping(); //裡面會對 Class 做 SelectAll 動作。

            int min = 9, max = 1;
            foreach (JHClassRecord each in JHClass.SelectAll())
            {
                if (!each.GradeYear.HasValue) continue;

                min = Math.Min(each.GradeYear.Value, min);
                max = Math.Max(each.GradeYear.Value, max);
            }
            intGradeyear.MinValue = min;
            intGradeyear.MaxValue = max;
            intGradeyear.Value = min;
        }

        private bool RecalculateAll = false;

        private void btnCalc_Click(object sender, EventArgs e)
        {
            //_ClearDomainScore = chkClearDomainScore.Checked;
                        
            //_domainScoreSetting.DomainScoreClear = chkClearDomainScore.Checked;

            //2017/6/19  穎驊修改，因應 [02-03][02] 轉入生語文領域成績問題 項目， 將原"刪除全部領域成績並重算" 的功能隱藏，把原本setting 註解後，現在預設皆為false
            //而若欲一次批次大量修改刪除領域成績，在本次[02-02][06] 計算學期科目成績新增清空原成績模式 項目，也已增加 "刪除"欄位， 提供使用者已匯入的方式，刪除既有領域成績
            _domainScoreSetting.DomainScoreClear = false;

            _domainScoreSetting.DomainScoreLimit = chkScoreLimite.Checked;

            //if (_ClearDomainScore)
            if (_domainScoreSetting.DomainScoreClear)
            {
                if (MsgBox.Show("確認要進行全部刪除並重新計算?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            DialogResult dr = MsgBox.Show("您確定要計算學生學期領域成績？", MessageBoxButtons.YesNo);

            if (dr == DialogResult.No) return;

            if (Control.ModifierKeys == Keys.Shift)
            {
                string msg = "您確定要刪掉本學期所有領域成績重算？";
                if (MsgBox.Show(msg, "密技", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    RecalculateAll = true;
            }
            else
                RecalculateAll = false;

            SchoolYear = intSchoolYear.Value;
            Semester = intSemester.Value;

            btnCalc.Enabled = false;

            //取得指定年級的所有學生。
            StudentIDs = Util.GetGradeyearStudents(intGradeyear.Value);

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
                throw new CalculationException(noValid, "下列學生沒有計算規則，無法計算成績。");

            Students.ReadSemesterScore(SchoolYear, Semester, this);

            if (RecalculateAll) Students.ClearDomainScore(SemesterData.Empty);

            //Students.CalcuateDomainSemesterScore(new string[] { }, _ClearDomainScore);
            Students.CalcuateDomainSemesterScore(new string[] { }, _domainScoreSetting);
            Students.SaveSemesterScore(this);

            //儲存 Log。
            try
            {
                LogSaver logger = FISCA.LogAgent.ApplicationLog.CreateLogSaverInstance();
                logger.Log("學生學期領域成績計算(教務作業)", "計算學生學期領域成績", "計算學生數：" + Students.Count);
                //logger.BatchLogCompleted += delegate(object sender1, EventArgs e1) { };
                //logger.BatchLogFailure += delegate(object sender1, LogErrorEventArgs e1) { };
                foreach (StudentScore each in Students)
                {
                    StringBuilder description = new StringBuilder();
                    description.AppendLine(string.Format("學生：{0}({1})", each.Name, each.StudentNumber));
                    description.AppendLine(string.Format("學年度：{0} 學期：{1}", SchoolYear, Semester));

                    foreach (LogData log in each.SemestersScore[SemesterData.Empty].Domain.Log)
                        description.Append(log.ToString());

                    description.Append(each.SemestersScore[SemesterData.Empty].LearningLog.ToString());
                    description.Append(each.SemestersScore[SemesterData.Empty].CourseLog.ToString());

                    logger.AddBatch("學生學期領域成績計算(教務作業)", "計算學生學期領域成績", "student", each.Id, description.ToString());
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
