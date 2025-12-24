using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Interfaces;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.DTOs;

namespace MessAidVOne.Application.Features.AuthManagement.Commands
{
    public class LogInCommandHandler : ICommandHandler<LogInCommand, Result<LoginDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public LogInCommandHandler(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
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

            bool isPasswordValid = await _authService.VerifyPassword(userInfo, request.Password);
            if (!isPasswordValid)
            {
                return (false, "Invalid password.", null);
            }

            return (true, "Ok", userInfo);
        }
    }
}
