using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Aspose.Cells;
using Campus.Rating;
using FISCA.Presentation.Controls;
using JHEvaluation.ScoreCalculation;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using JHSchool.Data;

namespace JointAdmissionModule.StudentScoreSummaryReport5
{
    internal class Report
    {
        private List<ReportStudent> Students { get; set; }

        private List<string> DetailDomain { get; set; }

        /// <summary>
        /// 檢查是否產生匯入格式檔案
        /// </summary>
        public static string checkOutputImportType = string.Empty;
        public static string checkAddressType = string.Empty;

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


        public Report(List<ReportStudent> students,string ReportTitle)
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
            wb.Open(new MemoryStream(Properties.Resources._100學年度國中在校成績證明單_五專一般));
            template.Open(new MemoryStream(Properties.Resources._100學年度國中在校成績證明單_五專一般));
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
            
            // 填入學生值 普通
            foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
            { 
                // 就讀國中                 
                wst.Cells[DataRow + 2, 0].PutValue(wst.Cells[DataRow + 2, 0].StringValue+sasr5r.SchoolName);
                // 就讀國中代碼
                wst.Cells[DataRow+2, 4].PutValue(wst.Cells[DataRow+2, 4].StringValue+sasr5r.SchoolCode);
                // 班級
                wst.Cells[DataRow+3, 0].PutValue(wst.Cells[DataRow+3, 0].StringValue+sasr5r.ClassName);
                // 姓名
                wst.Cells[DataRow+3, 2].PutValue(wst.Cells[DataRow+3, 2].StringValue+sasr5r.Name);
                // 身分證統一編號
                wst.Cells[DataRow+3, 4].PutValue(wst.Cells[DataRow+3, 4].StringValue+sasr5r.IDNumber);                                
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
                            RowMove=0;
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
                
                DataRow += RowNumber;
            }

