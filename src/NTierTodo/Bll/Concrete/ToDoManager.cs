using NTierTodo.Bll.Abstract;
using NTierTodo.Dal.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using NTierTodo.Bll.Dto;
using NTierTodo.Bll.Exception;
using NTierTodo.Dal.Entities;
using NTierTodo.SignalR;

namespace NTierTodo.Bll.Concrete
{
    public class ToDoManager : IToDoManager
    {
        private readonly IToDoRepository _repository;
        private readonly IHubContext<Notifier> _notifier;
        private readonly IMapper _mapper;

        public ToDoManager(IToDoRepository repository, IHubContext<Notifier> notifier, IMapper mapper)
        {
            _repository = repository;
            _notifier = notifier;
            _mapper = mapper;
        }

        private void Validate(Guid id)
        {
            if (id == default(Guid))
                throw new ValidationException(nameof(id), "Is default!");
        }

        private void Validate(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ValidationException(nameof(description), "Is null or empty");

            if (description.Length < 3)
                throw new ValidationException(nameof(description), "length < 3");
        }

        private void ValidateNotNull(ToDoDto todo)
        {
            if (todo == null)
                throw new ValidationException(" ", $"todo is null");
        }

        public ToDoDto Get(Guid id)
        {
            Validate(id);

            var todo = _repository[id];

            if (todo == null)
                return null;

            return _mapper.Map<ToDoDto>(todo);
        }

        public IEnumerable<ToDoDto> GetAll()
        {
            return _repository.All().Select(todo => _mapper.Map<ToDoDto>(todo));
        }

        public ToDoDto Create(ToDoDto todo)
        {
            ValidateNotNull(todo);
            Validate(todo.Description);

            var memento = _mapper.Map<ToDo>(todo);

            var id = Guid.NewGuid();

            memento.Id = id;

            _repository.Create(memento);

            return this.Get(id);
        }

        public void Update(ToDoDto todo)
        {
            ValidateNotNull(todo);
            Validate(todo.Id);
            Validate(todo.Description);

            var memento = _repository[todo.Id];

            //Only MakeComplite can update IsComplite prop
            memento.Description = todo.Description;

            _repository.Update(memento);
        }

        public void Delete(Guid id)
        {
            _repository.Delete(id);
        }

        public void MakeComplite(Guid id)
        {
            var todo = _repository[id];

            if (todo == null)
                throw new ValidationException(" ", $"todo with id {id} not found");

            todo.IsComplite = true;

            _repository.Update(todo);

            _notifier.Clients.All.InvokeAsync("Notify", todo.Description + " is complete");
        }
    }
}
