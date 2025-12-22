using MassAidVOne.Application.Interfaces;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.AuthManagement
{
    public class ForgetPasswordCommandHandler : ICommandHandler<ForgetPasswordCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public ForgetPasswordCommandHandler(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }

        public async Task<Result<bool>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message, User) = await ValidateForgetPasswordRequest(request);

            var updatedPassword = await _authService.SetUserPassword(User!, request.NewPassword);
            if (!updatedPassword.IsSuccess)
            {
                return Result<bool>.Failure("Failed to update password.");
            }

            return Result<bool>.Success(true);
        }

        private async Task<(bool, string, UserInformation?)> ValidateForgetPasswordRequest(ForgetPasswordCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.", null);
            }

            var userInfo = await _userInformationRepository.GetByConditionAsync(
                 u => u.Email == request.Email);
            if (userInfo == null || userInfo.IsDeleted == true || userInfo.IsActive == false)
            {
                return (false, "Invalid user.", null);
            }

            var otpResult = await _authService.VerifyOtp(new OtpVerificationCommand
            {
                OtpId = request.OtpId,
                OtpCode = request.OtpCode,
                Email = request.Email
            });
            if (!otpResult.IsSuccess)
            {
                return (false, "Invalid otp information.", null);
            }

            return (true, "Ok", userInfo);
        }
    }
}
