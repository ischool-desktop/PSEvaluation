using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using JHSchool.Data;
using Campus.Report;
using JHSchool.Evaluation;
using JHSchool.Evaluation.Mapping;
using System.Linq;

namespace HsinChu.StudentRecordReport
{
    // 學生學期成績處理至報表
    class StudentSemesterScoreProcessor
    {
        private const int TotalRow = 44;
        private const int GraduateShift = 20;

        private DocumentBuilder _builder;
        private DomainRowManager _manager;
        private SemesterMap _map = new SemesterMap();
        private Dictionary<string, bool> _domains = new Dictionary<string, bool>();

        // 畢業成績
        private K12.Data.GradScoreRecord _StudGradScore;

        public DegreeMapper DegreeMapper { get; set; }
        public bool PrintPeriod { get; set; }
        public bool PrintCredit { get; set; }

        private ReportConfiguration _config;
        //private bool _printPeriod = false;
        //private bool _printCredit = false;

        private Cell _cell;
        private Run _run;

        public StudentSemesterScoreProcessor(DocumentBuilder builder, SemesterMap map,string type, Dictionary<string, bool> domains,K12.Data.GradScoreRecord StudGradScore)
        {
            builder.MoveToMergeField("成績");
            _builder = builder;
            _manager = new DomainRowManager(type);
            _cell = builder.CurrentParagraph.ParentNode as Cell;
            _run = new Run(_cell.Document);
            _run.Font.Name = builder.Font.Name;
            _run.Font.Size = builder.Font.Size;
            _run.Text = string.Empty;
            _map = map;
            _domains = domains;
            _StudGradScore = StudGradScore;

            _config = new ReportConfiguration(Global.ReportName);
            PrintPeriod = _config.GetBoolean("列印節數", false);
            PrintCredit = _config.GetBoolean("列印權數", false);
        }

        public void SetData(List<JHSemesterScoreRecord> semesterScoreList)
        {
            foreach (JHSemesterScoreRecord record in semesterScoreList)
                _manager.Add(record);

            FillSchoolYear();
            FillDomainRows();
        }

        private void FillSchoolYear()
        {
            int index = 0;
            foreach (string name in new string[] { "一上學年度", "一下學年度", "二上學年度", "二下學年度", "三上學年度", "三下學年度" })
            {
                _builder.MoveToMergeField(name);
                _builder.Write(_map.SchoolYearMapping.ContainsKey(index) ? _map.SchoolYearMapping[index] + "學年度" : "");
                index++;
            }
        }

