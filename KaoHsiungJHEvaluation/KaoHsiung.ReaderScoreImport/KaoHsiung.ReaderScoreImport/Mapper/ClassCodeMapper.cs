using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using KaoHsiung.ReaderScoreImport.UDT;

namespace KaoHsiung.ReaderScoreImport.Mapper
{
    internal class ClassCodeMapper : CodeMapper
    {
        private static CodeMapper _instance;

        public static CodeMapper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ClassCodeMapper();
                return _instance;
            }
        }

        private ClassCodeMapper()
        {
        }

        protected override void LoadCodes()
        {
            base.LoadCodes();

            AccessHelper helper = new AccessHelper();

            foreach (ClassCode item in helper.Select<ClassCode>())
            {
                if (!CodeMap.ContainsKey(item.Code))
                    CodeMap.Add(item.Code, item.ClassName);
            }
        }
    }
}
