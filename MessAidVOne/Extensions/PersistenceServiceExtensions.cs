using MassAidVOne.Application.Interfaces;
using MassAidVOne.Infrastructure.Persistence;
using MassAidVOne.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessAidVOne.API.Extensions
{
    public static class PersistenceServiceExtensions
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<MessManagementContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICustomRepository, CustomRepository>();

            return services;
        }
    }
}
