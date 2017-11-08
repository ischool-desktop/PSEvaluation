using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FISCA;
using FISCA.Deployment;
using FISCA.Presentation;

namespace JHEvaluation.ExamScoreCopy
{
    public static class Program
    {
        internal static ModuleMode Mode { get; private set; }

        [MainMethod]
        public static void Main()
        {
            DeployModeSetup();

            RibbonBarButton rbItem = K12.Presentation.NLDPanels.Class.RibbonBarItems["教務"]["評量成績複製"];
            rbItem.Image = Properties.Resources.rotate_save_64;
            rbItem.Enable = false;
            rbItem.Click += delegate
            {
                MainForm.Run();
            };

            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                rbItem.Enable = (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0);
            };
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
