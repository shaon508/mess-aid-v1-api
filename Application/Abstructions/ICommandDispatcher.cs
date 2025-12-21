namespace MessAidVOne.Application.Abstructions
{
    public interface ICommandDispatcher
    {
        Task<TResponse> Dispatch<TResponse>(ICommand<TResponse> command);
    }

}
