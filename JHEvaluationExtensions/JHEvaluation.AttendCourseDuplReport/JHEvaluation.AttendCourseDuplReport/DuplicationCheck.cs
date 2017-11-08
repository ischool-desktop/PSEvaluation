using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using System.ComponentModel;
using FISCA.Presentation.Controls;

namespace JHEvaluation.AttendCourseDuplReport
{
    internal class DuplicationCheck
    {
        /// <summary>
        /// 要檢查的學生
        /// </summary>
        private List<JHStudentRecord> Students { get; set; }

        private int SchoolYear { get; set; }
        private int Semester { get; set; }

        private BackgroundWorker Worker { get; set; }

        public DuplicationCheck(List<JHStudentRecord> students, int schoolYear, int semester)
        {
            Students = students;
            SchoolYear = schoolYear;
            Semester = semester;

            Worker = new BackgroundWorker();
            Worker.WorkerReportsProgress = true;
            Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
        }

        public void Run()
        {
            Worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            StudentAttendInfo info = new StudentAttendInfo();

            Worker.ReportProgress(0);
            //快取指定學期的課程清單，查詢用。
            Dictionary<string, JHCourseRecord> courseDict = new Dictionary<string, JHCourseRecord>();
            foreach (JHCourseRecord course in JHCourse.SelectBySchoolYearAndSemester(SchoolYear, Semester))
                courseDict.Add(course.ID, course);

            Worker.ReportProgress(30);
            foreach (JHSCAttendRecord sc in JHSCAttend.SelectByStudentIDs(Students.AsKeyList()))
            {
                //修習的課程必須是指定學期的課程
                if (courseDict.ContainsKey(sc.RefCourseID))
                    info.Add(sc.RefStudentID, courseDict[sc.RefCourseID]);
            }

            Worker.ReportProgress(80);
            info.RemoveRegular();
            Worker.ReportProgress(100);
            e.Result = info;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MsgBox.Show("檢查過程中發生錯誤。" + e.Error.Message);
                FISCA.Presentation.MotherForm.SetStatusBarMessage("檢查失敗");
                return;
            }

            FISCA.Presentation.MotherForm.SetStatusBarMessage("檢查重覆修課完成");

            StudentAttendInfo info = e.Result as StudentAttendInfo;

            CheckForm checkForm = new CheckForm(info, SchoolYear, Semester);
            checkForm.ShowDialog();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("檢查重覆修課中...", e.ProgressPercentage);
            //FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }
    }

    internal static class DuplicationCheckExtension
    {
        public static List<string> AsKeyList(this List<JHStudentRecord> students)
        {
            var list = from s in students select s.ID;
            return list.ToList<string>();
        }
    }
}