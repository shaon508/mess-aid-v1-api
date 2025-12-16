using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MessAidVOne.Application.Utilities;

public class ActivityService : IActivityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<ActivityInformation> _activityInformation;

    public ActivityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _activityInformation = _unitOfWork.Repository<ActivityInformation>();
    }

    public async Task CreateActivityAsync(
        ActivityEvent activityEvent,
        long actionUserId,
        long entityId,
        List<UserActivityDetails> targets,
        Dictionary<string, string>? placeholders)
    {
        var activity = new ActivityInformation
        {
            EventKey = activityEvent.Key,
            ActorUserId = actionUserId,
            EntityId = entityId,
            EntityType = activityEvent.Domain.ToString(),
            Description = ActivityDescriptionBuilder.Build(
                    activityEvent.DescriptionTemplate,
                    placeholders),
        };

        foreach (var target in targets)
        {
            activity.UserActivities.Add(new UserActivity
            {
                UserId = target.UserId,
                
                IsRead = false
            });
        }

        await _activityInformation.AddAsync(activity);
        await _unitOfWork.SaveChangesAsync();
    }
}
