//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using K12.Data.Configuration;
//using System.Xml;
//using System.Collections.ObjectModel;

//namespace JHSchool.Evaluation
//{
//    /// <summary>
//    /// 代表一門科目資料。
//    /// </summary>
//    public struct Subject2 : IComparable<Subject2>
//    {
//        /// <summary>
//        /// 建立 Subject2 結構。
//        /// </summary>
//        /// <param name="name">科目名稱。</param>
//        /// <param name="catalog">分類名稱，例如國中的領域。(也可指定為 string.Empty。)</param>
//        public Subject2(string name, string catalog)
//            : this()
//        {
//            Name = name;
//            Catalog = catalog;
//        }

//        /// <summary>
//        /// 建立 Subject2 結構。
//        /// </summary>
//        /// <param name="catalog">分類名稱，例如國中的領域。</param>
//        public Subject2(string name)
//            : this()
//        {
//            Name = name;
//            Catalog = string.Empty;
//        }

//        /// <summary>
//        /// 科目名稱。
//        /// </summary>
//        public string Name { get; set; }

//        /// <summary>
//        /// 分類名稱，例如國中的領域。
//        /// </summary>
//        public string Catalog { get; set; }

//        private static IComparer<Subject2> _Ordinal = null;

//        /// <summary>
//        /// 科目的排序類別。
//        /// </summary>
//        public static IComparer<Subject2> Ordinal
//        {
//            get
//            {
//                if (_Ordinal == null)
//                    _Ordinal = new Subject2Comparer(Subjects, Domains);
//                return _Ordinal;
//            }
//        }

//        /// <summary>
//        /// 提供計算 Subject2 的前後順序。
//        /// </summary>
//        /// <param name="x"></param>
//        /// <param name="y"></param>
//        /// <returns></returns>
//        public static int CompareSubject2Ordinal(string x, string y)
//        {
//            return Ordinal.Compare(new Subject2(x), new Subject2(y));
//        }

//        /// <summary>
//        /// 提供計算 Catalog 的前後順序。
//        /// </summary>
//        /// <param name="x"></param>
//        /// <param name="y"></param>
//        /// <returns></returns>
//        public static int CompareCatalogOrdinal(string x, string y)
//        {
//            return Ordinal.Compare(new Subject2(string.Empty, x), new Subject2(string.Empty, y));
//        }

//        /// <summary>
//        /// 計算 Subject2 Struct 的前後順序。
//        /// </summary>
//        /// <param name="x"></param>
//        /// <param name="y"></param>
//        /// <returns></returns>
//        public static int CompareOrdinal(Subject2 x, Subject2 y)
//        {
//            return Ordinal.Compare(x, y);
//        }

//        /// <summary>
//        /// 取得科目的英文名稱
//        /// </summary>
//        /// <param name="subject">科目中文名稱</param>
//        /// <returns>科目英文名稱</returns>
//        public static string GetSubjectEnglish(string subject)
//        {
//            if (_InnerSubjects.ContainsKey(subject))
//                return _InnerSubjects[subject].EnglishName;
//            return string.Empty;
//        }

//        /// <summary>
//        /// 取得領域的英文名稱
//        /// </summary>
//        /// <param name="domain">領域中文名稱</param>
//        /// <returns>領域英文名稱</returns>
//        public static string GetDomainEnglish(string domain)
//        {
//            if (_InnerDomains.ContainsKey(domain))
//                return _InnerDomains[domain].EnglishName;
//            return string.Empty;
//        }

//        public static string GetDomainGroup(string domain)
//        {
//            if (_InnerDomains.ContainsKey(domain))
//                return _InnerDomains[domain].Group;
//            return string.Empty;
//        }

//        private static List<string> _Subjects = null;
//        /// <summary>
//        /// 取得科目清單
//        /// </summary>
//        public static IList<string> Subjects
//        {
//            get
//            {
//                if (_Subjects == null)
//                    ReloadOrdinal();
//                return _Subjects.AsReadOnly();
//            }
//        }

//        private static List<string> _Domains = null;
//        /// <summary>
//        /// 取得領域清單
//        /// </summary>
//        public static IList<string> Domains
//        {
//            get
//            {
//                if (_Domains == null)
//                    ReloadOrdinal();
//                return _Domains.AsReadOnly();
//            }
//        }

//        private static Dictionary<string, InnerSubject> _InnerSubjects = null;
//        private static Dictionary<string, InnerDomain> _InnerDomains = null;

//        internal static void ReloadOrdinal()
//        {
//            XmlDocument doc = new XmlDocument();
//            doc.LoadXml(Properties.Resources.DefaultOrdinal);
//            XmlElement defaultData = doc.DocumentElement;

