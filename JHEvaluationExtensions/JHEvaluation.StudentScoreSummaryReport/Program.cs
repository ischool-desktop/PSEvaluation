using System;
using System.Collections.Generic;
using System.Text;
using FISCA;
using Framework.Security;
using FISCA.Presentation;
using K12.Presentation;
using FISCA.Deployment;
using System.Windows.Forms;
using JHSchool.Data;
using System.IO;

namespace JHEvaluation.StudentScoreSummaryReport
{
    public static class Program
    {
        internal static ModuleMode Mode { get; private set; }

        //權限代碼。
        private const string PermissionCode_SutdentReport = "JHEvaluation.Student.StudentReport";

        private const string PermissionCode = "JHEvaluation.Student.StudentScoreSummaryReport";
        private const string PermissionCodeEnglish = "JHEvaluation.Student.StudentScoreSummaryReportEnglish";

        [MainMethod()]
        [STAThread()]
        public static void Main()
        {
            DeployModeSetup();

# if LocalDebug
            DeployParameters dparams = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");
            FISCA.Authentication.DSAServices.SetLicense(@"C:\Users\yaoming\Desktop\ischool\SmartSchoolLicense.key");
            FISCA.Authentication.DSAServices.Login("admin", "1234");

            //JHClassRecord cls = JHClass.SelectByID("24");//正興 301 班。

            List<string> students = new List<string>();
            //foreach (JHStudentRecord each in cls.Students)
            //    students.Add(each.ID);

            //呵!
            students.Clear();
            students.Add("852");

            // test.hc 143136

            Aspose.Words.License lic = new Aspose.Words.License();
            lic.SetLicense(new MemoryStream(Prc.Aspose_Total));
            //new PrintForm(students).ShowDialog();
            new PrintFormEnglish(students).ShowDialog();
# else

            //註冊權限管理項目。
            Catalog detail = RoleAclSource.Instance["學生"]["報表"];

            detail.Add(new ReportFeature(PermissionCode_SutdentReport, "學籍表"));
            detail.Add(new ReportFeature(PermissionCode, "在校成績證明書"));
            detail.Add(new ReportFeature(PermissionCodeEnglish, "在校成績證明書(英文)"));

            //註冊報表功能項目。

            MenuButton mb0 = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["學籍表"];
            mb0.Enable = false;
            mb0.Click += delegate
            {
                new PrintForm_StudentReport(K12.Presentation.NLDPanels.Student.SelectedSource).ShowDialog();

                //抓取學生資料 

                //List<K12.Data.SemesterScoreRecord> ssr_list = K12.Data.SemesterScore.SelectByStudentIDs(K12.Presentation.NLDPanels.Student.SelectedSource);

                //List<K12.Data.AttendanceRecord> ar_list = K12.Data.Attendance.SelectByStudentIDs(K12.Presentation.NLDPanels.Student.SelectedSource);

                //List<K12.Data.StudentRecord> sr_list = K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);

                //List<K12.Data.UpdateRecordBatchRecord> urbr_list = K12.Data.UpdateRecordBatch.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);

                //List<K12.Data.UpdateRecordRecord> urr_list = K12.Data.UpdateRecord.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);

                //List<K12.Data.GradScoreRecord> gsr_list = K12.Data.GradScore.SelectByIDs<K12.Data.GradScoreRecord>(K12.Presentation.NLDPanels.Student.SelectedSource);

                //List<K12.Data.SemesterHistoryRecord> shr_list = K12.Data.SemesterHistory.SelectByStudentIDs(K12.Presentation.NLDPanels.Student.SelectedSource);

                //List<K12.Data.MoralScoreRecord> msr_list = K12.Data.MoralScore.SelectByStudentIDs(K12.Presentation.NLDPanels.Student.SelectedSource);

                //CreateFieldTemplate();
            };


            MenuButton mb = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["在校成績證明書"];
            mb.Enable = false;
            mb.Click += delegate
            {
                //new PrintForm(K12.Presentation.NLDPanels.Student.SelectedSource).ShowDialog();
                CreateFieldTemplate();
            };

            MenuButton mb2 = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["成績相關報表"]["在校成績證明書(英文版)"];
            mb2.Enable = false;
            mb2.Click += delegate
            {
                new PrintFormEnglish(K12.Presentation.NLDPanels.Student.SelectedSource).ShowDialog();
            };

           
            //權限判斷。
            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                mb0.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) &&
                    Framework.User.Acl[PermissionCode_SutdentReport].Executable;

