using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using FluentValidation.AspNetCore;
using NTierTodo.Bll;
using NTierTodo.Filters;

namespace NTierTodo.Controllers
{
    [ValidateModel]
    [Route("api/v1/[controller]")]
    public class ToDoController : Controller
    {
        private readonly IToDoManager _manager;

        public ToDoController(IToDoManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_manager.GetAll().ToList());
        }

        [HttpGet("{id}")]
        [ValidateTodoExists]
        public IActionResult Get(Guid id)
        {
            return Ok(_manager.Get(id));
        }


        [HttpPost]
        public IActionResult Post([FromBody]ToDoDto value)
        {
            return Ok(_manager.Create(value));
        }

        [HttpPost("{id}/[action]")]
        [ValidateTodoExists]
        public IActionResult MakeComplete(Guid id)
        {
            _manager.MakeComplite(id);

            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult Count()
        {
            return Ok(_manager.GetAll().Count());
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [ValidateTodoExists]
        public IActionResult Put(Guid id, [FromBody, /*CustomizeValidator(RuleSet = "Update")*/]ToDoDto value)
        {
            value.Id = id;

            _manager.Update(value);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _manager.Delete(id);

            return Ok();
        }
    }
}
