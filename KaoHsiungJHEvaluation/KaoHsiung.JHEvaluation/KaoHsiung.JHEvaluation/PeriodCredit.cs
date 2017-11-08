//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace JHSchool.Evaluation
//{
//    /// <summary>
//    /// 只用來處理節數/權數。
//    /// </summary>
//    public class PeriodCredit
//    {
//        public decimal Period { get; set; }
//        public decimal Credit { get; set; }
//        public string Error { get; private set; }

//        public PeriodCredit()
//        {
//            Period = 0;
//            Credit = 0;
//            Error = string.Empty;
//        }

//        public PeriodCredit(decimal p, decimal c)
//            : this()
//        {
//            Period = p;
//            Credit = c;
//        }

//        public bool Parse(string pc)
//        {
//            bool success = true;

//            if (pc.Contains("/"))
//            {
//                #region 檢查 P/C 格式
//                string[] parts = pc.Split('/');
//                if (parts.Length == 2)
//                {
//                    string period = parts[0];
//                    string credit = parts[1];

//                    bool period_valid = IsValid(period);
//                    bool credit_valid = IsValid(credit);

//                    if (!period_valid && !credit_valid)
//                    {
//                        success = false;
//                        Error = "節數/權數必須為數值";
//                    }
//                    else if (!period_valid)
//                    {
//                        success = false;
//                        Error = "節數必須為數值";
//                    }
//                    else if (!credit_valid)
//                    {
//                        success = false;
//                        Error = "權數必須為數值";
//                    }
//                    else
//                    {
//                        Period = decimal.Parse(period);
//                        Credit = decimal.Parse(credit);
//                    }
//                }
//                else
//                {
//                    success = false;
//                    Error = "格式錯誤";
//                }
//                #endregion
//            }
//            else
//                if (IsValid(pc)) Period = Credit = decimal.Parse(pc);

//            return success;
//        }

//        private bool IsValid(string s)
//        {
//            decimal i;
//            if (decimal.TryParse(s, out i))
//                return true;
//            else
//                return false;
//        }

//        public override string ToString()
//        {
//            if (Period == Credit)
//                return "" + Period;
//            else
//                return "" + Period + "/" + Credit;
//        }

//        public static PeriodCredit operator +(PeriodCredit a, PeriodCredit b)
//        {
//            return new PeriodCredit(a.Period + b.Period, a.Credit + b.Credit);
//        }
//    }
//}
