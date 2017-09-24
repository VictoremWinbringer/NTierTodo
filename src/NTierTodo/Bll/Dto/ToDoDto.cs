using System;

namespace NTierTodo.Bll.Dto
{
    public class ToDoDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
    }
}
