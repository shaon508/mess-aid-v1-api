using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Interfaces;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.AuthManagement.Commands
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

            bool isOtpValid = await _authService.VerifyOtp(request.OtpId, request.OtpCode, request.Email);
            if (!isOtpValid)
            {
                return (false, "Invalid otp information.", null);
            }

            return (true, "Ok", userInfo);
        }
    }
}
