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

namespace JHEvaluation.ScoreCalculation
{
    public partial class SubjectScoreCalculateByGradeyear : BaseForm, IStatusReporter
    {
        private List<string> StudentIDs;
        private int SchoolYear { get; set; }
        private int Semester { get; set; }

        public SubjectScoreCalculateByGradeyear()
        {
            InitializeComponent();
        }

        private void SubjectScoreCalculate_Load(object sender, EventArgs e)
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
            //if (Program.Mode == ModuleMode.KaoHsiung)
            //{
            //    //因為平時評量儲存結構調整，98-1以前的科目成績不給算(包含98-1)
            //    SemesterData denySemesterData = new SemesterData(0, 98, 1);
            //    SemesterData selectedSemesterData = new SemesterData(0, intSchoolYear.Value, intSemester.Value);
            //    if (selectedSemesterData <= denySemesterData)
            //    {
            //        MsgBox.Show("因評量設定調整後尚未完成981「平時評量」及「文字評量」資料處理，故暫不開放981科目成績計算功能。\n若須重新計算，請與我們聯絡。");
            //        return;
            //    }
            //}

            DialogResult dr = MsgBox.Show("您確定要計算學生學期科目成績？", MessageBoxButtons.YesNo);

            if (dr == DialogResult.No) return;

            if (Control.ModifierKeys == Keys.Shift)
            {
                string msg = "您確定要刪掉本學期所有科目成績重算？";
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

            Students.ReadAttendScore(SchoolYear, Semester, new string[] { }, this);
            List<StudentScore> noCourses = Students.CalcuateAttendScore(this); //回傳沒有修課的學生。
            Students.SaveAttendScore(new string[] { }, this);
            Students.ReadSemesterScore(SchoolYear, Semester, this);

            if (RecalculateAll) Students.ClearSubjectScore(SemesterData.Empty);

            Students.CalcuateSubjectSemesterScore(new string[] { });
            Students.SaveSemesterScore(this);

            //儲存 Log。
            try
            {
                LogSaver logger = FISCA.LogAgent.ApplicationLog.CreateLogSaverInstance();
                logger.Log("學生學期科目成績計算(教務作業)", "計算學生學期科目成績", "計算學生數：" + Students.Count);
                //logger.BatchLogCompleted += delegate(object sender1, EventArgs e1) { };
                //logger.BatchLogFailure += delegate(object sender1, LogErrorEventArgs e1) { };
                foreach (StudentScore each in Students)
                {
                    StringBuilder description = new StringBuilder();
                    description.AppendLine(string.Format("學生：{0}({1})", each.Name, each.StudentNumber));
                    description.AppendLine(string.Format("學年度：{0} 學期：{1}", SchoolYear, Semester));

                    foreach (LogData log in each.SemestersScore[SemesterData.Empty].Subject.Log)
                        description.Append(log.ToString());

                    logger.AddBatch("學生學期科目成績計算(教務作業)", "計算學生學期科目成績", "student", each.Id, description.ToString());
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
