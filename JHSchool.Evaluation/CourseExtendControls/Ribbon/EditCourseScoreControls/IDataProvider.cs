using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FISCA.DSAUtil;
using System.Xml;
using JHSchool.Evaluation.Feature.Legacy;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    interface IDataProvider
    {
        ColumnHeader ColumnHeader { get;}
        RowCollection Rows { get;}
        ICell FindCell(string studentid, string columnName);
    }

    class TestDataProvider : IDataProvider
    {
        private ColumnHeader _columnHeader = null;
        private RowCollection _rowCollection = null;

        //public TestDataProvider()
        //{
        //    #region 塞資料
        //    _columnHeader = new ColumnHeader();
        //    _columnHeader.Columns.Add(new ColumnSetting("1","班級", 70));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "座號", 65));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "姓名", 70));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "學號", 70));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "成績1", 80));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "成績2", 80));

        //    _rowCollection = new RowCollection();

        //    RowEntity row = new RowEntity("1");            
        //    row.AddCell("班級", new StudentCell("資一忠"));
        //    row.AddCell("座號", new StudentCell("1"));
        //    row.AddCell("姓名", new StudentCell("路西華"));
        //    row.AddCell("學號", new StudentCell("84203226"));        
        //    row.AddCell("成績1", new ExamCell("100"));   
        //    row.AddCell("成績2", new ExamCell("100"));
        //    _rowCollection.Add("1", row);

        //    row = new RowEntity("2");
        //    row.AddCell("班級", new StudentCell("資一忠"));
        //    row.AddCell("座號", new StudentCell("2"));
        //    row.AddCell("姓名", new StudentCell("有紅痔"));
        //    row.AddCell("學號", new StudentCell("8825252"));
        //    row.AddCell("成績1", new ExamCell("90"));
        //    row.AddCell("成績2", new ExamCell("90"));
        //    _rowCollection.Add("2", row);

        //    row = new RowEntity("3");
        //    row.AddCell("班級", new StudentCell("資一忠"));
        //    row.AddCell("座號", new StudentCell("3"));
        //    row.AddCell("姓名", new StudentCell("君士坦丁"));
        //    row.AddCell("學號", new StudentCell("08009786"));
        //    row.AddCell("成績1", new ExamCell("30"));
        //    row.AddCell("成績2", new ExamCell("50"));
        //    _rowCollection.Add("3", row);

        //    row = new RowEntity("4");
        //    row.AddCell("班級", new StudentCell("資一忠"));
        //    row.AddCell("座號", new StudentCell("4"));
        //    row.AddCell("姓名", new StudentCell("赫丘力"));
        //    row.AddCell("學號", new StudentCell("08000800"));
        //    row.AddCell("成績1", new ExamCell("10"));
        //    row.AddCell("成績2", new ExamCell("10"));
        //    _rowCollection.Add("4", row);

        //    row = new RowEntity("5");
        //    row.AddCell("班級", new StudentCell("資一忠"));
        //    row.AddCell("座號", new StudentCell("5"));
        //    row.AddCell("姓名", new StudentCell("路西華"));
        //    row.AddCell("學號", new StudentCell("88880000"));
        //    row.AddCell("成績1", new ExamCell("30"));
        //    row.AddCell("成績2", new ExamCell("80"));
        //    _rowCollection.Add("5", row);

        //    #endregion
        //}

        #region IDataProvider 成員

        public ColumnHeader ColumnHeader
        {
            get { return _columnHeader; }
        }

        public RowCollection Rows
        {
            get { return _rowCollection; }
        }

        public ICell FindCell(string studentid, string columnName)
        {
            return _rowCollection.FindCell(studentid, columnName);
        }

        #endregion
    }

    class SmartSchoolDataProvider : IDataProvider
    {
        private string _courseid;
        private ColumnHeader _columnHeader;
        private RowCollection _rowCollection;

        public SmartSchoolDataProvider(string courseid)
        {
            _courseid = courseid;
            DSResponse dsrsp = QueryCourse.GetCourseExam(courseid);
            DSXmlHelper helper = dsrsp.GetContent();
            Dictionary<string, string> examList = new Dictionary<string, string>();
            // 塞 Header
            _columnHeader = new ColumnHeader();
            _columnHeader.Columns.Add(new ColumnSetting("-1", "班級", 60));
            _columnHeader.Columns.Add(new ColumnSetting("-1", "座號", 40, new DecimalComparer()));
            _columnHeader.Columns.Add(new ColumnSetting("-1", "姓名", 70));
            _columnHeader.Columns.Add(new ColumnSetting("-1", "學號", 70));
            
            foreach (XmlNode node in helper.GetElements("Course"))
            {
                string examName = node.SelectSingleNode("ExamName").InnerText;
                string examid = node.SelectSingleNode("RefExamID").InnerText;

                _columnHeader.Columns.Add(new ColumnSetting(examid, examName, 80, new DecimalComparer()));
                examList.Add(examid, examName);
            }
            _columnHeader.Columns.Add(new ColumnSetting("-1", "課程成績", 100, new DecimalComparer()));


            // 取回本課程所有學生的考試成績
            DSResponse scoreResp = QueryCourse.GetSECTake(courseid);
            XmlElement scoreElement = scoreResp.GetContent().GetElement(".");


            // 塞 Row
            _rowCollection = new RowCollection();
            dsrsp = QueryCourse.GetSCAttend(courseid);
            helper = dsrsp.GetContent();

            foreach (XmlNode node in helper.GetElements("Student"))
            {
                RowEntity row = new RowEntity(node.Attributes["ID"].Value);
                row.AddCell("班級", new StudentCell(node.SelectSingleNode("ClassName").InnerText));
                row.AddCell("座號", new StudentCell(node.SelectSingleNode("SeatNumber").InnerText));
                row.AddCell("姓名", new StudentCell(node.SelectSingleNode("Name").InnerText));
                row.AddCell("學號", new StudentCell(node.SelectSingleNode("StudentNumber").InnerText));
                foreach (string examid in examList.Keys)
                {
                    ScoreInfo score = GetScore(scoreElement, examid, node.Attributes["ID"].Value);
                    row.AddCell(examList[examid], new ExamCell(score));
                }
                row.AddCell("課程成績", new ScoreExamCell(new ScoreInfo(node.Attributes["ID"].Value,node.SelectSingleNode("Score").InnerText)));
                _rowCollection.Add(node.Attributes["ID"].Value, row);
            }
        }

        #region IDataProvider 成員

        public ColumnHeader ColumnHeader
        {
            get { return _columnHeader; }
        }

        public RowCollection Rows
        {
            get { return _rowCollection; }
        }

        public ICell FindCell(string studentid, string columnName)
        {
            return _rowCollection.FindCell(studentid, columnName);
        }

        #endregion

        private ScoreInfo GetScore(XmlElement element, string examid, string attendid)
        {
            foreach (XmlNode node in element.SelectNodes("Score"))
            {
                if (node.SelectSingleNode("ExamID").InnerText == examid && node.SelectSingleNode("AttendID").InnerText == attendid)
                    return new ScoreInfo(node.Attributes["ID"].Value, node.SelectSingleNode("Score").InnerText);
            }
            return new ScoreInfo();
        }
    }

    public class ScoreInfo
    {
        private string _score;

        public string Score
        {
            get { return _score; }
            set { _score = value; }
        }
        private string _id;

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public ScoreInfo(string id, string score)
        {
            _id = id;
            _score = score;
        }

        public ScoreInfo()
        {
            _id = "";
            _score = "";
        }
    }
}
