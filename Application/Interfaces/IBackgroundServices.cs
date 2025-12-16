namespace MassAidVOne.Application.Interfaces
{
    public interface IBackgroundServices
    {
        Task DoDeleteUsedOrUnUsedOtp();
        Task DoProcessActivityOutboxAsync();
    }
}
