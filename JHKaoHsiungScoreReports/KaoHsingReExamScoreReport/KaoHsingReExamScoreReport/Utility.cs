using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaoHsingReExamScoreReport.DAO;
using JHSchool.Data;
using K12.Data;
using System.Windows.Forms;
using System.IO;
using Aspose.Cells;
using FISCA.Presentation.Controls;

namespace KaoHsingReExamScoreReport
{
    public class Utility
    {

        /// <summary>
        /// 取得領域成績並判斷是否及格
        /// </summary>
        /// <param name="studDataList"></param>
        /// <param name="SchoolYear"></param>
        /// <param name="Semester"></param>
        /// <param name="passScore"></param>
        /// <returns></returns>
        public static List<StudentData> CalcStudDomainScorePass(List<StudentData> studDataList, int SchoolYear, int Semester,decimal passScore)
        {
            // 取得學期成績
            Dictionary<string, JHSemesterScoreRecord> studSemsScoreDict = new Dictionary<string, JHSemesterScoreRecord>();
            List<string> studIDList = (from data in studDataList select data.StudentID).ToList();
            List<JHSemesterScoreRecord> semsScoreList = JHSemesterScore.SelectByStudentIDs(studIDList);
            foreach (JHSemesterScoreRecord rec in semsScoreList)
            {
                if (rec.SchoolYear == SchoolYear && rec.Semester == Semester)
                    if (!studSemsScoreDict.ContainsKey(rec.RefStudentID))
                        studSemsScoreDict.Add(rec.RefStudentID, rec);
            }

            // 判斷學生領域成績是否及格
            foreach (StudentData sd in studDataList)
            {
                if (studSemsScoreDict.ContainsKey(sd.StudentID))
                {
                    sd.StudSemesterScoreRecord = studSemsScoreDict[sd.StudentID];
                    foreach (DomainScore ds in studSemsScoreDict[sd.StudentID].Domains.Values)
                    {                     
                        if (!sd.DomainScorePassDict.ContainsKey(ds.Domain))
                        {
                            sd.DomainScorePassDict.Add(ds.Domain, false);
                            sd.DomainScoreDict.Add(ds.Domain, 0);
                        }


                        if (ds.Score.HasValue)
                        {
                            
                            sd.DomainScoreDict[ds.Domain] = ds.Score.Value;
                            if (ds.Score.Value >= passScore)
                                sd.DomainScorePassDict[ds.Domain] = true;
                        }
                    }
                }
            }
            return studDataList;
        }

        /// <summary>
        /// 匯出 Excel
        /// </summary>
        /// <param name="inputReportName"></param>
        /// <param name="inputXls"></param>
        public static void CompletedXls(string inputReportName, Workbook inputXls)
        {
            string reportName = inputReportName;

            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".xls");

            Workbook wb = inputXls;

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

            try
            {
                wb.Save(path, SaveFormat.Excel97To2003);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".xls";
                sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, SaveFormat.Excel97To2003);

                    }
                    catch
                    {
                        MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 取得固定領域名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDomainNameList()
        {
            // 需要按照報表順序 (xlsx)
            List<string> retVal = new List<string>();
            retVal.Add("語文");
            retVal.Add("國語文");
            retVal.Add("英語");            
            retVal.Add("數學");
            retVal.Add("社會");
            retVal.Add("自然與生活科技");
            retVal.Add("藝術與人文");
            retVal.Add("健康與體育");
            retVal.Add("綜合活動");
            return retVal;
        }

    }
}
