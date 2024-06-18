using TaskManagement_API.Models;

namespace TaskManagement_API.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<TaskModel>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
    }
}
