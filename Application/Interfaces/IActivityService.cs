using MassAidVOne.Domain.Entities;

namespace MassAidVOne.Application.Interfaces
{
    public interface IActivityService
    {
        Task CreateActivityAsync(ActivityEvent activityEvent, long actionUserId, long entityId, List<UserActivityDetails> targets, Dictionary<string, string>? placeholders);
    }
}
