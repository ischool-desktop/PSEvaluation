using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaoHsiung.ReaderScoreImport.Validation
{
    internal sealed class DataValidator<T>
    {
        private List<IRecordValidator<T>> _validators;
        //public event EventHandler<ValidationEventArgs<T>> RecordValidated;
        //public event EventHandler<ValidationEventArgs<T>> RecordWarning;
        //public event EventHandler<ValidationEventArgs<T>> RecordError;

        public DataValidator()
        {
            _validators = new List<IRecordValidator<T>>();
        }

        public void Register(IRecordValidator<T> validator)
        {
            _validators.Add(validator);
        }

        public List<string> Validate(T record)
        {
            List<string> errors = new List<string>();

            foreach (IRecordValidator<T> validator in _validators)
            {
                string error = validator.Validate(record);
                if (!string.IsNullOrEmpty(error))
                    errors.Add(error);
            }

            return errors;
            //ValidationEventArgs<T> args = new ValidationEventArgs<T>(record, results);
            //if (RecordError)
            //    RecordError.Invoke(this, args);
        }
    }

    //internal class ValidationEventArgs<T> : EventArgs
    //{
    //    public ValidationEventArgs(T record, List<ValidResult> results)
    //    {
    //        Record = record;
    //        Results = results;
    //    }

    //    public T Record { get; private set; }
    //    public List<ValidResult> Results { get; private set; }
    //}
}
