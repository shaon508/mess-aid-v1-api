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

        public async Task<TResponse> Dispatch<TResponse>(ICommand<TResponse> command)
        {
            var handlerType =
                typeof(ICommandHandler<,>)
                .MakeGenericType(command.GetType(), typeof(TResponse));

            dynamic? handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
                throw new Exception($"Handler not registered for {command.GetType().Name}");

            return await handler.Handle((dynamic)command, CancellationToken.None);
        }
    }


}