        /// <summary>
        /// 寫入領域 Rows
        /// </summary>
        private void FillDomainRows()
        {
            Cell cell = _cell;
            Row row = _cell.ParentRow;
            Table table = row.ParentTable;
            int deleteIndex = table.IndexOf(row);
            int rowIndex = table.IndexOf(row);
            int SubjCount = 0;
            foreach (DomainRow domainRow in _manager.DomainRows)
            {
                SubjCount = 0;
                // 透過對照表查詢領域是否需要展開
                bool display = DomainSubjectExpand.展開;
                if (_domains.ContainsKey(domainRow.Domain))
                    display = _domains[domainRow.Domain];

                // 不展開，列印領域
                if (display == DomainSubjectExpand.不展開)
                {
                    if (domainRow.Scores.Count <= 0) continue;

                    table.Rows.Add(row.Clone(true));     
                    Row tempRow = table.LastRow;                    
                    WordHelper.MergeHorizontalCell(tempRow.Cells[0], 2);
                    WordHelper.Write(tempRow.Cells[0], (string.IsNullOrEmpty(domainRow.Domain) ? "彈性課程" : domainRow.Domain), _builder);
                    WriteDomain(tempRow.Cells[1], domainRow);
                }
                // 展開，列印科目
                else
                {
                    
                    int subjectCount = 0;
                    foreach (string subject in domainRow.SubjectScores.Keys)
                    {
                        subjectCount++;

                        table.Rows.Add(row.Clone(true));
                        Row tempRow = table.LastRow;

                        WordHelper.Write(tempRow.Cells[1], subject, _builder);
                        WriteSubject(tempRow.Cells[1], domainRow.SubjectScores[subject]);
                    }
                    SubjCount = subjectCount;
                    
                    // 當只有領域成績沒有科目成績時
                    if (subjectCount > 0)
                    {
                        Row startRow = table.Rows[table.Rows.Count - subjectCount];
                        WordHelper.Write(startRow.Cells[0], (string.IsNullOrEmpty(domainRow.Domain) ? "彈性課程" : domainRow.Domain), _builder);
                        if (subjectCount > 1)
                            WordHelper.MergeVerticalCell(startRow.Cells[0], subjectCount);
                    }
                }

                // 畢業成績
                if (_StudGradScore.Domains.ContainsKey(domainRow.Domain))
                {

                    Row tmpRow = table.Rows[table.Rows.Count - SubjCount];


                    // 當科目數 >=1
                    if (SubjCount >= 1)
                    {
                        WordHelper.MergeVerticalCell(tmpRow.Cells[20], SubjCount);
                        WordHelper.MergeVerticalCell(tmpRow.Cells[21], SubjCount);
                    }
                    else
                        tmpRow =table.LastRow ;


                        //Row tmpRow = table.LastRow;
                        if (_StudGradScore.Domains[domainRow.Domain].Score.HasValue)
                        {
                            // 分數
                            WordHelper.Write(tmpRow.Cells[20], _StudGradScore.Domains[domainRow.Domain].Score.Value.ToString(), _builder);
                            // 等第
                            WordHelper.Write(tmpRow.Cells[21], DegreeMapper.GetDegreeByScore(_StudGradScore.Domains[domainRow.Domain].Score.Value), _builder);
                        }
                }
            }
            
            table.Rows[deleteIndex].Remove();

            // 畫底出線
            foreach (Cell c in table.LastRow.Cells)
                c.CellFormat.Borders.Bottom.LineWidth = 1.5;


            //bool hasScore = false;

            //Cell cell = _cell;
            //Cell gradCell1 = GetMoveRightCell(cell, GraduateShift);
            //Cell gradCell2 = GetMoveRightCell(cell, GraduateShift + 1);

            //int first_row_index = _cell.ParentRow.ParentTable.IndexOf(_cell.ParentRow);

            //foreach (DomainRow row in _manager.DomainRows)
            //{
            //    // 透過對照表查詢領域是否需要展開
            //    bool display = DomainSubjectExpand.展開;
            //    if (_domains.ContainsKey(row.Domain))
            //        display = _domains[row.Domain];

            //    hasScore = true;
            //    //int count = row.Count;

            //    Write(cell, string.IsNullOrEmpty(row.Domain) ? "彈性課程" : row.Domain);

            //    // 不展開，列印領域
            //    if (display == DomainSubjectExpand.不展開)
            //    {
            //        MergeVerticalCell(cell, count);
            //        MergeVerticalCell(gradCell1, count);
            //        MergeVerticalCell(gradCell2, count);
            //    }
            //    // 展開，列印科目
            //    else
            //    {

            //        Cell subjectCell = GetMoveRightCell(cell, 1);
            //        foreach (SubjectRow subjectRow in row.SubjectRows)
            //        {
            //            WriteSubject(subjectCell, subjectRow);
            //            subjectCell = GetMoveDownCell(subjectCell, 1);
            //            if (subjectCell == null) break;
            //        }

            //    }

            //    cell = GetMoveDownCell(cell, count);
            //    gradCell1 = GetMoveDownCell(gradCell1, count);
            //    gradCell2 = GetMoveDownCell(gradCell2, count);

            //    if (cell == null) break;
            //}

            //if (hasScore)
            //{
            //    do
            //    {
            //        cell.CellFormat.VerticalMerge = CellMerge.Previous;
            //        cell = GetMoveDownCell(cell, 1);

            //        gradCell1.CellFormat.VerticalMerge = CellMerge.Previous;
            //        gradCell1 = GetMoveDownCell(gradCell1, 1);

            //        gradCell2.CellFormat.VerticalMerge = CellMerge.Previous;
            //        gradCell2 = GetMoveDownCell(gradCell2, 1);
            //    }
            //    while (cell != null);
            //}



        }

