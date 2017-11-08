using System.Xml;

namespace JHEvaluation.AssignmentExamManager
{
    public class AssignmentExamSetupRecord
    {
        public string SubExamID { get; set; }

        public string Name { get; set; }

        public void Load(XmlElement data)
        {
            SubExamID = data.GetAttribute("SubExamID");
            Name = data.GetAttribute("Name");
        }
    }
}