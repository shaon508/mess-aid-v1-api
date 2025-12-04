namespace MassAidVOne.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserInformationDto>> GetUserInfoByUserAsync();
        public Task<Result<UserInformationDto>> AddUserAsync(AddUserRequest user);
        Task<Result<UserInformationDto>> ModifyUserAsync(ModifyUserRequest request);
    }
}
