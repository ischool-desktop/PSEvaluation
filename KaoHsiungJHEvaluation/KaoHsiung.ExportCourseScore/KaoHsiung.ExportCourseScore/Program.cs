using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using FISCA.Permission;
using K12.Presentation;

namespace KaoHsiung.ExportCourseScore
{
    /// <summary>
    /// 高雄課程成績總表
    /// </summary>
    public class Program
    {
        [FISCA.MainMethod()]
        public static void Main()
        {
            RibbonBarItem rbRptItem1 = MotherForm.RibbonBarItems["課程", "資料統計"];            
            rbRptItem1["報表"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbRptItem1["報表"]["成績相關報表"]["課程成績總表"].Enable = UserAcl.Current["KaoHsiung.ExportCourseScore"].Executable;
            rbRptItem1["報表"]["成績相關報表"]["課程成績總表"].Click += delegate
            {
                if (NLDPanels.Course.SelectedSource.Count > 0)
                {
                    ExportScore es = new ExportScore(Utility.sortCourseIDList(NLDPanels.Course.SelectedSource));
                    es.Export();
                }
                else
                    FISCA.Presentation.Controls.MsgBox.Show("請選擇課程.");
            };


            // 課程成績總表
            Catalog catalog = RoleAclSource.Instance["課程"]["報表"];
            catalog.Add(new RibbonFeature("KaoHsiung.ExportCourseScore", "課程成績總表"));

        }
    }
}
