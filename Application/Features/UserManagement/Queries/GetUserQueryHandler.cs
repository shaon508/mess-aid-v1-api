using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.UserManagement.Queries
{
    public class GetUserQueryHandler : IQueryHandler<GetUserQuery, Result<UserInformationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<UserInformation> _userInformationRepository;
        public GetUserQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }
        public async Task<Result<UserInformationDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userInformationRepository.GetByIdAsync(AppUserContext.UserId);
            if (user == null)
            {
                return Result<UserInformationDto>.Failure("User not found.");
            }
            var userDto = new UserInformationDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                PhotoUrl = user.PhotoUrl,
                UserType = user.UserType?.ToString(),
                IsActive = user.IsActive ?? false
            };
            return Result<UserInformationDto>.Success(userDto);
        }
      
    }
}
