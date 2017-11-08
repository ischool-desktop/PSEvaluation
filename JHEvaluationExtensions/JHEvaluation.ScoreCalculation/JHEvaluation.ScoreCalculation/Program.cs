using System;
using System.Collections.Generic;
using System.Text;
using FISCA;
using FISCA.Deployment;
using Framework.Security;
using FISCA.Presentation;
using K12.Presentation;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Framework;
using JHSchool;
using DataRationality;
using JHSchool.Evaluation.StudentExtendControls;

namespace JHEvaluation.ScoreCalculation
{
    public static class Program
    {
        internal static ModuleMode Mode { get; private set; }

        //權限代碼。
        private const string CalcStudentCode = "JHSchool.Student.Ribbon0057";
        private const string CalcAdminCode = "JHSchool.EduAdmin.Ribbon0045";
        private const string BatchSHistoryCode = "JHSchool.EduAdmin.Ribbon.BatchSemesterHistory"; //批次管理學期歷程。
        private const string GradFilteStudentCode = "JHSchool.Student.Ribbon0058"; //學生畢業資格審查。
        private const string GradFilteAdminCode = "JHSchool.EduAdmin.Ribbon0046"; //教務畢業資格審查。

        [MainMethod()]
        public static void Main()
        {
# if LocalDebug
            DeployParameters dparams = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");
            TestMode(dparams);
            return;
# else



            DeployModeSetup();

            //2017/5/9 穎驊 自JHSchool.Evaluation 搬過來
            //畢業成績
            Student.Instance.AddDetailBulider(new DetailBulider<GraduationScoreItem>());

            #region 教務作業
            //學期歷程。
            MenuButton btnSemsHistory = JHSchool.Affair.EduAdmin.Instance.RibbonBarItems["批次作業/檢視"]["成績作業"]["產生學期歷程"];
            btnSemsHistory.Enable = Framework.User.Acl[BatchSHistoryCode].Executable;
            btnSemsHistory.Click += delegate
            {
                new JHEvaluation.ScoreCalculation.SemesterHistory.BatchSemesterHistory().ShowDialog();
            };
            #endregion

            //註冊成績計算功能項目。
            MenuButton mb = NLDPanels.Student.RibbonBarItems["教務"]["成績作業"];
            mb.Enable = false;

            mb["計算科目成績"].Click += delegate { new SubjectScoreCalculate(NLDPanels.Student.SelectedSource).ShowDialog(); };

            mb["計算領域成績"].Click += delegate { new DomainScoreCalculate(NLDPanels.Student.SelectedSource).ShowDialog(); };

            mb["計算學習領域成績"].Click += delegate { new LearningDomainScoreCalculate(NLDPanels.Student.SelectedSource).ShowDialog(); };

            mb["加總學習領域文字描述"].Click += delegate { new DomainTextScoreSum(NLDPanels.Student.SelectedSource).ShowDialog(); };

            JHSchool.Affair.EduAdmin.Instance.RibbonBarItems["批次作業/檢視"]["成績作業"].Size = RibbonBarButton.MenuButtonSize.Large;
            MenuButton mbAdmin = JHSchool.Affair.EduAdmin.Instance.RibbonBarItems["批次作業/檢視"]["成績作業"];
            mbAdmin.Image = Properties.Resources.calc_save_64;
            mbAdmin["批次計算科目成績"].Click += delegate
            { new SubjectScoreCalculateByGradeyear().ShowDialog(); };

            mbAdmin["批次計算領域成績"].Click += delegate
            { new DomainScoreCalculateByGradeyear().ShowDialog(); };

            mbAdmin["批次計算學習領域成績"].Click += delegate
            { new LearningDomainScoreCalculateByGradeyear().ShowDialog(); };

            mbAdmin["批次加總學習領域文字描述"].Click += delegate
            { new DomainTextScoreSumByGradeyear().ShowDialog(); };

            /** 學生「畢業作業」。 **/
            RibbonBarButton rbItem = K12.Presentation.NLDPanels.Student.RibbonBarItems["教務"]["畢業作業"];
            //學生->計算畢業成績。
            rbItem["計算畢業成績"].Enable = User.Acl[CalcStudentCode].Executable;
            rbItem["計算畢業成績"].Click += delegate
            {
                new GraduateScoreCalculate(NLDPanels.Student.SelectedSource).ShowDialog();
            };

            //2017/5/9 穎驊 自JHSchool.Evaluation 搬過來
            #region 學生/資料統計/報表
            RibbonBarButton rbButton = Student.Instance.RibbonBarItems["資料統計"]["報表"];
            rbButton["成績相關報表"]["畢業預警報表"].Enable = User.Acl["JHSchool.Student.Report0010"].Executable;
            rbButton["成績相關報表"]["畢業預警報表"].Click += delegate
            {
                if (Student.Instance.SelectedList.Count <= 0) return;
                JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReport report = new JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReport(Student.Instance.SelectedList);
            };
            rbButton["學務相關報表"]["畢業預警報表"].Enable = User.Acl["JHSchool.Student.Report0010"].Executable;
            rbButton["學務相關報表"]["畢業預警報表"].Click += delegate
            {
                if (Student.Instance.SelectedList.Count <= 0) return;
                JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReport report = new JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReport(Student.Instance.SelectedList);
            };
            #endregion

            //註冊「畢業資格審查」。
            rbItem["畢業資格審查"].Enable = User.Acl[GradFilteStudentCode].Executable;
            //rbItem["畢業資格審查"].Image = JHSchool.Evaluation.CourseExtendControls.Ribbon.Resources.graduation_64;
            rbItem["畢業資格審查"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0) return;
                Form form = new JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationInspectWizard("Student");
                form.ShowDialog();
            };

