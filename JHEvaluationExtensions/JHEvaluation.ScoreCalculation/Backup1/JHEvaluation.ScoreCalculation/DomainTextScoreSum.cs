using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using FISCA.LogAgent;
using FISCA.Presentation;
using FISCA.Presentation.Controls;

namespace JHEvaluation.ScoreCalculation
{
    /// <summary>
    /// 依選取學生及學年度學期計算學習領域成績
    /// </summary>
    public partial class DomainTextScoreSum : BaseForm, IStatusReporter
    {
        /// <summary>
        /// 學生編號列表
        /// </summary>
        private List<string> StudentIDs;
        /// <summary>
        /// 學年度
        /// </summary>
        private int SchoolYear { get; set; }
        /// <summary>
        /// 學期
        /// </summary>
        private int Semester { get; set; }

        /// <summary>
        /// 建構式，傳入學生編號列表
        /// </summary>
        /// <param name="studentIds"></param>
        public DomainTextScoreSum(List<string> studentIds)
        {
            InitializeComponent();
            StudentIDs = studentIds;
        }

        private void DomainScoreCalculate_Load(object sender, EventArgs e)
        {
            Util.SetSemesterDefaultItems(intSchoolYear, intSemester);
            StudentScore.SetClassMapping();
        }

        private bool RecalculateAll = false;

        private void btnCalc_Click(object sender, EventArgs e)
        {
            DialogResult dr = MsgBox.Show("您確定要加總學生學期學習領域文字描述？", MessageBoxButtons.YesNo);

            if (dr == DialogResult.No) return;

            //if (Control.ModifierKeys == Keys.Shift)
            //{
            //    string msg = "您確定要刪掉本學期所有學習領域成績重算？";
            //    if (MsgBox.Show(msg, "密技", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            //        RecalculateAll = true;
            //}
            //else
            //    RecalculateAll = false;

            SchoolYear = intSchoolYear.Value;
            Semester = intSemester.Value;

            btnCalc.Enabled = false;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<StudentScore> Students = StudentIDs.ToStudentScore();

            //List<StudentScore> noValid = Students.ReadCalculationRule(this); //沒有計算規則的學生。

            //if (noValid.Count > 0)
            //    throw new CalculationException(noValid, "下列學生沒有計算規則，無法計算成績。");

            Students.ReadSemesterScore(SchoolYear, Semester, this);

            //if (RecalculateAll) Students.ClearLearningDomainScore(SemesterData.Empty);

            Students.SumDomainTextScore(new string[] { });

            Students.SaveSemesterScore(this);

            //儲存 Log。
            try
            {
                LogSaver logger = FISCA.LogAgent.ApplicationLog.CreateLogSaverInstance();
                logger.Log("學生學期領域文字描述加總(學生)", "加總學生學期學習領域文字描述", "計算學生數：" + Students.Count);
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

                    logger.AddBatch("學生學期領域文字描述加總(學生)", "加總學生學期學習領域文字描述", "student", each.Id, description.ToString());
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

            MsgBox.Show("文字描述加總完成。");
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