using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using JHEvaluation.StudentSemesterScoreNotification.Association.UDT;
using System.Xml;
using JHSchool.Behavior.BusinessLogic;

namespace JHEvaluation.StudentSemesterScoreNotification
{
    internal class DataCache
    {
        //key: student id
        private Dictionary<string, AutoSummaryRecord> _autoSummaryCache;
        //key: student id
        private Dictionary<string, JHSemesterScoreRecord> _semesterScoreCache;
        //key: student id
        private Dictionary<string, K12.Data.SemesterHistoryItem> _historyItemCache;

        private Dictionary<string, AssnScore> _assnScoreCache;

        public DataCache(Options options)
        {
            var student_ids = from student in options.Students select student.ID;

            #region 取得 AutoSummary
            _autoSummaryCache = new Dictionary<string, AutoSummaryRecord>();
            foreach (AutoSummaryRecord record in AutoSummary.Select(student_ids.ToList<string>(), null))
            {
                if (record.SchoolYear == options.SchoolYear &&
                    record.Semester == options.Semester)
                {
                    if (!_autoSummaryCache.ContainsKey(record.RefStudentID))
                        _autoSummaryCache.Add(record.RefStudentID, record);
                }
            }
            #endregion

            #region 取得 SemesterScore
            _semesterScoreCache = new Dictionary<string, JHSemesterScoreRecord>();
            foreach (var record in JHSemesterScore.SelectByStudentIDs(student_ids.ToList<string>()))
            {
                if (record.SchoolYear == options.SchoolYear &&
                    record.Semester == options.Semester)
                {
                    if (!_semesterScoreCache.ContainsKey(record.RefStudentID))
                        _semesterScoreCache.Add(record.RefStudentID, record);
                }
            }
            #endregion

            #region 取得 SemesterHistoryItem
            _historyItemCache = new Dictionary<string, K12.Data.SemesterHistoryItem>();
            foreach (var record in JHSemesterHistory.SelectByStudentIDs(student_ids))
            {
                foreach (var item in record.SemesterHistoryItems)
                {
                    if (item.SchoolYear == options.SchoolYear &&
                        item.Semester == options.Semester)
                    {
                        if (!_historyItemCache.ContainsKey(item.RefStudentID))
                            _historyItemCache.Add(item.RefStudentID, item);
                    }
                }
            }
            #endregion

            if (Global.Params["Mode"] == "KaoHsiung")
            {
                #region 取得社團成績
                _assnScoreCache = new Dictionary<string, AssnScore>();

                FISCA.UDT.AccessHelper ah = new FISCA.UDT.AccessHelper();
                string condition = string.Format("SchoolYear='{0}' and Semester='{1}'", options.SchoolYear, options.Semester);
                List<AssnCode> list = ah.Select<AssnCode>(condition);
                foreach (AssnCode record in list)
                {
                    if (!_assnScoreCache.ContainsKey(record.StudentID))
                    {
                        XmlElement scores = K12.Data.XmlHelper.LoadXml(record.Scores);
                        XmlElement itemElement = (XmlElement)scores.SelectSingleNode("Item");
                        if (itemElement != null)
                        {
                            AssnScore assnScore = new AssnScore()
                            {
                                Score = itemElement.GetAttribute("Score"),
                                Effort = itemElement.GetAttribute("Effort"),
                                Text = itemElement.GetAttribute("Text")
                            };
                            _assnScoreCache.Add(record.StudentID, assnScore);
                        }
                    }
                }

                //<Content>
                //<Item AssociationName="籃球社" Score="" Effort="" Text=""></Item>
                //</Content>
                #endregion
            }
        }

        public AutoSummaryRecord GetAutoSummary(string student_id)
        {
            if (_autoSummaryCache.ContainsKey(student_id)) return _autoSummaryCache[student_id];
            else return null;
        }

        public JHSemesterScoreRecord GetSemesterScore(string student_id)
        {
            if (_semesterScoreCache.ContainsKey(student_id)) return _semesterScoreCache[student_id];
            else return null;
        }

        public K12.Data.SemesterHistoryItem GetSemesterHistoryItem(string student_id)
        {
            if (_historyItemCache.ContainsKey(student_id)) return _historyItemCache[student_id];
            else return null;
        }

        public AssnScore GetAssnScore(string student_id)
        {
            if (Global.Params["Mode"] != "KaoHsiung") return null;

            if (_assnScoreCache.ContainsKey(student_id)) return _assnScoreCache[student_id];
            else return null;
        }
    }
}
