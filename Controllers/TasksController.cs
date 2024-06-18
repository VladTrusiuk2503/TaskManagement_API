using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using TaskManagement_API.DTOs;
using TaskManagement_API.Models;
using TaskManagement_API.Repositories;

namespace TaskManagement_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ILogger<TasksController> _logger;
        private readonly TaskRepository _taskRepository;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        public TasksController(TaskRepository taskRepository, ILogger<TasksController> logger, IMemoryCache cache, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromQuery] PaginationParams @params, CancellationToken cancellationToken = default)
        {
            const string cacheKey = "AllTasks";

            if (!_cache.TryGetValue(cacheKey, out List<TaskDto> cachedTasks))
            {
                try
                {

                    var allTasks = await _taskRepository.GetAllAsync(cancellationToken);
                    var paginagedtasks = allTasks.Skip((@params.Page - 1) * @params.ItemsPerPage)
                                               .Take(@params.ItemsPerPage)
                                               .ToList();

                    var taskDtos = allTasks.Select(task => new TaskDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        IsCompleted = task.IsCompleted
                    });

                    // Set cache entry with expiration and eviction options
                    _cache.Set(cacheKey, taskDtos, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5),
                        SlidingExpiration = TimeSpan.FromMinutes(2) // Evict if not accessed in 2 minutes
                    });

                    _logger.LogInformation("Tasks retrieved from repository and cached.");
                    return Ok(taskDtos);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving tasks.");
                    return StatusCode(500, "An error occurred while retrieving tasks.");
                }
            }
            else
            {
                _logger.LogInformation("Tasks retrieved from cache.");
                return Ok(cachedTasks);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel taskModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _taskRepository.AddAsync(taskModel);

                _logger.LogInformation($"Задача с ID {taskModel.Id} успешно создана.");

                string locationUrl = $"http://localhost:5101/tasks/{taskModel.Id}";
                return CreatedAtRoute("default", new { controller = "Tasks", id = taskModel.Id }, taskModel);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"Ошибка валидации при создании задачи: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (DbException ex)
            {
                _logger.LogError($"Ошибка базы данных при создании задачи: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the task.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Неожиданная ошибка при создании задачи: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the task.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskModel updatedTask)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingTask = await _taskRepository.GetByIdAsync(id);
                if (existingTask == null)
                {
                    return NotFound();
                }

                _mapper.Map(updatedTask, existingTask);

                await _taskRepository.UpdateAsync(existingTask);

                return Ok(_mapper.Map<TaskDto>(existingTask));


            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Невозможно обновить задачу из-за конфликта. TaskId: {taskId}", id);
                return StatusCode(503, "Невозможно обновить задачу из-за конфликта. Попробуйте позже.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при обновлении задачи. TaskId: {taskId}", id);
                return StatusCode(500, "Произошла ошибка при обновлении задачи.");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var taskToDelete = await _taskRepository.GetByIdAsync(id);

                if (taskToDelete == null)
                {
                    return NotFound();
                }

                try
                {
                    await _taskRepository.RemoveAsync(taskToDelete);
                    return NoContent();
                }
                catch (DbUpdateException ex)
                {
                    // Обработать конфликт базы данных
                    _logger.LogError(ex, "Невозможно удалить задачу из-за конфликта. TaskId: {taskId}", id);
                    return StatusCode(503, "Невозможно удалить задачу из-за конфликта. Попробуйте позже.");
                }
            }
            catch (Exception ex)
            {
                // Обработать другие ошибки
                _logger.LogError(ex, "Произошла ошибка при удалении задачи. TaskId: {taskId}", id);
                return StatusCode(500, "Произошла ошибка при удалении задачи.");
            }
        }
    }
}
