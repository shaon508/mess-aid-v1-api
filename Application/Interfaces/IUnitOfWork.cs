namespace MassAidVOne.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : BaseEntity;
        Task<long> SaveChangesAsync();
    }
}
