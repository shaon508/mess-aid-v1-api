using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Application.Abstructions;
using static MassAidVOne.Domain.Entities.Enums;

namespace MessAidVOne.Application.Features.MessManagement.Commands
{
    public class AddMemberCommandHandler : ICommandHandler<AddMemberCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepository<UserInformation> _userInformationRepository;
        private readonly IRepository<MessInformation> _messInformationRepository;
        private readonly IRepository<MemberInformation> _memberInformationRepository;

        public AddMemberCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _userInformationRepository = _unitOfWork.Repository<UserInformation>();
            _messInformationRepository = _unitOfWork.Repository<MessInformation>();
            _memberInformationRepository = _unitOfWork.Repository<MemberInformation>();

        }

        public async Task<Result<bool>> Handle(AddMemberCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message, Mess) = await ValidateAddMemberRequest(request);
            if (!IsValid)
            {
                return Result<bool>.Failure(Message);
            }

            var photoUrl = string.Empty;
            if (request.Photo != null)
            {
                var (IsSuccess, UploadMessage, PhotoUrl) = await _cloudinaryService.UploadImageAsync(request.Photo);
                if (!IsSuccess)
                {
                    return Result<bool>.Failure(UploadMessage);
                }
                photoUrl = PhotoUrl!;
            }

            long? userId = null;
            if (!string.IsNullOrEmpty(request.Email))
            {
                var user = await _userInformationRepository.GetByConditionAsync(u => u.Email == request.Email && u.IsDeleted != true);
                if (user != null)
                {
                    userId = user.Id;
                }
            }

            var memberInfo = new MemberInformation
            {
                Name = request.Name,
                Email = request.Email,
                PhotoUrl = photoUrl,
                Type = MemberType.Member,
                JoinedDate = DateTime.UtcNow,
                UserId = userId,
                IsMealAutoUpdate = request.IsMealAutoUpdate,
                MessId = Mess!.Id,
                IsActive = true
            };

            await _memberInformationRepository.AddAsync(memberInfo);
            await _unitOfWork.SaveChangesAsync();

            #region Activity Information
            var metadata = new Dictionary<string, object>
            {
                { "ActorUserId", AppUserContext.UserId },
                { "EntityId", memberInfo.Id },
                { "EntityType", nameof(MemberInformation) },
                { "TargetUserIds", new List<long> { AppUserContext.UserId } },
                { "ActivityEvent", ActivityEvents.AddedMember },
                { "Placeholders", new Dictionary<string, string>
                    {
                        { "#ActionUserId", AppUserContext.UserId.ToString() },
                        { "#MessName", Mess.Name },
                        { "#TargetUserId", userId?.ToString() ?? request.Name }
                    }
                }
            };
            #endregion

            return Result<bool>.Success(true, metaData: metadata);
        }

        private async Task<(bool, string, MessInformation?)> ValidateAddMemberRequest(AddMemberCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.", null);
            }

            var existingMess = await _messInformationRepository.GetByIdAsync(request.MessId);
            if (existingMess == null || existingMess.IsDeleted == true)
            {
                return (false, "Mess information not found.", null);
            }

            var memberList = await _memberInformationRepository.GetListByConditionAsync(m =>
                m.MessId == request.MessId &&
                m.IsDeleted != true &&
                (m.Email == request.Email || (m.UserId != null && m.UserId == AppUserContext.UserId))
            );
            if (!memberList.Any(m => m.UserId == AppUserContext.UserId))
            {
                return (false, "Only mess members can add new members.", null);
            }
            if (memberList.Any(m => m.Email == request.Email))
            {
                return (false, "A member with the same email already exists in this mess.", null);
            }

            return (true, "Ok", existingMess);
        }
    }
}
