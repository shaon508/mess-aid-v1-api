using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.MessManagement.Commands
{
    public class CreatMessCommandHandler : ICommandHandler<CreatMessCommand, Result<MessInformationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepository<MessInformation> _messInformationRepository;

        public CreatMessCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _messInformationRepository = _unitOfWork.Repository<MessInformation>();

        }

        public async Task<Result<MessInformationDto>> Handle(CreatMessCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message) = await ValidateCreatMessRequest(request);
            if (!IsValid)
            {
                return Result<MessInformationDto>.Failure(Message);
            }

            var photoUrl = string.Empty;
            if (request.Photo != null)
            {
                var (IsSuccess, UploadMessage, PhotoUrl) = await _cloudinaryService.UploadImageAsync(request.Photo);
                if (!IsSuccess)
                {
                    return Result<MessInformationDto>.Failure(UploadMessage);
                }
                photoUrl = PhotoUrl!;
            }

            var messInfo = new MessInformation
            {
                Name = request.Name,
                Address = request.Address ?? "",
                IsActive = true
            };

            await _messInformationRepository.AddAsync(messInfo);
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

            return Result<MessInformationDto>.Success(new MessInformationDto
            {
                Name = messInfo.Name,
                Address = messInfo.Address,
                PhotoUrl = photoUrl,
                IsActive = messInfo.IsActive
            }, metaData: metadata);
        }

        private async Task<(bool, string)> ValidateCreatMessRequest(CreatMessCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.");
            }

            return (true, "Ok");
        }
    }
}
