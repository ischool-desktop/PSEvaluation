using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.DailyBehavior;
using System.Xml;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.DailyBehavior
{
    public class DailyBehaviorData
    {
        public List<DailyBehaviorItem> Items { get; set; }

        public DailyBehaviorData()
        {
            Items = new List<DailyBehaviorItem>();
        }

        public void SetItem(DailyBehaviorItem dailyBehaviorItem)
        {
            DailyBehaviorItem item = GetItem(dailyBehaviorItem.Name, dailyBehaviorItem.SchoolYear, dailyBehaviorItem.Semester);

            if (item == null)
                Items.Add(dailyBehaviorItem);
            else
            {
                //item.Index = dailyBehaviorItem.Index;
                item.Degree = dailyBehaviorItem.Degree;
            }
        }

        public DailyBehaviorItem GetItem(string name, string schoolyear, string semester)
        {
            foreach (DailyBehaviorItem item in Items)
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
            XmlElement element = doc.CreateElement("DailyBehavior");

            foreach (DailyBehaviorItem item in Items)
            {
                if (item.SchoolYear != schoolyear || item.Semester != semester) continue;

                XmlElement itemElement = doc.CreateElement("Item");
                itemElement.SetAttribute("Name", item.Name);
                itemElement.SetAttribute("Degree", item.Degree);
                itemElement.SetAttribute("Index", item.Index);

                element.AppendChild(itemElement);
            }
            return element;
        }
    }
}
