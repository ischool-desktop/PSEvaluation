using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using JHSchool.Data;

namespace JHEvaluation.ExamScoreCopy
{
    internal class Validator
    {
        /// <summary>
        /// 驗證課程
        /// </summary>
        /// <param name="courses"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static bool ValidateCourses(List<JHCourseRecord> courses, UserConfig config)
        {
            MessageView view = new MessageView(MessageItemType.Course);
            view.ButtonVisible = false;
            view.Title = string.Format("{0} 學年度 第 {1} 學期 {2}", config.SchoolYear, config.Semester, config.Exam.Name);

            #region 留下有效的科目課程
            List<JHCourseRecord> validCourses = new List<JHCourseRecord>();
            foreach (JHCourseRecord course in courses)
            {
                if (course.Subject != config.SourceSubject &&
                    !config.TargetSubjects.Contains(course.Subject)) continue;
                validCourses.Add(course);
            }
            #endregion

            List<JHCourseRecord> temp = new List<JHCourseRecord>();

            #region 檢查有沒有評量設定
            foreach (JHCourseRecord course in validCourses)
            {
                if (string.IsNullOrEmpty(course.RefAssessmentSetupID))
                    view.Add(course.ID, course.Name, "沒有評量設定");
                else
                    temp.Add(course);
            }
            validCourses = new List<JHCourseRecord>(temp);
            temp.Clear();
            #endregion

            #region 評量設定必須是一樣的
            List<string> asIDs = new List<string>();
            foreach (JHCourseRecord course in validCourses)
            {
                if (!asIDs.Contains(course.RefAssessmentSetupID))
                    asIDs.Add(course.RefAssessmentSetupID);
            }
            if (asIDs.Count > 1)
            {
                Dictionary<string, JHAssessmentSetupRecord> asDict = new Dictionary<string, JHAssessmentSetupRecord>();
                foreach (JHAssessmentSetupRecord asRecord in JHAssessmentSetup.SelectAll())
                    asDict.Add(asRecord.ID, asRecord);

                foreach (JHCourseRecord course in validCourses)
                    view.Add(course.ID, course.Name, "評量設定為「" + asDict[course.RefAssessmentSetupID].Name + "」，所有科目需要有相同的評量設定");
            }
            #endregion

            if (view.HasMessage)
            {
                view.ShowDialog();
                return false;
            }

            #region 評量設定裡沒有選擇的試別
            if (asIDs.Count == 1)
            {
                bool found = false;
                foreach (JHAEIncludeRecord ae in JHAEInclude.SelectByAssessmentSetupID(asIDs[0]))
                {
                    if (ae.RefExamID == config.Exam.ID)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    JHAssessmentSetupRecord asRecord = JHAssessmentSetup.SelectByID(asIDs[0]);
                    foreach (JHCourseRecord course in validCourses)
                        view.Add(course.ID, course.Name, "評量設定「" + asRecord.Name + "」，沒有「" + config.Exam.Name + "」試別");
                }
            }
            #endregion

            if (view.HasMessage)
            {
                view.ShowDialog();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 驗證修課記錄
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static bool ValidateStudentAttends(UserConfig config)
        {
            MessageView view = new MessageView(MessageItemType.Student);
            view.ButtonText = "略過並繼續";
            view.Title = string.Format("{0} 學年度 第 {1} 學期 {2}", config.SchoolYear, config.Semester, config.Exam.Name);

            List<string> ignoreIDs = new List<string>();

            foreach (StudentData sd in AttendManager.Instance.GetStudentDataList())
            {
                string info = GetStudentInfo(sd.Student);
                if (sd.Attends.SubjectExists(config.SourceSubject) == false)
                {
                    view.Add(sd.Student.ID, info, "沒有修習來源科目「" + config.SourceSubject + "」的課程");
                    ignoreIDs.Add(sd.Student.ID);
                }
                foreach (string subject in config.TargetSubjects)
                {
                    if (sd.Attends.SubjectExists(subject) == false)
                    {
                        view.Add(sd.Student.ID, info, "沒有修習目的科目「" + subject + "」的課程");
                        ignoreIDs.Add(sd.Student.ID);
                    }
                }
            }

            if (view.HasMessage)
            {
                if (view.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    AttendManager.Instance.DeleteIgnoreStudents(ignoreIDs);
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 驗證評量成績
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static bool ValidateStudentScores(UserConfig config)
        {
            MessageView view = new MessageView(MessageItemType.Student);
            view.ButtonText = "進行複製";
            view.Title = string.Format("{0} 學年度 第 {1} 學期 {2}", config.SchoolYear, config.Semester, config.Exam.Name);

            List<string> ignoreIDs = new List<string>();

            foreach (StudentData sd in AttendManager.Instance.GetStudentDataList())
            {
                string info = GetStudentInfo(sd.Student);
                if (sd.Scores.SubjectExists(config.SourceSubject) == false)
                {
                    view.Add(sd.Student.ID, info, "沒有來源科目「" + config.SourceSubject + "」的評量成績，不進行複製", MessageLevel.Error);
                    ignoreIDs.Add(sd.Student.ID);
                }
                else
                {
                    foreach (string subject in config.TargetSubjects)
                    {
                        if (sd.Scores.SubjectExists(subject) == true
                            && (sd.Scores[subject].Effort != null || sd.Scores[subject].Score != null))
                            view.Add(sd.Student.ID, info, "目的科目「" + subject + "」已有評量成績，成績將會被覆蓋", MessageLevel.Warning);
                        else
                            view.Add(sd.Student.ID, info, "新增科目「" + subject + "」的評量成績");
                    }
                }
            }

            if (view.HasMessage)
            {
                AttendManager.Instance.DeleteIgnoreStudents(ignoreIDs);
                if (AttendManager.Instance.GetStudentDataList().Count <= 0)
                    view.ButtonVisible = false;

                if (view.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return true;
            }

            return false;
        }

        private static string GetStudentInfo(JHStudentRecord student)
        {
            string s = "";
            if (student.Class != null) s += student.Class.Name;
            if (!string.IsNullOrEmpty("" + student.SeatNo)) s += " " + student.SeatNo + "號";
            if (!string.IsNullOrEmpty(student.StudentNumber)) s += " (" + student.StudentNumber + ")";
            s += " " + student.Name;
            return s;
        }
    }
}
