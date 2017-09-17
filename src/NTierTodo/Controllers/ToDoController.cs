using Microsoft.AspNetCore.Mvc;
using NTierTodo.Bll.Abstract;
using NTierTodo.Bll.Dto;
using NTierTodo.Bll.Exception;
using System;
using System.Linq;
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
            var result = _manager.GetAll();

            return Ok(result.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                return Ok(_manager.Get(id));
            }
            catch (ValidationException e)
            {
                ModelState.AddModelError(e.Property, e.Message);

                return BadRequest(ModelState);
            }
        }


        [HttpPost]
        public IActionResult Post([FromBody]ToDoDto value)
        {
            try
            {
                return Ok(_manager.Create(value));
            }
            catch (ValidationException e)
            {
                ModelState.AddModelError(e.Property, e.Message);

                return BadRequest(ModelState);
            }
        }

        [HttpPost("{id}/[action]")]
        public IActionResult MakeComplite(Guid id)
        {
            try
            {
                _manager.MakeComplite(id);

                return Ok();
            }
            catch (ValidationException e)
            {
                ModelState.AddModelError(e.Property, e.Message);

                return BadRequest(ModelState);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody]ToDoDto value)
        {
            try
            {
                _manager.Update(value);

                return Ok();
            }
            catch (ValidationException e)
            {
                ModelState.AddModelError(e.Property, e.Message);

                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _manager.Delete(id);

                return Ok();
            }
            catch (ValidationException e)
            {
                ModelState.AddModelError(e.Property, e.Message);

                return BadRequest(ModelState);
            }
        }
    }
}
