using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.Abstructions;
using static MassAidVOne.Domain.Entities.Enums;

namespace MessAidVOne.Application.Features.AuthManagement
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<UserInformationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IPasswordManagerService _passwordManagerService;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly IRepository<OtpInformation> _otpInformationRepository;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordManagerService passwordManagerService, IAuthService authService, IOtpService otpService, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _passwordManagerService = passwordManagerService;
            _authService = authService;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
            _otpInformationRepository = _unitOfWork.Repository<OtpInformation>();
            _otpService = otpService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Result<UserInformationDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message) = await ValidateCreatUserRequest(request);
            if (!IsValid)
            {
                return Result<UserInformationDto>.Failure(Message);
            }

            var photoUrl = string.Empty;
            if (request.Photo != null)
            {
                try
                {
                    photoUrl = await _cloudinaryService.UploadImageAsync(request.Photo);
                }
                catch (Exception ex)
                {
                    return Result<UserInformationDto>.Failure(ex.Message);
                }
            }

            var hashedPassword = _passwordManagerService.HashedPassword(request.Password);

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
                PhotoUrl = (user.PhotoUrl != null) ? user.PhotoUrl.ToLongUrl() : "",
                IsActive = user.IsActive ?? false,
            };
            return Result<UserInformationDto>.Success(userDto);
        }

        private async Task<(bool, string)> ValidateCreatUserRequest(CreateUserCommand request)
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
            bool isOtpVerified = (otpInfo.IsDeleted == false && otpInfo.Status == OtpStatus.Active && otpInfo.OtpCode == request.OtpCode && otpInfo.Email == request.Email);
            if (!isOtpVerified)
            {
                return (false, "Otp information is incorrect.");
            }

            return (true, "Ok");
        }
    }
}
