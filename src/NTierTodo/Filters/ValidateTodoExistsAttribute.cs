using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NTierTodo.Bll;

namespace NTierTodo.Filters
{
    public class ValidateTodoExistsAttribute : TypeFilterAttribute
    {
        public ValidateTodoExistsAttribute() : base(typeof
            (ValidateTodoExistsFilterImpl))
        {
        }

        private class ValidateTodoExistsFilterImpl : ActionFilterAttribute
        {
            private readonly IToDoManager _manager;

            public ValidateTodoExistsFilterImpl(IToDoManager manager)
            {
                _manager = manager;
            }

            public override void OnActionExecuting(ActionExecutingContext context)
            {
                if (context.ActionArguments.ContainsKey("id"))
                {
                    var id = context.ActionArguments["id"] as Guid?;
                    if (id.HasValue)
                    {
                        if ((_manager.GetAll()).All(
                            a => a.Id != id.Value))
                        {
                            context.Result = new NotFoundObjectResult(id.Value);
                        }
                    }
                }
            }
        }
    }
}