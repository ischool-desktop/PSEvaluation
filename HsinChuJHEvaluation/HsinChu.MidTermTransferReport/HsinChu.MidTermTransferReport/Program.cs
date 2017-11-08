using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using Framework.Security;

namespace HsinChu.MidTermTransferReport
{
    /// <summary>
    /// 期中轉學成績證明書
    /// </summary>
    public static class Program
    {
        [MainMethod]
        public static void Main()
        {
            const string code = "HsinChu.JHEvaluation.Student.Report0003";

            Catalog detail = RoleAclSource.Instance["學生"]["報表"];
            detail.Add(new ReportFeature(code, "期中轉學成績證明書"));

            MenuButton mb = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "資料統計"]["報表"]["成績相關報表"]["期中轉學成績證明書"];
            //MenuButton mb = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "資料統計"]["快點轉學啦"];
            mb.Enable = false;
            mb.Click += delegate
            {
                MainForm.Run();
            };

            //高雄客製化功能,要透過Code來呼叫此功能
            string URL期中轉學證明書 = "ischool/國中系統/學生/報表/成績/期中轉學證明書";
            FISCA.Features.Register(URL期中轉學證明書, arg =>
            {
                 MainForm.Run();

            });

            //要選學生才可以執行
            //JHSchool.Student.Instance.SelectedListChanged += delegate
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && Framework.User.Acl[code].Executable;
                //mb.Enable = (JHSchool.Student.Instance.SelectedKeys.Count > 0);
            };
        }
    }
}
