using System.Linq.Expressions;
using RoomFInder.Models;

namespace RoomFInder.Repository;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetActiveAsync();

    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
    Task<IEnumerable<Comment>> GetCommentsByRoomIdAsync(int roomId);
}