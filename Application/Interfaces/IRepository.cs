using System.Linq.Expressions;

namespace MassAidVOne.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SoftDeleteAsync(T entity);
        Task<T> GetByIdAsync(long id);
        Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetListByConditionAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task SoftDeleteRangeAsync(Expression<Func<T, bool>> predicate);
        Task UpdateRangeSelectedAsync(Expression<Func<T, bool>> predicate, Action<T> updateAction);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task<List<T>> GetBatchByConditionAsync(Expression<Func<T, bool>> expression, int? take = null);
    }
}