            /** 教務「畢業作業」。 **/
            rbItem = JHSchool.Affair.EduAdmin.Instance.RibbonBarItems["批次作業/檢視"]["畢業作業"];
            JHSchool.Affair.EduAdmin.Instance.RibbonBarItems["批次作業/檢視"]["畢業作業"].Size = RibbonBarButton.MenuButtonSize.Large;
            //rbItem.Image = Properties.Resources.graduation_write_64;
            //rbItem.Size = RibbonBarButton.MenuButtonSize.Large;
            //教務->計算畢業成績
            rbItem["計算畢業成績"].Enable = User.Acl[CalcAdminCode].Executable;
            rbItem["計算畢業成績"].Visible = false; //先不要出來。
            rbItem["計算畢業成績"].Click += delegate
            {
            };

            //註冊「畢業資格審查」。
            rbItem["畢業資格審查"].Enable = User.Acl[GradFilteAdminCode].Executable;
            //rbItem["畢業資格審查"].Image = JHSchool.Evaluation.CourseExtendControls.Ribbon.Resources.graduation_64;
            rbItem["畢業資格審查"].Click += delegate
            {
                Form form = new JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationInspectWizard("EduAdmin");
                form.ShowDialog();
            };

            //註冊學期歷程的權限管理。
            Catalog catalog11 = RoleAclSource.Instance["教務作業"];
            catalog11.Add(new RibbonFeature(BatchSHistoryCode, "批次產生學期歷程"));

            //權限判斷。
            //權限註冊的部份還是留在成績系統內。
            mbAdmin.Enable = Framework.User.Acl[CalcAdminCode].Executable;

            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                //學生學期成績計算。
                mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) &&
                    User.Acl[CalcStudentCode].Executable;

                //學生計算畢業成績。
                K12.Presentation.NLDPanels.Student.RibbonBarItems["教務"]["畢業作業"]["計算畢業成績"].Enable =
                    (NLDPanels.Student.SelectedSource.Count > 0) &&
                    User.Acl[CalcStudentCode].Executable;

                //學生畢業資格審查。
                K12.Presentation.NLDPanels.Student.RibbonBarItems["教務"]["畢業作業"]["畢業資格審查"].Enable =
                    (NLDPanels.Student.SelectedSource.Count > 0) &&
                    User.Acl[GradFilteStudentCode].Executable;
            };
#endif

            //2017/5/9 穎驊 自JHSchool.Evaluation 搬過來
            //學生
            Catalog detail = RoleAclSource.Instance["學生"]["資料項目"];
            detail.Add(new DetailItemFeature(typeof(SemesterScoreItem)));
            detail.Add(new DetailItemFeature(typeof(GraduationScoreItem)));
            detail.Add(new DetailItemFeature(typeof(CourseScoreItem)));


            //2017/5/9 穎驊 自JHSchool.Evaluation 搬過來
            // 學生學期歷程與學期成績學年度學期檢查
            DataRationalityManager.Checks.Add(new JHSchool.Evaluation.StudentExtendControls.Ribbon.CheckStudentSemHistoryScoreRAT());
        }

        private static void DeployModeSetup()
        {
            DeployParameters dparams = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");

            //Mode = ModuleMode.KaoHsiung;
            //return;

            if (dparams["Mode"].ToUpper() == "KaoHsiung".ToUpper())
                Mode = ModuleMode.KaoHsiung; //高雄。
            else
                Mode = ModuleMode.HsinChu;  //新竹。
        }

        //private static void TestMode(DeployParameters dparams)
        //{
        //    FISCA.Authentication.DSAServices.SetLicense(@"C:\Users\yaoming\Desktop\ischool.dir\SmartSchoolLicense.key");
        //    FISCA.Authentication.DSAServices.Login("admin", "1234");

        //    new CalculationTest(dparams["Mode"]).ShowDialog();
        //}
    }

    internal enum ModuleMode
    {
        /// <summary>
        /// 新竹
        /// </summary>
        HsinChu,
        /// <summary>
        /// 高雄
        /// </summary>
        KaoHsiung
    }
}
