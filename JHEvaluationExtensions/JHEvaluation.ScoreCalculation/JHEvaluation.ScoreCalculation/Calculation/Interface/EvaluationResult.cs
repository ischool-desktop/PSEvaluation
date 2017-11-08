using System.Collections.Generic;

namespace JHSchool.Evaluation.Calculation
{
    /// <summary>
    /// 畢業判斷結果
    /// </summary>
    public class EvaluationResult : Dictionary<string, List<ResultDetail>>
    {
        /// <summary>
        /// 以年級及學期為鍵值來合併內容
        /// </summary>
        /// <param name="id"></param>
        /// <param name="results"></param>
        public void MergeResults(string id, IEnumerable<ResultDetail> results)
        {
            if (!this.ContainsKey(id))
                this.Add(id, new List<ResultDetail>());

            Dictionary<string, ResultDetail> rd_dict = new Dictionary<string, ResultDetail>();

            //將results整理成以年級及學期為鍵值
            foreach (ResultDetail rd in this[id])
                rd_dict.Add(rd.GradeYear + "_" + rd.Semester, rd);

            foreach (ResultDetail rd in results)
            {
                //若沒有對應的年級及學期則新增
                if (!rd_dict.ContainsKey(rd.GradeYear + "_" + rd.Semester))
                    rd_dict.Add(rd.GradeYear + "_" + rd.Semester, new ResultDetail(id, rd.GradeYear, rd.Semester));

                //加入訊息及訊息細節
                rd_dict[rd.GradeYear + "_" + rd.Semester].AddMessages(rd.Messages);
                rd_dict[rd.GradeYear + "_" + rd.Semester].AddDetails(rd.Details);
            }

            //將ResultDetail清除並重新加入
            this[id].Clear();
            this[id].AddRange(rd_dict.Values);
        }
    }
}