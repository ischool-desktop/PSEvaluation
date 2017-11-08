using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words;
using JHSchool.Data;

namespace HsinChu.StudentRecordReport
{
    class StudentDomainTextProcessor
    {
        private DomainTextManager _manager;
        private Cell _cell;
        private Run _run;
        private SemesterMap _map = new SemesterMap();

        public StudentDomainTextProcessor(DocumentBuilder builder, SemesterMap map)
        {
            _cell = builder.CurrentParagraph.ParentNode as Cell;

            _manager = new DomainTextManager();
            _run = new Run(_cell.Document);
            _run.Font.Name = builder.Font.Name;
            _run.Font.Size = builder.Font.Size;
            _run.Text = string.Empty;
            _map = map;
        }

        public void SetData(List<JHSemesterScoreRecord> records)
        {
            foreach (JHSemesterScoreRecord record in records)
                _manager.Add(record);

            FillDomainText();
        }

        private void FillDomainText()
        {
            Cell cell = _cell;

            foreach (DomainText domainText in _manager.DomainTexts)
            {
                foreach (SemesterData sems in domainText.Texts.Keys)
                {
                    int index = -1;
                    if (_map.SemesterMapping.ContainsKey(sems))
                        index = _map.SemesterMapping[sems];
                    if (index < 0) continue;
                    Cell currentCell = GetSemesterCell(cell, index);
                    Write(currentCell, domainText.Texts[sems]);
                }

                cell = GetMoveDownCell(cell, 3);
                if (cell == null) break;
            }
        }

        private Cell GetSemesterCell(Cell cell, int count)
        {
            if (count % 2 == 0)
            {
                return GetMoveDownCell(cell, count / 2);
            }
            else
            {
                return GetMoveDownCell(GetMoveRightCell(cell, 2), count / 2);
            }
        }

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

        private void Write(Cell cell, string text)
        {
            if (cell.FirstParagraph == null)
                cell.Paragraphs.Add(new Paragraph(cell.Document));
            cell.FirstParagraph.Runs.Clear();
            _run.Text = text;
            cell.FirstParagraph.Runs.Add(_run.Clone(true));
        }
    }

    class DomainTextManager
    {
        private List<string> _sortOrder;
        private Dictionary<string, DomainText> _domains;
        public List<DomainText> DomainTexts
        {
            get
            {
                List<DomainText> rows = new List<DomainText>();
                foreach (string key in _sortOrder)
                {
                    if (_domains.ContainsKey(key))
                        rows.Add(_domains[key]);
                }
                return rows;
            }
        }

        public DomainTextManager()
        {
            _sortOrder = new List<string>();
            _sortOrder.AddRange(JHSchool.Evaluation.Domain.SelectGeneral());
            _sortOrder.Add("彈性課程");
            _sortOrder.AddRange(JHSchool.Evaluation.Domain.SelectSpecial());
            _domains = new Dictionary<string, DomainText>();
        }

        public void Add(JHSemesterScoreRecord record)
        {
            SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

            foreach (K12.Data.DomainScore domain in record.Domains.Values)
            {
                if (!_domains.ContainsKey(domain.Domain))
                    _domains.Add(domain.Domain, new DomainText(domain.Domain));

                DomainText text = _domains[domain.Domain];
                text.Add(semester, domain.Text);
            }

            // 加入彈性課程文字評量
            string strDomainName = "彈性課程";
            // 存彈性課程用
            List<string> strTextList = new List<string> ();
            foreach (K12.Data.SubjectScore subj in record.Subjects.Values)
            {               
                // 科目是彈性課程
                if (string.IsNullOrEmpty(subj.Domain))
                    strTextList.Add(subj.Text);
            }

                if (!_domains.ContainsKey(strDomainName))
                    _domains.Add(strDomainName, new DomainText(strDomainName));
                DomainText text1 = _domains[strDomainName];
                text1.Add(semester, string.Join(";",strTextList.ToArray ()));
            
        }
    }

    class DomainText
    {
        public string Domain { get; private set; }
        private Dictionary<SemesterData, string> _texts;
        public Dictionary<SemesterData, string> Texts
        {
            get { return _texts; }
        }

        public DomainText(string domain)
        {
            Domain = domain;
            _texts = new Dictionary<SemesterData, string>();
        }

        internal void Add(SemesterData semester, string text)
        {
            if (!_texts.ContainsKey(semester))
                _texts.Add(semester, string.Empty);

            if (!string.IsNullOrEmpty(text))
            {
                text = text.Replace('>', ':').Replace('<', ',');
                if (text.StartsWith(",")) text = text.Substring(1);
                _texts[semester] = text;
            }
        }
    }
}
