using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.DailyBehavior;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.DailyLifeRecommend;
using System.Xml;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.DailyLifeRecommend
{
    public class DailyLifeRecommendData
    {
        public List<DailyLifeRecommendItem> Items { get; set; }

        public DailyLifeRecommendData()
        {
            Items = new List<DailyLifeRecommendItem>();
        }

        public void SetItem(DailyLifeRecommendItem dailyLifeRecommendItem)
        {
            DailyLifeRecommendItem item = GetItem(dailyLifeRecommendItem.Name, dailyLifeRecommendItem.SchoolYear, dailyLifeRecommendItem.Semester);

            if (item == null)
                Items.Add(dailyLifeRecommendItem);
            else
                item.Description = dailyLifeRecommendItem.Description;
        }

        public DailyLifeRecommendItem GetItem(string name, string schoolyear, string semester)
        {
            foreach (DailyLifeRecommendItem item in Items)
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
            XmlElement element = doc.CreateElement("DailyLifeRecommend");

            foreach (DailyLifeRecommendItem item in Items)
            {
                if (item.SchoolYear != schoolyear || item.Semester != semester) continue;

                //XmlElement itemElement = doc.CreateElement("Item");
                //itemElement.SetAttribute("Name", item.Name);
                //itemElement.SetAttribute("Description", item.Description);                

                //element.AppendChild(itemElement);
                element.SetAttribute("Name", item.Name);
                element.SetAttribute("Description", item.Description);  

            }
            return element;
        }
    }
}
