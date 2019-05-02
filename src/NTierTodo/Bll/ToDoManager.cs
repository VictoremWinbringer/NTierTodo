using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using NTierTodo.Bll.Exception;
using NTierTodo.Dal.Abstract;
using NTierTodo.Dal.Entities;
using NTierTodo.SignalR;

namespace NTierTodo.Bll
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

        public ToDoDto Get(Guid id)
        {
            var todo = _repository[id];

            if (todo == null)
                return null;

            return _mapper.Map<ToDoDto>(todo);
        }

        public IEnumerable<ToDoDto> GetAll()
        {
            return _repository.Select(todo => _mapper.Map<ToDoDto>(todo));
        }

        public ToDoDto Create(ToDoDto todo)
        {
            var memento = _mapper.Map<ToDo>(todo);

            var id = Guid.NewGuid();

            memento.Id = id;
            
            if(string.IsNullOrWhiteSpace(todo.Description))
                throw new TodoValidationException(nameof(todo.Description));

            _repository.Create(memento);

            return this.Get(id);
        }

        public void Update(ToDoDto todo)
        {
            var memento = _repository[todo.Id];

            //Only MakeComplite can update IsComplite prop
            if(string.IsNullOrWhiteSpace(todo.Description))
                throw new TodoValidationException(nameof(todo.Description));
            
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

            if (todo.IsComplete)
                return;

            todo.IsComplete = true;

            _repository.Update(todo);

            _notifier.Clients.All.InvokeAsync("Notify", todo.Description + " is complete");
        }
    }
}
