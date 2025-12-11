using MassAidVOne.Domain.Entities;

namespace MessAidVOne.Application.Interfaces
{
    public interface IActivityOutboxService
    {
        Task EnqueueAsync(ActivityEvent activityEvent, long actorUserId, long entityId, string entityType, List<long> targetUserIds, Dictionary<string, string>? placeholders = null);
    }
}
