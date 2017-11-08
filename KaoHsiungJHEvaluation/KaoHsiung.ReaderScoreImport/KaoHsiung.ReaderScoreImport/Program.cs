using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using Framework.Security;
using FISCA.Presentation;
using Framework;

namespace KaoHsiung.ReaderScoreImport
{
    /// <summary>
    /// 讀卡
    /// </summary>
    public static class Program
    {
        [MainMethod]
        public static void Main()
        {
            RibbonBarItem rbItem = FISCA.Presentation.MotherForm.RibbonBarItems["課程", "讀卡"];

            RibbonBarButton importButton = rbItem["匯入讀卡成績"];
            importButton.Size = RibbonBarButton.MenuButtonSize.Large;
            importButton.Enable = User.Acl["KaoHsiung.JHEvaluation.Course.ReaderScoreImport01"].Executable;
            importButton.Image = Properties.Resources.proyector_save_64;
            importButton.Click += delegate
            {
                ImportStartupForm form = new ImportStartupForm();
                form.ShowDialog();
            };

            RibbonBarButton classButton = rbItem["班級代碼設定"];
            classButton.Size = RibbonBarButton.MenuButtonSize.Small;
            classButton.Enable = User.Acl["KaoHsiung.JHEvaluation.Course.ReaderScoreImport02"].Executable; ;
            classButton.Click += delegate
            {
                new ClassCodeConfig().ShowDialog();
            };

            RibbonBarButton examButton = rbItem["試別代碼設定"];
            examButton.Size = RibbonBarButton.MenuButtonSize.Small;
            examButton.Enable = User.Acl["KaoHsiung.JHEvaluation.Course.ReaderScoreImport03"].Executable; ;
            examButton.Click += delegate
            {
                new ExamCodeConfig().ShowDialog();
            };

            RibbonBarButton subjectButton = rbItem["科目代碼設定"];
            subjectButton.Size = RibbonBarButton.MenuButtonSize.Small;
            subjectButton.Enable = User.Acl["KaoHsiung.JHEvaluation.Course.ReaderScoreImport04"].Executable; ;
            subjectButton.Click += delegate
            {
                new SubjectCodeConfig().ShowDialog();
            };


            Catalog detail = RoleAclSource.Instance["課程"]["功能按鈕"];
            detail.Add(new ReportFeature("KaoHsiung.JHEvaluation.Course.ReaderScoreImport01", "匯入讀卡成績"));
            detail.Add(new ReportFeature("KaoHsiung.JHEvaluation.Course.ReaderScoreImport02", "班級代碼設定"));
            detail.Add(new ReportFeature("KaoHsiung.JHEvaluation.Course.ReaderScoreImport03", "試別代碼設定"));
            detail.Add(new ReportFeature("KaoHsiung.JHEvaluation.Course.ReaderScoreImport04", "科目代碼設定"));
        }
    }
}
