using System;
using System.Collections.Generic;
using System.Text;
using JHSchool;
using JHSchool.Evaluation;
using FISCA.Presentation;
using FISCA.Presentation.Controls;

namespace KaoHsiung.JHEvaluation
{
    class SetupEduAdmin
    {
        internal static void Init()
        {

            #region 教務作業/成績作業
            RibbonBarButton rbItem = JHSchool.Affair.EduAdmin.Instance.RibbonBarItems["基本設定"]["設定"];
            //rbItem["評分樣版管理"].Image = KaoHsiung.JHEvaluation.Properties.Resources.notepad_level_64;
            //rbItem["評分樣版管理"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["評分樣版設定"].Enable = Framework.User.Acl["JHSchool.EduAdmin.Ribbon0010"].Executable;
            rbItem["評分樣版設定"].Click += delegate
            {
                new KaoHsiung.JHEvaluation.EduAdminExtendControls.Ribbon.AssessmentSetupManager().ShowDialog();
            };

            //移到其他模組。
            //rbItem["計算成績"].Enable = Framework.User.Acl["JHSchool.EduAdmin.Ribbon0045"].Executable;
            //rbItem["計算成績"].Image = KaoHsiung.JHEvaluation.Properties.Resources.calcScore;
            //rbItem["計算成績"].Click += delegate
            //{
            //    System.Windows.Forms.Form form = new KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizardStudent("EduAdmin");
            //    form.ShowDialog();
            //};

            RibbonBarButton rbItem1 = JHSchool.Affair.EduAdmin.Instance.RibbonBarItems["批次作業/檢視"]["成績作業"];
            //rbItem1["評量輸入狀況"].Image = Properties.Resources.成績輸入檢查;
            rbItem1["評量輸入狀況檢視"].Enable = Framework.User.Acl["JHSchool.EduAdmin.Ribbon0020"].Executable;
            rbItem1["評量輸入狀況檢視"].Click += delegate
            {
                new KaoHsiung.JHEvaluation.EduAdminExtendControls.Ribbon.CourseScoreStatusForm().ShowDialog();
            };
            #endregion
        }
    }
}
