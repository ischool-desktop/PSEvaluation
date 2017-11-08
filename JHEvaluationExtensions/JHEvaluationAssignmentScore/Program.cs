using System;
using System.Collections.Generic;
using System.Text;
using FISCA.Presentation;
using FISCA;
using FISCA.Permission;

namespace JHEvaluation.AssignmentScore
{
    public class Program
    {
        [MainMethod]
        public static void Main()
        {
            Global.Params = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");

            if (Global.Params["Mode"].ToUpper() == "KaoHsiung".ToUpper())
            {
                RibbonBarButton button = MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績作業"];
                //button.Size = RibbonBarButton.MenuButtonSize.Large;
                //button.Image = Properties.Resources.preview_info_64;
                button["平時評量輸入狀況檢視"].Enable = UserAcl.Current["JHSchool.Course.Ribbon.CourseScoreStatusForm"].Executable;
                button["平時評量輸入狀況檢視"].Click += delegate
                {
                    CourseScoreStatusForm cssf = new CourseScoreStatusForm();
                    cssf.ShowDialog();
                };

                RibbonBarButton button1 = MotherForm.RibbonBarItems["課程", "教務"]["平時評量輸入"];
                button1.Image = Resource1.skills_ok_64;
                button1.Enable = false;
                button1.Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 1)
                    {
                        JHSchool.Data.JHCourseRecord courseRec = JHSchool.Data.JHCourse.SelectByID(K12.Presentation.NLDPanels.Course.SelectedSource[0]);

                        MainFormScoreInput mainForm = new MainFormScoreInput(courseRec);
                        mainForm.ShowDialog();
                    }
                };

                K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate
                {
                    button1.Enable = UserAcl.Current["JHSchool.Course.Ribbon.MainFormScoreInput"].Executable && (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 1);
                };

                //增加權限控管 by dylan(2010/11/25)

                Catalog ribbon1 = RoleAclSource.Instance["教務作業"];
                ribbon1.Add(new RibbonFeature("JHSchool.Course.Ribbon.CourseScoreStatusForm", "平時評量輸入狀況"));

                Catalog ribbon2 = RoleAclSource.Instance["課程"]["功能按鈕"];
                ribbon2.Add(new RibbonFeature("JHSchool.Course.Ribbon.MainFormScoreInput", "平時評量輸入"));
            }
        }

    }
}
