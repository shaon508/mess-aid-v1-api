
using MessAidVOne.Application.Abstructions;
using Microsoft.Extensions.DependencyInjection;

namespace MessAidVOne.Application.Dispatcher
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> Dispatch<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IQueryHandler<,>)
                .MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            return await handler.Handle((dynamic)query, cancellationToken);
        }
    }
}
