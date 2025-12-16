using MassAidVOne.Application.Interfaces;
using MassAidVOne.Infrastructure.Persistence;
using MassAidVOne.Persistence.Repositories;
using MessAidVOne.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MessAidVOne.API.Extensions
{
    public static class PersistenceServiceExtensions
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = config.GetConnectionString("DefaultConnection");

            string activityConnectionString = config.GetConnectionString("ActivityConnection");

            services.AddDbContext<MessManagementContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

             services.AddDbContext<ActivityManagementContext>(options =>
                options.UseMySql(activityConnectionString, ServerVersion.AutoDetect(activityConnectionString)));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICustomRepository, CustomRepository>();

            services.AddScoped<IActivityCustomRepository, ActivityCustomRepository>();

            return services;
        }
    }
}
