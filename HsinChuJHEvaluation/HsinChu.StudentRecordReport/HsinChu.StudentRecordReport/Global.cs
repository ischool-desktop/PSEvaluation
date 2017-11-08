using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HsinChu.StudentRecordReport
{
    internal class Global
    {
        public static string ReportName = "學籍表";

        /// <summary>
        /// 服務學時數暫存使用
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> _SLRDict = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 2012/9/2 - 單檔儲存設定
        /// </summary>
        public static string OneFileSave = "學籍表_檔案儲存設定";

        public static string CDate(string p)
        {
            DateTime d = DateTime.Now;
            if (p != "" && DateTime.TryParse(p, out d))
            {
                return "" + (d.Year - 1911) + "/" + d.Month + "/" + d.Day;
            }
            else
                return "";
        }

        /// <summary>
        /// 日常生活表現名稱對照使用
        /// </summary>
        internal static Dictionary<string, string> DLBehaviorConfigNameDict = new Dictionary<string, string>();

        /// <summary>
        /// 日常生活表現子項目名稱,呼叫GetDLBehaviorConfigNameDict 一同取得
        /// </summary>
        internal static Dictionary<string, List<string>> DLBehaviorConfigItemNameDict = new Dictionary<string, List<string>>();

        /// <summary>
        /// XML 內解析子項目名稱
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        internal static List<string> ParseItems(XElement elm)
        {
            List<string> retVal = new List<string>();

            foreach (XElement subElm in elm.Elements("Item"))
            {
                // 因為社團功能，所以要將"社團活動" 字不放入
                string name = subElm.Attribute("Name").Value;
                if (name != "社團活動")
                    retVal.Add(name);
            }
            return retVal;
        }

        /// <summary>
        /// 取得日常生活表現設定名稱
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, string> GetDLBehaviorConfigNameDict()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            try
            {
                DLBehaviorConfigItemNameDict.Clear();
                // 包含新竹與高雄
                K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["DLBehaviorConfig"];
                if (!string.IsNullOrEmpty(cd["DailyBehavior"]))
                {
                    string key = "日常行為表現";
                    //日常行為表現
                    XElement e1 = XElement.Parse(cd["DailyBehavior"]);
                    string name = e1.Attribute("Name").Value;
                    retVal.Add(key, name);

                    // 日常生活表現子項目
                    List<string> items = ParseItems(e1);
                    if (items.Count > 0)
                        DLBehaviorConfigItemNameDict.Add(key, items);

                }
                if (!string.IsNullOrEmpty(cd["OtherRecommend"]))
                {
                    //其它表現
                    XElement e2 = XElement.Parse(cd["OtherRecommend"]);
                    string name = e2.Attribute("Name").Value;
                    retVal.Add("其它表現", name);
                }
                if (!string.IsNullOrEmpty(cd["DailyLifeRecommend"]))
                {
                    //日常生活表現具體建議
                    XElement e3 = XElement.Parse(cd["DailyLifeRecommend"]);
                    string name = e3.Attribute("Name").Value;
                    retVal.Add("日常生活表現具體建議", name);  // 高雄
                    retVal.Add("綜合評語", name);  // 新竹
                }
                if (!string.IsNullOrEmpty(cd["GroupActivity"]))
                {
                    string key = "團體活動表現";
                    //團體活動表現
                    XElement e4 = XElement.Parse(cd["GroupActivity"]);
                    string name = e4.Attribute("Name").Value;
                    retVal.Add(key, name);

                    // 團體活動表現
                    List<string> items = ParseItems(e4);
                    if (items.Count > 0)
                        DLBehaviorConfigItemNameDict.Add(key, items);

                }
                if (!string.IsNullOrEmpty(cd["PublicService"]))
                {
                    string key = "公共服務表現";
                    //公共服務表現
                    XElement e5 = XElement.Parse(cd["PublicService"]);
                    string name = e5.Attribute("Name").Value;
                    retVal.Add(key, name);
                    List<string> items = ParseItems(e5);
                    if (items.Count > 0)
                        DLBehaviorConfigItemNameDict.Add(key, items);

                }
                if (!string.IsNullOrEmpty(cd["SchoolSpecial"]))
                {
                    string key = "校內外特殊表現";
                    //校內外特殊表現,新竹沒有子項目，高雄有子項目
                    XElement e6 = XElement.Parse(cd["SchoolSpecial"]);
                    string name = e6.Attribute("Name").Value;
                    retVal.Add(key, name);
                    List<string> items = ParseItems(e6);
                    if (items.Count > 0)
                        DLBehaviorConfigItemNameDict.Add(key, items);
                }
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("日常生活表現設定檔解析失敗!" + ex.Message);
            }

            return retVal;
        }
    }
}
