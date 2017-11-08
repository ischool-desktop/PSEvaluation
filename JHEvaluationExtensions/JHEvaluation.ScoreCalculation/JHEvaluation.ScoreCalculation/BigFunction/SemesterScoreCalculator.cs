using System;
using System.Collections.Generic;
using System.Text;
using JHEvaluation.ScoreCalculation.ScoreStruct;
using SCSemsScore = JHEvaluation.ScoreCalculation.ScoreStruct.SemesterScore;
using System.Linq;
using System.Windows.Forms;

namespace JHEvaluation.ScoreCalculation.BigFunction
{
    class SemesterScoreCalculator
    {
        private List<StudentScore> Students { get; set; }

        private UniqueSet<string> Filter { get; set; }

        Dictionary<string, string> Dic_StudentAttendScore = new Dictionary<string, string>();

        Dictionary<string, List<string>> errCheck = new Dictionary<string, List<string>>();

        public SemesterScoreCalculator(List<StudentScore> students, IEnumerable<string> filter)
        {
            Students = students;
            Filter = new UniqueSet<string>(filter);
        }

        /// <summary>
        /// 計算科目成績
        /// </summary>
        public void CalculateSubjectScore()
        {
            foreach (StudentScore student in Students)
            {
                if (student.CalculationRule == null) continue; //沒有成績計算規則就不計算。

                SCSemsScore Sems = student.SemestersScore[SemesterData.Empty];

                //對全部的學期科目成績作一次擇優
                foreach (string subject in Sems.Subject)
                {
                    if (Sems.Subject.Contains(subject))
                    {
                        SemesterSubjectScore sss = Sems.Subject[subject];

                        //沒有原始成績就將既有的成績填入
                        if (!sss.ScoreOrigin.HasValue)
                            sss.ScoreOrigin = sss.Value;

                        //有原始或補考成績才做擇優
                        if (sss.ScoreOrigin.HasValue || sss.ScoreMakeup.HasValue)
                            sss.BetterScoreSelection();
                    }
                }



                // 2016/7/13 穎驊註解，原本為整理重覆科目使用，後來功能被另外取代掉，又保留Dic_StudentAttendScore 的話，
                // 如果再 //處理修課課程 foreach (string subject in student.AttendScore)，不使用in student.AttendScore 而是用 in Dic_StudentAttendScore
                // 則會出現 下一個學生使用上一個學生的修課紀錄，假如上一個學生有著下一個學生沒有的修課內容，就會造成錯誤


                //foreach (string subject in student.AttendScore)
                //{

                //    if (!Dic_StudentAttendScore.ContainsKey(subject))
                //    {
                //        Dic_StudentAttendScore.Add(subject, subject);

                //    }
                //    else {

                //        //MessageBox.Show("科目:" + subject + "在課程科目名稱有重覆，" + "如繼續匯入將導致學期科目成績遺漏誤植，請確認您在" + subject + "這一科的課程資料無誤");
                //    }

                //}



                //處理修課課程
                foreach (string subject in student.AttendScore)
                {
                    if (!IsValidItem(subject)) continue; //慮掉不算的科目。

                    AttendScore attend = student.AttendScore[subject];
                    SemesterSubjectScore SemsSubj = null;

                    if (Sems.Subject.Contains(subject))
                        SemsSubj = Sems.Subject[subject];
                    else
                        SemsSubj = new SemesterSubjectScore();

                    if (attend.ToSemester) //列入學期成績。
                    {
                        //分數、權數都有資料才進行計算科目成績。
                        if (attend.Value.HasValue && attend.Weight.HasValue && attend.Period.HasValue)
                        {
                            decimal score = student.CalculationRule.ParseSubjectScore(attend.Value.Value); //進位處理。
                            //SemsSubj.Value = score;
                            SemsSubj.Weight = attend.Weight.Value;
                            SemsSubj.Period = attend.Period.Value;
                            SemsSubj.Effort = attend.Effort;
                            SemsSubj.Text = attend.Text;
                            SemsSubj.Domain = attend.Domain;

                            //填到原始成績
                            SemsSubj.ScoreOrigin = score;
                            //擇優成績
                            SemsSubj.BetterScoreSelection();
                        }
                        else //資料不合理，保持原來的分數狀態。
                            continue;

                        if (!Sems.Subject.Contains(subject))
                            Sems.Subject.Add(subject, SemsSubj);
                    }
                    else
                    {
                        //不計入學期成績，就將其刪除掉。
                        if (Sems.Subject.Contains(subject))
                            Sems.Subject.Remove(subject);
                    }
                }
            }
        }

