using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using TaskManagement_API.Models;
using TaskManagement_API.Repositories;

namespace TaskManagement_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskRepository _TaskRepository;

        public TasksController(TaskRepository taskRepository)
        {
            _TaskRepository = taskRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                var tasks = await _TaskRepository.GetAllAsync();
                return Ok(tasks);
            }
            catch (Exception)
            {

                return StatusCode(500, "An error occurred while retrieving tasks.");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyID(int id)
        {
            try
            {
                var task = await _TaskRepository.GetByIdAsync(id);

                if (task == null)
                {
                    return NotFound();
                }

                return Ok(task);
            }
            catch (Exception)
            {

                return StatusCode(500, "An error occurred while retrieving the task by ID.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] MyTask myTask)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _TaskRepository.AddAsync(myTask);


                return CreatedAtAction(nameof(GetbyID), new { id = myTask.Id }, myTask);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the task.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] MyTask updatedTask)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingTask = await _TaskRepository.GetByIdAsync(id);
                if (existingTask == null)
                {
                    return NotFound();
                }


                existingTask.Title = updatedTask.Title;
                existingTask.Description = updatedTask.Description;


                await _TaskRepository.UpdateAsync(existingTask);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the task.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var taskToDelete = await _TaskRepository.GetByIdAsync(id);

                if (taskToDelete == null)
                {
                    return NotFound();
                }

                await _TaskRepository.RemoveAsync(taskToDelete);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the task.");
            }
        }
    }
}
