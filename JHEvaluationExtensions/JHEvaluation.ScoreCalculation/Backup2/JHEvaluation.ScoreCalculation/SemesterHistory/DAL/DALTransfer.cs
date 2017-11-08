using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace JHEvaluation.ScoreCalculation.SemesterHistory.DAL
{
    class DALTransfer
    {
        /// <summary>
        /// 取得年級的上課天數
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, int> GetInSchoolDayByGardeYear()
        {
            Dictionary<int, int> _InSchoolDay = new Dictionary<int, int>();
            K12.Data.SchoolHolidayRecord shr = K12.Data.SchoolHoliday.SelectSchoolHolidayRecord();
            if (shr != null)
            {
                if (shr.SchoolDayCountG1 != null)
                {
                    _InSchoolDay.Add(1, shr.SchoolDayCountG1);
                    _InSchoolDay.Add(7, shr.SchoolDayCountG1);
                }

                if (shr.SchoolDayCountG2 != null)
                {
                    _InSchoolDay.Add(2, shr.SchoolDayCountG2);
                    _InSchoolDay.Add(8, shr.SchoolDayCountG2);
                }
                if (shr.SchoolDayCountG3 != null)
                {
                    _InSchoolDay.Add(3, shr.SchoolDayCountG3);
                    _InSchoolDay.Add(9, shr.SchoolDayCountG3);
                }
            }
            return _InSchoolDay;
        }

        /// <summary>
        /// 讀取學習歷程項目
        /// </summary>
        /// <param name="SchoolYear"></param>
        /// <param name="Semester"></param>
        /// <param name="StudentIDList"></param>
        /// <returns></returns>
        public static List<SemesterHistoryItemEntity> GetSemesterHistoryItemEntityList(int SchoolYear, int Semester, List<string> StudentIDList)
        {
            Dictionary<int, int> SchoolDays = GetInSchoolDayByGardeYear();
            List<SemesterHistoryItemEntity> SemesterHistoryItemEntityList = new List<SemesterHistoryItemEntity>();
            Dictionary<string, JHSemesterHistoryRecord> studSemesterHistoryRecordDic = new Dictionary<string, JHSemesterHistoryRecord>();
            foreach (JHSemesterHistoryRecord shr in JHSemesterHistory.SelectByStudentIDs(StudentIDList))
                studSemesterHistoryRecordDic.Add(shr.RefStudentID, shr);

            
            
            foreach (JHStudentRecord studRec in JHStudent.SelectByIDs(StudentIDList))
            {
                SemesterHistoryItemEntity shie = new SemesterHistoryItemEntity();

                // 預設
                shie.HasSemsterHistoryRec = false;
                shie.EditMode = SemesterHistoryItemEntity._EditMode.Insert;
                shie.Schoolyear = SchoolYear;
                shie.Semester = Semester;
                shie.StudentNumber = studRec.StudentNumber;
                shie.Name = studRec.Name;

                if (studSemesterHistoryRecordDic.ContainsKey(studRec.ID ))
                {
                    if (studSemesterHistoryRecordDic[studRec.ID].SemesterHistoryItems.Count > 0)
                    {
                        shie.HasSemsterHistoryRec = true;
                        shie.EditMode = SemesterHistoryItemEntity._EditMode.Insert;

                        foreach (K12.Data.SemesterHistoryItem shi in studSemesterHistoryRecordDic[studRec.ID].SemesterHistoryItems)
                        {
                            if (shi.SchoolYear == SchoolYear && shi.Semester == Semester)
                                shie.EditMode = SemesterHistoryItemEntity._EditMode.Update;
                        }
                    }                    
                }
                // 座號
                if (studRec.SeatNo.HasValue)
                    shie.SeatNo = studRec.SeatNo.Value;

                shie.StudentID = studRec.ID;
                if (studRec.Class != null)
                {
                    if (studRec.Class.GradeYear.HasValue)
                    {
                        //年級
                        shie.GradeYear = studRec.Class.GradeYear.Value;
                        if (SchoolDays.ContainsKey(shie.GradeYear))
                        {
                            // 上課天數
                            shie.SchoolDayCount = SchoolDays[shie.GradeYear];
                        }
                    }
                    // 班及名稱
                    shie.ClassName = studRec.Class.Name;

                    // 班導師
                    if(studRec.Class.Teacher != null )
                        shie.TeacherName = studRec.Class.Teacher.Name;
                }
                SemesterHistoryItemEntityList.Add(shie);
            }            

            return SemesterHistoryItemEntityList;
        }

        /// <summary>
        /// 儲存學習歷程項目
        /// </summary>
        /// <param name="SemesterHistoryItemEntityList"></param>
        public static void SetSemesterHistoryItemEntityList(List<SemesterHistoryItemEntity> SemesterHistoryItemEntityList, List<string> StudntIDList)
        {
            Dictionary<string, JHSemesterHistoryRecord> JHSemesterHistoryRecordDic = new Dictionary<string, JHSemesterHistoryRecord>();

            foreach (JHSemesterHistoryRecord shr in JHSemesterHistory.SelectByStudentIDs(StudntIDList))
                JHSemesterHistoryRecordDic.Add(shr.RefStudentID, shr);

            List<JHSemesterHistoryRecord> UpadteRecList = new List<JHSemesterHistoryRecord>();

            foreach (SemesterHistoryItemEntity shie in SemesterHistoryItemEntityList)
            {
                K12.Data.SemesterHistoryItem shi = new K12.Data.SemesterHistoryItem();
                shi.ClassName = shie.ClassName;
                shi.GradeYear = shie.GradeYear;
                shi.SchoolDayCount = shie.SchoolDayCount;
                shi.SchoolYear = shie.Schoolyear;
                shi.SeatNo = shie.SeatNo;
                shi.Semester = shie.Semester;
                shi.Teacher = shie.TeacherName;                


                if (JHSemesterHistoryRecordDic.ContainsKey(shie.StudentID))
                {                    
                    // 完全沒學習歷程
                    if (shie.HasSemsterHistoryRec == false && shie.EditMode == SemesterHistoryItemEntity._EditMode.Insert)
                    {
                        JHSemesterHistoryRecordDic[shie.StudentID].SemesterHistoryItems.Add(shi);
                    }

                    // 有學習歷程
                    if (shie.HasSemsterHistoryRec)
                    {
                        K12.Data.SemesterHistoryItem rmItem=null;
                        foreach (K12.Data.SemesterHistoryItem sh in JHSemesterHistoryRecordDic[shie.StudentID].SemesterHistoryItems)
                        {
                            // 檢查當有同學年度學期移除
                            if (sh.SchoolYear == shi.SchoolYear && sh.Semester == shi.Semester)
                            {
                                rmItem = sh;
                            }
                        }

                        // 先移除舊的
                        if (rmItem != null)
                            JHSemesterHistoryRecordDic[shie.StudentID].SemesterHistoryItems.Remove(rmItem);


                        if (shie.EditMode == SemesterHistoryItemEntity._EditMode.Insert)
                        {
                            JHSemesterHistoryRecordDic[shie.StudentID].SemesterHistoryItems.Add(shi);
                        }

                        if (shie.EditMode == SemesterHistoryItemEntity._EditMode.Update)
                        {
                            JHSemesterHistoryRecordDic[shie.StudentID].SemesterHistoryItems.Add(shi);
                        }


                        if (shie.EditMode == SemesterHistoryItemEntity._EditMode.Delete)
                        {


                        }
                    }
                    UpadteRecList.Add(JHSemesterHistoryRecordDic[shie.StudentID]);
                }
            }
            JHSemesterHistory.Update(UpadteRecList);
        }

        /// <summary>
        /// 取得一般或輟學學生(有班級與年級)
        /// </summary>
        /// <returns></returns>
        public static List<JHStudentRecord> GetJHStudentRecordListBy1()
        {
            List<JHStudentRecord> studRecList = new List<JHStudentRecord>();
            foreach (JHStudentRecord studRec in JHStudent.SelectAll())
            {
                if (studRec.Status == K12.Data.StudentRecord.StudentStatus.一般 || studRec.Status == K12.Data.StudentRecord.StudentStatus.輟學)
                    if (studRec.Class != null)
                        if (studRec.Class.GradeYear.HasValue)
                            studRecList.Add(studRec);
            }
            return studRecList;
        }
    }
}
