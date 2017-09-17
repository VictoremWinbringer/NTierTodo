using System;

namespace NTierTodo.Bll.Model
{
    public class ToDoModel
    {
        public Guid Id { get; set; }
        public Description Description { get; set; }
        public bool IsComplite { get; set; }

        public void MakeComplite()
        {
            IsComplite = true;
        }

    }
}
