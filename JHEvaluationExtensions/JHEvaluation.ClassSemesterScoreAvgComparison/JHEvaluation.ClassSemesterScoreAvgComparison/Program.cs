using System;
using System.Collections.Generic;
using System.Windows.Forms;
using JHSchool;
using Framework;
using Framework.Security;



namespace JHEvaluation.ClassSemesterScoreAvgComparison
{
    public static class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            Catalog detail = RoleAclSource.Instance["班級"]["報表"];
            detail.Add(new ReportFeature("KaoHsiung.JHEvaluation.Class.ClassSemesterScoreAvgComparison", "班級學期成績比較表"));

            FISCA.Presentation.MenuButton mb = FISCA.Presentation.MotherForm.RibbonBarItems["班級", "資料統計"]["報表"]["成績相關報表"]["班級學期成績平均比較表"];
            mb.Enable = false;
            mb.Click += delegate
            {
                ClassSemesterScoreAvgComparison.ClassSemsScoreAvgCmpForm cssac = new ClassSemsScoreAvgCmpForm();
                cssac.ShowDialog();
            };


            //要選班級才可以執行
            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                mb.Enable = (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0) && Framework.User.Acl["KaoHsiung.JHEvaluation.Class.ClassSemesterScoreAvgComparison"].Executable;
            };
        
        }
    }
}
