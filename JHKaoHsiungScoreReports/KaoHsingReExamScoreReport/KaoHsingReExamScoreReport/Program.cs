using FISCA.Permission;
using FISCA.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaoHsingReExamScoreReport
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            RibbonBarItem rbItem2 = MotherForm.RibbonBarItems["班級", "資料統計"];
            rbItem2["報表"]["成績相關報表"]["領域補考名單(給導師)"].Enable = UserAcl.Current["KaoHsingReExamScoreReport.ReDomainForTeacherForm"].Executable;
            rbItem2["報表"]["成績相關報表"]["領域補考名單(給導師)"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0)
                {
                    Forms.ReDomainForTeacherForm rdft = new Forms.ReDomainForTeacherForm(K12.Presentation.NLDPanels.Class.SelectedSource);
                    rdft.ShowDialog();
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選擇選班級");
                    return;
                }

            };

            // 領域補考名單(給導師)
            Catalog catalog1b = RoleAclSource.Instance["班級"]["功能按鈕"];
            catalog1b.Add(new RibbonFeature("KaoHsingReExamScoreReport.ReDomainForTeacherForm", "領域補考名單(給導師)"));

            RibbonBarItem rbItem2b = MotherForm.RibbonBarItems["班級", "資料統計"];
            rbItem2b["報表"]["成績相關報表"]["領域補考名單(給試務)"].Enable = UserAcl.Current["KaoHsingReExamScoreReport.ReDomainForUserForm"].Executable;
            rbItem2b["報表"]["成績相關報表"]["領域補考名單(給試務)"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0)
                {

                    Forms.ReDomainForUserForm rduf = new Forms.ReDomainForUserForm(K12.Presentation.NLDPanels.Class.SelectedSource);
                    rduf.ShowDialog();
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選擇選班級");
                    return;
                }

            };

            // 領域補考名單(給試務)
            Catalog catalog2b = RoleAclSource.Instance["班級"]["功能按鈕"];
            catalog2b.Add(new RibbonFeature("KaoHsingReExamScoreReport.ReDomainForUserForm", "領域補考名單(給試務)"));


            RibbonBarItem rbItem2c = MotherForm.RibbonBarItems["學生", "資料統計"];
            rbItem2c["報表"]["成績相關報表"]["領域補考通知單"].Enable = UserAcl.Current["KaoHsingReExamScoreReport.ReDomainForStudentForm"].Executable;
            rbItem2c["報表"]["成績相關報表"]["領域補考通知單"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0)
                {

                    Forms.ReDomainForStudentForm rdfsf = new Forms.ReDomainForStudentForm(K12.Presentation.NLDPanels.Student.SelectedSource);
                    rdfsf.ShowDialog();
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選擇選學生");
                    return;
                }

            };

            // 領域補考名單
            Catalog catalog2c = RoleAclSource.Instance["學生"]["功能按鈕"];
            catalog2c.Add(new RibbonFeature("KaoHsingReExamScoreReport.ReDomainForStudentForm", "領域補考通知單"));

        }
    }
}
