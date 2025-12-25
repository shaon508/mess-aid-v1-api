namespace MessAidVOne.Application.Abstructions
{
    public interface IQueryDispatcher
    {
        Task<TResult> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    }
}
