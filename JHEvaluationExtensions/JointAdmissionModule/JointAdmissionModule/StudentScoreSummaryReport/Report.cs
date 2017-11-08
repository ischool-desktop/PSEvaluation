using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Aspose.Words;
using Aspose.Words.Reporting;
using Campus.Rating;
using FISCA.Presentation.Controls;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using JHSchool.Data;
using Subj = JHSchool.Evaluation.Subject;

namespace JointAdmissionModule.StudentScoreSummaryReport
{
    internal class Report
    {
        private ReportPreference PrintSetting { get; set; }

        private List<ReportStudent> Students { get; set; }

        private Dictionary<string, List<string>> PrintAbsences { get; set; }

        private List<string> DetailDomain { get; set; }

        /// <summary>
        /// 是否產生匯入檔
        /// </summary>
        public static bool CheckExportFile = false;

        // 學生名次與名次百分比(UDT)
        private Dictionary<string, List<DAL.UserDefData>> _StudRankData;

        private const string LearningDomainName = "學期成績平均";
        private const string LearningDomainNameSpcAdd = "特種身分加分後之學期成績平均";
        private const string LearningDomainNameP = "學期成績平均排名/百分比";
        private const string LearningDomainNameSpcAddP = "特種身分加分後之學期成績平均排名/百分比";
        private const string CalcMessage = "樂學計分";
        private const string StudSpcName1 = "原住民(持文化及語言能力證明)";
        private const string StudSpcName2 = "原住民(未持文化及語言能力證明)";
        private const string StudSpcName3 = "境外優秀科學技術人才子女";
        private const string StudSpcName4 = "政府派赴國外工作人員子女";
        private const string StudSpcName5 = "蒙藏生";
        private const string StudSpcName6 = "身心障礙生";


        public Report(List<ReportStudent> students, ReportPreference printSetting)
        {
            Students = students;
            Students.Sort(delegate(ReportStudent x, ReportStudent y)
            {
                return x.OrderString.CompareTo(y.OrderString);
            });

            PrintSetting = printSetting;

            DetailDomain = new List<string>();

            //新竹的要把語文詳列。
            if (Program.Mode == ModuleMode.HsinChu)
                DetailDomain.Add("語文");

            DetailDomain.Add("彈性課程");
            DetailDomain.Add("");

            PrintAbsences = printSetting.AcceptAbsences.PeriodOptionsFromString();

            // 取得學生ID
            List<string> sidList = (from xx in students select xx.StudentID).ToList();
            // 取得學生放在UDT的排名資料
            _StudRankData = DAL.UDTTransfer.GetDataFromUDTDict(sidList);

            // 需產生匯入檔
            if (CheckExportFile)
                ExportInputFile();                
        }

