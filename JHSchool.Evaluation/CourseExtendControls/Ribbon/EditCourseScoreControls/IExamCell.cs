using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Evaluation.CourseExtendControls.Ribbon.EditCourseScoreControls
{
    interface IExamCell : ICell
    {
        IStandard Standard { get;}
        void SetValue(string value);
        bool IsDirty { get;}
        void Reset();
        bool IsValid();
        string Key { get;}
        string DefaultValue { get;}
    }

    abstract class AbstractExamCell : IExamCell
    {
        private string _oldValue;
        private string _nowValue;
        private IStandard _standard;
        private string _key;

        public string DefaultValue
        {
            get { return _oldValue; }
        }

        protected void OnInitialized(ScoreInfo scoreInfo)
        {
            _oldValue = scoreInfo.Score;
            _nowValue = scoreInfo.Score;
            _key = scoreInfo.ID;
            _standard = StandardFactory.getInstance("Normal");
        }

        #region IExamCell 成員

        public string Key
        {
            get { return _key; }
        }

        public virtual IStandard Standard
        {
            get { return _standard; }
        }

        public virtual void SetValue(string value)
        {
            _nowValue = value;
        }

        public virtual bool IsDirty
        {
            get { return _oldValue != _nowValue; }
        }

        public virtual void Reset()
        {
            _nowValue = _oldValue;
        }

        public virtual bool IsValid()
        {
            decimal d;
            if (!string.IsNullOrEmpty(_nowValue) && !decimal.TryParse(_nowValue, out d) && _nowValue != "缺")
                return false;
            return true;
        }
        #endregion

        #region ICell 成員

        public string GetValue()
        {
            return _nowValue;
        }

        #endregion
    }

    class ExamCell : AbstractExamCell
    {
        public ExamCell()
        {
            OnInitialized(new ScoreInfo());
        }

        public ExamCell(ScoreInfo scoreInfo)
        {
            OnInitialized(scoreInfo);
        }
    }

    class ScoreExamCell : AbstractExamCell
    {
        public ScoreExamCell()
        {
            OnInitialized(new ScoreInfo());
        }

        public ScoreExamCell(ScoreInfo scoreInfo)
        {
            OnInitialized(scoreInfo);
        }

        public override bool IsValid()
        {
            decimal d;
            if (!string.IsNullOrEmpty(GetValue()) && !decimal.TryParse(GetValue(), out d))
                return false;
            return true;        
        }
    }
}
