using System.Linq.Expressions;
using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Utilities;
using Microsoft.EntityFrameworkCore;

namespace MassAidVOne.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly MessManagementContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(MessManagementContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public async Task DeleteAsync(T entity)
        {
              _dbSet.Remove(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            entity.ModifiedOn = DateTime.UtcNow;
            entity.ModifiedBy = AppUserContext.UserId;
            _dbSet.Update(entity);
        }

        public async Task SoftDeleteRangeAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = await _context.Set<T>().Where(predicate).ToListAsync();

            if (entities.Count == 0)
                return;

            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.DeletedOn = DateTime.UtcNow;
                entity.DeletedBy = AppUserContext.UserId;
            }

            _context.Set<T>().UpdateRange(entities);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            var entityList = entities?.ToList();

            if (entityList == null || !entityList.Any())
                return;

            foreach (var entity in entityList)
            {
                entity.ModifiedBy = AppUserContext.UserId;
                entity.ModifiedOn = DateTime.UtcNow;
            }

            _dbSet.UpdateRange(entityList);
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateRangeSelectedAsync(Expression<Func<T, bool>> predicate, Action<T> updateAction)
        {
            var entities = await _context.Set<T>().Where(predicate).ToListAsync();

            if (entities.Count == 0)
                return;

            foreach (var entity in entities)
            {
                updateAction(entity);
            }

            _context.Set<T>().UpdateRange(entities);

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public Task<T> GetByIdAsync(long Id)
        {
            return _dbSet.FirstAsync(x => x.Id == Id);
        }

        public async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetListByConditionAsync(Expression<Func<T, bool>> predicate,
                params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }

    }
}
