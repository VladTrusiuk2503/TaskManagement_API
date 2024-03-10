using TaskManagement_API.Models;
using Microsoft.EntityFrameworkCore;
namespace TaskManagement_API.Repositories
{
    public class TaskRepository : IRepository<MyTask>
    {
        private readonly TaskContext _context;

        public TaskRepository(TaskContext context)
        {
            _context = context;
        }

        public async Task<MyTask> GetByIdAsync(int id)
        {
            return await _context.Task.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<MyTask>> GetAllAsync()
        {
            return await _context.Task.ToListAsync();
        }

        public async Task AddAsync(MyTask myTask)
        {
            await _context.Task.AddAsync(myTask);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MyTask myTask)
        {
            _context.Task.Update(myTask);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(MyTask myTask)
        {
            _context.Task.Remove(myTask);
            await _context.SaveChangesAsync();
        }


    }
}
