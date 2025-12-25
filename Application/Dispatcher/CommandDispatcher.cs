
using MessAidVOne.Application.Abstructions;
using Microsoft.Extensions.DependencyInjection;

namespace MessAidVOne.Application.Dispatcher
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> Dispatch<TResult>(
            ICommand<TResult> command,
            CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(ICommandHandler<,>)
                .MakeGenericType(command.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            return await handler.Handle((dynamic)command, cancellationToken);
        }
    }
}
