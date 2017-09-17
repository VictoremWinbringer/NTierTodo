using System;
using System.Collections.Generic;
using NTierTodo.Bll.Dto;

namespace NTierTodo.Bll.Abstract
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
