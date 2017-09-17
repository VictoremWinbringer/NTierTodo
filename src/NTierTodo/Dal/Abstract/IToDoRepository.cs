using NTierTodo.Dal.Entity;
using System;
using System.Collections.Generic;

namespace NTierTodo.Dal.Abstract
{
    public interface IToDoRepository
    {
        ToDo this[Guid id] { get; }
        IEnumerable<ToDo> All();
        void Create(ToDo todo);
        void Update(ToDo todo);
        void Delete(Guid id);
    }
}
