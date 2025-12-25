using System.Net;
using MassAidVOne.Application.Interfaces;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.Features.MessManagement.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessAidVOne.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class MessController : ControllerBase
    {
        private readonly ICommandDispatcher _dispatcher;
        private readonly IActivityCustomRepository _activityCustomRepository;
        public MessController(ICommandDispatcher dispatcher, IActivityCustomRepository activityCustomRepository)
        {
            _dispatcher = dispatcher;
            _activityCustomRepository = activityCustomRepository;
        }


        [HttpPost("mess")]
        [ProducesResponseType(typeof(ApiResponse<MessInformationDto>), 200)]
        public async Task<IActionResult> CreateMessAsync([FromForm] CreatMessCommand command)
        {
            var result = await _dispatcher.Dispatch(command);
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
        [ProducesResponseType(typeof(ApiResponse<MessInformationDto>), 200)]
        public async Task<IActionResult> EditMessAsync([FromForm] ModifyMessCommand command)
        {
            var result = await _dispatcher.Dispatch(command);
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

    }
}
