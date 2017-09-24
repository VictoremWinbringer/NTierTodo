
using System;
using System.Collections.Generic;
using NTierTodo.Dal.Entities;

namespace NTierTodo.Dal.Abstract
{
    public interface IToDoRepository : IEnumerable<ToDo>
    {
        ToDo this[Guid id] { get; }
        void Create(ToDo todo);
        void Update(ToDo todo);
        void Delete(Guid id);
    }
}
