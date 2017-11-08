using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JHEvaluation.ClassSemesterScoreAvgComparison
{
    // 報表處理
    class ClassSemsScoreAvgCmpReporter
    {
        private Dictionary<string, DAL.ClassEntity> _ClassEntityDic;
        private Dictionary<string, int> _WSTTitleColIdx;
        private Workbook _wb;
        private Worksheet _wst;
        private BackgroundWorker _BGWorker;
        private List<string> _ClassNameList;
        private string _SchoolName = string.Empty;
        private string _SchoolYear=string.Empty;
        private string _Semester = string.Empty;
        private ScoreHeaderIndexer shi;
        public static int UserSelectCount = 0;

        public ClassSemsScoreAvgCmpReporter(Dictionary<string,DAL.ClassEntity> ClassEntityDic,List<string> SubjList,List<string> DomainList,string SchoolName,string SchoolYear,string Semester)
        {
            shi = new ScoreHeaderIndexer();
            foreach (string str in SubjList)
                shi.Add(str, false, 0);

            foreach (string str in DomainList)
                shi.Add(str, true, 0);

            shi.Sort(DAL.DALTransfer.GetDomainMapping());

            int ColIdx = 1;
            foreach (Header h in shi)
            {
                h.ColumnIndex = ColIdx;
                ColIdx += 2;
            }
            
            // 建立 ColIdx

            _ClassEntityDic = ClassEntityDic;

            _ClassNameList = new List<string>();

            foreach (string str in _ClassEntityDic.Keys)
                _ClassNameList.Add(str);

            _ClassNameList.Sort();

            _SchoolName = SchoolName;
            _SchoolYear = SchoolYear;
            _Semester = Semester;

            _BGWorker = new BackgroundWorker();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
            _BGWorker.RunWorkerAsync();
        }

        void _BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MsgBox.Show("產生報表時發生錯誤" + e.Error.Message);
                return;
            }
            
            Workbook book = e.Result as Workbook;

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists) dir.Create();

            path = Path.Combine(path, "班級學期成績比較表.xls");

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                book.Save(path, FileFormatType.Excel2003);


                if (MsgBox.Show("班級學期成績比較表 產生完成，是否立刻開啟？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(path);
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("儲存失敗");
            }

        }

        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _wb = new Workbook();
            if (_ClassNameList.Count > 30 || UserSelectCount > 14)
                _wb.Open(new MemoryStream(Properties.Resources.班級學期成績平均比較表60));
            else
                _wb.Open(new MemoryStream(Properties.Resources.班級學期成績比較表樣版));

            _wst = _wb.Worksheets[0];
            int RowIdx = 3;

            _wst.Cells[0, 0].PutValue(string.Format("{0}  {1}學年度  第{2}學期  (領域總成績)各班各科平均成績比較表)", _SchoolName, _SchoolYear,_Semester));

            // 填入欄位
            foreach (Header h in shi)
            {
                if (h.IsDomain)
                {
                    _wst.Cells[2, h.ColumnIndex].Style.Font.IsBold = true;                
                }  
                    
                _wst.Cells[2, h.ColumnIndex].PutValue(h.Name);
                _wst.Cells[2, h.ColumnIndex + 1].PutValue("排序");
            }

            foreach (string str in _ClassNameList)
            {
                if (_ClassEntityDic.ContainsKey(str))
                {
                    // 班名
                    _wst.Cells[RowIdx, 0].PutValue(str);

                    //// 填入成績
                    foreach (Header h in shi)
                    { 
                        string nameIdx;
                        if (h.IsDomain)
                            nameIdx = h.Name + "_";
                        else
                            nameIdx = h.Name;

                        _wst.Cells[RowIdx,h.ColumnIndex].PutValue(_ClassEntityDic[str].GetClassSubjAvgScore(nameIdx));
                    
                    }

                    // 放班級人數
                    int peoColIdx = 32;
                    // 找到人數放位置
                    for (int i = 32; i <= 60; i++)
                        if (_wst.Cells[2, i].StringValue.Trim() == "人數")
                        {
                            peoColIdx = i;
                            break;
                        }
  
                    // 填入班人數
                    _wst.Cells[RowIdx, peoColIdx].PutValue(_ClassEntityDic[str].StudCount);
                    RowIdx++;
                }                
            }
            e.Result = _wb;
        }        

    }
}
