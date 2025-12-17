using MessAidVOne.Application.DTOs.Requests;

namespace MassAidVOne.Application.Interfaces
{
    public interface IMessService
    {
        Task<Result<MessInformationResponseDto>> AddMessAsync(AddMessRequest request);
        Task<Result<MessInformationResponseDto>> ModifyMessAsync(ModifyMessRequest request);
    }
}
