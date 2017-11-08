using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Aspose.Cells;
using K12.Data;
using System.IO;
using System.Windows.Forms;

namespace KaoHsiung.ExportCourseScoreDiff
{
    /// <summary>
    /// 產生成績
    /// </summary>
    public class ExportScore
    {
        List<string> _CourseIDList;
        /// <summary>
        /// 定期考試名稱
        /// </summary>
        Dictionary<string, Dictionary<string, string>> _ExamNameDict;
        /// <summary>
        /// 平時考試名稱
        /// </summary>
        Dictionary<string, Dictionary<string, Dictionary<string, decimal>>> _OrdinarilyDict;

        /// <summary>
        /// 課程學生考試成績
        /// </summary>
        Dictionary<string, List<CourseStudentsScore>> _CousreStudentScoreDict;

        /// <summary>
        /// 課程基本資料
        /// </summary>
        Dictionary<string, CourseRecord> _CourseRecDict;

        /// <summary>
        /// 當課程有超出列印範圍
        /// </summary>
        List<string> _PrintRangeError;

        /// <summary>
        /// 產生出來Excel
        /// </summary>
        Workbook _wb_export;

        /// <summary>
        /// 樣板
        /// </summary>
        Workbook _wb_template;

        BackgroundWorker _bgWorker;

        int _SchoolYear;
        int _Semester;

        public ExportScore(int SchoolYear,int Semester)
        {
            _SchoolYear = SchoolYear;
            _Semester = Semester;
            _CourseIDList = new List<string>();
            _wb_template = new Workbook();
            _wb_export = new Workbook();
            _PrintRangeError = new List<string>();
            _ExamNameDict = new Dictionary<string, Dictionary<string, string>>();
            _OrdinarilyDict = new Dictionary<string, Dictionary<string, Dictionary<string, decimal>>>();
            _CousreStudentScoreDict = new Dictionary<string, List<CourseStudentsScore>>();
            _CourseRecDict = new Dictionary<string, CourseRecord>();
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += new DoWorkEventHandler(_bgWorker_DoWork);
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.ProgressChanged += new ProgressChangedEventHandler(_bgWorker_ProgressChanged);
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWorker_RunWorkerCompleted);
        }

