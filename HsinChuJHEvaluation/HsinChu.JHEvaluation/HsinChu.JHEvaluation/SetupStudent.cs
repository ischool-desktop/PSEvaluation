using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool;
using FISCA.Presentation;
using JHSchool.Data;

namespace HsinChu.JHEvaluation
{
    class SetupStudent
    {
        static EventHandler _eh = FISCA.InteractionService.PublishEvent("CalculationHelper.SaveSemesterScore");

        internal static void Init()
        {
            #region ContentItem 資料項目
            //學期成績
            //Student.Instance.AddDetailBulider(new DetailBulider<HsinChu.JHEvaluation.StudentExtendControls.SemesterScoreItem>());
            //修課及評量成績
            Student.Instance.AddDetailBulider(new DetailBulider<HsinChu.JHEvaluation.StudentExtendControls.CourseScoreItem>());
            #endregion

            #region 學生/資料統計/匯入匯出
            RibbonBarButton rbItemExport = Student.Instance.RibbonBarItems["資料統計"]["匯出"];
            RibbonBarButton rbItemImport = Student.Instance.RibbonBarItems["資料統計"]["匯入"];

            rbItemExport["成績相關匯出"]["匯出學期科目成績"].Enable = Framework.User.Acl["JHSchool.Student.Ribbon0180"].Executable;
            rbItemExport["成績相關匯出"]["匯出學期科目成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new ImportExport.ExportSemesterSubjectScore();
                JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };
            rbItemExport["成績相關匯出"]["匯出學期領域成績"].Enable = Framework.User.Acl["JHSchool.Student.Ribbon0181"].Executable;
            rbItemExport["成績相關匯出"]["匯出學期領域成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new ImportExport.ExportSemesterDomainScore();
                JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };
            rbItemExport["成績相關匯出"]["匯出畢業成績"].Enable = Framework.User.Acl["JHSchool.Student.Ribbon0182"].Executable;
            rbItemExport["成績相關匯出"]["匯出畢業成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new ImportExport.ExportGradScore();
                JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };
            rbItemExport["成績相關匯出"]["匯出評量成績"].Enable = Framework.User.Acl["JHSchool.Student.Ribbon0184"].Executable;
            rbItemExport["成績相關匯出"]["匯出評量成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new ImportExport.ExportExamScore();
                JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            rbItemImport["成績相關匯入"]["匯入學期科目成績"].Enable = Framework.User.Acl["JHSchool.Student.Ribbon0190"].Executable;
            rbItemImport["成績相關匯入"]["匯入學期科目成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer importer = new ImportExport.ImportSemesterSubjectScore();
                JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();

                _eh(null, EventArgs.Empty);
            };
            rbItemImport["成績相關匯入"]["匯入學期領域成績"].Enable = Framework.User.Acl["JHSchool.Student.Ribbon0191"].Executable;
            rbItemImport["成績相關匯入"]["匯入學期領域成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer importer = new ImportExport.ImportSemesterDomainScore();
                JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();

                _eh(null, EventArgs.Empty);
            };
            rbItemImport["成績相關匯入"]["匯入畢業成績"].Enable = Framework.User.Acl["JHSchool.Student.Ribbon0192"].Executable;
            rbItemImport["成績相關匯入"]["匯入畢業成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer importer = new ImportExport.ImportGradScore();
                JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };
            rbItemImport["成績相關匯入"]["匯入評量成績"].Enable = Framework.User.Acl["JHSchool.Student.Ribbon0194"].Executable;
            rbItemImport["成績相關匯入"]["匯入評量成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer importer = new ImportExport.ImportExamScore();
                JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };
            #endregion

            #region Standard Function
            JHSchool.SF.Evaluation.QuickInputSemesterScoreForm.RegisterHandler(delegate(string studentId)
            {
                HsinChu.JHEvaluation.StudentExtendControls.SemesterScoreItemRelated.QuickInputSemesterScoreForm form;
                form = new HsinChu.JHEvaluation.StudentExtendControls.SemesterScoreItemRelated.QuickInputSemesterScoreForm(JHStudent.SelectByID(studentId));
                return form.ShowDialog();
            });
            //JHSchool.SF.Evaluation.SemesterScoreEditor.RegisterHandler(delegate(string studentId)
            //{
            //    HsinChu.JHEvaluation.StudentExtendControls.SemesterScoreItemRelated.SemesterScoreEditor form;
            //    form = new HsinChu.JHEvaluation.StudentExtendControls.SemesterScoreItemRelated.SemesterScoreEditor(Student.Instance[studentId]);
            //    return form.ShowDialog();
            //});
            //JHSchool.SF.Evaluation.SemesterScoreEditor.RegisterHandler(delegate(string studentId, int schoolYear, int semester)
            //{
            //    HsinChu.JHEvaluation.StudentExtendControls.SemesterScoreItemRelated.SemesterScoreEditor form;
            //    form = new HsinChu.JHEvaluation.StudentExtendControls.SemesterScoreItemRelated.SemesterScoreEditor(Student.Instance[studentId], JHSchool.Data.JHSemesterScore.SelectBySchoolYearAndSemester(studentId, schoolYear, semester));
            //    return form.ShowDialog();
            //});
            #endregion
        }
    }
}
