using FISCA;
using FISCA.Presentation;
using Framework.Security;
using Framework;

namespace JHEvaluation.AssignmentExamManager
{
    public static class Program
    {
        [MainMethod]
        public static void Main()
        {
            Global.Params = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");

            if (Global.Params["Mode"].ToUpper() == "KaoHsiung".ToUpper())
            {
                RibbonBarButton button = MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績作業"];
                //button.Size = RibbonBarButton.MenuButtonSize.Large;
                //button.Image = Properties.Resources.preview_info_64;
                button["小考輸入狀況檢視"].BeginGroup = true;
                button["小考輸入狀況檢視"].Enable = User.Acl["JHSchool.Course.Ribbon.AssignmentExamManager"].Executable;
                button["小考輸入狀況檢視"].Click += delegate
                {
                    MainForm.Run(); 
                };

                Catalog ribbon1 = RoleAclSource.Instance["教務作業"];
                ribbon1.Add(new RibbonFeature("JHSchool.Course.Ribbon.AssignmentExamManager", "小考輸入狀況檢視"));
            }
        }
    }
}
