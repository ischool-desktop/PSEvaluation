using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking
{
    public class Rank
    {
        public Rank()
        {
            Sequence = false;
        }

        public bool Sequence { get; set; }

        public RankData Perform(RankData source)
        {
            List<decimal> score_list = new List<decimal>();
            foreach (RankScore rs in source.Values)
                score_list.Add(rs.Score);
            score_list.Sort(delegate(decimal a, decimal b)
            {
                return b.CompareTo(a);
            });

            List<string> id_list = new List<string>(new string[source.Count]);
            foreach (string id in source.Keys)
            {
                int index = score_list.IndexOf(source[id].Score);
                if (index < 0) throw new Exception("排名發生錯誤");
                while (!string.IsNullOrEmpty(id_list[index])) index++;
                id_list[index] = id;
            }

            int rank = 0;
            int prank = 1;

            decimal last = decimal.MinValue;
            foreach (string id in id_list)
            {
                if (source[id].Score == last)
                {
                    if (!Sequence) rank++;
                    source[id].Rank = prank;
                }
                else
                {
                    rank++;
                    source[id].Rank = rank;
                    prank = rank;
                }

                last = source[id].Score;
                //prank = rank;
            }

            return source;
        }

        public static void Test()
        {
            Dictionary<string, decimal> aa = new Dictionary<string, decimal>();
            aa.Add("a", 70);
            aa.Add("b", 80);
            aa.Add("c", 50);
            aa.Add("d", 90);
            aa.Add("q", 90);
            aa.Add("e", 30);

            Rank ranking = new Rank();
            ranking.Sequence = false;
            //ranking.Perform(aa);


        }
    }
}
