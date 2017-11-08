using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FISCA;
using FISCA.Deployment;
using JHSchool.Data;
using FISCA.Presentation;
using K12.Presentation;
using Framework;
using JHSchool.Evaluation;

namespace JHEvaluation.Rating
{
    public static class Program
    {
        internal static ModuleMode Mode { get; private set; }

        //權限代碼。
        private const string StudentRatingCode = "JHSchool.Student.Ribbon0059";

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        [MainMethod()]
        public static void Main()
        {
            DeployModeSetup();

# if LocalDebug
            TestProject();
            return;
# else
            NLDPanels.Student.RibbonBarItems["教務"]["排名作業"].Size = RibbonBarButton.MenuButtonSize.Large;
            NLDPanels.Student.RibbonBarItems["教務"]["排名作業"].Image = Properties.Resources.refresh_window_64;

            RibbonBarButton rbButton = NLDPanels.Student.RibbonBarItems["教務"]["排名作業"];
            //rbButton.Size = RibbonBarButton.MenuButtonSize.Large;
            rbButton.Image = Properties.Resources.refresh_window_64;
            rbButton.Enable = false;
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                rbButton.Enable = User.Acl[StudentRatingCode].Executable &&
                    NLDPanels.Student.SelectedSource.Count > 0;
            };

            rbButton["評量成績排名"].Enable = true;
            rbButton["評量成績排名"].Click += delegate
            {
                FormRating form = new FormExamRating();
                form.SetRatingStudents(NLDPanels.Student.SelectedSource);
                form.ShowDialog();
            };


            rbButton["學期成績排名"].Enable = true;
            rbButton["學期成績排名"].Click += delegate
            {
                FormRating form = new FormSemesterRating();
                form.SetRatingStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
                form.ShowDialog();
            };

            rbButton["學期科目成績排名(多學期)"].Enable = true;
            rbButton["學期科目成績排名(多學期)"].Click += delegate
            {
                FormRating form = new FormSemestersRating();
                form.SetRatingStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
                form.ShowDialog();
            };
#endif
        }

        private static void DeployModeSetup()
        {
            DeployParameters dparams = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");

            if (dparams["Mode"].ToUpper() == "KaoHsiung".ToUpper())
                Mode = ModuleMode.KaoHsiung; //高雄。
            else
                Mode = ModuleMode.HsinChu;  //新竹。
        }

        private static void TestProject()
        {
            //FISCA.Authentication.DSAServices.SetLicense(@"C:\Users\yaoming\Desktop\ischool.dir\SmartSchoolLicense.key");
            //FISCA.Authentication.DSAServices.Login("admin", "1234");

            //JHClassRecord cls = JHClass.SelectByID("24");//正興 301 班。

            ////FormRating fr = new FormExamDomain();
            //FormRating fr = new FormExamRating();
            ////fr.SetRatingStudents(cls.Students.ToKeys());
            //int t1 = Environment.TickCount;
            //fr.SetRatingStudents(JHStudent.SelectAll().ToKeys());
            //Console.WriteLine(string.Format("SetRatingStudents Time：{0}", Environment.TickCount - t1));

            //fr.ShowDialog();
        }
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
