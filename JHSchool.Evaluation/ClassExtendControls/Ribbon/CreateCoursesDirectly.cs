using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Evaluation.ClassExtendControls.Ribbon.CreateCoursesRelated;
using JHSchool.Data;

namespace JHSchool.Evaluation.ClassExtendControls.Ribbon
{
    public class CreateCoursesDirectly
    {
        public CreateCoursesDirectly()
        {
            List<JHClassRecord> list = JHClass.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource);
            CreateClassCourseForm form = new CreateClassCourseForm(list);
            form.ShowDialog();
        }
    }
}
