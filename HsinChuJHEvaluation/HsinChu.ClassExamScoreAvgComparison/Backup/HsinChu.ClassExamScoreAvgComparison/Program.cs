using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Security;
using FISCA.Presentation;
using FISCA;

namespace HsinChu.ClassExamScoreAvgComparison
{
    /// <summary>
    /// 班級評量成績平均比較表
    /// </summary>
    public static class Program
    {
        [MainMethod()]
        public static void Main()
        {
            string permission = "HsinChu.JHEvaluation.Class.Report0001";

            Catalog detail = RoleAclSource.Instance["班級"]["報表"];
            detail.Add(new ReportFeature(permission, "班級評量成績平均比較表"));

            MenuButton mb3 = FISCA.Presentation.MotherForm.RibbonBarItems["班級", "資料統計"]["報表"]["成績相關報表"]["班級評量成績平均比較表"];
            mb3.Enable = false;
            mb3.Click += delegate
            {
                MainForm.Run();
            };

            //要選班級才可以執行
            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                mb3.Enable = (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0) &&
                    Framework.User.Acl[permission].Executable;

            };
        }
    }
}
