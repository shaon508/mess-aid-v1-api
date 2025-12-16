using MessAidVOne.Application.DTOs.Requests;

namespace MassAidVOne.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserInformationResponseDto>> GetUserInfoByUserAsync();
        public Task<Result<UserInformationResponseDto>> AddUserAsync(AddUserRequest user);
        Task<Result<UserInformationResponseDto>> ModifyUserAsync(ModifyUserRequest request);
    }
}
