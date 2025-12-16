using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MySql;
using MassAidVOne.Application.Interfaces;
using MassAidVOne.Application.Services;

namespace MessAidVOne.API.Extensions
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHangfire(configuration =>
            {
                configuration.UseSimpleAssemblyNameTypeSerializer()
                             .UseRecommendedSerializerSettings()
                             .UseColouredConsoleLogProvider()
                             .UseStorage(new MySqlStorage(
                                 config.GetConnectionString("ActivityConnection") + ";Allow User Variables=True;",
                                 new MySqlStorageOptions
                                 {
                                     QueuePollInterval = TimeSpan.FromSeconds(15),
                                     PrepareSchemaIfNecessary = true,
                                 }
                             ));
            });

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Environment.ProcessorCount * 1;
                options.Queues = new[] { "default" };
                options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
            });

            services.AddScoped<IBackgroundServices, BackgroundServices>();

            return services;
        }

        public static void UseHangfireDashboard(this IApplicationBuilder app, IConfiguration config)
        {
            var username = config["Hangfire:Username"] ?? "admin";
            var password = config["Hangfire:Password"] ?? "password123";

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        RequireSsl = true,
                        SslRedirect = false,
                        LoginCaseSensitive = true,
                        Users = new[]
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = username,
                                PasswordClear = password
                            }
                        }
                    })
                }
            });
        }

        // Separate method to register recurring jobs
        public static void RegisterHangfireJobs(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<IBackgroundServices>(
                "DeleteUsedOrUnUsedOtp",
                service => service.DoDeleteUsedOrUnUsedOtp(),
                Cron.Minutely);
            
            recurringJobManager.AddOrUpdate<IBackgroundServices>(
                "DoProcessActivityOutboxAsync",
                service => service.DoProcessActivityOutboxAsync(),
                Cron.Minutely);
        }
    }
}