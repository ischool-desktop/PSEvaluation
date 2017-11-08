using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using JHEvaluation.ScoreCalculation;
using JHSchool.Data;
using K12.Data;

namespace JointAdmissionModule.StudentScoreSummaryReport_HsinChu
{
    internal class ReportStudent : StudentScore, Campus.Rating.IStudent
    {
        public ReportStudent(JHStudentRecord student)
            : base(student)
        {
            Places = new Campus.Rating.PlaceCollection();
            HeaderList = new ReportHeaderList();
            Gender = student.Gender;
            Birthday = student.Birthday.HasValue ? student.Birthday.Value.ToString("yyyy/MM/dd") : "";
            Summaries = new Dictionary<SemesterData, XmlElement>();
            EntranceDate = string.Empty;
            GraduateDate = string.Empty;
            EnglishName = student.EnglishName;
            IDNumber = student.IDNumber;
            StudentID = student.ID;
        }

        public string Gender { get; private set; }

        public string Birthday { get; private set; }

        public string EnglishName { get; private set; }

        public string IDNumber { get; private set; }

        /// <summary>
        /// 學生特種身分類別名稱
        /// </summary>
        public string SpcStudTypeName { get; set; }

        /// <summary>
        /// 轉入異動日期(最新筆)
        /// </summary>
        public string TransUpdateDateStr { get; set; }

        public Image GraduatePhoto { get; set; }

        /// <summary>
        /// 學期的 Header Index。
        /// </summary>
        public ReportHeaderList HeaderList { get; private set; }

        #region IStudent 成員

        public Campus.Rating.PlaceCollection Places { get; private set; }

        #endregion

        public Dictionary<SemesterData, XmlElement> Summaries { get; private set; }

        /// <summary>
        /// 學生入學日期(民國格式)。
        /// </summary>
        public string EntranceDate { get; set; }

        /// <summary>
        /// 學生入學日期(西元格式)。
        /// </summary>
        public string EngEntranceDate { get; set; }

        /// <summary>
        /// 學生入學日期(民國格式)。
        /// </summary>
        public string GraduateDate { get; set; }

        /// <summary>
        /// 學生入學日期(西元格式)。
        /// </summary>
        public string EngGraduateDate { get; set; }

        /// <summary>
        /// 最後一筆轉入異動學年度,假設沒有轉入異動,null
        /// </summary>
        public int? LastEnterSchoolyear { get; set; }
        /// <summary>
        /// 最後一筆轉入異動學期,假設沒有轉入異動,null
        /// </summary>
        public int? LastEnterSemester { get; set; }

        /// <summary>
        /// 最後一筆轉入異動年級,假設沒有轉入異動,null
        /// </summary>
        public int? LastEnterGradeYear { get; set; }

        /// <summary>
        /// 加分比重(當特種身分學生使用),一般Null
        /// </summary>
        public decimal? AddWeight { get; set; }

        /// <summary>
        /// 學生ID
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 八上學期領域成績
        /// </summary>
        public decimal LearnDomainScore81 { get; set; }

        /// <summary>
        /// 八上學期領域成績（核心）
        /// </summary>
        public decimal CoreLearnDomainScore81 { get; set; }

        /// <summary>
        /// 八上學期領域成績（核心），特殊生
        /// </summary>
        public decimal CoreLearnDomainScore81Add { get; set; }

        /// <summary>
        /// 八下學期領域成績
        /// </summary>
        public decimal LearnDomainScore82 { get; set; }

        /// <summary>
        /// 八下學期領域成績（核心）
        /// </summary>
        public decimal CoreLearnDomainScore82 { get; set; }

        /// <summary>
        /// 八下學期領域成績（核心），特殊生
        /// </summary>
        public decimal CoreLearnDomainScore82Add { get; set; }

        /// <summary>
        /// 九上學期領域成績
        /// </summary>
        public decimal LearnDomainScore91 { get; set; }

        /// <summary>
        /// 九上學期領域成績（核心）
        /// </summary>
        public decimal CoreLearnDomainScore91 { get; set; }

        /// <summary>
        /// 九上學期領域成績（核心），特殊生
        /// </summary>
        public decimal CoreLearnDomainScore91Add { get; set; }
    }

    internal class ReportHeaderList : Dictionary<SemesterData, int>
    {
        private Dictionary<SemesterData, SemesterData> RawList = new Dictionary<SemesterData, SemesterData>(10);

        public ReportHeaderList()
        {
        }

        /// <summary>
        /// 包含學年度學期年級的 SemesterData。
        /// </summary>
        /// <param name="semester"></param>
        /// <param name="count"></param>
        public void AddRaw(SemesterData semester, int count)
        {
            SemesterData sd = new SemesterData(0, semester.SchoolYear, semester.Semester);

            RawList.Add(sd, semester); //原始的 SemesterData。
            Add(sd, count);
        }

