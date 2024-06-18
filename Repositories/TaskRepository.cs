using TaskManagement_API.Models;
using Microsoft.EntityFrameworkCore;
namespace TaskManagement_API.Repositories
{
    public class TaskRepository : IRepository<TaskModel>
    {
        private readonly TaskContext _context;

        public TaskRepository(TaskContext context)
        {
            _context = context;
        }

        public async Task<TaskModel> GetByIdAsync(int id)
        {
            return await _context.Task.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<TaskModel>> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = _context.Task; 

            return await query.ToListAsync(cancellationToken);
        }

        public async Task AddAsync(TaskModel myTask)
        {
            await _context.Task.AddAsync(myTask);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskModel myTask)
        {
            _context.Task.Update(myTask);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(TaskModel myTask)
        {
            _context.Task.Remove(myTask);
            await _context.SaveChangesAsync();
        }


    }
}
