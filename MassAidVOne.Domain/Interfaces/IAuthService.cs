namespace MassAidVOne.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<string> HashedPassword(string Password);
        Task<Result<bool>> SetUserPassword(UserInformation userInformation, string password);
        Task<bool> VerifyOtp(long otpId, int otpCode, string email);
        Task<bool> VerifyPassword(UserInformation User, string EnteredPassword);
    }
}
