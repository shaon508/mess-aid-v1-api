using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MessAidVOne.Application.DTOs;
using MessAidVOne.Application.Interfaces;
using System.Text.Json;

namespace MessAidVOne.Application.Services
{
    public class ActivityOutboxService : IActivityOutboxService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ActivityOutbox> _activityOutboxRepository;

        public ActivityOutboxService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _activityOutboxRepository = _unitOfWork.Repository<ActivityOutbox>();
        }

        public async Task EnqueueAsync(
            ActivityEvent activityEvent,
            long actorUserId,
            long entityId,
            string entityType,
            List<long> targetUserIds,
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
                PayloadJson = JsonSerializer.Serialize(payload)
            };

            await _activityOutboxRepository.AddAsync(outbox);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
