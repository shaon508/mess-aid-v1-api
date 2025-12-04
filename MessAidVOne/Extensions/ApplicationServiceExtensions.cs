using MassAidVOne.Application.Interfaces;
using MassAidVOne.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;

namespace MessAidVOne.API.Extensions
{
    public static class ApplicationServiceExtensions // Changed to static class
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.Configure<EmailSetting>(
                configuration.GetSection("EmailSettings"));

            services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
            services.AddScoped<IPasswordManagerService, PasswordManagerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOtpService, OtpService>();

            return services;
        }
    }
}
