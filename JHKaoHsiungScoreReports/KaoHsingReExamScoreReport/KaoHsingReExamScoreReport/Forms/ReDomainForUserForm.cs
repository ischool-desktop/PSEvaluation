using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using KaoHsingReExamScoreReport.DAO;
using Aspose.Cells;
using System.IO;

namespace KaoHsingReExamScoreReport.Forms
{
    public partial class ReDomainForUserForm : BaseForm
    {
        BackgroundWorker _bgWorker;
        int _SchoolYear = 0;
        int _Semester = 0;
        List<ClassData> _ClassDataList;
        List<StudentData> _StudentDataList;
        List<string> _SelectClassIDList;
        decimal _passScore = 60;

        public ReDomainForUserForm(List<string> ClassIDList)
        {
            InitializeComponent();
            _bgWorker = new BackgroundWorker();
            _ClassDataList = new List<ClassData>();
            _StudentDataList = new List<StudentData>();
            _SelectClassIDList = ClassIDList;
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.DoWork += _bgWorker_DoWork;
            _bgWorker.RunWorkerCompleted += _bgWorker_RunWorkerCompleted;
            _bgWorker.ProgressChanged += _bgWorker_ProgressChanged;
        }

        void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("領域補考名單產生中 ...", e.ProgressPercentage);
        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnPrint.Enabled = true;

            Workbook wb = (Workbook)e.Result;

            if (wb != null)
                Utility.CompletedXls("領域補考名單-給試務", wb);
            FISCA.Presentation.MotherForm.SetStatusBarMessage("領域補考名單產生完成.");
        }

        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _bgWorker.ReportProgress(1);
            // 取得班級
            _ClassDataList = QueryData.GetClassDataByClassIDs(_SelectClassIDList);

            _bgWorker.ReportProgress(20);
            // 取得學生
            _StudentDataList = QueryData.GetStudentDataListByClassIDs(_SelectClassIDList);

            _bgWorker.ReportProgress(40);
            // 取得學期成績並判斷
            _StudentDataList = Utility.CalcStudDomainScorePass(_StudentDataList, _SchoolYear, _Semester, _passScore);

            _bgWorker.ReportProgress(70);
            // 取得樣版
            Workbook wb = new Workbook(new MemoryStream(Properties.Resources.給試務樣版));

            // 使用領域名稱
            List<string> DomainNameList = Utility.GetDomainNameList();
            // copy Template
            int wstIdx = wb.Worksheets.AddCopy("給試務樣版");
            Worksheet wst = wb.Worksheets[wstIdx];
            wst.Name = "給試務";

            // 依班級列印
            int rowIdx = 1;
            Dictionary<string, int> ReExamDomainCountDict = new Dictionary<string, int>();
            foreach (ClassData cd in _ClassDataList)
            {
                ReExamDomainCountDict.Clear();
                foreach (StudentData sd in _StudentDataList.Where(x => x.ClassID == cd.ClassID))
                {
                    foreach (string name in DomainNameList)
                    {
                        if (sd.DomainScorePassDict.ContainsKey(name))
                        {
                            if (sd.DomainScorePassDict[name] == false)
                            {
                                if (!ReExamDomainCountDict.ContainsKey(name))
                                    ReExamDomainCountDict.Add(name, 0);

                                ReExamDomainCountDict[name]++;
                            }

                        }
                    }
                }

                // 計算各領域不及格人數
                cd.DomainReExamCount = ReExamDomainCountDict;

                wst.Cells[rowIdx, 0].PutValue(cd.ClassName);
                int colIdx = 1;
                foreach (string name in DomainNameList)
                {
                    wst.Cells[rowIdx, colIdx].PutValue(0);
                    if (cd.DomainReExamCount.ContainsKey(name))
                        wst.Cells[rowIdx, colIdx].PutValue(cd.DomainReExamCount[name]);

                    colIdx++;
                }
                rowIdx++;
            }

            _bgWorker.ReportProgress(100);
            string rr = "J" + rowIdx;
            Range rng = wst.Cells.CreateRange("A2", rr);
            Style sty = wb.CreateStyle();
            sty.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            sty.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            sty.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            sty.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            StyleFlag sf = new StyleFlag();
            sf.Borders = true;
            rng.ApplyStyle(sty, sf);
            // 移除樣版
            wb.Worksheets.RemoveAt("給試務樣版");

            e.Result = wb;

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReDomainForUserForm_Load(object sender, EventArgs e)
        {
            this.MinimumSize = this.MaximumSize = this.Size;

            // 預設學年度、學期
            iptSchoolYear.Value = int.Parse(K12.Data.School.DefaultSchoolYear);
            iptSemester.Value = int.Parse(K12.Data.School.DefaultSemester);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;

            // 使用者選學年度、學期
            _SchoolYear = iptSchoolYear.Value;
            _Semester = iptSemester.Value;

            _bgWorker.RunWorkerAsync();
        }
    }
}
