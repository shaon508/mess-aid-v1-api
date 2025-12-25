using MassAidVOne.Domain.Entities;

namespace MassAidVOne.Application.Interfaces
{
    public interface IActivityCustomRepository
    {
        Task EnqueueActivityAsync(ActivityEvent activityEvent, long actorUserId, long entityId, string entityType, List<long> targetUserIds, Dictionary<string, string>? placeholders = null);

        Task ProcessActivityOutboxAsync();

        Task CreateActivityAsync(ActivityEvent activityEvent, long actorUserId, long entityId, List<UserActivityDetails> targets, Dictionary<string, string>? placeholders);

        Task EnqueueActivityFromMetaDataAsync(Dictionary<string, object> metaData);
    }
}
