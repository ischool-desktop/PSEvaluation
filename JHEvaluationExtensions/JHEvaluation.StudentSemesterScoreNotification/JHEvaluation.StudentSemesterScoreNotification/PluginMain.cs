using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using Framework.Security;
using FISCA.Deployment;

namespace JHEvaluation.StudentSemesterScoreNotification
{
    public static class PluginMain
    {
        [MainMethod]
        public static void Main()
        {
            const string code = "KaoHsiung.JHEvaluation.Student.Report0005";
            const string code_class = "KaoHsiung.JHEvaluation.Student.Report0005_class";
            Catalog detail = RoleAclSource.Instance["學生"]["報表"];
            detail.Add(new ReportFeature(code, "學期成績通知單"));

            Catalog detail_class = RoleAclSource.Instance["班級"]["報表"];
            detail_class.Add(new ReportFeature(code_class, "學期成績通知單"));

            Global.Params = ModuleLoader.GetDeployParametsers(typeof(PluginMain), "Mode=KaoHsiung");

            MenuButton mb = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "資料統計"]["報表"]["成績相關報表"]["學期成績通知單"];            
            mb.Enable = false;
            mb.Click += delegate
            {
                // 指定來源
                MainForm._PrintSource = MainForm.PrintSource.學生;
                MainForm.Run();
            };

            //要選學生才可以執行
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && Framework.User.Acl[code].Executable;
                //mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0);
            };


            MenuButton mb_class = FISCA.Presentation.MotherForm.RibbonBarItems["班級", "資料統計"]["報表"]["成績相關報表"]["學期成績通知單"];
            mb_class.Enable = false;
            mb_class.Click += delegate
            {
                // 指定來源
                MainForm._PrintSource = MainForm.PrintSource.班級;
                MainForm.Run();
            };

            //要選班級才可以執行
            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                mb_class.Enable = (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0) && Framework.User.Acl[code_class].Executable;
                
            };





        }
    }
}