                mb.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) &&
                    Framework.User.Acl[PermissionCode].Executable;

                mb2.Enable = (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0) &&
                    Framework.User.Acl[PermissionCodeEnglish].Executable;                
            };
#endif
        }

        private static void DeployModeSetup()
        {
            DeployParameters dparams = ModuleLoader.GetDeployParametsers(typeof(Program), "Mode=KaoHsiung");

            if (dparams["Mode"].ToUpper() == "KaoHsiung".ToUpper())
                Mode = ModuleMode.KaoHsiung; //高雄。
            else
                Mode = ModuleMode.HsinChu;  //新竹。

            //            Mode = ModuleMode.KaoHsiung; //高雄。

            //            Mode = ModuleMode.HsinChu;  //新竹。
        }


        // 穎驊搬過來的工具，可以一次大量建立有規則的功能變數，可以省下很多時間。
        internal static void CreateFieldTemplate()
        {
            #region 產生欄位表
            Aspose.Words.Document doc = new Aspose.Words.Document(new System.IO.MemoryStream(Properties.Resources.Template));
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            int maxNum = 30;


            #region 康橋學籍表功能變數


            #region 領域成績功能變數
            builder.Writeln("領域成績");
            builder.StartTable();
            builder.InsertCell();
            builder.Write("一年級上學期");
            builder.InsertCell();
            builder.Write("一年級下學期");
            builder.InsertCell();
            builder.Write("二年級上學期");
            builder.InsertCell();
            builder.Write("二年級下學期");
            builder.InsertCell();
            builder.Write("三年級上學期");
            builder.InsertCell();
            builder.Write("三年級下學期");
            builder.InsertCell();
            builder.Write("四年級上學期");
            builder.InsertCell();
            builder.Write("四年級下學期");
            builder.InsertCell();
            builder.Write("五年級上學期");
            builder.InsertCell();
            builder.Write("五年級下學期");
            builder.InsertCell();
            builder.Write("六年級上學期");
            builder.InsertCell();
            builder.Write("六年級下學期");

            List<string> domain_list = new List<string>();

            domain_list.Add("領域_語文_成績");
            domain_list.Add("領域_語文_等第");
            domain_list.Add("領域_數學_成績");
            domain_list.Add("領域_數學_等第");
            domain_list.Add("領域_生活課程_成績");
            domain_list.Add("領域_生活課程_等第");
            domain_list.Add("領域_自然與生活科技_成績");
            domain_list.Add("領域_自然與生活科技_等第");
            domain_list.Add("領域_藝術與人文_成績");
            domain_list.Add("領域_藝術與人文_等第");
            domain_list.Add("領域_社會_成績");
            domain_list.Add("領域_社會_等第");
            domain_list.Add("領域_健康與體育_成績");
            domain_list.Add("領域_健康與體育_等第");
            domain_list.Add("領域_綜合活動_成績");
            domain_list.Add("領域_綜合活動_等第");
            domain_list.Add("領域_學習領域總成績_成績");
            domain_list.Add("領域_學習領域總成績_等第");


            builder.EndRow();
            for (int i = 1; i <= domain_list.Count; i++)
            {
                for (int s = 1; s <= 12; s++)
                {
                    builder.InsertCell();
                    builder.InsertField("MERGEFIELD " + domain_list[i - 1] + "_" + s + " \\* MERGEFORMAT ", "«D" + s + "»");
                }

                builder.EndRow();
            }

            builder.EndTable();
            #endregion


            #region 領域成績功能變數
            builder.Writeln("科目成績");
            builder.StartTable();
            builder.InsertCell();
            builder.Write("一年級上學期");
            builder.InsertCell();
            builder.Write("一年級下學期");
            builder.InsertCell();
            builder.Write("二年級上學期");
            builder.InsertCell();
            builder.Write("二年級下學期");
            builder.InsertCell();
            builder.Write("三年級上學期");
            builder.InsertCell();
            builder.Write("三年級下學期");
            builder.InsertCell();
            builder.Write("四年級上學期");
            builder.InsertCell();
            builder.Write("四年級下學期");
            builder.InsertCell();
            builder.Write("五年級上學期");
            builder.InsertCell();
            builder.Write("五年級下學期");
            builder.InsertCell();
            builder.Write("六年級上學期");
            builder.InsertCell();
            builder.Write("六年級下學期");

            List<string> subject_list = new List<string>();

            subject_list.Add("科目_國語_成績");
            subject_list.Add("科目_國語_等第");
            subject_list.Add("科目_英語_成績");
            subject_list.Add("科目_英語_等第");
            


            builder.EndRow();
            for (int i = 1; i <= subject_list.Count; i++)
            {
                for (int s = 1; s <= 12; s++)
                {
                    builder.InsertCell();
                    builder.InsertField("MERGEFIELD " + subject_list[i - 1] + "_" + s + " \\* MERGEFORMAT ", "«S" + s + "»");
                }

                builder.EndRow();
            }

            builder.EndTable();
            #endregion


            #region 出缺勤功能變數
            builder.Writeln("出缺勤紀錄");
            builder.StartTable();
            builder.InsertCell();
            builder.Write("一年級上學期");
            builder.InsertCell();
            builder.Write("一年級下學期");
            builder.InsertCell();
            builder.Write("二年級上學期");
            builder.InsertCell();
            builder.Write("二年級下學期");
            builder.InsertCell();
            builder.Write("三年級上學期");
            builder.InsertCell();
            builder.Write("三年級下學期");
            builder.InsertCell();
            builder.Write("四年級上學期");
            builder.InsertCell();
            builder.Write("四年級下學期");
            builder.InsertCell();
            builder.Write("五年級上學期");
            builder.InsertCell();
            builder.Write("五年級下學期");
            builder.InsertCell();
            builder.Write("六年級上學期");
            builder.InsertCell();
            builder.Write("六年級下學期");

            List<string> attendance_list = new List<string>();

            attendance_list.Add("應出席日數");
            attendance_list.Add("事假日數");
            attendance_list.Add("病假日數");
            attendance_list.Add("公假日數");
            attendance_list.Add("喪假日數");
            attendance_list.Add("曠課日數");
            attendance_list.Add("缺席總日數");


            builder.EndRow();
            for (int i = 1; i <= attendance_list.Count; i++)
            {
                for (int s = 1; s <= 12; s++)
                {
                    builder.InsertCell();
                    builder.InsertField("MERGEFIELD " + attendance_list[i - 1] + "_" + s + " \\* MERGEFORMAT ", "«A" + s + "»");
                }

                builder.EndRow();
            }

            builder.EndTable();
            #endregion

            #region 日常生活表現及具體建議功能變數
            builder.Writeln("日常生活表現及具體建議");
            builder.StartTable();
            builder.InsertCell();
            builder.Write("一年級上學期");
            builder.InsertCell();
            builder.Write("一年級下學期");
            builder.InsertCell();
            builder.Write("二年級上學期");
            builder.InsertCell();
            builder.Write("二年級下學期");
            builder.InsertCell();
            builder.Write("三年級上學期");
            builder.InsertCell();
            builder.Write("三年級下學期");
            builder.InsertCell();
            builder.Write("四年級上學期");
            builder.InsertCell();
            builder.Write("四年級下學期");
            builder.InsertCell();
            builder.Write("五年級上學期");
            builder.InsertCell();
            builder.Write("五年級下學期");
            builder.InsertCell();
            builder.Write("六年級上學期");
            builder.InsertCell();
            builder.Write("六年級下學期");

            List<string> daily_list = new List<string>();

            daily_list.Add("日常生活表現及具體建議");
           


            builder.EndRow();
            for (int i = 1; i <= daily_list.Count; i++)
            {
                for (int s = 1; s <= 12; s++)
                {
                    builder.InsertCell();
                    builder.InsertField("MERGEFIELD " + daily_list[i - 1] + "_" + s + " \\* MERGEFORMAT ", "«日常生活表現及具體建議_" + s + "»");
                }

                builder.EndRow();
            }

            builder.EndTable();
            #endregion

            #region 校內外特殊表現功能變數
            builder.Writeln("校內外特殊表現");
            builder.StartTable();
            builder.InsertCell();
            builder.Write("一年級上學期");
            builder.InsertCell();
            builder.Write("一年級下學期");
            builder.InsertCell();
            builder.Write("二年級上學期");
            builder.InsertCell();
            builder.Write("二年級下學期");
            builder.InsertCell();
            builder.Write("三年級上學期");
            builder.InsertCell();
            builder.Write("三年級下學期");
            builder.InsertCell();
            builder.Write("四年級上學期");
            builder.InsertCell();
            builder.Write("四年級下學期");
            builder.InsertCell();
            builder.Write("五年級上學期");
            builder.InsertCell();
            builder.Write("五年級下學期");
            builder.InsertCell();
            builder.Write("六年級上學期");
            builder.InsertCell();
            builder.Write("六年級下學期");

            List<string> specialPerform_list = new List<string>();

            specialPerform_list.Add("校內外特殊表現");



            builder.EndRow();
            for (int i = 1; i <= specialPerform_list.Count; i++)
            {
                for (int s = 1; s <= 12; s++)
                {
                    builder.InsertCell();
                    builder.InsertField("MERGEFIELD " + specialPerform_list[i - 1] + "_" + s + " \\* MERGEFORMAT ", "«校內外特殊表現_" + s + "»");
                }

                builder.EndRow();
            }

            builder.EndTable();
            #endregion

            #endregion


            #region 科目學期學年評量成績
            builder.Writeln("成績");
            builder.StartTable();
            builder.InsertCell();
            builder.Write("科目");
            builder.InsertCell();
            builder.Write("學分");
            builder.InsertCell();
            builder.Write("評量參考成績");
            builder.InsertCell();
            builder.Write("評量成績");
            builder.InsertCell();
            builder.Write("學年成績");
            builder.InsertCell();
            builder.Write("學期成績");
            builder.InsertCell();
            builder.Write("上學期成績");

            builder.InsertCell();
            builder.Write("學期科目原始成績");
            builder.InsertCell();
            builder.Write("學期科目補考成績");
            builder.InsertCell();
            builder.Write("學期科目重修成績");
            builder.InsertCell();
            builder.Write("學期科目手動調整成績");
            builder.InsertCell();
            builder.Write("學期科目學年調整成績");

            builder.InsertCell();
            builder.Write("上學期科目原始成績");
            builder.InsertCell();
            builder.Write("上學期科目補考成績");
            builder.InsertCell();
            builder.Write("上學期科目重修成績");
            builder.InsertCell();
            builder.Write("上學期科目手動調整成績");
            builder.InsertCell();
            builder.Write("上學期科目學年調整成績");
            builder.EndRow();
            for (int i = 1; i <= maxNum; i++)
            {
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 科目" + i + " \\* MERGEFORMAT ", "«N" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學分" + i + " \\* MERGEFORMAT ", "«C" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 前次成績" + i + " \\* MERGEFORMAT ", "«SP" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 成績" + i + " \\* MERGEFORMAT ", "«S" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學年科目成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 上學期科目成績" + i + " \\* MERGEFORMAT ", "«R»");

                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目原始成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目補考成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目重修成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目手動調整成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目學年調整成績" + i + " \\* MERGEFORMAT ", "«R»");

                builder.InsertCell();
                builder.InsertField("MERGEFIELD 上學期科目原始成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 上學期科目補考成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 上學期科目重修成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 上學期科目手動調整成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 上學期科目學年調整成績" + i + " \\* MERGEFORMAT ", "«R»");

                builder.EndRow();
            }

            builder.EndTable();

            builder.Write("固定變數");
            builder.StartTable();
            builder.InsertCell();
            builder.Write("項目");
            builder.InsertCell();
            builder.Write("變數");
            builder.EndRow();
            foreach (string key in new string[]{
                    "學期學業成績"
                    ,"學期體育成績"
                    ,"學期國防通識成績"
                    ,"學期健康與護理成績"
                    ,"學期實習科目成績"
                    ,"學期德行成績"
                    ,"學期學業成績班排名"
                    ,"上學期學業成績"
                    ,"上學期體育成績"
                    ,"上學期國防通識成績"
                    ,"上學期健康與護理成績"
                    ,"上學期實習科目成績"
                    ,"上學期德行成績"
                    ,"學年學業成績"
                    ,"學年體育成績"
                    ,"學年國防通識成績"
                    ,"學年健康與護理成績"
                    ,"學年實習科目成績"
                    ,"學年德行成績"
                    ,"學年學業成績班排名"
                    ,"導師評語"
                    ,"大功統計"
                    ,"小功統計"
                    ,"嘉獎統計"
                    ,"大過統計"
                    ,"小過統計"
                    ,"警告統計"
                    ,"留校察看"
                    ,"班導師"
                })
            {
                builder.InsertCell();
                builder.Write(key);
                builder.InsertCell();
                builder.InsertField("MERGEFIELD " + key + " \\* MERGEFORMAT ", "«" + key + "»");
                builder.EndRow();
            }
            foreach (var key in new string[] { "班", "科", "類別1", "類別2", "校" })
            {
                builder.InsertCell();
                builder.Write("學期學業成績" + key + "排名");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期學業成績" + key + "排名 \\* MERGEFORMAT ", "«學期學業成績" + key + "排名»");
                builder.InsertField("MERGEFIELD 學期學業成績" + key + "排名母數 \\b /  \\* MERGEFORMAT ", "/«學期學業成績" + key + "排名母數»");
                builder.EndRow();
            }
            builder.EndTable();
            #endregion

            #region 學期科目成績排名
            builder.Writeln("學期科目成績排名");
            builder.StartTable();
            builder.InsertCell();
            builder.Write("科目");
            builder.InsertCell();
            builder.Write("學期科目排名成績");
            builder.InsertCell();
            builder.Write("學期科目班排名");
            builder.InsertCell();
            builder.Write("學期科目科排名");
            builder.InsertCell();
            builder.Write("學期科目類別1排名");
            builder.InsertCell();
            builder.Write("學期科目類別2排名");
            builder.InsertCell();
            builder.Write("學期科目全校排名");
            builder.EndRow();
            for (int i = 1; i <= maxNum; i++)
            {
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 科目" + i + " \\* MERGEFORMAT ", "«N" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目排名成績" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目班排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 學期科目班排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目科排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 學期科目科排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目類別1排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 學期科目類別1排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目類別2排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 學期科目類別2排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學期科目全校排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 學期科目全校排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.EndRow();
            }
            builder.EndTable();
            #endregion

            #region 科目成績及排名
            builder.Writeln("科目成績及排名");
            builder.StartTable();
            builder.InsertCell();
            builder.Write("科目");
            builder.InsertCell();
            builder.Write("學分數");
            builder.InsertCell();
            builder.Write("前次成績");
            builder.InsertCell();
            builder.Write("科目成績");
            builder.InsertCell();
            builder.Write("班排名");
            builder.InsertCell();
            builder.Write("科排名");
            builder.InsertCell();
            builder.Write("類別1排名");
            builder.InsertCell();
            builder.Write("類別2排名");
            builder.InsertCell();
            builder.Write("全校排名");
            builder.EndRow();
            for (int i = 1; i <= maxNum; i++)
            {
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 科目" + i + " \\* MERGEFORMAT ", "«N" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 學分" + i + " \\* MERGEFORMAT ", "«C" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 前次成績" + i + " \\* MERGEFORMAT ", "«SP" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 成績" + i + " \\* MERGEFORMAT ", "«S" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 班排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 班排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 科排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 科排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 類別1排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 類別1排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 類別2排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 類別2排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD 全校排名" + i + " \\* MERGEFORMAT ", "«R»");
                builder.InsertField("MERGEFIELD 全校排名母數" + i + " \\b /  \\* MERGEFORMAT ", "/«T»");
                builder.EndRow();
            }

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("項目");
            builder.InsertCell();
            builder.Write("成績");
            builder.InsertCell();
            builder.Write("班排名");
            builder.InsertCell();
            builder.Write("科排名");
            builder.InsertCell();
            builder.Write("類別1排名");
            builder.InsertCell();
            builder.Write("類別2排名");
            builder.InsertCell();
            builder.Write("全校排名");
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("總分");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 總分 \\* MERGEFORMAT ", "«總分»");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 總分班排名 \\* MERGEFORMAT ", "«RS»");
            builder.InsertField("MERGEFIELD 總分班排名母數 \\b /  \\* MERGEFORMAT ", "/«TS»");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 總分科排名 \\* MERGEFORMAT ", "«RS»");
            builder.InsertField("MERGEFIELD 總分科排名母數 \\b /  \\* MERGEFORMAT ", "/«TS»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 總分全校排名 \\* MERGEFORMAT ", "«RS»");
            builder.InsertField("MERGEFIELD 總分全校排名母數 \\b /  \\* MERGEFORMAT ", "/«TS»");
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("平均");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 平均 \\* MERGEFORMAT ", "«平均»");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 平均班排名 \\* MERGEFORMAT ", "«RA»");
            builder.InsertField("MERGEFIELD 平均班排名母數 \\b /  \\* MERGEFORMAT ", "/«TA»");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 平均科排名 \\* MERGEFORMAT ", "«RA»");
            builder.InsertField("MERGEFIELD 平均科排名母數 \\b /  \\* MERGEFORMAT ", "/«TA»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 平均全校排名 \\* MERGEFORMAT ", "«RA»");
            builder.InsertField("MERGEFIELD 平均全校排名母數 \\b /  \\* MERGEFORMAT ", "/«TA»");
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("加權總分");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 加權總分 \\* MERGEFORMAT ", "«加權總»");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 加權總分班排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 加權總分班排名母數 \\b /  \\* MERGEFORMAT ", "/«TP»");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 加權總分科排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 加權總分科排名母數 \\b /  \\* MERGEFORMAT ", "/«TP»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 加權總分全校排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 加權總分全校排名母數 \\b /  \\* MERGEFORMAT ", "/«TP»");
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("加權平均");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 加權平均 \\* MERGEFORMAT ", "«加權均»");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 加權平均班排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 加權平均班排名母數 \\b /  \\* MERGEFORMAT ", "/«TP»");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 加權平均科排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 加權平均科排名母數 \\b /  \\* MERGEFORMAT ", "/«TP»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 加權平均全校排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 加權平均全校排名母數 \\b /  \\* MERGEFORMAT ", "/«TP»");
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("類1總分");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別1總分 \\* MERGEFORMAT ", "«類1總»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別1總分排名 \\* MERGEFORMAT ", "«RS»");
            builder.InsertField("MERGEFIELD 類別1總分排名母數 \\b /  \\* MERGEFORMAT ", "«/TS»");
            builder.InsertCell();
            builder.InsertCell();
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("類1平均");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別1平均 \\* MERGEFORMAT ", "«類1均»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別1平均排名 \\* MERGEFORMAT ", "«RA»");
            builder.InsertField("MERGEFIELD 類別1平均排名母數 \\b /  \\* MERGEFORMAT ", "«/TA»");
            builder.InsertCell();
            builder.InsertCell();
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("類1加權總分");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別1加權總分 \\* MERGEFORMAT ", "«類1加總»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別1加權總分排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 類別1加權總分排名母數 \\b /  \\* MERGEFORMAT ", "«/TP»");
            builder.InsertCell();
            builder.InsertCell();
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("類1加權平均");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別1加權平均 \\* MERGEFORMAT ", "«類1加均»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別1加權平均排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 類別1加權平均排名母數 \\b /  \\* MERGEFORMAT ", "«/TP»");
            builder.InsertCell();
            builder.InsertCell();
            builder.EndRow();


            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("類2總分");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別2總分 \\* MERGEFORMAT ", "«類2總»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別2總分排名 \\* MERGEFORMAT ", "«RS»");
            builder.InsertField("MERGEFIELD 類別2總分排名母數 \\b /  \\* MERGEFORMAT ", "«/TS»");
            builder.InsertCell();
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("類2平均");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別2平均 \\* MERGEFORMAT ", "«類2均»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別2平均排名 \\* MERGEFORMAT ", "«RA»");
            builder.InsertField("MERGEFIELD 類別2平均排名母數 \\b /  \\* MERGEFORMAT ", "«/TA»");
            builder.InsertCell();
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("類2加權總分");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別2加權總分 \\* MERGEFORMAT ", "«類2加總»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別2加權總分排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 類別2加權總分排名母數 \\b /  \\* MERGEFORMAT ", "«/TP»");
            builder.InsertCell();
            builder.EndRow();

            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.Write("類2加權平均");
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別2加權平均 \\* MERGEFORMAT ", "«類2加均»");
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertCell();
            builder.InsertField("MERGEFIELD 類別2加權平均排名 \\* MERGEFORMAT ", "«RP»");
            builder.InsertField("MERGEFIELD 類別2加權平均排名母數 \\b /  \\* MERGEFORMAT ", "«/TP»");
            builder.InsertCell();
            builder.EndRow();

            builder.EndTable();
            #endregion

            #region 各項科目成績分析
            foreach (string key in new string[] { "班", "科", "校", "類1", "類2" })
            {
                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);

                builder.Writeln(key + "成績分析及組距");

                builder.StartTable();
                builder.InsertCell(); builder.Write("科目");
                builder.InsertCell(); builder.Write("高標");
                builder.InsertCell(); builder.Write("均標");
                builder.InsertCell(); builder.Write("低標");
                builder.InsertCell(); builder.Write("標準差");
                builder.InsertCell(); builder.Write("100以上");
                builder.InsertCell(); builder.Write("90以上");
                builder.InsertCell(); builder.Write("80以上");
                builder.InsertCell(); builder.Write("70以上");
                builder.InsertCell(); builder.Write("60以上");
                builder.InsertCell(); builder.Write("50以上");
                builder.InsertCell(); builder.Write("40以上");
                builder.InsertCell(); builder.Write("30以上");
                builder.InsertCell(); builder.Write("20以上");
                builder.InsertCell(); builder.Write("10以上");
                builder.EndRow();
                for (int subjectIndex = 1; subjectIndex <= maxNum; subjectIndex++)
                {
                    builder.InsertCell(); builder.InsertField("MERGEFIELD 科目" + subjectIndex + " \\* MERGEFORMAT ", "«N" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "高標" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "均標" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "低標" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "標準差" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count100Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count90Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count80Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count70Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count60Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count50Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count40Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count30Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count20Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count10Up \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.EndRow();
                }
                builder.EndTable();

                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
                builder.StartTable();
                builder.InsertCell(); builder.Write("科目");
                builder.InsertCell(); builder.Write("高標");
                builder.InsertCell(); builder.Write("均標");
                builder.InsertCell(); builder.Write("低標");
                builder.InsertCell(); builder.Write("標準差");
                builder.InsertCell(); builder.Write("90以上小於100");
                builder.InsertCell(); builder.Write("80以上小於90");
                builder.InsertCell(); builder.Write("70以上小於80");
                builder.InsertCell(); builder.Write("60以上小於70");
                builder.InsertCell(); builder.Write("50以上小於60");
                builder.InsertCell(); builder.Write("40以上小於50");
                builder.InsertCell(); builder.Write("30以上小於40");
                builder.InsertCell(); builder.Write("20以上小於30");
                builder.InsertCell(); builder.Write("10以上小於20");
                builder.EndRow();
                for (int subjectIndex = 1; subjectIndex <= maxNum; subjectIndex++)
                {
                    builder.InsertCell(); builder.InsertField("MERGEFIELD 科目" + subjectIndex + " \\* MERGEFORMAT ", "«N" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "高標" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "均標" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "低標" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "標準差" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count90 \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count80 \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count70 \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count60 \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count50 \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count40 \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count30 \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count20 \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count10 \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.EndRow();
                }
                builder.EndTable();

                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
                builder.StartTable();
                builder.InsertCell(); builder.Write("科目");
                builder.InsertCell(); builder.Write("高標");
                builder.InsertCell(); builder.Write("均標");
                builder.InsertCell(); builder.Write("低標");
                builder.InsertCell(); builder.Write("標準差");
                builder.InsertCell(); builder.Write("小於90");
                builder.InsertCell(); builder.Write("小於80");
                builder.InsertCell(); builder.Write("小於70");
                builder.InsertCell(); builder.Write("小於60");
                builder.InsertCell(); builder.Write("小於50");
                builder.InsertCell(); builder.Write("小於40");
                builder.InsertCell(); builder.Write("小於30");
                builder.InsertCell(); builder.Write("小於20");
                builder.InsertCell(); builder.Write("小於10");
                builder.EndRow();
                for (int subjectIndex = 1; subjectIndex <= maxNum; subjectIndex++)
                {
                    builder.InsertCell(); builder.InsertField("MERGEFIELD 科目" + subjectIndex + " \\* MERGEFORMAT ", "«N" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "高標" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "均標" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "低標" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "標準差" + subjectIndex + " \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count90Down \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count80Down \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count70Down \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count60Down \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count50Down \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count40Down \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count30Down \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count20Down \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距" + subjectIndex + "count10Down \\* MERGEFORMAT ", "«C" + subjectIndex + "»");
                    builder.EndRow();
                }
                builder.EndTable();
            }
            #endregion

            #region 加總成績分析
            builder.Writeln("加總成績分析及組距");

            builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
            builder.StartTable();
            builder.InsertCell(); builder.Write("項目");
            builder.InsertCell(); builder.Write("高標");
            builder.InsertCell(); builder.Write("均標");
            builder.InsertCell(); builder.Write("低標");
            builder.InsertCell(); builder.Write("標準差");
            builder.InsertCell(); builder.Write("100以上");
            builder.InsertCell(); builder.Write("90以上");
            builder.InsertCell(); builder.Write("80以上");
            builder.InsertCell(); builder.Write("70以上");
            builder.InsertCell(); builder.Write("60以上");
            builder.InsertCell(); builder.Write("50以上");
            builder.InsertCell(); builder.Write("40以上");
            builder.InsertCell(); builder.Write("30以上");
            builder.InsertCell(); builder.Write("20以上");
            builder.InsertCell(); builder.Write("10以上");
            builder.EndRow();
            foreach (string key in new string[] { "總分班", "總分科", "總分校", "平均班", "平均科", "平均校", "加權總分班", "加權總分科", "加權總分校", "加權平均班", "加權平均科", "加權平均校", "類1總分", "類1平均", "類1加權總分", "類1加權平均", "類2總分", "類2平均", "類2加權總分", "類2加權平均" })
            {
                builder.InsertCell(); builder.Write(key);
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "高標 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "均標 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "低標 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "標準差 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count100Up \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count90Up \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count80Up \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count70Up \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count60Up \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count50Up \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count40Up \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count30Up \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count20Up \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count10Up \\* MERGEFORMAT ", "«C»");
                builder.EndRow();
            }
            builder.EndTable();
            builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
            builder.StartTable();
            builder.InsertCell(); builder.Write("項目");
            builder.InsertCell(); builder.Write("高標");
            builder.InsertCell(); builder.Write("均標");
            builder.InsertCell(); builder.Write("低標");
            builder.InsertCell(); builder.Write("標準差");
            builder.InsertCell(); builder.Write("90以上小於100");
            builder.InsertCell(); builder.Write("80以上小於90");
            builder.InsertCell(); builder.Write("70以上小於80");
            builder.InsertCell(); builder.Write("60以上小於70");
            builder.InsertCell(); builder.Write("50以上小於60");
            builder.InsertCell(); builder.Write("40以上小於50");
            builder.InsertCell(); builder.Write("30以上小於40");
            builder.InsertCell(); builder.Write("20以上小於30");
            builder.InsertCell(); builder.Write("10以上小於20");
            builder.EndRow();
            foreach (string key in new string[] { "總分班", "總分科", "總分校", "平均班", "平均科", "平均校", "加權總分班", "加權總分科", "加權總分校", "加權平均班", "加權平均科", "加權平均校", "類1總分", "類1平均", "類1加權總分", "類1加權平均", "類2總分", "類2平均", "類2加權總分", "類2加權平均" })
            {
                builder.InsertCell(); builder.Write(key);
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "高標 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "均標 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "低標 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "標準差 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count90 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count80 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count70 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count60 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count50 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count40 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count30 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count20 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count10 \\* MERGEFORMAT ", "«C»");
                builder.EndRow();
            }
            builder.EndTable();
            builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
            builder.StartTable();
            builder.InsertCell(); builder.Write("項目");
            builder.InsertCell(); builder.Write("高標");
            builder.InsertCell(); builder.Write("均標");
            builder.InsertCell(); builder.Write("低標");
            builder.InsertCell(); builder.Write("標準差");
            builder.InsertCell(); builder.Write("小於90");
            builder.InsertCell(); builder.Write("小於80");
            builder.InsertCell(); builder.Write("小於70");
            builder.InsertCell(); builder.Write("小於60");
            builder.InsertCell(); builder.Write("小於50");
            builder.InsertCell(); builder.Write("小於40");
            builder.InsertCell(); builder.Write("小於30");
            builder.InsertCell(); builder.Write("小於20");
            builder.InsertCell(); builder.Write("小於10");
            builder.EndRow();
            foreach (string key in new string[] { "總分班", "總分科", "總分校", "平均班", "平均科", "平均校", "加權總分班", "加權總分科", "加權總分校", "加權平均班", "加權平均科", "加權平均校", "類1總分", "類1平均", "類1加權總分", "類1加權平均", "類2總分", "類2平均", "類2加權總分", "類2加權平均" })
            {
                builder.InsertCell(); builder.Write(key);
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "高標 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "均標 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "低標 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "標準差 \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count90Down \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count80Down \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count70Down \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count60Down \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count50Down \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count40Down \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count30Down \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count20Down \\* MERGEFORMAT ", "«C»");
                builder.InsertCell(); builder.InsertField("MERGEFIELD " + key + "組距count10Down \\* MERGEFORMAT ", "«C»");
                builder.EndRow();
            }
            builder.EndTable();
            #endregion
            #endregion

            #region 儲存檔案
            string inputReportName = "個人評量成績單合併欄位總表";
            string reportName = inputReportName;

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".doc");

            if (System.IO.File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!System.IO.File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                doc.Save(path, Aspose.Words.SaveFormat.Doc);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".doc";
                sd.Filter = "Excel檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        doc.Save(path, Aspose.Words.SaveFormat.Doc);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            #endregion
        }
    }

    internal enum ModuleMode
    {
        /// <summary>
        /// 新竹
        /// </summary>
        HsinChu,
        /// <summary>
        /// 高雄
        /// </summary>
        KaoHsiung
    }



}
