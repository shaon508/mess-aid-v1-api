using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.DTOs.Requests;
using static MassAidVOne.Domain.Entities.Enums;

namespace MassAidVOne.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordManagerService _passwordManagerService;
        private readonly IRepository<UserInformation> _userRepository;
        private readonly IRepository<OtpInformation> _otpRepository;

        public UserService(IUnitOfWork unitOfWork, IOtpService otpService, IPasswordManagerService passwordManagerService)
        {
            _unitOfWork = unitOfWork;
            _passwordManagerService = passwordManagerService;
            _userRepository = _unitOfWork.Repository<UserInformation>();
            _otpRepository = _unitOfWork.Repository<OtpInformation>();
        }


        #region User creat feature
        public async Task<Result<UserInformationDto>> AddUserAsync(AddUserRequest request)
        {
            bool isUserExist = await _userRepository.GetByConditionAsync(u => u.Email == request.Email) != null;
            if (isUserExist)
            {
                return Result<UserInformationDto>.Failure("User already exist."); ;
            }
            var otpInfo = await _otpRepository.GetByIdAsync(request.OtpId);
            if (otpInfo == null)
            {
                return Result<UserInformationDto>.Failure("Otp information is incorrect.");
            }
            bool isOtpVerified = otpInfo.IsDeleted == false && otpInfo.Status == OtpStatus.Active && otpInfo.OtpCode == request.OtpCode && otpInfo.Email == request.Email;
            if (!isOtpVerified)
            {
                return Result<UserInformationDto>.Failure("Otp information is incorrect.");
            }

            var photUrl = string.Empty;
            if (request.Photo != null)
            {
                var uploadResult = await ImageUploadUtilities.UploadImageAsync(request.Photo, FileUploadPath.User);
                if (!uploadResult.IsSuccess)
                {
                    return Result<UserInformationDto>.Failure(uploadResult.Message);
                }
                photUrl = uploadResult.Data!;
            }

            var hashedPassword = _passwordManagerService.HashedPassword(request.Password);

            var user = new UserInformation
            {
                Name = request.Name,
                Email = request.Email,
                Address = request.Address,
                UserType = UserType.User,
                PhotoUrl = photUrl,
                UserPassword = hashedPassword,
                IsActive = true,
                IsDeleted = false
            };

            await _userRepository.AddAsync(user);
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
        #endregion


        #region Modify user feature
        public async Task<Result<UserInformationDto>> ModifyUserAsync(ModifyUserRequest request)
        {
            var userInfo = await _userRepository.GetByIdAsync(request.Id);
            if (userInfo == null || userInfo.IsDeleted == true)
            {
                return Result<UserInformationDto>.Failure("User already exists.");
            }

            var photUrl = string.Empty;
            if (request.Photo != null)
            {
                var uploadResult = await ImageUploadUtilities.UploadImageAsync(request.Photo, FileUploadPath.User);
                if (!uploadResult.IsSuccess)
                {
                    return Result<UserInformationDto>.Failure(uploadResult.Message);
                }
                photUrl = uploadResult.Data!;
            }

            var user = new UserInformation
            {
                Name = request.Name,
                Email = request.Email,
                Address = request.Address,
                PhotoUrl = photUrl
            };

            await _userRepository.UpdateAsync(user);
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
        #endregion


        #region User view feature
        public async Task<Result<UserInformationDto>> GetUserInfoByUserAsync()
        {
            var userInfo = await _userRepository.GetByIdAsync(AppUserContext.UserId);
            if (userInfo == null || userInfo.IsDeleted == true)
            {
                return Result<UserInformationDto>.Failure("No user found.");
            }
            var userDto = new UserInformationDto
            {
                Id = userInfo.Id,
                Name = userInfo.Name,
                Email = userInfo.Email,
                Address = userInfo.Address,
                UserType = userInfo.UserType,
                PhotoUrl = (userInfo.PhotoUrl != null) ? userInfo.PhotoUrl.ToLongUrl() : "",
                IsActive = userInfo.IsActive ?? false,
            };
            return Result<UserInformationDto>.Success(userDto);
        }
        #endregion



    }
}
