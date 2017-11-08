using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace JointAdmissionModule.DAL
{
    class DALTransfer
    {
        /// <summary>
        /// 存放年排名使用年級,年排名,成績
        /// </summary>
        public static Dictionary<string, Dictionary<string, List<StudRankScore>>> StudRankScoreDict = new Dictionary<string, Dictionary<string, List<StudRankScore>>>();        

        public enum SchoolType {高中,五專}

        /// <summary>
        /// 報表使用狀態
        /// </summary>
        public static SchoolType _SchoolType = SchoolType.高中;
        /// <summary>
        /// 取得學生類別設定完整名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> GetStudTypeConfigFullNameList()
        {
            List<string> retVal = new List<string>();
            // 取得學生設定標籤內的類別名稱            
            List<TagConfigRecord> studTagConf = TagConfig.SelectByCategory(TagCategory.Student);
            retVal = (from tag in studTagConf orderby tag.FullName ascending select tag.FullName ).ToList();
            return retVal;
        }

        /// <summary>
        /// 取得特種身分名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> GetJoinStudSpecTypeName()
        {
            List<string> retVal = new List<string>();
            retVal.Add("原住民(持文化及語言能力證明)");
            retVal.Add("原住民(未持文化及語言能力證明)");
            retVal.Add("境外優秀科學技術人才子女");
            retVal.Add("政府派赴國外工作人員子女");
            retVal.Add("蒙藏生");
            retVal.Add("身心障礙生");
            retVal.Add("境外科技人才子女未滿一學期");
            retVal.Add("境外科技人才子女滿一學期未滿一學年");
            retVal.Add("境外科技人才子女滿一學年未滿二學年");
            retVal.Add("境外科技人才子女滿二學年未滿三學年");
            retVal.Add("派外人員子女未滿一學期");
            retVal.Add("派外人員子女滿一學期未滿一學年");
            retVal.Add("派外人員子女滿一學年未滿二學年");
            retVal.Add("派外人員子女滿二學年未滿三學年");
            return retVal;
        }



        /// <summary>
        /// 透過學生類別取得 UDT 內設定不排名學生.ID
        /// </summary>
        /// <returns></returns>
        public static List<string> GetNonRankStudentIDFromUDTByStudentTag(List<StudentTagRecord> StudTagRecList,SchoolType ST)
        {
            List<string> TagNameList = new List<string>();
            List<string> retVal = new List<string>();
            TagNameList = (from data in UDTTransfer.GetDataFromUDT_StudTypeWeight() where data.CheckNonRank==true && data.SchoolType== ST.ToString () select data.StudentType).ToList();

            foreach (StudentTagRecord StudTagRec in StudTagRecList)
            { 
                if(TagNameList.Contains(StudTagRec.FullName ))
                    retVal.Add(StudTagRec.RefStudentID );
            }            
            return retVal;
        }

        /// <summary>
        /// 透過學生類別取得 UDT 內設定加分比重 學生.ID,比重
        /// </summary>
        /// <param name="StudTagRecList"></param>
        /// <param name="ST"></param>
        /// <returns></returns>
        public static Dictionary<string, decimal> GetStudentAddWeightFormUDTByStudentTag(List<StudentTagRecord> StudTagRecList, SchoolType ST)
        {
            Dictionary<string, decimal> retVal = new Dictionary<string, decimal>();
            Dictionary<string, decimal> weight = new Dictionary<string, decimal>();

            foreach (DAL.UserDefData_StudTypeWeight udd in UDTTransfer.GetDataFromUDT_StudTypeWeight().Where(x=>x.SchoolType == ST.ToString ()))
            {
                if(udd.AddWeight < 0)
                    continue ;

                if(!weight.ContainsKey (udd.StudentType))
                    weight.Add(udd.StudentType,udd.AddWeight);
            }

            foreach (StudentTagRecord StudTagRec in StudTagRecList )
            {
                if(weight.ContainsKey(StudTagRec.FullName ))
                {
                    if (retVal.ContainsKey(StudTagRec.RefStudentID))
                    {
                        // 當有2個以上，取其高
                        if(retVal[StudTagRec.RefStudentID]<weight[StudTagRec.FullName])
                            retVal[StudTagRec.RefStudentID]=weight[StudTagRec.FullName];
                    }
                    else
                        retVal.Add(StudTagRec.RefStudentID, weight[StudTagRec.FullName]);
                }            
            }
            return retVal;        
        }


        /// <summary>
        /// 透過學生類別取得 UDT 內設定特種身分 學生.ID,特種身分
        /// </summary>
        /// <param name="StudTagRecList"></param>
        /// <param name="ST"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetStudentSpcTypeFormUDTByStudentTag(List<StudentTagRecord> StudTagRecList, SchoolType ST)
        {
            Dictionary<string, DAL.UserDefData_StudTypeWeight> retVal = new Dictionary<string, DAL.UserDefData_StudTypeWeight>();
            Dictionary<string, DAL.UserDefData_StudTypeWeight> weight = new Dictionary<string, DAL.UserDefData_StudTypeWeight>();
            Dictionary<string, string> retVal1 = new Dictionary<string, string>();

            foreach (DAL.UserDefData_StudTypeWeight udd in UDTTransfer.GetDataFromUDT_StudTypeWeight().Where(x => x.SchoolType == ST.ToString()))
            {
                if (udd.AddWeight < 0)
                    continue;

                if (!weight.ContainsKey(udd.StudentType))
                    weight.Add(udd.StudentType,udd);
            }

            foreach (StudentTagRecord StudTagRec in StudTagRecList)
            {
                if (weight.ContainsKey(StudTagRec.FullName))
                {
                    if (retVal.ContainsKey(StudTagRec.RefStudentID))
                    {
                        // 當有2個以上，取其高
                        if (retVal[StudTagRec.RefStudentID].AddWeight < weight[StudTagRec.FullName].AddWeight)
                            retVal[StudTagRec.RefStudentID] = weight[StudTagRec.FullName];
                    }
                    else
                        retVal.Add(StudTagRec.RefStudentID, weight[StudTagRec.FullName]);
                }
            }

            foreach (KeyValuePair<string, DAL.UserDefData_StudTypeWeight> dd in retVal)
            {
                // 回傳特種身分
                retVal1.Add(dd.Key, dd.Value.JoinStudType);
            }

            return retVal1;        
        }


        /// <summary>
        /// 透過學生ID取得最後一筆轉入異動
        /// </summary>
        /// <param name="StudentIDList"></param>
        /// <returns></returns>
        public static Dictionary<string, UpdateRecordRecord> GetStudentUpdareDate3ByStudentID(List<string> StudentIDList)
        {
            Dictionary<string, UpdateRecordRecord> retVal = new Dictionary<string, UpdateRecordRecord>();
            List<UpdateRecordRecord> UpdateRecList = (from data in UpdateRecord.SelectByStudentIDs(StudentIDList) where data.UpdateCode == "3" orderby DateTime.Parse(data.UpdateDate) descending select data).ToList();
            foreach (UpdateRecordRecord ur in UpdateRecList)
            {
                if (!retVal.ContainsKey(ur.StudentID))
                {
                    retVal.Add(ur.StudentID, ur);
                }
            }
            return retVal;
        }

        /// <summary>
        /// 五專領域名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDomainNameList()
        {
            List<string> DomainNameList = new List<string>();
            DomainNameList.Add("國語文");
            DomainNameList.Add("英語");
            DomainNameList.Add("數學");
            DomainNameList.Add("社會");
            DomainNameList.Add("自然與生活科技");
            DomainNameList.Add("藝術與人文");
            DomainNameList.Add("健康與體育");
            DomainNameList.Add("綜合活動");

            return DomainNameList;
        }

        /// <summary>
        /// 五專領域名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCoreDomainNameList()
        {
            List<string> DomainNameList = new List<string>();
            DomainNameList.Add("國語文");
            DomainNameList.Add("英語");
            DomainNameList.Add("數學");
            DomainNameList.Add("社會");
            DomainNameList.Add("自然與生活科技");

            return DomainNameList;
        }
    }
}
