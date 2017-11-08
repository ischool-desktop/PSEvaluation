using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JHSchool.Evaluation.EduAdminExtendControls.Ribbon.ProgramPlanRelated
{
    public interface IGraduationPlanEditor
    {
        void SetSource(XmlElement source);
        XmlElement GetSource();
        bool IsDirty { get; }
        event EventHandler IsDirtyChanged;
        bool IsValidated
        {
            get;
        }
    }
}