        void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("平時評量成績差異檢視產生中...", e.ProgressPercentage);
        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("平時評量成績差異檢視產生完成。", 100);
            if (e.Cancelled)
            {
                FISCA.Presentation.Controls.MsgBox.Show("資料產生過程發生錯誤:");
            }
            else
            {
                try
                {
                    string FilePath = Application.StartupPath + "\\Reports\\平時評量成績差異檢視.xls";
                    _wb_export.Save(FilePath, Aspose.Cells.FileFormatType.Excel2003);
                    System.Diagnostics.Process.Start(FilePath);

                }
                catch
                {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "另存新檔";
                    sd.FileName = "平時評量成績差異檢視.xls";
                    sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                    if (sd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            _wb_export.Save(sd.FileName, Aspose.Cells.FileFormatType.Excel2003);
                            System.Diagnostics.Process.Start(sd.FileName);
                        }
                        catch
                        {
                            MessageBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
        }

        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _bgWorker.ReportProgress(1);
                // 讀取 excel 樣板                
                _wb_export.Open(new MemoryStream(Properties.Resources.課程平時評量差異樣版));
                _wb_template.Open(new MemoryStream(Properties.Resources.課程平時評量差異樣版));
                Worksheet wstDif = _wb_export.Worksheets["平時成績有差異"];                
                

                _bgWorker.ReportProgress(10);

                // 課程基本資料
                _CourseRecDict.Clear();
                List<CourseRecord> coList = Course.SelectBySchoolYearAndSemester(_SchoolYear, _Semester);
                foreach (CourseRecord rec in coList)
                    _CourseRecDict.Add(rec.ID, rec);

                _CourseIDList = (from data in coList select data.ID).ToList();

                _bgWorker.ReportProgress(30);
                // 取得定期考試名稱
                _ExamNameDict = Utility.GetCourseExamNameDict(_CourseIDList);

                _bgWorker.ReportProgress(50);
                // 取得小考試別名稱
                _OrdinarilyDict = Utility.GetCourseOrdinarilyNameDict(_CourseIDList);
                _bgWorker.ReportProgress(70);

                // 取得課程學生成績
                _CousreStudentScoreDict.Clear();
                foreach (CourseStudentsScore rec in Utility.GetCourseStudentList(_CourseIDList))
                {
                    if (!_CousreStudentScoreDict.ContainsKey(rec.CourseID))
                    {
                        List<CourseStudentsScore> list = new List<CourseStudentsScore>();
                        _CousreStudentScoreDict.Add(rec.CourseID, list);
                    }

                    _CousreStudentScoreDict[rec.CourseID].Add(rec);
                }
                _bgWorker.ReportProgress(90);

                int ScoreDifRow = 1;
                // 合併到工作表
                foreach (string CourseID in _CourseIDList)
                {
                    // 小考
                    Dictionary<string, int> OrdinarilyColIdxDict = new Dictionary<string, int>();
                    Dictionary<string, Dictionary<string, decimal>> OrdinarilyDict = new Dictionary<string, Dictionary<string, decimal>>();
                    if (_OrdinarilyDict.ContainsKey(CourseID))
                    {
                        OrdinarilyDict = _OrdinarilyDict[CourseID];
                        List<string> tmpList2 = OrdinarilyDict.Keys.ToList();                                                                     
                    }
                    
                    // 學生成績
                    if (_CousreStudentScoreDict.ContainsKey(CourseID))
                    {
                        foreach (CourseStudentsScore data in _CousreStudentScoreDict[CourseID])
                        {
                            // 試算平時成績
                            decimal totalCount = 0;
                            decimal scoreWidghtCount = 0;
                            bool hasScore = false;
                            foreach (var sbItem in OrdinarilyDict.Values)
                            {
                                foreach (var eid in sbItem.Keys)
                                {
                                    if (data.ScoreItems.ContainsKey(eid))
                                        if (data.ScoreItems[eid].Score.HasValue)
                                        {
                                            hasScore = true;
                                            totalCount += sbItem[eid];
                                            scoreWidghtCount += data.ScoreItems[eid].Score.Value * sbItem[eid];
                                        }
                                }
                            }
                            if (totalCount > 0 && hasScore)
                            {
                                decimal value = Math.Round((scoreWidghtCount / totalCount), 2, MidpointRounding.AwayFromZero);
                               
                                if (data.ScoreItems.ContainsKey(Global.CourseOrdinarilyScoreKey)
                                    && data.ScoreItems[Global.CourseOrdinarilyScoreKey].Score.HasValue
                                    && value != data.ScoreItems[Global.CourseOrdinarilyScoreKey].Score.Value
                                    )
                                {

                                    // 將平時成績結果與試算不同放入另一個工作表
                                    // 課程名稱,班級,座號,姓名,學號,平時成績,試算平時成績
                                     // 課程名稱                                                                        
                                    wstDif.Cells[ScoreDifRow, 0].PutValue(data.CourseName);
                                    // 班級
                                    wstDif.Cells[ScoreDifRow, 1].PutValue(data.ClassName);
                                    // 座號
                                    if(data.SeatNo.HasValue)
                                        wstDif.Cells[ScoreDifRow, 2].PutValue(data.SeatNo.Value.ToString());
                                    // 姓名
                                    wstDif.Cells[ScoreDifRow, 3].PutValue(data.StudentName);
                                    // 學號
                                    wstDif.Cells[ScoreDifRow, 4].PutValue(data.StudentNumber);
                                    // 平時
                                    wstDif.Cells[ScoreDifRow, 5].PutValue(data.ScoreItems[Global.CourseOrdinarilyScoreKey].Score.Value);
                                    // 試算平時
                                    wstDif.Cells[ScoreDifRow, 6].PutValue(value);

                                    ScoreDifRow++;
                                }

                            }                            
                        }
                    }
                }
                wstDif.AutoFitColumns();
                _bgWorker.ReportProgress(99);

            }
            catch (Exception ex)
            {
                e.Result = ex;
                e.Cancel = true;
            }
        }

        /// <summary>
        ///產生資料
        /// </summary>
        public void Export()
        {
            _bgWorker.RunWorkerAsync();
        }
    }
}
