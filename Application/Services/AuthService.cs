using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.DTOs.Requests;
using MessAidVOne.Application.DTOs.Responses;
using MessAidVOne.Application.Features.AuthManagement;
using static MassAidVOne.Domain.Entities.Enums;

namespace MassAidVOne.Application.Services
{
    public class AuthService : IAuthService
    {

        public readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordManagerService _passwordManagerService;
        private readonly IRepository<OtpInformation> _otpInformationRepository;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public AuthService(IUnitOfWork unitOfWork, IOtpService otpService, IEmailService emailService, IPasswordManagerService passwordManagerService)
        {
            _unitOfWork = unitOfWork;
            _passwordManagerService = passwordManagerService;
            _otpInformationRepository = _unitOfWork.Repository<OtpInformation>();
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }


        #region Verify otp feature
        public async Task<Result<bool>> VerifyOtp(OtpVerificationCommand request)
        {
            var otpInfo = await _otpInformationRepository.GetByIdAsync(request.OtpId);
            if (otpInfo == null || otpInfo.IsDeleted == true)
                return Result<bool>.Failure("Invalid otp information.");

            bool isValid = true;

            otpInfo.Attempts = ++otpInfo.Attempts;

            if (otpInfo.Status != OtpStatus.Active)
                return Result<bool>.Failure("Invalid otp information.");

            if (otpInfo.CreatedOn.AddMinutes(otpInfo.LifeTime ?? 0) < DateTime.UtcNow)
            {
                otpInfo.Status = OtpStatus.Expired;
                isValid = false;
            }

            if (otpInfo.OtpCode != request.OtpCode || otpInfo.Email != request.Email)
                isValid = false;

            if (isValid)
            {
                otpInfo.Status = isValid ? OtpStatus.Used : otpInfo.Status;

                await _otpInformationRepository.UpdateAsync(otpInfo);
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            else
            {
                return Result<bool>.Failure("Otp information is invalid.");
            }
        }
        #endregion


        #region User password set feature
        public async Task<Result<bool>> SetUserPassword(UserInformation userInformation, string password)
        {
            if (userInformation == null || userInformation.IsDeleted == true || userInformation.IsActive == false)
            {
                return Result<bool>.Failure("Invalid user.");
            }

            userInformation.UserPassword = _passwordManagerService.HashedPassword(password);

            await _userInformationRepository.UpdateAsync(userInformation);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);

        }
        #endregion


    }
}