//            ConfigData cd = K12.Data.School.Configuration["JHEvaluation_Ordinal"];
//            XmlElement SubjectOrdinal = cd.GetXml("SubjectOrdinal", defaultData.SelectSingleNode("Subjects") as XmlElement);
//            XmlElement DomainOrdinal = cd.GetXml("DomainOrdinal", defaultData.SelectSingleNode("Domains") as XmlElement);

//            if (_InnerSubjects == null) _InnerSubjects = new Dictionary<string, InnerSubject>();
//            if (_InnerDomains == null) _InnerDomains = new Dictionary<string, InnerDomain>();

//            _InnerSubjects.Clear();
//            _InnerDomains.Clear();

//            foreach (XmlElement subjectElement in SubjectOrdinal.SelectNodes("Subject"))
//            {
//                InnerSubject iSubject = new InnerSubject()
//                {
//                    Name = subjectElement.GetAttribute("Name"),
//                    EnglishName = subjectElement.GetAttribute("EnglishName"),
//                    Order = ParseInt(subjectElement.GetAttribute("Order"))
//                };
//                _InnerSubjects.Add(iSubject.Name, iSubject);
//            }

//            foreach (XmlElement domainElement in DomainOrdinal.SelectNodes("Domain"))
//            {
//                InnerDomain iDomain = new InnerDomain()
//                {
//                    Name = domainElement.GetAttribute("Name"),
//                    EnglishName = domainElement.GetAttribute("EnglishName"),
//                    Order = ParseInt(domainElement.GetAttribute("Order")),
//                    Group = domainElement.GetAttribute("Group")
//                };
//                _InnerDomains.Add(iDomain.Name, iDomain);
//            }

//            _Subjects = new List<string>(_InnerSubjects.Keys);
//            _Domains = new List<string>(_InnerDomains.Keys);
//        }

//        private static int ParseInt(string s)
//        {
//            int d;
//            return int.TryParse(s, out d) ? d : int.MaxValue;
//        }

//        #region IComparable<Subject2> 成員

//        /// <summary>
//        /// 比較科目的順序。
//        /// </summary>
//        /// <param name="other"></param>
//        /// <returns></returns>
//        public int CompareTo(Subject2 other)
//        {
//            return CompareOrdinal(this, other);
//        }

//        #endregion

//        //internal void UpdateDomainOrdinal(List<InnerDomain> domains)
//        //{
//        //    domains.Sort(delegate(InnerDomain x, InnerDomain y)
//        //    {
//        //        return x.Order.CompareTo(y.Order);
//        //    });

//        //    //XmlDocument doc = new XmlDocument();
//        //    //XmlElement defaultData = 

//        //    //ConfigData cd = K12.Data.School.Configuration["JHEvaluation_Ordinal"];
//        //    //cd
//        //    ReloadOrdinal();
//        //}

//        internal struct InnerDomain
//        {
//            public string Name { get; set; }
//            public string EnglishName { get; set; }
//            public int Order { get; set; }
//            public string Group { get; set; }
//        }

//        internal struct InnerSubject
//        {
//            public string Name { get; set; }
//            public string EnglishName { get; set; }
//            public int Order { get; set; }
//        }
//    }

//    internal class Subject2Comparer : IComparer<Subject2>
//    {
//        private IList<string> SubjectOrder;
//        private IList<string> DomainOrder;

//        internal Subject2Comparer(IList<string> subjects, IList<string> domains)
//        {
//            SubjectOrder = subjects;
//            DomainOrder = domains;
//        }

//        #region IComparer<Subject2> 成員

//        public int Compare(Subject2 x, Subject2 y)
//        {
//            int xSubjPos = SubjectOrder.IndexOf(x.Name);
//            int ySubjPos = SubjectOrder.IndexOf(y.Name);

//            int xCataPos = DomainOrder.IndexOf(x.Catalog);
//            int yCataPos = DomainOrder.IndexOf(y.Catalog);

//            xSubjPos = xSubjPos < 0 ? ushort.MaxValue : xSubjPos;
//            ySubjPos = ySubjPos < 0 ? ushort.MaxValue : ySubjPos;

//            xCataPos = xCataPos < 0 ? ushort.MaxValue >> 1 : xCataPos;
//            yCataPos = yCataPos < 0 ? ushort.MaxValue >> 1 : yCataPos;

//            int xPos = xSubjPos | xCataPos << 16;
//            int yPos = ySubjPos | yCataPos << 16;

//            return xPos.CompareTo(yPos);
//        }

//        #endregion
//    }
//}
