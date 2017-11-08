using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation
{
    public class ProgramPlanRecord
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public List<ProgramSubject> Subjects { get; private set; }

        public ProgramPlanRecord()
        {
            ID = string.Empty;
            Name = string.Empty;
            Subjects = new List<ProgramSubject>();
        }

        public ProgramPlanRecord(XmlElement data)
        {
            DSXmlHelper helper = new DSXmlHelper(data);
            ID = helper.GetText("@ID");
            Name = helper.GetText("Name");

            List<ProgramSubject> list = new List<ProgramSubject>();
            foreach (var subject in helper.GetElements("Content/GraduationPlan/Subject"))
                list.Add(new ProgramSubject(subject));
            Subjects = list;
        }
    }

    public class ProgramSubject : ICloneable
    {
        public string GradeYear { get; set; }
        public string Semester{ get; set; }
        public string Credit { get; set; }
        public string Period { get; set; }
        public string Domain { get; set; }
        public string FullName { get; set; }
        public string Level { get; set; }
        public string SubjectName { get; set; }
        public bool CalcFlag { get; set; }

        public int RowIndex { get; set; } //這是…

        public ProgramSubject(XmlElement element)
        {
            GradeYear = element.GetAttribute("GradeYear");
            Semester = element.GetAttribute("Semester");
            Credit = element.GetAttribute("Credit");
            Period = element.GetAttribute("Period");
            Domain = element.GetAttribute("Domain");
            FullName = element.GetAttribute("FullName");
            Level = element.GetAttribute("Level");
            SubjectName = element.GetAttribute("SubjectName");
            bool b;
            CalcFlag = bool.TryParse(element.GetAttribute("CalcFlag"), out b) ? b : true;

            RowIndex = int.Parse(element.SelectSingleNode("Grouping/@RowIndex").InnerText);

            //<Subject 
            //  Category="一般科目" Credit="1" Domain="外國語文" Entry=""
            //  FullName="生活美語Ⅰ" GradeYear="1" Level="1" 
            //  NotIncludedInCalc="false" NotIncludedInCredit="false" 
            //  Required="選修" RequiredBy="校訂" Semester="1" SubjectName="生活美語">
            //
            //<Grouping RowIndex="1" startLevel="1"/>
            //</Subject>
        }

        public ProgramSubject() { }

        #region ICloneable 成員

        public object Clone()
        {
            ProgramSubject newSubject = new ProgramSubject();
            newSubject.GradeYear = this.GradeYear;
            newSubject.Semester = this.Semester;
            newSubject.Credit = this.Credit;
            newSubject.Period = this.Period;
            newSubject.Domain = this.Domain;
            newSubject.FullName = this.FullName;
            newSubject.Level = this.Level;
            newSubject.SubjectName = this.SubjectName;
            newSubject.CalcFlag = this.CalcFlag;
            newSubject.RowIndex = this.RowIndex;
            return newSubject;
        }

        #endregion
    }
}
