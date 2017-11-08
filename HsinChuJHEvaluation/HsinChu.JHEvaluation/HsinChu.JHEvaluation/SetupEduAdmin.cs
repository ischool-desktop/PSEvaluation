using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;

namespace HsinChu.JHEvaluation
{
    class SetupEduAdmin
    {
        internal static void Init()
        {
            #region RibbonBar

            #region 教務作業/成績作業
            RibbonBarButton rbItem = JHSchool.Affair.EduAdmin.Instance.RibbonBarItems["基本設定"]["設定"];
            //rbItem["評量設定"].Image = HsinChu.JHEvaluation.Properties.Resources.評量設定;
            rbItem["評分樣版設定"].Enable = Framework.User.Acl["JHSchool.EduAdmin.Ribbon0010"].Executable;
            rbItem["評分樣版設定"].Click += delegate
            {
                new HsinChu.JHEvaluation.CourseExtendControls.Ribbon.AssessmentSetupManager().ShowDialog();
            };

            RibbonBarButton rbItem1 = JHSchool.Affair.EduAdmin.Instance.RibbonBarItems["批次作業/檢視"]["成績作業"];
            //rbItem1["評量輸入狀況檢視"].Image = Properties.Resources.成績輸入檢查;
            rbItem1["評量輸入狀況檢視"].Enable = Framework.User.Acl["JHSchool.EduAdmin.Ribbon0020"].Executable;
            rbItem1["評量輸入狀況檢視"].Click += delegate
            {
                new HsinChu.JHEvaluation.EduAdminExtendControls.Ribbon.CourseScoreStatusForm().ShowDialog();
            };
            #endregion

            #endregion
        }
    }
}
