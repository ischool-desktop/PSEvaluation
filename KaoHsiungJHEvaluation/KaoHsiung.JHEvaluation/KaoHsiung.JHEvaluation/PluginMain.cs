using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA;

namespace KaoHsiung.JHEvaluation
{
    public static class PluginMain
    {
        [Dependency("JHSchool.Evaluation")]
        [MainMethod("KaoHsiung.JHEvaluation")]
        public static void Main()
        {
            //if (System.IO.File.Exists(System.IO.Path.Combine(Application.StartupPath, "高雄開發")))
            //{
                SetupStudent.Init();
                SetupCourse.Init();
                SetupEduAdmin.Init();
            //}
        }
    }
}
