using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using SmartSchool.Feature.Course;
using JHSchool.Evaluation.Feature.Legacy;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.CalculationWizard
{
    class SCETake
    {
        private string _identity, _scattend_id, _exam_id, _score;

        public SCETake(XmlElement data)
        {
            DSXmlHelper obj = new DSXmlHelper(data);
            _identity = obj.GetText("@ID");
            _exam_id = obj.GetText("ExamID");
            _score = obj.GetText("Score");
            _scattend_id = obj.GetText("AttendID");
        }

        public string Identity
        {
            get { return _identity; }
        }

        public string ExamId
        {
            get { return _exam_id; }
        }

        public string SCAttendId
        {
            get { return _scattend_id; }
        }

        public string Score
        {
            get { return _score; }
        }

        public static SCETakeCollection GetSCETakes(IProgressUI progress, params string[] courseIds)
        {
            SCETakePackingLoader loader = new SCETakePackingLoader(progress, courseIds);
            return loader.LoadData();
        }
    }

    class SCETakeCollection : Dictionary<string, SCETake>
    {
        public void Add(SCETake scetake)
        {
            Add(scetake.Identity, scetake);
        }
    }

    /// <summary>
    /// 每個修課記錄對於每個評量只有唯的成績。
    /// </summary>
    class ExamScoreCollection : Dictionary<string, SCETake>
    {
        public void Add(SCETake scetake)
        {
            Add(scetake.ExamId, scetake);
        }
    }

    class SCETakePackingLoader
    {
        private const int PackingSize = 30;

        private IProgressUI _progress;
        private List<List<string>> _packings;

        public SCETakePackingLoader(IProgressUI progress, string[] courseIds)
        {
            _progress = progress;

            _packings = new List<List<string>>();
            List<string> package = new List<string>();
            for (int i = 0; i < courseIds.Length; i++)
            {
                if (i % PackingSize == 0)
                {
                    package = new List<string>();
                    _packings.Add(package);
                }
                package.Add(courseIds[i]);
            }
        }

        public SCETakeCollection LoadData()
        {
            int current = 0;
            SCETakeCollection includes = new SCETakeCollection();
            foreach (List<string> each in _packings)
            {
                current++;

                if (_progress.Cancellation) break;

                _progress.ReportProgress(string.Format("下載成績相關資料({0}%)", Math.Round(((float)current / (float)_packings.Count) * 100, 0)), 0);
                XmlElement xmlRecords = QueryCourse.GetSECTake(each.ToArray()).GetContent().BaseElement;

                foreach (XmlElement attend in xmlRecords.SelectNodes("Score"))
                {
                    SCETake include = new SCETake(attend);
                    includes.Add(include);
                }
            }

            return includes;
        }
    }
}
