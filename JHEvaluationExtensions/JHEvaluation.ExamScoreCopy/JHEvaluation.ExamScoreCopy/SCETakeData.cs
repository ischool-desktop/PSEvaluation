using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JHSchool.Data;

namespace JHEvaluation.ExamScoreCopy
{
    /// <summary>
    /// 評量成績資料結構
    /// </summary>
    internal class SCETakeData
    {
        public JHSCETakeRecord Origin { get; private set; }

        public SCETakeData(JHSCETakeRecord record)
        {
            Origin = record;

            XmlElement xmlrecord = record.ToXML();

            #region 嘗試取得 AssignmentScore
            XmlNode assignment = xmlrecord.SelectSingleNode("Extension/Extension/AssignmentScore");
            if (assignment != null) AssignmentScore = ParseDecimalAllowNull(assignment.InnerText);
            #endregion

            #region 嘗試取得 Effort
            XmlNode effort = xmlrecord.SelectSingleNode("Extension/Extension/Effort");
            if (effort != null) Effort = (int?)ParseDecimalAllowNull(effort.InnerText);
            #endregion
        }

        /// <summary>
        /// 分數評量
        /// </summary>
        public decimal? Score { get { return Origin.Score; } set { Origin.Score = value; } }
        /// <summary>
        /// 平時分數(新竹)
        /// </summary>
        public decimal? AssignmentScore { get; set; }
        /// <summary>
        /// 文字評量
        /// </summary>
        public string Text { get { return Origin.Text; } set { Origin.Text = value; } }
        /// <summary>
        /// 努力程度(高雄)
        /// </summary>
        public int? Effort { get; set; }

        private decimal? ParseDecimalAllowNull(string p)
        {
            decimal d;
            if (decimal.TryParse(p, out d))
                return d;
            return null;
        }

        public JHSCETakeRecord AsJHSCETakeRecord()
        {
            XmlElement root = Origin.ToXML();

            if (Program.Mode == ModuleMode.KaoHsiung)
            {
                XmlNode effort = root.SelectSingleNode("Extension/Extension/Effort");
                if (effort == null)
                {
                    effort = root.OwnerDocument.CreateElement("Effort");
                    root.SelectSingleNode("Extension/Extension").AppendChild(effort);
                }
                effort.InnerText = "" + Effort;
            }
            else if (Program.Mode == ModuleMode.HsinChu)
            {
                XmlNode assignment = root.SelectSingleNode("Extension/Extension/AssignmentScore");
                if (assignment == null)
                {
                    assignment = root.OwnerDocument.CreateElement("AssignmentScore");
                    root.SelectSingleNode("Extension/Extension").AppendChild(assignment);
                }
                assignment.InnerText = "" + AssignmentScore;
            }

            Origin.Load(root);
            return Origin;
        }
    }
}
