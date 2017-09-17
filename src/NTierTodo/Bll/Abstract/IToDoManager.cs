using NTierTodo.Bll.Model;
using System;
using System.Collections.Generic;

namespace NTierTodo.Bll.Abstract
{
    public interface IToDoManager
    {
        ToDoModel Get(Guid id);
        IEnumerable<ToDoModel> GetAll();
        ToDoModel Create(ToDoModel todo);
        void Update(ToDoModel todo);
        void Delete(Guid id);
        void MakeComplite(Guid id);
    }
}
