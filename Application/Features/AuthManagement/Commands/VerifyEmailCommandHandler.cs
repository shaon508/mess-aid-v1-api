using MassAidVOne.Application.Interfaces;
using MessAidVOne.Application.Abstructions;
using static MassAidVOne.Domain.Entities.Enums;

namespace MessAidVOne.Application.Features.AuthManagement.Commands
{
    public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand, Result<OtpInformationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOtpService _otpService;
        private readonly IRepository<OtpInformation> _otpInformationRepository;

        public VerifyEmailCommandHandler(IOtpService otpService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _otpInformationRepository = _unitOfWork.Repository<OtpInformation>();
        }

        public async Task<Result<OtpInformationDto>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var (IsValid, Message) = await ValidateVerifyEmailRequest(request);
            var otp = await _otpService.GenerateOtpAndSendOtp(request.Email);

            var otpInformation = new OtpInformation
            {
                Email = request.Email,
                OtpCode = otp,
                ActivityType = OtpActivityType.VerifyEmail,
                LifeTime = 3,
                Status = OtpStatus.Active,
            };

            await _otpInformationRepository.AddAsync(otpInformation);
            await _unitOfWork.SaveChangesAsync();

            var otpInformationDto = new OtpInformationDto
            {
                OtpId = otpInformation.Id,
                Email = otpInformation.Email,
            };
            return Result<OtpInformationDto>.Success(otpInformationDto);
        }

        private async Task<(bool, string)> ValidateVerifyEmailRequest(VerifyEmailCommand request)
        {
            if (request == null)
            {
                return (false, "Invalid information.");
            }

            return (true, "Ok");
        }
    }
}
