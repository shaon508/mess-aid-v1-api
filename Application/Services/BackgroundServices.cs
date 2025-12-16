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
        private readonly IActivityCustomRepository _activityCustomRepository;
        private readonly IRepository<OtpInformation> _otpInformationrepository;

        public BackgroundServices(IUnitOfWork unitOfWork, IActivityCustomRepository activityCustomRepository)
        {
            _unitOfWork = unitOfWork;
            _otpInformationrepository = _unitOfWork.Repository<OtpInformation>();
            _activityCustomRepository = activityCustomRepository;
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


        #region Process Activity Outbox
        public async Task DoProcessActivityOutboxAsync()
        {
            await _activityCustomRepository.ProcessActivityOutboxAsync();
        }
        #endregion

    }
}
