using MassAidVOne.Application.Interfaces;

namespace MassAidVOne.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IRepository<OtpInformation> _otpInformationrepository;
        private readonly IEmailService _emailService;

        public OtpService(IEmailService emailService, IRepository<OtpInformation> otpInformationrepository)
        {
            _emailService = emailService;
            _otpInformationrepository = otpInformationrepository;
        }

        public async Task<int> GenerateOtpAndSendOtp(string ToEmail)
        {
            var result = new Random().Next(1111, 10000);
            var otp = result.ToString();
            var subject = "OTP";
            var body = $"Your otp is {otp}";
            await _emailService.SendEmailAsync(ToEmail, subject, body);
            return result;
        }
    }
}
