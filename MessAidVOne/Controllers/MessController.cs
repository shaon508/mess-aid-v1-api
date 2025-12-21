using System.Net;
using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MessAidVOne.Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessAidVOne.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class MessController : ControllerBase
    {

        private readonly IMessService _messService;
        private readonly IActivityCustomRepository _activityCustomRepository;
        public MessController(IMessService messService, IActivityCustomRepository activityCustomRepository)
        {
            _messService = messService;
            _activityCustomRepository = activityCustomRepository;
        }


        [HttpPost("mess")]
        [ProducesResponseType(typeof(ApiResponse<MessInformationResponseDto>), 200)]
        public async Task<IActionResult> CraeteMessAsync([FromForm] AddMessRequest request)
        {
            var result = await _messService.AddMessAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<string>
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = result.ErrorMessage,
                    Data = null
                });
            }

            await _activityCustomRepository.EnqueueActivityFromMetaDataAsync(result.MetaData);

            return Ok(new ApiResponse<object>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Mess create successful.",
                Data = result.Data
            });
        }

        [HttpPut("mess")]
        [ProducesResponseType(typeof(ApiResponse<MessInformationResponseDto>), 200)]
        public async Task<IActionResult> EditMessAsync([FromForm] ModifyMessRequest request)
        {
            var result = await _messService.ModifyMessAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<string>
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = result.ErrorMessage,
                    Data = null
                });
            }

            await _activityCustomRepository.EnqueueActivityFromMetaDataAsync(result.MetaData);

            return Ok(new ApiResponse<object>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Mess info modify successful.",
                Data = result.Data,
            });
        }



        //[HttpGet("user")]
        //[ProducesResponseType(typeof(ApiResponse<UserInformationResponseDto>), 200)]
        //[Authorize]
        //public async Task<IActionResult> GetUserInfoByUser()
        //{
        //    var result = await _userService.GetUserInfoByUserAsync();
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(new ApiResponse<string>
        //        {
        //            HttpStatusCode = HttpStatusCode.BadRequest,
        //            Message = result.ErrorMessage,
        //            Data = null
        //        });
        //    }
        //    return Ok(new ApiResponse<object>
        //    {
        //        HttpStatusCode = HttpStatusCode.OK,
        //        Message = "User information retrieved successfully.",
        //        Data = result.Data
        //    });
        //}

    }
}
