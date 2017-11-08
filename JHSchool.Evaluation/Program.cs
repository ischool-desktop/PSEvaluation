using System.Collections.Generic;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using Framework;
using Framework.Security;
using JHSchool.Affair;
using JHSchool.Evaluation.CourseExtendControls;
using JHSchool.Evaluation.CourseExtendControls.Ribbon;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon;
using System;
using DataRationality;
using K12.Presentation;

namespace JHSchool.Evaluation
{
    public static class Program
    {
        [MainMethod("JHSchool.Evaluation")]
        public static void Main()
        {
            #region SyncAllBackground
            //授課教師
            if (!TCInstruct.Instance.Loaded) TCInstruct.Instance.SyncAllBackground();
            //修課記錄
            //if (!SCAttend.Instance.Loaded) SCAttend.Instance.SyncAllBackground();
            //課程規劃
            if (!ProgramPlan.Instance.Loaded) ProgramPlan.Instance.SyncAllBackground();
            //計算規則
            if (!ScoreCalcRule.Instance.Loaded) ScoreCalcRule.Instance.SyncAllBackground();
            //評量設定
            if (!AssessmentSetup.Instance.Loaded) AssessmentSetup.Instance.SyncAllBackground();
            #endregion

            #region SetupPresentation
            //授課教師
            TCInstruct.Instance.SetupPresentation();
            //修課記錄
            SCAttend.Instance.SetupPresentation();
            //課程規劃
            ProgramPlan.Instance.SetupPresentation();
            //計算規則
            ScoreCalcRule.Instance.SetupPresentation();
            //評量設定
            AssessmentSetup.Instance.SetupPresentation();
            #endregion

            #region ContentItem 資料項目
            //學期成績
            //Student.Instance.AddDetailBulider(new DetailBulider<SemesterScoreItem>());
            //Student.Instance.AddDetailBulider(new DetailBulider<DLScoreItem>());
            //課程基本資訊
            Course.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<CourseExtendControls.BasicInfoItem>());

            //成績計算
            Course.Instance.AddDetailBulider(new DetailBulider<ScoreCalcSetupItem>());
            //修課學生

            Course.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<SCAttendItem>());
            //電子報表(因相關功能未完成先註)
            //Course.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<CourseExtendControls.ElectronicPaperItem>());
            //班級課程規劃
            Class.Instance.AddDetailBulider(new DetailBulider<ClassExtendControls.ClassBaseInfoItem_Extend>());
            
            //暫時註解
            //個人活動表現紀錄
            //Student.Instance.AddDetailBulider(new DetailBulider<StudentActivityRecordItem>());

            // 2017/5/9 穎驊  註解 下面位子 搬去 JHEvaluation.ScoreCalculation
            //畢業成績
            //Student.Instance.AddDetailBulider(new DetailBulider<GraduationScoreItem>());

            //修課及評量成績
            //Student.Instance.AddDetailBulider(new DetailBulider<CourseScoreItem>());
            #endregion

            #region RibbonBar

            NLDPanels.Student.RibbonBarItems["教務"]["成績作業"].Size = RibbonBarButton.MenuButtonSize.Large;
            NLDPanels.Student.RibbonBarItems["教務"]["成績作業"].Image = Properties.Resources.calc_save_64;

            NLDPanels.Student.RibbonBarItems["教務"]["畢業作業"].Size = RibbonBarButton.MenuButtonSize.Large;
            NLDPanels.Student.RibbonBarItems["教務"]["畢業作業"].Image = Properties.Resources.graduated_write_64;

            #region 學生/資料統計/報表
            RibbonBarButton rbButton = Student.Instance.RibbonBarItems["資料統計"]["報表"];

            // 2017/5/9 穎驊  註解 下面位子 搬去 JHEvaluation.ScoreCalculation
            //rbButton["成績相關報表"]["畢業預警報表"].Enable = User.Acl["JHSchool.Student.Report0010"].Executable;
            //rbButton["成績相關報表"]["畢業預警報表"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count <= 0) return;
            //    JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReport report = new JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReport(Student.Instance.SelectedList);
            //};
            //rbButton["學務相關報表"]["畢業預警報表"].Enable = User.Acl["JHSchool.Student.Report0010"].Executable;
            //rbButton["學務相關報表"]["畢業預警報表"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count <= 0) return;
            //    JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReport report = new JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationPredictReport(Student.Instance.SelectedList);
            //};
            #endregion

