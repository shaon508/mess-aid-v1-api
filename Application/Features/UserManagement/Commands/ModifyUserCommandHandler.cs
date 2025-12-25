using MassAidVOne.Application.Interfaces;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.UserManagement.Commands
{
    public class ModifyUserCommandHandler : ICommandHandler<ModifyUserCommand, Result<UserInformationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepository<UserInformation> _userInformationRepository;

        public ModifyUserCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
        }

        public async Task<Result<UserInformationDto>> Handle(ModifyUserCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message, User) = await ValidateModifyUserRequest(request);
            if (!IsValid)
            {
                return Result<UserInformationDto>.Failure(Message);
            }

            var photoUrl = request.IsPhotoRemove ? string.Empty : User!.PhotoUrl;
            if (request.Photo != null)
            {
                var (IsSuccess, UploadMessage, PhotoUrl) = await _cloudinaryService.UploadImageAsync(request.Photo);
                if (!IsSuccess)
                {
                    return Result<UserInformationDto>.Failure(UploadMessage);
                }
                photoUrl = PhotoUrl!;
            }

            User!.Name = request.Name;
            User.Email = request.Email;
            User.PhotoUrl = photoUrl;
            User.Address = request.Address;

            await _userInformationRepository.UpdateAsync(User);
            await _unitOfWork.SaveChangesAsync();

            var userDto = new UserInformationDto
            {
                Id = User.Id,
                Name = User.Name,
                Email = User.Email,
                Address = User.Address,
                UserType = User.UserType,
                PhotoUrl = photoUrl,
                IsActive = User.IsActive ?? false,
            };
            return Result<UserInformationDto>.Success(userDto);
        }

        private async Task<(bool, string, UserInformation?)> ValidateModifyUserRequest(ModifyUserCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.", null);
            }

            var userInfo = await _userInformationRepository.GetByIdAsync(request.Id);
            if (userInfo == null || userInfo.IsDeleted == true)
            {
                return (false, "User not found.", null);
            }

            return (true, "Ok", userInfo);
        }
    }
}
