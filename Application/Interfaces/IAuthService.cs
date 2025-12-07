namespace MassAidVOne.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<OtpInformationDto>> VerifyEmail(EmailVerificationRequest request);
        Task<Result<bool>> VerifyOtp(OtpVerificationRequest request);
        Task<Result<bool>> ForgetPassword(ForgetPasswordRequest request);


        Task<Result<LoginResponseDto?>> Login(LogInRequest request);
        Task<Result<bool>> Logout(CancellationToken cancellationToken);
        Task<Result<bool>> ChangePassword(ChangePasswordRequest request);
    }
}