            #region 學生/資料統計/匯入匯出
            RibbonBarButton rbItemExport = Student.Instance.RibbonBarItems["資料統計"]["匯出"];
            RibbonBarButton rbItemImport = Student.Instance.RibbonBarItems["資料統計"]["匯入"];
            //rbItemExport["匯出學期科目成績"].Enable = User.Acl["JHSchool.Student.Ribbon0180"].Executable;
            //rbItemExport["匯出學期科目成績"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Evaluation.ImportExport.ExportSemesterSubjectScore();
            //    JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
            //    exporter.InitializeExport(wizard);
            //    wizard.ShowDialog();
            //};
            //rbItemExport["匯出學期領域成績"].Enable = User.Acl["JHSchool.Student.Ribbon0181"].Executable;
            //rbItemExport["匯出學期領域成績"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Evaluation.ImportExport.ExportSemesterDomainScore();
            //    JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
            //    exporter.InitializeExport(wizard);
            //    wizard.ShowDialog();
            //};
            //rbItemExport["匯出畢業成績"].Enable = User.Acl["JHSchool.Student.Ribbon0182"].Executable;
            //rbItemExport["匯出畢業成績"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Evaluation.ImportExport.ExportGradScore();
            //    JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
            //    exporter.InitializeExport(wizard);
            //    wizard.ShowDialog();
            //};
            rbItemExport["成績相關匯出"]["匯出課程成績"].Enable = User.Acl["JHSchool.Student.Ribbon0183"].Executable;
            rbItemExport["成績相關匯出"]["匯出課程成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Evaluation.ImportExport.ExportCourseScore();
                JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            rbItemExport["成績相關匯出"]["匯出學期歷程"].Enable = User.Acl["JHSchool.Student.Ribbon0169"].Executable;
            rbItemExport["成績相關匯出"]["匯出學期歷程"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Evaluation.ImportExport.ExportSemesterHistory();
                JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            //rbItemExport["匯出評量成績"].Enable = User.Acl["JHSchool.Student.Ribbon0184"].Executable;
            //rbItemExport["匯出評量成績"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Evaluation.ImportExport.ExportExamScore();
            //    JHSchool.Evaluation.ImportExport.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
            //    exporter.InitializeExport(wizard);
            //    wizard.ShowDialog();
            //};

            //rbItemImport["匯入學期科目成績"].Enable = User.Acl["JHSchool.Student.Ribbon0190"].Executable;
            //rbItemImport["匯入學期科目成績"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Evaluation.ImportExport.ImportSemesterSubjectScore();
            //    JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
            //    importer.InitializeImport(wizard);
            //    wizard.ShowDialog();
            //};
            //rbItemImport["匯入學期領域成績"].Enable = User.Acl["JHSchool.Student.Ribbon0191"].Executable;
            //rbItemImport["匯入學期領域成績"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Evaluation.ImportExport.ImportSemesterDomainScore();
            //    JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
            //    importer.InitializeImport(wizard);
            //    wizard.ShowDialog();
            //};
            //rbItemImport["匯入畢業成績"].Enable = User.Acl["JHSchool.Student.Ribbon0192"].Executable;
            //rbItemImport["匯入畢業成績"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Evaluation.ImportExport.ImportGradScore();
            //    JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
            //    importer.InitializeImport(wizard);
            //    wizard.ShowDialog();
            //};
            rbItemImport["成績相關匯入"]["匯入課程成績"].Enable = User.Acl["JHSchool.Student.Ribbon0193"].Executable;
            rbItemImport["成績相關匯入"]["匯入課程成績"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Evaluation.ImportExport.ImportCourseScore();
                JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            rbItemImport["成績相關匯入"]["匯入學期歷程"].Enable = User.Acl["JHSchool.Student.Ribbon0170"].Executable;
            rbItemImport["成績相關匯入"]["匯入學期歷程"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Evaluation.ImportExport.ImportSemesterHistory();
                JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            //rbItemImport["匯入評量成績"].Enable = User.Acl["JHSchool.Student.Ribbon0194"].Executable;
            //rbItemImport["匯入評量成績"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Evaluation.ImportExport.ImportExamScore();
            //    JHSchool.Evaluation.ImportExport.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.ImportStudentV2(importer.Text, importer.Image);
            //    importer.InitializeImport(wizard);
            //    wizard.ShowDialog();
            //};
            #endregion

            #region 學生/教務
            //移到成績計算模組。
            RibbonBarItem rbItem = Student.Instance.RibbonBarItems["教務"];
            //rbItem["畢業資格審查"].Enable = User.Acl["JHSchool.Student.Ribbon0058"].Executable;
            //rbItem["畢業資格審查"].Image = JHSchool.Evaluation.CourseExtendControls.Ribbon.Resources.graduation_64;
            //rbItem["畢業資格審查"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count == 0) return;
            //    Form form = new JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationInspectWizard("Student");
            //    form.ShowDialog();
            //};
            #endregion

            #region 學生/成績/排名
            /** 程式碼移動到 JHEvaluation.Rating Module 中。*/

            //rbButton = Student.Instance.RibbonBarItems["成績"]["排名"];
            //rbButton.Enable = User.Acl["JHSchool.Student.Ribbon0059"].Executable;
            //rbButton["評量科目成績排名"].Enable = true;
            //rbButton["評量科目成績排名"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count > 0)
            //    {
            //        JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormExamSubject form = new JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormExamSubject();
            //        form.SetRatingStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
            //        form.ShowDialog();
            //    }
            //};

            ////rbButton.Enable = User.Acl[""].Executable;
            //rbButton["評量領域成績排名"].Enable = true;
            //rbButton["評量領域成績排名"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count > 0)
            //    {
            //        JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormExamDomain form= new JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormExamDomain();
            //        form.SetRatingStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
            //        form.ShowDialog();
            //    }
            //};

            //rbButton["學期科目成績排名"].Enable = true;
            //rbButton["學期科目成績排名"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count > 0)
            //    {
            //        JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormSemesterSubject form= new JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormSemesterSubject();
            //        form.SetRatingStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
            //        form.ShowDialog();
            //    }
            //};

            //rbButton["學期領域成績排名"].Enable = true;
            //rbButton["學期領域成績排名"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count > 0)
            //    {
            //        JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormSemesterDomain form = new JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormSemesterDomain();
            //        form.SetRatingStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
            //        form.ShowDialog();
            //    }
            //};

            //rbButton["學期科目成績排名(多學期)"].Enable = true;
            //rbButton["學期科目成績排名(多學期)"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count > 0)
            //    {
            //        JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormSemestersSubject form = new JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormSemestersSubject();
            //        form.SetRatingStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
            //        form.ShowDialog();
            //    }
            //};

            //rbButton["學期領域成績排名(多學期)"].Enable = true;
            //rbButton["學期領域成績排名(多學期)"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count > 0)
            //    {
            //        JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormSemestersDomain form = new JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormSemestersDomain();
            //        form.SetRatingStudents(K12.Presentation.NLDPanels.Student.SelectedSource);
            //        form.ShowDialog();
            //    }
            //};

            //rbButton["畢業成績排名"].Enable = true;
            //rbButton["畢業成績排名"].Click += delegate
            //{
            //    if (Student.Instance.SelectedList.Count > 0)
            //    {
            //        Form form = new JHSchool.Evaluation.StudentExtendControls.Ribbon.Rating.FormGraduation();
            //        form.ShowDialog();
            //    }
            //};
            #endregion

            #region 班級/資料統計/報表
            MenuButton semsGoodStudentReportButton = Class.Instance.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["學期優異表現名單"];
            semsGoodStudentReportButton.Enable = User.Acl["JHSchool.Class.Report0180"].Executable;
            semsGoodStudentReportButton.Click += delegate
            {
                if (Class.Instance.SelectedList.Count > 0)
                {
                    Form form = new JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport.SemsGoodStudentReport();
                    form.ShowDialog();
                }
            };
            #endregion

            #region 班級/教務
            rbButton = K12.Presentation.NLDPanels.Class.RibbonBarItems["教務"]["班級開課"];
            //rbButton = Class.Instance.RibbonBarItems["成績"]["班級開課"];
            rbButton.Enable = User.Acl["JHSchool.Class.Ribbon0070"].Executable;
            rbButton.Image = Properties.Resources.organigram_refresh_64;
            rbButton["依課程規劃表開課"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0)
                    JHSchool.Evaluation.ClassExtendControls.Ribbon.CreateCoursesByProgramPlan.Run();
            };
            rbButton["直接開課"].Click += delegate
            {
                if (Class.Instance.SelectedList.Count > 0) new JHSchool.Evaluation.ClassExtendControls.Ribbon.CreateCoursesDirectly();
            };
            #endregion

            #region 課程/編輯
            rbItem = Course.Instance.RibbonBarItems["編輯"];
            rbButton = rbItem["新增"];
            rbButton.Size = RibbonBarButton.MenuButtonSize.Large;
            rbButton.Image = JHSchool.Evaluation.CourseExtendControls.Ribbon.Resources.btnAddCourse;
            rbButton.Enable = User.Acl["JHSchool.Course.Ribbon0000"].Executable;
            rbButton.Click += delegate
            {
                new JHSchool.Evaluation.CourseExtendControls.Ribbon.AddCourse().ShowDialog();
            };

            rbButton = rbItem["刪除"];
            rbButton.Size = RibbonBarButton.MenuButtonSize.Large;
            rbButton.Image = JHSchool.Evaluation.CourseExtendControls.Ribbon.Resources.btnDeleteCourse;
            rbButton.Enable = User.Acl["JHSchool.Course.Ribbon0010"].Executable;
            rbButton.Click += delegate
            {
                if (Course.Instance.SelectedKeys.Count == 1)
                {
                    JHSchool.Data.JHCourseRecord record = JHSchool.Data.JHCourse.SelectByID(Course.Instance.SelectedKeys[0]);
                    //int CourseAttendCot = Course.Instance.Items[record.ID].GetAttendStudents().Count;
                    List<JHSchool.Data.JHSCAttendRecord> scattendList = JHSchool.Data.JHSCAttend.SelectByStudentIDAndCourseID(new List<string>() { }, new List<string>() { record.ID });
                    int attendStudentCount = 0;
                    foreach (JHSchool.Data.JHSCAttendRecord scattend in scattendList)
                    {
                        if (scattend.Student.Status == K12.Data.StudentRecord.StudentStatus.一般)
                            attendStudentCount++;
                    }

                    if (attendStudentCount > 0)
                        MsgBox.Show(record.Name + " 有" + attendStudentCount.ToString() + "位修課學生，請先移除修課學生後再刪除課程.");
                    else
                    {
                        string msg = string.Format("確定要刪除「{0}」？", record.Name);
                        if (MsgBox.Show(msg, "刪除課程", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            #region 自動刪除非一般學生的修課記錄
                            List<JHSchool.Data.JHSCAttendRecord> deleteSCAttendList = new List<JHSchool.Data.JHSCAttendRecord>();
                            foreach (JHSchool.Data.JHSCAttendRecord scattend in scattendList)
                            {
                                JHSchool.Data.JHStudentRecord stuRecord = JHSchool.Data.JHStudent.SelectByID(scattend.RefStudentID);
                                if (stuRecord == null) continue;
                                if (stuRecord.Status != K12.Data.StudentRecord.StudentStatus.一般)
                                    deleteSCAttendList.Add(scattend);
                            }
                            List<string> studentIDs = new List<string>();
                            foreach (JHSchool.Data.JHSCAttendRecord scattend in deleteSCAttendList)
                                studentIDs.Add(scattend.RefStudentID);
                            List<JHSchool.Data.JHSCETakeRecord> sceList = JHSchool.Data.JHSCETake.SelectByStudentAndCourse(studentIDs, new List<string>() { record.ID });
                            JHSchool.Data.JHSCETake.Delete(sceList);
                            JHSchool.Data.JHSCAttend.Delete(deleteSCAttendList);
                            #endregion

                            JHSchool.Data.JHCourse.Delete(record);
                            //CourseRecordEditor crd = Course.Instance.Items[record.ID].GetEditor();
                            //crd.Remove = true;
                            //crd.Save();
                            // 加這主要是重新整理
                            Course.Instance.SyncDataBackground(record.ID);
                        }
                        else
                            return;
                    }
                }
            };

            RibbonBarButton CouItem = Course.Instance.RibbonBarItems["編輯"]["刪除"];
            Course.Instance.SelectedListChanged += delegate
            {
                // 課程刪除不能多選
                CouItem.Enable = (Course.Instance.SelectedList.Count < 2) && User.Acl["JHSchool.Course.Ribbon0010"].Executable;
            };
            #endregion

            #region 課程/資料統計/報表
            rbButton = Course.Instance.RibbonBarItems["資料統計"]["報表"];
            rbButton["學生修課清單"].Enable = User.Acl["JHSchool.Course.Report0000"].Executable;
            rbButton["學生修課清單"].Click += delegate
            {
                if (Course.Instance.SelectedList.Count >= 1)
                {
                    new JHSchool.Evaluation.ClassExtendControls.Ribbon.StuinCourse.StuinCourse();
                }
                else
                    MsgBox.Show("請選擇課程");
            };
            #endregion

            #region 課程/資料統計/匯入匯出
            RibbonBarItem rbItemCourseImportExport = Course.Instance.RibbonBarItems["資料統計"];
            rbItemCourseImportExport["匯出"]["匯出課程修課學生"].Enable = User.Acl["JHSchool.Course.Ribbon0031"].Executable;
            rbItemCourseImportExport["匯出"]["匯出課程修課學生"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Evaluation.ImportExport.Course.ExportCourseStudents("");
                JHSchool.Evaluation.ImportExport.Course.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.Course.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };
            rbItemCourseImportExport["匯入"]["匯入課程修課學生"].Enable = User.Acl["JHSchool.Course.Ribbon0021"].Executable;
            rbItemCourseImportExport["匯入"]["匯入課程修課學生"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Evaluation.ImportExport.Course.ImportCourseStudents("");
                JHSchool.Evaluation.ImportExport.Course.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.Course.ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            //rbItemCourseImportExport = FISCA.Presentation.MotherForm.RibbonBarItems["社團作業", "資料統計"];
            //rbItemCourseImportExport["匯出"]["匯出社團參與學生"].Enable = User.Acl["JHSchool.Course.Ribbon0031"].Executable;
            //rbItemCourseImportExport["匯出"]["匯出社團參與學生"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Evaluation.ImportExport.Course.ExportCourseStudents("社團");
            //    JHSchool.Evaluation.ImportExport.Course.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.Course.ExportStudentV2(exporter.Text, exporter.Image, "社團", K12.Presentation.NLDPanels.Course.SelectedSource);
            //    exporter.InitializeExport(wizard);
            //    wizard.ShowDialog();
            //};
            //rbItemCourseImportExport["匯入"]["匯入社團參與學生"].Enable = User.Acl["JHSchool.Course.Ribbon0021"].Executable;
            //rbItemCourseImportExport["匯入"]["匯入社團參與學生"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Evaluation.ImportExport.Course.ImportCourseStudents("社團");
            //    JHSchool.Evaluation.ImportExport.Course.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.Course.ImportStudentV2(importer.Text, importer.Image);
            //    importer.InitializeImport(wizard);
            //    wizard.ShowDialog();
            //};
            #endregion

            #region 課程/教務
            RibbonBarButton group = Course.Instance.RibbonBarItems["教務"]["分組上課"];
            group.Size = RibbonBarButton.MenuButtonSize.Medium;
            group.Image = Properties.Resources.meeting_refresh_64;
            group.Enable = User.Acl["JHSchool.Course.Ribbon0060"].Executable;
            group.Click += delegate
            {
                if (Course.Instance.SelectedList.Count > 0)
                    new JHSchool.Evaluation.Legacy.SwapAttendStudents().ShowDialog();
            };

            //課程超過7項,則"分組上課"不能點擊
            Course.Instance.SelectedListChanged += delegate
            {
                //分組上課不能超過七個課程。
                group.Enable = (Course.Instance.SelectedList.Count <= 7) && User.Acl["JHSchool.Course.Ribbon0060"].Executable;
            };

            RibbonBarItem scores = Course.Instance.RibbonBarItems["教務"];
            //scores["成績輸入"].Size = RibbonBarButton.MenuButtonSize.Medium;
            //scores["成績輸入"].Image = Resources.exam_write_64;
            //scores["成績輸入"].Enable = User.Acl["JHSchool.Course.Ribbon0070"].Executable;
            //scores["成績輸入"].Click += delegate
            //{
            //    if (Course.Instance.SelectedList.Count == 1)
            //    {
            //        //new JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScore(Course.Instance.SelectedList[0]).ShowDialog();
            //        CourseRecord courseRecord = Course.Instance.SelectedList[0];
            //        if (courseRecord.GetAssessmentSetup() == null)
            //            MsgBox.Show("課程 '" + courseRecord.Name + "' 沒有評量設定。");
            //        else
            //            new JHSchool.Evaluation.CourseExtendControls.Ribbon.CourseScoreInputForm(courseRecord).ShowDialog();
            //    }
            //};
            //Course.Instance.SelectedListChanged += delegate
            //{
            //    scores["成績輸入"].Enable = Course.Instance.SelectedList.Count == 1 && User.Acl["JHSchool.Course.Ribbon0070"].Executable;
            //};

            //scores["成績計算"].Size = RibbonBarButton.MenuButtonSize.Medium;
            //scores["成績計算"].Image = Resources.calcScore;
            //scores["成績計算"].Enable = User.Acl["JHSchool.Course.Ribbon0080"].Executable;
            //scores["成績計算"].Click += delegate
            //{
            //    new JHSchool.Evaluation.CourseExtendControls.Ribbon.CalculateionWizard().ShowDialog();
            //};

            // UNDONE: 等有討論出什麼結論再說吧…
            //scores["合科什麼鬼的"].Size = RibbonBarButton.MenuButtonSize.Medium;
            //scores["合科什麼鬼的"].Enable = true;
            //scores["合科什麼鬼的"].Click += delegate
            //{
            //    List<JHSchool.Data.JHCourseRecord> courseList = JHSchool.Data.JHCourse.SelectByIDs(K12.Presentation.NLDPanels.Course.SelectedSource);
            //    if (courseList.Count <= 0) return;

            //    if (SubjectCombinationConfigForm.CheckAssessmentSetup(courseList) == true)
            //    {
            //        new SubjectCombinationConfigForm(courseList).ShowDialog();
            //    }
            //};
            #endregion

            #region 教務作業/課務作業

            rbItem = EduAdmin.Instance.RibbonBarItems["基本設定"];

            rbItem["管理"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["管理"].Image = Properties.Resources.network_lock_64;
            rbItem["管理"]["領域資料管理"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon.DomainList"].Executable;
            rbItem["管理"]["領域資料管理"].Click += delegate
            {
                new DomainListTable().ShowDialog();
            };

            rbItem["管理"]["科目資料管理"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon.DomainList"].Executable;
            rbItem["管理"]["科目資料管理"].Click += delegate
            {
                new SubjectListTable().ShowDialog();
            };

            //rbItem["管理"]["評量名稱管理"].Image = Resources.評量名稱管理;
            rbItem["管理"]["評量名稱管理"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon0000"].Executable;
            rbItem["管理"]["評量名稱管理"].Click += delegate
            {
                new JHSchool.Evaluation.CourseExtendControls.Ribbon.ExamManager().ShowDialog();
            };

            //rbItem["等第對照表"].Size = RibbonBarButton.MenuButtonSize.Medium;
            //rbItem["等第對照表"].Image = Resources.對照表;
            rbItem["管理"]["等第對照管理"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon0031"].Executable;
            rbItem["管理"]["等第對照管理"].Click += delegate
            {
                new JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ScoreMappingTable().ShowDialog();
            };
            #endregion

            #region 教務作業/成績作業
            rbItem["對照/代碼"].Image = Properties.Resources.notepad_lock_64;
            rbItem["對照/代碼"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["對照/代碼"]["文字描述代碼表"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon0032"].Executable;
            rbItem["對照/代碼"]["文字描述代碼表"].Click += delegate
            {
                TextMappingTable text = new TextMappingTable();
                text.ShowDialog();
            };

            //rbItem["努力程度對照表"].Size = RibbonBarButton.MenuButtonSize.Medium;
            //rbItem["努力程度對照表"].Image = Resources.對照表;
            rbItem["對照/代碼"]["努力程度代碼表"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon0030"].Executable;
            rbItem["對照/代碼"]["努力程度代碼表"].Click += delegate
            {
                new JHSchool.Evaluation.EduAdminExtendControls.Ribbon.EffortDegreeTable().ShowDialog();
            };
            EffortDegreeTable.CheckDefault();

            //如果需要此分類再進行應用
            rbItem["設定"].Image = Properties.Resources.sandglass_unlock_64;
            rbItem["設定"].Size = RibbonBarButton.MenuButtonSize.Large;

            ScoreMappingTable.CheckDefault();

            //rbItem["課程規劃表"].Size = RibbonBarButton.MenuButtonSize.Large;
            //rbItem["課程規劃表"].Image = ClassExtendControls.Ribbon.Resources.btnProgramPlan_Image;
            rbItem["設定"]["課程規劃表"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon0050"].Executable;
            rbItem["設定"]["課程規劃表"].Click += delegate
            {
                new ProgramPlanManager().ShowDialog();
            };

            //rbItem["成績計算規則"].Size = RibbonBarButton.MenuButtonSize.Large;
            //rbItem["成績計算規則"].Image = JHSchool.Evaluation.CourseExtendControls.Ribbon.Resources.course_plan;
            rbItem["設定"]["成績計算規則"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon0040"].Executable;
            rbItem["設定"]["成績計算規則"].Click += delegate
            {
                //if (Control.ModifierKeys == Keys.Shift)
                new JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ScoreCalcRuleManager().ShowDialog();
            };


            //移到成績計算模組。
            //rbItem["畢業資格審查"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon0046"].Executable;
            //rbItem["畢業資格審查"].Image = JHSchool.Evaluation.CourseExtendControls.Ribbon.Resources.graduation_64;
            //rbItem["畢業資格審查"].Click += delegate
            //{
            //    Form form = new JHSchool.Evaluation.StudentExtendControls.Ribbon.GraduationInspectWizard("EduAdmin");
            //    form.ShowDialog();
            //};

            //rbItem["評量輸入狀況"].Image = Resources.成績輸入檢查;
            //rbItem["評量輸入狀況"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon0020"].Executable;
            //rbItem["評量輸入狀況"].Click += delegate
            //{
            //    new JHSchool.Evaluation.EduAdminExtendControls.Ribbon.CourseScoreStatusForm().ShowDialog();
            //};

            //rbItem["評量設定"].Image = Resources.評量設定;
            //rbItem["評量設定"].Enable = User.Acl["JHSchool.EduAdmin.Ribbon0010"].Executable;
            //rbItem["評量設定"].Click += delegate
            //{
            //    new JHSchool.Evaluation.CourseExtendControls.Ribbon.AssessmentSetupManager().ShowDialog();
            //};

            //rbItem["特殊教育領域設定"].Image = Resources.;

            //rbItem["特殊教育領域設定"].Enable = true;
            //rbItem["特殊教育領域設定"].Click += delegate
            //{
            //    new SpecialEduDomainTable().ShowDialog();
            //};
            #endregion

            #endregion

            //--------------------------------下面還沒整理的分隔線--------------------------------

            #region 註冊權限管理
            
            //學生
            Catalog detail = RoleAclSource.Instance["學生"]["資料項目"];

            // 2017/5/9 穎驊  註解 下面位子 搬去 JHEvaluation.ScoreCalculation
            //detail.Add(new DetailItemFeature(typeof(SemesterScoreItem)));
            //detail.Add(new DetailItemFeature(typeof(GraduationScoreItem)));
            //detail.Add(new DetailItemFeature(typeof(CourseScoreItem)));

            Catalog ribbon = RoleAclSource.Instance["學生"]["功能按鈕"];
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0055", "課程規劃"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0056", "計算規則"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0057", "計算成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0058", "畢業資格審查"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0059", "排名")); //程式碼移動到 JHEvaluation.Rating Module 中。

            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0180", "匯出學期科目成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0181", "匯出學期領域成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0182", "匯出畢業成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0183", "匯出課程成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0184", "匯出評量成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0169", "匯出學期歷程"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0190", "匯入學期科目成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0191", "匯入學期領域成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0192", "匯入畢業成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0193", "匯入課程成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0194", "匯入評量成績"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0170", "匯入學期歷程"));

            ribbon = RoleAclSource.Instance["學生"]["報表"];
            ribbon.Add(new ReportFeature("JHSchool.Student.Report0010", "畢業預警報表"));

            //班級
            ribbon = RoleAclSource.Instance["班級"]["功能按鈕"];
            ribbon.Add(new RibbonFeature("JHSchool.Class.Ribbon0055", "課程規劃"));
            ribbon.Add(new RibbonFeature("JHSchool.Class.Ribbon0056", "計算規則"));
            ribbon.Add(new RibbonFeature("JHSchool.Class.Ribbon0070", "班級開課"));

            ribbon = RoleAclSource.Instance["班級"]["報表"];
            ribbon.Add(new RibbonFeature("JHSchool.Class.Report0180", "學期優異表現名單"));

            //課程
            ribbon = RoleAclSource.Instance["課程"]["功能按鈕"];
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0031", "匯出課程修課學生"));
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0021", "匯入課程修課學生"));
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon.AssignAssessmentSetup", "評量設定")); //增加權限控管 by dylan(2010/11/25)

            detail = RoleAclSource.Instance["課程"]["資料項目"];
            detail.Add(new DetailItemFeature(typeof(CourseExtendControls.BasicInfoItem)));
            detail.Add(new DetailItemFeature(typeof(CourseExtendControls.ScoreCalcSetupItem)));
            detail.Add(new DetailItemFeature(typeof(CourseExtendControls.SCAttendItem)));

            // //電子報表(因相關功能未完成先註)
            //detail.Add(new DetailItemFeature(typeof(CourseExtendControls.ElectronicPaperItem)));

            //教務作業
            ribbon = RoleAclSource.Instance["教務作業"];
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon.DomainList", "領域清單"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon.SubjectList", "科目清單"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0000", "評量名稱管理"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0010", "評量設定"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0020", "評量輸入狀況"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0030", "努力程度對照表"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0031", "等第對照表"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0032", "文字描述代碼表"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0040", "成績計算規則"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0045", "計算成績"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0046", "畢業資格審查"));
            ribbon.Add(new RibbonFeature("JHSchool.EduAdmin.Ribbon0050", "課程規劃表"));

            //建文的舊功能
            //ribbon = RoleAclSource.Instance["學務作業"];
            //ribbon.Add(new RibbonFeature("JHSchool.StuAdmin.Ribbon0090.2", "幹部名稱管理"));
            //ribbon.Add(new RibbonFeature("JHSchool.StuAdmin.Ribbon0091", "班級幹部管理"));
            //ribbon.Add(new RibbonFeature("JHSchool.StuAdmin.Ribbon0092", "社團幹部管理"));
            //ribbon.Add(new RibbonFeature("JHSchool.StuAdmin.Ribbon0093", "學校幹部管理"));
            //ribbon.Add(new RibbonFeature("JHSchool.StuAdmin.Ribbon0094", "競賽項目管理"));
            #endregion

            Domain.TestDrive();
            Course.Instance.AddView(new TeacherCategoryView());

            // 2017/5/9 穎驊  註解 下面位子 搬去 JHEvaluation.ScoreCalculation
            // 學生學期歷程與學期成績學年度學期檢查
            //DataRationalityManager.Checks.Add(new StudentExtendControls.Ribbon.CheckStudentSemHistoryScoreRAT());

        }
    }
}