        /// <summary>
        /// 產生匯入檔
        /// </summary>
        private void ExportInputFile()
        {
            List<ReportStudentData> ExportData = new List<ReportStudentData>();

            List<string> SIDList = (from x in Students select x.StudentID).ToList();

            // 父母資料
            List<JHParentRecord> ParentRecList = JHParent.SelectByStudentIDs(SIDList);

            // 地址
            List<JHAddressRecord> AddressRecList = JHAddress.SelectByStudentIDs(SIDList);

            // 電話
            List<JHPhoneRecord> PhoneRecList = JHPhone.SelectByStudentIDs(SIDList);
            List<SLearningDomainParser> allsems = new List<SLearningDomainParser>();
            allsems.Add(new SLearningDomainParser(1, 1));
            allsems.Add(new SLearningDomainParser(1, 2));
            allsems.Add(new SLearningDomainParser(2, 1));
            allsems.Add(new SLearningDomainParser(2, 2));
            allsems.Add(new SLearningDomainParser(3, 1));
            allsems.Add(new SLearningDomainParser(3, 2));
            allsems.Add(new SLearningDomainParser(7, 1));
            allsems.Add(new SLearningDomainParser(7, 2));
            allsems.Add(new SLearningDomainParser(8, 1));
            allsems.Add(new SLearningDomainParser(8, 2));
            allsems.Add(new SLearningDomainParser(9, 1));
            allsems.Add(new SLearningDomainParser(9, 2));

            //// 取得 UDT 對照
            //List<JointAdmissionModule.DAL.UserDefData_StudTypeWeight> udtList = DAL.UDTTransfer.GetDataFromUDT_StudTypeWeight().Where(x => x.SchoolType == "高中").ToList();
            //// 建立特種身分代碼對照表
            //Dictionary<string, string> specCodeDict = new Dictionary<string, string>();
            //foreach (JointAdmissionModule.DAL.UserDefData_StudTypeWeight dd in udtList)
            //    if (!specCodeDict.ContainsKey(dd.JoinStudType))
            //        specCodeDict.Add(dd.JoinStudType, dd.JoinStudTypeCode);


            // 資料轉換
            foreach (ReportStudent stud in Students)
            {
                // 是否已有年排名資料
                List<DAL.UserDefData> uddList = new List<JointAdmissionModule.DAL.UserDefData>();
                if (_StudRankData.ContainsKey(stud.StudentID))
                    uddList = _StudRankData[stud.StudentID];


                ReportStudentData rsd = new ReportStudentData();
                // 班級
                if (stud.Class != null)
                    rsd.ClassName = stud.Class.Name;

                // 座號
                rsd.SeatNo = stud.SeatNo;
                
                // 姓名
                rsd.Name = stud.Name;

                // 家長姓名
                foreach (JHParentRecord rec in ParentRecList.Where(x => x.RefStudentID == stud.StudentID))
                {
                    rsd.ParentName = rec.CustodianName;
                }

                foreach (JHPhoneRecord rec in PhoneRecList.Where(x => x.RefStudentID == stud.StudentID))
                {
                    // 電話
                    rsd.Phone = rec.Contact;
                    // 手機
                    rsd.CellPhone = rec.Cell;                    
                }
                // 生日
                DateTime dt;
                if (DateTime.TryParse(stud.Birthday, out dt))
                {
                    rsd.Birthday = dt;
                }

                // 性別
                if (stud.Gender == "男")
                    rsd.GenderCode = "1";

                if (stud.Gender == "女")
                    rsd.GenderCode = "2";
                
                
                // 身分證字號
                rsd.IDNumber = stud.IDNumber;

                foreach (JHAddressRecord rec in AddressRecList.Where(x => x.RefStudentID == stud.StudentID))
                {
                    // 郵區號
                    rsd.ZipCode = rec.MailingZipCode;
                    // 聯絡地址
                    rsd.Address = rec.MailingCounty + rec.MailingTown + rec.MailingDistrict + rec.MailingArea + rec.MailingDetail;
                }

                //string code = "";
                // 特種身分(原民未持1、原民認證2、身心障礙3、其他4)
                if (!string.IsNullOrEmpty(stud.SpcStudTypeName))
                {
                    rsd.SpceTypeCode = 4;
                    if (stud.SpcStudTypeName == StudSpcName1)
                        rsd.SpceTypeCode = 2;
                    if (stud.SpcStudTypeName == StudSpcName2)
                        rsd.SpceTypeCode = 1;
                    if (stud.SpcStudTypeName == StudSpcName6)
                       rsd.SpceTypeCode = 3;              
                }

                PlaceCollection Places = stud.Places.NS("年排名");
                
                // 這寫法主要判斷當名次在不同年級時可能會讀到錯誤資料
                //if (Places[placeKey].Score == 0 && Places[placeKey].Level == 1) { }
                //else

                foreach (SLearningDomainParser semsIndex in allsems)
                {

                    if ((semsIndex.Grade == 1 || semsIndex.Grade == 7) && semsIndex.sm == 1)
                    {
                        // 一上名次
                        // 一上名次百分比
                        string placeKey = semsIndex.Name;
                        if (Places.Contains(placeKey))
                        {
                            if (Places[placeKey].Score == 0 && Places[placeKey].Level == 1) { }
                            else
                            {
                                rsd.Rank11 = Places[placeKey].Level;
                                rsd.RankPercent11 = Places[placeKey].Percentage;

                                // 當有加分即時運算
                                if (stud.AddWeight.HasValue)
                                {
                                    int Level = 1,Percentage=1;
                                    List<Place> PList = new List<Place>();
                                    decimal sc = Places[placeKey].Score * stud.AddWeight.Value;
                                    if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(stud.GradeYear))
                                    {
                                        if (DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].ContainsKey(placeKey))
                                        {
                                            PList = (from data in DAL.DALTransfer.StudRankScoreDict[stud.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                            if (PList.Count > 0)
                                            {
                                                PList.OrderBy(x => x.Score);
                                                Level = PList[0].Level;
                                                Percentage = PList[0].Percentage;
                                            }

                                            rsd.Rank11 = Level;
                                            rsd.RankPercent11 = Percentage;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if ((semsIndex.Grade == 1 || semsIndex.Grade == 7) && semsIndex.sm == 2)
                    {
                        // 一下名次
                        // 一下名次百分比
                        string placeKey = semsIndex.Name;
                        if (Places.Contains(placeKey))
                        {
                            if (Places[placeKey].Score == 0 && Places[placeKey].Level == 1) { }
                            else
                            {
                                rsd.Rank12 = Places[placeKey].Level;
                                rsd.RankPercent12 = Places[placeKey].Percentage;

                                // 當有加分即時運算
                                if (stud.AddWeight.HasValue)
                                {
                                    int Level = 1, Percentage = 1;
                                    List<Place> PList = new List<Place>();
                                    decimal sc = Places[placeKey].Score * stud.AddWeight.Value;
                                    if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(stud.GradeYear))
                                    {
                                        if (DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].ContainsKey(placeKey))
                                        {
                                            PList = (from data in DAL.DALTransfer.StudRankScoreDict[stud.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                            if (PList.Count > 0)
                                            {
                                                PList.OrderBy(x => x.Score);
                                                Level = PList[0].Level;
                                                Percentage = PList[0].Percentage;
                                            }

                                            rsd.Rank12 = Level;
                                            rsd.RankPercent12 = Percentage;

                                        }
                                    }
                                }
                            }
                        }

                    }

                    if ((semsIndex.Grade == 2 || semsIndex.Grade == 8) && semsIndex.sm == 1)
                    {
                        // 二上名次
                        // 二上名次百分比
                        string placeKey = semsIndex.Name;
                        if (Places.Contains(placeKey))
                        {
                            if (Places[placeKey].Score == 0 && Places[placeKey].Level == 1) { }
                            else
                            {
                                rsd.Rank21 = Places[placeKey].Level;
                                rsd.RankPercent21 = Places[placeKey].Percentage;

                                // 當有加分即時運算
                                if (stud.AddWeight.HasValue)
                                {
                                    int Level = 1, Percentage = 1;
                                    List<Place> PList = new List<Place>();
                                    decimal sc = Places[placeKey].Score * stud.AddWeight.Value;
                                    if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(stud.GradeYear))
                                    {
                                        if (DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].ContainsKey(placeKey))
                                        {
                                            PList = (from data in DAL.DALTransfer.StudRankScoreDict[stud.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                            if (PList.Count > 0)
                                            {
                                                PList.OrderBy(x => x.Score);
                                                Level = PList[0].Level;
                                                Percentage = PList[0].Percentage;
                                            }
                                        }

                                        rsd.Rank21 = Level;
                                        rsd.RankPercent21 = Percentage;

                                    }
                                }
                            }
                        }

                    }

                    if ((semsIndex.Grade == 2 || semsIndex.Grade == 8) && semsIndex.sm == 2)
                    {
                        // 二下名次
                        // 二下名次百分比
                        string placeKey = semsIndex.Name;
                        if (Places.Contains(placeKey))
                        {
                            if (Places[placeKey].Score == 0 && Places[placeKey].Level == 1) { }
                            else
                            {
                                rsd.Rank22 = Places[placeKey].Level;
                                rsd.RankPercent22 = Places[placeKey].Percentage;

                                // 當有加分即時運算
                                if (stud.AddWeight.HasValue)
                                {
                                    int Level = 1, Percentage = 1;
                                    List<Place> PList = new List<Place>();
                                    decimal sc = Places[placeKey].Score * stud.AddWeight.Value;
                                    if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(stud.GradeYear))
                                    {
                                        if (DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].ContainsKey(placeKey))
                                        {
                                            PList = (from data in DAL.DALTransfer.StudRankScoreDict[stud.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                            if (PList.Count > 0)
                                            {
                                                PList.OrderBy(x => x.Score);
                                                Level = PList[0].Level;
                                                Percentage = PList[0].Percentage;
                                            }

                                            rsd.Rank22 = Level;
                                            rsd.RankPercent22 = Percentage;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if ((semsIndex.Grade == 3 || semsIndex.Grade == 9) && semsIndex.sm == 1)
                    {
                        // 三上名次
                        // 三上名次百分比
                        string placeKey = semsIndex.Name;
                        if (Places.Contains(placeKey))
                        {
                            if (Places[placeKey].Score == 0 && Places[placeKey].Level == 1) { }
                            else
                            {
                                rsd.Rank31 = Places[placeKey].Level;
                                rsd.RankPercent31 = Places[placeKey].Percentage;

                                // 當有加分即時運算
                                if (stud.AddWeight.HasValue)
                                {
                                    int Level = 1, Percentage = 1;
                                    List<Place> PList = new List<Place>();
                                    decimal sc = Places[placeKey].Score * stud.AddWeight.Value;
                                    if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(stud.GradeYear))
                                    {
                                        if (DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].ContainsKey(placeKey))
                                        {
                                            PList = (from data in DAL.DALTransfer.StudRankScoreDict[stud.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                            if (PList.Count > 0)
                                            {
                                                PList.OrderBy(x => x.Score);
                                                Level = PList[0].Level;
                                                Percentage = PList[0].Percentage;
                                            }
                                        }
                                        rsd.Rank31 = Level;
                                        rsd.RankPercent31 = Percentage;
                                    }
                                }
                            }
                        }
                    }

                }
                if (Places.Contains("學習領域"))
                {
                    string placeKey = "學習領域";
                    // 五學期名次
                    rsd.AvgRank5 = Places[placeKey].Level;
                    // 五學期名次百分比
                    rsd.AvgRankPercent5 = Places[placeKey].Percentage;

                    // 當有加分即時運算
                    if (stud.AddWeight.HasValue)
                    {
                        int Level = 1, Percentage = 1;
                        List<Place> PList = new List<Place>();
                        decimal sc = Places[placeKey].Score * stud.AddWeight.Value;
                        if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(stud.GradeYear))
                        {
                            placeKey = "學期總平均";

                            if (DAL.DALTransfer.StudRankScoreDict[stud.GradeYear].ContainsKey(placeKey))
                            {
                                PList = (from data in DAL.DALTransfer.StudRankScoreDict[stud.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                if (PList.Count > 0)
                                {
                                    PList.OrderBy(x => x.Score);
                                    Level = PList[0].Level;
                                    Percentage = PList[0].Percentage;
                                }
                            }

                            rsd.AvgRank5 = Level;
                            rsd.AvgRankPercent5 = Percentage;
                        }
                    }
                }



                // 處理已有年排名(在UDT有存資料，填入 UDT 資料)
                if (uddList.Count > 0 && stud.LastEnterSemester.HasValue)
                {
                    foreach (DAL.UserDefData udd in uddList)
                    {
                        // 七上
                        if ((udd.GradeYear == 1 || udd.GradeYear == 7) && udd.Semester == 1)
                        {
                            rsd.Rank11 = udd.GradeRank;
                            rsd.RankPercent11 = udd.GradeRankPercent;

                            // 特種身分加分
                            if (stud.AddWeight.HasValue)
                            {
                                rsd.Rank11 = udd.GradeRankAdd;
                                rsd.RankPercent11 = udd.GradeRankPercentAdd;
                            }

                        }

                        // 七下
                        if ((udd.GradeYear == 1 || udd.GradeYear == 7) && udd.Semester == 2)
                        {
                            rsd.Rank12 = udd.GradeRank;
                            rsd.RankPercent12 = udd.GradeRankPercent;


                            // 特種身分加分
                            if (stud.AddWeight.HasValue)
                            {
                                rsd.Rank12 = udd.GradeRankAdd;
                                rsd.RankPercent12 = udd.GradeRankPercentAdd;
                            }
                        }

                        // 八上
                        if ((udd.GradeYear==2 ||udd.GradeYear==8) && udd.Semester ==1)
                        {
                            rsd.Rank21 = udd.GradeRank;
                            rsd.RankPercent21 = udd.GradeRankPercent;


                            // 特種身分加分
                            if (stud.AddWeight.HasValue)
                            {
                                rsd.Rank21 = udd.GradeRankAdd;
                                rsd.RankPercent21 = udd.GradeRankPercentAdd;
                            }
                        }

                        // 八下
                        if ((udd.GradeYear == 2 || udd.GradeYear == 8) && udd.Semester == 2)
                        {
                            rsd.Rank22 = udd.GradeRank;
                            rsd.RankPercent22 = udd.GradeRankPercent;


                            // 特種身分加分
                            if (stud.AddWeight.HasValue)
                            {
                                rsd.Rank22 = udd.GradeRankAdd;
                                rsd.RankPercent22 = udd.GradeRankPercentAdd;
                            }
                        }

                        // 九上
                        if ((udd.GradeYear == 3 || udd.GradeYear == 9) && udd.Semester == 1)
                        {
                            rsd.Rank31 = udd.GradeRank;
                            rsd.RankPercent31 = udd.GradeRankPercent;


                            // 特種身分加分
                            if (stud.AddWeight.HasValue)
                            {
                                rsd.Rank31 = udd.GradeRankAdd;
                                rsd.RankPercent31 = udd.GradeRankPercentAdd;
                            }
                        }

                    }
                }

                // 判斷轉入生，清空之前名次
                if (stud.LastEnterGradeYear.HasValue && stud.LastEnterSchoolyear.HasValue )
                {
                    int grYear = 0;

                    // 先判斷年級
                    if (stud.LastEnterGradeYear >= 6)
                    {
                        grYear = stud.LastEnterGradeYear.Value - 6;
                    }
                    else
                    {
                        grYear = stud.LastEnterGradeYear.Value;
                    }

                    // 判斷udd有輸入資料
                    if (grYear == 3 && stud.LastEnterSemester == 1)
                    {
                        bool clear11 = true, clear12 = true, clear21 = true, clear22 = true;

                        foreach (DAL.UserDefData udd in uddList)
                        {
                            if (udd.GradeYear == 1 && udd.Semester == 1)
                                clear11 = false;

                            if (udd.GradeYear == 1 && udd.Semester == 2)
                                clear12 = false;

                            if (udd.GradeYear == 2 && udd.Semester == 1)
                                clear21 = false;

                            if (udd.GradeYear == 2 && udd.Semester == 2)
                                clear22 = false;                        
                        }

                        if (clear11)
                        {
                            rsd.Rank11 = null;
                            rsd.RankPercent11 = null;
                        }

                        if (clear12)
                        {
                            rsd.Rank12 = null;
                            rsd.RankPercent12 = null;
                        }

                        if (clear21)
                        {
                            rsd.Rank21 = null;
                            rsd.RankPercent21 = null;
                        }

                        if (clear22)
                        {
                            rsd.Rank22 = null;
                            rsd.RankPercent22 = null;
                        }
                    }

                    if (grYear == 2 && stud.LastEnterSemester == 2)
                    {
                        bool clear11 = true, clear12 = true, clear21 = true;

                        foreach (DAL.UserDefData udd in uddList)
                        {
                            if (udd.GradeYear == 1 && udd.Semester == 1)
                                clear11 = false;

                            if (udd.GradeYear == 1 && udd.Semester == 2)
                                clear12 = false;

                            if (udd.GradeYear == 2 && udd.Semester == 1)
                                clear21 = false;
                        }

                        if (clear11)
                        {
                            rsd.Rank11 = null;
                            rsd.RankPercent11 = null;
                        }

                        if (clear12)
                        {
                            rsd.Rank12 = null;
                            rsd.RankPercent12 = null;
                        }

                        if (clear21)
                        {
                            rsd.Rank21 = null;
                            rsd.RankPercent21 = null;
                        }
                    }

                    if (grYear == 2 && stud.LastEnterSemester == 1)
                    {
                        bool clear11 = true, clear12 = true;

                        foreach (DAL.UserDefData udd in uddList)
                        {
                            if (udd.GradeYear == 1 && udd.Semester == 1)
                                clear11 = false;

                            if (udd.GradeYear == 1 && udd.Semester == 2)
                                clear12 = false;
                        }

                        if (clear11)
                        {
                            rsd.Rank11 = null;
                            rsd.RankPercent11 = null;
                        }

                        if (clear12)
                        {
                            rsd.Rank12 = null;
                            rsd.RankPercent12 = null;
                        }
                    }

                    if (grYear == 1 && stud.LastEnterSemester == 2)
                    {
                        bool clear11 = true;

                        foreach (DAL.UserDefData udd in uddList)
                        {
                            if (udd.GradeYear == 1 && udd.Semester == 1)
                                clear11 = false;
                        }

                        if (clear11)
                        {
                            rsd.Rank11 = null;
                            rsd.RankPercent11 = null;
                        }
                    }

                    // 當沒有輸入全部清空
                    if (uddList.Count == 0)
                    {
                        if (grYear == 3 && stud.LastEnterSemester == 1)
                        {
                            rsd.Rank22 = null;
                            rsd.Rank21 = null;
                            rsd.Rank12 = null;
                            rsd.Rank11 = null;
                            rsd.RankPercent22 = null;
                            rsd.RankPercent21 = null;
                            rsd.RankPercent12 = null;
                            rsd.RankPercent11 = null;
                        }

                        if (grYear == 2 && stud.LastEnterSemester == 2)
                        {
                            rsd.Rank21 = null;
                            rsd.Rank12 = null;
                            rsd.Rank11 = null;
                            rsd.RankPercent21 = null;
                            rsd.RankPercent12 = null;
                            rsd.RankPercent11 = null;
                        }

                        if (grYear == 2 && stud.LastEnterSemester == 1)
                        {
                            rsd.Rank12 = null;
                            rsd.Rank11 = null;
                            rsd.RankPercent12 = null;
                            rsd.RankPercent11 = null;
                        }


                        if (grYear == 1 && stud.LastEnterSemester == 2)
                        {
                            rsd.Rank11 = null;
                            rsd.RankPercent11 = null;
                        }
                    }
                }

                rsd.UpdateDate3 = stud.TransUpdateDateStr;
                ExportData.Add(rsd);
            }
        
            // 印報表
            if (ExportData.Count > 0)
            {
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
                wb.Open(new MemoryStream(Properties.Resources._100高雄高中多元入學匯入檔));
                Aspose.Cells.Worksheet wst = wb.Worksheets[0];
    
                int RowIdx = 1;

                foreach (ReportStudentData rsd in ExportData)
                {
                    // 班級 0
                    wst.Cells[RowIdx, 0].PutValue(rsd.ClassName);

                    // 座號 1
                    wst.Cells[RowIdx, 1].PutValue(string.Format("{0:00}",rsd.SeatNo));

                    // 姓名 2
                    wst.Cells[RowIdx, 2].PutValue(rsd.Name);

                    // 家長姓名 3
                    wst.Cells[RowIdx, 3].PutValue(rsd.ParentName);

                    if (rsd.Birthday.HasValue)
                    {
                        // 年 4
                        wst.Cells[RowIdx, 4].PutValue((rsd.Birthday.Value.Year -1911));

                        // 月 5
                        wst.Cells[RowIdx, 5].PutValue(rsd.Birthday.Value.Month);

                        // 日 6
                        wst.Cells[RowIdx, 6].PutValue(rsd.Birthday.Value.Day);
                    }

                    // 性別 7
                    wst.Cells[RowIdx, 7].PutValue(rsd.GenderCode);

                    // 身分證字號 8
                    wst.Cells[RowIdx, 8].PutValue(rsd.IDNumber);

                    // 郵區號 9
                    wst.Cells[RowIdx, 9].PutValue(rsd.ZipCode);

                    // 聯絡地址 10
                    wst.Cells[RowIdx, 10].PutValue(rsd.Address);

                    // 電話 11
                    wst.Cells[RowIdx, 11].PutValue(rsd.Phone);

                    // 手機 12
                    wst.Cells[RowIdx, 12].PutValue(rsd.CellPhone);

                    // 特種身分(原民未持1、原民認證2、身心障礙3、其他4) 13
                    if(rsd.SpceTypeCode.HasValue)
                        wst.Cells[RowIdx, 13].PutValue(rsd.SpceTypeCode.Value);
                    else
                        wst.Cells[RowIdx, 13].PutValue("");

                    // 一上名次 14
                    if(rsd.Rank11.HasValue)
                        wst.Cells[RowIdx, 15].PutValue(rsd.Rank11.Value);

                    // 一上名次百分比 15
                    if(rsd.RankPercent11.HasValue )
                        wst.Cells[RowIdx, 16].PutValue(rsd.RankPercent11.Value);

                    // 一下名次 16
                    if(rsd.Rank12.HasValue)
                        wst.Cells[RowIdx, 17].PutValue(rsd.Rank12.Value);

                    // 一下名次百分比 17
                    if(rsd.RankPercent12.HasValue)
                        wst.Cells[RowIdx, 18].PutValue(rsd.RankPercent12.Value);

                    // 二上名次 18
                    if(rsd.Rank21.HasValue )
                        wst.Cells[RowIdx, 19].PutValue(rsd.Rank21.Value);

                    // 二上名次百分比 19
                    if(rsd.RankPercent21.HasValue )
                        wst.Cells[RowIdx, 20].PutValue(rsd.RankPercent21.Value);

                    // 二下名次 20
                    if(rsd.Rank22.HasValue)
                        wst.Cells[RowIdx, 21].PutValue(rsd.Rank22.Value);

                    // 二下名次百分比 21
                    if(rsd.RankPercent22.HasValue)
                    wst.Cells[RowIdx, 22].PutValue(rsd.RankPercent22.Value);

                    // 三上名次 22
                    if(rsd.Rank31.HasValue)
                    wst.Cells[RowIdx, 23].PutValue(rsd.Rank31.Value);

                    // 三上名次百分比 23
                    if(rsd.RankPercent31.HasValue )
                    wst.Cells[RowIdx, 24].PutValue(rsd.RankPercent31.Value);

                    // 五學期名次 24
                    if(rsd.AvgRank5.HasValue )
                    wst.Cells[RowIdx, 25].PutValue(rsd.AvgRank5.Value);

                    // 五學期名次百分比 25
                    if(rsd.AvgRankPercent5.HasValue )
                    wst.Cells[RowIdx, 26].PutValue(rsd.AvgRankPercent5.Value);

                    // 是否是轉學生1是，否空白
                    if(!string.IsNullOrEmpty(rsd.UpdateDate3))
                        wst.Cells[RowIdx, 27].PutValue("1");
                    RowIdx++;
                }
                string FileName = "高中(高雄)用國中匯入檔.xls";

                // 儲存檔案
                try
                {
                    string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!dir.Exists) dir.Create();

                    path = Path.Combine(path, FileName);

                    if (File.Exists(path))
                    {
                        int i = 1;
                        while (true)
                        {
                            string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                            if (!File.Exists(newPath))
                            {
                                path = newPath;
                                break;
                            }
                        }
                    }
                    wb.Save(path, Aspose.Cells.FileFormatType.Excel2003);
                    if (MsgBox.Show(FileName + "產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗");
                }
            }
        }


        public Document Print()
        {
            Document doc = PrintSetting.Template.ToDocument();

            doc.MailMerge.MergeField += new Aspose.Words.Reporting.MergeFieldEventHandler(MailMerge_MergeField);

            doc.MailMerge.Execute(new MergeDataSource(Students, PrintSetting));

            return doc;
        }

        //最好這程式有人能維護的了.......
        private void MailMerge_MergeField(object sender, MergeFieldEventArgs e)
        {
            //不是 Fix 開頭的合併欄位不處理。
            if (!e.FieldName.ToUpper().StartsWith("Fix".ToUpper())) return;

            DocumentBuilder builder = new DocumentBuilder(e.Document);

            ReportStudent student = e.FieldValue as ReportStudent;

            //如果合併值不是 ReportStudent 就跳過...意思是有問題...。
            if (student == null) return;

            builder.MoveToField(e.Field, true);
            e.Field.Remove();

            if (e.FieldName == "Fix:年級學期")
            {
                #region 列印年級學期資訊(有點複雜)。
                SemesterDataCollection semesters = student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters);
                Row SemesterRow = builder.CurrentParagraph.ParentNode.ParentNode.NextSibling as Row; //下一個 Row。
                Paragraph originParagraph = builder.CurrentParagraph;

                int count = 0, offset = 1;

                foreach (SemesterData each in semesters)
                {
                    string currentGradeyear = Util.GetGradeyearString(each.GradeYear.ToString());

                    //如果沒有年級，就跳過。
                    if (string.IsNullOrEmpty(currentGradeyear)) continue;

                    builder.Write(currentGradeyear + "年級");
                    Paragraph nextPh = Util.NextCell(builder.CurrentParagraph);
                    if (nextPh == null) break; //沒有下一個 Cell ，就不印資料了。
                    builder.MoveTo(nextPh);

                    Paragraph resetParagraph = builder.CurrentParagraph;
                    SemesterRow.Cells[count + offset].Write(builder, GetSemesterString(each));

                    SemesterData semester = new SemesterData(0, each.SchoolYear, each.Semester);
                    if (!student.HeaderList.ContainsKey(semester))
                        student.HeaderList.AddRaw(each, count); //不要懷疑，這是對的。

                    builder.MoveTo(resetParagraph);
                    count++;
                }

                builder.MoveTo(originParagraph);
                (originParagraph.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                Paragraph nextParagrap = originParagraph;
                string previousGradeyear = GetText(originParagraph);
                while ((nextParagrap = Util.NextCell(nextParagrap)) != null)
                {
                    if (GetText(nextParagrap) == previousGradeyear && !string.IsNullOrEmpty(previousGradeyear))
                    {
                        (nextParagrap.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                        (nextParagrap.ParentNode as Cell).Paragraphs[0].Runs.Clear();
                    }
                    else
                        (nextParagrap.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;

                    previousGradeyear = GetText(nextParagrap);
                }
                #endregion
            }
            else if (e.FieldName == "Fix:科目資訊")
            {
                #region 列印科目資料(爆炸複雜)

                Row template = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                Table table = template.ParentNode as Table;

                if (PrintSetting.ListMethod == ListMethod.DomainOnly)
                {
                    #region 列印領域
                    UniqueSet<RowHeader> RowIndexs = new UniqueSet<RowHeader>();
                    //Environment.OSVersion.Platform
                    #region 列印 RowHeader
                    foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                    {
                        SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);

                        //如果不包含該學期成績資料，就跳過。
                        if (!student.SemestersScore.Contains(sysems)) continue;

                        SemesterScore semsscore = student.SemestersScore[sysems];

                        ////準備彈性課程的科目(要詳列出來)。
                        //foreach (string strSubject in semsscore.Subject)
                        //{
                        //    SemesterSubjectScore subject = semsscore.Subject[strSubject];

                        //    if (DetailDomain.Contains(subject.Domain))
                        //    {
                        //        RefineDomain(subject);

                        //        RowHeader header = new RowHeader(subject.Domain, strSubject);
                        //        header.IsDomain = false;

                        //        if (!RowIndexs.Contains(header))
                        //            RowIndexs.Add(header);
                        //    }
                        //}

                        //準備領域資料。
                        foreach (string strDomain in semsscore.Domain)
                        {
                            if (!Subj.Domains.Contains(strDomain)) continue;

                            SemesterDomainScore domain = semsscore.Domain[strDomain];

                            if (!DetailDomain.Contains(strDomain))
                            {
                                RowHeader header = new RowHeader(strDomain, string.Empty);
                                header.IsDomain = true;

                                if (!RowIndexs.Contains(header))
                                    RowIndexs.Add(header);
                            }
                        }
                    }

                    RowHeader lheader = new RowHeader(LearningDomainName, string.Empty);
                    RowHeader lheader1 = new RowHeader(LearningDomainNameSpcAdd, string.Empty);
                    RowHeader lheader2 = new RowHeader(LearningDomainNameP, string.Empty);
                    RowHeader lheader3 = new RowHeader(LearningDomainNameSpcAddP, string.Empty);
                    RowHeader lheader4 = new RowHeader(CalcMessage, string.Empty);
                    
                    lheader.IsDomain = true;
                    lheader1.IsDomain = true;
                    lheader2.IsDomain = true;
                    lheader3.IsDomain = true;
                    lheader4.IsDomain = true;
                    RowIndexs.Add(lheader);
                    RowIndexs.Add(lheader1);
                    RowIndexs.Add(lheader2);
                    RowIndexs.Add(lheader3);
                    RowIndexs.Add(lheader4);

                    List<RowHeader> sortedHeaders = SortHeader(RowIndexs.ToList());

                    //產生 Row。
                    List<RowHeader> indexHeaders = new List<RowHeader>();
                    Row current = template;
                    int rowIndex = 0;
                    foreach (RowHeader header in sortedHeaders)
                    {
                        RowHeader indexH = header;
                        indexH.Index = rowIndex++;
                        indexHeaders.Add(indexH);
                        bool hasGroup = !string.IsNullOrEmpty(Subj.GetDomainGroup(header.Domain));
                        string groupName = Subj.GetDomainGroup(header.Domain);

                        Row datarow = table.InsertBefore(template.Clone(true), current) as Row;

                        //if (header.Domain == LearningDomainName)
                        //{
                        //    string headerName = string.Empty;

                        //    if (PrintSetting.PrintRank)
                        //        headerName = "學期成績平均排名";

                        //    if (PrintSetting.PrintRankPercentage)
                        //        headerName = "學期成績平均百分比";

                        //    if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                        //        headerName = "學期成績平均排名/百分比";

                        //    if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                        //    {
                        //        Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                        //        lrow.Cells[0].Write(builder, headerName);
                        //        rowIndex++;
                        //    }
                        //}

                        //if (header.Domain == LearningDomainNameSpcAdd)
                        //{
                        //    string headerName = string.Empty;

                        //    if (PrintSetting.PrintRank)
                        //        headerName = "特種身分加分後之學期成績平均排名";

                        //    if (PrintSetting.PrintRankPercentage)
                        //        headerName = "特種身分加分後之學期成績平均百分比";

                        //    if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                        //        headerName = "特種身分加分後之學期成績平均排名/百分比";

                        //    if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                        //    {
                        //        Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                        //        lrow.Cells[0].Write(builder, headerName);
                        //        rowIndex++;
                        //    }
                        //}

                        if (header.IsDomain)
                        {
                            if (hasGroup)
                            {
                                datarow.Cells[0].Write(builder, groupName);
                                datarow.Cells[1].Write(builder, header.Domain);
                            }
                            else
                            {
                                datarow.Cells[0].Write(builder, header.Domain);
                            }
                        }
                        else
                        {
                            if (header.Domain == LearningDomainNameP)
                            {
                                string headerName = string.Empty;

                                if (PrintSetting.PrintRank)
                                    headerName = "學期成績平均排名";

                                if (PrintSetting.PrintRankPercentage)
                                    headerName = "學期成績平均百分比";

                                if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                                    headerName = "學期成績平均排名/百分比";

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                                    lrow.Cells[0].Write(builder, headerName);
                                    rowIndex++;
                                }
                            }

                            if (header.Domain == LearningDomainNameSpcAddP)
                            {
                                string headerName = string.Empty;

                                if (PrintSetting.PrintRank)
                                    headerName = "特種身分加分後之學期成績平均排名";

                                if (PrintSetting.PrintRankPercentage)
                                    headerName = "特種身分加分後之學期成績平均百分比";

                                if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                                    headerName = "特種身分加分後之學期成績平均排名/百分比";

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                                    lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                                    lrow.Cells[0].Write(builder, headerName);
                                    rowIndex++;
                                }
                            }

                            //把空白的領域當成「彈性課程」。
                            string domain = IsFlexible(header.Domain) ? "彈性課程" : header.Domain;

                            // 修改不需要彈性課程，標頭
                            datarow.Cells[0].Write(builder, "^^");
                            datarow.Cells[1].Write(builder, "^_^");
                        }
                    }
                    #endregion

                    #region 填資料
                    Row RatingRow = null;
                    // 用在加分後
                    Row RatingRowAdd = null;
                    // 是否已有年排名資料
                    List<DAL.UserDefData> uddList = new List<JointAdmissionModule.DAL.UserDefData>();
                    if (_StudRankData.ContainsKey(student.StudentID))
                        uddList = _StudRankData[student.StudentID];
                    
                    foreach (RowHeader header in indexHeaders)
                    {
                        SemesterDataCollection semesters = new SemesterDataCollection();
                        Row row = table.Rows[header.Index + 3];
                        foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                        {
                            SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                            semesters.Add(sysems);

                            if (!student.SemestersScore.Contains(sysems)) continue;
                            if (!student.HeaderList.ContainsKey(sysems)) continue;

                            int columnIndex = student.HeaderList[sysems];
                            SemesterScore semsscore = student.SemestersScore[sysems];

                            decimal? score = null;
                            decimal? weight = null;

                            if (header.IsDomain)
                            {
                                if (semsscore.Domain.Contains(header.Domain))
                                {
                                    score = semsscore.Domain[header.Domain].Value;
                                    weight = semsscore.Domain[header.Domain].Weight;
                                }
                            }
                            else
                            {
                                // 這段在顯示彈性課程成績，先註。
                                //if (semsscore.Subject.Contains(header.Subject))
                                //{
                                //    score = semsscore.Subject[header.Subject].Value;
                                //    weight = semsscore.Subject[header.Subject].Weight;
                                //}
                            }

                            if (header.Domain == LearningDomainName)
                            {
                                score = semsscore.LearnDomainScore;

                                row.Cells[columnIndex * 3 + 2].CellFormat.FitText = false;
                                row.Cells[columnIndex * 3 + 2].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[columnIndex * 3 + 3].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (!score.HasValue) continue;

                                row.Cells[columnIndex * 3 + 2].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(score.Value)));

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    RatingRow = row.NextSibling as Row;
                                    RatingRow = RatingRow.NextSibling as Row;
                                }
                            }
                            else if (header.Domain == LearningDomainNameSpcAdd)
                            {
                                // 加分後填入值
                                score = semsscore.LearnDomainScore;

                                row.Cells[columnIndex * 3 + 2].CellFormat.FitText = false;
                                row.Cells[columnIndex * 3 + 2].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[columnIndex * 3 + 3].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (!score.HasValue) continue;
                                // 處理加分
                                decimal sc = score.Value;
                                if (student.AddWeight.HasValue)
                                    sc = Math.Round(sc * student.AddWeight.Value, 2,MidpointRounding.AwayFromZero); ;

                                // 一般生填空白
                                if (string.IsNullOrEmpty(student.SpcStudTypeName))
                                {
                                    row.Cells[columnIndex * 3 + 2].Write(builder, "");
                                    row.Cells[columnIndex * 3 + 4].Write(builder, "");
                                }
                                else
                                {
                                    // 特種身
                                    row.Cells[columnIndex * 3 + 2].Write(builder, sc + "");
                                    row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(sc)));                                
                                }

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    if (RatingRow != null)
                                    {
                                        RatingRowAdd = RatingRow.NextSibling as Row;
                                       
                                       
                                    }
                                }
                            }
                            else
                            {
                                if (!score.HasValue) continue;
                                if (!weight.HasValue) weight = 0;

                                row.Cells[columnIndex * 3 + 2].Write(builder, ((double)weight) + "");
                                row.Cells[columnIndex * 3 + 3].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(score.Value)));
                            }
                        }

                        //算平均...
                        decimal? avgScore = null;
                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                avgScore = student.SemestersScore.AvgLearningDomainScore(semesters);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseLearnDomainScore(avgScore.Value);
                            }
                            else
                            {
                                avgScore = student.SemestersScore.AvgDomainScore(semesters, header.Domain);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseDomainScore(avgScore.Value);
                            }
                        }
                        else
                        {
                            // 這段在處理彈性課程平均，先註。
                            //avgScore = student.SemestersScore.AvgSubjectScore(semesters, header.Subject);

                            //if (!avgScore.HasValue) continue;

                            //if (student.CalculationRule == null)
                            //    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                            //else
                            //    avgScore = student.CalculationRule.ParseSubjectScore(avgScore.Value);
                        }

                        if (avgScore.HasValue)
                        {
                            row.Cells[20].Write(builder, (double)avgScore + "");
                            row.Cells[21].Write(builder, Util.GetDegree(avgScore.Value));
                            //decimal scAddScore = 0;
                            //// 特種身分
                            //if (student.AddWeight.HasValue)
                            //{
                            //    Row RowSp = row.NextSibling as Row;
                            //    scAddScore = student.AddWeight.Value * avgScore.Value;
                            //    RowSp.Cells[20].Write(builder, (double)scAddScore + "");
                            //    RowSp.Cells[21].Write(builder, Util.GetDegree(scAddScore));                               
                            
                            //}
                        }
                    }

                    // 處理特種分身平均顯示
                    decimal scAddScore = 0;
                    // 特種身分
                    int rrIdx = 0;
                    foreach(RowHeader rh in indexHeaders)
                    {
                        if (rh.Domain == LearningDomainNameSpcAdd)
                        {
                            rrIdx = rh.Index + 3;
                            break;
                        }
                    }

                    if (student.AddWeight.HasValue && rrIdx >0)
                    {
                        // 顯示平均
                        if (student.Places.NS("年排名").Contains("學習領域"))
                        {
                            scAddScore = Math.Round(student.Places.NS("年排名")["學習領域"].Score * student.AddWeight.Value,2,MidpointRounding.AwayFromZero);
                            table.Rows[rrIdx].Cells[20].Write(builder, (double)scAddScore + "");
                            table.Rows[rrIdx].Cells[21].Write(builder, Util.GetDegree(scAddScore));                       
                        }


                    }

                    // 處理年排名與百分比
                    if (RatingRow != null)
                    {
                        PlaceCollection places = student.Places.NS("年排名");                       

                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);

                            if (raw == SemesterData.Empty) continue;

                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            Cell datacell = RatingRow.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            bool UseRatingRank = true;
                            // 處理已有年排名(在UDT有存資料)
                            if (uddList.Count > 0 && student.LastEnterSemester.HasValue)
                            {
                                foreach (DAL.UserDefData udd in uddList)
                                {
                                    if (udd.SchoolYear == semsIndex.SchoolYear && udd.Semester == semsIndex.Semester)
                                    {
                                        datacell.Write(builder, GetPlaceString2(udd.GradeRank, udd.GradeRankPercent));
                                        UseRatingRank = false;
                                    }
                                }
                            }

                            // 判斷轉入生
                            if (student.LastEnterGradeYear.HasValue && student.LastEnterSchoolyear.HasValue)
                            {
                                if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                {
                                   
                                        bool clear = true;
                                        
                                        // 當有資料
                                        foreach (DAL.UserDefData udd in (from data in uddList where data.SchoolYear== semsIndex.SchoolYear && data.Semester== semsIndex.Semester select data))
                                                clear = false;

                                        if (clear)
                                        {
                                            // 和異動同年同學期不動
                                            if (semsIndex.SchoolYear == student.LastEnterSchoolyear && semsIndex.Semester == student.LastEnterSemester)
                                                clear = false;
                                        }

                                        if (clear)
                                        {
                                            // 當同一學年第2學期，如果維持不清空
                                            if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester == 2)
                                                clear = false;
                                        }


                                        if (clear)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                   
                                }


                                if (uddList.Count == 0)
                                {
                                    if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                    {
                                        if (semsIndex.SchoolYear < student.LastEnterSchoolyear.Value)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                        else if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester < student.LastEnterSemester)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                    }
                                }
                            }

                            // 使用即時運算排名與百分比
                            if(UseRatingRank)
                                if (places.Contains(placeKey))
                                    datacell.Write(builder, GetPlaceString(places, placeKey));                                     
                        }
                        
                        // 顯示平均
                        if (places.Contains("學習領域"))
                            RatingRow.Cells[20].Write(builder, GetPlaceString(places, "學習領域"));
                    }

                    int? LevelAdd=null,PercentageAdd=null;
                    // 處理年排名與百分比(加分後)
                    if (RatingRowAdd != null)
                    {
                        PlaceCollection places = student.Places.NS("年排名");

                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);
                            
                            if (raw == SemesterData.Empty) continue;
                            
                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            //if (!places.Contains(placeKey))
                            //    continue;

                            Cell datacell = RatingRowAdd.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            // 如果是一般生直接填空白
                            if (string.IsNullOrEmpty(student.SpcStudTypeName))
                            {
                                datacell.Write(builder, "");
                                continue;
                            }
                            int Level=1,Percentage=1;

                            // 處理加分後
                            if (places.Contains(placeKey))
                            {
                                if (student.AddWeight.HasValue)
                                {
                                    List<Place> PList = new List<Place>();
                                    decimal sc = places[placeKey].Score * student.AddWeight.Value;
                                    if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(student.GradeYear))
                                    {
                                        if (DAL.DALTransfer.StudRankScoreDict[student.GradeYear].ContainsKey(placeKey))
                                        {
                                            PList = (from data in DAL.DALTransfer.StudRankScoreDict[student.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                            if (PList.Count > 0)
                                            {
                                                PList.OrderBy(x => x.Score);
                                                Level = PList[0].Level;
                                                Percentage = PList[0].Percentage;
                                            }
                                            else
                                            {
                                                Level = 1;
                                                Percentage = 1;
                                            }
                                        }
                                    }
                                }
                            }
                            

                            bool UseRatingRank = true;
                            // 處理已有年排名(UDT)
                            if (uddList.Count > 0 && student.LastEnterSemester.HasValue)
                            {
                                foreach (DAL.UserDefData udd in uddList)
                                {
                                    if (udd.SchoolYear == semsIndex.SchoolYear && udd.Semester == semsIndex.Semester)
                                    {
                                        datacell.Write(builder, GetPlaceString2(udd.GradeRankAdd, udd.GradeRankPercentAdd));
                                        UseRatingRank = false;
                                    }
                                }
                            }

                            // 判斷轉入生
                            if (student.LastEnterGradeYear.HasValue && student.LastEnterSchoolyear.HasValue)
                            {
                                if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                {

                                    bool clear = true;

                                    // 當有資料
                                    foreach (DAL.UserDefData udd in (from data in uddList where data.SchoolYear == semsIndex.SchoolYear && data.Semester == semsIndex.Semester select data))
                                        clear = false;

                                    if (clear)
                                    {
                                        // 和異動同年同學期不動
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear && semsIndex.Semester == student.LastEnterSemester)
                                            clear = false;
                                    }

                                    if (clear)
                                    {
                                        // 當同一學年第2學期，如果維持不清空
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester == 2)
                                            clear = false;
                                    }



                                    if (clear)
                                    {
                                        datacell.Write(builder, "");
                                        UseRatingRank = false;
                                    }

                                }


                                if (uddList.Count == 0)
                                {
                                    if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                    {
                                        if (semsIndex.SchoolYear < student.LastEnterSchoolyear.Value)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                        else if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester < student.LastEnterSemester)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                    }
                                }
                            }

