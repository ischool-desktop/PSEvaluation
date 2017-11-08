using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using KaoHsiung.ReaderScoreImport.UDT;

namespace KaoHsiung.ReaderScoreImport.Mapper
{
    internal class ExamCodeMapper : CodeMapper
    {
        private static CodeMapper _instance;

        public static CodeMapper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ExamCodeMapper();
                return _instance;
            }
        }

        private ExamCodeMapper()
        {
        }

        protected override void LoadCodes()
        {
            base.LoadCodes();

            AccessHelper helper = new AccessHelper();

            foreach (ExamCode item in helper.Select<ExamCode>())
            {
                if (!CodeMap.ContainsKey(item.Code))
                    CodeMap.Add(item.Code, item.ExamName);
            }
        }
    }
}
