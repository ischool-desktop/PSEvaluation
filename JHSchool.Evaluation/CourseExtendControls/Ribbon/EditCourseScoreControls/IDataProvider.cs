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
        //    #region ����
        //    _columnHeader = new ColumnHeader();
        //    _columnHeader.Columns.Add(new ColumnSetting("1","�Z��", 70));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "�y��", 65));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "�m�W", 70));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "�Ǹ�", 70));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "���Z1", 80));
        //    _columnHeader.Columns.Add(new ColumnSetting("1", "���Z2", 80));

        //    _rowCollection = new RowCollection();

        //    RowEntity row = new RowEntity("1");            
        //    row.AddCell("�Z��", new StudentCell("��@��"));
        //    row.AddCell("�y��", new StudentCell("1"));
        //    row.AddCell("�m�W", new StudentCell("�����"));
        //    row.AddCell("�Ǹ�", new StudentCell("84203226"));        
        //    row.AddCell("���Z1", new ExamCell("100"));   
        //    row.AddCell("���Z2", new ExamCell("100"));
        //    _rowCollection.Add("1", row);

        //    row = new RowEntity("2");
        //    row.AddCell("�Z��", new StudentCell("��@��"));
        //    row.AddCell("�y��", new StudentCell("2"));
        //    row.AddCell("�m�W", new StudentCell("������"));
        //    row.AddCell("�Ǹ�", new StudentCell("8825252"));
        //    row.AddCell("���Z1", new ExamCell("90"));
        //    row.AddCell("���Z2", new ExamCell("90"));
        //    _rowCollection.Add("2", row);

        //    row = new RowEntity("3");
        //    row.AddCell("�Z��", new StudentCell("��@��"));
        //    row.AddCell("�y��", new StudentCell("3"));
        //    row.AddCell("�m�W", new StudentCell("�g�h�Z�B"));
        //    row.AddCell("�Ǹ�", new StudentCell("08009786"));
        //    row.AddCell("���Z1", new ExamCell("30"));
        //    row.AddCell("���Z2", new ExamCell("50"));
        //    _rowCollection.Add("3", row);

        //    row = new RowEntity("4");
        //    row.AddCell("�Z��", new StudentCell("��@��"));
        //    row.AddCell("�y��", new StudentCell("4"));
        //    row.AddCell("�m�W", new StudentCell("���C�O"));
        //    row.AddCell("�Ǹ�", new StudentCell("08000800"));
        //    row.AddCell("���Z1", new ExamCell("10"));
        //    row.AddCell("���Z2", new ExamCell("10"));
        //    _rowCollection.Add("4", row);

        //    row = new RowEntity("5");
        //    row.AddCell("�Z��", new StudentCell("��@��"));
        //    row.AddCell("�y��", new StudentCell("5"));
        //    row.AddCell("�m�W", new StudentCell("�����"));
        //    row.AddCell("�Ǹ�", new StudentCell("88880000"));
        //    row.AddCell("���Z1", new ExamCell("30"));
        //    row.AddCell("���Z2", new ExamCell("80"));
        //    _rowCollection.Add("5", row);

        //    #endregion
        //}

        #region IDataProvider ����

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
            // �� Header
            _columnHeader = new ColumnHeader();
            _columnHeader.Columns.Add(new ColumnSetting("-1", "�Z��", 60));
            _columnHeader.Columns.Add(new ColumnSetting("-1", "�y��", 40, new DecimalComparer()));
            _columnHeader.Columns.Add(new ColumnSetting("-1", "�m�W", 70));
            _columnHeader.Columns.Add(new ColumnSetting("-1", "�Ǹ�", 70));
            
            foreach (XmlNode node in helper.GetElements("Course"))
            {
                string examName = node.SelectSingleNode("ExamName").InnerText;
                string examid = node.SelectSingleNode("RefExamID").InnerText;

                _columnHeader.Columns.Add(new ColumnSetting(examid, examName, 80, new DecimalComparer()));
                examList.Add(examid, examName);
            }
            _columnHeader.Columns.Add(new ColumnSetting("-1", "�ҵ{���Z", 100, new DecimalComparer()));


            // ���^���ҵ{�Ҧ��ǥͪ��Ҹզ��Z
            DSResponse scoreResp = QueryCourse.GetSECTake(courseid);
            XmlElement scoreElement = scoreResp.GetContent().GetElement(".");


            // �� Row
            _rowCollection = new RowCollection();
            dsrsp = QueryCourse.GetSCAttend(courseid);
            helper = dsrsp.GetContent();

            foreach (XmlNode node in helper.GetElements("Student"))
            {
                RowEntity row = new RowEntity(node.Attributes["ID"].Value);
                row.AddCell("�Z��", new StudentCell(node.SelectSingleNode("ClassName").InnerText));
                row.AddCell("�y��", new StudentCell(node.SelectSingleNode("SeatNumber").InnerText));
                row.AddCell("�m�W", new StudentCell(node.SelectSingleNode("Name").InnerText));
                row.AddCell("�Ǹ�", new StudentCell(node.SelectSingleNode("StudentNumber").InnerText));
                foreach (string examid in examList.Keys)
                {
                    ScoreInfo score = GetScore(scoreElement, examid, node.Attributes["ID"].Value);
                    row.AddCell(examList[examid], new ExamCell(score));
                }
                row.AddCell("�ҵ{���Z", new ScoreExamCell(new ScoreInfo(node.Attributes["ID"].Value,node.SelectSingleNode("Score").InnerText)));
                _rowCollection.Add(node.Attributes["ID"].Value, row);
            }
        }

        #region IDataProvider ����

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
