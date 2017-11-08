using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    class StandardFactory
    {
        private static Dictionary<string, IStandard> _standards;
        public static IStandard getInstance(string standardName)
        {
            if (_standards == null)
            {
                _standards = new Dictionary<string, IStandard>();

                NormalStandard standard = new NormalStandard();
                _standards.Add(standard.StandardName, standard);
            }
            if (_standards.ContainsKey(standardName))
                return _standards[standardName];
            else
                return _standards["Normal"];
        }
    }
}
