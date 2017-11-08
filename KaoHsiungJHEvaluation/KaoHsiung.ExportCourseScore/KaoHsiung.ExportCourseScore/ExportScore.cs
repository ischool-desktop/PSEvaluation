using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Aspose.Cells;
using K12.Data;
using System.IO;
using System.Windows.Forms;

namespace KaoHsiung.ExportCourseScore
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

        public ExportScore(List<string> CourseIDList)
        {
            _CourseIDList = CourseIDList;
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
            FISCA.Presentation.MotherForm.SetStatusBarMessage("課程成績總表生中...", e.ProgressPercentage);
        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("課程成績總表生完成。", 100);
            if (e.Cancelled)
            {
                FISCA.Presentation.Controls.MsgBox.Show("資料產生過程發生錯誤:");
            }
            else
            {
                if (_PrintRangeError.Count > 0)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(string.Join(",", _PrintRangeError.ToArray()) + " 小考成績超出列印範圍，資料將無法完整列印出來，請老師將這些課程手動處理，調整列印範圍和紙張大小，Excel 2003調整列印範圍方式：先選取工作表欄A~有資料欄，再選擇[檔案]>[列印範圍]>[設定列印範圍]，調整紙張大小：選[檔案]>[版面設定]>[頁面]內設定。");
                }

                try
                {
                    string FilePath = Application.StartupPath + "\\Reports\\課程成績總表.xls";
                    _wb_export.Save(FilePath, Aspose.Cells.FileFormatType.Excel2003);
                    System.Diagnostics.Process.Start(FilePath);

                }
                catch
                {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "另存新檔";
                    sd.FileName = "課程成績總表.xls";
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
                _PrintRangeError.Clear();

                _bgWorker.ReportProgress(1);
                // 讀取 excel 樣板                
                _wb_export.Open(new MemoryStream(Properties.Resources.課程成績總表樣板));
                _wb_template.Open(new MemoryStream(Properties.Resources.課程成績總表樣板));
                Range title = _wb_template.Worksheets[0].Cells.CreateRange(0, 2, false);// GetRangeByName("Title");
                Range copyRow = _wb_template.Worksheets[0].Cells.CreateRange(2, 1, false);//.GetRangeByName("CopyRow");

                Worksheet wst = _wb_export.Worksheets[0];
                Worksheet wstDif = _wb_export.Worksheets["平時成績有差異"];

                Style styleObject = null;

                _bgWorker.ReportProgress(10);

                // 課程基本資料
                _CourseRecDict.Clear();
                List<CourseRecord> coList = Course.SelectByIDs(_CourseIDList);
                foreach (CourseRecord rec in coList)
                    _CourseRecDict.Add(rec.ID, rec);
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

                int DataCurrentRowIdx = 0, TitleStartRowIdx = 0;
                int TitleRowCount = title.RowCount;
                int ScoreDifRow = 1;
                // 合併到工作表
                foreach (string CourseID in _CourseIDList)
                {
                    // 課程名稱
                    if (_CourseRecDict.ContainsKey(CourseID))
                    {
                        TitleStartRowIdx = DataCurrentRowIdx;
                        wst.Cells.CreateRange(TitleStartRowIdx, TitleRowCount, false).Copy(title);
                        string TitleName = "";
                        if (_CourseRecDict[CourseID].SchoolYear.HasValue)
                            TitleName += _CourseRecDict[CourseID].SchoolYear.Value + "學年度";

                        if (_CourseRecDict[CourseID].Semester.HasValue)
                            TitleName += " 第" + _CourseRecDict[CourseID].Semester.Value + "學期 ";

                        TitleName += _CourseRecDict[CourseID].Name;

                        wst.Cells[TitleStartRowIdx, 0].PutValue(TitleName);
                    }

                    DataCurrentRowIdx += TitleRowCount;

                    // 欄位名稱建立
                    // columIdx
                    // 課程總成績
                    int CousreScoreIdx = 4;
                    // 定期
                    Dictionary<string, int> examColIdxDict = new Dictionary<string, int>();
                    Dictionary<string, string> examNameDict = new Dictionary<string, string>();
                    if (_ExamNameDict.ContainsKey(CourseID))
                    {
                        examNameDict = _ExamNameDict[CourseID];
                        List<string> tmpList1 = examNameDict.Keys.ToList();
                        tmpList1.Sort();
                        // 定期評量索引                    
                        int intT = 5;
                        foreach (string str in tmpList1)
                        {
                            examColIdxDict.Add(str, intT);
                            intT++;
                        }
                    }
                    // 平時評量
                    int OrdinarilyIdx = examNameDict.Count + 5;

                    // 小考
                    Dictionary<string, int> OrdinarilyColIdxDict = new Dictionary<string, int>();
                    Dictionary<string, Dictionary<string, decimal>> OrdinarilyDict = new Dictionary<string, Dictionary<string, decimal>>();
                    if (_OrdinarilyDict.ContainsKey(CourseID))
                    {
                        OrdinarilyDict = _OrdinarilyDict[CourseID];
                        List<string> tmpList2 = OrdinarilyDict.Keys.ToList();
                        // tmpList2.Sort();  小考不排序

                        int intT2 = OrdinarilyIdx + 2;
                        foreach (string str in tmpList2)
                        {
                            OrdinarilyColIdxDict.Add(str, intT2);
                            intT2++;
                        }

                        // 超出列印範圍紀錄
                        if (intT2 > 26)
                        {
                            string msg = "";
                            if (_CourseRecDict[CourseID].SchoolYear.HasValue)
                                msg += _CourseRecDict[CourseID].SchoolYear.Value + "學年度 ";
                            if (_CourseRecDict[CourseID].Semester.HasValue)
                                msg += "第" + _CourseRecDict[CourseID].Semester.Value + "學期 ";
                            msg += _CourseRecDict[CourseID].Name;
                            _PrintRangeError.Add(msg);
                        }
                    }

                    int examRowIdx = TitleStartRowIdx + 1;
                    // 定期
                    foreach (KeyValuePair<string, int> idx in examColIdxDict)
                        wst.Cells[examRowIdx, idx.Value].PutValue(idx.Key);

                    // 平時
                    wst.Cells[examRowIdx, OrdinarilyIdx].PutValue(Global.CourseOrdinarilyScoreKey);

                    // 試算平時成績
                    wst.Cells[examRowIdx, OrdinarilyIdx + 1].PutValue("試算平時評量");

                    // 小考
                    foreach (KeyValuePair<string, int> idx in OrdinarilyColIdxDict)
                        wst.Cells[examRowIdx, idx.Value].PutValue(idx.Key);

                    // 學生成績
                    if (_CousreStudentScoreDict.ContainsKey(CourseID))
                    {

                        foreach (CourseStudentsScore data in _CousreStudentScoreDict[CourseID])
                        {
                            wst.Cells.CreateRange(DataCurrentRowIdx, 1, false).Copy(copyRow);
                            wst.Cells[DataCurrentRowIdx, 0].PutValue(data.ClassName);

                            if (data.SeatNo.HasValue)
                                wst.Cells[DataCurrentRowIdx, 1].PutValue(data.SeatNo.Value.ToString());

                            wst.Cells[DataCurrentRowIdx, 2].PutValue(data.StudentName);
                            wst.Cells[DataCurrentRowIdx, 3].PutValue(data.StudentNumber);
                            // 課程總成績
                            if (data.ScoreItems.ContainsKey(Global.CousreScoreAvgKey))
                            {
                                if (data.ScoreItems[Global.CousreScoreAvgKey].Score.HasValue)
                                    wst.Cells[DataCurrentRowIdx, CousreScoreIdx].PutValue(data.ScoreItems[Global.CousreScoreAvgKey].Score.Value);
                            }

                            // 定期評量成績
                            foreach (KeyValuePair<string, int> idx in examColIdxDict)
                            {
                                if (data.ScoreItems.ContainsKey(idx.Key))
                                    if (data.ScoreItems[idx.Key].Score.HasValue)
                                        wst.Cells[DataCurrentRowIdx, idx.Value].PutValue(data.ScoreItems[idx.Key].Score.Value);
                            }

                            // 平時成績
                            if (data.ScoreItems.ContainsKey(Global.CourseOrdinarilyScoreKey))
                                if (data.ScoreItems[Global.CourseOrdinarilyScoreKey].Score.HasValue)
                                    wst.Cells[DataCurrentRowIdx, OrdinarilyIdx].PutValue(data.ScoreItems[Global.CourseOrdinarilyScoreKey].Score.Value);

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
                                wst.Cells[DataCurrentRowIdx, OrdinarilyIdx + 1].PutValue(value);
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

                                    if (styleObject == null)
                                    {
                                        styleObject = _wb_export.Styles[_wb_export.Styles.Add()];
                                        styleObject.Copy(wst.Cells[DataCurrentRowIdx, OrdinarilyIdx + 1].Style);
                                        styleObject.Font.Color = System.Drawing.Color.Red;
                                    }
                                    wst.Cells[DataCurrentRowIdx, OrdinarilyIdx + 1].Style = styleObject;
                                }

                            }

                            // 小考，需要轉換subID
                            foreach (KeyValuePair<string, int> idx in OrdinarilyColIdxDict)
                            {
                                if (OrdinarilyDict.ContainsKey(idx.Key))
                                {
                                    foreach (string sbItemID in OrdinarilyDict[idx.Key].Keys)
                                    {
                                        if (data.ScoreItems.ContainsKey(sbItemID))
                                            if (data.ScoreItems[sbItemID].Score.HasValue)
                                                wst.Cells[DataCurrentRowIdx, idx.Value].PutValue(data.ScoreItems[sbItemID].Score.Value);
                                    }
                                }
                            }
                            DataCurrentRowIdx++;
                        }
                    }
                    wst.HPageBreaks.Add(DataCurrentRowIdx, 0);
                }
                wstDif.AutoFitColumns();

                // 當沒有差異資料移除有差異工作表
                if (ScoreDifRow == 1)
                    _wb_export.Worksheets.RemoveAt("平時成績有差異");

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
