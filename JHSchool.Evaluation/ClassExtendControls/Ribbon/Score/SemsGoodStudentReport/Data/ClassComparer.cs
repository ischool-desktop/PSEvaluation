using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon.Score.SemsGoodStudentReport.Data
{
    public class ClassComparer:IComparer<ClassData>
    {
        #region IComparer<ClassData> 成員

        public int Compare(ClassData x, ClassData y)
        {
            int xGradeYear,yGradeYear,xDisplayOrder,yDisplayOrder;

            xGradeYear = x.ClassRecord.GradeYear.GetValueOrDefault(int.MaxValue);

            yGradeYear = y.ClassRecord.GradeYear.GetValueOrDefault(int.MaxValue);

            //if (!int.TryParse(x.ClassRecord.GradeYear, out xGradeYear))
            //    xGradeYear = int.MaxValue;
            //if (!int.TryParse(y.ClassRecord.GradeYear, out yGradeYear))
            //    yGradeYear = int.MaxValue;
            if (!int.TryParse(x.ClassRecord.DisplayOrder, out xDisplayOrder))
                xDisplayOrder = int.MaxValue;
            if (!int.TryParse(y.ClassRecord.DisplayOrder, out yDisplayOrder))
                yDisplayOrder = int.MaxValue;

            if (xGradeYear == yGradeYear)            
                return xDisplayOrder.CompareTo(yDisplayOrder);             
            else            
                return xGradeYear.CompareTo(yGradeYear);            
        }

        #endregion
    }
}
