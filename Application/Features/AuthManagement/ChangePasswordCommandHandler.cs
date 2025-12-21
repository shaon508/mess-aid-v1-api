using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.AuthManagement
{
    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IPasswordManagerService _passwordManagerService;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordManagerService passwordManagerService, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _passwordManagerService = passwordManagerService;
            _authService = authService;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
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

            var updatedPassword = await _authService.SetUserPassword(userInfo, request.NewPassword);
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
    }
}
