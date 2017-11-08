using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.API.PlugIn;
using Framework;
using System.Xml;
using System.Threading;
using JHSchool.Evaluation.Editor;
using JHSchool.Data;
using JHSchool.Evaluation;
using JHSchool;

namespace HsinChu.JHEvaluation.ImportExport
{
    class ImportGradScore : SmartSchool.API.PlugIn.Import.Importer
    {
        public ImportGradScore()
        {
            this.Image = null;
            this.Text = "匯入畢業成績";
        }

        public override void InitializeImport(SmartSchool.API.PlugIn.Import.ImportWizard wizard)
        {
            Dictionary<string, int> _ID_SchoolYear_Semester_GradeYear = new Dictionary<string, int>();
            Dictionary<string, List<string>> _ID_SchoolYear_Semester_Subject = new Dictionary<string, List<string>>();
            Dictionary<string, JHStudentRecord> _StudentCollection = new Dictionary<string, JHStudentRecord>();
            Dictionary<JHStudentRecord, Dictionary<int, decimal>> _StudentPassScore = new Dictionary<JHStudentRecord, Dictionary<int, decimal>>();

            wizard.PackageLimit = 3000;
            wizard.ImportableFields.AddRange("領域", "分數評量");
            wizard.RequiredFields.AddRange("領域", "分數評量");

            wizard.ValidateStart += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateStartEventArgs e)
            {
                #region ValidateStart
                _ID_SchoolYear_Semester_GradeYear.Clear();
                _ID_SchoolYear_Semester_Subject.Clear();
                _StudentCollection.Clear();

                List<JHStudentRecord> list = JHStudent.SelectByIDs(e.List);

                MultiThreadWorker<JHStudentRecord> loader = new MultiThreadWorker<JHStudentRecord>();
                loader.MaxThreads = 3;
                loader.PackageSize = 250;
                loader.PackageWorker += delegate(object sender1, PackageWorkEventArgs<JHStudentRecord> e1)
                {
                    GradScore.Instance.SyncDataBackground(e.List);
                };
                loader.Run(list);

                foreach (JHStudentRecord stu in list)
                {
                    if (!_StudentCollection.ContainsKey(stu.ID))
                        _StudentCollection.Add(stu.ID, stu);
                }
                #endregion
            };
            wizard.ValidateRow += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateRowEventArgs e)
            {
                #region ValidateRow
                int t;
                decimal d;
                JHStudentRecord student;
                if (_StudentCollection.ContainsKey(e.Data.ID))
                {
                    student = _StudentCollection[e.Data.ID];
                }
                else
                {
                    e.ErrorMessage = "壓根就沒有這個學生" + e.Data.ID;
                    return;
                }
                bool inputFormatPass = true;
                #region 驗各欄位填寫格式
                foreach (string field in e.SelectFields)
                {
                    string value = e.Data[field];
                    switch (field)
                    {
                        default:
                            break;
                        case "領域":
                            //if (value == "")
                            //{
                            //    inputFormatPass &= false;
                            //    e.ErrorFields.Add(field, "必須填寫");
                            //}
                            //else if (!Domains.Contains(value))
                            //{
                            //    inputFormatPass &= false;
                            //    e.ErrorFields.Add(field, "必須為七大領域、學習領域、課程學習");
                            //}
                            break;
                        case "分數評量":
                            if (value != "" && !decimal.TryParse(value, out d))
                            {
                                inputFormatPass &= false;
                                e.ErrorFields.Add(field, "必須填入空白或數值");
                            }
                            break;
                    }
                }
                #endregion
                //輸入格式正確才會針對情節做檢驗
                if (inputFormatPass)
                {
                    string errorMessage = "";

                    string key = e.Data.ID;
                    string skey = e.Data["領域"];

                    if (!_ID_SchoolYear_Semester_Subject.ContainsKey(key))
                        _ID_SchoolYear_Semester_Subject.Add(key, new List<string>());
                    if (_ID_SchoolYear_Semester_Subject[key].Contains(skey))
                    {
                        errorMessage += (errorMessage == "" ? "" : "\n") + "同一學生不允許多筆相同畢業領域的資料";
                    }
                    else
                        _ID_SchoolYear_Semester_Subject[key].Add(skey);

                    e.ErrorMessage = errorMessage;
                }
                #endregion
            };

