using MassAidVOne.Application.Interfaces;
using MassAidVOne.Application.Services;
using MassAidVOne.Infrastructure.Services;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.Dispatcher;
using MessAidVOne.Application.Features.AuthManagement;
using MessAidVOne.Application.Services;
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
            services.AddScoped<IPasswordManagerService, PasswordManagerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IMessService, MessService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();


            services.AddScoped<ICommandDispatcher, CommandDispatcher>();

            //services.AddScoped<ICommandHandler<ChangePasswordCommand, Result<bool>>,
            //                  ChangePasswordCommandHandler>();


            return services;
        }
    }
}
