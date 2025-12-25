using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.MessManagement.Commands
{
    public class ModifyMessCommandHandler : ICommandHandler<ModifyMessCommand, Result<MessInformationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepository<MessInformation> _messInformationRepository;

        public ModifyMessCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _messInformationRepository = _unitOfWork.Repository<MessInformation>();

        }

        public async Task<Result<MessInformationDto>> Handle(ModifyMessCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message, Mess) = await ValidateModifyMessRequest(request);


            var photoUrl = request.IsPhotoRemove ? string.Empty : Mess!.PhotoUrl;
            if (request.Photo != null)
            {
                var (IsSuccess, UploadMessage, PhotoUrl) = await _cloudinaryService.UploadImageAsync(request.Photo);
                if (!IsSuccess)
                {
                    return Result<MessInformationDto>.Failure(UploadMessage);
                }
                photoUrl = PhotoUrl!;
            }

            Mess!.Name = request.Name;
            Mess.Address = request.Address ?? "";
            Mess.PhotoUrl = photoUrl;

            await _messInformationRepository.UpdateAsync(Mess);
            await _unitOfWork.SaveChangesAsync();

            #region Activity Information
            var metadata = new Dictionary<string, object>
            {
                { "ActorUserId", AppUserContext.UserId },
                { "EntityId", Mess.Id },
                { "EntityType", nameof(MessInformation) },
                { "TargetUserIds", new List<long> { AppUserContext.UserId } },
                { "ActivityEvent", ActivityEvents.ModifyMess },
                { "Placeholders", new Dictionary<string, string>
                    {
                        { "#MessName", Mess.Name },
                        { "#ActionUserId", AppUserContext.UserId.ToString() }
                    }
                }
            };
            #endregion

            return Result<MessInformationDto>.Success(new MessInformationDto
            {
                Name = Mess.Name,
                Address = Mess.Address,
                PhotoUrl = photoUrl,
                IsActive = Mess.IsActive
            }, metaData: metadata);
        }

        private async Task<(bool, string, MessInformation?)> ValidateModifyMessRequest(ModifyMessCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.", null);
            }

            var messInfo = await _messInformationRepository.GetByIdAsync(request.Id);
            if (messInfo == null || messInfo.IsDeleted == true)
            {
                return (false, "Mess information not found.", null);
            }


            return (true, "Ok", messInfo);
        }
    }
}
