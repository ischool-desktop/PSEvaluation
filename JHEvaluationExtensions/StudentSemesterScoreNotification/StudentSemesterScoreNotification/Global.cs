using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentSemesterScoreNotification
{
    class Global
    {
        public const string TemplateConfigName = "學期成績通知單(含補考成績)_列印設定";

        public const int SupportSubjectCount = 30, SupportDomainCount = 20, SupportAbsentCount = 20, SupportClubCount = 5;

        /// <summary>
        /// 列印用領域名稱
        /// </summary>
        /// <returns></returns>
        public static List<string> PriDomainNameList()
        {
            List<string> value = new List<string>();
            value.Add("語文");
            value.Add("國語文");
            value.Add("英語");
            value.Add("數學");
            value.Add("社會");
            value.Add("自然與生活科技");
            value.Add("藝術與人文");
            value.Add("健康與體育");
            value.Add("綜合活動");
            value.Add("彈性課程");            
            return value;
        }

        public static Dictionary<string, string> DLBehaviorRef = new Dictionary<string, string>() 
        { 
            {"日常行為表現","DailyBehavior/Item"},
            {"團體活動表現","GroupActivity/Item"},
            {"公共服務表現","PublicService/Item"},
            {"校內外特殊表現","SchoolSpecial/Item"},
            {"具體建議","DailyLifeRecommend"},
            {"其他表現","OtherRecommend"},
            {"綜合評語","DailyLifeRecommend"}
        };

        public static string GetKey(params string[] list)
        {
            return string.Join("_", list);
        }
    }
}
