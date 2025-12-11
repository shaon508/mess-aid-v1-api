using System.Text.Json;
using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MessAidVOne.Application.DTOs;
using static MassAidVOne.Domain.Entities.Enum;

namespace MassAidVOne.Application.Services
{
    public class BackgroundServices : IBackgroundServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActivityService _activityService;
        private readonly IRepository<OtpInformation> _otpInformationrepository;
        private readonly IRepository<ActivityOutbox> _activityOutboxRepository;

        public BackgroundServices(IUnitOfWork unitOfWork, IActivityService activityService)
        {
            _unitOfWork = unitOfWork;
            _activityService = activityService;
            _otpInformationrepository = _unitOfWork.Repository<OtpInformation>();
            _activityOutboxRepository = _unitOfWork.Repository<ActivityOutbox>();
        }


        #region Delete used or unused OTPs
        public async Task DoDeleteUsedOrUnUsedOtp()
        {
            var otpList = await _otpInformationrepository.GetListByConditionAsync(x => x.CreatedOn.AddMinutes((int)x.LifeTime) < DateTime.UtcNow && x.IsDeleted == false && x.Status != OtpStatus.Expired);
            if (otpList is not null && otpList.Any())
            {
                foreach (var otp in otpList)
                {
                    otp.IsDeleted = true;
                    otp.DeletedOn = DateTime.UtcNow;
                    otp.ModifiedOn = DateTime.UtcNow;
                    otp.Status = OtpStatus.Expired;
                }
                await _otpInformationrepository.UpdateRangeAsync(otpList);
                await _unitOfWork.SaveChangesAsync();
            }
            await Task.CompletedTask;
        }
        #endregion

        #region Activity outbox processing
        public async Task DoProcessActivityOutboxAsync()
        {
            var activityOutboxService = await _activityOutboxRepository.GetBatchByConditionAsync(x => x.Status == (sbyte)OutboxStatus.Pending &&
             x.ProcessingAttempts < 5, 50);

            foreach (var outbox in activityOutboxService)
            {
                outbox.Status = (sbyte)OutboxStatus.Processing;
                outbox.ProcessingAttempts++;
                await _unitOfWork.SaveChangesAsync();

                var payload = JsonSerializer
                    .Deserialize<ActivityOutboxPayload>(outbox.PayloadJson)!;

                var targets = payload.TargetUserIds
                    .Select(id => new UserActivityDetails { UserId = id })
                    .ToList();

                var activityEvent = ActivityEvents.FromKey(outbox.EventKey);

                await _activityService.CreateActivityAsync(
                    activityEvent,
                    outbox.ActorUserId,
                    outbox.EntityId,
                    targets,
                    payload.Placeholders);

                outbox.Status = (sbyte)OutboxStatus.Completed;
                outbox.ProcessedAt = DateTime.UtcNow;

            }
            if (activityOutboxService.Any())
            {
                await _activityOutboxRepository.UpdateRangeAsync(activityOutboxService);
            }
            await _unitOfWork.SaveChangesAsync();
        }
        #endregion

    }
}
