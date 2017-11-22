using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentSemesterScoreNotification
{
    class Permissions
    {
        public static bool 學生學期成績通知單權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[學生學期成績通知單].Executable;
            }
        }

        public static bool 班級學期成績通知單權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[班級學期成績通知單].Executable;
            }
        }

        public static string 學生學期成績通知單 = "StudentSemesterScoreNotification-{A98BD8B1-D40A-46D6-B34F-2A7AF473BD00}";
        public static string 班級學期成績通知單 = "StudentSemesterScoreNotification-{21CD2D46-D72B-49CB-A6CF-1AED8BE49777}";
    }
}
