using MassAidVOne.Application.Interfaces;
using MassAidVOne.Persistence.Repositories;

namespace MassAidVOne.Infrastructure.Persistence
{
    public class UnitOfWork(MessManagementContext context) : IUnitOfWork, IDisposable
    {
        private readonly MessManagementContext _context = context;
        private readonly Dictionary<Type, object> _repositories = [];
        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            if (_repositories.TryGetValue(typeof(T), out var repo))
                return (IRepository<T>)repo;

            var newRepo = new Repository<T>(_context);
            _repositories[typeof(T)] = newRepo;
            return newRepo;
        }
        public async Task<long> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
