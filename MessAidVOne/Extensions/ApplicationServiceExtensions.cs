using MassAidVOne.Application.Interfaces;
using MassAidVOne.Application.Services;
using MassAidVOne.Domain.Interfaces;
using MassAidVOne.Infrastructure.Services;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.Dispatcher;
using MessAidVOne.Application.Extensions;
using Microsoft.AspNetCore.Identity;

namespace MessAidVOne.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.Configure<EmailSetting>(
                configuration.GetSection("EmailSettings"));

            services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();


            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();

            services.AddCqrsHandlers();

            return services;
        }
    }
}
