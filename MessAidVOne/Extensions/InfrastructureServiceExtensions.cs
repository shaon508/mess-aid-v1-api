using MassAidVOne.Application.Interfaces;
using MassAidVOne.Infrastructure.Configurations;
using MassAidVOne.Infrastructure.Persistence;
using MassAidVOne.Infrastructure.Services;
using MassAidVOne.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessAidVOne.API.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CloudinarySettings>(
                config.GetSection("Cloudinary"));

            services.AddScoped<ICloudinaryService, CloudinaryService>();

            return services;
        }
    }
}
