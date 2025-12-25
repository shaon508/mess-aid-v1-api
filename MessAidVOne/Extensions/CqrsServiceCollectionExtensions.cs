using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Extensions
{
    public static class CqrsServiceCollectionExtensions
    {
        public static IServiceCollection AddCqrsHandlers(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .ToArray();

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );

            return services;
        }
    }
}
