using FISCA;
using FISCA.Deployment;
using FISCA.Permission;
using FISCA.Presentation;
using K12.Presentation;

namespace JointAdmissionModule
{
    public class Program
    {
        internal static ModuleMode Mode { get; private set; }

        //權限代碼。
        private const string PermissionCode = "JHSchool.Student.JointAdmissionModule";
        private const string PermissionCodeRpt = "JHSchool.Student.JointAdmissionModuleRpt";
        private const string PermissionCodeRpt5 = "JHSchool.Student.JointAdmissionModuleRpt5";
        private const string PermissionCodeRpt_HsinChu = "JHSchool.Student.JointAdmissionModuleRptHsinChu";
        [FISCA.MainMethod("JointAdmissionModule")]
        public static void Main()
        {
            DeployModeSetup();

            if (FISCA.Permission.UserAcl.Current[PermissionCode].Editable || FISCA.Permission.UserAcl.Current[PermissionCode].Viewable)
                K12.Presentation.NLDPanels.Student.AddDetailBulider(new FISCA.Presentation.DetailBulider<UCSocreRankItem>());

            Catalog detail = RoleAclSource.Instance["學生"]["資料項目"];
            detail.Add(new DetailItemFeature(PermissionCode, "學期成績排名與排名百分比"));           
            
            //K12.Presentation.NLDPanels.Student.ListPaneContexMenu["學期成績排名與排名百分比(固定)"].Enable = UserAcl.Current["JHSchool.Student.JointAdmissionModule"].Executable;
            K12.Presentation.NLDPanels.Student.ListPaneContexMenu["學期成績排名與排名百分比"].Enable = FISCA.Permission.UserAcl.Current[PermissionCode].Editable;
            K12.Presentation.NLDPanels.Student.ListPaneContexMenu["學期成績排名與排名百分比"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0)
                {
                    StudSemScoreRankPInput sssrpi = new StudSemScoreRankPInput(K12.Presentation.NLDPanels.Student.SelectedSource[0]);
                    sssrpi.ShowDialog();
                }
            };

            //RibbonBarItem rbItem = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "成績作業"];
            //RibbonBarButton SetButton = rbItem["設定學生身份加分比(多元入學)"];
            //SetButton.Enable = User.Acl[PermissionCode].Editable;
            //SetButton.Click += delegate
            //{
            //    SetStudAddWeight ssaw = new SetStudAddWeight();
            //    ssaw.Show();
            //};


            //註冊權限管理項目。
            Catalog detail1 = RoleAclSource.Instance["學生"]["報表"];
            if(Mode== ModuleMode.KaoHsiung)
                detail1.Add(new ReportFeature(PermissionCodeRpt, "高雄區高中高職免試入學成績證明"));

            detail1.Add(new ReportFeature(PermissionCodeRpt5, "北中南區五專免試入學成績證明"));

            //if (Mode == ModuleMode.HsinChu)
            detail1.Add(new ReportFeature(PermissionCodeRpt_HsinChu, "高中高職免試入學成績證明"));

            
            //註冊報表功能項目。
            if (Mode == ModuleMode.KaoHsiung)
            {
                MenuButton mb = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["高中高職免試入學相關報表"]["高雄區高中高職免試入學成績證明"];
                mb.Enable = false;
                mb.Click += (sender,e)=> new StudentScoreSummaryReport.PrintForm(K12.Presentation.NLDPanels.Student.SelectedSource).ShowDialog();
                K12.Presentation.NLDPanels.Student.SelectedSourceChanged += (sender,e) => mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && FISCA.Permission.UserAcl.Current[PermissionCodeRpt].Executable;
            }

            //註冊報表功能項目。
            //2013/6/13 - Dylan增加判斷權限
            MenuButton mb1 = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["五專免試入學相關報表"]["北中南區五專免試入學成績證明"];
            mb1.Enable = false;
            mb1.Click += (sender,e) => new StudentScoreSummaryReport5.PrintForm(K12.Presentation.NLDPanels.Student.SelectedSource).ShowDialog();;
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += (sender, e) => mb1.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && FISCA.Permission.UserAcl.Current[PermissionCodeRpt5].Executable;

            //註冊報表功能項目。
            MenuButton mb2 = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["高中高職免試入學相關報表"]["竹苗區高中高職免試入學成績證明"];
            mb2.Enable = false;
            mb2.Click += (sender,e) => new StudentScoreSummaryReport_HsinChu.PrintForm(K12.Presentation.NLDPanels.Student.SelectedSource, ModuleMode.HsinChu).ShowDialog();
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += (sender,e) => mb2.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && FISCA.Permission.UserAcl.Current[PermissionCodeRpt_HsinChu].Executable;

            //註冊報表功能項目。
            MenuButton mb3 = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["高中高職免試入學相關報表"]["台南區高中高職免試入學成績證明"];
            mb3.Enable = false;
            mb3.Click += (sender, e) => new StudentScoreSummaryReport_HsinChu.PrintForm(K12.Presentation.NLDPanels.Student.SelectedSource, ModuleMode.Tainan).ShowDialog();
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += (sender, e) => mb3.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && FISCA.Permission.UserAcl.Current[PermissionCodeRpt_HsinChu].Executable;

            //註冊報表功能項目。
            MenuButton mb4 = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["高中高職免試入學相關報表"]["中投區高中高職免試入學成績證明"];
            mb4.Enable = false;
            mb4.Click += (sender, e) => new StudentScoreSummaryReport_HsinChu.PrintForm(K12.Presentation.NLDPanels.Student.SelectedSource, ModuleMode.Taichung).ShowDialog();
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += (sender, e) => mb4.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && FISCA.Permission.UserAcl.Current[PermissionCodeRpt_HsinChu].Executable;
        }

        private static void DeployModeSetup()
        {
            DeployParameters dparams = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");

            if (dparams["Mode"].ToUpper() == "KaoHsiung".ToUpper())
                Mode = ModuleMode.KaoHsiung; //高雄。
            else
                Mode = ModuleMode.HsinChu;  //新竹。
        }
    }

    public enum ModuleMode
    {
        /// <summary>
        /// 新竹
        /// </summary>
        HsinChu,
        /// <summary>
        /// 高雄
        /// </summary>
        KaoHsiung,
        /// <summary>
        /// 台南
        /// </summary>
        Tainan,
        /// <summary>
        /// 台中
        /// </summary>
        Taichung,
        /// <summary>
        /// 其他
        /// </summary>
        Other
    }
}