﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Aspose.Words;
using FISCA.Presentation;
using FISCA.Presentation.Controls;
using JHEvaluation.ScoreCalculation;
using Campus.Rating;
using JHSchool.Data;
using Campus.Report;
using JHSchool.Behavior.BusinessLogic;
using System.IO;
using System.Data;

namespace JHEvaluation.StudentScoreSummaryReport
{
    public partial class PrintForm_StudentReport : BaseForm, IStatusReporter
    {
        internal const string ConfigName = "StudentReport";

        private List<string> StudentIDs { get; set; }

        private ReportPreference Preference { get; set; }

        private BackgroundWorker MasterWorker = new BackgroundWorker();

        private BackgroundWorker ConvertToPDF_Worker = new BackgroundWorker();

        //private List<ReportStudent> PrintStudents = new List<ReportStudent>();

        private List<K12.Data.StudentRecord> PrintStudents = new List<K12.Data.StudentRecord>();

        // 學生基本資料 [studentID,Data]
        Dictionary<string, K12.Data.StudentRecord> sr_dict = new Dictionary<string, K12.Data.StudentRecord>();

        // 學期歷程 [studentID,Data]
        Dictionary<string, K12.Data.SemesterHistoryRecord> shr_dict = new Dictionary<string, K12.Data.SemesterHistoryRecord>();


        // 缺曠 [studentID,List<Data>]
        Dictionary<string, List<K12.Data.AttendanceRecord>> ar_dict = new Dictionary<string, List<K12.Data.AttendanceRecord>>();
        

        private string fbdPath = "";

        private DoWorkEventArgs e_For_ConvertToPDF_Worker;

        public PrintForm_StudentReport(List<string> studentIds)
        {
            InitializeComponent();

            StudentIDs = studentIds;
            Preference = new ReportPreference(ConfigName, Prc.康橋國小學籍表_樣板_);
            MasterWorker.DoWork += new DoWorkEventHandler(MasterWorker_DoWork);
            MasterWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MasterWorker_RunWorkerCompleted);

            ConvertToPDF_Worker.DoWork += new DoWorkEventHandler(ConvertToPDF_Worker_DoWork);
            ConvertToPDF_Worker.WorkerReportsProgress = true;
            ConvertToPDF_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ConvertToPDF_Worker_RunWorkerCompleted);

