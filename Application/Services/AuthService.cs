using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Interfaces;
using MessAidVOne.Application.Features.AuthManagement.Commands;
using Microsoft.AspNetCore.Identity;
using static MassAidVOne.Domain.Entities.Enums;

namespace MassAidVOne.Application.Services
{
    public class AuthService : IAuthService
    {

        public readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<Object> _passwordHasher;
        private readonly IRepository<OtpInformation> _otpInformationRepository;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher<object> passwordHasher = null)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _otpInformationRepository = _unitOfWork.Repository<OtpInformation>();
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }

        #region Verify otp feature
        public async Task<bool> VerifyOtp(long otpId, int otpCode, string email)
        {
            var otpInfo = await _otpInformationRepository.GetByIdAsync(otpId);
            if (otpInfo == null || otpInfo.IsDeleted == true)
                return false;

            bool isValid = true;

            otpInfo.Attempts = ++otpInfo.Attempts;

            if (otpInfo.Status != OtpStatus.Active)
                return false;

            if (otpInfo.CreatedOn.AddMinutes(otpInfo.LifeTime ?? 0) < DateTime.UtcNow)
            {
                otpInfo.Status = OtpStatus.Expired;
                isValid = false;
            }

            if (otpInfo.OtpCode != otpCode || otpInfo.Email != email)
                isValid = false;

            if (isValid)
            {
                otpInfo.Status = isValid ? OtpStatus.Used : otpInfo.Status;

                await _otpInformationRepository.UpdateAsync(otpInfo);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
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

            userInformation.UserPassword = await HashedPassword(password);

            await _userInformationRepository.UpdateAsync(userInformation);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);

        }
        #endregion


        #region Password Hasher
        public async Task<string> HashedPassword(string Password)
        {
            var hashedPassword = _passwordHasher.HashPassword(null, Password);
            return hashedPassword;
        }
        #endregion


        #region Verify Password
        public async Task<bool> VerifyPassword(UserInformation User, string EnteredPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, User.UserPassword, EnteredPassword);

            return result == PasswordVerificationResult.Success;
        }
        #endregion

    }
}
