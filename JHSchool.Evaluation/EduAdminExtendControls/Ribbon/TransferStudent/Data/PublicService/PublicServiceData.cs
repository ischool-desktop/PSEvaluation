using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.PublicService
{
    public class PublicServiceData
    {
        public List<PublicServiceItem> Items { get; set; }

        public PublicServiceData()
        {
            Items = new List<PublicServiceItem>();
        }

        public void SetItem(PublicServiceItem publicServiceItem)
        {
            PublicServiceItem item = GetItem(publicServiceItem.Name, publicServiceItem.SchoolYear, publicServiceItem.Semester);

            if (item == null)
                Items.Add(publicServiceItem);
            else            
                item.Description = publicServiceItem.Description;         
        }

        public PublicServiceItem GetItem(string name, string schoolyear, string semester)
        {
            foreach (PublicServiceItem item in Items)
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
            XmlElement element = doc.CreateElement("PublicService");

            foreach (PublicServiceItem item in Items)
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
