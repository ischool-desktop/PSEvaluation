using System;
using System.Collections.Generic;
using System.Text;

namespace KaoHsiung.ReaderScoreImport.Mapper
{
    internal abstract class CodeMapper
    {
        protected Dictionary<string, string> CodeMap;

        public CodeMapper()
        {
            CodeMap = new Dictionary<string, string>();
            LoadCodes();
        }

        protected virtual void LoadCodes()
        {
            CodeMap.Clear();
        }

        public void Reload()
        {
            LoadCodes();
        }

        public bool CheckCodeExists(string code)
        {
            return (CodeMap.ContainsKey(code));
        }

        public string Map(string code)
        {
            if (CodeMap.ContainsKey(code))
                return CodeMap[code];
            else
                return string.Empty;
        }
    }
}