        /// <summary>
        /// 用「學年度、學期」取得原始的 SemesterData。
        /// </summary>
        /// <param name="semester"></param>
        /// <returns></returns>
        public SemesterData GetSRaw(SemesterData semester)
        {
            SemesterData sd;

            if (RawList.TryGetValue(semester, out sd))
                return sd;
            else
                return SemesterData.Empty;
        }
    }

    internal static class StudentScore_Extens
    {
        #region ReadUpdateRecordDate
        public static void ReadUpdateRecordDate(this IEnumerable<ReportStudent> students, IStatusReporter reporter)
        {
            int t1 = Environment.TickCount;
            Dictionary<string, ReportStudent> dicstudents = students.ToDictionary();
            List<string> keys = students.ToSC().ToKeys();

            Campus.FunctionSpliter<string, JHUpdateRecordRecord> selectData = new Campus.FunctionSpliter<string, JHUpdateRecordRecord>(500, 5);
            selectData.Function = delegate(List<string> ps)
            {
                return JHUpdateRecord.SelectByStudentIDs(ps);
            };
            List<JHUpdateRecordRecord> updaterecords = selectData.Execute(keys);

            Dictionary<string, List<JHUpdateRecordRecord>> dicupdaterecords = new Dictionary<string, List<JHUpdateRecordRecord>>();
            string ValidCodes = "1:2";  //新生:1 轉入:3 復學:6，畢業:2
            foreach (JHUpdateRecordRecord each in updaterecords)
            {
                //不是要處理的代碼，就跳過。
                if (ValidCodes.IndexOf(each.UpdateCode) < 0) continue;

                if (!dicupdaterecords.ContainsKey(each.StudentID))
                    dicupdaterecords.Add(each.StudentID, new List<JHUpdateRecordRecord>());
                dicupdaterecords[each.StudentID].Add(each);
            }

            foreach (KeyValuePair<string, List<JHUpdateRecordRecord>> each in dicupdaterecords)
            {
                each.Value.Sort(delegate(JHUpdateRecordRecord x, JHUpdateRecordRecord y)
                {
                    DateTime xx, yy;

                    if (!DateTime.TryParse(x.UpdateDate, out xx))
                        xx = DateTime.MinValue;

                    if (!DateTime.TryParse(y.UpdateDate, out yy))
                        yy = DateTime.MinValue;

                    return xx.CompareTo(yy);
                });
            }

            string ECodes = "1"; //入學
            string GCodes = "2";    //畢業
            foreach (KeyValuePair<string, List<JHUpdateRecordRecord>> each in dicupdaterecords)
            {
                if (!dicstudents.ContainsKey(each.Key)) continue;
                ReportStudent student = dicstudents[each.Key];

                JHUpdateRecordRecord e = each.Value[0];
                JHUpdateRecordRecord g = each.Value[each.Value.Count - 1];

                if (ECodes.IndexOf(e.UpdateCode) >= 0)
                {
                    DateTime dt;

                    if (DateTime.TryParse(e.UpdateDate, out dt))
                    {
                        student.EntranceDate = string.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day);
                        student.EngEntranceDate = dt.ToString(Util.EnglishFormat, Util.USCulture);
                    }
                }

                if (GCodes.IndexOf(g.UpdateCode) >= 0)
                {
                    DateTime dt;

                    if (DateTime.TryParse(g.UpdateDate, out dt))
                    {
                        student.GraduateDate = string.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day);
                        student.EngGraduateDate = dt.ToString(Util.EnglishFormat, Util.USCulture);
                    }
                }
            }

            Console.WriteLine(Environment.TickCount - t1);
        }
        #endregion

        public static void ReadGraduatePhoto(this IEnumerable<ReportStudent> students, IStatusReporter reporter)
        {
            Dictionary<string, ReportStudent> dicstudents = students.ToDictionary();
            List<string> keys = students.ToSC().ToKeys();

            Campus.FunctionSpliter<string, PhotoRecord> selectData = new Campus.FunctionSpliter<string, PhotoRecord>(300, 5);
            selectData.Function = delegate(List<string> ps)
            {
                List<PhotoRecord> photos = new List<PhotoRecord>();
                foreach (KeyValuePair<string, string> each in Photo.SelectGraduatePhoto(ps))
                    photos.Add(new PhotoRecord(each.Key, each.Value));
                return photos;
            };
            List<PhotoRecord> updaterecords = selectData.Execute(keys);

            foreach (PhotoRecord each in updaterecords)
            {
                if (dicstudents.ContainsKey(each.ID))
                    dicstudents[each.ID].GraduatePhoto = each.Photo;
            }
        }

        private class PhotoRecord
        {
            public PhotoRecord(string id, string photo)
            {
                ID = id;

                if (!string.IsNullOrEmpty(photo))
                {
                    Stream imgstream = new MemoryStream(Convert.FromBase64String(photo));
                    imgstream.Seek(0, SeekOrigin.Begin);

                    Photo = Image.FromStream(imgstream);
                }
                else
                    Photo = null;
            }

            public string ID { get; private set; }

            public Image Photo { get; private set; }
        }
    }
}
