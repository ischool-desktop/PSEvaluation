using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Framework;
using FISCA;
using FISCA.Presentation;
using Framework.Security;

namespace KaoHsiung.StudentRecordReport
{
    /// <summary>
    /// 學籍表
    /// </summary>
    public static class Program
    {
        [MainMethod()]
        public static void Main()
        {
            string code = "KaoHsiung.JHEvaluation.Student.Report0000";
            Catalog detail = RoleAclSource.Instance["學生"]["報表"];
            detail.Add(new ReportFeature(code, "學籍表"));

            MenuButton mb = MotherForm.RibbonBarItems["學生", "資料統計"]["報表"]["成績相關報表"]["學籍表"];
            mb.Enable = false;
            mb.Click += delegate
            {
                MainForm form = new MainForm(EnterType.Student);
                form.ShowDialog();
            };


            string URL學籍表 = "ischool/國中系統/學生/報表/成績/學籍表";
            FISCA.Features.Register(URL學籍表, arg =>
            {
                 MainForm form = new MainForm(EnterType.Student);
                 form.ShowDialog();
            });

            MenuButton mb2 = FISCA.Presentation.MotherForm.RibbonBarItems["班級", "資料統計"]["報表"]["成績相關報表"]["學籍表"];
            mb2.Enable = false;
            mb2.Click += delegate
            {
                MainForm.Run(EnterType.Class);
            };

            //要選學生才可以執行
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) && User.Acl[code].Executable;
            };

            //要選班級才可以執行
            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                mb2.Enable = (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0) && Framework.User.Acl[code].Executable;
                //mb.Enable = (JHSchool.Student.Instance.SelectedKeys.Count > 0);
            };

        }
    }
}
