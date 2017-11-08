using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using JHSchool.Evaluation.Feature.Legacy;

namespace KaoHsiung.JHEvaluation.StudentExtendControls.Ribbon.CalculationWizard
{
    class ExamTemplate
    {
        private string _identity, _template_name;
        private TEIncludeCollection _teincludes;

        public ExamTemplate(XmlElement data)
        {
            DSXmlHelper objData = new DSXmlHelper(data);
            _identity = objData.GetText("@ID");
            _template_name = objData.GetText("TemplateName");
            _allow_upload = objData.GetText("AllowUpload") == "是" ? true : false;

            _teincludes = new TEIncludeCollection();
        }

        public string Identity
        {
            get { return _identity; }
        }

        public string TemplateName
        {
            get { return _template_name; }
        }

        private bool _allow_upload;
        /// <summary>
        /// SmartSchool  會議決議，此屬性改變意義為「課程成績來源」。
        /// True 為「教師提供」，False 為「學校計算」。
        /// </summary>
        public bool AllowUpload
        {
            get { return _allow_upload; }
        }

        public TEIncludeCollection TEIncludes
        {
            get { return _teincludes; }
        }

        public static ExamTemplateCollection GetExamTemplates()
        {
            XmlElement templates = QueryTemplate.GetAbstractList();

            ExamTemplateCollection objTemplates = new ExamTemplateCollection();
            foreach (XmlElement each in templates.SelectNodes("ExamTemplate"))
            {
                ExamTemplate template = new ExamTemplate(each);
                objTemplates.Add(template.Identity, template);
            }

            return objTemplates;
        }
    }

    class ExamTemplateCollection : Dictionary<string, ExamTemplate>
    {
    }
}