            wizard.ImportPackage += delegate(object sender, SmartSchool.API.PlugIn.Import.ImportPackageEventArgs e)
            {
                #region ImportPackage
                Dictionary<string, List<RowData>> id_Rows = new Dictionary<string, List<RowData>>();
                #region 分包裝
                foreach (RowData data in e.Items)
                {
                    if (!id_Rows.ContainsKey(data.ID))
                        id_Rows.Add(data.ID, new List<RowData>());
                    id_Rows[data.ID].Add(data);
                }
                #endregion

                Dictionary<string, GradScoreRecordEditor> gradDict = new Dictionary<string, GradScoreRecordEditor>();

                //交叉比對各學生資料
                #region 交叉比對各學生資料
                foreach (string id in id_Rows.Keys)
                {
                    JHStudentRecord studentRec = _StudentCollection[id];

                    GradScoreRecord record = GradScore.Instance.Items[studentRec.ID];
                    GradScoreRecordEditor editor = null;
                    if (record != null)
                        editor = record.GetEditor();
                    else
                        editor = new GradScoreRecordEditor(Student.Instance.Items[studentRec.ID]);

                    if (!gradDict.ContainsKey(studentRec.ID))
                        gradDict.Add(studentRec.ID, editor);

                    //要匯入的學期科目成績
                    Dictionary<string, RowData> importScoreDictionary = new Dictionary<string, RowData>();

                    #region 整理要匯入的資料
                    foreach (RowData row in id_Rows[id])
                    {
                        int t;
                        string domain = row["領域"];

                        if (!importScoreDictionary.ContainsKey(domain))
                            importScoreDictionary.Add(domain, row);
                    }
                    #endregion

                    //開始處理ImportScore
                    foreach (string domain in importScoreDictionary.Keys)
                    {
                        RowData data = importScoreDictionary[domain];
                        if (domain == "學習領域")
                        {
                            decimal d;
                            if (decimal.TryParse(data["分數評量"], out d))
                                editor.LearnDomainScore = d;
                            else
                                editor.LearnDomainScore = null;
                        }
                        else if (domain == "課程學習")
                        {
                            decimal d;
                            if (decimal.TryParse(data["分數評量"], out d))
                                editor.CourseLearnScore = d;
                            else
                                editor.CourseLearnScore = null;
                        }
                        else
                        {
                            if (!editor.Domains.ContainsKey(domain))
                                editor.Domains.Add(domain, new GradDomainScore(domain));
                            decimal d;
                            if (decimal.TryParse(data["分數評量"], out d))
                                editor.Domains[domain].Score = d;
                            else
                                editor.Domains[domain].Score = null;
                        }
                    }
                }
                #endregion

                if (gradDict.Values.Count > 0)
                {
                    #region 分批次兩路上傳
                    List<List<GradScoreRecordEditor>> updatePackages = new List<List<GradScoreRecordEditor>>();
                    List<List<GradScoreRecordEditor>> updatePackages2 = new List<List<GradScoreRecordEditor>>();
                    {
                        List<GradScoreRecordEditor> package = null;
                        int count = 0;
                        foreach (GradScoreRecordEditor var in gradDict.Values)
                        {
                            if (count == 0)
                            {
                                package = new List<GradScoreRecordEditor>(30);
                                count = 30;
                                if ((updatePackages.Count & 1) == 0)
                                    updatePackages.Add(package);
                                else
                                    updatePackages2.Add(package);
                            }
                            package.Add(var);
                            count--;
                        }
                    }
                    Thread threadUpdateSemesterSubjectScore = new Thread(new ParameterizedThreadStart(updateSemesterSubjectScore));
                    threadUpdateSemesterSubjectScore.IsBackground = true;
                    threadUpdateSemesterSubjectScore.Start(updatePackages);
                    Thread threadUpdateSemesterSubjectScore2 = new Thread(new ParameterizedThreadStart(updateSemesterSubjectScore));
                    threadUpdateSemesterSubjectScore2.IsBackground = true;
                    threadUpdateSemesterSubjectScore2.Start(updatePackages2);

                    threadUpdateSemesterSubjectScore.Join();
                    threadUpdateSemesterSubjectScore2.Join();
                    #endregion
                }

                FISCA.LogAgent.ApplicationLog.Log("成績系統.匯入匯出", "匯入畢業成績", "總共匯入" + gradDict.Values.Count + "筆畢業成績。");
                #endregion
            };
            wizard.ImportComplete += delegate
            {
                //EventHub.Instance.InvokScoreChanged(new List<string>(_StudentCollection.Keys).ToArray());
            };
        }

        private void updateSemesterSubjectScore(object item)
        {
            List<List<GradScoreRecordEditor>> updatePackages = (List<List<GradScoreRecordEditor>>)item;
            foreach (List<GradScoreRecordEditor> package in updatePackages)
            {
                package.SaveAllEditors();
            }
        }
    }
}
