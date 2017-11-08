//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using System.Xml;

//namespace KaoHsiung.TransferReport
//{
//    public partial class DomainPanel : UserControl
//    {
//        private string _domain;
//        private Dictionary<string, ListViewItem> _items;

//        public DomainPanel(string domain)
//        {
//            InitializeComponent();

//            _domain = domain;
//            _items = new Dictionary<string, ListViewItem>();

//            this.groupPanel.Text = domain + "領域";
//            this.listView.Items.Clear();
//        }

//        internal void AddSubject(string subject)
//        {
//            ListViewItem item = new ListViewItem(subject);
//            item.Checked = false;
//            _items.Add(subject, item);
//            this.listView.Items.Add(item);
//        }

//        internal void CheckSubject(string subjectName)
//        {
//            if (_items.ContainsKey(subjectName))
//                _items[subjectName].Checked = true;
//        }

//        internal void CheckAll()
//        {
//            foreach (ListViewItem item in _items.Values)
//                item.Checked = true;
//        }

//        internal XmlElement ToXml()
//        {
//            //<Domain Name="語文">
//            //    <Subject Name="國文"/>
//            //    <Subject Name="英文"/>
//            //</Domain>
//            XmlDocument doc = new XmlDocument();
//            XmlElement element = doc.CreateElement("Domain");
//            element.SetAttribute("Name", _domain);
//            foreach (string subject in _items.Keys)
//            {
//                ListViewItem item = _items[subject];
//                if (item.Checked)
//                {
//                    XmlElement subjectElement = doc.CreateElement("Subject");
//                    subjectElement.SetAttribute("Name", subject);
//                    element.AppendChild(subjectElement);
//                }
//            }
//            return element;
//        }
//    }
//}
