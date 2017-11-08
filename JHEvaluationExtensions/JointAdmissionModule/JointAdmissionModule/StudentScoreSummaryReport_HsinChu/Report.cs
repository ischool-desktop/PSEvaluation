using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Cells;
using Campus.Rating;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;

namespace JointAdmissionModule.StudentScoreSummaryReport_HsinChu
{
    internal class Report
    {
        private List<ReportStudent> Students { get; set; }

        private List<string> DetailDomain { get; set; }

        /// <summary>
        /// 檢查是否產生匯入格式檔案
        /// </summary>
        public static bool checkOutputImportFile = false;
        public static bool checkWst3aa = false;
        public static bool checkWst3ab = false;
        public static string checkAddressType = "";

        // 學生名次與名次百分比(UDT)
        private Dictionary<string, List<DAL.UserDefData>> _StudRankData;

        private const string LearningDomainName = "學期成績平均";
        private const string LearningDomainNameSpcAdd = "特種身分加分後之學期成績平均";
        private const string LearningDomainNameP = "學期成績平均百分比";
        private const string LearningDomainNameSpcAddP = "特種身分加分後之學期成績平均百分比";
        private const string CalcMessage = "計分方式";
        private const string StudSpcName1 = "原住民(持文化及語言能力證明)";
        private const string StudSpcName2 = "原住民(未持文化及語言能力證明)";
        private const string StudSpcName3 = "境外優秀科學技術人才子女";
        private const string StudSpcName4 = "政府派赴國外工作人員子女";
        private const string StudSpcName5 = "蒙藏生";
        private const string StudSpcName6 = "身心障礙生";

