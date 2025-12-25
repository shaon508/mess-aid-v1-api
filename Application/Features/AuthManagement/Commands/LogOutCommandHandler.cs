using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.AuthManagement.Commands
{
    public class LogOutCommandHandler : ICommandHandler<LogOutCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public LogOutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }

        public async Task<Result<bool>> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message) = await ValidateLogOutRequest(request);
            if (IsValid)
            {
                return Result<bool>.Failure(Message);
            }

            return Result<bool>.Success(true);
        }

        private async Task<(bool, string)> ValidateLogOutRequest(LogOutCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.");
            }

            var userInfo = await _userInformationRepository.GetByIdAsync(AppUserContext.UserId);
            if (userInfo == null || userInfo.IsDeleted == true || userInfo.IsActive == false)
            {
                return (false, "Invalid user.");
            }

            return (true, "Ok");
        }
    }
}
