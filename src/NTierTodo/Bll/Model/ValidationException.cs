using System;

namespace NTierTodo.Bll.Model
{
    public class ValidationException : Exception
    {
        public string Property { get; }

        public ValidationException(string property, string message) : base(message)
        {
            Property = property;
        }
    }
}
