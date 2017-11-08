using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using System.Xml;

namespace HsinChu.JHEvaluation.Data
{
    /// <summary>
    /// 新竹
    /// </summary>
    public partial class HC
    {
        /// <summary>
        /// 新竹國中評量成績記錄
        /// </summary>
        public class JHSCETakeRecord
        {
            private JHSchool.Data.JHSCETakeRecord _record;

            /// <summary>
            /// 取得或設定學生ID
            /// </summary>
            public string RefStudentID
            {
                get { return _record.RefStudentID; }
                set { _record.RefStudentID = value; }
            }

            /// <summary>
            /// 取得或設定課程ID
            /// </summary>
            public string RefCourseID
            {
                get { return _record.RefCourseID; }
                set { _record.RefCourseID = value; }
            }

            /// <summary>
            /// 取得或設定修課ID
            /// </summary>
            public string RefSCAttendID
            {
                get { return _record.RefSCAttendID; }
                set { _record.RefSCAttendID = value; }
            }

            /// <summary>
            /// 取得或設定試別ID
            /// </summary>
            public string RefExamID
            {
                get { return _record.RefExamID; }
                set { _record.RefExamID = value; }
            }

            /// <summary>
            /// 取得或設定定期分數
            /// </summary>
            public decimal? Score
            {
                get { return _record.Score; }
                set { _record.Score = value; }
            }

            /// <summary>
            /// 取得或設定平時分數
            /// </summary>
            public decimal? AssignmentScore { get; set; }

            /// <summary>
            /// 取得或設定文字描述
            /// </summary>
            public string Text
            {
                get { return _record.Text; }
                set { _record.Text = value; }
            }

            /// <summary>
            /// 建構子
            /// </summary>
            /// <param name="record"></param>
            public JHSCETakeRecord(JHSchool.Data.JHSCETakeRecord record)
            {
                _record = record;

                #region 嘗試取得 AssignmentScore
                XmlNode assignment = record.ToXML().SelectSingleNode("Extension/Extension/AssignmentScore");
                if (assignment != null) AssignmentScore = ParseDecimalAllowNull(assignment.InnerText);
                #endregion
            }

            private decimal? ParseDecimalAllowNull(string p)
            {
                decimal d;
                if (decimal.TryParse(p, out d))
                    return d;
                return null;
            }

            /// <summary>
            /// 轉型成JHSCETakeRecord
            /// </summary>
            /// <returns></returns>
            public JHSchool.Data.JHSCETakeRecord AsJHSCETakeRecord()
            {
                XmlElement root = _record.ToXML();
                XmlNode assignment = root.SelectSingleNode("Extension/Extension/AssignmentScore");
                if (assignment == null)
                {
                    assignment = root.OwnerDocument.CreateElement("AssignmentScore");
                    root.SelectSingleNode("Extension/Extension").AppendChild(assignment);
                }
                assignment.InnerText = "" + AssignmentScore;
                _record.Load(root);
                return _record;
            }
        }
    }

    public static class HC_JHSCETakeRecord_ExtendMethod
    {
        public static List<JHSchool.Data.JHSCETakeRecord> AsJHSCETakeRecords(this List<HC.JHSCETakeRecord> list)
        {
            List<JHSchool.Data.JHSCETakeRecord> result = new List<JHSCETakeRecord>();
            foreach (var item in list)
                result.Add(item.AsJHSCETakeRecord());
            return result;
        }

        public static List<HC.JHSCETakeRecord> AsHCJHSCETakeRecords(this List<JHSchool.Data.JHSCETakeRecord> list)
        {
            List<HC.JHSCETakeRecord> result = new List<HC.JHSCETakeRecord>();
            foreach (var item in list)
                result.Add(new HC.JHSCETakeRecord(item));
            return result;
        }
    }
}
