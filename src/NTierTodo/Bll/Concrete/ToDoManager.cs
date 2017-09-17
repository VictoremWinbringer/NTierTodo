using NTierTodo.Bll.Abstract;
using NTierTodo.Bll.Model;
using NTierTodo.Dal.Abstract;
using NTierTodo.Dal.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using NTierTodo.SignalR;

namespace NTierTodo.Bll.Concrete
{
    public class ToDoManager : IToDoManager
    {
        private readonly IToDoRepository _repository;
        private readonly IHubContext<Notifier> _notifier;

        public ToDoManager(IToDoRepository repository, IHubContext<Notifier> notifier)
        {
            _repository = repository;
            _notifier = notifier;
        }
        public ToDoModel Get(Guid id)
        {
            if (id == default(Guid))
                throw new ValidationException(nameof(ToDoModel.Id), "Is default!");

            var todo = _repository[id];

            if (todo == null)
                return null;

            return new ToDoModel
            {
                Description = new Description(todo.Description),
                Id = todo.Id,
                IsComplite = todo.IsComplite
            };
        }

        public IEnumerable<ToDoModel> GetAll()
        {
            return _repository.All().Select(todo => new ToDoModel
            {
                Description = new Description(todo.Description),
                Id = todo.Id,
                IsComplite = todo.IsComplite
            });
        }

        public ToDoModel Create(ToDoModel todo)
        {
            var id = Guid.NewGuid();

            _repository.Create(new ToDo
            {
                Description = todo.Description.Value,
                Id = id,
                IsComplite = false,
            });

            return this.Get(id);
        }

        public void Update(ToDoModel todo)
        {
            if (todo.Id == default(Guid))
                throw new ValidationException(nameof(ToDoModel.Id), "Is default!");

            _repository.Update(new ToDo
            {
                Description = todo.Description.Value,
                Id = todo.Id,
                IsComplite = false,
            });
        }

        public void Delete(Guid id)
        {
            _repository.Delete(id);
        }

        public void MakeComplite(Guid id)
        {
            var todo = Get(id);

            if (todo == null)
                throw new ValidationException(" ", $"todo with id {id} not found");

            todo.MakeComplite();

            _repository.Update(new ToDo
            {
                Description = todo.Description.Value,
                Id = todo.Id,
                IsComplite = todo.IsComplite
            });

            _notifier.Clients.All.InvokeAsync("Notify", todo.Description.Value + " is complete");
        }
    }
}
