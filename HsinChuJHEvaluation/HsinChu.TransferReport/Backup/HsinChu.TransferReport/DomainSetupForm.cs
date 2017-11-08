//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;
//using FISCA.Presentation.Controls;
//using System.Xml;

//namespace HsinChu.TransferReport
//{
//    public partial class DomainSetupForm : BaseForm
//    {
//        private List<string> _domains;
//        private string _name;
//        private Dictionary<string, DomainPanel> _panels;
//        private Dictionary<string, List<string>> _subjects;

//        public DomainSetupForm(string name)
//        {
//            InitializeComponent();

//            _domains = new List<string>(new string[] { "語文", "健康與體育", "社會", "藝術與人文", "數學", "自然與生活科技", "綜合活動", "彈性課程" });
//            _panels = new Dictionary<string, DomainPanel>();
//            _subjects = new Dictionary<string, List<string>>();
//            _name = name;

//            //LoadProgramPlan();
//            LoadCourses();
//            CreatePanels();
//            LoadConfiguration();
//        }

//        //private void LoadProgramPlan()
//        //{
//        //    foreach (K12.Data.ProgramPlanRecord record in K12.Data.ProgramPlan.SelectAll())
//        //    {
//        //        foreach (K12.Data.ProgramSubject subject in record.Subjects)
//        //        {
//        //            if (!_subjects.ContainsKey(subject.Domain))
//        //                _subjects.Add(subject.Domain, new List<string>());
//        //            if (!_subjects[subject.Domain].Contains(subject.SubjectName))
//        //                _subjects[subject.Domain].Add(subject.SubjectName);
//        //        }
//        //    }
//        //}

//        private void LoadCourses()
//        {
//            foreach (JHSchool.Data.JHCourseRecord record in JHSchool.Data.JHCourse.SelectAll())
//            {
//                if (!_subjects.ContainsKey(record.Domain))
//                    _subjects.Add(record.Domain, new List<string>());
//                if (!_subjects[record.Domain].Contains(record.Subject))
//                    _subjects[record.Domain].Add(record.Subject);
//            }
//        }

//        private void CreatePanels()
//        {
//            foreach (string domain in _domains)
//            {
//                DomainPanel domainPanel = new DomainPanel(domain);
//                _panels.Add(domain, domainPanel);

//                if (_subjects.ContainsKey(domain))
//                {
//                    foreach (string subject in _subjects[domain])
//                        domainPanel.AddSubject(subject);
//                }

//                this.cardPanel.Controls.Add(domainPanel);
//            }
//        }

//        private void LoadConfiguration()
//        {
//            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration[_name];
//            if (cd.Contains("領域科目設定") && !string.IsNullOrEmpty(cd["領域科目設定"]))
//            {
//                XmlElement element = Framework.XmlHelper.LoadXml(cd["領域科目設定"]);
//                foreach (string domain in _domains)
//                {
//                    //<DomainSetup>
//                    //    <Domain Name="語文">
//                    //        <Subject Name="國文"/>
//                    //        <Subject Name="英文"/>
//                    //    </Domain>
//                    //    <Domain Name="數學">
//                    //        <Subject Name="數學"/>
//                    //    </Domain>
//                    //</DomainSetup>
//                    XmlElement domainElement = (XmlElement)element.SelectSingleNode("Domain[@Name='" + domain + "']");
//                    if (domainElement != null)
//                    {
//                        DomainPanel domainPanel = _panels[domain];
//                        foreach (XmlElement subject in domainElement.SelectNodes("Subject"))
//                        {
//                            string subjectName = subject.GetAttribute("Name");
//                            domainPanel.CheckSubject(subjectName);
//                        }
//                    }
//                }
//            }
//            else
//            {
//                #region 預設「語文」及「彈性課程」全選
//                foreach (string domain in new string[] { "語文", "彈性課程" })
//                {
//                    if (_panels.ContainsKey(domain))
//                        _panels[domain].CheckAll();
//                }
//                #endregion

//                MsgBox.Show("第一次設定請記得按「儲存設定」");
//            }
//        }

//        private void SaveConfiguration()
//        {
//            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration[_name];

//            XmlDocument doc = new XmlDocument();
//            XmlElement element = doc.CreateElement("DomainSetup");

//            foreach (DomainPanel panel in _panels.Values)
//            {
//                XmlElement panelElement = panel.ToXml();
//                element.AppendChild(doc.ImportNode(panelElement, true));
//            }

//            cd["領域科目設定"] = element.OuterXml;
//            cd.Save();
//        }

//        private void btnClose_Click(object sender, EventArgs e)
//        {
//            this.DialogResult = DialogResult.Cancel;
//            this.Close();
//        }

//        private void btnSave_Click(object sender, EventArgs e)
//        {
//            SaveConfiguration();
//            this.DialogResult = DialogResult.OK;
//        }
//    }
//}
