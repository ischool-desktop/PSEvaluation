using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using Framework.Security;

namespace HsinChu.StudentExamScoreReport
{
    /// <summary>
    /// 個人評量成績單
    /// </summary>
    public static class Program
    {
        [MainMethod]
        public static void Main()
        {
            Catalog detail = RoleAclSource.Instance["學生"]["報表"];
            detail.Add(new ReportFeature("HsinChu.JHEvaluation.Student.Report0002", "個人評量成績單"));

            MenuButton mb = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "資料統計"]["報表"]["成績相關報表"]["個人評量成績單"];
            //MenuButton mb = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "資料統計"]["快點啦"];
            mb.Enable = false;
            mb.Click += delegate
            {
                MainForm.Run(EnterType.Student);
            };

            MenuButton mb2 = FISCA.Presentation.MotherForm.RibbonBarItems["班級", "資料統計"]["報表"]["成績相關報表"]["個人評量成績單"];
            mb2.Enable = false;
            mb2.Click += delegate
            {
                MainForm.Run(EnterType.Class);
            };

            //要選學生才可以執行
            //JHSchool.Student.Instance.SelectedListChanged += delegate
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && Framework.User.Acl["HsinChu.JHEvaluation.Student.Report0002"].Executable;
            };

            //要選班級才可以執行
            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                mb2.Enable = (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0) && Framework.User.Acl["HsinChu.JHEvaluation.Student.Report0002"].Executable;

                //mb.Enable = (JHSchool.Student.Instance.SelectedKeys.Count > 0);
            };
        }
    }
}
