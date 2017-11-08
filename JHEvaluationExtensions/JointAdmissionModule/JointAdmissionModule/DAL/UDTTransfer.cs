using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace JointAdmissionModule.DAL
{
    class UDTTransfer
    {
        public enum _StudWeightConfigIdx { }

        /// <summary>
        /// 取得單筆學生UDT資料(年排名與百分比)
        /// </summary>
        /// <param name="StudentID"></param>
        /// <returns></returns>
        public static List<UserDefData> GetDataFromUDT(string StudentID)
        {
            AccessHelper accHelper = new AccessHelper();
            string query = "RefID='" + StudentID + "'";
            return accHelper.Select<UserDefData>(query);
        }

        /// <summary>
        /// 取得多筆學生UDT資料(年排名與百分比)
        /// </summary>
        /// <param name="StudentIDList"></param>
        /// <returns></returns>
        public static Dictionary<string,List<UserDefData>> GetDataFromUDTDict(List<string> StudentIDList)
        {
            Dictionary<string, List<UserDefData>> retVal = new Dictionary<string, List<UserDefData>>();
            AccessHelper accHelper = new AccessHelper();
            string query = "RefID in ('" + String.Join("','", StudentIDList.ToArray()) + "')";
            List<UserDefData> data= accHelper.Select<DAL.UserDefData>(query);

            foreach (UserDefData udd in data)
            {
                if (retVal.ContainsKey(udd.RefID))
                {
                    retVal[udd.RefID].Add(udd);
                }
                else
                {
                    List<UserDefData> uddL = new List<UserDefData>();
                    uddL.Add(udd);
                    retVal.Add(udd.RefID, uddL);
                }
            
            }
            return retVal;
        }

        /// <summary>
        /// 取得學生類別與加分比重設定資料
        /// </summary>
        /// <returns></returns>
        public static List<UserDefData_StudTypeWeight> GetDataFromUDT_StudTypeWeight()
        {
            AccessHelper accHelper = new AccessHelper();
            return accHelper.Select<UserDefData_StudTypeWeight>();
        }

        /// <summary>
        /// 新增資料到 UDT(年排名與百分比)
        /// </summary>
        /// <param name="?"></param>
        public static void InsertDataToUDT(List<UserDefData> data)
        {
            AccessHelper accHelper = new AccessHelper();            
            accHelper.InsertValues(data.ToArray());
        }

        /// <summary>
        /// 新增資料到 UDT_StudTypeWeight
        /// </summary>
        /// <param name="data"></param>
        public static void InsertDataToUDT_StudTypeWeight(List<UserDefData_StudTypeWeight> data)
        {
            AccessHelper accHelper = new AccessHelper();
            accHelper.InsertValues(data.ToArray());        
        }


        /// <summary>
        /// 刪除 UDT 內資料(年排名與百分比)
        /// </summary>
        /// <param name="data"></param>
        public static void DeleteDataToUDT(List<UserDefData> data)
        {
            AccessHelper accHelper = new AccessHelper();
            accHelper.DeletedValues(data.ToArray());            
        }

        /// <summary>
        /// 刪除 UDT_StudTypeWeight 內資料
        /// </summary>
        /// <param name="data"></param>
        public static void DeleteDataToUDT_StudTypeWeight(List<UserDefData_StudTypeWeight> data)
        {
            AccessHelper accHelper = new AccessHelper();
            accHelper.DeletedValues(data.ToArray());
        }
    }
}
