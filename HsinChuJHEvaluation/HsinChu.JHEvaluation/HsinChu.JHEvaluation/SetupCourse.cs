using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using JHSchool;
using JHSchool.Evaluation;

namespace HsinChu.JHEvaluation
{
    class SetupCourse
    {
        internal static void Init()
        {
            #region RibbonBar

            #region 課程/成績
            RibbonBarItem scores = JHSchool.Course.Instance.RibbonBarItems["教務"];
            scores["成績輸入"].Size = RibbonBarButton.MenuButtonSize.Medium;
            scores["成績輸入"].Image = HsinChu.JHEvaluation.Properties.Resources.exam_write_64;
            scores["成績輸入"].Enable = Framework.User.Acl["JHSchool.Course.Ribbon0070"].Executable;
            scores["成績輸入"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 1)
                {
                    CourseRecord courseRecord = Course.Instance.SelectedList[0];
                    if (AssessmentSetup.Instance[courseRecord.RefAssessmentSetupID] == null)
                        FISCA.Presentation.Controls.MsgBox.Show("課程 '" + courseRecord.Name + "' 沒有評量設定。");
                    else
                        new HsinChu.JHEvaluation.CourseExtendControls.Ribbon.CourseScoreInputForm(courseRecord).ShowDialog();
                }
            };
            K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate
            {
                scores["成績輸入"].Enable = K12.Presentation.NLDPanels.Course.SelectedSource.Count == 1 && Framework.User.Acl["JHSchool.Course.Ribbon0070"].Executable;
            };
            #endregion

            #endregion

            //#region Register Standard Function
            //JHSchool.SF.Evaluation.CourseScoreInputForm.RegisterHandler(delegate(string courseId)
            //{
            //    HsinChu.JHEvaluation.CourseExtendControls.Ribbon.CourseScoreInputForm form;
            //    form = new HsinChu.JHEvaluation.CourseExtendControls.Ribbon.CourseScoreInputForm(Course.Instance[courseId]);
            //    return form.ShowDialog();
            //});
            //#endregion
        }
    }
}
