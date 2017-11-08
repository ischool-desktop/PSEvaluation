using FISCA.Presentation;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace JHSchool.Evaluation.StudentExtendControls
{
    /// <summary>
    /// 這個 class 只是為了可以讓「學期成績」的權限優先載入。
    /// </summary>
    [FCode("JHSchool.Student.Detail0050", "學期成績")]
    internal class SemesterScoreItem : DetailContent
    {
        public SemesterScoreItem()
        {

        }
    }
}