            string FileName = "五專用國中在校成績證明單.xls";

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
                wb.Save(path, FileFormatType.Excel2003);
                if (MsgBox.Show(FileName + "產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(path);
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("儲存失敗");
            }
            #endregion

            #region 特種身分

            if (rptStudListSpec.Count > 0)
            {
                // 讀取樣板
                Workbook template1 = new Workbook();
                Workbook wb1 = new Workbook();
                Worksheet wst1;
                wb1.Open(new MemoryStream(Properties.Resources._100學年度國中在校成績證明單_五專特種));
                template1.Open(new MemoryStream(Properties.Resources._100學年度國中在校成績證明單_五專特種));
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
                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudListSpec)
                {
                    // 就讀國中
                    wst1.Cells[DataRow1 + 2, 0].PutValue(wst1.Cells[DataRow1 + 2, 0].StringValue+sasr5r.SchoolName);
                    // 就讀國中代碼
                    wst1.Cells[DataRow1 + 2, 4].PutValue(wst1.Cells[DataRow1 + 2, 4].StringValue+sasr5r.SchoolCode);
                    // 班級
                    wst1.Cells[DataRow1 + 3, 0].PutValue(wst1.Cells[DataRow1 + 3, 0].StringValue+sasr5r.ClassName);
                    // 姓名
                    wst1.Cells[DataRow1 + 3, 2].PutValue(wst1.Cells[DataRow1 + 3, 2].StringValue+sasr5r.Name);
                    // 身分證統一編號
                    wst1.Cells[DataRow1 + 3, 4].PutValue(wst1.Cells[DataRow1 + 3, 4].StringValue+sasr5r.IDNumber);

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
                            wst1.Cells[DataRow1 + RowMove, 4].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScore.GetFormat());  //領域學期成績
                            wst1.Cells[DataRow1 + RowMove, 5].PutValue(sasr5r.StudDomainscoreAvgDict[KeyName].DomainScoreRankPercent.GetFormat());  //領域學期成績排名百分比
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

                string FileName1="五專用國中在校成績證明單(特種身分).xls";
                // 儲存檔案
                try
                {
                    string path1 = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
                    DirectoryInfo dir = new DirectoryInfo(path1);
                    if (!dir.Exists) dir.Create();

                    path1 = Path.Combine(path1, FileName1);

                    if (File.Exists(path1))
                    {
                        int i = 1;
                        while (true)
                        {
                            string newPath = Path.GetDirectoryName(path1) + "\\" + Path.GetFileNameWithoutExtension(path1) + (i++) + Path.GetExtension(path1);
                            if (!File.Exists(newPath))
                            {
                                path1 = newPath;
                                break;
                            }
                        }
                    }

                    wb1.Save(path1, FileFormatType.Excel2003);


                    if (MsgBox.Show(FileName1 + "產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(path1);
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗");
                }
            }
            #endregion
                        
            #region 產生匯入格式檔

            // 確定要產生
            if (checkOutputImportType.Equals("南區"))
            {
                #region 南區
                // 讀取樣板
                Workbook wb2 = new Workbook();
                Worksheet wst2;
                wb2.Open(new MemoryStream(Properties.Resources._100五專免試國中報名系統檔));
                wst2 = wb2.Worksheets[0];

                // 學生ID List
                List<string> studIDList = (from data in rptStudList select data.StudentID).ToList();
                // 監護人資料
                List<JHParentRecord> ParentRecList = JHParent.SelectByStudentIDs(studIDList);
                // 地址
                List<JHAddressRecord> addressRecList = JHAddress.SelectByStudentIDs(studIDList);
                int RowIdx = 2;

                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
                {
                    // 學校代碼  Col:1
                    wst2.Cells[RowIdx, 1].PutValue(sasr5r.SchoolCode);
                    // 班級  Col:4
                    wst2.Cells[RowIdx, 4].PutValue(sasr5r.ClassName);
                    // 座號  Col:5
                    wst2.Cells[RowIdx, 5].PutValue(sasr5r.SeatNo);
                    // 學生姓名  Col:6
                    wst2.Cells[RowIdx, 6].PutValue(sasr5r.Name);
                    // 身分證統一編號  Col:7
                    wst2.Cells[RowIdx, 7].PutValue(sasr5r.IDNumber);
                    // 性別  Col:8
                    wst2.Cells[RowIdx, 8].PutValue(sasr5r.GenderNo);
                    if (sasr5r.BirthDay.HasValue)
                    {
                        // 出生年  Col:9
                        wst2.Cells[RowIdx, 9].PutValue(string.Format("{0:00}",(sasr5r.BirthDay.Value.Year-1911)));
                        // 出生月  Col:10
                        wst2.Cells[RowIdx, 10].PutValue(string.Format("{0:00}", sasr5r.BirthDay.Value.Month));
                        // 出生日  Col:11
                        wst2.Cells[RowIdx, 11].PutValue(string.Format("{0:00}", sasr5r.BirthDay.Value.Day));
                    }
                    
                    // 低收入戶  Col:18
                    wst2.Cells[RowIdx, 18].PutValue("0");
                    // 失業勞工子女  Col:19
                    wst2.Cells[RowIdx, 19].PutValue("0");

                    foreach (JHParentRecord rec in ParentRecList.Where(x => x.RefStudentID == sasr5r.StudentID))
                    {
                        // 家長姓名  Col:21
                        wst2.Cells[RowIdx, 21].PutValue(rec.CustodianName);
                        // 緊急聯絡市話  Col:22
                        // 過濾-()
                        string strTel = rec.CustodianPhone.Replace("-","");
                        strTel = strTel.Replace("(", "");
                        strTel = strTel.Replace(")", "");

                        wst2.Cells[RowIdx, 22].PutValue(strTel);
                    }

                    foreach (JHAddressRecord rec in addressRecList.Where(x => x.RefStudentID == sasr5r.StudentID))
                    {
                        string str = "";
                        if (checkAddressType == "戶籍")
                        {
                            // 郵遞區號  Col:23
                            wst2.Cells[RowIdx, 23].PutValue(rec.PermanentZipCode);
                            // 住址  Col:24
                            str = rec.PermanentCounty + rec.PermanentTown + rec.PermanentDistrict + rec.PermanentArea + rec.PermanentDetail;
                            wst2.Cells[RowIdx, 24].PutValue(str);
                        }
                        else
                        {
                            // 郵遞區號  Col:23
                            wst2.Cells[RowIdx, 23].PutValue(rec.MailingZipCode);
                            // 住址  Col:24
                            str = rec.MailingCounty + rec.MailingTown + rec.MailingDistrict + rec.MailingArea + rec.MailingDetail;
                            wst2.Cells[RowIdx, 24].PutValue(str);
                        }
                    }

                    // 免試學校代碼  Col:15
                                        
                    // 國語文  Col:16
                    wst2.Cells[RowIdx, 28].PutValue(GetStudDomainscoreAvgPercentRank("國語文", sasr5r.StudDomainscoreAvgDict, false));
                    // 英語  Col:17
                    wst2.Cells[RowIdx, 29].PutValue(GetStudDomainscoreAvgPercentRank("英語", sasr5r.StudDomainscoreAvgDict, false));
                    // 數學  Col:18
                    wst2.Cells[RowIdx, 30].PutValue(GetStudDomainscoreAvgPercentRank("數學", sasr5r.StudDomainscoreAvgDict, false));
                    // 社會  Col:19
                    wst2.Cells[RowIdx, 31].PutValue(GetStudDomainscoreAvgPercentRank("社會", sasr5r.StudDomainscoreAvgDict, false));
                    // 自然與生活科技  Col:20
                    wst2.Cells[RowIdx, 32].PutValue(GetStudDomainscoreAvgPercentRank("自然與生活科技", sasr5r.StudDomainscoreAvgDict, false));
                    // 藝術與人文  Col:21
                    wst2.Cells[RowIdx, 33].PutValue(GetStudDomainscoreAvgPercentRank("藝術與人文", sasr5r.StudDomainscoreAvgDict, false));
                    // 健康與體育  Col:22
                    wst2.Cells[RowIdx, 34].PutValue(GetStudDomainscoreAvgPercentRank("健康與體育", sasr5r.StudDomainscoreAvgDict, false));
                    // 綜合活動  Col:23
                    wst2.Cells[RowIdx, 35].PutValue(GetStudDomainscoreAvgPercentRank("綜合活動", sasr5r.StudDomainscoreAvgDict, false));
                    // 總成績  Col:24
                    if(sasr5r.DomainScoreAvgRankPercent.HasValue)
                        wst2.Cells[RowIdx, 36].PutValue(sasr5r.DomainScoreAvgRankPercent.Value);
                    // 特種身分  Col:25
                    if(string.IsNullOrEmpty(sasr5r.StudSpecTypeCode))
                        wst2.Cells[RowIdx, 37].PutValue("0");
                    else
                        wst2.Cells[RowIdx, 37].PutValue(sasr5r.StudSpecTypeCode);

                    // 特國語文  Col:26
                    wst2.Cells[RowIdx, 38].PutValue(GetStudDomainscoreAvgPercentRank("國語文", sasr5r.StudDomainscoreAvgDict, true));
                    // 特英語  Col:27
                    wst2.Cells[RowIdx, 39].PutValue(GetStudDomainscoreAvgPercentRank("英語", sasr5r.StudDomainscoreAvgDict, true));
                    // 特數學  Col:28
                    wst2.Cells[RowIdx, 40].PutValue(GetStudDomainscoreAvgPercentRank("數學", sasr5r.StudDomainscoreAvgDict, true));
                    // 特社會  Col:29
                    wst2.Cells[RowIdx, 41].PutValue(GetStudDomainscoreAvgPercentRank("社會", sasr5r.StudDomainscoreAvgDict, true));
                    // 特自然與生活科技  Col:30
                    wst2.Cells[RowIdx, 42].PutValue(GetStudDomainscoreAvgPercentRank("自然與生活科技", sasr5r.StudDomainscoreAvgDict, true));
                    // 特藝術與人文  Col:31
                    wst2.Cells[RowIdx, 43].PutValue(GetStudDomainscoreAvgPercentRank("藝術與人文", sasr5r.StudDomainscoreAvgDict, true));
                    // 特健康與體育  Col:32
                    wst2.Cells[RowIdx, 44].PutValue(GetStudDomainscoreAvgPercentRank("健康與體育", sasr5r.StudDomainscoreAvgDict, true));
                    // 特綜合活動  Col:33
                    wst2.Cells[RowIdx, 45].PutValue(GetStudDomainscoreAvgPercentRank("綜合活動", sasr5r.StudDomainscoreAvgDict, true));
                    
                    // 特總成績  Col:34
                    if (sasr5r.DomainScoreAvgRankPercentAdd.HasValue)
                        wst2.Cells[RowIdx, 46].PutValue(sasr5r.DomainScoreAvgRankPercentAdd.Value);

                    // 二上國  Col:35
                    wst2.Cells[RowIdx, 47].PutValue(GetStudDomainscorePercentRank("國語文", sasr5r.StudDomainSocreDict, 81));
                    // 二下國  Col:43
                    wst2.Cells[RowIdx, 55].PutValue(GetStudDomainscorePercentRank("國語文", sasr5r.StudDomainSocreDict, 82));
                    // 三上國  Col:51
                    wst2.Cells[RowIdx, 63].PutValue(GetStudDomainscorePercentRank("國語文", sasr5r.StudDomainSocreDict, 91));

                    // 二上英  Col:36
                    wst2.Cells[RowIdx, 48].PutValue(GetStudDomainscorePercentRank("英語", sasr5r.StudDomainSocreDict, 81));
                    // 二下英  Col:44
                    wst2.Cells[RowIdx, 56].PutValue(GetStudDomainscorePercentRank("英語", sasr5r.StudDomainSocreDict, 82));
                    // 三上英  Col:52
                    wst2.Cells[RowIdx, 64].PutValue(GetStudDomainscorePercentRank("英語", sasr5r.StudDomainSocreDict, 91));

                    
                    // 二上數  Col:37
                    wst2.Cells[RowIdx, 49].PutValue(GetStudDomainscorePercentRank("數學", sasr5r.StudDomainSocreDict, 81));
                    // 二下數  Col:45
                    wst2.Cells[RowIdx, 57].PutValue(GetStudDomainscorePercentRank("數學", sasr5r.StudDomainSocreDict, 82));
                    // 三上數  Col:53
                    wst2.Cells[RowIdx, 65].PutValue(GetStudDomainscorePercentRank("數學", sasr5r.StudDomainSocreDict, 91));

                    // 二上社  Col:38
                    wst2.Cells[RowIdx, 50].PutValue(GetStudDomainscorePercentRank("社會", sasr5r.StudDomainSocreDict, 81));
                    // 二下社  Col:46
                    wst2.Cells[RowIdx, 58].PutValue(GetStudDomainscorePercentRank("社會", sasr5r.StudDomainSocreDict, 82));
                    // 三上社  Col:54
                    wst2.Cells[RowIdx, 66].PutValue(GetStudDomainscorePercentRank("社會", sasr5r.StudDomainSocreDict, 91));

                    // 二上自  Col:39
                    wst2.Cells[RowIdx, 51].PutValue(GetStudDomainscorePercentRank("自然與生活科技", sasr5r.StudDomainSocreDict, 81));
                    // 二下自  Col:47
                    wst2.Cells[RowIdx, 59].PutValue(GetStudDomainscorePercentRank("自然與生活科技", sasr5r.StudDomainSocreDict, 82));
                    // 三上自  Col:55
                    wst2.Cells[RowIdx, 67].PutValue(GetStudDomainscorePercentRank("自然與生活科技", sasr5r.StudDomainSocreDict, 91));


                    // 二上藝  Col:40
                    wst2.Cells[RowIdx, 52].PutValue(GetStudDomainscorePercentRank("藝術與人文", sasr5r.StudDomainSocreDict, 81));
                    // 二下藝  Col:48
                    wst2.Cells[RowIdx, 60].PutValue(GetStudDomainscorePercentRank("藝術與人文", sasr5r.StudDomainSocreDict, 82));
                    // 三上藝  Col:56
                    wst2.Cells[RowIdx, 68].PutValue(GetStudDomainscorePercentRank("藝術與人文", sasr5r.StudDomainSocreDict, 91));

                    // 二上健  Col:41
                    wst2.Cells[RowIdx, 53].PutValue(GetStudDomainscorePercentRank("健康與體育", sasr5r.StudDomainSocreDict, 81));
                    // 二下健  Col:49
                    wst2.Cells[RowIdx, 61].PutValue(GetStudDomainscorePercentRank("健康與體育", sasr5r.StudDomainSocreDict, 82));
                    // 三上健  Col:57
                    wst2.Cells[RowIdx, 69].PutValue(GetStudDomainscorePercentRank("健康與體育", sasr5r.StudDomainSocreDict, 91));

                    // 二上綜  Col:42
                    wst2.Cells[RowIdx, 54].PutValue(GetStudDomainscorePercentRank("綜合活動", sasr5r.StudDomainSocreDict, 81));
                    // 二下綜  Col:50
                    wst2.Cells[RowIdx, 62].PutValue(GetStudDomainscorePercentRank("綜合活動", sasr5r.StudDomainSocreDict, 82));
                    // 三上綜  Col:58
                    wst2.Cells[RowIdx, 70].PutValue(GetStudDomainscorePercentRank("綜合活動", sasr5r.StudDomainSocreDict, 91));
                    
                    RowIdx++;
                }

                string FileName2 = "五專免試國中報名匯入檔案.xls";

                Campus.Report.ReportSaver.SaveWorkbook(wb2, FileName2);
                #endregion
            }
            else if (checkOutputImportType.Equals("中區"))
            {
                #region 中區
                // 讀取樣板
                Workbook wb2 = new Workbook();
                Worksheet wst2;
                wb2.Open(new MemoryStream(Properties.Resources._100中區五專免試國中報名系統檔));
                wst2 = wb2.Worksheets[0];

                // 學生ID List
                List<string> studIDList = (from data in rptStudList select data.StudentID).ToList();
                // 監護人資料
                List<JHParentRecord> ParentRecList = JHParent.SelectByStudentIDs(studIDList);
                // 地址
                List<JHAddressRecord> addressRecList = JHAddress.SelectByStudentIDs(studIDList);

                
                int RowIdx = 1;

                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
                {
                    //考區代碼
                    wst2.Cells[RowIdx, 0].PutValue(string.Empty);
                    //學校代碼
                    wst2.Cells[RowIdx, 1].PutValue(sasr5r.SchoolCode);
                    //報名序號
                    wst2.Cells[RowIdx, 2].PutValue(string.Empty);
                    //學號
                    wst2.Cells[RowIdx, 3].PutValue(sasr5r.StudentNumber);
                    //班級  Col:4
                    wst2.Cells[RowIdx, 4].PutValue(sasr5r.ClassName);
                    // 座號  Col:5
                    wst2.Cells[RowIdx, 5].PutValue(sasr5r.SeatNo);
                    // 學生姓名  Col:6
                    wst2.Cells[RowIdx, 6].PutValue(sasr5r.Name);
                    // 身分證統一編號  Col:7
                    wst2.Cells[RowIdx, 7].PutValue(sasr5r.IDNumber);                    

                    // 性別  Col:8
                    wst2.Cells[RowIdx, 8].PutValue(sasr5r.GenderNo);
                    if (sasr5r.BirthDay.HasValue)
                    {
                        // 出生年  Col:9
                        wst2.Cells[RowIdx, 9].PutValue(string.Format("{0:00}", (sasr5r.BirthDay.Value.Year - 1911)));
                        // 出生月  Col:10
                        wst2.Cells[RowIdx, 10].PutValue(string.Format("{0:00}", sasr5r.BirthDay.Value.Month));
                        // 出生日  Col:11
                        wst2.Cells[RowIdx, 11].PutValue(string.Format("{0:00}", sasr5r.BirthDay.Value.Day));
                    }

                    //畢業學校	
                    wst2.Cells[RowIdx, 12].PutValue(string.Empty);
                    //畢業年度	
                    wst2.Cells[RowIdx, 13].PutValue(string.Empty);
                    //畢肄業	
                    wst2.Cells[RowIdx, 14].PutValue(string.Empty);
                    //學生身分	
                    wst2.Cells[RowIdx, 15].PutValue(sasr5r.StudSpecTypeCode);                    
                    //身心障礙	
                    wst2.Cells[RowIdx, 16].PutValue(string.Empty);
                    //分發區	
                    wst2.Cells[RowIdx, 17].PutValue(string.Empty);
                    //低收入戶	
                    wst2.Cells[RowIdx, 18].PutValue("0");                    
                    //失業給付者	
                    wst2.Cells[RowIdx, 19].PutValue("0");
                    //資料授權	
                    
                    //家長姓名	電話	                  
                    foreach (JHParentRecord rec in ParentRecList.Where(x => x.RefStudentID == sasr5r.StudentID))
                    {
                        // 家長姓名  Col:21
                        wst2.Cells[RowIdx, 21].PutValue(rec.CustodianName);
                        // 緊急聯絡市話  Col:22
                        // 過濾-()
                        string strTel = rec.CustodianPhone.Replace("-", "");
                        strTel = strTel.Replace("(", "");
                        strTel = strTel.Replace(")", "");

                        wst2.Cells[RowIdx, 22].PutValue(strTel);
                    }

                    //郵遞區號	地址	
                    foreach (JHAddressRecord rec in addressRecList.Where(x => x.RefStudentID == sasr5r.StudentID))
                    {
                        string str = "";
                        if (checkAddressType == "戶籍")
                        {
                            // 郵遞區號  Col:23
                            wst2.Cells[RowIdx, 23].PutValue(rec.PermanentZipCode);
                            // 住址  Col:24
                            str = rec.PermanentCounty + rec.PermanentTown + rec.PermanentDistrict + rec.PermanentArea + rec.PermanentDetail;
                            wst2.Cells[RowIdx, 24].PutValue(str);
                        }
                        else
                        {
                            // 郵遞區號  Col:23
                            wst2.Cells[RowIdx, 23].PutValue(rec.MailingZipCode);
                            // 住址  Col:24
                            str = rec.MailingCounty + rec.MailingTown + rec.MailingDistrict + rec.MailingArea + rec.MailingDetail;
                            wst2.Cells[RowIdx, 24].PutValue(str);
                        }
                    }

                    //各領域排名
                    for (int i = 0; i < DomainNameList.Count; i++)
                        wst2.Cells[RowIdx, 25 + i].PutValue(GetStudDomainscoreAvgPercentRank(DomainNameList[i], sasr5r.StudDomainscoreAvgDict, false));
                    //總成績
                    wst2.Cells[RowIdx, 33].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());
                    //各領域加分後排名
                    for (int i = 0; i < DomainNameList.Count; i++)
                        wst2.Cells[RowIdx, 34 + i].PutValue(GetStudDomainscoreAvgPercentRank(DomainNameList[i], sasr5r.StudDomainscoreAvgDict, true));

                    //加分後總成績
                    wst2.Cells[RowIdx, 42].PutValue(sasr5r.DomainScoreAvgRankPercentAdd.GetFormat());
                    
                    RowIdx++;
                }

                string FileName2 = "五專免試國中報名匯入檔案.xls";

                Campus.Report.ReportSaver.SaveWorkbook(wb2, FileName2);
                #endregion
            }
            else if (checkOutputImportType.Equals("北區"))
            {
                #region 北區
                // 讀取樣板
                Workbook wb2 = new Workbook();
                Worksheet wst2;
                wb2.Open(new MemoryStream(Properties.Resources._100北區五專免試國中報名系統檔));
                wst2 = wb2.Worksheets[0];              
                
                // 學生ID List
                List<string> studIDList = (from data in rptStudList select data.StudentID).ToList();
                // 監護人資料
                List<JHParentRecord> ParentRecList = JHParent.SelectByStudentIDs(studIDList);
                // 地址
                List<JHAddressRecord> addressRecList = JHAddress.SelectByStudentIDs(studIDList);
                int RowIdx = 1;

                foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
                {
                    //學校代碼
                    wst2.Cells[RowIdx, 0].PutValue(sasr5r.SchoolCode);
                    //國中報名序號
                    wst2.Cells[RowIdx, 1].PutValue(string.Empty);
                    //班級
                    wst2.Cells[RowIdx, 2].PutValue(sasr5r.ClassName);
                    //座號
                    wst2.Cells[RowIdx, 3].PutValue(sasr5r.SeatNo);
                    //學生姓名
                    wst2.Cells[RowIdx, 4].PutValue(sasr5r.Name);
                    //身分證字號	
                    wst2.Cells[RowIdx, 5].PutValue(sasr5r.IDNumber);
                    //性別	
                    wst2.Cells[RowIdx, 6].PutValue(sasr5r.GenderNo);

                    if (sasr5r.BirthDay.HasValue)
                    {
                        // 出生年
                        wst2.Cells[RowIdx, 7].PutValue(string.Format("{0:00}", (sasr5r.BirthDay.Value.Year - 1911)));
                        // 出生月
                        wst2.Cells[RowIdx, 8].PutValue(string.Format("{0:00}", sasr5r.BirthDay.Value.Month));
                        // 出生日
                        wst2.Cells[RowIdx, 9].PutValue(string.Format("{0:00}", sasr5r.BirthDay.Value.Day));
                    }
                    
                    //家長姓名	緊急連絡電話	
                    foreach (JHParentRecord rec in ParentRecList.Where(x => x.RefStudentID == sasr5r.StudentID))
                    {
                        // 家長姓名
                        wst2.Cells[RowIdx, 10].PutValue(rec.CustodianName);
                        // 緊急聯絡市話
                        // 過濾-()
                        string strTel = rec.CustodianPhone.Replace("-", "");
                        strTel = strTel.Replace("(", "");
                        strTel = strTel.Replace(")", "");
                        wst2.Cells[RowIdx, 11].PutValue(strTel);
                    }

                    //郵遞區號  地址	
                    foreach (JHAddressRecord rec in addressRecList.Where(x => x.RefStudentID == sasr5r.StudentID))
                    {
                        string str = "";
                        if (checkAddressType == "戶籍")
                        {
                            // 郵遞區號 
                            wst2.Cells[RowIdx, 12].PutValue(rec.PermanentZipCode);
                            // 住址
                            str = rec.PermanentCounty + rec.PermanentTown + rec.PermanentDistrict + rec.PermanentArea + rec.PermanentDetail;
                            wst2.Cells[RowIdx, 13].PutValue(str);
                        }
                        else
                        {
                            // 郵遞區號
                            wst2.Cells[RowIdx, 12].PutValue(rec.MailingZipCode);
                            // 住址
                            str = rec.MailingCounty + rec.MailingTown + rec.MailingDistrict + rec.MailingArea + rec.MailingDetail;
                            wst2.Cells[RowIdx, 13].PutValue(str);
                        }
                    }

                    //考生身分別
                    wst2.Cells[RowIdx, 14].PutValue(sasr5r.StudSpecTypeCode);   
                    //報名方式          
                    wst2.Cells[RowIdx, 15].PutValue("0");                    
                    //報名學校代碼
                    wst2.Cells[RowIdx, 16].PutValue(string.Empty);
                    //中低收入戶	
                    wst2.Cells[RowIdx, 17].PutValue("0");
                    //低收入戶	
                    wst2.Cells[RowIdx, 18].PutValue("0");
                    //失業給付	
                    wst2.Cells[RowIdx, 19].PutValue("0");

                    //各領域排名
                    for(int i=0;i<DomainNameList.Count;i++)
                        wst2.Cells[RowIdx, 20+i].PutValue(GetStudDomainscoreAvgPercentRank(DomainNameList[i],sasr5r.StudDomainscoreAvgDict, false));
                    //總成績排名
                    wst2.Cells[RowIdx, 28].PutValue(sasr5r.DomainScoreAvgRankPercent.GetFormat());
                    //各領域加分後排名
                    for (int i = 0; i < DomainNameList.Count; i++)
                        wst2.Cells[RowIdx, 29 + i].PutValue(GetStudDomainscoreAvgPercentRank(DomainNameList[i], sasr5r.StudDomainscoreAvgDict,true));
                    //加分後總成績排名
                    wst2.Cells[RowIdx, 37].PutValue(sasr5r.DomainScoreAvgRankPercentAdd.GetFormat());

                    RowIdx++;
                }

                string FileName2 = "五專免試國中報名匯入檔案.xls";

                Campus.Report.ReportSaver.SaveWorkbook(wb2, FileName2);
                #endregion
            }
            #endregion


            //#region 產生匯入新竹高中匯入格式
            //if(Program.Mode == ModuleMode.HsinChu)
            //{
            //            // 確定要產生
            //    if (checkOutputImportFile)
            //    {
            //        // 讀取樣板
            //        Workbook wb3 = new Workbook();
            //        Worksheet wst3;
            //        wb3.Open(new MemoryStream(Properties.Resources._100新竹高中免試國中報名系統檔));
            //        wst3 = wb3.Worksheets[0];

            //        int RowIdx = 1;
            //        foreach (StudentAvgScoreRank5Rpt sasr5r in rptStudList)
            //        {

            //            // 學生姓名: 0
            //            wst3.Cells[RowIdx, 0].PutValue(sasr5r.Name);

            //            // 身分證字號: 1
            //            wst3.Cells[RowIdx, 1].PutValue(sasr5r.IDNumber);

            //            // 學生身分: 2
            //            if (string.IsNullOrEmpty(sasr5r.StudSpecTypeCode))
            //                wst3.Cells[RowIdx, 2].PutValue("0");
            //            else
            //                wst3.Cells[RowIdx, 2].PutValue(sasr5r.StudSpecTypeCode);

            //            // 身心障礙學生: 3

            //            // 學業成績總平均: 4
            //            if (sasr5r.DomainScoreAvg.HasValue)
            //                wst3.Cells[RowIdx, 4].PutValue(Math.Round(sasr5r.DomainScoreAvg.Value,2,MidpointRounding.AwayFromZero));

            //            // 學業成績全校排名百分比: 5
            //            if(sasr5r.DomainScoreAvgRankPercent.HasValue )
            //                 wst3.Cells[RowIdx, 5].PutValue(sasr5r.DomainScoreAvgRankPercent.Value );

            //            // 國文成績總平均: 6
            //            wst3.Cells[RowIdx, 6].PutValue(GetStudDomainscoreAvg("國語文", sasr5r.StudDomainscoreAvgDict,false));

            //            // 國文成績全校排名百分比: 7
            //            wst3.Cells[RowIdx, 7].PutValue(GetStudDomainscoreAvgPercentRank("國語文", sasr5r.StudDomainscoreAvgDict,false));

            //            // 英語成績總平均: 8
            //            wst3.Cells[RowIdx, 8].PutValue(GetStudDomainscoreAvg("英語", sasr5r.StudDomainscoreAvgDict,false));
            //            // 英語成績全校排名百分比: 9
            //            wst3.Cells[RowIdx, 9].PutValue(GetStudDomainscoreAvgPercentRank("英語", sasr5r.StudDomainscoreAvgDict,false));

            //            // 數學成績總平均: 10
            //            wst3.Cells[RowIdx, 10].PutValue(GetStudDomainscoreAvg("數學", sasr5r.StudDomainscoreAvgDict,false));
            //            // 數學成績全校排名百分比: 11
            //            wst3.Cells[RowIdx, 11].PutValue(GetStudDomainscoreAvgPercentRank("數學", sasr5r.StudDomainscoreAvgDict,false));
            //            // 社會成績總平均: 12
            //            wst3.Cells[RowIdx, 12].PutValue(GetStudDomainscoreAvg("社會", sasr5r.StudDomainscoreAvgDict,false));
            //            // 社會成績全校排名百分比: 13
            //            wst3.Cells[RowIdx, 13].PutValue(GetStudDomainscoreAvgPercentRank("社會", sasr5r.StudDomainscoreAvgDict,false));
            //            // 自然成績總平均: 14
            //            wst3.Cells[RowIdx, 14].PutValue(GetStudDomainscoreAvg("自然與生活科技", sasr5r.StudDomainscoreAvgDict,false));
            //            // 自然成績全校排名百分比: 15
            //            wst3.Cells[RowIdx, 15].PutValue(GetStudDomainscoreAvgPercentRank("自然與生活科技", sasr5r.StudDomainscoreAvgDict,false));
            //            // 藝術與人文成績總平均: 16
            //            wst3.Cells[RowIdx, 16].PutValue(GetStudDomainscoreAvg("藝術與人文", sasr5r.StudDomainscoreAvgDict,false));
            //            // 藝術與人文成績全校排名百分比: 17
            //            wst3.Cells[RowIdx, 17].PutValue(GetStudDomainscoreAvgPercentRank("藝術與人文", sasr5r.StudDomainscoreAvgDict,false));
            //            // 健康與體育成績總平均: 18
            //            wst3.Cells[RowIdx, 18].PutValue(GetStudDomainscoreAvg("健康與體育", sasr5r.StudDomainscoreAvgDict,false));
            //            // 健康與體育成績全校排名百分比: 19
            //            wst3.Cells[RowIdx, 19].PutValue(GetStudDomainscoreAvgPercentRank("健康與體育", sasr5r.StudDomainscoreAvgDict,false));
            //            // 綜合活動成績總平均: 20
            //            wst3.Cells[RowIdx, 20].PutValue(GetStudDomainscoreAvg("綜合活動", sasr5r.StudDomainscoreAvgDict,false));
            //            // 綜合活動成績全校排名百分比: 21
            //            wst3.Cells[RowIdx, 21].PutValue(GetStudDomainscoreAvgPercentRank("綜合活動", sasr5r.StudDomainscoreAvgDict,false));
            //            // 學業成績總平均(加分後): 22
            //            if(sasr5r.DomainScoreAvgAdd.HasValue )
            //                wst3.Cells[RowIdx,22].PutValue(Math.Round(sasr5r.DomainScoreAvgAdd.Value ,2,MidpointRounding.AwayFromZero ));
            //            // 學業成績全校排名百分比(加分後): 23
            //            if(sasr5r.DomainScoreAvgRankPercentAdd.HasValue )
            //                wst3.Cells[RowIdx,23].PutValue(sasr5r.DomainScoreAvgRankPercentAdd.Value);

            //            // 國文成績總平均(加分後): 24
            //            wst3.Cells[RowIdx, 24].PutValue(GetStudDomainscoreAvg("國語文", sasr5r.StudDomainscoreAvgDict,true));
            //            // 國文成績全校排名百分比(加分後): 25
            //            wst3.Cells[RowIdx, 25].PutValue(GetStudDomainscoreAvgPercentRank("國語文", sasr5r.StudDomainscoreAvgDict,true));
            //            // 英語成績總平均(加分後): 26
            //            wst3.Cells[RowIdx, 26].PutValue(GetStudDomainscoreAvg("英語", sasr5r.StudDomainscoreAvgDict,true));
            //            // 英語成績全校排名百分比(加分後): 27
            //            wst3.Cells[RowIdx, 27].PutValue(GetStudDomainscoreAvgPercentRank("英語", sasr5r.StudDomainscoreAvgDict,true));
            //            // 數學成績總平均(加分後): 28
            //            wst3.Cells[RowIdx, 28].PutValue(GetStudDomainscoreAvg("數學", sasr5r.StudDomainscoreAvgDict,true));
            //            // 數學成績全校排名百分比(加分後): 29
            //            wst3.Cells[RowIdx, 29].PutValue(GetStudDomainscoreAvgPercentRank("數學", sasr5r.StudDomainscoreAvgDict,true));
            //            // 社會成績總平均(加分後): 30
            //            wst3.Cells[RowIdx, 30].PutValue(GetStudDomainscoreAvg("社會", sasr5r.StudDomainscoreAvgDict,true));
            //            // 社會成績全校排名百分比(加分後): 31
            //            wst3.Cells[RowIdx, 31].PutValue(GetStudDomainscoreAvgPercentRank("社會", sasr5r.StudDomainscoreAvgDict,true));
            //            // 自然成績總平均(加分後): 32
            //            wst3.Cells[RowIdx, 32].PutValue(GetStudDomainscoreAvg("自然與生活科技", sasr5r.StudDomainscoreAvgDict,true));
            //            // 自然成績全校排名百分比(加分後): 33
            //            wst3.Cells[RowIdx, 33].PutValue(GetStudDomainscoreAvgPercentRank("自然與生活科技", sasr5r.StudDomainscoreAvgDict,true));
            //            // 藝術與人文成績總平均(加分後): 34
            //            wst3.Cells[RowIdx, 34].PutValue(GetStudDomainscoreAvg("藝術與人文", sasr5r.StudDomainscoreAvgDict,true));
            //            // 藝術與人文成績全校排名百分比(加分後): 35
            //            wst3.Cells[RowIdx, 35].PutValue(GetStudDomainscoreAvgPercentRank("藝術與人文", sasr5r.StudDomainscoreAvgDict,true));
            //            // 健康與體育成績總平均(加分後): 36
            //            wst3.Cells[RowIdx, 36].PutValue(GetStudDomainscoreAvg("健康與體育", sasr5r.StudDomainscoreAvgDict,true));
            //            // 健康與體育成績全校排名百分比(加分後): 37
            //            wst3.Cells[RowIdx, 37].PutValue(GetStudDomainscoreAvgPercentRank("健康與體育", sasr5r.StudDomainscoreAvgDict,true));
            //            // 綜合活動成績總平均(加分後): 38
            //            wst3.Cells[RowIdx, 38].PutValue(GetStudDomainscoreAvgPercentRank("綜合活動", sasr5r.StudDomainscoreAvgDict,true));
            //            // 綜合活動成績全校排名百分比(加分後): 39
            //            wst3.Cells[RowIdx, 39].PutValue(GetStudDomainscoreAvgPercentRank("綜合活動", sasr5r.StudDomainscoreAvgDict,true));

            //            RowIdx++;
            //        }

            //        string FileName3 = "100學年度新竹高中免試國中報名匯入檔案.xls";

            //        // 儲存檔案
            //        try
            //        {
            //            string path3 = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            //            DirectoryInfo dir = new DirectoryInfo(path3);
            //            if (!dir.Exists) dir.Create();

            //            path3 = Path.Combine(path3, FileName3);

            //            if (File.Exists(path3))
            //            {
            //                int i = 1;
            //                while (true)
            //                {
            //                    string newPath = Path.GetDirectoryName(path3) + "\\" + Path.GetFileNameWithoutExtension(path3) + (i++) + Path.GetExtension(path3);
            //                    if (!File.Exists(newPath))
            //                    {
            //                        path3 = newPath;
            //                        break;
            //                    }
            //                }
            //            }
            //            wb3.Save(path3, FileFormatType.Excel97);
            //            if (MsgBox.Show(FileName3 + "產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //            {
            //                System.Diagnostics.Process.Start(path3);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            MsgBox.Show("儲存失敗");
            //        }
            //    }
            //}
            //#endregion
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
            List<JointAdmissionModule.DAL.UserDefData_StudTypeWeight> udtList = DAL.UDTTransfer.GetDataFromUDT_StudTypeWeight().Where(x => x.SchoolType == "五專").ToList ();
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
                sascrr.StudentNumber = stud.StudentNumber;
                sascrr.Name = stud.Name;
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
                
                // 學期領域平均
                if(Places.Contains("學習領域"))
                {
                    
                    sascrr.DomainScoreAvg = Places["學習領域"].Score;
                    if (sascrr.DomainScoreAvg.HasValue && stud.AddWeight.HasValue)
                    {
                        sascrr.DomainScoreAvgAdd = (sascrr.DomainScoreAvg.Value * stud.AddWeight.Value).ToTwonPoint();
                        sascrr.DomainScoreAvgRankPercentAdd = GetRankPercentAdd("學期總平均", sascrr.DomainScoreAvg.Value, stud.GradeYear, stud.AddWeight.Value);
                    }
                    sascrr.DomainScoreAvgRankPercent = Places["學習領域"].Percentage;
                    
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