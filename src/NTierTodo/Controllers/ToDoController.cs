using Microsoft.AspNetCore.Mvc;
using NTierTodo.Bll.Abstract;
using NTierTodo.Bll.Model;
using NTierTodo.ViewModels;
using System;
using System.Linq;

namespace NTierTodo.Controllers
{
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
            return Ok(result.Select(t => new ToDoVm
            {
                Id = t.Id,
                Description = t.Description.Value,
                IsComplite = t.IsComplite
            }).ToList());
        }
        
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var todo = _manager.Get(id);

                if (todo == null)
                    return NotFound();

                return Ok(new ToDoVm
                {
                    Description = todo.Description.Value,
                    Id = todo.Id,
                    IsComplite = todo.IsComplite
                });

            }
            catch (ValidationException e)
            {
                ModelState.AddModelError(e.Property, e.Message);

                return BadRequest(ModelState);
            }
        }
        

        [HttpPost]
        public IActionResult Post([FromBody]ToDoVm value)
        {
            if (value == null)
                return NotFound();

            try
            {
                return Ok(_manager.Create(new ToDoModel
                {
                    Id = value.Id,
                    Description = new Description(value.Description),
                    IsComplite = value.IsComplite
                }));
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        public IActionResult Put(Guid id, [FromBody]ToDoVm value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (value == null)
                return NotFound();

            try
            {
                _manager.Update(new ToDoModel
                {
                    Id = id,
                    Description = new Description(value.Description),
                    IsComplite = value.IsComplite
                });

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