                            //// 使用即時運算排名與百分比
                            if (UseRatingRank)
                            {
                                if (places.Contains(placeKey))
                                {
                                    if (student.AddWeight.HasValue)
                                        datacell.Write(builder, GetPlaceString2(Level, Percentage));
                                    else                                    
                                        datacell.Write(builder, GetPlaceString(places, placeKey));
                                      
                                }
                            }

                            //// 每學期加分後
                            //if (places.Contains(placeKey))
                            //    datacell.Write(builder, "^_^");

                        }

                        //這是加分後平均
                        //if (places.Contains(LearningDomainNameSpcAddP))                            
                        //    RatingRowAdd.Cells[20].Write(builder, GetPlaceString(places, LearningDomainNameSpcAddP));

                        
                        // 加分後
                        if (student.AddWeight.HasValue)
                        {
                            decimal sc1=0;
                            PercentageAdd = null; LevelAdd = null;
                            List<Place> PList = new List<Place>();
                            if(places.Contains("學習領域"))
                                sc1 = places["學習領域"].Score * student.AddWeight.Value;
                            if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(student.GradeYear))
                            {
                                if (DAL.DALTransfer.StudRankScoreDict[student.GradeYear].ContainsKey("學期總平均"))
                                {
                                    PList = (from data in DAL.DALTransfer.StudRankScoreDict[student.GradeYear]["學期總平均"] where data.Place.Score >= sc1 orderby data.Place.Score ascending select data.Place).ToList();
                                    if (PList.Count > 0)
                                    {
                                        PList.OrderBy(x => x.Score);
                                        LevelAdd = PList[0].Level;
                                        PercentageAdd = PList[0].Percentage;
                                    }
                                    else
                                    {
                                        LevelAdd = 1;
                                        PercentageAdd = 1;
                                    }
                                }
                                if(LevelAdd.HasValue && PercentageAdd.HasValue )
                                    RatingRowAdd.Cells[20].Write(builder, GetPlaceString2(LevelAdd.Value, PercentageAdd.Value));
                            }
                        }

                    }

                    // 樂學計分方式
                    if(RatingRowAdd !=null )
                    {
                        //string str = "單一學期學習領域成績計分=100-(名次百分比)×100+1";
                        Row RowStr = RatingRowAdd.NextSibling as Row;
                        //RowStr.Cells[2].Write(builder, str);

                        PlaceCollection places = student.Places.NS("年排名");
                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);

                            if (raw == SemesterData.Empty) continue;

                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            Cell datacell = RowStr.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            bool UseRatingRank = true;

                            // 處理已有年排名(在UDT有存資料)
                            if (uddList.Count > 0 && student.LastEnterSemester.HasValue)
                            {
                                foreach (DAL.UserDefData udd in uddList)
                                {
                                    if (udd.SchoolYear == semsIndex.SchoolYear && udd.Semester == semsIndex.Semester)
                                    {
                                        if(student.AddWeight.HasValue )
                                            datacell.Write(builder, GetPlaceString3(udd.GradeRankPercentAdd));
                                        else
                                            datacell.Write(builder, GetPlaceString3(udd.GradeRankPercent));
                                        UseRatingRank = false;
                                    }
                                }
                            }

                            // 判斷轉入生
                            if (student.LastEnterGradeYear.HasValue && student.LastEnterSchoolyear.HasValue )
                            {
                                if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                {

                                    bool clear = true;

                                    // 當有資料
                                    foreach (DAL.UserDefData udd in (from data in uddList where data.SchoolYear == semsIndex.SchoolYear && data.Semester == semsIndex.Semester select data))
                                        clear = false;

                                    if (clear)
                                    {
                                        // 和異動同年同學期不動
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear && semsIndex.Semester == student.LastEnterSemester)
                                            clear = false;
                                    }

                                    if (clear)
                                    {
                                        // 當同一學年第2學期，如果維持不清空
                                        if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester == 2)
                                            clear = false;
                                    }



                                    if (clear)
                                    {
                                        datacell.Write(builder, "");
                                        UseRatingRank = false;
                                    }

                                }

                                if (uddList.Count == 0)
                                {
                                    if (semsIndex.SchoolYear <= student.LastEnterSchoolyear.Value)
                                    {
                                        if (semsIndex.SchoolYear < student.LastEnterSchoolyear.Value)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                        else if (semsIndex.SchoolYear == student.LastEnterSchoolyear.Value && semsIndex.Semester < student.LastEnterSemester)
                                        {
                                            datacell.Write(builder, "");
                                            UseRatingRank = false;
                                        }
                                    }
                                }
                            }

                            if (UseRatingRank)
                            {

                                // 處理樂學計算
                                if (student.AddWeight.HasValue == false)
                                    if (places.Contains(placeKey))
                                        datacell.Write(builder, GetPlaceString3(places[placeKey].Percentage));


                                int Level = 1, Percentage = 1;

                                // 處理加分後
                                if (places.Contains(placeKey))
                                {
                                    if (student.AddWeight.HasValue)
                                    {
                                        List<Place> PList = new List<Place>();
                                        decimal sc = places[placeKey].Score * student.AddWeight.Value;
                                        if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(student.GradeYear))
                                        {
                                            if (DAL.DALTransfer.StudRankScoreDict[student.GradeYear].ContainsKey(placeKey))
                                            {
                                                PList = (from data in DAL.DALTransfer.StudRankScoreDict[student.GradeYear][placeKey] where data.Place.Score >= sc orderby data.Place.Score ascending select data.Place).ToList();
                                                if (PList.Count > 0)
                                                {
                                                    PList.OrderBy(x => x.Score);
                                                    Level = PList[0].Level;
                                                    Percentage = PList[0].Percentage;
                                                }
                                                else
                                                {
                                                    Level = 1;
                                                    Percentage = 1;
                                                }
                                            }
                                        }
                                        datacell.Write(builder, GetPlaceString3(Percentage));
                                    }
                                }
                            }

                        }

                        //// 顯示平均
                        //Place p;
                        //if (places.Contains("學習領域"))
                        //{
                            //if (student.AddWeight.HasValue)
                            //{
                            //    if(PercentageAdd.HasValue )
                            //        RowStr.Cells[20].Write(builder, GetPlaceString3(PercentageAdd.Value));
                            //}
                            //else
                            //{
                        //        p = places["學習領域"];
                        //        RowStr.Cells[20].Write(builder, GetPlaceString3(p.Percentage));
                        //    //}
                        //}
                    }
                    #endregion

                    #region 合併相關欄位。
                    string previousCellDomain = string.Empty;
                    foreach (RowHeader header in indexHeaders)
                    {
                        bool hasGroup = !string.IsNullOrEmpty(Subj.GetDomainGroup(header.Domain));
                        string groupName = Subj.GetDomainGroup(header.Domain);

                        Row row = table.Rows[header.Index + 3];

                        if (previousCellDomain == row.Cells[0].ToTxt())
                            row.Cells[0].CellFormat.VerticalMerge = CellMerge.Previous;
                        else
                            row.Cells[0].CellFormat.VerticalMerge = CellMerge.First;

                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                #region 學習領域
                                // 這段在處理 header
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;

                                #endregion
                            }
                            else if (header.Domain == LearningDomainNameSpcAdd)
                            {
                                #region 學習領域(加分後)
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;

                                
                                #endregion                            
                            }
                            else if (header.Domain == LearningDomainNameP)
                            {
                                // 學成成績排名與百分比
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {

                                    //Row lrow = row.NextSibling as Row;
                                    Row lrow = row;
                                    lrow.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                    lrow.Cells[0].CellFormat.FitText = false;
                                    lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                    Paragraph mp = lrow.Cells[2].Paragraphs[0];
                                    for (int i = 0; i < (3 * 6); i++)
                                    {
                                        if (i % 3 == 0)
                                        {
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                            (mp.ParentNode as Cell).CellFormat.FitText = false;
                                        }
                                        else
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                        mp = Util.NextCell(mp as Paragraph);
                                    }
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                    mp = Util.NextCell(mp as Paragraph);
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                                }

                            }
                            else if (header.Domain == LearningDomainNameSpcAddP)
                            {
                                // 學習成績排名與百分比(加分後)
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    //Row lrow = row.NextSibling as Row;

                                    Row lrow = row;
                                    lrow.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                    lrow.Cells[0].CellFormat.FitText = false;
                                    lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                    Paragraph mp = lrow.Cells[2].Paragraphs[0];
                                    for (int i = 0; i < (3 * 6); i++)
                                    {
                                        if (i % 3 == 0)
                                        {
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                            (mp.ParentNode as Cell).CellFormat.FitText = false;
                                        }
                                        else
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                        mp = Util.NextCell(mp as Paragraph);
                                    }
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                    mp = Util.NextCell(mp as Paragraph);
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                                }

                            }
                            else if (header.Domain==CalcMessage)
                            {
                                // 文字字串
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                Row lrow = row;
                                lrow.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                lrow.Cells[0].CellFormat.FitText = false;
                                lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                Paragraph mp = lrow.Cells[2].Paragraphs[0];
                                for (int i = 0; i < (3 * 6); i++)
                                {
                                   // if (i % 18 == 0)
                                        if (i % 3 == 0)
                                        {
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                            (mp.ParentNode as Cell).CellFormat.FitText = false;
                                        }
                                        else
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                    mp = Util.NextCell(mp as Paragraph);
                                }
                                (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                mp = Util.NextCell(mp as Paragraph);
                                (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                            }
                            else
                            {
                                if (!hasGroup)
                                {
                                    row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                    row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                }
                            }
                        }

                        previousCellDomain = row.Cells[0].ToTxt();
                    }
                    #endregion

                    #endregion
                }
                else if (PrintSetting.ListMethod == ListMethod.SubjectOnly)
                {
                    #region 列印科目
                    UniqueSet<RowHeader> RowIndexs = new UniqueSet<RowHeader>();

                    foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                    {
                        SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                        if (!student.SemestersScore.Contains(sysems)) continue;

                        SemesterScore semsscore = student.SemestersScore[sysems];

                        foreach (string strSubject in semsscore.Subject)
                        {
                            SemesterSubjectScore subject = semsscore.Subject[strSubject];

                            RowHeader header;
                            if (IsFlexible(subject.Domain))
                                header = new RowHeader("彈性課程", strSubject);
                            else
                                header = new RowHeader(subject.Domain, strSubject);

                            header.IsDomain = false;

                            if (!RowIndexs.Contains(header))
                                RowIndexs.Add(header);
                        }
                    }

                    RowHeader lheader = new RowHeader(LearningDomainName, string.Empty);
                    lheader.IsDomain = true;
                    //RowIndexs.Add(lheader);

                    //List<RowHeader> sortedHeaders = SortHeader(RowIndexs.ToList());
                    List<RowHeader> sortedHeaders = RowIndexs.ToList();

                    sortedHeaders.Sort(delegate(RowHeader x, RowHeader y)
                    {
                        Subj xx = new JHSchool.Evaluation.Subject(x.Subject, x.Domain);
                        Subj yy = new JHSchool.Evaluation.Subject(y.Subject, y.Domain);

                        return xx.CompareTo(yy);
                    });
                    //sortedHeaders.Sort(Util.SortSubject);
                    //sortedHeaders.Sort(Util.SortDomain);

                    //把學習領域放在正確的地方。
                    foreach (RowHeader eachHeader in sortedHeaders.ToArray())
                    {
                        if (IsFlexible(eachHeader.Domain))
                        {
                            int index = sortedHeaders.IndexOf(eachHeader);
                            sortedHeaders.Insert(index, lheader);
                            break;
                        }
                    }
                    if (sortedHeaders.IndexOf(lheader) < 0)
                        sortedHeaders.Add(lheader);

                    //產生 Row。
                    List<RowHeader> indexHeaders = new List<RowHeader>();
                    Row current = template;
                    int rowIndex = 0;
                    foreach (RowHeader header in sortedHeaders)
                    {
                        RowHeader indexH = header;
                        indexH.Index = rowIndex++;
                        indexHeaders.Add(indexH);

                        Row datarow = table.InsertBefore(template.Clone(true), current) as Row;

                        if (header.Domain == LearningDomainName)
                        {
                            string headerName = string.Empty;

                            if (PrintSetting.PrintRank)
                                headerName = "學習領域排名";

                            if (PrintSetting.PrintRankPercentage)
                                headerName = "學習領域百分比";

                            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                                headerName = "學習領域排名/百分比";

                            if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                            {
                                Row lrow = table.InsertAfter(template.Clone(true), datarow) as Row;
                                lrow.Cells[0].Write(builder, headerName);
                                rowIndex++;
                            }
                        }

                        if (IsFlexible(header.Domain))
                        {
                            datarow.Cells[0].Write(builder, header.Domain);
                            datarow.Cells[1].Write(builder, header.Subject);
                        }
                        else if (header.Domain == LearningDomainName)
                            datarow.Cells[0].Write(builder, header.Domain);
                        else
                            datarow.Cells[0].Write(builder, header.Subject);
                    }

                    //填資料
                    Row RatingRow = null;
                    foreach (RowHeader header in indexHeaders)
                    {
                        SemesterDataCollection semesters = new SemesterDataCollection();
                        Row row = table.Rows[header.Index + 3];
                        foreach (SemesterData semester in student.SHistory.GetGradeYearSemester().GetSemesters(PrintSetting.PrintSemesters))
                        {
                            SemesterData sysems = new SemesterData(0, semester.SchoolYear, semester.Semester);
                            semesters.Add(sysems);

                            if (!student.SemestersScore.Contains(sysems)) continue;
                            if (!student.HeaderList.ContainsKey(sysems)) continue;

                            int columnIndex = student.HeaderList[sysems];
                            SemesterScore semsscore = student.SemestersScore[sysems];

                            decimal? score = null;
                            decimal? weight = null;

                            if (header.IsDomain)
                            {
                                if (semsscore.Domain.Contains(header.Domain))
                                {
                                    score = semsscore.Domain[header.Domain].Value;
                                    weight = semsscore.Domain[header.Domain].Weight;
                                }
                            }
                            else
                            {
                                if (semsscore.Subject.Contains(header.Subject))
                                {
                                    score = semsscore.Subject[header.Subject].Value;
                                    weight = semsscore.Subject[header.Subject].Weight;
                                }
                            }

                            if (header.Domain == LearningDomainName)
                            {
                                score = semsscore.LearnDomainScore;

                                row.Cells[columnIndex * 3 + 2].CellFormat.FitText = false;
                                row.Cells[columnIndex * 3 + 2].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[columnIndex * 3 + 3].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (!score.HasValue) continue;

                                row.Cells[columnIndex * 3 + 2].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(score.Value)));

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                    RatingRow = row.NextSibling as Row;
                            }
                            else
                            {
                                if (!score.HasValue) continue;
                                if (!weight.HasValue) weight = 0;

                                row.Cells[columnIndex * 3 + 2].Write(builder, ((double)weight) + "");
                                row.Cells[columnIndex * 3 + 3].Write(builder, ((double)score) + "");
                                row.Cells[columnIndex * 3 + 4].Write(builder, (Util.GetDegree(score.Value)));
                            }
                        }

                        //算平均...
                        decimal? avgScore = null;
                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                avgScore = student.SemestersScore.AvgLearningDomainScore(semesters);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseLearnDomainScore(avgScore.Value);
                            }
                            else
                            {
                                avgScore = student.SemestersScore.AvgDomainScore(semesters, header.Domain);

                                if (!avgScore.HasValue) continue;

                                if (student.CalculationRule == null)
                                    avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                                else
                                    avgScore = student.CalculationRule.ParseDomainScore(avgScore.Value);
                            }
                        }
                        else
                        {
                            avgScore = student.SemestersScore.AvgSubjectScore(semesters, header.Subject);

                            if (!avgScore.HasValue) continue;

                            if (student.CalculationRule == null)
                                avgScore = Math.Round(avgScore.Value, 2, MidpointRounding.AwayFromZero);
                            else
                                avgScore = student.CalculationRule.ParseSubjectScore(avgScore.Value);
                        }

                        row.Cells[20].Write(builder, (double)avgScore + "");
                        row.Cells[21].Write(builder, Util.GetDegree(avgScore.Value));
                    }
                    if (RatingRow != null)
                    {
                        PlaceCollection places = student.Places.NS("年排名");

                        foreach (SemesterData semsIndex in student.HeaderList.Keys)
                        {
                            SemesterData raw = student.HeaderList.GetSRaw(semsIndex);

                            if (raw == SemesterData.Empty) continue;

                            string placeKey = SLearningDomainParser.GetSemesterString(raw);

                            Cell datacell = RatingRow.Cells[student.HeaderList[semsIndex] * 3 + 2];

                            if (places.Contains(placeKey))
                                datacell.Write(builder, GetPlaceString(places, placeKey));
                        }

                        if (places.Contains(LearningDomainName))
                            RatingRow.Cells[20].Write(builder, GetPlaceString(places, LearningDomainName));
                    }

                    //合併相關欄位。
                    string previousCellDomain = string.Empty;
                    foreach (RowHeader header in indexHeaders)
                    {
                        Row row = table.Rows[header.Index + 3];

                        if (header.IsDomain)
                        {
                            if (header.Domain == LearningDomainName)
                            {
                                row.Cells[0].CellFormat.FitText = false;
                                row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;

                                if (PrintSetting.PrintRank || PrintSetting.PrintRankPercentage)
                                {
                                    Row lrow = row.NextSibling as Row;
                                    lrow.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                                    lrow.Cells[0].CellFormat.FitText = false;
                                    lrow.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                                    Paragraph mp = lrow.Cells[2].Paragraphs[0];
                                    for (int i = 0; i < (3 * 6); i++)
                                    {
                                        if (i % 3 == 0)
                                        {
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                            (mp.ParentNode as Cell).CellFormat.FitText = false;
                                        }
                                        else
                                            (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;

                                        mp = Util.NextCell(mp as Paragraph);
                                    }
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.First;
                                    mp = Util.NextCell(mp as Paragraph);
                                    (mp.ParentNode as Cell).CellFormat.HorizontalMerge = CellMerge.Previous;
                                }
                            }
                        }
                        else if (IsFlexible(header.Domain))
                        {
                            if (previousCellDomain == header.Domain)
                                row.Cells[0].CellFormat.VerticalMerge = CellMerge.Previous;
                            else
                                row.Cells[0].CellFormat.VerticalMerge = CellMerge.First;

                            previousCellDomain = header.Domain;
                        }
                        else
                        {
                            //row.Cells[0].CellFormat.FitText = true;
                            row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
                            row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
                        }
                    }
                    #endregion
                }

                template.NextSibling.Remove();
                template.Remove();
                #endregion
            }
            else if (e.FieldName == "Fix:缺曠獎懲")
            {
                #region 列印獎懲資料
                int Offset = 2;
                Row MeritA = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                Row MeritB = MeritA.NextSibling as Row;
                Row MeritC = MeritB.NextSibling as Row;
                Row DemeritA = MeritC.NextSibling as Row;
                Row DemeritB = DemeritA.NextSibling as Row;
                Row DemeritC = DemeritB.NextSibling as Row;
                Row DisciplineSet = DemeritC.NextSibling as Row;
                Row DisciplineNormal = DisciplineSet.NextSibling as Row;

                foreach (SemesterData each in student.Summaries.Keys)
                {
                    XmlElement summary = student.Summaries[each];

                    if (!student.HeaderList.ContainsKey(each)) continue;

                    int ColumnIndex = student.HeaderList[each];

                    XmlElement xmlmerit = summary.SelectSingleNode("DisciplineStatistics/Merit") as XmlElement;
                    XmlElement xmldemerit = summary.SelectSingleNode("DisciplineStatistics/Demerit") as XmlElement;

                    if (xmlmerit != null)
                    {
                        if (!string.IsNullOrEmpty(GetString(xmlmerit.GetAttribute("A"))))
                            MeritA.Cells[Offset + ColumnIndex].Write(builder, GetString(xmlmerit.GetAttribute("A")));
                        if (!string.IsNullOrEmpty(GetString(xmlmerit.GetAttribute("B"))))
                            MeritB.Cells[Offset + ColumnIndex].Write(builder, GetString(xmlmerit.GetAttribute("B")));
                        if (!string.IsNullOrEmpty(GetString(xmlmerit.GetAttribute("C"))))
                            MeritC.Cells[Offset + ColumnIndex].Write(builder, GetString(xmlmerit.GetAttribute("C")));
                    }

                    if (xmldemerit != null)
                    {
                        if (!string.IsNullOrEmpty(GetString(xmldemerit.GetAttribute("A"))))
                            DemeritA.Cells[Offset + ColumnIndex].Write(builder, GetString(xmldemerit.GetAttribute("A")));
                        if (!string.IsNullOrEmpty(GetString(xmldemerit.GetAttribute("B"))))
                            DemeritB.Cells[Offset + ColumnIndex].Write(builder, GetString(xmldemerit.GetAttribute("B")));
                        if (!string.IsNullOrEmpty(GetString(xmldemerit.GetAttribute("C"))))
                            DemeritC.Cells[Offset + ColumnIndex].Write(builder, GetString(xmldemerit.GetAttribute("C")));
                    }

                    StringBuilder normalString = new StringBuilder();
                    StringBuilder setString = new StringBuilder();

                    foreach (XmlElement absence in summary.SelectNodes("AttendanceStatistics/Absence"))
                    {
                        string count = absence.GetAttribute("Count");
                        string periodType = absence.GetAttribute("PeriodType");
                        string periodName = absence.GetAttribute("Name");

                        if (string.IsNullOrEmpty(count)) continue;
                        if (!PrintAbsences.ContainsKey(periodType)) continue;
                        if (!PrintAbsences[periodType].Contains(periodName)) continue;

                        if (periodType == "一般")
                        {
                            if (normalString.Length > 0) normalString.AppendLine();
                            normalString.Append(periodName + "：" + count);
                        }
                        else if (periodType == "集會")
                        {
                            if (setString.Length > 0) setString.AppendLine();
                            setString.Append(periodName + "：" + count);
                        }
                    }

                    DisciplineNormal.Cells[Offset + ColumnIndex].Write(builder, normalString.ToString());
                    DisciplineSet.Cells[Offset + ColumnIndex].Write(builder, setString.ToString());
                }
                #endregion
            }
        }

        private void RefineDomain(SemesterSubjectScore subject)
        {
            if (subject.Domain.Trim() == "彈性課程")
                subject.Domain = string.Empty;
        }

        private bool IsFlexible(string domainName)
        {
            if (string.IsNullOrEmpty(domainName))
                return true;

            if (domainName == "彈性課程")
                return true;

            return false;
        }

        /// <summary>
        /// 將領域、彈性課程、學習領域分開排序。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<RowHeader> SortHeader(List<RowHeader> list)
        {
            List<RowHeader> domains = new List<RowHeader>();
            List<RowHeader> subjects = new List<RowHeader>();
            RowHeader? ldomain = null;
            RowHeader? ldomain1 = null;
            RowHeader? ldomain2 = null;
            RowHeader? ldomain3 = null;
            RowHeader? ldomain4 = null;

            foreach (RowHeader each in list)
            {
                if (each.Domain == LearningDomainName)
                {
                    ldomain = each;
                    continue;
                }

                if (each.Domain == LearningDomainNameSpcAdd)
                {
                    ldomain1 = each;
                    continue;
                }

                if (each.Domain == LearningDomainNameP)
                {
                    ldomain2 = each;
                    continue;
                }

                if (each.Domain == LearningDomainNameSpcAddP)
                {
                    ldomain3 = each;
                    continue;
                }

                if (each.Domain == CalcMessage)
                {
                    ldomain4 = each;
                    continue;                
                }

                if (Program.Mode == ModuleMode.HsinChu) //新竹..... = =''
                {
                    if (each.IsDomain || each.Domain == "語文")
                        domains.Add(each);
                    else
                        subjects.Add(each);
                }
                else
                {
                    if (each.IsDomain)
                        domains.Add(each);
                    else
                        subjects.Add(each);
                }
            }
            domains.Sort(delegate(RowHeader x, RowHeader y)
            {
                Subj xx = new JHSchool.Evaluation.Subject(x.Subject, x.Domain);
                Subj yy = new JHSchool.Evaluation.Subject(y.Subject, y.Domain);

                return xx.CompareTo(yy);
            });

            subjects.Sort((x, y) => (Subj.CompareSubjectOrdinal(x.Subject, y.Subject)));

            List<RowHeader> result = new List<RowHeader>();
            result.AddRange(domains);
            if (ldomain != null) result.Add(ldomain.Value);
            if (ldomain1 != null) result.Add(ldomain1.Value);
            if (ldomain2 != null) result.Add(ldomain2.Value);
            if (ldomain3 != null) result.Add(ldomain3.Value);
            if (ldomain4 != null) result.Add(ldomain4.Value);
            result.AddRange(subjects);

            return result;
        }

        private string GetPlaceString(PlaceCollection places, string placeKey)
        {
             Place place = places[placeKey];
            //decimal percentage = (100m * ((decimal)place.Level / (decimal)place.Radix));
            

            ////小於1%的話，就是1%。
            //if (percentage < 1) percentage = 1;

            string result = string.Empty;

            if (PrintSetting.PrintRank)
                result = place.Level.ToString();

            if (PrintSetting.PrintRankPercentage)
                result = place.Percentage + "%";
                //result = Math.Round(percentage, 0, MidpointRounding.AwayFromZero) + "%";

            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                result = string.Format("{0}/{1}%", place.Level, place.Percentage);
                //result = string.Format("{0}/{1}%", place.Level, Math.Round(percentage, 0, MidpointRounding.AwayFromZero));

            return result;
        }

        /// <summary>
        /// 處理加分後
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="Percentage"></param>
        /// <returns></returns>
        private string GetPlaceString2(int Level, int Percentage)
        {
            string result = string.Empty;
            if (PrintSetting.PrintRank)
                result = Level.ToString();

            if (PrintSetting.PrintRankPercentage)
                result = Percentage + "%";
            if (PrintSetting.PrintRank && PrintSetting.PrintRankPercentage)
                result = string.Format("{0}/{1}%", Level, Percentage);
            return result;
        }

        /// <summary>
        /// 處理樂學計算
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="Percentage"></param>
        /// <returns></returns>
        private string GetPlaceString3(int Percentage)
        {
            string result = string.Empty;

            int num = 0;
            // 公式：100-名次百分比+1
            num = 100 - Percentage + 1;
            if (num >0)
                result = string.Format("{0}", num);
            
            return result;
        }


        private static string GetString(string number)
        {
            int output;

            if (int.TryParse(number, out output))
                if (output <= 0) return string.Empty;

            return number;
        }

        private static string GetText(Paragraph originParagraph)
        {
            return originParagraph.ToTxt().Replace("«Fix:年級學期»", "").Replace("\r\n", "");
        }

        private static string GetSemesterString(SemesterData each)
        {
            return string.Format("{0} {1}", each.SchoolYear.ToString(), each.Semester == 1 ? "上" : "下");
        }

        private class MergeDataSource : IMailMergeDataSource
        {
            public List<ReportStudent> Students { get; private set; }

            private ReportPreference Preference { get; set; }

            private int Index { get; set; }

            private string PrintDate = DateTime.Now.ToString("yyyy/MM/dd");

            public MergeDataSource(List<ReportStudent> students, ReportPreference preference)
            {
                Students = students;
                Preference = preference;
                Index = -1;
            }

            #region IMailMergeDataSource 成員

            public bool GetValue(string fieldName, out object fieldValue)
            {
                fieldValue = string.Empty;
                ReportStudent student = Students[Index];

                if (fieldName.ToUpper().StartsWith("Fix:".ToUpper()))
                {
                    fieldValue = student;
                    return true;
                }

                switch (fieldName)
                {
                    case "學號":
                        fieldValue = student.StudentNumber;
                        break;
                    case "班級":
                        fieldValue = student.ClassName;
                        break;
                    case "座號":
                        fieldValue = student.SeatNo;
                        break;
                    case "姓名":
                        fieldValue = student.Name;
                        break;
                    case "性別":
                        fieldValue = student.Gender;
                        break;
                    case "生日":
                        DateTime dtBirthday;

                        if (DateTime.TryParse(student.Birthday, out dtBirthday))
                            fieldValue = dtBirthday.Year - 1911 + "/" + dtBirthday.Month + "/" + dtBirthday.Day;
                        else
                            fieldValue = string.Empty;

                        break;
                    case "列印日期":
                        DateTime dtPrintDate;

                        if (DateTime.TryParse(PrintDate, out dtPrintDate))
                            fieldValue = dtPrintDate.Year - 1911 + "/" + dtPrintDate.Month + "/" + dtPrintDate.Day;
                        else
                            fieldValue = string.Empty;
                        break;
                    case "學校名稱":
                        fieldValue = K12.Data.School.ChineseName;
                        break;
                    case "入學日期":
                        if (string.IsNullOrEmpty(Preference.EntranceDate))
                            fieldValue = student.EntranceDate;
                        else
                            fieldValue = Preference.EntranceDate;
                        break;
                    case "畢業日期":
                        if (string.IsNullOrEmpty(Preference.GraduateDate))
                            fieldValue = student.GraduateDate;
                        else
                            fieldValue = Preference.GraduateDate;
                        break;
                    //case "特種身分類別":
                    //    if (string.IsNullOrEmpty(student.SpcStudTypeName))
                    //        fieldValue = "";
                    //    else
                    //        fieldValue = student.SpcStudTypeName;
                    //    break;

                    case "轉入異動日期":
                        if (string.IsNullOrEmpty(student.TransUpdateDateStr))
                            fieldValue = "";
                        else
                            fieldValue = student.TransUpdateDateStr;
                        break;

                    case "特身名稱1":                        
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□"+StudSpcName1 ;
                        }
                        else 
                        {
                            if (StudSpcName1.Trim() == student.SpcStudTypeName.Trim ())
                                fieldValue = "■" + StudSpcName1;
                            else
                                fieldValue = "□"+StudSpcName1;
                        }
                            break;

                    case "特身名稱2":

                            if (string.IsNullOrEmpty(student.SpcStudTypeName))
                            {
                                fieldValue = "□"+StudSpcName2 ;
                            }
                            else
                            {
                                if (StudSpcName2.Trim() == student.SpcStudTypeName.Trim())
                                    fieldValue = "■" + StudSpcName2;
                                else
                                    fieldValue = "□" + StudSpcName2;

                            }

                        break;

                    case "特身名稱3":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□"+StudSpcName3;
                        }
                        else
                        {
                            if (StudSpcName3.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName3;
                            else
                                fieldValue = "□" + StudSpcName3;

                        }

                        break;

                    case "特身名稱4":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                               fieldValue = "□"+StudSpcName4 ;
                        }
                        else
                        {
                            if (StudSpcName4.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName4;
                            else
                                fieldValue = "□" + StudSpcName4;

                        }

                        break;

                    case "特身名稱5":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□"+StudSpcName5 ;
                        }
                        else
                        {
                            if (StudSpcName5.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName5;
                            else
                                fieldValue = "□" + StudSpcName5;

                        }

                        break;

                    case "特身名稱6":
                        if (string.IsNullOrEmpty(student.SpcStudTypeName))
                        {
                            fieldValue = "□"+StudSpcName6;
                        }
                        else
                        {
                            if (StudSpcName6.Trim() == student.SpcStudTypeName.Trim())
                                fieldValue = "■" + StudSpcName6;
                            else
                                fieldValue = "□" + StudSpcName6;

                        }

                        break;

                }

                return true;
            }

            public bool MoveNext()
            {
                Index++;

                return (Index < Students.Count);
            }

            public string TableName { get { return string.Empty; } }

            #endregion
        }
    }
}