        /// <summary>
        /// 計算領域成績
        /// </summary>
        /// <param name="defaultRule"></param>
        //public void CalculateDomainScore(ScoreCalculator defaultRule,bool clearDomainScore)
        public void CalculateDomainScore(ScoreCalculator defaultRule, DomainScoreSetting setting)
        {
            EffortMap effortmap = new EffortMap(); //努力程度對照表。
            foreach (StudentScore student in Students)
            {
                SemesterScore semsscore = student.SemestersScore[SemesterData.Empty];

                SemesterDomainScoreCollection dscores = semsscore.Domain;
                SemesterSubjectScoreCollection jscores = semsscore.Subject;
                ScoreCalculator rule = student.CalculationRule;

                if (rule == null)
                    rule = defaultRule;

                //各領域分數加總。
                Dictionary<string, decimal> domainTotal = new Dictionary<string, decimal>();
                //各領域原始分數加總。
                Dictionary<string, decimal> domainOriginTotal = new Dictionary<string, decimal>();
                //各領域權重加總。
                Dictionary<string, decimal> domainWeight = new Dictionary<string, decimal>();
                //各領域節數加總。
                Dictionary<string, decimal> domainPeriod = new Dictionary<string, decimal>();
                //文字評量串接。
                Dictionary<string, string> domainText = new Dictionary<string, string>();

                //各領域補考分數加總。
                Dictionary<string, decimal> domainMakeUpScoreTotal = new Dictionary<string, decimal>();

                //該領域的科目有補考成績清單
                List<string> haveMakeUpScoreDomains = new List<string>();

                // 只計算領域成績，不計算科目
                bool OnlyCalcDomainScore = false;

                // 檢查科目成績是否有成績，當有成績且成績的科目領域名稱非空白才計算
                int CheckSubjDomainisNotNullCot = 0;

                // 檢查是否有非科目領域是空白的科目
                foreach (string str in jscores)
                {
                    SemesterSubjectScore objSubj = jscores[str];
                    string strDomainName = objSubj.Domain.Trim();
                    if (!string.IsNullOrEmpty(strDomainName))
                        CheckSubjDomainisNotNullCot++;

                    //該領域的科目有補考成績將被加入
                    if (objSubj.ScoreMakeup.HasValue && !haveMakeUpScoreDomains.Contains(strDomainName))
                    {
                        haveMakeUpScoreDomains.Add(strDomainName);
                    }
                }

                // 當沒有科目成績或科目成績內領域沒有非空白，只計算領域成績。
                if (jscores.Count == 0 || CheckSubjDomainisNotNullCot == 0)
                    OnlyCalcDomainScore = true;
                else
                    OnlyCalcDomainScore = false;


                if (OnlyCalcDomainScore == false)
                {
                    // 從科目計算到領域

                    #region 總計各領域的總分、權重、節數。
                    foreach (string strSubj in jscores)
                    {
                        SemesterSubjectScore objSubj = jscores[strSubj];
                        string strDomain = objSubj.Domain.Injection();

                        //不計算的領域就不算。
                        if (!IsValidItem(strDomain)) continue;

                        if (objSubj.Value.HasValue && objSubj.Weight.HasValue && objSubj.Period.HasValue)
                        {
                            if (!objSubj.ScoreOrigin.HasValue)
                                objSubj.ScoreOrigin = objSubj.Value;

                            //// 針對高雄處理 國語文跟英語合成語文領域
                            //if (Program.Mode == ModuleMode.KaoHsiung && (strDomain == "國語文" || strDomain == "英語"))
                            //{
                            //    if (!domainTotal.ContainsKey(khDomain))
                            //    {
                            //        domainTotal.Add(khDomain, 0);
                            //        domainOriginTotal.Add(khDomain, 0);
                            //        domainWeight.Add(khDomain, 0);
                            //        domainPeriod.Add(khDomain, 0);
                            //        domainText.Add(khDomain, string.Empty);

                            //        //領域補考成績
                            //        domain_MakeUpScore_Total.Add(khDomain, 0);
                            //    }

                            //    domainTotal[khDomain] += objSubj.Value.Value * objSubj.Weight.Value;
                            //    //科目的原始成績加總
                            //    domainOriginTotal[khDomain] += objSubj.ScoreOrigin.Value * objSubj.Weight.Value;

                            //    domainWeight[khDomain] += objSubj.Weight.Value;
                            //    domainPeriod[khDomain] += objSubj.Period.Value;
                            //    // 2016/2/26 經高雄繼斌與蔡主任討論，語文領域文字描述不需要儲存
                            //    domainText[khDomain] = "";//+= GetDomainSubjectText(strSubj, objSubj.Text);

                            //    //領域補考成績總和 ，算法為 先用各科目的"成績" 加權計算 ， 之後會再判斷， 此成績總和 是否含有"補考成績" 是否為"補考成績總和"
                            //    domain_MakeUpScore_Total[khDomain] += objSubj.Value.Value * objSubj.Weight.Value;
                            //}

                            if (!domainTotal.ContainsKey(strDomain))
                            {
                                domainTotal.Add(strDomain, 0);
                                domainOriginTotal.Add(strDomain, 0);
                                domainWeight.Add(strDomain, 0);
                                domainPeriod.Add(strDomain, 0);
                                domainText.Add(strDomain, string.Empty);

                                //領域補考成績
                                domainMakeUpScoreTotal.Add(strDomain, 0);
                            }

                            domainTotal[strDomain] += objSubj.Value.Value * objSubj.Weight.Value;
                            //科目的原始成績加總
                            domainOriginTotal[strDomain] += objSubj.ScoreOrigin.Value * objSubj.Weight.Value;

                            domainWeight[strDomain] += objSubj.Weight.Value;
                            domainPeriod[strDomain] += objSubj.Period.Value;
                            domainText[strDomain] += GetDomainSubjectText(strSubj, objSubj.Text);

                            //領域補考成績總和 ，算法為 先用各科目的"成績" 加權計算 ， 之後會再判斷， 此成績總和 是否含有"補考成績" 是否為"補考成績總和"
                            domainMakeUpScoreTotal[strDomain] += objSubj.Value.Value * objSubj.Weight.Value;
                        }
                    }
                    #endregion

                    #region 計算各領域加權平均。
                    /* (2014/11/18 補考調整)調整為保留領域成績中的資訊，但是移除多的領域成績項目。 */
                    // 從科目算過來清空原來領域成績，以科目成績的領域為主
                    //dscores.Clear();
                    foreach (string strDomain in domainTotal.Keys)
                    {
                        decimal total = domainTotal[strDomain];
                        decimal totalOrigin = domainOriginTotal[strDomain];

                        decimal weight = domainWeight[strDomain];
                        decimal period = domainPeriod[strDomain];
                        string text = "";

                        // 補考總分
                        decimal makeupScoreTotal = domainMakeUpScoreTotal[strDomain];

                        if (domainText.ContainsKey(strDomain))
                            text = string.Join(";", domainText[strDomain].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

                        if (weight <= 0) continue; //沒有權重就不計算，保留原來的成績。

                        decimal weightOriginAvg = rule.ParseDomainScore(totalOrigin / weight);

                        // 補考總分平均
                        decimal? makeup_score_total_Avg = rule.ParseDomainScore(makeupScoreTotal / weight);

                        //將成績更新回學生。
                        SemesterDomainScore dscore = null;
                        if (dscores.Contains(strDomain))
                            dscore = dscores[strDomain];
                        else
                        {
                            dscore = new SemesterDomainScore();
                            dscores.Add(strDomain, dscore);
                        }

                        //先將算好的成績帶入領域成績,後面的擇優判斷才不會有問題
                        dscore.ScoreOrigin = weightOriginAvg;
                        dscore.Weight = weight;
                        dscore.Period = period;
                        dscore.Text = text;
                        dscore.Effort = effortmap.GetCodeByScore(weightOriginAvg);
                        dscore.ScoreMakeup = haveMakeUpScoreDomains.Contains(strDomain) ? makeup_score_total_Avg : dscore.ScoreMakeup;//補考成績若無更新則保留原值
                        //填入dscore.Value
                        if (dscore.ScoreOrigin.HasValue || dscore.ScoreMakeup.HasValue)
                            dscore.BetterScoreSelection(setting.DomainScoreLimit);
                    }
                    #endregion
                    #region 高雄專用語文領域成績結算
                    //高雄市特有領域成績
                    if (Program.Mode == ModuleMode.KaoHsiung)
                    {
                        decimal total = 0;
                        decimal totalOrigin = 0;
                        decimal totalEffort = 0;

                        decimal weight = 0;
                        decimal period = 0;

                        bool hasMakeupScore = false;
                        foreach (var subDomain in new string[] { "國語文", "英語" })
                        {
                            if (dscores.Contains(subDomain))
                            {
                                var subScore = dscores[subDomain];
                                total += subScore.Value.Value * subScore.Weight.Value;
                                totalOrigin += subScore.ScoreOrigin.Value * subScore.Weight.Value;
                                totalEffort += subScore.Effort.Value * subScore.Weight.Value;
                                weight += subScore.Weight.HasValue ? subScore.Weight.Value : 0;
                                period += subScore.Period.HasValue ? subScore.Period.Value : 0;

                                if (subScore.ScoreMakeup.HasValue)
                                    hasMakeupScore = true;
                            }
                        }
                        if (weight > 0)
                        {
                            decimal weightValueAvg = rule.ParseDomainScore(total / weight);
                            decimal weightOriginAvg = rule.ParseDomainScore(totalOrigin / weight);
                            int effortAvg = Convert.ToInt32(decimal.Round(totalEffort / weight, 0, MidpointRounding.AwayFromZero));

                            var strDomain = "語文";
                            //將成績更新回學生。
                            SemesterDomainScore dscore = null;
                            if (dscores.Contains(strDomain))
                                dscore = dscores[strDomain];
                            else
                            {
                                dscore = new SemesterDomainScore();
                                dscores.Add(strDomain, dscore, 0);
                            }

                            //先將算好的成績帶入領域成績,後面的擇優判斷才不會有問題
                            dscore.ScoreOrigin = weightOriginAvg;
                            dscore.Weight = weight;
                            dscore.Period = period;
                            dscore.Text = "";
                            dscore.Effort = effortAvg;
                            dscore.ScoreMakeup = hasMakeupScore ? weightValueAvg : dscore.ScoreMakeup;//補考成績若無更新則保留原值
                            //填入dscore.Value
                            if (dscore.ScoreOrigin.HasValue || dscore.ScoreMakeup.HasValue)
                                dscore.BetterScoreSelection(setting.DomainScoreLimit);
                        }
                    }
                    #endregion
                    //清除不應該存在領域成績
                    //if (clearDomainScore)
                    if (setting.DomainScoreClear)
                    {
                        foreach (var domainName in dscores.ToArray())
                        {
                            //如果新計算的領域成績中不包含在原領域清單中，就移除他。
                            if (!domainTotal.ContainsKey(domainName))
                                dscores.Remove(domainName);
                        }
                    }
                }


                //計算課程學習成績。
                ScoreResult result = CalcDomainWeightAvgScore(dscores, new UniqueSet<string>());
                if (result.Score.HasValue)
                    semsscore.CourseLearnScore = rule.ParseLearnDomainScore(result.Score.Value);
                if (result.ScoreOrigin.HasValue)
                    semsscore.CourseLearnScoreOrigin = rule.ParseLearnDomainScore(result.ScoreOrigin.Value);

                //計算學習領域成績。
                result = CalcDomainWeightAvgScore(dscores, Util.VariableDomains);
                if (result.Score.HasValue)
                    semsscore.LearnDomainScore = rule.ParseLearnDomainScore(result.Score.Value);
                if (result.ScoreOrigin.HasValue)
                    semsscore.LearnDomainScoreOrigin = rule.ParseLearnDomainScore(result.ScoreOrigin.Value);
            }

        }

        /// <summary>
        /// 單純從學期科目成績的文字描述加總到學期領域成績的文字描述
        /// </summary>
        /// <param name="defaultRule"></param>
        public void SumDomainTextScore()
        {
            foreach (StudentScore student in Students)
            {
                SemesterScore semsscore = student.SemestersScore[SemesterData.Empty];
                SemesterDomainScoreCollection dscores = semsscore.Domain;
                SemesterSubjectScoreCollection jscores = semsscore.Subject;

                Dictionary<string, string> domainText = new Dictionary<string, string>();

                #region 總計各領域的總分、權重、節數。
                foreach (string strSubj in jscores)
                {
                    SemesterSubjectScore objSubj = jscores[strSubj];
                    string strDomain = objSubj.Domain.Injection();

                    //不計算的領域就不算。
                    if (!IsValidItem(strDomain)) continue;

                    if (objSubj.Value.HasValue && objSubj.Weight.HasValue && objSubj.Period.HasValue)
                    {
                        if (!domainText.ContainsKey(strDomain))
                        {
                            domainText.Add(strDomain, string.Empty);
                        }

                        domainText[strDomain] += GetDomainSubjectText(strSubj, objSubj.Text);
                    }
                }
                #endregion

                #region 計算各領域加權平均。
                foreach (string strDomain in domainText.Keys)
                {
                    string text = string.Join(";", domainText[strDomain].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

                    //將成績更新回學生。
                    SemesterDomainScore dscore = null;
                    if (dscores.Contains(strDomain))
                        dscore = dscores[strDomain];
                    else
                    {
                        dscore = new SemesterDomainScore();
                        dscores.Add(strDomain, dscore);
                    }

                    dscore.Text = text;
                }
                #endregion
            }
        }

        /// <summary>
        /// 計算學習領域成績
        /// </summary>
        /// <param name="defaultRule"></param>
        public void CalculateLearningDomainScore(ScoreCalculator defaultRule)
        {
            foreach (StudentScore student in Students)
            {
                SemesterScore semsscore = student.SemestersScore[SemesterData.Empty];
                SemesterDomainScoreCollection dscores = semsscore.Domain;
                ScoreCalculator rule = student.CalculationRule;

                if (rule == null)
                    rule = defaultRule;

                #region 計算各領域加權平均。
                //計算學習領域成績。
                //decimal? result = CalcDomainWeightAvgScore(dscores, Util.VariableDomains);
                ScoreResult result = CalcDomainWeightAvgScore(dscores, Util.VariableDomains); ;
                if (result.Score.HasValue)
                    semsscore.LearnDomainScore = rule.ParseLearnDomainScore(result.Score.Value);

                if (result.ScoreOrigin.HasValue)
                    semsscore.LearnDomainScoreOrigin = rule.ParseLearnDomainScore(result.ScoreOrigin.Value);
                #endregion

            }
        }

        private static ScoreResult CalcDomainWeightAvgScore(SemesterDomainScoreCollection dscores, UniqueSet<string> excludeItem)
        {
            decimal TotalScore = 0, TotalScoreOrigin = 0, TotalWeight = 0;
            foreach (string strDomain in dscores)
            {
                // 高雄語文只顯示不計算
                if (Program.Mode == ModuleMode.KaoHsiung)
                    if (strDomain == "語文")
                        continue;

                if (excludeItem.Contains(strDomain.Trim())) continue;

                SemesterDomainScore dscore = dscores[strDomain];
                if (dscore.Value.HasValue && dscore.Weight.HasValue) //dscore.Value 是原來的結構。
                {
                    TotalScore += (dscore.Value.Value * dscore.Weight.Value); //擇優成績。

                    if (dscore.ScoreOrigin.HasValue)
                        TotalScoreOrigin += (dscore.ScoreOrigin.Value * dscore.Weight.Value); //原始成績。
                    else
                        TotalScoreOrigin += (dscore.Value.Value * dscore.Weight.Value); //將擇優當成原始。

                    TotalWeight += dscore.Weight.Value; //比重不會因為哪種成績而不同。
                }
            }

            if (TotalWeight <= 0) return new ScoreResult(); //沒有成績。

            ScoreResult sr = new ScoreResult();

            sr.Score = TotalScore / TotalWeight;
            sr.ScoreOrigin = TotalScoreOrigin / TotalWeight;

            return sr;
        }

        private class ScoreResult
        {
            /// <summary>
            /// 成績，一般是擇優後的成績，大部份的報表都會使用此成績。
            /// </summary>
            public decimal? Score { get; set; }

            /// <summary>
            /// 原始成績，最原始的成績。
            /// </summary>
            public decimal? ScoreOrigin { get; set; }
        }

        private string GetDomainSubjectText(string subjName, string subjText)
        {
            if (!string.IsNullOrEmpty(subjText))
            {
                return string.Format("({0}){1};", subjName, subjText);
            }
            else
                return string.Empty;
        }

        private bool IsValidItem(string item)
        {
            if (Filter.Count <= 0) return true;

            return (Filter.Contains(item));
        }
    }
}
