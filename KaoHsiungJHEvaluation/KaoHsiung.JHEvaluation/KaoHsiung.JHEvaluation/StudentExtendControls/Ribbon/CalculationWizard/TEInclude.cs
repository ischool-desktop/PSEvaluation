using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using JHSchool.Evaluation.Feature.Legacy;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard
{
    class TEInclude
    {
        private string _identity, _exam_id, _exam_template_id, _exam_name;
        private bool _input_required, _use_score, _use_effort;
        private float _weight;

        public TEInclude(XmlElement data)
        {
            DSXmlHelper obj = new DSXmlHelper(data);
            _identity = obj.GetText("@ID");
            _exam_id = obj.GetText("RefExamID");
            _exam_template_id = obj.GetText("ExamTemplateID");
            _input_required = (obj.GetText("InputRequired") == "是" ? true : false);
            _exam_name = obj.GetText("ExamName");
            _use_score = (obj.GetText("UseScore") == "是" ? true : false);
            _use_effort = (obj.GetText("Extension/Extension/UseEffort") == "是" ? true : false);

            _weight = 0;
            float.TryParse(obj.GetText("Weight"), out _weight);
        }

        public string Identity
        {
            get { return _identity; }
        }

        public string ExamId
        {
            get { return _exam_id; }
        }

        public string ExamTemplateId
        {
            get { return _exam_template_id; }
        }

        public bool InputRequired
        {
            get { return _input_required; }
        }

        public float Weight
        {
            get { return _weight; }
        }

        public string ExamName
        {
            get { return _exam_name; }
        }

        public bool UseScore
        {
            get { return _use_score; }
        }

        public bool UseEffort
        {
            get { return _use_effort; }
        }

        public static TEIncludeCollection GetTEIncludes()
        {
            XmlElement xmlRecords = QueryTemplate.GetIncludeExamList();

            TEIncludeCollection includes = new TEIncludeCollection();
            foreach (XmlElement each in xmlRecords.SelectNodes("IncludeExam"))
            {
                TEInclude include = new TEInclude(each);
                includes.Add(include.Identity, include);
            }

            return includes;
        }
    }

    class TEIncludeCollection : Dictionary<string, TEInclude>
    {
    }
}
