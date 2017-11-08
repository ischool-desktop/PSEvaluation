using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Framework;

namespace JHSchool.Evaluation
{
    //<SemesterMoralScore ID="19">
    //    <Name>黃靖婷</Name>
    //    <Semester>1</Semester>
    //    <RefStudentID>168708</RefStudentID>
    //    <TextScore>....一大堆東西</TextScore>
    //    <SchoolYear>96</SchoolYear>
    //</SemesterMoralScore>

    /// <summary>
    /// 學生日常生表現紀錄
    /// </summary>
    public class MoralScoreRecord
    {
        internal MoralScoreRecord(XmlElement element)
        {
            XmlHelper helper = new XmlHelper(element);

            RefStudentID = helper.GetString("RefStudentID");
            ID = helper.GetString("@ID");
            SchoolYear = helper.GetString("SchoolYear");
            Semester = helper.GetString("Semester");
            TextScore = helper.GetElement("TextScore");
        }

        internal string RefStudentID { get; private set; }            //學生編號
        public string ID { get; private set; }                                  //記錄編號
        public string SchoolYear { get; private set; }                  //學年度
        public string Semester { get; private set; }                     //學期
        public XmlElement TextScore { get; private set; }          //文字評量
    }
}
