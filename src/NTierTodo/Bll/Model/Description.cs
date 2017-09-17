namespace NTierTodo.Bll.Model
{
    public struct Description
    {
        public string Value { get; }

        public Description(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(nameof(Description), "Is null or empty");

            if (value.Length < 3)
                throw new ValidationException(nameof(Description), "length < 3");

            this.Value = value.Trim();
        }
    }
}