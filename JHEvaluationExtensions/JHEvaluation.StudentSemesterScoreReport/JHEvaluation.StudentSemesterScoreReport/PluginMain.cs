using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using Framework.Security;

namespace JHEvaluation.StudentSemesterScoreReport
{
    public static class PluginMain
    {
        [MainMethod]
        public static void Main()
        {
            const string code = "KaoHsiung.JHEvaluation.Student.Report0004";
            const string code_class = "KaoHsiung.JHEvaluation.Student.Report0004_class";
            Catalog detail = RoleAclSource.Instance["學生"]["報表"];
            detail.Add(new ReportFeature(code, "學期成績證明單"));

            Catalog detail_Class = RoleAclSource.Instance["班級"]["報表"];
            detail_Class.Add(new ReportFeature(code_class, "學期成績證明單"));        

          //  Global.Params = ModuleLoader.GetDeployParametsers(typeof(PluginMain), "Mode=HsinChu");
            Global.Params = ModuleLoader.GetDeployParametsers(typeof(PluginMain), "Mode=KaoHsiung");
            MenuButton mb = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "資料統計"]["報表"]["成績相關報表"]["學期成績證明單"];            
            mb.Enable = false;
            mb.Click += delegate
            {
                MainForm._PrintSource = MainForm.PrintSource.學生;
                MainForm.Run();
            };

            //要選學生才可以執行
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && Framework.User.Acl[code].Executable;
                //mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0);
            };

            MenuButton mb_Class = FISCA.Presentation.MotherForm.RibbonBarItems["班級", "資料統計"]["報表"]["成績相關報表"]["學期成績證明單"];
            mb_Class.Enable = false;
            mb_Class.Click += delegate
            {
                MainForm._PrintSource = MainForm.PrintSource.班級;
                MainForm.Run();
            };

            //要選班級才可以執行
            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                mb_Class.Enable = (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0) && Framework.User.Acl[code_class].Executable;
                
            };


        }
    }
}