        public Report(List<ReportStudent> students,string ReportTitle,ModuleMode CurrentMode)
        {
            Students = students;
            Students.Sort(delegate(ReportStudent x, ReportStudent y)
            {
                return x.OrderString.CompareTo(y.OrderString);
            });

            // 取得學生ID
            List<string> sidList = (from xx in students select xx.StudentID).ToList();
            // 取得學生放在UDT的排名資料
            _StudRankData = DAL.UDTTransfer.GetDataFromUDTDict(sidList);
            List<StudentAvgScoreRank5Rpt> rptStudList = StudentAvgScoreRank5RptParse(students);

            // 放特種身分
            List<StudentAvgScoreRank5Rpt> rptStudListSpec = new List<StudentAvgScoreRank5Rpt>();
            foreach (StudentAvgScoreRank5Rpt stud in rptStudList)
            {
                if (stud.AddWeight.HasValue && stud.StudSpecType != "")
                    rptStudListSpec.Add(stud);            
            }

            // 讀取領域名稱
            List<string> DomainNameList = DAL.DALTransfer.GetDomainNameList();

            #region 一般生
            // 讀取樣板
            Workbook template = new Workbook();
            Workbook wb = new Workbook();
            Worksheet wst;
            byte[] ReportSource = null;
            
            if (CurrentMode == ModuleMode.Tainan)
                ReportSource = Properties.Resources._100學年度國中在校成績證明單_台南一般;
            else if (CurrentMode == ModuleMode.Taichung)
                ReportSource = Properties.Resources._100學年度國中在校成績證明單_中投一般;
            else 
                ReportSource = Properties.Resources._100學年度國中在校成績證明單_竹苗一般;

            wb.Open(new MemoryStream(ReportSource));
            template.Open(new MemoryStream(ReportSource));
            template.Worksheets[0].Cells[1, 0].PutValue(ReportTitle);
            wb.Worksheets[0].Cells[1, 0].PutValue(ReportTitle);
            wst = wb.Worksheets[0];
            Range all = template.Worksheets.GetRangeByName("All");
            int RowNumber = all.RowCount;
            int RowIndex = RowNumber;

            // 檢查並計算人數是否超過 sheet 可放數量
            if ((RowNumber * rptStudList.Count) > 65535)
                return ;

            // 複製樣板 當超過1人才複製。
            if(rptStudList.Count>1) 
            for (int i = 1; i < rptStudList.Count; i++)
            {
                wst.Cells.CreateRange(RowIndex, RowNumber, false).Copy(all);
                // 插入分頁
                wst.HPageBreaks.Add(RowIndex, 0);
                RowIndex += RowNumber;                
            }

            int DataRow = 0;

            if (CurrentMode == ModuleMode.HsinChu)
            {
                #region 新竹一般
                // 填入學生值 普通
                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
                {
                    // 就讀國中                 
                    wst.Cells[DataRow + 2, 0].PutValue(wst.Cells[DataRow + 2, 0].StringValue + sasr5r.SchoolName);
                    // 就讀國中代碼
                    wst.Cells[DataRow + 2, 4].PutValue(wst.Cells[DataRow + 2, 4].StringValue + sasr5r.SchoolCode);
                    // 班級
                    wst.Cells[DataRow + 3, 0].PutValue(wst.Cells[DataRow + 3, 0].StringValue + sasr5r.ClassName);
                    // 姓名
                    wst.Cells[DataRow + 3, 2].PutValue(wst.Cells[DataRow + 3, 2].StringValue + sasr5r.Name);
                    // 身分證統一編號
                    wst.Cells[DataRow + 3, 4].PutValue(wst.Cells[DataRow + 3, 4].StringValue + sasr5r.IDNumber);
                    // 領域成績
                    string KeyName = ""; int RowMove = 0;
                    foreach (string DomainName in DomainNameList)
                    {
                        switch (DomainName)
                        {
                            case "國語文":
                                KeyName = "國語文";
                                RowMove = 5;
                                break;
                            case "英語":
                                KeyName = "英語";
                                RowMove = 6;
                                break;
                            case "數學":
                                KeyName = "數學";
                                RowMove = 7;
                                break;
                            case "社會":
                                KeyName = "社會";
                                RowMove = 8;
                                break;

                            case "自然與生活科技":
                                KeyName = "自然與生活科技";
                                RowMove = 9;
                                break;
                            case "藝術與人文":
                                KeyName = "藝術與人文";
                                RowMove = 10;
                                break;
                            case "健康與體育":
                                KeyName = "健康與體育";
                                RowMove = 11;
                                break;
                            case "綜合活動":
                                KeyName = "綜合活動";
                                RowMove = 12;
                                break;

                            default:
                                KeyName = "";
                                RowMove = 0;
                                break;
                        }

                        if (sasr5r.StudDomainSocreDict.ContainsKey(KeyName))
                        {
                            wst.Cells[DataRow + RowMove, 1].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score81.GetFormat());  //8上
                            wst.Cells[DataRow + RowMove, 2].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score82.GetFormat());  //8下
                            wst.Cells[DataRow + RowMove, 3].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score91.GetFormat());  //九上
                        }
                        if (sasr5r.StudDomainscoreAvgDict.ContainsKey(KeyName))
                        {
                            wst.Cells[DataRow + RowMove, 4].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScore.GetFormat());  //領域學期成績
                            wst.Cells[DataRow + RowMove, 5].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercent.GetFormat());  //領域學期成績排名百分比
                        }
                    }

                    // 學期成績平均
                    wst.Cells[DataRow + 14, 1].PutValue(sasr5r.DomainScore81.GetFormat());  //8上
                    wst.Cells[DataRow + 14, 2].PutValue(sasr5r.DomainScore82.GetFormat());  //8下
                    wst.Cells[DataRow + 14, 3].PutValue(sasr5r.DomainScore91.GetFormat());  //九上

                    // 3學期總成績平均
                    wst.Cells[DataRow + 15, 1].PutValue(sasr5r.DomainScoreAvg.GetFormat());
                    // 3學期總成績平均名次百分比
                    wst.Cells[DataRow + 15, 5].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());
                    // 3學期「核心領域」總成績平均
                    //wst.Cells[DataRow + 16, 1].PutValue(sasr5r.CoreDomainScoreAvg.GetFormat());
                    // 3學期「核心領域」總成績平均名次百分比
                    //wst.Cells[DataRow + 16, 5].PutValue(sasr5r.CoreDomainScoreAvgRankPercent.GetFormat());

                    DataRow += RowNumber;
                }
                #endregion
            }
            else if (CurrentMode == ModuleMode.Taichung)
            {
                #region 中投一般
                // 填入學生值 普通
                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
                {
                    // 就讀國中                 
                    wst.Cells[DataRow + 2, 0].PutValue(wst.Cells[DataRow + 2, 0].StringValue + sasr5r.SchoolName);
                    // 就讀國中代碼
                    wst.Cells[DataRow + 2, 4].PutValue(wst.Cells[DataRow + 2, 4].StringValue + sasr5r.SchoolCode);
                    // 班級
                    wst.Cells[DataRow + 3, 0].PutValue(wst.Cells[DataRow + 3, 0].StringValue + sasr5r.ClassName);
                    // 姓名
                    wst.Cells[DataRow + 3, 2].PutValue(wst.Cells[DataRow + 3, 2].StringValue + sasr5r.Name);
                    // 身分證統一編號
                    wst.Cells[DataRow + 3, 4].PutValue(wst.Cells[DataRow + 3, 4].StringValue + sasr5r.IDNumber);
                    // 領域成績
                    string KeyName = ""; int RowMove = 0;
                    foreach (string DomainName in DomainNameList)
                    {
                        switch (DomainName)
                        {
                            case "國語文":
                                KeyName = "國語文";
                                RowMove = 5;
                                break;
                            case "英語":
                                KeyName = "英語";
                                RowMove = 6;
                                break;
                            case "數學":
                                KeyName = "數學";
                                RowMove = 7;
                                break;
                            case "社會":
                                KeyName = "社會";
                                RowMove = 8;
                                break;

                            case "自然與生活科技":
                                KeyName = "自然與生活科技";
                                RowMove = 9;
                                break;
                            case "藝術與人文":
                                KeyName = "藝術與人文";
                                RowMove = 10;
                                break;
                            case "健康與體育":
                                KeyName = "健康與體育";
                                RowMove = 11;
                                break;
                            case "綜合活動":
                                KeyName = "綜合活動";
                                RowMove = 12;
                                break;

                            default:
                                KeyName = "";
                                RowMove = 0;
                                break;
                        }

                        if (sasr5r.StudDomainSocreDict.ContainsKey(KeyName))
                        {
                            wst.Cells[DataRow + RowMove, 1].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score81.GetFormat());  //8上
                            wst.Cells[DataRow + RowMove, 2].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score82.GetFormat());  //8下
                            wst.Cells[DataRow + RowMove, 3].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score91.GetFormat());  //九上
                        }
                        if (sasr5r.StudDomainscoreAvgDict.ContainsKey(KeyName))
                        {
                            wst.Cells[DataRow + RowMove, 4].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScore.GetFormat());             //領域學期成績
                            wst.Cells[DataRow + RowMove, 5].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercent.GetFormat());  //領域學期成績排名百分比
                        }
                    }

                    // 學期成績平均
                    wst.Cells[DataRow + 14, 1].PutValue(sasr5r.DomainScore81.GetFormat());  //8上
                    wst.Cells[DataRow + 14, 2].PutValue(sasr5r.DomainScore82.GetFormat());  //8下
                    wst.Cells[DataRow + 14, 3].PutValue(sasr5r.DomainScore91.GetFormat());  //九上
                    // 3學期總成績平均
                    wst.Cells[DataRow + 15, 1].PutValue(sasr5r.DomainScoreAvg.GetFormat());
                    // 3學期總成績平均名次百分比
                    wst.Cells[DataRow + 15, 5].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());
                    DataRow += RowNumber;
                }
                #endregion 
            }
            else if (CurrentMode == ModuleMode.Tainan)
            {
                #region 臺南一般
                // 填入學生值 普通
                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
                {
                    #region 基本資料
                    // 就讀國中                 
                    wst.Cells[DataRow + 2, 0].PutValue(wst.Cells[DataRow + 2, 0].StringValue + sasr5r.SchoolName);
                    // 就讀國中代碼
                    wst.Cells[DataRow + 2, 4].PutValue(wst.Cells[DataRow + 2, 4].StringValue + sasr5r.SchoolCode);
                    // 班級
                    wst.Cells[DataRow + 3, 0].PutValue(wst.Cells[DataRow + 3, 0].StringValue + sasr5r.ClassName);
                    // 座號
                    wst.Cells[DataRow + 3, 1].PutValue(wst.Cells[DataRow + 3, 1].StringValue + sasr5r.SeatNo);
                    // 姓名
                    wst.Cells[DataRow + 3, 2].PutValue(wst.Cells[DataRow + 3, 2].StringValue + sasr5r.Name);
                    // 身分證統一編號
                    wst.Cells[DataRow + 3, 4].PutValue(wst.Cells[DataRow + 3, 4].StringValue + sasr5r.IDNumber);
                    #endregion

                    #region 領域成績
                    string KeyName = ""; int RowMove = 0;
                    foreach (string DomainName in DomainNameList)
                    {
                        switch (DomainName)
                        {
                            case "國語文":
                                KeyName = "國語文";
                                RowMove = 5;
                                break;
                            case "英語":
                                KeyName = "英語";
                                RowMove = 6;
                                break;
                            case "數學":
                                KeyName = "數學";
                                RowMove = 7;
                                break;
                            case "社會":
                                KeyName = "社會";
                                RowMove = 8;
                                break;

                            case "自然與生活科技":
                                KeyName = "自然與生活科技";
                                RowMove = 9;
                                break;
                            case "藝術與人文":
                                KeyName = "藝術與人文";
                                RowMove = 10;
                                break;
                            case "健康與體育":
                                KeyName = "健康與體育";
                                RowMove = 11;
                                break;
                            case "綜合活動":
                                KeyName = "綜合活動";
                                RowMove = 12;
                                break;

                            default:
                                KeyName = "";
                                RowMove = 0;
                                break;
                        }

                        if (sasr5r.StudDomainSocreDict.ContainsKey(KeyName))
                        {
                            wst.Cells[DataRow + RowMove, 1].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score81.GetFormat());  //8上
                            wst.Cells[DataRow + RowMove, 2].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score82.GetFormat());  //8下
                            wst.Cells[DataRow + RowMove, 3].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score91.GetFormat());  //九上
                        }
                        if (sasr5r.StudDomainscoreAvgDict.ContainsKey(KeyName))
                        {
                            wst.Cells[DataRow + RowMove, 4].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScore.GetFormat());             //領域學期成績
                            wst.Cells[DataRow + RowMove, 5].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercent.GetFormat());  //領域學期成績排名百分比
                        }
                    }
                    #endregion

                    #region 統計值
                    // 學期成績平均
                    wst.Cells[DataRow + 13, 1].PutValue(sasr5r.DomainScore81.GetFormat());  //8上
                    wst.Cells[DataRow + 13, 2].PutValue(sasr5r.DomainScore82.GetFormat());  //8下
                    wst.Cells[DataRow + 13, 3].PutValue(sasr5r.DomainScore91.GetFormat());  //九上
                    // 五學科學期成績平均
                    wst.Cells[DataRow + 14, 1].PutValue(sasr5r.CoreDomainScore81.GetFormat());  //8上
                    wst.Cells[DataRow + 14, 2].PutValue(sasr5r.CoreDomainScore82.GetFormat());  //8下
                    wst.Cells[DataRow + 14, 3].PutValue(sasr5r.CoreDomainScore91.GetFormat());  //九上
                    // 3學期總成績平均
                    wst.Cells[DataRow + 15, 1].PutValue(sasr5r.DomainScoreAvg.GetFormat());
                    // 3學期總成績平均名次百分比
                    wst.Cells[DataRow + 15, 5].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());
                    // 3學期「核心領域」總成績平均
                    wst.Cells[DataRow + 16, 1].PutValue(sasr5r.CoreDomainScoreAvg.GetFormat());
                    // 3學期「核心領域」總成績平均名次百分比
                    wst.Cells[DataRow + 16, 5].PutValue(sasr5r.CoreDomainScoreAvgRankPercent.GetFormat());
                    #endregion

                    DataRow += RowNumber;
                }
                #endregion
            }

            // 儲存檔案
            Campus.Report.ReportSaver.SaveWorkbook(wb,"國中在校成績證明單.xls");
            #endregion

            #region 特種身分

            if (rptStudListSpec.Count > 0)
            {
                // 讀取樣板
                Workbook template1 = new Workbook();
                Workbook wb1 = new Workbook();
                Worksheet wst1;
                byte[] SpecialReportSource = null;
                
                if (CurrentMode == ModuleMode.Tainan)
                  SpecialReportSource = Properties.Resources._100學年度國中在校成績證明單_台南特種;
                else if (CurrentMode == ModuleMode.Taichung)
                  SpecialReportSource = Properties.Resources._100學年度國中在校成績證明單_中投特種;
                else 
                  SpecialReportSource = Properties.Resources._100學年度國中在校成績證明單_竹苗特種;

                wb1.Open(new MemoryStream(SpecialReportSource));
                template1.Open(new MemoryStream(SpecialReportSource));
                template1.Worksheets[0].Cells[1, 0].PutValue(ReportTitle);
                wb1.Worksheets[0].Cells[1, 0].PutValue(ReportTitle);
                wst1 = wb1.Worksheets[0];
                Range all1 = template1.Worksheets.GetRangeByName("All");
                int RowNumber1 = all1.RowCount;
                int RowIndex1 = RowNumber1;

                // 檢查並計算人數是否超過 sheet 可放數量
                if ((RowNumber1 * rptStudListSpec.Count) > 65535)
                    return;

                // 複製樣板 當超過1人才複製。
                if (rptStudListSpec.Count > 1)
                    for (int i = 1; i < rptStudListSpec.Count; i++)
                    {
                        wst1.Cells.CreateRange(RowIndex1, RowNumber1, false).Copy(all1);
                        // 插入分頁
                        wst1.HPageBreaks.Add(RowIndex1, 0);
                        RowIndex1 += RowNumber1;
                    }

                int DataRow1 = 0;

                List<string> SpTypeName = new List<string>();
                SpTypeName.Add("原住民(持文化及語言能力證明)");
                SpTypeName.Add("原住民(未持文化及語言能力證明)");
                SpTypeName.Add("境外優秀科學技術人才子女");
                SpTypeName.Add("政府派赴國外工作人員子女");
                SpTypeName.Add("蒙藏生");
                SpTypeName.Add("身心障礙生    (以上請就考生最有利之特種身分勾選並限勾選一項)");

                List<string> studSpecTypeData = new List<string>();

                // 填入學生值 普通
                if (CurrentMode == ModuleMode.HsinChu)
                {
                    #region 新竹特種
                    foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudListSpec)
                    {
                        // 就讀國中
                        wst1.Cells[DataRow1 + 2, 0].PutValue(wst1.Cells[DataRow1 + 2, 0].StringValue + sasr5r.SchoolName);
                        // 就讀國中代碼
                        wst1.Cells[DataRow1 + 2, 4].PutValue(wst1.Cells[DataRow1 + 2, 4].StringValue + sasr5r.SchoolCode);
                        // 班級
                        wst1.Cells[DataRow1 + 3, 0].PutValue(wst1.Cells[DataRow1 + 3, 0].StringValue + sasr5r.ClassName);
                        // 姓名
                        wst1.Cells[DataRow1 + 3, 2].PutValue(wst1.Cells[DataRow1 + 3, 2].StringValue + sasr5r.Name);
                        // 身分證統一編號
                        wst1.Cells[DataRow1 + 3, 4].PutValue(wst1.Cells[DataRow1 + 3, 4].StringValue + sasr5r.IDNumber);
                        // 標示特種身分
                        studSpecTypeData.Clear();
                        foreach (string str in SpTypeName)
                        {
                            if (str.IndexOf(sasr5r.StudSpecType) > -1)
                                studSpecTypeData.Add("■" + str);
                            else
                                studSpecTypeData.Add("□" + str);
                        }

                        // 標特種身分
                        if (studSpecTypeData.Count >= 6)
                        {
                            wst1.Cells[DataRow1 + 4, 1].PutValue(studSpecTypeData[0] + studSpecTypeData[1]);
                            wst1.Cells[DataRow1 + 5, 1].PutValue(studSpecTypeData[2] + studSpecTypeData[3] + studSpecTypeData[4]);
                            wst1.Cells[DataRow1 + 6, 1].PutValue(studSpecTypeData[5]);
                        }

                        // 領域成績
                        string KeyName = ""; int RowMove = 0;
                        foreach (string DomainName in DomainNameList)
                        {
                            switch (DomainName)
                            {
                                case "國語文":
                                    KeyName = "國語文";
                                    RowMove = 8;
                                    break;
                                case "英語":
                                    KeyName = "英語";
                                    RowMove = 9;
                                    break;
                                case "數學":
                                    KeyName = "數學";
                                    RowMove = 10;
                                    break;
                                case "社會":
                                    KeyName = "社會";
                                    RowMove = 11;
                                    break;

                                case "自然與生活科技":
                                    KeyName = "自然與生活科技";
                                    RowMove = 12;
                                    break;
                                case "藝術與人文":
                                    KeyName = "藝術與人文";
                                    RowMove = 13;
                                    break;
                                case "健康與體育":
                                    KeyName = "健康與體育";
                                    RowMove = 14;
                                    break;
                                case "綜合活動":
                                    KeyName = "綜合活動";
                                    RowMove = 15;
                                    break;

                                default:
                                    KeyName = "";
                                    RowMove = 0;
                                    break;
                            }

                            if (sasr5r.StudDomainSocreDict.ContainsKey(KeyName))
                            {
                                wst1.Cells[DataRow1 + RowMove, 1].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score81.GetFormat());  //8上
                                wst1.Cells[DataRow1 + RowMove, 2].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score82.GetFormat());  //8下
                                wst1.Cells[DataRow1 + RowMove, 3].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score91.GetFormat());  //九上
                            }
                            if (sasr5r.StudDomainscoreAvgDict.ContainsKey(KeyName))
                            {
                                wst1.Cells[DataRow1 + RowMove, 4].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScore.GetFormat());                //領域學期成績
                                wst1.Cells[DataRow1 + RowMove, 5].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercent.GetFormat());     //領域學期成績排名百分比
                                wst1.Cells[DataRow1 + RowMove, 6].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercentAdd.GetFormat());  //領域學期成績排名百分比(加分)
                            }
                        }

                        // 學期成績平均
                        wst1.Cells[DataRow1 + 17, 1].PutValue(sasr5r.DomainScore81.GetFormat());  //8上
                        wst1.Cells[DataRow1 + 17, 2].PutValue(sasr5r.DomainScore82.GetFormat());  //8下
                        wst1.Cells[DataRow1 + 17, 3].PutValue(sasr5r.DomainScore91.GetFormat());  //九上
                        // 3學期總成績平均
                        wst1.Cells[DataRow1 + 18, 1].PutValue(sasr5r.DomainScoreAvg.GetFormat());
                        // 3學期總成績平均名次百分比
                        wst1.Cells[DataRow1 + 18, 5].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());
                        // 學期成績平均(加分後)
                        wst1.Cells[DataRow1 + 20, 1].PutValue(sasr5r.DomainScore81Add.GetFormat());  //8上
                        wst1.Cells[DataRow1 + 20, 2].PutValue(sasr5r.DomainScore82Add.GetFormat());  //8下
                        wst1.Cells[DataRow1 + 20, 3].PutValue(sasr5r.DomainScore91Add.GetFormat());  //九上
                        // 3學期總成績平均(加分後)
                        wst1.Cells[DataRow1 + 21, 1].PutValue(sasr5r.DomainScoreAvgAdd.GetFormat());
                        // 3學期總成績平均名次百分比(加分後)
                        wst1.Cells[DataRow1 + 21, 6].PutValue(sasr5r.DomainScoreAvgRankPercentAdd.GetFormat());
                        // 3學期「核心領域」總成績平均
                        //wst1.Cells[DataRow1 + 22, 1].PutValue(sasr5r.CoreDomainScoreAvgAdd.GetFormat());
                        // 3學期「核心領域」總成績平均名次百分比
                        //wst1.Cells[DataRow1 + 22, 6].PutValue(sasr5r.CoreDomainScoreAvgRankPercentAdd.GetFormat());

                        DataRow1 += RowNumber1;
                    }
                    #endregion
                }else if (CurrentMode == ModuleMode.Taichung)
                {
                    #region 台中特種
                    foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudListSpec)
                    {
                        // 就讀國中
                        wst1.Cells[DataRow1 + 2, 0].PutValue(wst1.Cells[DataRow1 + 2, 0].StringValue + sasr5r.SchoolName);
                        // 就讀國中代碼
                        wst1.Cells[DataRow1 + 2, 4].PutValue(wst1.Cells[DataRow1 + 2, 4].StringValue + sasr5r.SchoolCode);
                        // 班級
                        wst1.Cells[DataRow1 + 3, 0].PutValue(wst1.Cells[DataRow1 + 3, 0].StringValue + sasr5r.ClassName);
                        // 姓名
                        wst1.Cells[DataRow1 + 3, 2].PutValue(wst1.Cells[DataRow1 + 3, 2].StringValue + sasr5r.Name);
                        // 身分證統一編號
                        wst1.Cells[DataRow1 + 3, 4].PutValue(wst1.Cells[DataRow1 + 3, 4].StringValue + sasr5r.IDNumber);

                        // 標示特種身分
                        studSpecTypeData.Clear();
                        foreach (string str in SpTypeName)
                        {
                            if (str.IndexOf(sasr5r.StudSpecType) > -1)
                                studSpecTypeData.Add("■" + str);
                            else
                                studSpecTypeData.Add("□" + str);
                        }

                        // 標特種身分
                        if (studSpecTypeData.Count >= 6)
                        {
                            wst1.Cells[DataRow1 + 4, 1].PutValue(studSpecTypeData[0] + studSpecTypeData[1]);
                            wst1.Cells[DataRow1 + 5, 1].PutValue(studSpecTypeData[2] + studSpecTypeData[3] + studSpecTypeData[4]);
                            wst1.Cells[DataRow1 + 6, 1].PutValue(studSpecTypeData[5]);
                        }

                        // 領域成績
                        string KeyName = ""; int RowMove = 0;
                        foreach (string DomainName in DomainNameList)
                        {
                            switch (DomainName)
                            {
                                case "國語文":
                                    KeyName = "國語文";
                                    RowMove = 8;
                                    break;
                                case "英語":
                                    KeyName = "英語";
                                    RowMove = 9;
                                    break;
                                case "數學":
                                    KeyName = "數學";
                                    RowMove = 10;
                                    break;
                                case "社會":
                                    KeyName = "社會";
                                    RowMove = 11;
                                    break;

                                case "自然與生活科技":
                                    KeyName = "自然與生活科技";
                                    RowMove = 12;
                                    break;
                                case "藝術與人文":
                                    KeyName = "藝術與人文";
                                    RowMove = 13;
                                    break;
                                case "健康與體育":
                                    KeyName = "健康與體育";
                                    RowMove = 14;
                                    break;
                                case "綜合活動":
                                    KeyName = "綜合活動";
                                    RowMove = 15;
                                    break;

                                default:
                                    KeyName = "";
                                    RowMove = 0;
                                    break;
                            }

                            if (sasr5r.StudDomainSocreDict.ContainsKey(KeyName))
                            {
                                wst1.Cells[DataRow1 + RowMove, 1].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score81.GetFormat());  //8上
                                wst1.Cells[DataRow1 + RowMove, 2].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score82.GetFormat());  //8下
                                wst1.Cells[DataRow1 + RowMove, 3].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score91.GetFormat());  //九上
                            }
                            if (sasr5r.StudDomainscoreAvgDict.ContainsKey(KeyName))
                            {
                                wst1.Cells[DataRow1 + RowMove, 4].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScore.GetFormat());                //領域學期成績       
                                wst1.Cells[DataRow1 + RowMove, 5].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercent.GetFormat());  //領域學期成績排名百分比(加分)
                                wst1.Cells[DataRow1 + RowMove, 6].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercentAdd.GetFormat());  //領域學期成績排名百分比(加分)
                            }
                        }

                        // 學期成績平均
                        wst1.Cells[DataRow1 + 17, 1].PutValue(sasr5r.DomainScore81.GetFormat());  //8上
                        wst1.Cells[DataRow1 + 17, 2].PutValue(sasr5r.DomainScore82.GetFormat());  //8下
                        wst1.Cells[DataRow1 + 17, 3].PutValue(sasr5r.DomainScore91.GetFormat());  //九上

                        // 3學期總成績平均
                        wst1.Cells[DataRow1 + 18, 1].PutValue(sasr5r.DomainScoreAvg.GetFormat());

                        // 3學期總成績平均名次百分比
                        wst1.Cells[DataRow1 + 18, 5].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());

                        // 學期成績平均(加分後)
                        wst1.Cells[DataRow1 + 20, 1].PutValue(sasr5r.DomainScore81Add.GetFormat());  //8上
                        wst1.Cells[DataRow1 + 20, 2].PutValue(sasr5r.DomainScore82Add.GetFormat());  //8下
                        wst1.Cells[DataRow1 + 20, 3].PutValue(sasr5r.DomainScore91Add.GetFormat());  //九上

                        // 3學期總成績平均(加分後)
                        wst1.Cells[DataRow1 + 21, 1].PutValue(sasr5r.DomainScoreAvgAdd.GetFormat());
                        // 3學期總成績平均名次百分比(加分後)
                        wst1.Cells[DataRow1 + 21, 6].PutValue(sasr5r.DomainScoreAvgRankPercentAdd.GetFormat());

                        DataRow1 += RowNumber1;
                    }
                    #endregion
                }
                else if (CurrentMode == ModuleMode.Tainan)
                {
                    #region 臺南特種
                    foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudListSpec)
                    {
                        #region 學生基本資料
                        // 就讀國中
                        wst1.Cells[DataRow1 + 2, 0].PutValue(wst1.Cells[DataRow1 + 2, 0].StringValue + sasr5r.SchoolName);
                        // 就讀國中代碼
                        wst1.Cells[DataRow1 + 2, 4].PutValue(wst1.Cells[DataRow1 + 2, 4].StringValue + sasr5r.SchoolCode);
                        // 班級
                        wst1.Cells[DataRow1 + 3, 0].PutValue(wst1.Cells[DataRow1 + 3, 0].StringValue + sasr5r.ClassName);
                        // 座號
                        wst1.Cells[DataRow1 + 3, 1].PutValue(wst1.Cells[DataRow1 + 3, 1].StringValue + sasr5r.SeatNo);
                        // 姓名
                        wst1.Cells[DataRow1 + 3, 2].PutValue(wst1.Cells[DataRow1 + 3, 2].StringValue + sasr5r.Name);
                        // 身分證統一編號
                        wst1.Cells[DataRow1 + 3, 4].PutValue(wst1.Cells[DataRow1 + 3, 4].StringValue + sasr5r.IDNumber);
                        // 標示特種身分
                        studSpecTypeData.Clear();
                        foreach (string str in SpTypeName)
                        {
                            if (str.IndexOf(sasr5r.StudSpecType) > -1)
                                studSpecTypeData.Add("■" + str);
                            else
                                studSpecTypeData.Add("□" + str);
                        }

                        // 標特種身分
                        if (studSpecTypeData.Count >= 6)
                        {
                            wst1.Cells[DataRow1 + 4, 1].PutValue(studSpecTypeData[0] + studSpecTypeData[1]);
                            wst1.Cells[DataRow1 + 5, 1].PutValue(studSpecTypeData[2] + studSpecTypeData[3] + studSpecTypeData[4]);
                            wst1.Cells[DataRow1 + 6, 1].PutValue(studSpecTypeData[5]);
                        }
                        #endregion

                        #region 領域成績
                        string KeyName = ""; int RowMove = 0;
                        foreach (string DomainName in DomainNameList)
                        {
                            switch (DomainName)
                            {
                                case "國語文":
                                    KeyName = "國語文";
                                    RowMove = 8;
                                    break;
                                case "英語":
                                    KeyName = "英語";
                                    RowMove = 9;
                                    break;
                                case "數學":
                                    KeyName = "數學";
                                    RowMove = 10;
                                    break;
                                case "社會":
                                    KeyName = "社會";
                                    RowMove = 11;
                                    break;

                                case "自然與生活科技":
                                    KeyName = "自然與生活科技";
                                    RowMove = 12;
                                    break;
                                case "藝術與人文":
                                    KeyName = "藝術與人文";
                                    RowMove = 13;
                                    break;
                                case "健康與體育":
                                    KeyName = "健康與體育";
                                    RowMove = 14;
                                    break;
                                case "綜合活動":
                                    KeyName = "綜合活動";
                                    RowMove = 15;
                                    break;

                                default:
                                    KeyName = "";
                                    RowMove = 0;
                                    break;
                            }

                            if (sasr5r.StudDomainSocreDict.ContainsKey(KeyName))
                            {
                                wst1.Cells[DataRow1 + RowMove, 1].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score81.GetFormat());  //8上
                                wst1.Cells[DataRow1 + RowMove, 2].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score82.GetFormat());  //8下
                                wst1.Cells[DataRow1 + RowMove, 3].PutValue(sasr5r.StudDomainSocreDict[KeyName].Score91.GetFormat());  //九上
                            }
                            if (sasr5r.StudDomainscoreAvgDict.ContainsKey(KeyName))
                            {
                                wst1.Cells[DataRow1 + RowMove, 4].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScore.GetFormat());                //領域學期成績
                                wst1.Cells[DataRow1 + RowMove, 5].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercent.GetFormat());     //領域學期成績排名百分比
                                wst1.Cells[DataRow1 + RowMove, 6].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercentAdd.GetFormat());  //領域學期成績排名百分比(加分)
                            }
                        }
                        #endregion

                        #region 統計值
                        // 學期成績平均
                        wst1.Cells[DataRow1 + 16, 1].PutValue(sasr5r.DomainScore81.GetFormat());  //8上
                        wst1.Cells[DataRow1 + 16, 2].PutValue(sasr5r.DomainScore82.GetFormat());  //8下
                        wst1.Cells[DataRow1 + 16, 3].PutValue(sasr5r.DomainScore91.GetFormat());  //九上
                        // 3學期總成績平均
                        wst1.Cells[DataRow1 + 17, 1].PutValue(sasr5r.DomainScoreAvg.GetFormat());
                        // 3學期總成績平均名次百分比
                        wst1.Cells[DataRow1 + 17, 6].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());
                        // 學期成績平均(加分後)
                        wst1.Cells[DataRow1 + 18, 1].PutValue(sasr5r.CoreDomainScore81Add.GetFormat());
                        wst1.Cells[DataRow1 + 18, 2].PutValue(sasr5r.CoreDomainScore82Add.GetFormat());
                        wst1.Cells[DataRow1 + 18, 3].PutValue(sasr5r.CoreDomainScore91Add.GetFormat());  //九上
                        // 3學期總成績平均(加分後)
                        wst1.Cells[DataRow1 + 19, 1].PutValue(sasr5r.DomainScoreAvgAdd.GetFormat());
                        // 3學期總成績平均名次百分比(加分後)
                        wst1.Cells[DataRow1 + 19, 6].PutValue(sasr5r.DomainScoreAvgRankPercentAdd.GetFormat());
                        // 3學期「核心領域」總成績平均
                        wst1.Cells[DataRow1 + 20, 1].PutValue(sasr5r.CoreDomainScoreAvgAdd.GetFormat());
                        // 3學期「核心領域」總成績平均名次百分比
                        wst1.Cells[DataRow1 + 20, 6].PutValue(sasr5r.CoreDomainScoreAvgRankPercentAdd.GetFormat());
                        #endregion

                        DataRow1 += RowNumber1;
                    }
                    #endregion
                }

                Campus.Report.ReportSaver.SaveWorkbook(wb1,"國中在校成績證明單(特種身分).xls");
            }
            #endregion
             
            #region 報名系統檔
            if (!checkOutputImportFile)
                return;

            if (CurrentMode ==  ModuleMode.HsinChu)
            {
                #region 新竹匯入
                // 讀取樣板
                //Workbook wb3 = new Workbook();
                //Worksheet wst3;
                //wb3.Open(new MemoryStream(Properties.Resources._100新竹高中免試國中報名系統檔));
                //wst3 = wb3.Worksheets[0];

                // 2013新版
                Workbook wb3aa = new Workbook();
                wb3aa.Open(new MemoryStream(Properties.Resources._102新竹高中免試在校學習成績資料));
                Worksheet wst3aa = wb3aa.Worksheets[0];

                Workbook wb3ab = new Workbook();
                wb3ab.Open(new MemoryStream(Properties.Resources._102新竹高中免試三學期各科成績));
                Worksheet wst3ab = wb3ab.Worksheets[0];

                //int RowIdx = 1;
                int rowIdxa = 1,rowIdxb=1;

                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
                {
                    // wst3aa
                    // 身分證字號, col:0
                    wst3aa.Cells[rowIdxa, 0].PutValue(sasr5r.IDNumber);
                    // 學業成績總平均, col:1
                    wst3aa.Cells[rowIdxa, 1].PutValue(sasr5r.DomainScoreAvg.GetFormat());
                    // 學業成績全校排名百分比, col:2
                    wst3aa.Cells[rowIdxa, 2].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());
                    // 國文成績總平均, col:3
                    wst3aa.Cells[rowIdxa, 3].PutValue(GetStudDomainscoreAvg("國語文", sasr5r.StudDomainscoreAvgDict, false));
                    // 國文成績全校排名百分比, col:4
                    wst3aa.Cells[rowIdxa, 4].PutValue(GetStudDomainscoreAvgPercentRank("國語文", sasr5r.StudDomainscoreAvgDict, false));
                    // 英語成績總平均, col:5
                    wst3aa.Cells[rowIdxa, 5].PutValue(GetStudDomainscoreAvg("英語", sasr5r.StudDomainscoreAvgDict, false));
                    // 英語成績全校排名百分比, col:6
                    wst3aa.Cells[rowIdxa, 6].PutValue(GetStudDomainscoreAvgPercentRank("英語", sasr5r.StudDomainscoreAvgDict, false));
                    // 數學成績總平均, col:7
                    wst3aa.Cells[rowIdxa, 7].PutValue(GetStudDomainscoreAvg("數學", sasr5r.StudDomainscoreAvgDict, false));
                    // 數學成績全校排名百分比, col:8
                    wst3aa.Cells[rowIdxa, 8].PutValue(GetStudDomainscoreAvgPercentRank("數學", sasr5r.StudDomainscoreAvgDict, false));
                    // 社會成績總平均, col:9
                    wst3aa.Cells[rowIdxa, 9].PutValue(GetStudDomainscoreAvg("社會", sasr5r.StudDomainscoreAvgDict, false));
                    // 社會成績全校排名百分比, col:10
                    wst3aa.Cells[rowIdxa, 10].PutValue(GetStudDomainscoreAvgPercentRank("社會", sasr5r.StudDomainscoreAvgDict, false));
                    // 自然成績總平均, col:11
                    wst3aa.Cells[rowIdxa, 11].PutValue(GetStudDomainscoreAvg("自然與生活科技", sasr5r.StudDomainscoreAvgDict, false));
                    // 自然成績全校排名百分比, col:12
                    wst3aa.Cells[rowIdxa, 12].PutValue(GetStudDomainscoreAvgPercentRank("自然與生活科技", sasr5r.StudDomainscoreAvgDict, false));
                    // 藝術與人文成績總平均, col:13
                    wst3aa.Cells[rowIdxa, 13].PutValue(GetStudDomainscoreAvg("藝術與人文", sasr5r.StudDomainscoreAvgDict, false));
                    // 藝術與人文成績全校排名百分比, col:14
                    wst3aa.Cells[rowIdxa, 14].PutValue(GetStudDomainscoreAvgPercentRank("藝術與人文", sasr5r.StudDomainscoreAvgDict, false));
                    // 健康與體育成績總平均, col:15
                    wst3aa.Cells[rowIdxa, 15].PutValue(GetStudDomainscoreAvg("健康與體育", sasr5r.StudDomainscoreAvgDict, false));
                    // 健康與體育成績全校排名百分比, col:16
                    wst3aa.Cells[rowIdxa, 16].PutValue(GetStudDomainscoreAvgPercentRank("健康與體育", sasr5r.StudDomainscoreAvgDict, false));
                    // 綜合活動成績總平均, col:17
                    wst3aa.Cells[rowIdxa, 17].PutValue(GetStudDomainscoreAvg("綜合活動", sasr5r.StudDomainscoreAvgDict, false));
                    // 綜合活動成績全校排名百分比, col:18
                    wst3aa.Cells[rowIdxa, 18].PutValue(GetStudDomainscoreAvgPercentRank("綜合活動", sasr5r.StudDomainscoreAvgDict, false));
                    // 學業成績總平均(加分後), col:19
                    wst3aa.Cells[rowIdxa, 19].PutValue(sasr5r.DomainScoreAvgAdd.GetFormat());
                    // 學業成績全校排名百分比(加分後), col:20
                    wst3aa.Cells[rowIdxa, 20].PutValue(sasr5r.DomainScoreAvgRankPercentAdd.GetFormat());
                    // 國文成績總平均(加分後), col:21
                    wst3aa.Cells[rowIdxa, 21].PutValue(GetStudDomainscoreAvg("國語文", sasr5r.StudDomainscoreAvgDict, true));
                    // 國文成績全校排名百分比(加分後), col:22
                    wst3aa.Cells[rowIdxa, 22].PutValue(GetStudDomainscoreAvgPercentRank("國語文", sasr5r.StudDomainscoreAvgDict, true));
                    // 英語成績總平均(加分後), col:23
                    wst3aa.Cells[rowIdxa, 23].PutValue(GetStudDomainscoreAvg("英語", sasr5r.StudDomainscoreAvgDict, true));
                    // 英語成績全校排名百分比(加分後), col:24
                    wst3aa.Cells[rowIdxa, 24].PutValue(GetStudDomainscoreAvgPercentRank("英語", sasr5r.StudDomainscoreAvgDict, true));
                    // 數學成績總平均(加分後), col:25
                    wst3aa.Cells[rowIdxa, 25].PutValue(GetStudDomainscoreAvg("數學", sasr5r.StudDomainscoreAvgDict, true));
                    // 數學成績全校排名百分比(加分後), col:26
                    wst3aa.Cells[rowIdxa, 26].PutValue(GetStudDomainscoreAvgPercentRank("數學", sasr5r.StudDomainscoreAvgDict, true));
                    // 社會成績總平均(加分後), col:27
                    wst3aa.Cells[rowIdxa, 27].PutValue(GetStudDomainscoreAvg("社會", sasr5r.StudDomainscoreAvgDict, true));
                    // 社會成績全校排名百分比(加分後), col:28
                    wst3aa.Cells[rowIdxa, 28].PutValue(GetStudDomainscoreAvgPercentRank("社會", sasr5r.StudDomainscoreAvgDict, true));
                    // 自然成績總平均(加分後), col:29
                    wst3aa.Cells[rowIdxa, 29].PutValue(GetStudDomainscoreAvg("自然與生活科技", sasr5r.StudDomainscoreAvgDict, true));
                    // 自然成績全校排名百分比(加分後), col:30
                    wst3aa.Cells[rowIdxa, 30].PutValue(GetStudDomainscoreAvgPercentRank("自然與生活科技", sasr5r.StudDomainscoreAvgDict, true));
                    // 藝術與人文成績總平均(加分後), col:31
                    wst3aa.Cells[rowIdxa, 31].PutValue(GetStudDomainscoreAvg("藝術與人文", sasr5r.StudDomainscoreAvgDict, true));
                    // 藝術與人文成績全校排名百分比(加分後), col:32
                    wst3aa.Cells[rowIdxa, 32].PutValue(GetStudDomainscoreAvgPercentRank("藝術與人文", sasr5r.StudDomainscoreAvgDict, true));
                    // 健康與體育成績總平均(加分後), col:33
                    wst3aa.Cells[rowIdxa, 33].PutValue(GetStudDomainscoreAvg("健康與體育", sasr5r.StudDomainscoreAvgDict, true));
                    // 健康與體育成績全校排名百分比(加分後), col:34
                    wst3aa.Cells[rowIdxa, 34].PutValue(GetStudDomainscoreAvgPercentRank("健康與體育", sasr5r.StudDomainscoreAvgDict, true));
                    // 綜合活動成績總平均(加分後), col:35
                    wst3aa.Cells[rowIdxa, 35].PutValue(GetStudDomainscoreAvg("綜合活動", sasr5r.StudDomainscoreAvgDict, true));
                    // 綜合活動成績全校排名百分比(加分後), col:36
                    wst3aa.Cells[rowIdxa, 36].PutValue(GetStudDomainscoreAvgPercentRank("綜合活動", sasr5r.StudDomainscoreAvgDict, true));

                    // wst3ab
                    // 身分證統一編號, col:0
                    wst3ab.Cells[rowIdxb, 0].PutValue(sasr5r.IDNumber);
                    // 八上國, col:1
                    wst3ab.Cells[rowIdxb, 1].PutValue(sasr5r.StudDomainSocreDict["國語文"].Score81.GetFormat());
                    // 八下國, col:2
                    wst3ab.Cells[rowIdxb, 2].PutValue(sasr5r.StudDomainSocreDict["國語文"].Score82.GetFormat());
                    // 九上國, col:3
                    wst3ab.Cells[rowIdxb, 3].PutValue(sasr5r.StudDomainSocreDict["國語文"].Score91.GetFormat());
                    // 八上英, col:4
                    wst3ab.Cells[rowIdxb, 4].PutValue(sasr5r.StudDomainSocreDict["英語"].Score81.GetFormat());
                    // 八下英, col:5
                    wst3ab.Cells[rowIdxb, 5].PutValue(sasr5r.StudDomainSocreDict["英語"].Score82.GetFormat());
                    // 九上英, col:6
                    wst3ab.Cells[rowIdxb, 6].PutValue(sasr5r.StudDomainSocreDict["英語"].Score91.GetFormat());
                    // 八上數, col:7
                    wst3ab.Cells[rowIdxb, 7].PutValue(sasr5r.StudDomainSocreDict["數學"].Score81.GetFormat());
                    // 八下數, col:8
                    wst3ab.Cells[rowIdxb, 8].PutValue(sasr5r.StudDomainSocreDict["數學"].Score82.GetFormat());
                    // 九上數, col:9
                    wst3ab.Cells[rowIdxb, 9].PutValue(sasr5r.StudDomainSocreDict["數學"].Score91.GetFormat());
                    // 八上社, col:10
                    wst3ab.Cells[rowIdxb, 10].PutValue(sasr5r.StudDomainSocreDict["社會"].Score81.GetFormat());
                    // 八下社, col:11
                    wst3ab.Cells[rowIdxb, 11].PutValue(sasr5r.StudDomainSocreDict["社會"].Score82.GetFormat());
                    // 九上社, col:12
                    wst3ab.Cells[rowIdxb, 12].PutValue(sasr5r.StudDomainSocreDict["社會"].Score91.GetFormat());
                    // 八上自, col:13
                    wst3ab.Cells[rowIdxb, 13].PutValue(sasr5r.StudDomainSocreDict["自然與生活科技"].Score81.GetFormat());
                    // 八下自, col:14
                    wst3ab.Cells[rowIdxb, 14].PutValue(sasr5r.StudDomainSocreDict["自然與生活科技"].Score82.GetFormat());
                    // 九上自, col:15
                    wst3ab.Cells[rowIdxb, 15].PutValue(sasr5r.StudDomainSocreDict["自然與生活科技"].Score91.GetFormat());
                    // 八上藝, col:16
                    wst3ab.Cells[rowIdxb, 16].PutValue(sasr5r.StudDomainSocreDict["藝術與人文"].Score81.GetFormat());
                    // 八下藝, col:17
                    wst3ab.Cells[rowIdxb, 17].PutValue(sasr5r.StudDomainSocreDict["藝術與人文"].Score82.GetFormat());
                    // 九上藝, col:18
                    wst3ab.Cells[rowIdxb, 18].PutValue(sasr5r.StudDomainSocreDict["藝術與人文"].Score91.GetFormat());
                    // 八上健, col:19
                    wst3ab.Cells[rowIdxb, 19].PutValue(sasr5r.StudDomainSocreDict["健康與體育"].Score81.GetFormat());
                    // 八下健, col:20
                    wst3ab.Cells[rowIdxb, 20].PutValue(sasr5r.StudDomainSocreDict["健康與體育"].Score82.GetFormat());
                    // 九上健, col:21
                    wst3ab.Cells[rowIdxb, 21].PutValue(sasr5r.StudDomainSocreDict["健康與體育"].Score91.GetFormat());
                    // 八上綜, col:22
                    wst3ab.Cells[rowIdxb, 22].PutValue(sasr5r.StudDomainSocreDict["綜合活動"].Score81.GetFormat());
                    // 八下綜, col:23
                    wst3ab.Cells[rowIdxb, 23].PutValue(sasr5r.StudDomainSocreDict["綜合活動"].Score82.GetFormat());
                    // 九上綜, col:24
                    wst3ab.Cells[rowIdxb, 24].PutValue(sasr5r.StudDomainSocreDict["綜合活動"].Score91.GetFormat());
                    // 八上總平均, col:25
                    wst3ab.Cells[rowIdxb, 25].PutValue(sasr5r.DomainScore81.GetFormat());
                    // 八下總平均, col:26
                    wst3ab.Cells[rowIdxb, 26].PutValue(sasr5r.DomainScore82.GetFormat());
                    // 九上總平均, col:27
                    wst3ab.Cells[rowIdxb, 27].PutValue(sasr5r.DomainScore91.GetFormat());
                    // 八上總平均(加分後), col:28
                    wst3ab.Cells[rowIdxb, 28].PutValue(sasr5r.DomainScore81Add);
                    // 八下總平均(加分後), col:29
                    wst3ab.Cells[rowIdxb, 29].PutValue(sasr5r.DomainScore82Add);
                    // 九上總平均(加分後), col:30
                    wst3ab.Cells[rowIdxb, 30].PutValue(sasr5r.DomainScore91Add);

                    rowIdxa++; rowIdxb++;
                }

                if(checkWst3aa)
                    Campus.Report.ReportSaver.SaveWorkbook(wb3aa, "免試在校學習成績資料匯入檔案");

                if(checkWst3ab)
                    Campus.Report.ReportSaver.SaveWorkbook(wb3ab, "免試三學期各科成績匯入檔案");

                #endregion
            }
            else if (CurrentMode == ModuleMode.Taichung)
            {
                #region 台中匯入
                // 讀取樣板
                Workbook wb3 = new Workbook();
                Worksheet wst3;
                wb3.Open(new MemoryStream(Properties.Resources._100中投區國中報名系統檔));
                wst3 = wb3.Worksheets[0];

                int RowIdx = 1;
                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
                {
                    // 身分證字號: 1
                    wst3.Cells[RowIdx, 0].PutValue(sasr5r.IDNumber);

                    for(int i=0;i<DomainNameList.Count;i++)
                    {
                        //各領域成績總平均
                        wst3.Cells[RowIdx, 1+i].PutValue(GetStudDomainscoreAvg(DomainNameList[i], sasr5r.StudDomainscoreAvgDict, false));
                        //各領域成績全校排名百分比
                        wst3.Cells[RowIdx, 9 + i].PutValue(GetStudDomainscoreAvgPercentRank(DomainNameList[i], sasr5r.StudDomainscoreAvgDict, false)); 
                    }

                    //國二上平均	小數2位
                    wst3.Cells[RowIdx, 17].PutValue(sasr5r.DomainScore81.GetFormat());
                    //國二下平均	小數2位
                    wst3.Cells[RowIdx, 18].PutValue(sasr5r.DomainScore82.GetFormat());
                    //國三上平均	小數2位
                    wst3.Cells[RowIdx, 19].PutValue(sasr5r.DomainScore91.GetFormat());
                    //3學期總平均	小數2位
                    wst3.Cells[RowIdx, 20].PutValue(sasr5r.DomainScoreAvg.GetFormat());
                    //3學期百分比	整數
                    wst3.Cells[RowIdx, 21].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());

                    //各領域（特百分比）
                    for (int i = 0; i < DomainNameList.Count; i++)
                        wst3.Cells[RowIdx, 22 + i].PutValue(GetStudDomainscoreAvgPercentRank(DomainNameList[i],sasr5r.StudDomainscoreAvgDict, true));

                    //國二上平均(特)	小數2位
                    wst3.Cells[RowIdx, 30].PutValue(sasr5r.DomainScore81Add);
                    //國二下平均(特)	小數2位
                    wst3.Cells[RowIdx, 31].PutValue(sasr5r.DomainScore82Add);
                    //國三上平均(特)	小數2位
                    wst3.Cells[RowIdx, 32].PutValue(sasr5r.DomainScore91Add);
                    //3學期總平均(特)	小數2位
                    wst3.Cells[RowIdx, 33].PutValue(sasr5r.DomainScoreAvgAdd);
                    //3學期百分比(特)	整數
                    wst3.Cells[RowIdx, 34].PutValue(sasr5r.DomainScoreAvgRankPercentAdd);

                    //學期各領域成績
                    for (int i = 0; i < DomainNameList.Count; i++)
                    {
                        //各領域(8上)	小數2位
                        wst3.Cells[RowIdx,35  + i].PutValue(sasr5r.StudDomainSocreDict[DomainNameList[i]].Score81.GetFormat());
                        //各領域(8下)	小數2位
                        wst3.Cells[RowIdx,43 + i].PutValue(sasr5r.StudDomainSocreDict[DomainNameList[i]].Score82.GetFormat());
                        //各領域(9上)	小數2位
                        wst3.Cells[RowIdx,51 + i].PutValue(sasr5r.StudDomainSocreDict[DomainNameList[i]].Score91.GetFormat());
                    }

                    RowIdx++;
                }

                string FileName3 = "免試國中報名匯入檔案.xls";

                Campus.Report.ReportSaver.SaveWorkbook(wb3, FileName3);
                #endregion
            }
            else if (CurrentMode == ModuleMode.Tainan)
            {
                #region 台南匯入
                // 讀取樣板
                Workbook wb3 = new Workbook();
                Worksheet wst3;
                wb3.Open(new MemoryStream(Properties.Resources._101台南區嘉義區高中免試國中報名系統檔));
                wst3 = wb3.Worksheets[0];

                int RowIdx = 1;

                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
                {
                    //學號
                    wst3.Cells[RowIdx, 0].PutValue(sasr5r.StudentNumber);

                    for (int i = 0; i < DomainNameList.Count; i++)
                    {
                        if (sasr5r.StudDomainSocreDict.ContainsKey(DomainNameList[i]))
                        {
                            //八年級上學期領域成績
                             wst3.Cells[RowIdx, 1 + i].PutValue(sasr5r.StudDomainSocreDict[DomainNameList[i]].Score81.GetFormat());
                            //八年級下學期領域成績
                             wst3.Cells[RowIdx, 10 + i].PutValue(sasr5r.StudDomainSocreDict[DomainNameList[i]].Score82.GetFormat());
                            //九年級上學期領域成績
                             wst3.Cells[RowIdx, 19 + i].PutValue(sasr5r.StudDomainSocreDict[DomainNameList[i]].Score91.GetFormat());
                            //領域成績平均
                             wst3.Cells[RowIdx, 28 + i].PutValue(sasr5r.StudDomainscoreAvgDict[DomainNameList[i]].DomainScore.GetFormat());
                            //領域成績排名（國語文成績排名百分比	英語成績排名百分比	數學成績排名百分比	社會成績排名百分比	自然與生活科技成績排名百分比 藝術與人文成績排名百分比 健康與體育成績排名百分比 綜合活動成績排名百分比）
                             wst3.Cells[RowIdx, 37 + i].PutValue(sasr5r.StudDomainscoreAvgDict[DomainNameList[i]].DomainScoreRankPercent.GetFormat());
                        }
                    }

                    //學期平均(八年級上學期)
                     wst3.Cells[RowIdx, 9].PutValue(sasr5r.DomainScore81.GetFormat());
                    //學期平均(八年級下學期)
                     wst3.Cells[RowIdx, 18].PutValue(sasr5r.DomainScore82.GetFormat());
                    //學期平均(九年級上學期)
                    wst3.Cells[RowIdx, 27].PutValue(sasr5r.DomainScore91.GetFormat());
                    //三學期總平均
                     wst3.Cells[RowIdx, 36].PutValue(sasr5r.DomainScoreAvg.GetFormat());
                    //三學期總排名百分比
                     wst3.Cells[RowIdx, 45].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());
                    //前五學科三學期總平均
                    wst3.Cells[RowIdx, 70].PutValue(sasr5r.CoreDomainScoreAvg.GetFormat());
                    //前五學科三學期總平均排名百分比
                     wst3.Cells[RowIdx, 72].PutValue(sasr5r.CoreDomainScoreAvgRankPercent);

                    if (sasr5r.AddWeight.HasValue && !string.IsNullOrWhiteSpace(sasr5r.StudSpecType))
                    {
                        for (int i = 0; i < DomainNameList.Count; i++)
                        {
                            if (sasr5r.StudDomainSocreDict.ContainsKey(DomainNameList[i]))
                            {
                                //特殊生加分後領域成績（特殊生加分後國語文成績平均 特殊生加分後英語成績平均	特殊生加分後數學成績平均 特殊生加分後社會成績平均	特殊生加分後自然與生活科技成績平均 特殊生加分後藝術與人文成績平均 特殊生加分後健康與體育成績平均	特殊生加分後綜合活動成績平均）
                                 wst3.Cells[RowIdx, 46 + i].PutValue(sasr5r.StudDomainscoreAvgDict[DomainNameList[i]].DomainScoreAdd.GetFormat());
                                //特殊生加分後國語文成績排名百分比	特殊生加分後英語成績排名百分比	特殊生加分後數學成績排名百分比	特殊生加分後社會成績排名百分比	特殊生加分後自然與生活科技成績排名百分比	特殊生加分後藝術與人文成績排名百分比	特殊生加分後健康與體育成績排名百分比	特殊生加分後綜合活動成績排名百分比
                                 wst3.Cells[RowIdx, 55 + i].PutValue(sasr5r.StudDomainscoreAvgDict[DomainNameList[i]].DomainScoreRankPercentAdd.GetFormat());
                            }
                        }
                    }

                    //特殊生加分後三學期總平均
                    wst3.Cells[RowIdx, 54].PutValue(sasr5r.DomainScoreAvgAdd.GetFormat());
                    //特殊生加分後三學期總排名百分比
                    wst3.Cells[RowIdx, 63].PutValue(sasr5r.DomainScoreAvgRankPercentAdd.GetFormat());
                    //前五學科學期成績平均(八年級上學期)	
                     wst3.Cells[RowIdx, 64].PutValue(sasr5r.CoreDomainScore81.GetFormat());
                    //前五學科學期成績平均(八年級下學期)	
                     wst3.Cells[RowIdx, 65].PutValue(sasr5r.CoreDomainScore82.GetFormat());
                    //前五學科學期成績平均(九年級上學期)
                    wst3.Cells[RowIdx, 66].PutValue(sasr5r.CoreDomainScore91.GetFormat());
                    //特殊生加分後前五學科學期成績平均(八年級上學期)
                    wst3.Cells[RowIdx, 67].PutValue(sasr5r.CoreDomainScore81Add.GetFormat());
                    //特殊生加分後前五學科學期成績平均(八年級下學期)	
                    wst3.Cells[RowIdx, 68].PutValue(sasr5r.CoreDomainScore82Add.GetFormat());
                    //特殊生加分後前五學科學期成績平均(九年級上學期)
                    wst3.Cells[RowIdx, 69].PutValue(sasr5r.CoreDomainScore91Add.GetFormat());
                    //特殊生加分後前五學科三學期總平均
                    wst3.Cells[RowIdx, 71].PutValue(sasr5r.CoreDomainScoreAvgAdd.GetFormat());
                    //特殊生加分後前五學科三學期總平均排名百分比
                    wst3.Cells[RowIdx, 73].PutValue(sasr5r.CoreDomainScoreAvgRankPercentAdd.GetFormat());

                    RowIdx++;
                }

                string FileName3 = "免試國中報名匯入檔案.xls";
                Campus.Report.ReportSaver.SaveWorkbook(wb3, FileName3);
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// 取得領域平均(匯出格式檔用)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="data"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>

        private string GetStudDomainscoreAvg(string Name, Dictionary<string, DomainAvgScore5Rpt> data, bool isAdd)
        {
            decimal? retVal = null;
            if (data.ContainsKey(Name))
            {
                if (isAdd)
                {
                    if (data[Name].DomainScoreAdd.HasValue)
                        retVal = data[Name].DomainScoreAdd.Value;
                }
                else
                {
                    if (data[Name].DomainScore.HasValue)
                        retVal = data[Name].DomainScore;
                }
            }

            return retVal.GetFormat();

            //if (retVal.HasValue)
            //{
            //    return string.Format("{0:###.##}", retVal.Value);
            //}
            //else
            //    return "";        
        }

        /// <summary>
        /// 取得領域平均內排名百分比(匯出格式檔用)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="data"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        private string GetStudDomainscoreAvgPercentRank(string Name, Dictionary<string, DomainAvgScore5Rpt> data,bool isAdd)
        {
            int? retVal=null;
            if (data.ContainsKey(Name))
            {
                if (isAdd)
                {
                    if (data[Name].DomainScoreRankPercentAdd.HasValue)
                        retVal = data[Name].DomainScoreRankPercentAdd.Value;
                }
                else
                {
                    if (data[Name].DomainScoreRankPercent.HasValue)
                        retVal = data[Name].DomainScoreRankPercent.Value;                
                }
            }

            return retVal.GetFormat();

            //if (retVal.HasValue)
            //{
            //    return string.Format("{0:###}", retVal.Value);
            //}
            //else
            //    return "";
        }
        
        /// <summary>
        /// 取得領域成績(匯入報表用)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetStudDomainscorePercentRank(string Name,Dictionary<string,DomainScore5Rpt> data,int type)
        {
            decimal? retVal = null;

            if (data.ContainsKey(Name))
            {
                if (type == 81)
                    if (data[Name].Score81.HasValue)
                        retVal = data[Name].Score81.Value;

                if (type == 82)
                    if (data[Name].Score82.HasValue)
                        retVal = data[Name].Score82.Value;

                if (type == 91)
                    if (data[Name].Score91.HasValue)
                        retVal = data[Name].Score91.Value;            
            }

            return retVal.GetFormat();

            //if (retVal.HasValue)
            //{
            //    return string.Format("{0:###.00}", retVal.Value);
            //}
            //else
            //    return "";
        }
        /// <summary>
        /// 將ReportStudent轉成StudentAvgScoreRank5Rpt
        /// </summary>
        /// <param name="rptStudList"></param>
        /// <returns></returns>
        public static List<StudentAvgScoreRank5Rpt> StudentAvgScoreRank5RptParse(List<ReportStudent> rptStudList)
        {
            List<StudentAvgScoreRank5Rpt> retVal = new List<StudentAvgScoreRank5Rpt>();
            string SchoolCode = K12.Data.School.Code,SchoolName=K12.Data.School.ChineseName;

            // 取得 UDT 對照
            List<JointAdmissionModule.DAL.UserDefData_StudTypeWeight> udtList = DAL.UDTTransfer.GetDataFromUDT_StudTypeWeight().Where(x => x.SchoolType == "高中").ToList ();
            // 建立特種身分代碼對照表
            Dictionary<string, string> specCodeDict = new Dictionary<string, string>();
            foreach (JointAdmissionModule.DAL.UserDefData_StudTypeWeight dd in udtList)
                if (!specCodeDict.ContainsKey(dd.JoinStudType))
                    specCodeDict.Add(dd.JoinStudType, dd.JoinStudTypeCode);            

            // 領域索引
            Dictionary<string, string> DomainNameDict = new Dictionary<string, string>();

            foreach (string str in DAL.DALTransfer.GetDomainNameList())
            {
                string key=str+"領域3學期平均";
                if(!DomainNameDict.ContainsKey(str))
                    DomainNameDict.Add(key, str);
            }

            foreach (ReportStudent stud in rptStudList)
            {
                // 取得年級2,3 學年度
                int gr2Sy = 0, gr3Sy = 0;

                foreach (JHEvaluation.ScoreCalculation.SemesterData sd in stud.SHistory.GetGradeYearSemester())
                {
                    if (sd.GradeYear == 8 || sd.GradeYear == 2)
                        gr2Sy = sd.SchoolYear;

                    if (sd.GradeYear == 9 || sd.GradeYear == 3)
                        gr3Sy = sd.SchoolYear;
                }

                StudentAvgScoreRank5Rpt sascrr = new StudentAvgScoreRank5Rpt();
                if (stud.Class != null)
                    sascrr.ClassName = stud.Class.Name;
                sascrr.IDNumber = stud.IDNumber;
                sascrr.Name = stud.Name;
                sascrr.StudentNumber = stud.StudentNumber;
                // 特種身分
                sascrr.StudSpecType = stud.SpcStudTypeName;

                // 特種身分代碼
                if (stud.SpcStudTypeName != null) 
                if (specCodeDict.ContainsKey(stud.SpcStudTypeName))
                    sascrr.StudSpecTypeCode = specCodeDict[stud.SpcStudTypeName];

                sascrr.SchoolCode = SchoolCode;
                sascrr.SchoolName = SchoolName;
                // 加分比
                sascrr.AddWeight = stud.AddWeight;
                sascrr.SeatNo = stud.SeatNo;
                sascrr.StudentID = stud.StudentID;
                //將特殊身分
                sascrr.CoreDomainScore81 = stud.CoreLearnDomainScore81;
                sascrr.CoreDomainScore82 = stud.CoreLearnDomainScore82;
                sascrr.CoreDomainScore91 = stud.CoreLearnDomainScore91;

                //先四捨五入後再相乘
                if (stud.AddWeight.HasValue)
                {
                    sascrr.CoreDomainScore81Add = Math.Round(sascrr.CoreDomainScore81.Value, 2, MidpointRounding.AwayFromZero) * stud.AddWeight.Value;
                    sascrr.CoreDomainScore82Add = Math.Round(sascrr.CoreDomainScore82.Value, 2, MidpointRounding.AwayFromZero) * stud.AddWeight.Value;
                    sascrr.CoreDomainScore91Add = Math.Round(sascrr.CoreDomainScore91.Value, 2, MidpointRounding.AwayFromZero) * stud.AddWeight.Value;
                }

                DateTime dt;

                if(DateTime.TryParse(stud.Birthday ,out dt))
                    sascrr.BirthDay =dt;

                if (stud.Gender == "男")
                    sascrr.GenderNo = "1";

                if (stud.Gender == "女")
                    sascrr.GenderNo = "2";

                PlaceCollection Places = stud.Places.NS("年排名");

                // 7大領域平均
                foreach (KeyValuePair<string,string> name in DomainNameDict)
                {
                    // 有領域成績
                    if (Places.Contains(name.Key))
                    {
                        Place p1 = Places[name.Key];

                        DomainAvgScore5Rpt dasr = new DomainAvgScore5Rpt();
                        dasr.DomainName = name.Value;
                        dasr.DomainScore = p1.Score;
                        dasr.DomainScoreRankPercent = p1.Percentage;
                        if (stud.AddWeight.HasValue && dasr.DomainScore.HasValue)
                        {
                            // 加分後成績
                            dasr.DomainScoreAdd = (stud.AddWeight.Value * dasr.DomainScore.Value).ToTwonPoint();
                            // 加分後排名百分比
                            dasr.DomainScoreRankPercentAdd = GetRankPercentAdd(name.Key, p1.Score, stud.GradeYear, stud.AddWeight.Value);
                        }
                        if(!sascrr.StudDomainscoreAvgDict.ContainsKey(name.Value ))
                            sascrr.StudDomainscoreAvgDict.Add(name.Value, dasr);
                    }
                }

                foreach (string DomainName in DomainNameDict.Values)
                {
                    DomainScore5Rpt ds5 = new DomainScore5Rpt();
                    ds5.Name =DomainName ;
                    sascrr.StudDomainSocreDict.Add(DomainName, ds5);
                }
                               
                foreach (SemesterScore ss in stud.SemestersScore)
                {
                    // 八上
                    if (ss.SchoolYear == gr2Sy && ss.Semester == 1)
                    {
                        sascrr.DomainScore81 = ss.LearnDomainScore;
                        if (stud.AddWeight.HasValue && sascrr.DomainScore81.HasValue)
                            sascrr.DomainScore81Add = (sascrr.DomainScore81.Value * stud.AddWeight.Value).ToTwonPoint();                        

                        foreach (KeyValuePair<string, DomainScore5Rpt> da in sascrr.StudDomainSocreDict)
                            if (ss.Domain.Contains(da.Key))
                            {
                                if (ss.Domain[da.Key].Value.HasValue)
                                    da.Value.Score81 = ss.Domain[da.Key].Value.Value;

                            }
                            else
                            {
                                // 新竹
                                if (Program.Mode == ModuleMode.HsinChu)
                                {
                                    if (da.Key.Equals("國語文"))
                                        da.Value.Score81 = ss.GetDomainScoreFromSubject("國").Item1;
                                    else if (da.Key.Equals("英語"))
                                        da.Value.Score81 = ss.GetDomainScoreFromSubject("英").Item1;
                                }
                            }
                    }

                    // 八下
                    if (ss.SchoolYear == gr2Sy && ss.Semester == 2)
                    {
                        sascrr.DomainScore82 = ss.LearnDomainScore;
                        if (stud.AddWeight.HasValue && sascrr.DomainScore82.HasValue)
                            sascrr.DomainScore82Add = (sascrr.DomainScore82.Value * stud.AddWeight.Value).ToTwonPoint();


                        foreach (KeyValuePair<string, DomainScore5Rpt> da in sascrr.StudDomainSocreDict)
                            if (ss.Domain.Contains(da.Key))
                            {
                                if (ss.Domain[da.Key].Value.HasValue)
                                    da.Value.Score82 = ss.Domain[da.Key].Value.Value;
                            }
                            else
                            {
                                // 新竹
                                if (Program.Mode == ModuleMode.HsinChu)
                                {
                                    if (da.Key.Equals("國語文"))
                                        da.Value.Score82 = ss.GetDomainScoreFromSubject("國").Item1;
                                    else if (da.Key.Equals("英語"))
                                        da.Value.Score82 = ss.GetDomainScoreFromSubject("英").Item1;
                                }
                            }
                    }

                    // 九上
                    if (ss.SchoolYear == gr3Sy && ss.Semester == 1)
                    {
                        sascrr.DomainScore91 = ss.LearnDomainScore;
                        if (stud.AddWeight.HasValue && sascrr.DomainScore91.HasValue)
                            sascrr.DomainScore91Add = (sascrr.DomainScore91.Value * stud.AddWeight.Value).ToTwonPoint();

                        foreach (KeyValuePair<string, DomainScore5Rpt> da in sascrr.StudDomainSocreDict)
                            if (ss.Domain.Contains(da.Key))
                            {
                                if (ss.Domain[da.Key].Value.HasValue)
                                    da.Value.Score91 = ss.Domain[da.Key].Value.Value;
                            } 
                            else
                            {
                                // 新竹
                                if (Program.Mode == ModuleMode.HsinChu)
                                {
                                    if (da.Key.Equals("國語文"))
                                        da.Value.Score91 = ss.GetDomainScoreFromSubject("國").Item1;
                                    else if (da.Key.Equals("英語"))
                                        da.Value.Score91 = ss.GetDomainScoreFromSubject("英").Item1;
                                }
                            }
                       
                    }
                }

                #region 學期領域平均
                if (Places.Contains("學習領域"))
                {
                    sascrr.DomainScoreAvg = Places["學習領域"].Score;
                    if (sascrr.DomainScoreAvg.HasValue && stud.AddWeight.HasValue)
                    {
                        //decimal value = ((sascrr.DomainScore81Add.GetValue() + sascrr.DomainScore82Add.GetValue() + sascrr.DomainScore91Add.GetValue()) / 3).ToTwonPoint();
                        decimal value = (sascrr.DomainScoreAvg.Value * stud.AddWeight.Value).ToTwonPoint();

                        sascrr.DomainScoreAvgAdd = value;
                        sascrr.DomainScoreAvgRankPercentAdd = GetRankPercentAdd("學期總平均", sascrr.DomainScoreAvg.Value, stud.GradeYear, stud.AddWeight.Value);
                    }
                    sascrr.DomainScoreAvgRankPercent = Places["學習領域"].Percentage;
                }

                if (Places.Contains("核心學習領域"))
                {
                    sascrr.CoreDomainScoreAvg = Places["核心學習領域"].Score;
                    if (sascrr.CoreDomainScoreAvg.HasValue && stud.AddWeight.HasValue)
                    {
                        //decimal value = ((sascrr.CoreDomainScore81Add.GetValue() + sascrr.CoreDomainScore82Add.GetValue() + sascrr.CoreDomainScore91Add.GetValue()) / 3).ToTwonPoint();
                        decimal value = (sascrr.CoreDomainScoreAvg.Value * stud.AddWeight.Value).ToTwonPoint();
                        sascrr.CoreDomainScoreAvgAdd = value;
                        sascrr.CoreDomainScoreAvgRankPercentAdd = GetRankPercentAdd("核心學期總平均", sascrr.CoreDomainScoreAvg.Value, stud.GradeYear, stud.AddWeight.Value);
                    }
                    sascrr.CoreDomainScoreAvgRankPercent = Places["核心學習領域"].Percentage;
                }
                #endregion

                retVal.Add(sascrr);
            }
            
            return retVal;        
        }

        
        /// <summary>
        /// 取得排名百分比
        /// </summary>
        /// <param name="places"></param>
        /// <param name="placeKey"></param>
        /// <returns></returns>
        private int? GetRankPercent(PlaceCollection places, string placeKey)
        {
            if (places.Contains(placeKey))
                return places[placeKey].Percentage;
            else
                return null;
        }

        /// <summary>
        /// 取得成績
        /// </summary>
        /// <param name="places"></param>
        /// <param name="placeKey"></param>
        /// <returns></returns>
        private decimal? GetScore(PlaceCollection places, string placeKey)
        {
            if (places.Contains(placeKey))
                return places[placeKey].Score;
            else
                return null;
        }


        /// <summary>
        /// 取得加分後排名百分比
        /// </summary>
        /// <param name="name"></param>
        /// <param name="score"></param>
        /// <param name="GradeYear"></param>
        /// <param name="AddPercent"></param>
        /// <returns></returns>
        public static int? GetRankPercentAdd(string name,decimal score,string GradeYear,decimal AddPercent)
        {
            int? retVal = 1;
            // 先取得年級，再分類別
            if (DAL.DALTransfer.StudRankScoreDict.ContainsKey(GradeYear))
            {
                if (DAL.DALTransfer.StudRankScoreDict[GradeYear].ContainsKey(name))
                { 
                    List<Place> PLi = (from data in DAL.DALTransfer.StudRankScoreDict[GradeYear][name] where data.Place.Score<=(score*AddPercent) orderby data.Place.Score descending  select data.Place).ToList ();
                    if (PLi.Count > 0)
                        retVal = PLi[0].Percentage;
                }            
            }
            return retVal;
        }

        private string GetPlaceString(PlaceCollection places, string placeKey)
        {
            Place place = places[placeKey];
            //decimal percentage = (100m * ((decimal)place.Level / (decimal)place.Radix));


            ////小於1%的話，就是1%。
            //if (percentage < 1) percentage = 1;

            string result = string.Empty;
            result = place.Percentage.ToString();

            //result = string.Format("{0}/{1}%", place.Level, place.Percentage);
            return result;
        }


        private static string GetString(string number)
        {
            int output;

            if (int.TryParse(number, out output))
                if (output <= 0) return string.Empty;

            return number;
        }

 
        private static string GetSemesterString(SemesterData each)
        {
            return string.Format("{0} {1}", each.SchoolYear.ToString(), each.Semester == 1 ? "上" : "下");
        }

    }
}