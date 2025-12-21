using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.DTOs.Requests;
using MessAidVOne.Application.DTOs.Responses;
using static MassAidVOne.Domain.Entities.Enums;

namespace MassAidVOne.Application.Services
{
    public class AuthService : IAuthService
    {

        public readonly IUnitOfWork _unitOfWork;
        private readonly IOtpService _otpService;
        private readonly IPasswordManagerService _passwordManagerService;
        private readonly IRepository<OtpInformation> _otpInformationRepository;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public AuthService(IUnitOfWork unitOfWork, IOtpService otpService, IEmailService emailService, IPasswordManagerService passwordManagerService)
        {
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _passwordManagerService = passwordManagerService;
            _otpInformationRepository = _unitOfWork.Repository<OtpInformation>();
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }


        #region Verify Email feature
        public async Task<Result<OtpInformationResponseDto>> VerifyEmail(EmailVerificationRequest request)
        {
            var otp = await _otpService.GenerateOtpAndSendOtp(request.Email);

            var otpInformation = new OtpInformation
            {
                Email = request.Email,
                OtpCode = otp,
                ActivityType = OtpActivityType.VerifyEmail,
                LifeTime = 3,
                Status = OtpStatus.Active,
            };

            await _otpInformationRepository.AddAsync(otpInformation);
            await _unitOfWork.SaveChangesAsync();

            var otpInformationDto = new OtpInformationResponseDto
            {
                OtpId = otpInformation.Id,
                Email = otpInformation.Email,
            };
            return Result<OtpInformationResponseDto>.Success(otpInformationDto);
        }
        #endregion


        #region Forget password feature
        public async Task<Result<bool>> ForgetPassword(ForgetPasswordRequest request)
        {
            var user = await _userInformationRepository.GetByConditionAsync(
                u => u.Email == request.Email);

            if (user == null || user.IsDeleted == true)
            {
                return Result<bool>.Failure("User not found.");
            }

            var otpVerifyRequest = new OtpVerificationRequest
            {
                Email = request.Email,
                OtpCode = request.OtpCode,
                OtpId = request.OtpId,
            };

            var otpResult = await VerifyOtp(otpVerifyRequest);

            if (!otpResult.IsSuccess)
            {
                return Result<bool>.Failure("Invalid OTP.");
            }

            var updatedPassword = await SetUserPassword(user, request.NewPassword);
            if (!updatedPassword.IsSuccess)
            {
                return Result<bool>.Failure("Failed to update password.");
            }


            return Result<bool>.Success(true);
        }
        #endregion


        #region User log-in feature
        public async Task<Result<LoginResponseDto?>> Login(LogInRequest request)
        {
            var user = await _userInformationRepository.GetByConditionAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Result<LoginResponseDto?>.Failure("User not found.");
            }

            bool isPasswordValid = _passwordManagerService.VerifyPassword(user, request.Password);
            if (!isPasswordValid)
            {
                return Result<LoginResponseDto?>.Failure("Wrong email or password.");
            }

            var userResponseDto = new LoginResponseDto
            {
                User = user,
            };

            return Result<LoginResponseDto?>.Success(userResponseDto);
        }

        #endregion


        #region Log out feature
        public async Task<Result<bool>> Logout(CancellationToken cancellationToken)
        {
            var user = await _userInformationRepository.GetByIdAsync(AppUserContext.UserId);

            if (user == null || user.IsDeleted == true)
            {
                return Result<bool>.Failure("User not exist.");
            }

            return Result<bool>.Success(true);
        }
        #endregion


        #region Change password feature
        public async Task<Result<bool>> ChangePassword(ChangePasswordRequest request)
        {
            var userInfo = await _userInformationRepository.GetByIdAsync(AppUserContext.UserId);
            if (userInfo == null || userInfo.IsDeleted == true || userInfo.IsActive == false)
            {
                return Result<bool>.Failure("Invalid user.");
            }


            bool isPasswordValid = _passwordManagerService.VerifyPassword(userInfo, request.OldPassword);
            if (!isPasswordValid)
            {
                return Result<bool>.Failure("Invalid password.");
            }
            bool isOldAndNewPasswordSame = _passwordManagerService.VerifyPassword(userInfo, request.NewPassword);
            if (isOldAndNewPasswordSame)
            {
                return Result<bool>.Failure("Your new password can't match with old password.");
            }

            var updatedPassword = await SetUserPassword(userInfo, request.NewPassword);
            if (!updatedPassword.IsSuccess)
            {
                return Result<bool>.Failure("Failed to update password.");
            }

            #region Activity Information
            var metadata = new Dictionary<string, object>
            {
                { "ActorUserId", userInfo.Id },
                { "EntityId", userInfo.Id },
                { "EntityType", nameof(UserInformation) },
                { "TargetUserIds", new List<long> { userInfo.Id } },
                { "ActivityEvent", ActivityEvents.ChangedPassword }
            };
            #endregion

            return Result<bool>.Success(true, metadata);

        }
        #endregion


        #region Verify otp feature
        public async Task<Result<bool>> VerifyOtp(OtpVerificationRequest request)
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
