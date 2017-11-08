using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation
{
    public class AEIncludeRecord
    {
        public string RefAssessmentSetupID { get; private set; }
        public string RefExamID { get; private set; }
        public string ID { get; private set; }
        public string Name { get; private set; }

        public bool UseText { get; private set; }
        public bool UseScore { get; private set; }
        public bool UseEffort { get; private set; }

        public int Weight { get; private set; }
        public string StartTime { get; private set; }
        public string EndTime { get; private set; }

        /// <summary>
        /// 取得試別樣版
        /// </summary>
        public AssessmentSetupRecord AssessmentSetup { get { return JHSchool.Evaluation.AssessmentSetup.Instance.Items[RefAssessmentSetupID]; } }
        /// <summary>
        /// 取得試別
        /// </summary>
        public ExamRecord Exam { get { return JHSchool.Evaluation.Exam.Instance.Items[RefExamID]; } }

        public AEIncludeRecord(XmlElement data)
        {
            DSXmlHelper helper = new DSXmlHelper(data);

            RefAssessmentSetupID = helper.GetText("ExamTemplateID");
            RefExamID = helper.GetText("RefExamID");

            ID = helper.GetText("@ID");
            Name = helper.GetText("ExamName");
            UseText = (helper.GetText("Extension/Extension/UseText") == "是") ? true : false;
            UseScore = (helper.GetText("UseScore") == "是") ? true : false;
            
            int i = 0;
            Weight = int.TryParse(helper.GetText("Weight"), out i) ? i : 0;
            StartTime = helper.GetText("StartTime");
            EndTime = helper.GetText("EndTime");

            UseEffort = (helper.GetText("Extension/Extension/UseEffort") == "是") ? true : false;
        }
    }
}
