using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using JHSchool.Data;
using Framework.Security;

namespace JHEvaluation.AttendCourseDuplReport
{
    public static class Program
    {
        [MainMethod()]
        public static void Main()
        {
            string code = "JHEvaluation.Student.AttendCourseDuplReport";
            Catalog detail = RoleAclSource.Instance["學生"]["報表"];
            detail.Add(new ReportFeature(code, "學生學期修課檢查表"));

            MenuButton mb = K12.Presentation.NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["學生學期修課檢查表"];
            mb.Enable = false;
            mb.Click += delegate
            {
                SemesterSelect selectForm = new SemesterSelect();
                if (selectForm.ShowDialog() == DialogResult.OK)
                {
                    List<JHStudentRecord> students = JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);
                    DuplicationCheck check = new DuplicationCheck(students, selectForm.SchoolYear, selectForm.Semester);
                    check.Run();
                }
            };
            
            //權限判斷。
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) &&
                    Framework.User.Acl[code].Executable;
            };
        }
    }
}
