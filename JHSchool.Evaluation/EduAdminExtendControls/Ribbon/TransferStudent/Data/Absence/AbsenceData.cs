using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.Absence;
using System.Xml;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Absence
{
    public class AbsenceData
    {
        public List<AbsenceItem> Items { get; set; }

        public AbsenceData()
        {
            Items = new List<AbsenceItem>();
        }

        public AbsenceItem GetItem(string schoolyear, string semester, string periodType, string name)
        {
            foreach (AbsenceItem item in Items)
            {
                if (item.SchoolYear == schoolyear && item.Semester == semester && item.PeriodType == periodType && item.Name == name)
                    return item;
            }
            return null;
        }

        public void SetItem(AbsenceItem absenceItem)
        {
            AbsenceItem item = GetItem(absenceItem.SchoolYear, absenceItem.Semester, absenceItem.PeriodType,absenceItem.Name);

            if (item == null)
                Items.Add(absenceItem);
            else
                item.Count = absenceItem.Count;
        }

        public XmlElement GetSemesterElement(string schoolyear, string semester)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("AttendanceStatistics");
            
            foreach (AbsenceItem item in Items)
            {
                if (item.SchoolYear != schoolyear || item.Semester != semester) continue;

                XmlElement absenceElement = doc.CreateElement("Absence");
                absenceElement.SetAttribute("Name", item.Name);
                absenceElement.SetAttribute("PeriodType", item.PeriodType);
                absenceElement.SetAttribute("Count", item.Count.ToString());

                element.AppendChild(absenceElement);
            }
            return element;
        }
    }
}
