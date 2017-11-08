using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using K12.Presentation;
using FISCA.Permission;
using KaoHsiung.ExportCourseScoreDiff;

namespace KaoHsiung.ExportCourseScoreDiffDiff
{
    public class Program
    {
        [FISCA.MainMethod()]
        public static void Main()
        {
            RibbonBarItem rbRptItem1 = MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"];

            rbRptItem1["成績作業"]["平時評量差異檢視"].Enable = UserAcl.Current["KaoHsiung.ExportCourseScoreDiffDiff"].Executable;
            rbRptItem1["成績作業"]["平時評量差異檢視"].Click += delegate
            {
                MainForm mf = new MainForm();
                mf.ShowDialog();
            };


            // 課程成績總表
            Catalog catalog = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            catalog.Add(new RibbonFeature("KaoHsiung.ExportCourseScoreDiffDiff", "平時評量差異檢視"));

        }
    }
}
