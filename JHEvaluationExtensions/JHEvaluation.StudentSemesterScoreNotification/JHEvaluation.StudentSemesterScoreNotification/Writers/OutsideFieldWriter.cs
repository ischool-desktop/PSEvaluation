using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JHEvaluation.StudentSemesterScoreNotification.Writers
{
    internal class OutsideFieldWriter
    {
        private Options Options { get; set; }

        public OutsideFieldWriter(Options options)
        {
            Options = options;
        }

        public void Write(Aspose.Words.Document doc)
        {
            List<string> globalFieldName = new List<string>();
            List<object> globalFieldValue = new List<object>();

            globalFieldName.Add("學校名稱");
            globalFieldValue.Add(K12.Data.School.ChineseName);
            globalFieldName.Add("學年度");
            globalFieldValue.Add(Options.SchoolYear);
            globalFieldName.Add("學期");
            globalFieldValue.Add(Options.Semester);
            globalFieldName.Add("列印日期");
            globalFieldValue.Add(Global.CDate(DateTime.Now.ToString("yyyy/MM/dd")) + " " + DateTime.Now.ToString("HH:mm:ss"));

            string chancellor, eduDirector; //, stuDirector;
            chancellor = eduDirector = string.Empty; //stuDirector = string.Empty;

            XmlElement schoolInfo = K12.Data.School.Configuration["學校資訊"].PreviousData;
            XmlElement chancellorElement = (XmlElement)schoolInfo.SelectSingleNode("ChancellorChineseName");
            XmlElement eduDirectorElement = (XmlElement)schoolInfo.SelectSingleNode("EduDirectorName");
            //XmlElement stuDirectorElement = (XmlElement)schoolInfo.SelectSingleNode("StuDirectorName");

            if (chancellorElement != null) chancellor = chancellorElement.InnerText;
            if (eduDirectorElement != null) eduDirector = eduDirectorElement.InnerText;
            //if (stuDirectorElement != null) stuDirector = stuDirectorElement.InnerText;

            globalFieldName.Add("校長");
            globalFieldValue.Add(chancellor);
            globalFieldName.Add("教務主任");
            globalFieldValue.Add(eduDirector);

            doc.MailMerge.Execute(globalFieldName.ToArray(), globalFieldValue.ToArray());
        }
    }
}
