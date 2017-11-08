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
        /// 新竹國中AEIncludeRecord
        /// </summary>
        public class JHAEIncludeRecord
        {
            private JHSchool.Data.JHAEIncludeRecord _record;

            /// <summary>
            /// 取得AssessmentSetupRecord
            /// </summary>
            public JHAssessmentSetupRecord AssessmentSetup
            {
                get { return _record.AssessmentSetup; }
            }

            /// <summary>
            /// 取得ExamRecord
            /// </summary>
            public JHExamRecord Exam
            {
                get { return _record.Exam; }
            }

            /// <summary>
            /// 取得或設定結束時間
            /// </summary>
            public string EndTime
            {
                get { return _record.EndTime; }
                set { _record.EndTime = value; }
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
            /// 取得或設定試別名稱
            /// </summary>
            public string ExamName
            {
                get { return _record.ExamName; }
                set { _record.ExamName = value; }
            }

            /// <summary>
            /// 取得或設定評量設定ID
            /// </summary>
            public string RefAssessmentSetupID
            {
                get { return _record.RefAssessmentSetupID; }
                set { _record.RefAssessmentSetupID = value; }
            }

            /// <summary>
            /// 取得或設定開始時間
            /// </summary>
            public string StartTime
            {
                get { return _record.StartTime; }
                set { _record.StartTime = value; }
            }

            /// <summary>
            /// 取得或設定是否需評定期分數
            /// </summary>
            public bool UseScore
            {
                get { return _record.UseScore; }
                set { _record.UseScore = value; }
            }

            /// <summary>
            /// 取得或設定是否需評平時分數
            /// </summary>
            public bool UseAssignmentScore { get; set; }

            /// <summary>
            /// 取得或設定是否需評文字描述
            /// </summary>
            public bool UseText
            {
                get { return _record.UseText; }
                set { _record.UseText = value; }
            }

            /// <summary>
            /// 取得或設定權重
            /// </summary>
            public int Weight
            {
                get { return _record.Weight; }
                set { _record.Weight = value; }
            }

            /// <summary>
            /// 建構子
            /// </summary>
            /// <param name="record"></param>
            public JHAEIncludeRecord(JHSchool.Data.JHAEIncludeRecord record)
            {
                _record = record;

                #region 嘗試取得 UseAssignmentScore
                XmlNode assignment = record.ToXML().SelectSingleNode("Extension/Extension/UseAssignmentScore");
                if (assignment != null) UseAssignmentScore = ParseBool(assignment.InnerText);
                #endregion
            }

            private bool ParseBool(string p)
            {
                if (p == "是") return true;
                else return false;
            }

            /// <summary>
            /// 轉型成JHAEIncludeRecord
            /// </summary>
            /// <returns></returns>
            public JHSchool.Data.JHAEIncludeRecord AsJHAEIncludeRecord()
            {
                XmlElement root = _record.ToXML();
                XmlNode assignment = root.SelectSingleNode("Extension/Extension/UseAssignmentScore");
                if (assignment == null)
                {
                    assignment = root.OwnerDocument.CreateElement("UseAssignmentScore");
                    root.SelectSingleNode("Extension/Extension").AppendChild(assignment);
                }
                assignment.InnerText = UseAssignmentScore ? "是" : "否";
                _record.Load(root);
                return _record;
            }
        }
    }

    public static class HC_JHAEIncludeRecord_ExtendMethod
    {
        public static List<JHSchool.Data.JHAEIncludeRecord> AsJHAEIncludeRecords(this List<HC.JHAEIncludeRecord> list)
        {
            List<JHSchool.Data.JHAEIncludeRecord> result = new List<JHAEIncludeRecord>();
            foreach (var item in list)
                result.Add(item.AsJHAEIncludeRecord());
            return result;
        }

        public static List<HC.JHAEIncludeRecord> AsHCJHAEIncludeRecords(this List<JHSchool.Data.JHAEIncludeRecord> list)
        {
            List<HC.JHAEIncludeRecord> result = new List<HC.JHAEIncludeRecord>();
            foreach (var item in list)
                result.Add(new HC.JHAEIncludeRecord(item));
            return result;
        }
    }
}
