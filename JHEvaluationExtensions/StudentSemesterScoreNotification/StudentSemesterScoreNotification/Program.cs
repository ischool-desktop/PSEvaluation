using FISCA;
using FISCA.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentSemesterScoreNotification
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            //學生
            FISCA.Presentation.RibbonBarItem item1 = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "資料統計"];
            item1["報表"]["成績相關報表"]["學期成績通知單(含補考成績)"].Enable = false;
            item1["報表"]["成績相關報表"]["學期成績通知單(含補考成績)"].Click += delegate
            {
                new MainForm(MainForm.PrintType.學生).ShowDialog();
            };

            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                item1["報表"]["成績相關報表"]["學期成績通知單(含補考成績)"].Enable = K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0 && Permissions.學生學期成績通知單權限;
            };

            //權限設定
            Catalog permission = RoleAclSource.Instance["學生"]["功能按鈕"];
            permission.Add(new RibbonFeature(Permissions.學生學期成績通知單, "學期成績通知單(含補考成績)"));

            //班級
            FISCA.Presentation.RibbonBarItem item2 = FISCA.Presentation.MotherForm.RibbonBarItems["班級", "資料統計"];
            item2["報表"]["成績相關報表"]["學期成績通知單(含補考成績)"].Enable = false;
            item2["報表"]["成績相關報表"]["學期成績通知單(含補考成績)"].Click += delegate
            {
                new MainForm(MainForm.PrintType.班級).ShowDialog();
            };

            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                item2["報表"]["成績相關報表"]["學期成績通知單(含補考成績)"].Enable = K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0 && Permissions.班級學期成績通知單權限;
            };

            //權限設定
            Catalog permission2 = RoleAclSource.Instance["班級"]["功能按鈕"];
            permission2.Add(new RibbonFeature(Permissions.班級學期成績通知單, "學期成績通知單(含補考成績)"));
        }
    }
}
