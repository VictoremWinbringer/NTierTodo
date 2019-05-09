namespace NTierTodo.Bll.Exception
{
    public class TodoValidationException : System.Exception
    {
        public string Property { get; }

        public TodoValidationException(string property)
        {
            Property = property;
        }
    }
}
