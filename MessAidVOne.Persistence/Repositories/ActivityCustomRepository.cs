using System.Text.Json;
using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MessAidVOne.Application.DTOs;
using MessAidVOne.Application.Utilities;
using MessAidVOne.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using static MassAidVOne.Domain.Entities.Enums;

public class ActivityCustomRepository : IActivityCustomRepository
{
    private readonly ActivityManagementContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ActivityCustomRepository(ActivityManagementContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    #region Enqueue Activity From MetaData
    public async Task EnqueueActivityFromMetaDataAsync(Dictionary<string, object> metaData)
    {
        if (metaData == null || !metaData.Any())
            return;

        var activityEvent = GetValue<ActivityEvent>(metaData, "ActivityEvent");
        var actorUserId = GetValue<long>(metaData, "ActorUserId");
        var entityId = GetValue<long>(metaData, "EntityId");
        var entityType = GetValue<string>(metaData, "EntityType");
        var targetUserIds = GetValue<List<long>>(metaData, "TargetUserIds") ?? new List<long>();
        var placeholders = GetValue<Dictionary<string, string>>(metaData, "Placeholders")
            ?? new Dictionary<string, string>();

        if (activityEvent != null)
        {
            await EnqueueActivityAsync(
                activityEvent,
                actorUserId,
                entityId,
                entityType,
                targetUserIds,
                placeholders
            );
        }
    }
    #endregion

    #region Create Activity Outbox
    public async Task EnqueueActivityAsync(ActivityEvent activityEvent, long actorUserId,
        long entityId, string entityType, List<long> targetUserIds,
        Dictionary<string, string>? placeholders = null)
    {
        var payload = new ActivityOutboxPayload
        {
            TargetUserIds = targetUserIds,
            Placeholders = placeholders ?? new()
        };

        var outbox = new ActivityOutbox
        {
            EventKey = activityEvent.Key,
            Domain = (sbyte)activityEvent.Domain,
            ActorUserId = actorUserId,
            EntityId = entityId,
            EntityType = entityType,
            PayloadJson = JsonSerializer.Serialize(payload),
            Status = (sbyte)OutboxStatus.Pending,
            ProcessingAttempts = 0
        };

        await _context.ActivityOutboxes.AddAsync(outbox);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region Processing Activity Outbox
    public async Task ProcessActivityOutboxAsync()
    {
        var pendingOutboxes = await _context.ActivityOutboxes
            .Where(x => x.Status == (sbyte)OutboxStatus.Pending && x.ProcessingAttempts < 5)
            .Take(50)
            .ToListAsync();

        foreach (var outbox in pendingOutboxes)
        {
            outbox.Status = (sbyte)OutboxStatus.Processing;
            outbox.ProcessingAttempts++;

            var payload = JsonSerializer.Deserialize<ActivityOutboxPayload>(outbox.PayloadJson)!;

            var targets = payload.TargetUserIds
                .Select(id => new UserActivityDetails { UserId = id })
                .ToList();

            var activityEvent = ActivityEvents.FromKey(outbox.EventKey);

            await CreateActivityAsync(
                activityEvent,
                outbox.ActorUserId,
                outbox.EntityId,
                targets,
                payload.Placeholders
            );

            outbox.Status = (sbyte)OutboxStatus.Completed;
            outbox.ProcessedAt = DateTime.UtcNow;
        }

        if (pendingOutboxes.Any())
        {
            _context.ActivityOutboxes.UpdateRange(pendingOutboxes);
            await _context.SaveChangesAsync();
        }
    }
    #endregion

    #region Creating Activities
    public async Task CreateActivityAsync(ActivityEvent activityEvent, long actorUserId,
        long entityId, List<UserActivityDetails> targets, Dictionary<string, string>? placeholders)
    {
        var activity = new ActivityInformation
        {
            EventKey = activityEvent.Key,
            ActorUserId = actorUserId,
            EntityId = entityId,
            EntityType = activityEvent.Domain.ToString(),
            Description = ActivityDescriptionBuilder.Build(activityEvent.DescriptionTemplate, placeholders),
            UserActivities = targets.Select(t => new UserActivity
            {
                UserId = t.UserId,
                IsRead = false
            }).ToList()
        };

        await _context.ActivityInformations.AddAsync(activity);
        await _context.SaveChangesAsync();
    }
    #endregion
   
    #region Helper Methods
    private static T GetValue<T>(Dictionary<string, object> metaData, string key)
    {
        if (metaData.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }

        return default(T);
    }
    #endregion
}