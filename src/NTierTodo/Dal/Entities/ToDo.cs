using System;

namespace NTierTodo.Dal.Entities
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsComplite { get; set; }
    }
}
