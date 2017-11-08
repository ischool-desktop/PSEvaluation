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
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JHEvaluation.ScoreCalculation
{
    public partial class SubjectScoreCalculate : BaseForm, IStatusReporter
    {
        private List<string> StudentIDs;
        private int SchoolYear { get; set; }
        private int Semester { get; set; }

        private BackgroundWorker _BackgroundWorker = new BackgroundWorker();

        public SubjectScoreCalculate(List<string> studentIds)
        {
            InitializeComponent();
            StudentIDs = studentIds;

            _BackgroundWorker.WorkerReportsProgress = true;
            _BackgroundWorker.ProgressChanged += _BackgroundWorker_ProgressChanged;
            _BackgroundWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
            _BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void _BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }

        private void SubjectScoreCalculate_Load(object sender, EventArgs e)
        {
            Util.SetSemesterDefaultItems(intSchoolYear, intSemester);
            StudentScore.SetClassMapping();
        }

        private bool RecalculateAll = false;

        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (_BackgroundWorker.IsBusy) return;

            // 因為成績已經處理所以移除
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

            // 103-1 與 之後的定期、平時計算比例不同。
            if (Program.Mode == ModuleMode.KaoHsiung)
            {
                //因為評量計算比例調整，提示使用者。
                SemesterData denySemesterData = new SemesterData(0, 102, 2);
                SemesterData selectedSemesterData = new SemesterData(0, intSchoolYear.Value, intSemester.Value);
                if (selectedSemesterData <= denySemesterData)
                {
                    //MsgBox.Show("因評量計算比例在  103-1 之後有所調整，故暫不開放重新計算 102-2(含) 之前的成績。若須重新計算，請與我們聯絡。");
                    //return;

                    if (MsgBox.Show("103-1以前的學年度學期將採計50:50評量計算比例(現行制度為60:40),確認繼續?", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }

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
            _BackgroundWorker.RunWorkerAsync();
            Close();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<StudentScore> Students = StudentIDs.ToStudentScore();

            List<StudentScore> noValid = Students.ReadCalculationRule(this); //沒有計算規則的學生。

            if (noValid.Count > 0)
                throw new CalculationException(noValid, "下列學生沒有計算規則，無法計算成績。");


            // 2016/ 6/6 穎驊製作，恩正說從這邊先於Students.ReadAttendScore 攔科目名稱重覆，否則進去攔的資料太複雜，
            #region 檢查科目重複問題

            Feedback("檢查學生修課內容", 0);
            List<JHSCAttendRecord> scAttendList = JHSCAttend.SelectByStudentIDs(StudentIDs);

            Dictionary<string, List<JHSCAttendRecord>> scAttendCheck = new Dictionary<string, List<JHSchool.Data.JHSCAttendRecord>>();
            // 穎驊重做，檢查重覆修習科目
            Dictionary<string, List<JHSCAttendRecord>> duplicateErrorCourse = new Dictionary<string, List<JHSCAttendRecord>>();

            //把課程讀下來快取，穎驊筆記: 這一行超級重要，如果沒有的話至少多100倍的時間下面Foreach 才能算完，不誇張
            JHSchool.Data.JHCourse.SelectAll();

            //List<string> ErrorMessages = new List<string>();
            int count = 0;
            foreach (JHSCAttendRecord scAttendRec in scAttendList)
            {
                Feedback("檢查學生修課內容", ++count * 100 / scAttendList.Count);

                if (scAttendRec.Course.SchoolYear == SchoolYear && scAttendRec.Course.Semester == Semester && scAttendRec.Course.CalculationFlag == "1")
                {
                    String duplicateErrorCourseKey = scAttendRec.Student.ID + "_" + scAttendRec.Course.Subject;

                    if (!scAttendCheck.ContainsKey(duplicateErrorCourseKey))
                    {
                        scAttendCheck.Add(duplicateErrorCourseKey, new List<JHSchool.Data.JHSCAttendRecord>());
                    }
                    else
                    {
                        //重複了
                        if (!duplicateErrorCourse.ContainsKey(duplicateErrorCourseKey))
                            duplicateErrorCourse.Add(duplicateErrorCourseKey, scAttendCheck[duplicateErrorCourseKey]);
                    }
                    scAttendCheck[duplicateErrorCourseKey].Add(scAttendRec);
                }
            }


            if (duplicateErrorCourse.Count > 0)
            {
                throw new DuplicatedSubjectException(new List<List<JHSCAttendRecord>>(duplicateErrorCourse.Values));
            }
            #endregion

            Students.ReadAttendScore(SchoolYear, Semester, new string[] { }, this);
            noValid = Students.CalcuateAttendScore(this); //回傳沒有修課的學生。
            Students.SaveAttendScore(new string[] { }, this);
            Students.ReadSemesterScore(SchoolYear, Semester, this);

            if (RecalculateAll) Students.ClearSubjectScore(SemesterData.Empty);

            Students.CalcuateSubjectSemesterScore(new string[] { });
            Students.SaveSemesterScore(this);

            //儲存 Log。
            try
            {
                LogSaver logger = FISCA.LogAgent.ApplicationLog.CreateLogSaverInstance();
                logger.Log("學生學期科目成績計算(學生)", "計算學生學期科目成績", "計算學生數：" + Students.Count);
                //logger.BatchLogCompleted += delegate(object sender1, EventArgs e1) { };
                //logger.BatchLogFailure += delegate(object sender1, LogErrorEventArgs e1) { };
                foreach (StudentScore each in Students)
                {
                    StringBuilder description = new StringBuilder();
                    description.AppendLine(string.Format("學生：{0}({1})", each.Name, each.StudentNumber));
                    description.AppendLine(string.Format("學年度：{0} 學期：{1}", SchoolYear, Semester));

                    foreach (LogData log in each.SemestersScore[SemesterData.Empty].Subject.Log)
                        description.Append(log.ToString());

                    logger.AddBatch("學生學期科目成績計算(學生)", "計算學生學期科目成績", "student", each.Id, description.ToString());
                }
                logger.LogBatch();
            }
            catch { }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error is CalculationException)
                {
                    CalculationException ce = e.Error as CalculationException;
                    new StudentsView<StudentScore>(ce.Students, ce.Message).ShowDialog();
                }

                //2016/6/6 穎驊新增DuplicatedSubjectException，用來處理學生在同學期有同樣科目問題。
                if (e.Error is DuplicatedSubjectException)
                {
                    DuplicatedSubjectException dse = e.Error as DuplicatedSubjectException;
                    //new StudentsView<StudentScore>(dse.Students, dse.Message).ShowDialog();
                    var dialog = new DuplicatedSubject();
                    dialog.SetList(dse.DuplicatedList);
                    dialog.ShowDialog();
                }
                else
                {
                    MotherForm.SetStatusBarMessage("");
                    MsgBox.Show(e.Error.Message);
                }
            }
            else
            {
                MotherForm.SetStatusBarMessage("成績計算完成。");
                MsgBox.Show("成績計算完成。");
            }
        }

        #region IStatusReporter 成員

        public void Feedback(string message, int percentage)
        {
            _BackgroundWorker.ReportProgress(percentage, message);
            //if (InvokeRequired)
            //{
            //    Invoke(new Action<string, int>(Feedback), new object[] { message, percentage });
            //}
            //else
            //{
            //    MotherForm.SetStatusBarMessage(message, percentage);
            //    Application.DoEvents();
            //}
        }
        #endregion


    }
}
