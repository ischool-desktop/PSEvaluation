using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.StudentExtendControls.Ribbon.Ranking.Semester
{
    class RDomainScore
    {
        public RDomainScore(K12.Data.DomainScore score)
        {
            Name = score.Domain;
            Score = score.Score;
        }

        public RDomainScore(string domainName, decimal? score)
        {
            Name = domainName;
            Score = score;
        }

        public string Name { get; private set; }

        public decimal? Score { get; private set; }

        public bool HasScore { get { return Score.HasValue; } }

        public override string ToString()
        {
            return Name;
        }
    }
}
