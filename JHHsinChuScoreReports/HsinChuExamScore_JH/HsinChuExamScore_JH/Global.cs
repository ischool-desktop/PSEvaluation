using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace HsinChuExamScore_JH
{
    public class Global
    {
        #region 設定檔記錄用

        /// <summary>
        /// UDT TableName
        /// </summary>
        public const string _UDTTableName = "ischool.新竹國中評量成績通知單.configure";

        public static string _ProjectName = "國中新竹評量成績單";

        public static string _DefaultConfTypeName = "預設設定檔";

        public static string _UserConfTypeName = "使用者選擇設定檔";

        /// <summary>
        /// 設定檔預設名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> DefaultConfigNameList()
        {
            List<string> retVal = new List<string>();
            retVal.Add("領域成績單");
            retVal.Add("科目成績單");
            retVal.Add("科目及領域成績單_領域組距");
            retVal.Add("科目及領域成績單_科目組距");          
            return retVal;
        }

        #endregion




        /// <summary>
        /// 固定領域名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> DomainNameList()
        {
            List<string> retVal = new List<string>();
            retVal.Add("語文");
            retVal.Add("數學");
            retVal.Add("社會");
            retVal.Add("自然與生活科技");
            retVal.Add("健康與體育");
            retVal.Add("藝術與人文");
            retVal.Add("綜合活動");
            retVal.Add("彈性課程");
            return retVal;
        }

        /// <summary>
        /// 取得獎懲名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDisciplineNameList()
        {
            return new string[] { "大功", "小功", "嘉獎", "大過", "小過", "警告"}.ToList();
        }


        /// <summary>
        /// Data Table 內需要加入合併欄位
        /// </summary>
        /// <returns></returns>
        public static List<string> DTColumnsList()
        {
            List<string> retVal = new List<string>();
            // 固定欄位
            retVal.Add("StudentID");
            retVal.Add("學校名稱");
            retVal.Add("學年度");
            retVal.Add("學期");
            retVal.Add("試別名稱");
            retVal.Add("班級");
            retVal.Add("學號");
            retVal.Add("座號");
            retVal.Add("姓名");
            retVal.Add("監護人姓名");
            retVal.Add("父親姓名");
            retVal.Add("母親姓名");
            retVal.Add("戶籍地址");
            retVal.Add("聯絡地址");
            retVal.Add("其他地址");
            retVal.Add("領域成績加權平均");
            retVal.Add("科目定期評量加權平均");
            retVal.Add("科目平時評量加權平均");
            retVal.Add("科目總成績加權平均");
            retVal.Add("領域成績加權平均(不含彈性)");
            retVal.Add("科目定期評量加權平均(不含彈性)");
            retVal.Add("科目平時評量加權平均(不含彈性)");
            retVal.Add("科目總成績加權平均(不含彈性)");
            retVal.Add("領域成績加權總分");
            retVal.Add("科目定期評量加權總分");
            retVal.Add("科目平時評量加權總分");
            retVal.Add("科目總成績加權總分");
            retVal.Add("領域成績加權總分(不含彈性)");
            retVal.Add("科目定期評量加權總分(不含彈性)");
            retVal.Add("科目平時評量加權總分(不含彈性)");
            retVal.Add("科目總成績加權總分(不含彈性)");
            // 獎懲名稱
            foreach (string str in GetDisciplineNameList())
                retVal.Add(str + "區間統計");

            retVal.Add("缺曠紀錄");
            retVal.Add("服務學習時數");
            retVal.Add("校長");
            retVal.Add("教務主任");
            retVal.Add("班導師");
            retVal.Add("區間開始日期");
            retVal.Add("區間結束日期");
            retVal.Add("成績校正日期");
            return retVal;
        }       

        /// <summary>
        /// 匯出合併欄位總表Word
        /// </summary>
        public static void ExportMappingFieldWord()
        {
            #region 儲存檔案
            string inputReportName = "新竹評量成績單合併欄位總表.doc";
            string reportName = inputReportName;

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".doc");

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

                System.IO.FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

                stream.Write(Properties.Resources.新竹評量成績合併欄位總表, 0, Properties.Resources.新竹評量成績合併欄位總表.Length);
                stream.Flush();
                stream.Close();
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".doc";
                sd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        System.IO.FileStream stream = new FileStream(sd.FileName, FileMode.Create, FileAccess.Write);
                        stream.Write(Properties.Resources.新竹評量成績合併欄位總表, 0, Properties.Resources.新竹評量成績合併欄位總表.Length);
                        stream.Flush();
                        stream.Close();

                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            #endregion
        }
    }
}