        private void WriteDomain(Cell cell, DomainRow domain)
        {
            Row row = cell.ParentRow;
            int shift = row.IndexOf(cell) + 1;
            int fields = 3;
            foreach (SemesterData sems in domain.Scores.Keys)
            {
                ScoreData data = domain.Scores[sems];
                int index = -1;
                if (_map.SemesterMapping.ContainsKey(sems))
                    index = _map.SemesterMapping[sems];

                if (index < 0) continue;

                index = index * fields + shift;
                WordHelper.Write(row.Cells[index], GetPeriodCredit(data.GetPeriodCredit()), _builder);
                WordHelper.Write(row.Cells[index + 1], data.Score, _builder);
                WordHelper.Write(row.Cells[index + 2], GetDegree(data.Score), _builder);
            }
        }

        private string GetPeriodCredit(string orig)
        {
            JHSchool.Evaluation.PeriodCredit pc = new JHSchool.Evaluation.PeriodCredit();
            pc.Parse(orig);
            if (PrintPeriod && PrintCredit)
                return pc.ToString();
            else if (PrintPeriod)
                return "" + pc.Period;
            else if (PrintCredit)
                return "" + pc.Credit;
            else
                return string.Empty;
        }

        private string GetDegree(string p)
        {
            decimal d;
            if (decimal.TryParse(p, out d))
                return DegreeMapper.GetDegreeByScore(d);
            else
                return p;
        }    


        /// <summary>
        /// 垂直合併欄位
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
        private void MergeVerticalCell(Cell cell, int count)
        {
            count--;
            cell.CellFormat.VerticalMerge = CellMerge.First;

            for (int i = 0; i < count; i++)
            {
                cell = GetMoveDownCell(cell, 1);
                cell.CellFormat.VerticalMerge = CellMerge.Previous;
            }
        }

        /// <summary>
        /// 取得往下移動位置
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Cell GetMoveDownCell(Cell cell, int count)
        {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row) + count;

