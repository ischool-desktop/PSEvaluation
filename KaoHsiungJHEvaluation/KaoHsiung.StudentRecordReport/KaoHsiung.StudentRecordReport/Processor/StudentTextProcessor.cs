using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;
using JHSchool.Data;

namespace KaoHsiung.StudentRecordReport.Processor
{
    class StudentTextProcessor
    {
        private DomainTextManager _manager;
        private Cell _cell;
        private SemesterMap _map = new SemesterMap();
        private DocumentBuilder _builder;

        public StudentTextProcessor(DocumentBuilder builder, SemesterMap map)
        {
            _builder = builder;
            _builder.MoveToMergeField("學習領域評量");
            _cell = _builder.CurrentParagraph.ParentNode as Cell;
            _manager = new DomainTextManager();
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
            Row row = _cell.ParentRow;
            Table table = row.ParentTable;
            int deleteIndex = table.IndexOf(row);
            int rowIndex = table.IndexOf(row);

            foreach (DomainText domainText in _manager.DomainTexts)
            {
                List<SemesterData> semsList = new List<SemesterData>(domainText.Texts.Keys);
                semsList.Sort();

                foreach (SemesterData sems in semsList)
                {
                    
                    if (!_map.SemesterMapping.ContainsKey(sems)) continue;
                    // 修改成當有資料才列印
                    if (!string.IsNullOrEmpty(domainText.Texts[sems]))
                    {
                        table.Rows.Add(row.Clone(true));
                        Row tempRow = table.LastRow;


                        WordHelper.Write(tempRow.Cells[0], domainText.Domain, _builder);
                        WordHelper.Write(tempRow.Cells[1], GetGradeYearString(_map.SemesterMapping[sems]), _builder);
                        WordHelper.Write(tempRow.Cells[2], domainText.Texts[sems], _builder);
                    }
                    //int index = -1;
                    //if (_map.SemesterMapping.ContainsKey(sems))
                    //    index = _map.SemesterMapping[sems];
                    //if (index < 0) continue;
                    //Cell currentCell = GetSemesterCell(cell, index);
                    //Write(currentCell, domainText.Texts[sems]);
                }
            }

            table.Rows[deleteIndex].Remove();
        }

        private string GetGradeYearString(int index)
        {
            string semester = string.Empty;
            if (index % 2 == 0)
                semester = "上";
            else
                semester = "下";
            
            if (index <2) return "一" + semester;
            else if (index < 4) return "二" + semester;
            else if (index < 6) return "三" + semester;
            else return "";
        }
    }

    class DomainTextManager
    {
        
        private Dictionary<string, DomainText> _domains;
        //private List<string> _removeDomains;

        public List<DomainText> DomainTexts
        {
            get
            {
                List<DomainText> rows = new List<DomainText>(_domains.Values);

                //foreach (string domain in _config.Keys)
                //{
                //    if (_config[domain].Count > 0)
                //    {
                //        foreach (string subject in _config[domain])
                //        {
                //            if (_domains.ContainsKey(subject))
                //                rows.Add(_domains[subject]);
                //        }
                //    }
                //    else
                //    {
                //        if (_domains.ContainsKey(domain))
                //            rows.Add(_domains[domain]);
                //    }
                //}
                return rows;
            }
        }

        public DomainTextManager()
        {
            
            _domains = new Dictionary<string, DomainText>();
            //_removeDomains = new List<string>();
        }

        public void Add(JHSemesterScoreRecord record)
        {
            SemesterData semester = new SemesterData("" + record.SchoolYear, "" + record.Semester);

            //foreach (K12.Data.SubjectScore subject in record.Subjects.Values)
            //{
                //if (!_config.ContainsKey(subject.Domain)) continue;

                //if (!_config[subject.Domain].Contains(subject.Subject)) continue;
                //else
                //{
                //    if (!_removeDomains.Contains(subject.Domain))
                //        _removeDomains.Add(subject.Domain);
                //}

            //    if (!_domains.ContainsKey(subject.Subject))
            //        _domains.Add(subject.Subject, new DomainText(subject.Subject));

            //    DomainText text = _domains[subject.Subject];
            //    text.Add(semester, subject.Text);
            //}

            foreach (K12.Data.DomainScore domain in record.Domains.Values)
            {
                //if (!_config.ContainsKey(domain.Domain)) continue;
                //if (_removeDomains.Contains(domain.Domain)) continue;

                if (!_domains.ContainsKey(domain.Domain))
                    _domains.Add(domain.Domain, new DomainText(domain.Domain));

                DomainText text = _domains[domain.Domain];
                text.Add(semester, domain.Text);
            }
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
