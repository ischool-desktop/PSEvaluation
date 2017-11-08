using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;

using FISCA.Permission;
using FISCA.Presentation;
using K12.Presentation;
namespace KH_StudentScoreSummaryReport
{
    public class Program
    {
        [FISCA.MainMethod()]
        public static void Main()
        {           
            //註冊權限管理項目。
            Catalog detail1 = RoleAclSource.Instance["學生"]["報表"];

            detail1.Add(new ReportFeature("KH_StudentScoreSummaryReport", "高雄市免試入學在校成績證明書"));


            MenuButton mb = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["高雄市免試入學在校成績證明書"];
            mb.Enable = false;
            mb.Click += (sender, e) => new PrintForm(K12.Presentation.NLDPanels.Student.SelectedSource).ShowDialog();
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += (sender, e) => mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && FISCA.Permission.UserAcl.Current["KH_StudentScoreSummaryReport"].Executable;

        }
    }
}
