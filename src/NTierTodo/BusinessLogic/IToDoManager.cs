using System;
using System.Collections.Generic;

namespace NTierTodo.Bll
{
    public interface IToDoManager
    {
        ToDoDto Get(Guid id);
        IEnumerable<ToDoDto> GetAll();
        ToDoDto Create(ToDoDto todo);
        void Update(ToDoDto todo);
        void Delete(Guid id);
        void MakeComplite(Guid id);
    }
}
