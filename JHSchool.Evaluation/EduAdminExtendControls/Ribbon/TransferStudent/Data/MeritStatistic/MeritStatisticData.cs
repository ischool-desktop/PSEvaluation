using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.Absence;
using System.Xml;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.MeritStatistic
{
    public class MeritStatisticData
    {
        public List<MeritStatisticItem> Items { get; set; }

        public MeritStatisticData()
        {
            Items = new List<MeritStatisticItem>();
        }

        public MeritStatisticItem GetItem(string schoolyear, string semester, string merit)
        {
            foreach (MeritStatisticItem item in Items)
            {
                if (item.SchoolYear == schoolyear && item.Semester == semester && item.MeritType == merit)
                    return item;
            }
            return null;
        }

        public void SetItem(MeritStatisticItem meritStatisticItem)
        {
            MeritStatisticItem item = GetItem(meritStatisticItem.SchoolYear, meritStatisticItem.Semester, meritStatisticItem.MeritType);

            if (item == null)
                Items.Add(meritStatisticItem);
            else
                item.Count = meritStatisticItem.Count;
        }

        public XmlElement GetSemesterElement(string schoolyear, string semester)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("DisciplineStatistics");
            XmlElement meritElement = doc.CreateElement("Merit");
            element.AppendChild(meritElement);
            XmlElement demeritElement = doc.CreateElement("Demerit");
            element.AppendChild(demeritElement);

            int ma = 0, mb = 0, mc = 0, da = 0, db = 0, dc = 0;
            foreach (MeritStatisticItem item in Items)
            {
                if (item.SchoolYear != schoolyear || item.Semester != semester) continue;

                if (item.MeritType == "大功")
                    ma += item.Count;
                else if (item.MeritType == "小功")
                    mb += item.Count;
                else if (item.MeritType == "嘉獎")
                    mc += item.Count;
                else if (item.MeritType == "大過")
                    da += item.Count;
                else if (item.MeritType == "小過")
                    db += item.Count;
                else if (item.MeritType == "警告")
                    dc += item.Count;
            }
           
            meritElement.SetAttribute("A", ma.ToString());
            meritElement.SetAttribute("B", mb.ToString());
            meritElement.SetAttribute("C", mc.ToString());

            demeritElement.SetAttribute("A", da.ToString());
            demeritElement.SetAttribute("B", db.ToString());
            demeritElement.SetAttribute("C", dc.ToString());

            return element;
        }
    }
}
