using System;
using System.Collections.Generic;
using System.Text;

namespace JHEvaluation.StudentScoreSummaryReport
{
    internal struct RowHeader
    {
        public RowHeader(string domain, string subject)
            : this()
        {
            Subject = subject;
            Domain = domain;
            Index = -1;
            IsDomain = true;
        }

        public string Domain { get; set; }

        public string Subject { get; private set; }

        public int Index { get; set; }

        public bool IsDomain { get; set; }
    }
}
