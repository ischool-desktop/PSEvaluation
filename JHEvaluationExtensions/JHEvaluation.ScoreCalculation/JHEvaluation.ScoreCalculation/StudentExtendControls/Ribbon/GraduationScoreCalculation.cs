using System.Collections.Generic;
using System.ComponentModel;
using Framework;
using JHSchool.Evaluation.Calculation;
using JHSchool.Evaluation.Editor;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon
{
    internal class GraduationScoreCalculation
    {
        public static void Calculate(List<StudentRecord> students)
        {
            const string LearnDomain = "學習領域";
            const string CourseLearn = "課程學習";

            ScoreCalcRule.Instance.SyncAllBackground();
            //SemesterScore.Instance.SyncAllBackground();
            Dictionary<string, List<Data.JHSemesterScoreRecord>> studentSemesterScoreRecordCache = new Dictionary<string, List<JHSchool.Data.JHSemesterScoreRecord>>();
            foreach (Data.JHSemesterScoreRecord record in Data.JHSemesterScore.SelectByStudentIDs(students.AsKeyList()))
            {
                if (!studentSemesterScoreRecordCache.ContainsKey(record.RefStudentID))
                    studentSemesterScoreRecordCache.Add(record.RefStudentID, new List<JHSchool.Data.JHSemesterScoreRecord>());
                studentSemesterScoreRecordCache[record.RefStudentID].Add(record);
            }

            List<GradScoreRecordEditor> editors = new List<GradScoreRecordEditor>();

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            MultiThreadBackgroundWorker<GradScoreRecordEditor> multiWorker = new MultiThreadBackgroundWorker<GradScoreRecordEditor>();
            multiWorker.WorkerReportsProgress = true;
            multiWorker.AutoReportsProgress = true;
            multiWorker.PackageSize = 50;
            multiWorker.Loading = MultiThreadLoading.Light;

            worker.DoWork += delegate
            {
                Dictionary<string, ScoreCalculator> calc = new Dictionary<string, ScoreCalculator>();

                double studentTotal = students.Count;
                double studentCount = 0;

                foreach (StudentRecord student in students)
                {
                    studentCount++;

                    #region 取得成績計算規則
                    string calcID = string.Empty;
                    ScoreCalcRuleRecord calcRecord = student.GetScoreCalcRuleRecord();
                    if (calcRecord != null)
                    {
                        calcID = calcRecord.ID;
                        if (!calc.ContainsKey(calcID))
                        {
                            Data.JHScoreCalcRuleRecord record = null;
                            List<Data.JHScoreCalcRuleRecord> list = Data.JHScoreCalcRule.SelectByIDs(new string[] { calcRecord.ID });
                            if (list.Count > 0) record = list[0];
                            calc.Add(calcID, new ScoreCalculator(record));
                        }
                    }
                    else
                    {
                        if (!calc.ContainsKey(string.Empty))
                            calc.Add(string.Empty, new ScoreCalculator(null));
                    }
                    #endregion

                    #region 取得各學期成績
                    List<Data.JHSemesterScoreRecord> semesterScoreRecordList;
                    if (studentSemesterScoreRecordCache.ContainsKey(student.ID))
                        semesterScoreRecordList = studentSemesterScoreRecordCache[student.ID];
                    else
                        semesterScoreRecordList = new List<Data.JHSemesterScoreRecord>();
                    Dictionary<string, List<decimal>> domainScores = new Dictionary<string, List<decimal>>();

                    foreach (Data.JHSemesterScoreRecord record in semesterScoreRecordList)
                    {
                        foreach (K12.Data.DomainScore domain in record.Domains.Values)
                        {
                            if (!domainScores.ContainsKey(domain.Domain))
                                domainScores.Add(domain.Domain, new List<decimal>());

                            if (domain.Score.HasValue)
                                domainScores[domain.Domain].Add(domain.Score.Value);
                        }

                        if (!domainScores.ContainsKey(LearnDomain))
                            domainScores.Add(LearnDomain, new List<decimal>());
                        if (record.LearnDomainScore.HasValue)
                            domainScores[LearnDomain].Add(record.LearnDomainScore.Value);

                        if (!domainScores.ContainsKey(CourseLearn))
                            domainScores.Add(CourseLearn, new List<decimal>());
                        if (record.CourseLearnScore.HasValue)
                            domainScores[CourseLearn].Add(record.CourseLearnScore.Value);
                    }
                    #endregion

                    #region 產生畢業成績資料
                    GradScoreRecordEditor editor;
                    GradScoreRecord gradScoreRecord = GradScore.Instance.Items[student.ID];
                    if (gradScoreRecord != null)
                        editor = gradScoreRecord.GetEditor();
                    else
                        editor = new GradScoreRecordEditor(student);

                    editor.LearnDomainScore = null;
                    editor.CourseLearnScore = null;
                    editor.Domains.Clear();

                    foreach (string domain in domainScores.Keys)
                    {
                        decimal total = 0;
                        decimal count = domainScores[domain].Count;

                        if (count <= 0) continue;

                        foreach (decimal score in domainScores[domain])
                            total += score;

                        total = total / count;
                        total = calc[calcID].ParseGraduateScore(total);


                        if (domain == LearnDomain)
                            editor.LearnDomainScore = total;
                        else if (domain == CourseLearn)
                            editor.CourseLearnScore = total;
                        else
                        {
                            if (!editor.Domains.ContainsKey(domain))
                                editor.Domains.Add(domain, new GradDomainScore(domain));
                            editor.Domains[domain].Score = total;
                        }
                    }

                    editors.Add(editor);
                    #endregion

                    worker.ReportProgress((int)(studentCount * 100 / studentTotal));
                }
            };
            worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("計算畢業成績中", e.ProgressPercentage);
            };
            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    MsgBox.Show("計算畢業成績時發生錯誤。" + e.Error.Message);
                    editors.Clear();
                }
                else
                {
                    multiWorker.RunWorkerAsync(editors);
                }
            };

            multiWorker.DoWork += delegate(object sender, PackageDoWorkEventArgs<GradScoreRecordEditor> e)
            {
                IEnumerable<GradScoreRecordEditor> list = e.Items;
                list.SaveAllEditors();
            };
            multiWorker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("上傳畢業成績中", e.ProgressPercentage);
            };
            multiWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    MsgBox.Show("上傳畢業成績時發生錯誤。" + e.Error.Message);
                }
                else
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("上傳畢業成績完成");
                }
            };

            worker.RunWorkerAsync();
        }
    }
}
