using MassAidVOne.Application.Interfaces;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.DTOs.Responses;

namespace MessAidVOne.Application.Features.AuthManagement
{
    public class LogInCommandHandler : ICommandHandler<LogInCommand, Result<LoginDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordManagerService _passwordManagerService;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public LogInCommandHandler(IUnitOfWork unitOfWork, IPasswordManagerService passwordManagerService)
        {
            _unitOfWork = unitOfWork;
            _passwordManagerService = passwordManagerService;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }

        public async Task<Result<LoginDto>> Handle(LogInCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message, User) = await ValidateLogInRequest(request);

            var logInDto = new LoginDto
            {
                User = User!,
            };

            return Result<LoginDto>.Success(logInDto);
        }

        private async Task<(bool, string, UserInformation?)> ValidateLogInRequest(LogInCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.", null);
            }

            var userInfo = await _userInformationRepository.GetByConditionAsync(u => u.Email == request.Email);
            if (userInfo == null || userInfo.IsDeleted == true || userInfo.IsActive == false)
            {
                return (false, "Invalid user.", null);
            }

            bool isPasswordValid = _passwordManagerService.VerifyPassword(userInfo, request.Password);
            if (!isPasswordValid)
            {
                return (false, "Invalid password.", null);
            }

            return (true, "Ok", userInfo);
        }
    }
}
