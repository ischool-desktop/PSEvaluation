using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.TransferStudent.Data.GroupActivity
{
    public class GroupActivityData
    {
        public List<GroupActivityItem> Items { get; set; }

        public GroupActivityData()
        {
            Items = new List<GroupActivityItem>();
        }

        public void SetItem(GroupActivityItem groupActivityItem)
        {
            GroupActivityItem item = GetItem(groupActivityItem.Name, groupActivityItem.SchoolYear, groupActivityItem.Semester);

            if (item == null)
                Items.Add(groupActivityItem);
            else
            {
                item.Text = groupActivityItem.Text;
                item.Degree = groupActivityItem.Degree;
            }
        }

        public void SetItemDegree(string name, string schoolyear, string semester,string degree)
        {
            GroupActivityItem item = GetItem(name, schoolyear, semester);

            if (item == null)
            {
                GroupActivityItem groupActivityItem = new GroupActivityItem();
                groupActivityItem.SchoolYear = schoolyear;
                groupActivityItem.Semester = semester;
                groupActivityItem.Name = name;
                groupActivityItem.Degree = degree;
                groupActivityItem.Text = string.Empty;

                Items.Add(groupActivityItem);
            }
            else
            {             
                item.Degree = degree;
            }
        }

        public void SetItemText(string name, string schoolyear, string semester, string text)
        {
            GroupActivityItem item = GetItem(name, schoolyear, semester);

            if (item == null)
            {
                GroupActivityItem groupActivityItem = new GroupActivityItem();
                groupActivityItem.SchoolYear = schoolyear;
                groupActivityItem.Semester = semester;
                groupActivityItem.Name = name;
                groupActivityItem.Degree = string.Empty;
                groupActivityItem.Text = text;

                Items.Add(groupActivityItem);
            }
            else
            {
                item.Text = text;
            }
        }

        public GroupActivityItem GetItem(string name, string schoolyear, string semester)
        {
            foreach (GroupActivityItem item in Items)
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
            XmlElement element = doc.CreateElement("GroupActivity");

            foreach (GroupActivityItem item in Items)
            {
                if (item.SchoolYear != schoolyear || item.Semester != semester) continue;

                XmlElement itemElement = doc.CreateElement("Item");
                itemElement.SetAttribute("Name", item.Name);
                itemElement.SetAttribute("Degree", item.Degree);
                itemElement.SetAttribute("Description", item.Text);

                element.AppendChild(itemElement);
            }
            return element;
        }
    }
}
