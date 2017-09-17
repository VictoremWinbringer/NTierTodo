using System;

namespace NTierTodo.ViewModels
{
    public class ToDoVm
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsComplite { get; set; }
    }
}