            ConvertToPDF_Worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage(e.UserState.ToString(), e.ProgressPercentage);
            };
                        
            // 是否列印PDF
            rtnPDF.Checked = Preference.ConvertToPDF;

            // 是否要單檔列印
            OneFileSave.Checked = Preference.OneFileSave;
                    
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            Preference.ConvertToPDF = rtnPDF.Checked;

            Preference.OneFileSave = OneFileSave.Checked;
            
            Preference.Save(); //儲存設定值。

            //關閉畫面控制項
            Util.DisableControls(this);
            MasterWorker.RunWorkerAsync();
        }

        private void MasterWorker_DoWork(object sender, DoWorkEventArgs e)
        {                                                        
            if (StudentIDs.Count <= 0)
            {
                Feedback("", -1);  //把 Status bar Reset...
                throw new ArgumentException("沒有任何學生資料可列印。");
            }

            //抓取學生資料 

            //學生基本資料
            List<K12.Data.StudentRecord> sr_list = K12.Data.Student.SelectByIDs(StudentIDs);

            //學期成績(包含領域、科目)
            List<K12.Data.SemesterScoreRecord> ssr_list = K12.Data.SemesterScore.SelectByStudentIDs(StudentIDs);

            //缺曠
            List<K12.Data.AttendanceRecord> ar_list = K12.Data.Attendance.SelectByStudentIDs(StudentIDs);

            //不太確定這是哪一種異動
            //List<K12.Data.UpdateRecordBatchRecord> urbr_list = K12.Data.UpdateRecordBatch.SelectByIDs(StudentIDs);

            //學生異動
            List<K12.Data.UpdateRecordRecord> urr_list = K12.Data.UpdateRecord.SelectByIDs(StudentIDs);

            //畢業分數
            List<K12.Data.GradScoreRecord> gsr_list = K12.Data.GradScore.SelectByIDs<K12.Data.GradScoreRecord>(StudentIDs);

            //學期歷程
            List<K12.Data.SemesterHistoryRecord> shr_list = K12.Data.SemesterHistory.SelectByStudentIDs(StudentIDs);

            //日常生活表現、校內外特殊表現
            List<K12.Data.MoralScoreRecord> msr_list = K12.Data.MoralScore.SelectByStudentIDs(StudentIDs);




            //整理學生基本資料
            foreach (K12.Data.StudentRecord sr in sr_list)
            {
                if (!sr_dict.ContainsKey(sr.ID))
                {
                    sr_dict.Add(sr.ID, sr);
                }
            }


            //整理學期歷程
            foreach (K12.Data.SemesterHistoryRecord shr in shr_list)
            {
                if (!shr_dict.ContainsKey(shr.RefStudentID))
                {
                    shr_dict.Add(shr.RefStudentID, shr);
                }
            }

            

            //整理出缺勤紀錄
            foreach (K12.Data.AttendanceRecord ar in ar_list)
            {
                if (!ar_dict.ContainsKey(ar.RefStudentID))
                {
                    ar_dict.Add(ar.RefStudentID, new List<K12.Data.AttendanceRecord>());
                    ar_dict[ar.RefStudentID].Add(ar);
                }
                else
                {
                    ar_dict[ar.RefStudentID].Add(ar);
                }
            }

            #region 建立合併欄位總表
            //建立合併欄位總表
            DataTable table = new DataTable();

            #region 基本資料
            //基本資料
            table.Columns.Add("學生姓名");
            table.Columns.Add("學生性別");
            table.Columns.Add("出生日期");
            table.Columns.Add("入學年月");
            table.Columns.Add("學生身分證字號");
            table.Columns.Add("學號");
            #endregion

            #region 異動紀錄
            //異動紀錄
            table.Columns.Add("異動紀錄1_日期");
            table.Columns.Add("異動紀錄1_校名");
            table.Columns.Add("異動紀錄1_學號");
            table.Columns.Add("異動紀錄2_日期");
            table.Columns.Add("異動紀錄2_校名");
            table.Columns.Add("異動紀錄2_學號");
            table.Columns.Add("異動紀錄3_日期");
            table.Columns.Add("異動紀錄3_校名");
            table.Columns.Add("異動紀錄3_學號");
            table.Columns.Add("異動紀錄4_日期");
            table.Columns.Add("異動紀錄4_校名");
            table.Columns.Add("異動紀錄4_學號");
            #endregion

            #region 班級座號資料
            //班級座號資料
            table.Columns.Add("年級1_班級");
            table.Columns.Add("年級1_座號");
            table.Columns.Add("年級2_班級");
            table.Columns.Add("年級2_座號");
            table.Columns.Add("年級3_班級");
            table.Columns.Add("年級3_座號");
            table.Columns.Add("年級4_班級");
            table.Columns.Add("年級4_座號");
            table.Columns.Add("年級5_班級");
            table.Columns.Add("年級5_座號");
            table.Columns.Add("年級6_班級");
            table.Columns.Add("年級6_座號");
            #endregion

            #region 學年度
            //學年度
            table.Columns.Add("學年度1");
            table.Columns.Add("學年度2");
            table.Columns.Add("學年度3");
            table.Columns.Add("學年度4");
            table.Columns.Add("學年度5");
            table.Columns.Add("學年度6");
            #endregion

            #region 領域成績
            //領域成績

            table.Columns.Add("領域_語文_成績_1");
            table.Columns.Add("領域_語文_成績_2");
            table.Columns.Add("領域_語文_成績_3");
            table.Columns.Add("領域_語文_成績_4");
            table.Columns.Add("領域_語文_成績_5");
            table.Columns.Add("領域_語文_成績_6");
            table.Columns.Add("領域_語文_成績_7");
            table.Columns.Add("領域_語文_成績_8");
            table.Columns.Add("領域_語文_成績_9");
            table.Columns.Add("領域_語文_成績_10");
            table.Columns.Add("領域_語文_成績_11");
            table.Columns.Add("領域_語文_成績_12");


            table.Columns.Add("領域_語文_等第_1");
            table.Columns.Add("領域_語文_等第_2");
            table.Columns.Add("領域_語文_等第_3");
            table.Columns.Add("領域_語文_等第_4");
            table.Columns.Add("領域_語文_等第_5");
            table.Columns.Add("領域_語文_等第_6");
            table.Columns.Add("領域_語文_等第_7");
            table.Columns.Add("領域_語文_等第_8");
            table.Columns.Add("領域_語文_等第_9");
            table.Columns.Add("領域_語文_等第_10");
            table.Columns.Add("領域_語文_等第_11");
            table.Columns.Add("領域_語文_等第_12");


            table.Columns.Add("領域_數學_成績_1");
            table.Columns.Add("領域_數學_成績_2");
            table.Columns.Add("領域_數學_成績_3");
            table.Columns.Add("領域_數學_成績_4");
            table.Columns.Add("領域_數學_成績_5");
            table.Columns.Add("領域_數學_成績_6");
            table.Columns.Add("領域_數學_成績_7");
            table.Columns.Add("領域_數學_成績_8");
            table.Columns.Add("領域_數學_成績_9");
            table.Columns.Add("領域_數學_成績_10");
            table.Columns.Add("領域_數學_成績_11");
            table.Columns.Add("領域_數學_成績_12");


            table.Columns.Add("領域_數學_等第_1");
            table.Columns.Add("領域_數學_等第_2");
            table.Columns.Add("領域_數學_等第_3");
            table.Columns.Add("領域_數學_等第_4");
            table.Columns.Add("領域_數學_等第_5");
            table.Columns.Add("領域_數學_等第_6");
            table.Columns.Add("領域_數學_等第_7");
            table.Columns.Add("領域_數學_等第_8");
            table.Columns.Add("領域_數學_等第_9");
            table.Columns.Add("領域_數學_等第_10");
            table.Columns.Add("領域_數學_等第_11");
            table.Columns.Add("領域_數學_等第_12");


            table.Columns.Add("領域_生活課程_成績_1");
            table.Columns.Add("領域_生活課程_成績_2");
            table.Columns.Add("領域_生活課程_成績_3");
            table.Columns.Add("領域_生活課程_成績_4");
            table.Columns.Add("領域_生活課程_成績_5");
            table.Columns.Add("領域_生活課程_成績_6");
            table.Columns.Add("領域_生活課程_成績_7");
            table.Columns.Add("領域_生活課程_成績_8");
            table.Columns.Add("領域_生活課程_成績_9");
            table.Columns.Add("領域_生活課程_成績_10");
            table.Columns.Add("領域_生活課程_成績_11");
            table.Columns.Add("領域_生活課程_成績_12");


            table.Columns.Add("領域_生活課程_等第_1");
            table.Columns.Add("領域_生活課程_等第_2");
            table.Columns.Add("領域_生活課程_等第_3");
            table.Columns.Add("領域_生活課程_等第_4");
            table.Columns.Add("領域_生活課程_等第_5");
            table.Columns.Add("領域_生活課程_等第_6");
            table.Columns.Add("領域_生活課程_等第_7");
            table.Columns.Add("領域_生活課程_等第_8");
            table.Columns.Add("領域_生活課程_等第_9");
            table.Columns.Add("領域_生活課程_等第_10");
            table.Columns.Add("領域_生活課程_等第_11");
            table.Columns.Add("領域_生活課程_等第_12");



            table.Columns.Add("領域_自然與生活科技_成績_1");
            table.Columns.Add("領域_自然與生活科技_成績_2");
            table.Columns.Add("領域_自然與生活科技_成績_3");
            table.Columns.Add("領域_自然與生活科技_成績_4");
            table.Columns.Add("領域_自然與生活科技_成績_5");
            table.Columns.Add("領域_自然與生活科技_成績_6");
            table.Columns.Add("領域_自然與生活科技_成績_7");
            table.Columns.Add("領域_自然與生活科技_成績_8");
            table.Columns.Add("領域_自然與生活科技_成績_9");
            table.Columns.Add("領域_自然與生活科技_成績_10");
            table.Columns.Add("領域_自然與生活科技_成績_11");
            table.Columns.Add("領域_自然與生活科技_成績_12");


            table.Columns.Add("領域_自然與生活科技_等第_1");
            table.Columns.Add("領域_自然與生活科技_等第_2");
            table.Columns.Add("領域_自然與生活科技_等第_3");
            table.Columns.Add("領域_自然與生活科技_等第_4");
            table.Columns.Add("領域_自然與生活科技_等第_5");
            table.Columns.Add("領域_自然與生活科技_等第_6");
            table.Columns.Add("領域_自然與生活科技_等第_7");
            table.Columns.Add("領域_自然與生活科技_等第_8");
            table.Columns.Add("領域_自然與生活科技_等第_9");
            table.Columns.Add("領域_自然與生活科技_等第_10");
            table.Columns.Add("領域_自然與生活科技_等第_11");
            table.Columns.Add("領域_自然與生活科技_等第_12");


            table.Columns.Add("領域_藝術與人文_成績_1");
            table.Columns.Add("領域_藝術與人文_成績_2");
            table.Columns.Add("領域_藝術與人文_成績_3");
            table.Columns.Add("領域_藝術與人文_成績_4");
            table.Columns.Add("領域_藝術與人文_成績_5");
            table.Columns.Add("領域_藝術與人文_成績_6");
            table.Columns.Add("領域_藝術與人文_成績_7");
            table.Columns.Add("領域_藝術與人文_成績_8");
            table.Columns.Add("領域_藝術與人文_成績_9");
            table.Columns.Add("領域_藝術與人文_成績_10");
            table.Columns.Add("領域_藝術與人文_成績_11");
            table.Columns.Add("領域_藝術與人文_成績_12");


            table.Columns.Add("領域_藝術與人文_等第_1");
            table.Columns.Add("領域_藝術與人文_等第_2");
            table.Columns.Add("領域_藝術與人文_等第_3");
            table.Columns.Add("領域_藝術與人文_等第_4");
            table.Columns.Add("領域_藝術與人文_等第_5");
            table.Columns.Add("領域_藝術與人文_等第_6");
            table.Columns.Add("領域_藝術與人文_等第_7");
            table.Columns.Add("領域_藝術與人文_等第_8");
            table.Columns.Add("領域_藝術與人文_等第_9");
            table.Columns.Add("領域_藝術與人文_等第_10");
            table.Columns.Add("領域_藝術與人文_等第_11");
            table.Columns.Add("領域_藝術與人文_等第_12");



            table.Columns.Add("領域_社會_成績_1");
            table.Columns.Add("領域_社會_成績_2");
            table.Columns.Add("領域_社會_成績_3");
            table.Columns.Add("領域_社會_成績_4");
            table.Columns.Add("領域_社會_成績_5");
            table.Columns.Add("領域_社會_成績_6");
            table.Columns.Add("領域_社會_成績_7");
            table.Columns.Add("領域_社會_成績_8");
            table.Columns.Add("領域_社會_成績_9");
            table.Columns.Add("領域_社會_成績_10");
            table.Columns.Add("領域_社會_成績_11");
            table.Columns.Add("領域_社會_成績_12");


            table.Columns.Add("領域_社會_等第_1");
            table.Columns.Add("領域_社會_等第_2");
            table.Columns.Add("領域_社會_等第_3");
            table.Columns.Add("領域_社會_等第_4");
            table.Columns.Add("領域_社會_等第_5");
            table.Columns.Add("領域_社會_等第_6");
            table.Columns.Add("領域_社會_等第_7");
            table.Columns.Add("領域_社會_等第_8");
            table.Columns.Add("領域_社會_等第_9");
            table.Columns.Add("領域_社會_等第_10");
            table.Columns.Add("領域_社會_等第_11");
            table.Columns.Add("領域_社會_等第_12");


            table.Columns.Add("領域_健康與體育_成績_1");
            table.Columns.Add("領域_健康與體育_成績_2");
            table.Columns.Add("領域_健康與體育_成績_3");
            table.Columns.Add("領域_健康與體育_成績_4");
            table.Columns.Add("領域_健康與體育_成績_5");
            table.Columns.Add("領域_健康與體育_成績_6");
            table.Columns.Add("領域_健康與體育_成績_7");
            table.Columns.Add("領域_健康與體育_成績_8");
            table.Columns.Add("領域_健康與體育_成績_9");
            table.Columns.Add("領域_健康與體育_成績_10");
            table.Columns.Add("領域_健康與體育_成績_11");
            table.Columns.Add("領域_健康與體育_成績_12");


            table.Columns.Add("領域_健康與體育_等第_1");
            table.Columns.Add("領域_健康與體育_等第_2");
            table.Columns.Add("領域_健康與體育_等第_3");
            table.Columns.Add("領域_健康與體育_等第_4");
            table.Columns.Add("領域_健康與體育_等第_5");
            table.Columns.Add("領域_健康與體育_等第_6");
            table.Columns.Add("領域_健康與體育_等第_7");
            table.Columns.Add("領域_健康與體育_等第_8");
            table.Columns.Add("領域_健康與體育_等第_9");
            table.Columns.Add("領域_健康與體育_等第_10");
            table.Columns.Add("領域_健康與體育_等第_11");
            table.Columns.Add("領域_健康與體育_等第_12");


            table.Columns.Add("領域_綜合活動_成績_1");
            table.Columns.Add("領域_綜合活動_成績_2");
            table.Columns.Add("領域_綜合活動_成績_3");
            table.Columns.Add("領域_綜合活動_成績_4");
            table.Columns.Add("領域_綜合活動_成績_5");
            table.Columns.Add("領域_綜合活動_成績_6");
            table.Columns.Add("領域_綜合活動_成績_7");
            table.Columns.Add("領域_綜合活動_成績_8");
            table.Columns.Add("領域_綜合活動_成績_9");
            table.Columns.Add("領域_綜合活動_成績_10");
            table.Columns.Add("領域_綜合活動_成績_11");
            table.Columns.Add("領域_綜合活動_成績_12");


            table.Columns.Add("領域_綜合活動_等第_1");
            table.Columns.Add("領域_綜合活動_等第_2");
            table.Columns.Add("領域_綜合活動_等第_3");
            table.Columns.Add("領域_綜合活動_等第_4");
            table.Columns.Add("領域_綜合活動_等第_5");
            table.Columns.Add("領域_綜合活動_等第_6");
            table.Columns.Add("領域_綜合活動_等第_7");
            table.Columns.Add("領域_綜合活動_等第_8");
            table.Columns.Add("領域_綜合活動_等第_9");
            table.Columns.Add("領域_綜合活動_等第_10");
            table.Columns.Add("領域_綜合活動_等第_11");
            table.Columns.Add("領域_綜合活動_等第_12");


            table.Columns.Add("領域_學習領域總成績_成績_1");
            table.Columns.Add("領域_學習領域總成績_成績_2");
            table.Columns.Add("領域_學習領域總成績_成績_3");
            table.Columns.Add("領域_學習領域總成績_成績_4");
            table.Columns.Add("領域_學習領域總成績_成績_5");
            table.Columns.Add("領域_學習領域總成績_成績_6");
            table.Columns.Add("領域_學習領域總成績_成績_7");
            table.Columns.Add("領域_學習領域總成績_成績_8");
            table.Columns.Add("領域_學習領域總成績_成績_9");
            table.Columns.Add("領域_學習領域總成績_成績_10");
            table.Columns.Add("領域_學習領域總成績_成績_11");
            table.Columns.Add("領域_學習領域總成績_成績_12");


            table.Columns.Add("領域_學習領域總成績_等第_1");
            table.Columns.Add("領域_學習領域總成績_等第_2");
            table.Columns.Add("領域_學習領域總成績_等第_3");
            table.Columns.Add("領域_學習領域總成績_等第_4");
            table.Columns.Add("領域_學習領域總成績_等第_5");
            table.Columns.Add("領域_學習領域總成績_等第_6");
            table.Columns.Add("領域_學習領域總成績_等第_7");
            table.Columns.Add("領域_學習領域總成績_等第_8");
            table.Columns.Add("領域_學習領域總成績_等第_9");
            table.Columns.Add("領域_學習領域總成績_等第_10");
            table.Columns.Add("領域_學習領域總成績_等第_11");
            table.Columns.Add("領域_學習領域總成績_等第_12");


            #endregion

            #region 科目成績
            //科目成績
            table.Columns.Add("科目_國語_成績_1");
            table.Columns.Add("科目_國語_成績_2");
            table.Columns.Add("科目_國語_成績_3");
            table.Columns.Add("科目_國語_成績_4");
            table.Columns.Add("科目_國語_成績_5");
            table.Columns.Add("科目_國語_成績_6");
            table.Columns.Add("科目_國語_成績_7");
            table.Columns.Add("科目_國語_成績_8");
            table.Columns.Add("科目_國語_成績_9");
            table.Columns.Add("科目_國語_成績_10");
            table.Columns.Add("科目_國語_成績_11");
            table.Columns.Add("科目_國語_成績_12");

            table.Columns.Add("科目_國語_等第_1");
            table.Columns.Add("科目_國語_等第_2");
            table.Columns.Add("科目_國語_等第_3");
            table.Columns.Add("科目_國語_等第_4");
            table.Columns.Add("科目_國語_等第_5");
            table.Columns.Add("科目_國語_等第_6");
            table.Columns.Add("科目_國語_等第_7");
            table.Columns.Add("科目_國語_等第_8");
            table.Columns.Add("科目_國語_等第_9");
            table.Columns.Add("科目_國語_等第_10");
            table.Columns.Add("科目_國語_等第_11");
            table.Columns.Add("科目_國語_等第_12");

            table.Columns.Add("科目_英語_成績_1");
            table.Columns.Add("科目_英語_成績_2");
            table.Columns.Add("科目_英語_成績_3");
            table.Columns.Add("科目_英語_成績_4");
            table.Columns.Add("科目_英語_成績_5");
            table.Columns.Add("科目_英語_成績_6");
            table.Columns.Add("科目_英語_成績_7");
            table.Columns.Add("科目_英語_成績_8");
            table.Columns.Add("科目_英語_成績_9");
            table.Columns.Add("科目_英語_成績_10");
            table.Columns.Add("科目_英語_成績_11");
            table.Columns.Add("科目_英語_成績_12");

            table.Columns.Add("科目_英語_等第_1");
            table.Columns.Add("科目_英語_等第_2");
            table.Columns.Add("科目_英語_等第_3");
            table.Columns.Add("科目_英語_等第_4");
            table.Columns.Add("科目_英語_等第_5");
            table.Columns.Add("科目_英語_等第_6");
            table.Columns.Add("科目_英語_等第_7");
            table.Columns.Add("科目_英語_等第_8");
            table.Columns.Add("科目_英語_等第_9");
            table.Columns.Add("科目_英語_等第_10");
            table.Columns.Add("科目_英語_等第_11");
            table.Columns.Add("科目_英語_等第_12");


            #endregion

            #region 出缺勤紀錄
            //出缺勤紀錄
            table.Columns.Add("應出席日數_1");
            table.Columns.Add("應出席日數_2");
            table.Columns.Add("應出席日數_3");
            table.Columns.Add("應出席日數_4");
            table.Columns.Add("應出席日數_5");
            table.Columns.Add("應出席日數_6");
            table.Columns.Add("應出席日數_7");
            table.Columns.Add("應出席日數_8");
            table.Columns.Add("應出席日數_9");
            table.Columns.Add("應出席日數_10");
            table.Columns.Add("應出席日數_11");
            table.Columns.Add("應出席日數_12");


            table.Columns.Add("事假日數_1");
            table.Columns.Add("事假日數_2");
            table.Columns.Add("事假日數_3");
            table.Columns.Add("事假日數_4");
            table.Columns.Add("事假日數_5");
            table.Columns.Add("事假日數_6");
            table.Columns.Add("事假日數_7");
            table.Columns.Add("事假日數_8");
            table.Columns.Add("事假日數_9");
            table.Columns.Add("事假日數_10");
            table.Columns.Add("事假日數_11");
            table.Columns.Add("事假日數_12");


            table.Columns.Add("病假日數_1");
            table.Columns.Add("病假日數_2");
            table.Columns.Add("病假日數_3");
            table.Columns.Add("病假日數_4");
            table.Columns.Add("病假日數_5");
            table.Columns.Add("病假日數_6");
            table.Columns.Add("病假日數_7");
            table.Columns.Add("病假日數_8");
            table.Columns.Add("病假日數_9");
            table.Columns.Add("病假日數_10");
            table.Columns.Add("病假日數_11");
            table.Columns.Add("病假日數_12");

            table.Columns.Add("公假日數_1");
            table.Columns.Add("公假日數_2");
            table.Columns.Add("公假日數_3");
            table.Columns.Add("公假日數_4");
            table.Columns.Add("公假日數_5");
            table.Columns.Add("公假日數_6");
            table.Columns.Add("公假日數_7");
            table.Columns.Add("公假日數_8");
            table.Columns.Add("公假日數_9");
            table.Columns.Add("公假日數_10");
            table.Columns.Add("公假日數_11");
            table.Columns.Add("公假日數_12");

            table.Columns.Add("喪假日數_1");
            table.Columns.Add("喪假日數_2");
            table.Columns.Add("喪假日數_3");
            table.Columns.Add("喪假日數_4");
            table.Columns.Add("喪假日數_5");
            table.Columns.Add("喪假日數_6");
            table.Columns.Add("喪假日數_7");
            table.Columns.Add("喪假日數_8");
            table.Columns.Add("喪假日數_9");
            table.Columns.Add("喪假日數_10");
            table.Columns.Add("喪假日數_11");
            table.Columns.Add("喪假日數_12");

            table.Columns.Add("曠課日數_1");
            table.Columns.Add("曠課日數_2");
            table.Columns.Add("曠課日數_3");
            table.Columns.Add("曠課日數_4");
            table.Columns.Add("曠課日數_5");
            table.Columns.Add("曠課日數_6");
            table.Columns.Add("曠課日數_7");
            table.Columns.Add("曠課日數_8");
            table.Columns.Add("曠課日數_9");
            table.Columns.Add("曠課日數_10");
            table.Columns.Add("曠課日數_11");
            table.Columns.Add("曠課日數_12");

            table.Columns.Add("缺席總日數_1");
            table.Columns.Add("缺席總日數_2");
            table.Columns.Add("缺席總日數_3");
            table.Columns.Add("缺席總日數_4");
            table.Columns.Add("缺席總日數_5");
            table.Columns.Add("缺席總日數_6");
            table.Columns.Add("缺席總日數_7");
            table.Columns.Add("缺席總日數_8");
            table.Columns.Add("缺席總日數_9");
            table.Columns.Add("缺席總日數_10");
            table.Columns.Add("缺席總日數_11");
            table.Columns.Add("缺席總日數_12");


            #endregion

            #region 畢業總成績
            //畢業總成績
            table.Columns.Add("畢業總成績_平均");
            table.Columns.Add("畢業總成績_等第");
            table.Columns.Add("准予畢業");
            table.Columns.Add("發給修業證書");
            #endregion

            #region 日常生活表現及具體建議
            //日常生活表現及具體建議
            table.Columns.Add("日常生活表現及具體建議功能_1");
            table.Columns.Add("日常生活表現及具體建議功能_2");
            table.Columns.Add("日常生活表現及具體建議功能_3");
            table.Columns.Add("日常生活表現及具體建議功能_4");
            table.Columns.Add("日常生活表現及具體建議功能_5");
            table.Columns.Add("日常生活表現及具體建議功能_6");
            table.Columns.Add("日常生活表現及具體建議功能_7");
            table.Columns.Add("日常生活表現及具體建議功能_8");
            table.Columns.Add("日常生活表現及具體建議功能_9");
            table.Columns.Add("日常生活表現及具體建議功能_10");
            table.Columns.Add("日常生活表現及具體建議功能_11");
            table.Columns.Add("日常生活表現及具體建議功能_12");


            #endregion

            #region 校內外特殊表現
            //校內外特殊表現
            table.Columns.Add("校內外特殊表現_1");
            table.Columns.Add("校內外特殊表現_2");
            table.Columns.Add("校內外特殊表現_3");
            table.Columns.Add("校內外特殊表現_4");
            table.Columns.Add("校內外特殊表現_5");
            table.Columns.Add("校內外特殊表現_6");
            table.Columns.Add("校內外特殊表現_7");
            table.Columns.Add("校內外特殊表現_8");
            table.Columns.Add("校內外特殊表現_9");
            table.Columns.Add("校內外特殊表現_10");
            table.Columns.Add("校內外特殊表現_11");
            table.Columns.Add("校內外特殊表現_12");


            #endregion 

            #endregion

            Aspose.Words.Document document = new Aspose.Words.Document();

            e_For_ConvertToPDF_Worker = e;


            //整理所有的假別
            List<string> absenceType_list = new List<string>();

            absenceType_list.Add("事假");
            absenceType_list.Add("病假");
            absenceType_list.Add("公假");
            absenceType_list.Add("喪假");
            absenceType_list.Add("曠課");
            absenceType_list.Add("缺席總");


            Dictionary<string, decimal> arStatistic_dict = new Dictionary<string, decimal>();

            Dictionary<string, decimal> arStatistic_dict_days = new Dictionary<string, decimal>();

            foreach (string stuID in StudentIDs)
            {

                //把每一筆資料的字典都清乾淨，避免資料汙染
                arStatistic_dict.Clear();
                arStatistic_dict_days.Clear();

                foreach (string ab in absenceType_list)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        arStatistic_dict.Add(ab + "日數_" + i, 0);
                    }
                }

                int schoolyear_grade1 = 0;
                int schoolyear_grade2 = 0;
                int schoolyear_grade3 = 0;
                int schoolyear_grade4 = 0;
                int schoolyear_grade5 = 0;
                int schoolyear_grade6 = 0;

                


                DataRow row = table.NewRow();

                //學生基本資料
                if (sr_dict.ContainsKey(stuID))
                {
                    row["學生姓名"] = sr_dict[stuID].Name;
                    row["學生性別"] = sr_dict[stuID].Gender;
                    row["出生日期"] = sr_dict[stuID].Birthday;
                    row["入學年月"] = "";
                    row["學生身分證字號"] = sr_dict[stuID].IDNumber;
                    row["學號"] = sr_dict[stuID].StudentNumber;

                    PrintStudents.Add(sr_dict[stuID]);
                }

                //學期歷程
                if (shr_dict.ContainsKey(stuID))
                {                    
                    foreach (var item in shr_dict[stuID].SemesterHistoryItems)
                    {
                        if (item.GradeYear == 1)
                        {
                            row["學年度1"] = item.SchoolYear;
                            row["年級1_班級"] = item.ClassName;
                            row["年級1_座號"] = item.SeatNo;

                            //為學生的年級與學年配對
                            schoolyear_grade1 = item.SchoolYear;

                            if (item.Semester == 1)
                            {
                                row["應出席日數_1"] = item.SchoolDayCount; 
                            }
                            else
                            {
                                row["應出席日數_2"] = item.SchoolDayCount;

                            }

                        }
                        if (item.GradeYear == 2)
                        {
                            row["學年度2"] = item.SchoolYear;
                            row["年級2_班級"] = item.ClassName;
                            row["年級2_座號"] = item.SeatNo;

                            //為學生的年級與學年配對
                            schoolyear_grade2 = item.SchoolYear;

                            if (item.Semester == 1)
                            {
                                row["應出席日數_3"] = item.SchoolDayCount;
                            }
                            else
                            {
                                row["應出席日數_4"] = item.SchoolDayCount;

                            }
                        }
                        if (item.GradeYear == 3)
                        {
                            row["學年度3"] = item.SchoolYear;
                            row["年級3_班級"] = item.ClassName;
                            row["年級3_座號"] = item.SeatNo;

                            //為學生的年級與學年配對
                            schoolyear_grade3 = item.SchoolYear;

                            if (item.Semester == 1)
                            {
                                row["應出席日數_5"] = item.SchoolDayCount;
                            }
                            else
                            {
                                row["應出席日數_6"] = item.SchoolDayCount;

                            }
                        }
                        if (item.GradeYear == 4)
                        {
                            row["學年度4"] = item.SchoolYear;
                            row["年級4_班級"] = item.ClassName;
                            row["年級4_座號"] = item.SeatNo;

                            //為學生的年級與學年配對
                            schoolyear_grade4 = item.SchoolYear;

                            if (item.Semester == 1)
                            {
                                row["應出席日數_7"] = item.SchoolDayCount;
                            }
                            else
                            {
                                row["應出席日數_8"] = item.SchoolDayCount;

                            }
                        }
                        if (item.GradeYear == 5)
                        {
                            row["學年度5"] = item.SchoolYear;
                            row["年級5_班級"] = item.ClassName;
                            row["年級5_座號"] = item.SeatNo;

                            //為學生的年級與學年配對
                            schoolyear_grade5 = item.SchoolYear;

                            if (item.Semester == 1)
                            {
                                row["應出席日數_9"] = item.SchoolDayCount;
                            }
                            else
                            {
                                row["應出席日數_10"] = item.SchoolDayCount;

                            }
                        }
                        if (item.GradeYear == 6)
                        {
                            row["學年度6"] = item.SchoolYear;
                            row["年級6_班級"] = item.ClassName;
                            row["年級6_座號"] = item.SeatNo;

                            //為學生的年級與學年配對
                            schoolyear_grade6 = item.SchoolYear;

                            if (item.Semester == 1)
                            {
                                row["應出席日數_11"] = item.SchoolDayCount;
                            }
                            else
                            {
                                row["應出席日數_12"] = item.SchoolDayCount;

                            }
                        }
                    }
                                                           
                }

                //學年度與年級的對照字典
                Dictionary<int, int> schoolyear_grade_dict = new Dictionary<int, int>();

                schoolyear_grade_dict.Add(1, schoolyear_grade1);
                schoolyear_grade_dict.Add(2, schoolyear_grade2);
                schoolyear_grade_dict.Add(3, schoolyear_grade3);
                schoolyear_grade_dict.Add(4, schoolyear_grade4);
                schoolyear_grade_dict.Add(5, schoolyear_grade5);
                schoolyear_grade_dict.Add(6, schoolyear_grade6);


                //出缺勤
                if (ar_dict.ContainsKey(stuID))
                {
                    for (int grade = 1; grade <= 6; grade++)
                    {
                        foreach (var ar in ar_dict[stuID])
                        {
                            if (ar.SchoolYear == schoolyear_grade_dict[grade])
                            {
                                if (ar.Semester == 1)
                                {
                                    foreach (var detail in ar.PeriodDetail)
                                    {
                                        if (arStatistic_dict.ContainsKey(detail.AbsenceType + "日數_" + (grade * 2 - 1)))
                                        {
                                            
                                            //加一節，整學期節次與日數的關係，再最後再結算
                                            arStatistic_dict[detail.AbsenceType + "日數_" + (grade * 2 - 1)] += 1;

                                            // 不管是啥缺席，缺席總日數都加一節
                                            arStatistic_dict["缺席總日數_" + (grade * 2 - 1)] += 1;
                                            
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var detail in ar.PeriodDetail)
                                    {
                                        if (arStatistic_dict.ContainsKey(detail.AbsenceType + "日數_" + grade * 2))
                                        {
                                            
                                            //加一節，整學期節次與日數的關係，再最後再結算
                                            arStatistic_dict[detail.AbsenceType + "日數_" + grade * 2] += 1;

                                            // 不管是啥缺席，缺席總日數都加一節
                                            arStatistic_dict["缺席總日數_" + (grade * 2)] += 1;
                                        }
                                    }
                                }
                            }
                        }

                    }

                    foreach (string key in arStatistic_dict.Keys)
                    {
                        arStatistic_dict_days.Add(key, arStatistic_dict[key]);
                    }

                    //真正的填值，填日數，所以要做節次轉換
                    foreach (string key in arStatistic_dict_days.Keys)
                    {
                        //康橋一日有九節，多一節缺曠 = 多1/9 日缺曠，先暫時寫死九節設定，日後要去學務作業每日節次抓取
                        row[key] = Math.Round(arStatistic_dict_days[key] / 9, 2);                                             
                    }
                }


                table.Rows.Add(row);

                
            }



            document = Preference.Template.ToDocument();

            

            document.MailMerge.Execute(table);

            // 最終產物 .doc
            e.Result = document;

            Feedback("列印完成", -1);
        }

        private void MasterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Util.EnableControls(this);

            if (e.Error == null)
            {
                Document doc = e.Result as Document;

                //單檔列印
                if (OneFileSave.Checked)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    fbd.Description = "請選擇儲存資料夾";
                    fbd.ShowNewFolderButton = true;

                    if (fbd.ShowDialog() == DialogResult.Cancel) return;

                    fbdPath = fbd.SelectedPath;


                    Util.DisableControls(this);
                    ConvertToPDF_Worker.RunWorkerAsync();

                }
                else
                {
                    if (Preference.ConvertToPDF)
                    {
                        MotherForm.SetStatusBarMessage("正在轉換PDF格式... 請耐心等候");
                    }

                    Util.DisableControls(this);
                    ConvertToPDF_Worker.RunWorkerAsync();
                    
                }


            }
            else 
            {
                MsgBox.Show(e.Error.Message);
            }
            

            if (Preference.ConvertToPDF)
            {
                MotherForm.SetStatusBarMessage("正在轉換PDF格式", 0);
            }
            else
            {
                MotherForm.SetStatusBarMessage("產生完成", 100);
            }

        }

        private void ConvertToPDF_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Document doc = e_For_ConvertToPDF_Worker.Result as Document;

            if (!OneFileSave.Checked)
            {
                Util.Save(doc, "學籍表", Preference.ConvertToPDF);
            }
            else
            {

                int i = 0;

                foreach (Section section in doc.Sections)
                {
                    // 依照 學號_身分字號_班級_座號_姓名 .doc 來存檔
                    string fileName = "";

                    Document document = new Document();
                    document.Sections.Clear();
                    document.Sections.Add(document.ImportNode(section, true));
                    
                    

                    fileName = PrintStudents[i].StudentNumber;

                    fileName = PrintStudents[i].StudentNumber;

                    fileName += "_" + PrintStudents[i].IDNumber;

                    if (!string.IsNullOrEmpty(PrintStudents[i].RefClassID))
                    {
                        fileName += "_" + PrintStudents[i].Class.Name;
                    }
                    else
                    {
                        fileName += "_";
                    }

                    fileName += "_" + PrintStudents[i].SeatNo;

                    fileName += "_" + PrintStudents[i].Name;

                    //document.Save(fbd.SelectedPath + "\\" +fileName+ ".doc");

                    if (Preference.ConvertToPDF)
                    {
                        //string fPath = fbd.SelectedPath + "\\" + fileName + ".pdf";

                        string fPath = fbdPath + "\\" + fileName + ".pdf";

                        FileInfo fi = new FileInfo(fPath);

                        DirectoryInfo folder = new DirectoryInfo(Path.Combine(fi.DirectoryName, Path.GetRandomFileName()));
                        if (!folder.Exists) folder.Create();

                        FileInfo fileinfo = new FileInfo(Path.Combine(folder.FullName, fi.Name));

                        string XmlFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".xml";
                        string PDFFileName = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".pdf";

                        document.Save(XmlFileName, Aspose.Words.SaveFormat.AsposePdf);

                        Aspose.Pdf.Pdf pdf1 = new Aspose.Pdf.Pdf();

                        pdf1.BindXML(XmlFileName, null);
                        pdf1.Save(PDFFileName);

                        if (File.Exists(fPath))
                            File.Delete(Path.Combine(fi.DirectoryName, fi.Name));

                        File.Move(PDFFileName, fPath);
                        folder.Delete(true);

                        int percent = (((i + 1) * 100 / doc.Sections.Count));

                        ConvertToPDF_Worker.ReportProgress(percent, "PDF轉換中...進行到" + (i + 1) + "/" + doc.Sections.Count + "個檔案");
                    }
                    else
                    {
                        document.Save(fbdPath + "\\" + fileName + ".doc");

                        int percent = (((i + 1) * 100 / doc.Sections.Count));

                        ConvertToPDF_Worker.ReportProgress(percent, "Doc存檔...進行到" + (i + 1) + "/" + doc.Sections.Count + "個檔案");
                    }

                    i++;


                }
            }
        }


        private void ConvertToPDF_Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Util.EnableControls(this);

            if (Preference.ConvertToPDF)
            {
                MotherForm.SetStatusBarMessage("PDF轉換完成", 100);

            }
            else
            {
                MotherForm.SetStatusBarMessage("存檔完成", 100);

            }

        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region IStatusReporter 成員

        //回報進度條
        public void Feedback(string message, int percentage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, int>(Feedback), new object[] { message, percentage });
            }
            else
            {
                if (percentage < 0)
                    MotherForm.SetStatusBarMessage(message);
                else
                    MotherForm.SetStatusBarMessage(message, percentage);

                Application.DoEvents();
            }
        }

        #endregion
     
        //2017/12/19 穎驊特別註解，這邊先採用舊寫法提供使用者設定樣板， 僅能上傳、下載舊版的.doc 格式，若傳.docx 會錯誤
        // 日後有時間再改新寫法
        private void lnkTemplate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportTemplate defaultTemplate = new ReportTemplate(Prc.康橋國小學籍表_樣板_, TemplateType.Word);
            TemplateSettingForm form = new TemplateSettingForm(Preference.Template, defaultTemplate);
            form.DefaultFileName = "學籍表(樣版).doc";

            if (form.ShowDialog() == DialogResult.OK)
            {
                Preference.Template = (form.Template == defaultTemplate) ? null : form.Template;
                Preference.Save();
            }
        }

        //供使用者下載學籍表合併欄位總表
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //宣告產生的報表
            Aspose.Words.Document document = new Aspose.Words.Document();

            document= new Aspose.Words.Document(new System.IO.MemoryStream(Properties.Resources.康橋新竹國小學籍表功能變數));

            System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
            sd.Title = "另存新檔";
            sd.FileName = "學籍表合併欄位總表" + ".doc";
            sd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    document.Save(sd.FileName, Aspose.Words.SaveFormat.Doc);
                    System.Diagnostics.Process.Start(sd.FileName);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
            }

        }
    }
}
