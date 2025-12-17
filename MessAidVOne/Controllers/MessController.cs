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
        public async Task<IActionResult> Register([FromForm] AddMessRequest request)
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

            var activityEvent = (ActivityEvent)result.MetaData["ActivityEvent"];
            var actorUserId = (long)result.MetaData["ActorUserId"];
            var entityId = (long)result.MetaData["EntityId"];
            var entityType = (string)result.MetaData["EntityType"];
            var targetUserIds = (List<long>)result.MetaData["TargetUserIds"];
            var placeholders = (Dictionary<string, string>)result.MetaData["Placeholders"];

            await _activityCustomRepository.EnqueueActivityAsync(
                activityEvent: activityEvent,
                actorUserId: actorUserId,
                entityId: entityId,
                entityType: entityType,
                targetUserIds: targetUserIds,
                placeholders: placeholders
            );

            return Ok(new ApiResponse<object>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Mess create successful.",
                Data = result.Data
            });
        }

        [HttpPut("mess")]
        [ProducesResponseType(typeof(ApiResponse<MessInformationResponseDto>), 200)]
        [Authorize]
        public async Task<IActionResult> Register([FromForm] ModifyMessRequest request)
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
            return Ok(new ApiResponse<object>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Mess info update successful.",
                Data = result.Data
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
