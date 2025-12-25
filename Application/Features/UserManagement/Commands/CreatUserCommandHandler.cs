using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Interfaces;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.Abstructions;
using static MassAidVOne.Domain.Entities.Enums;

namespace MessAidVOne.Application.Features.UserManagement.Commands
{
    public class CreatUserCommandHandler : ICommandHandler<CreatUserCommand, Result<UserInformationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly IRepository<OtpInformation> _otpInformationRepository;

        public CreatUserCommandHandler(IUnitOfWork unitOfWork, IAuthService authService, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
            _otpInformationRepository = _unitOfWork.Repository<OtpInformation>();
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result<UserInformationDto>> Handle(CreatUserCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message) = await ValidateCreatUserRequest(request);
            if (!IsValid)
            {
                return Result<UserInformationDto>.Failure(Message);
            }

            var photoUrl = string.Empty;
            if (request.Photo != null)
            {
                var (IsSuccess, UploadMessage, PhotoUrl) = await _cloudinaryService.UploadImageAsync(request.Photo);
                if (!IsSuccess)
                {
                    return Result<UserInformationDto>.Failure(UploadMessage);
                }
                photoUrl = PhotoUrl!;
            }

            var hashedPassword = await _authService.HashedPassword(request.Password);

            var user = new UserInformation
            {
                Name = request.Name,
                Email = request.Email,
                Address = request.Address,
                UserType = UserType.User,
                PhotoUrl = photoUrl,
                UserPassword = hashedPassword,
                IsActive = true,
                IsDeleted = false
            };

            await _userInformationRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            var userDto = new UserInformationDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                UserType = user.UserType,
                PhotoUrl = user.PhotoUrl != null ? user.PhotoUrl.ToLongUrl() : "",
                IsActive = user.IsActive ?? false,
            };
            return Result<UserInformationDto>.Success(userDto);
        }

        private async Task<(bool, string)> ValidateCreatUserRequest(CreatUserCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.");
            }

            bool isUserExist = await _userInformationRepository.GetByConditionAsync(u => u.Email == request.Email && u.IsDeleted == false) != null;
            if (isUserExist)
            {
                return (false, "User already exist.");
            }

            var otpInfo = await _otpInformationRepository.GetByIdAsync(request.OtpId);
            if (otpInfo == null)
            {
                return (false, "Otp information is incorrect.");
            }
            bool isOtpVerified = otpInfo.IsDeleted == false && otpInfo.Status == OtpStatus.Active && otpInfo.OtpCode == request.OtpCode && otpInfo.Email == request.Email;
            if (!isOtpVerified)
            {
                return (false, "Otp information is incorrect.");
            }

            return (true, "Ok");
        }
    }
}
