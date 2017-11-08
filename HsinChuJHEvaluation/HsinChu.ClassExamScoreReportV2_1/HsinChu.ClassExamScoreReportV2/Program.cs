using System;
using System.Collections.Generic;
using System.Text;
using FISCA;
using FISCA.Presentation.Controls;
using K12.Presentation;
using Framework.Security;
using FISCA.Presentation;
using FISCA.Deployment;

namespace HsinChu.ClassExamScoreReportV21
{
    /// <summary>
    /// 班級評量成績單
    /// </summary>
    public static class Program
    {
        //權限代碼。
        private const string PermissionCode = "HsinChu.JHEvaluation.Class.Report0000v21";

        [MainMethod()]
        public static void Main()
        {
            //註冊權限管理項目。
            Catalog detail = RoleAclSource.Instance["班級"]["報表"];
            detail.Add(new ReportFeature(PermissionCode, "班級評量成績單(實中)"));

            //DeployParameters dparams = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");

            //註冊報表功能項目。
            MenuButton mb = NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["班級評量成績單(實中)"];
            mb.Enable = false;
            mb.Click += delegate
            {
                 MainForm.Run(NLDPanels.Class.SelectedSource);
            };

            //權限判斷。
            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                mb.Enable = (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0) &&
                    Framework.User.Acl[PermissionCode].Executable;
            };
        }
    }
}
