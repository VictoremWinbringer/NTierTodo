using Microsoft.AspNetCore.Mvc;
using NTierTodo.Bll.Abstract;
using NTierTodo.Bll.Dto;
using System;
using System.Linq;
using FluentValidation.AspNetCore;
using NTierTodo.Filters;

namespace NTierTodo.Controllers
{
    [ValidateModel]
    [ValidateTodoExists]
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
        public IActionResult MakeComplite(Guid id)
        {
            _manager.MakeComplite(id);

            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
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
