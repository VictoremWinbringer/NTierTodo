using System.Linq;
using FluentValidation;
using NTierTodo.Bll.Dto;
using NTierTodo.Dal.Abstract;

namespace NTierTodo.Bll.Validator
{
    public class TodoValidator : AbstractValidator<ToDoDto>
    {
        private const int MIN_DESCRIPTION_LENGTH = 3;
        private const int MAX_DESCRIPTION_LENGTH = 255;

        public TodoValidator(IToDoRepository repository)
        {
            RuleFor(t => t.Description).NotEmpty()
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("IsNullOrEmpty"), nameof(ToDoDto.Description)));
            RuleFor(t => t.Description).MinimumLength(MIN_DESCRIPTION_LENGTH)
               .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("MinLength"),
                    nameof(ToDoDto.Description), MIN_DESCRIPTION_LENGTH));
            RuleFor(t => t.Description).MaximumLength(MAX_DESCRIPTION_LENGTH)
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("MaxLength"), nameof(ToDoDto.Description), MAX_DESCRIPTION_LENGTH));
            RuleFor(t => t.Description).Must(t => repository.All().All(todo => todo.Description != t))
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("NotUnique"), nameof(ToDoDto.Description)));
        }
    }
}