using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Data;

namespace JHEvaluation.ScoreCalculation
{
    //2016/6/6 穎驊筆記，此為專處理重覆科目計算的Exception
    internal class DuplicatedSubjectException : Exception
    {
        public List<List<JHSCAttendRecord>> DuplicatedList { get; private set; }

 

        public DuplicatedSubjectException(List<List<JHSCAttendRecord>> JHSCATR)
        {
            DuplicatedList = JHSCATR;
        }


    }
}
