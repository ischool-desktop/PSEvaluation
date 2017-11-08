using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using Framework.Security;

namespace JHEvaluation.WuZhuanRegisterScoreImport
{
    public static class Program
    {
        [MainMethod()]
        public static void Main()
        {
            //string code = "KaoHsiung.JHEvaluation.Student.Report0006";
            //Catalog detail = RoleAclSource.Instance["學生"]["報表"];
            //detail.Add(new ReportFeature(code, "五專集體報名成績匯入檔"));

            //Global.Params = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");
            //if (Global.Params["Mode"].ToUpper() == "HsinChu".ToUpper()) return;

            //MenuButton mb = K12.Presentation.NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["五專集體報名成績匯入檔"];
            //mb.Enable = false;
            //mb.Click += delegate
            //{
            //    Export.Run();
            //};

            ////權限判斷。
            //K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            //{
            //    mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) &&
            //        Framework.User.Acl[code].Executable;
            //};
        }
    }
}
