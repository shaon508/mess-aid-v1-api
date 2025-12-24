using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MassAidVOne.Domain.Interfaces;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.AuthManagement.Commands
{
    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message, User) = await ValidateChangePasswordRequest(request);
            if (!IsValid || User == null)
            {
                return Result<bool>.Failure(Message);
            }

            var updatedPassword = await _authService.SetUserPassword(User, request.NewPassword);
            if (!updatedPassword.IsSuccess)
            {
                return Result<bool>.Failure("Failed to update password.");
            }

            #region Activity Information
            var metadata = new Dictionary<string, object>
            {
                { "ActorUserId", User.Id },
                { "EntityId", User.Id },
                { "EntityType", nameof(UserInformation) },
                { "TargetUserIds", new List<long> { User.Id } },
                { "ActivityEvent", ActivityEvents.ChangedPassword }
            };
            #endregion

            return Result<bool>.Success(true, metadata);
        }

        private async Task<(bool, string, UserInformation?)> ValidateChangePasswordRequest(ChangePasswordCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.", null);
            }

            var userInfo = await _userInformationRepository.GetByIdAsync(AppUserContext.UserId);
            if (userInfo == null || userInfo.IsDeleted == true || userInfo.IsActive == false)
            {
                return (false, "Invalid user.", null);
            }

            bool isPasswordValid = await _authService.VerifyPassword(userInfo, request.OldPassword);
            if (!isPasswordValid)
            {
                return (false, "Invalid password.", null);
            }

            bool isOldAndNewPasswordSame = await _authService.VerifyPassword(userInfo, request.NewPassword);
            if (isOldAndNewPasswordSame)
            {
                return (false, "Your new password can't match with old password.", null);
            }

            return (true, "Ok", userInfo);
        }
    }
}
