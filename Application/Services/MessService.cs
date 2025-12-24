//using MassAidVOne.Application.Interfaces;
//using MassAidVOne.Domain.Entities;
//using MassAidVOne.Domain.Utilities;
//using MessAidVOne.Application.DTOs.Requests;
//using static MassAidVOne.Domain.Entities.Enums;

//namespace MessAidVOne.Application.Services
//{
//    public class MessService : IMessService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IRepository<MessInformation> _messRepository;
//        private readonly IRepository<MemberInformation> _memberInformationRepository;
//        private readonly ICloudinaryService _cloudinaryService;

//        public MessService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
//        {
//            _unitOfWork = unitOfWork;
//            _messRepository = _unitOfWork.Repository<MessInformation>();
//            _memberInformationRepository = _unitOfWork.Repository<MemberInformation>();
//            _cloudinaryService = cloudinaryService;
//        }

//        #region Add Mess Information
//        public async Task<Result<MessInformationDto>> AddMessAsync(AddMessRequest request)
//        {
//            var photoUrl = string.Empty;
//            if (request.Photo != null)
//            {
//                try
//                {
//                    photoUrl = await _cloudinaryService.UploadImageAsync(request.Photo);
//                }
//                catch (Exception ex)
//                {
//                    return Result<MessInformationDto>.Failure(ex.Message);
//                }
//            }

//            var messInfo = new MessInformation
//            {
//                Name = request.Name,
//                Address = request.Address ?? "",
//                IsActive = true
//            };

//            await _messRepository.AddAsync(messInfo);
//            await _unitOfWork.SaveChangesAsync();

//            #region Activity Information
//            var metadata = new Dictionary<string, object>
//            {
//                { "ActorUserId", AppUserContext.UserId },
//                { "EntityId", messInfo.Id },
//                { "EntityType", nameof(MessInformation) },
//                { "TargetUserIds", new List<long> { AppUserContext.UserId } },
//                { "ActivityEvent", ActivityEvents.CreatedMess },
//                { "Placeholders", new Dictionary<string, string>
//                    {
//                        { "#ActionUserName", AppUserContext.UserName.ToString() },
//                        { "#MessName", messInfo.Name }
//                    }
//                }
//            };
//            #endregion

//            return Result<MessInformationDto>.Success(new MessInformationDto
//            {
//                Name = messInfo.Name,
//                Address = messInfo.Address,
//                PhotoUrl = photoUrl,
//                IsActive = messInfo.IsActive
//            }, metaData: metadata);
//        }
//        #endregion


//        #region Modify Mess Information
//        public async Task<Result<MessInformationDto>> ModifyMessAsync(ModifyMessRequest request)
//        {
//            var messInfo = await _messRepository.GetByIdAsync(request.Id);
//            if (messInfo == null)
//            {
//                return Result<MessInformationDto>.Failure("Mess information not found.");
//            }

//            var photoUrl = request.IsPhotoRemove ? string.Empty : messInfo.PhotoUrl;

//            if (request.Photo != null)
//            {
//                try
//                {
//                    photoUrl = await _cloudinaryService.UploadImageAsync(request.Photo);
//                }
//                catch (Exception ex)
//                {
//                    return Result<MessInformationDto>.Failure(ex.Message);
//                }
//            }

//            messInfo.Name = request.Name;
//            messInfo.Address = request.Address;
//            messInfo.PhotoUrl = photoUrl;

//            await _messRepository.UpdateAsync(messInfo);
//            await _unitOfWork.SaveChangesAsync();

//            #region Activity Information
//            var metadata = new Dictionary<string, object>
//            {
//                { "ActorUserId", AppUserContext.UserId },
//                { "EntityId", messInfo.Id },
//                { "EntityType", nameof(MessInformation) },
//                { "TargetUserIds", new List<long> { AppUserContext.UserId } },
//                { "ActivityEvent", ActivityEvents.ModifyMess },
//                { "Placeholders", new Dictionary<string, string>
//                    {
//                        { "#MessName", messInfo.Name },
//                        { "#ActionUserName", AppUserContext.UserName.ToString() }
//                    }
//                }
//            };
//            #endregion

//            return Result<MessInformationDto>.Success(new MessInformationDto
//            {
//                Name = messInfo.Name,
//                Address = messInfo.Address,
//                PhotoUrl = photoUrl,
//                IsActive = messInfo.IsActive
//            }, metaData: metadata);
//        }
//        #endregion
//    }
//}