            try
            {
                return table.Rows[row_index].Cells[col_index];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 取得往右移動位置
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Cell GetMoveRightCell(Cell cell, int count)
        {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row);

            try
            {
                return table.Rows[row_index].Cells[col_index + count];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        ///// <summary>
        ///// 寫入科目
        ///// </summary>
        ///// <param name="cell"></param>
        ///// <param name="subject"></param>
        //private void WriteSubject(Cell cell, SubjectRow subject)
        //{
        //    Write(cell, subject.Subject);
        //    Row row = cell.ParentRow;
        //    int shift = row.IndexOf(cell) + 1;
        //    int fields = 3;
        //    foreach (SemesterData sems in subject.Scores.Keys)
        //    {
        //        ScoreData data = subject.Scores[sems];
        //        int index = -1;
        //        if (_map.SemesterMapping.ContainsKey(sems))
        //            index = _map.SemesterMapping[sems];

        //        if (index < 0) continue;

        //        index = index * fields + shift;

        //        PeriodCredit pcObj = new PeriodCredit();
        //        pcObj.Parse(data.GetPeriodCredit());
        //        string pcDisplay = string.Empty;
        //        if (PrintPeriod && PrintCredit)
        //            pcDisplay = "" + pcObj.Period + "/" + pcObj.Credit;
        //        else if (PrintPeriod)
        //            pcDisplay = "" + pcObj.Period;
        //        else if (PrintCredit)
        //            pcDisplay = "" + pcObj.Credit;

        //        Write(row.Cells[index], pcDisplay);
        //        Write(row.Cells[index + 1], data.Score);
        //        Write(row.Cells[index + 2], data.GetDegree());
        //    }
        //}

        private void WriteSubject(Cell cell, Dictionary<SemesterData, ScoreData> dictionary)
        {
            Row row = cell.ParentRow;
            int shift = row.IndexOf(cell) + 1;
            int fields = 3;
            foreach (SemesterData sems in dictionary.Keys)
            {
                ScoreData data = dictionary[sems];
                int index = -1;
                if (_map.SemesterMapping.ContainsKey(sems))
                    index = _map.SemesterMapping[sems];

                if (index < 0) continue;

                index = index * fields + shift;
                WordHelper.Write(row.Cells[index], GetPeriodCredit(data.GetPeriodCredit()), _builder);
                WordHelper.Write(row.Cells[index + 1], data.Score, _builder);
                WordHelper.Write(row.Cells[index + 2], GetDegree(data.Score), _builder);
            }
        }

        /// <summary>
        /// 寫入資料
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="text"></param>
        private void Write(Cell cell, string text)
        {
            if (cell.FirstParagraph == null)
                cell.Paragraphs.Add(new Paragraph(cell.Document));
            cell.FirstParagraph.Runs.Clear();
            _run.Text = text;
            cell.FirstParagraph.Runs.Add(_run.Clone(true));
        }
    }


    class DomainRowManager
    {
        private Dictionary<string, DomainRow> _domains;

        private string _type;

        public DomainRowManager(string type)
        {
            _domains = new Dictionary<string, DomainRow>();

            _type = type;
        }

        public List<DomainRow> DomainRows
        {
            get
            {
                List<DomainRow> rows = new List<DomainRow>(_domains.Values);
                List<DomainRow> retVal = new List<DomainRow>();
                //foreach (string key in _config.Keys)
                //{
                //    if (_domains.ContainsKey(key))
                //        rows.Add(_domains[key]);
                //    else
                //        rows.Add(new DomainRow(key));
                //}
                List<string> sortList = (from x in rows select x.Domain).ToList();
                sortList.Sort(DomainSorter.Sort1);

                foreach (string str in sortList)
                    foreach (DomainRow dr in rows)
                        if (str == dr.Domain)
                            retVal.Add(dr);
                //return rows;
                return retVal;
            }
        }

        public void Add(JHSemesterScoreRecord record)
        {
            foreach (K12.Data.DomainScore domain in record.Domains.Values)
            {
                //if (!_config.ContainsKey(domain.Domain)) continue;

                if (!_domains.ContainsKey(domain.Domain))
                    _domains.Add(domain.Domain, new DomainRow(domain.Domain));

                DomainRow row = _domains[domain.Domain];

                row.Add(new SemesterData("" + record.SchoolYear, "" + record.Semester), domain);
            }

            foreach (K12.Data.SubjectScore subject in record.Subjects.Values)
            {
                //if (!_config.ContainsKey(subject.Domain)) continue;
                //if (_type == "Domain")
                //{
                //    if (!_config[subject.Domain].Contains(subject.Subject)) continue;
                //}

                if (!_domains.ContainsKey(subject.Domain))
                    _domains.Add(subject.Domain, new DomainRow(subject.Domain));

                DomainRow row = _domains[subject.Domain];
                row.Add(new SemesterData("" + record.SchoolYear, "" + record.Semester), subject);
            }
        }
    }


    // 舊寫法先註解
    //class DomainRowManager
    //{
    //    private List<string> _sortOrder;
    //    private Dictionary<string, DomainRow> _domains;

    //    public DomainRowManager()
    //    {
    //        _sortOrder = new List<string>();
    //        _sortOrder.AddRange(JHSchool.Evaluation.Domain.SelectGeneral());
    //        _sortOrder.Add("彈性課程");
    //        _sortOrder.AddRange(JHSchool.Evaluation.Domain.SelectSpecial());
    //        _sortOrder.Add("");
    //        _domains = new Dictionary<string, DomainRow>();
    //    }

    //    public List<DomainRow> DomainRows
    //    {
    //        get
    //        {
    //            List<DomainRow> rows = new List<DomainRow>();
    //            foreach (string key in _sortOrder)
    //            {
    //                if (_domains.ContainsKey(key))
    //                    rows.Add(_domains[key]);
    //            }
    //            return rows;
    //        }
    //    }

    //    public void Add(JHSemesterScoreRecord record)
    //    {
    //        foreach (K12.Data.SubjectScore subject in record.Subjects.Values)
    //        {
    //            if (!_domains.ContainsKey(subject.Domain))
    //                _domains.Add(subject.Domain, new DomainRow(subject.Domain));

    //            DomainRow row = _domains[subject.Domain];

    //            SubjectRow srow = row.GetSubjectRow(subject.Subject);
    //            srow.Add(new SemesterData("" + record.SchoolYear, "" + record.Semester), subject);
    //        }

    //        foreach (K12.Data.DomainScore domain in record.Domains.Values)
    //        {
    //            if (!_domains.ContainsKey(domain.Domain))
    //                _domains.Add(domain.Domain, new DomainRow(domain.Domain));

    //            DomainRow row = _domains[domain.Domain];

    //            SubjectRow srow = row.GetSubjectRow("領域成績");
    //            srow.Add(new SemesterData("" + record.SchoolYear, "" + record.Semester), domain);
    //        }
    //    }
    //}

    class DomainRow
    {
        public string Domain { get; private set; }

        private Dictionary<SemesterData, ScoreData> _scores;
        public Dictionary<SemesterData, ScoreData> Scores { get { return _scores; } }

        private Dictionary<string, Dictionary<SemesterData, ScoreData>> _subjectScores;
        public Dictionary<string, Dictionary<SemesterData, ScoreData>> SubjectScores { get { return _subjectScores; } }

        public DomainRow(string domain)
        {
            Domain = domain;
            _scores = new Dictionary<SemesterData, ScoreData>();
            _subjectScores = new Dictionary<string, Dictionary<SemesterData, ScoreData>>();
        }

        public bool Add(SemesterData semester, K12.Data.DomainScore domain)
        {
            if (!_scores.ContainsKey(semester))
            {
                _scores.Add(semester, new ScoreData("" + domain.Period, "" + domain.Credit, "" + domain.Score));
                return true;
            }
            return false;
        }

        public bool Add(SemesterData semester, K12.Data.SubjectScore subject)
        {
            if (!_subjectScores.ContainsKey(subject.Subject))
                _subjectScores.Add(subject.Subject, new Dictionary<SemesterData, ScoreData>());
            if (!_subjectScores[subject.Subject].ContainsKey(semester))
            {
                _subjectScores[subject.Subject].Add(semester, new ScoreData("" + subject.Period, "" + subject.Credit, "" + subject.Score));
                return true;
            }
            return false;
        }
    }

    // 舊寫法先註解
    //class DomainRow
    //{
    //    public string Domain { get; private set; }
    //    private Dictionary<string, SubjectRow> _subjects;
    //    public List<SubjectRow> SubjectRows
    //    {
    //        get
    //        {
    //            List<SubjectRow> rows = new List<SubjectRow>();
    //            SubjectRow last = null;
    //            foreach (string name in _subjects.Keys)
    //            {
    //                if (name == "領域成績") last = _subjects[name];
    //                else rows.Add(_subjects[name]);
    //            }
    //            if (last != null) rows.Insert(rows.Count, last);
    //            return rows;
    //        }
    //    }

    //    public int Count
    //    {
    //        get { return _subjects.Count; }
    //    }

    //    public DomainRow(string domain)
    //    {
    //        Domain = domain;
    //        _subjects = new Dictionary<string, SubjectRow>();
    //    }

    //    public bool Add(SubjectRow row)
    //    {
    //        if (!_subjects.ContainsKey(row.Subject))
    //        {
    //            _subjects.Add(row.Subject, row);
    //            return true;
    //        }
    //        return false;
    //    }

    //    internal SubjectRow GetSubjectRow(string subject)
    //    {
    //        if (!_subjects.ContainsKey(subject))
    //            _subjects.Add(subject, new SubjectRow(subject));
    //        return _subjects[subject];
    //    }
    //}

    class SubjectRow
    {
        public string Subject { get; private set; }
        private Dictionary<SemesterData, ScoreData> _scores;

        public Dictionary<SemesterData, ScoreData> Scores
        {
            get { return _scores; }
        }

        public SubjectRow(string subject)
        {
            Subject = subject;
            _scores = new Dictionary<SemesterData, ScoreData>();
        }

        public bool Add(SemesterData semester, K12.Data.DomainScore domain)
        {
            if (!_scores.ContainsKey(semester))
            {
                _scores.Add(semester, new ScoreData("" + domain.Period, "" + domain.Credit, "" + domain.Score));
                return true;
            }
            return false;
        }

        public bool Add(SemesterData semester, K12.Data.SubjectScore subject)
        {
            if (!_scores.ContainsKey(semester))
            {
                _scores.Add(semester, new ScoreData("" + subject.Period, "" + subject.Credit, "" + subject.Score));
                return true;
            }
            return false;
        }
    }


    class ScoreData
    {
        public string Period { get; private set; }
        public string Credit { get; private set; }
        public string Score { get; private set; }

        public ScoreData(string period, string credit, string score)
        {
            Period = period;
            Credit = credit;

            bool fix_need = score.Contains(".");
            while (fix_need)
            {
                if (score.EndsWith("0"))
                {
                    score = score.Substring(0, score.Length - 1);
                    fix_need = score.EndsWith("0");
                }
                else
                    fix_need = false;

                if (score.EndsWith("."))
                {
                    score = score.Substring(0, score.Length - 1);
                    fix_need = false;
                }
            }

            Score = score;
        }

        public string GetPeriodCredit()
        {
            return Period + "/" + Credit;
        }

        public string GetDegree()
        {
            decimal d;
            if (!decimal.TryParse(Score, out d))
                return string.Empty;
            else
            {
                if (d >= 90) return "優";
                else if (d >= 80) return "甲";
                else if (d >= 70) return "乙";
                else if (d >= 60) return "丙";
                else return "丁";
            }
        }
    }

    class WordHelper
    {
        internal static Run CreateRun(DocumentBuilder builder)
        {
            Run run = new Run(builder.Document);
            run.Font.Name = builder.Font.Name;
            run.Font.Size = builder.Font.Size;
            run.Text = string.Empty;
            return run;
        }

        private static Run run;

        internal static void Write(Cell cell, string text, DocumentBuilder builder)
        {
            if (cell.FirstParagraph == null)
                cell.Paragraphs.Add(new Paragraph(cell.Document));

            builder.MoveTo(cell.FirstParagraph);
            run = new Run(builder.Document);
            run.Font.Name = builder.Font.Name;
            run.Font.Size = builder.Font.Size;
            run.Text = text;

            cell.FirstParagraph.Runs.Clear();
            cell.FirstParagraph.Runs.Add(run.Clone(true));
        }

        internal static void MergeVerticalCell(Cell cell, int count)
        {
            count--;
            cell.CellFormat.VerticalMerge = CellMerge.First;

            for (int i = 0; i < count; i++)
            {
                cell = GetMoveDownCell(cell, 1);
                cell.CellFormat.VerticalMerge = CellMerge.Previous;
            }
        }

        internal static void MergeHorizontalCell(Cell cell, int count)
        {
            count--;
            cell.CellFormat.HorizontalMerge = CellMerge.First;

            for (int i = 0; i < count; i++)
            {
                cell = GetMoveRightCell(cell, 1);
                cell.CellFormat.HorizontalMerge = CellMerge.Previous;
            }
        }

        internal static Cell GetMoveDownCell(Cell cell, int count)
        {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row) + count;

            try
            {
                return table.Rows[row_index].Cells[col_index];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static Cell GetMoveLeftCell(Cell cell, int count)
        {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row);

            try
            {
                return table.Rows[row_index].Cells[col_index - count];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static Cell GetMoveRightCell(Cell cell, int count)
        {
            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row);

            try
            {
                return table.Rows[row_index].Cells[col_index + count];
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }

}
