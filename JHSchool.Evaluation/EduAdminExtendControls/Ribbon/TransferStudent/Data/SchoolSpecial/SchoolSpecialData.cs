using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.SchoolSpecial
{
    public class SchoolSpecialData
    {
        public List<SchoolSpecialItem> Items { get; set; }

        public SchoolSpecialData()
        {
            Items = new List<SchoolSpecialItem>();
        }

        public void SetItem(SchoolSpecialItem schoolSepcialItem)
        {
            SchoolSpecialItem item = GetItem(schoolSepcialItem.Name, schoolSepcialItem.SchoolYear, schoolSepcialItem.Semester);

            if (item == null)
                Items.Add(schoolSepcialItem);
            else            
                item.Description = schoolSepcialItem.Description;         
        }

        public SchoolSpecialItem GetItem(string name, string schoolyear, string semester)
        {
            foreach (SchoolSpecialItem item in Items)
            {
                if (item.SchoolYear != schoolyear) continue;
                if (item.Semester != semester) continue;
                if (item.Name != name) continue;

                return item;
            }
            return null;
        }

        public XmlElement GetSemesterElement(string schoolyear, string semester)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("SchoolSpecial");

            foreach (SchoolSpecialItem item in Items)
            {
                if (item.SchoolYear != schoolyear || item.Semester != semester) continue;

                XmlElement itemElement = doc.CreateElement("Item");
                itemElement.SetAttribute("Name", item.Name);
                itemElement.SetAttribute("Description", item.Description);

                element.AppendChild(itemElement);
            }
            return element;
        }
    }
}
