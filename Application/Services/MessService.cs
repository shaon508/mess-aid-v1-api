using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.DTOs.Requests;
using static MassAidVOne.Domain.Entities.Enums;

namespace MessAidVOne.Application.Services
{
    public class MessService : IMessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MessInformation> _messRepository;

        public MessService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _messRepository = _unitOfWork.Repository<MessInformation>();
        }

        #region Add Mess Information
        public async Task<Result<MessInformationResponseDto>> AddMessAsync(AddMessRequest request)
        {
            var photoUrl = string.Empty;
            if (request.Photo != null)
            {
                var uploadResult = await ImageUploadUtilities.UploadImageAsync(request.Photo, FileUploadPath.User);
                if (!uploadResult.IsSuccess)
                {
                    return Result<MessInformationResponseDto>.Failure(uploadResult.Message);
                }
                photoUrl = uploadResult.Data!;
            }

            var messInfo = new MessInformation
            {
                Name = request.Name,
                Address = request.Address ?? "",
                IsActive = true
            };

            await _messRepository.AddAsync(messInfo);
            await _unitOfWork.SaveChangesAsync();

            #region Activity Information
            var metadata = new Dictionary<string, object>
            {
                { "ActorUserId", AppUserContext.UserId },
                { "EntityId", messInfo.Id },
                { "EntityType", nameof(MessInformation) },
                { "TargetUserIds", new List<long> { AppUserContext.UserId } },
                { "ActivityEvent", ActivityEvents.CreatedMess },
                { "Placeholders", new Dictionary<string, string>
                    {
                        { "#ActionUserName", AppUserContext.UserName.ToString() },
                        { "#MessName", messInfo.Name }
                    }
                }
            };
            #endregion

            return Result<MessInformationResponseDto>.Success(new MessInformationResponseDto
            {
                Name = messInfo.Name,
                Address = messInfo.Address,
                PhotoUrl = photoUrl,
                IsActive = messInfo.IsActive
            }, metaData: metadata);
        }
        #endregion


        #region Modify Mess Information
        public async Task<Result<MessInformationResponseDto>> ModifyMessAsync(ModifyMessRequest request)
        {
            var messInfo = await _messRepository.GetByIdAsync(request.Id);
            if (messInfo == null)
            {
                return Result<MessInformationResponseDto>.Failure("Mess information not found.");
            }

            var photoUrl = request.IsPhotoRemove ? string.Empty : messInfo.PhotoUrl;

            if (request.Photo != null)
            {
                var uploadResult = await ImageUploadUtilities.UploadImageAsync(request.Photo, FileUploadPath.User);
                if (!uploadResult.IsSuccess)
                {
                    return Result<MessInformationResponseDto>.Failure(uploadResult.Message);
                }
                photoUrl = uploadResult.Data!;
            }

            messInfo.Name = request.Name;
            messInfo.Address = request.Address;
            messInfo.PhotoUrl = photoUrl;

            await _messRepository.UpdateAsync(messInfo);
            await _unitOfWork.SaveChangesAsync();

            return Result<MessInformationResponseDto>.Success(new MessInformationResponseDto
            {
                Name = messInfo.Name,
                Address = messInfo.Address,
                PhotoUrl = photoUrl,
                IsActive = messInfo.IsActive
            });
        }
        #endregion
    }
}
