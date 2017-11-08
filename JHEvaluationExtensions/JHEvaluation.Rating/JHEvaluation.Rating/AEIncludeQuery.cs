using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace JHEvaluation.Rating
{
    class AEIncludeQuery
    {
        /// <summary>
        /// RefAssessmentSetupID,ExamID,null...
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> AEIncludeData { get; set; }

        public AEIncludeQuery()
        {
            AEIncludeData = new Dictionary<string, Dictionary<string, string>>();
            List<JHAEIncludeRecord> aeincludes = JHAEInclude.SelectAll();
            foreach (JHAEIncludeRecord each in aeincludes)
            {
                if (!AEIncludeData.ContainsKey(each.RefAssessmentSetupID))
                    AEIncludeData.Add(each.RefAssessmentSetupID, new Dictionary<string, string>());

                AEIncludeData[each.RefAssessmentSetupID].Add(each.RefExamID, null);
            }
        }

        public bool Contains(string refAssessmentSetupID, string refExamId)
        {
            if (AEIncludeData.ContainsKey(refAssessmentSetupID))
                return (AEIncludeData[refAssessmentSetupID].ContainsKey(refExamId));
            else
                return false;
        }
    }
}
