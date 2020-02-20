using System.Collections.Generic;

namespace Core.Exceptions
{
    public class ValidationException : BaseException
    {
        public ValidationException() : base("Model could not be validated")
        {
            Extras = new List<Dictionary<string, string>>();

        }
        public ValidationException(List<Dictionary<string, string>> errors) : base("Model could not be validated.")
        {
            Extras = errors;
        }

        public ValidationException(string key, List<string> errors)
            : base("Model could not be validated.s")
        {
            var dictionary = new Dictionary<string, string>();
            errors.ForEach(t => dictionary.Add(key, t));
            Extras = new List<Dictionary<string, string>> { dictionary };
        }
    }
}
