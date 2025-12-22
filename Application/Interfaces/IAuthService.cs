using MessAidVOne.Application.Features.AuthManagement;

namespace MassAidVOne.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<bool>> SetUserPassword(UserInformation userInformation, string password);
        Task<Result<bool>> VerifyOtp(OtpVerificationCommand request);
    }
}
