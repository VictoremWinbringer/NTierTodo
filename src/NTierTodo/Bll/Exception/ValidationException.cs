namespace NTierTodo.Bll.Exception
{
    public class ValidationException : System.Exception
    {
        public string Property { get; }

        public ValidationException(string property, string message) : base(message)
        {
            Property = property;
        }
    }
}
