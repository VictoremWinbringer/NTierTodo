using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NTierTodo.Dal.Entities;

namespace NTierTodo.Dal.Concrete
{
    internal sealed class ToDoRepository : IToDoRepository
    {
        private readonly string _connection;

        public ToDoRepository(string connection)
        {
            _connection = connection;
        }
        public ToDo this[Guid id]
        {
            get
            {
                using (var db = new LiteDatabase(_connection))
                {
                    var todos = db.GetCollection<ToDo>(nameof(ToDo));

                    return todos.FindOne(x => x.Id == id);
                }
            }
        }

        private IEnumerable<ToDo> All()
        {
            using (var db = new LiteDatabase(_connection))
            {
                var todos = db.GetCollection<ToDo>(nameof(ToDo));

                return todos.FindAll().ToArray();
            }
        }

        public void Create(ToDo todo)
        {
            using (var db = new LiteDatabase(_connection))
            {
                var todos = db.GetCollection<ToDo>(nameof(ToDo));

                todos.Insert(todo);
            }
        }

        public void Update(ToDo todo)
        {
            using (var db = new LiteDatabase(_connection))
            {
                var todos = db.GetCollection<ToDo>(nameof(ToDo));

                todos.Update(todo);
            }
        }

        public void Delete(Guid id)
        {
            using (var db = new LiteDatabase(_connection))
            {
                var todos = db.GetCollection<ToDo>(nameof(ToDo));

                todos.Delete(x => x.Id == id);
            }
        }

        public IEnumerator<ToDo> GetEnumerator()
        {
            return this.All().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
