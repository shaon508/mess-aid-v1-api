namespace MassAidVOne.Application.Interfaces
{
    public interface IOtpService
    {
        public Task<int> GenerateOtpAndSendOtp(string ToEmail);
    }
}
