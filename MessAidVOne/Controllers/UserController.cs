using System.Net;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.Features.UserManagement.Commands;
using MessAidVOne.Application.Features.UserManagement.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessAidVOne.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public UserController(ICommandDispatcher dispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = dispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpPost("user")]
        [ProducesResponseType(typeof(ApiResponse<UserInformationDto>), 200)]
        public async Task<IActionResult> Register([FromForm] CreatUserCommand command)
        {
            var result = await _commandDispatcher.Dispatch(command);
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
                Message = "User registered successful.",
                Data = result.Data
            });
        }


        [HttpPut("user")]
        [ProducesResponseType(typeof(ApiResponse<UserInformationDto>), 200)]
        [Authorize]
        public async Task<IActionResult> Register([FromForm] ModifyUserCommand command)
        {
            var result = await _commandDispatcher.Dispatch(command);
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
                Message = "User update successful.",
                Data = result.Data
            });
        }


        [HttpGet("user")]
        [ProducesResponseType(typeof(ApiResponse<UserInformationDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetUserInfoByUser([FromQuery] GetUserQuery query)
        {
            var result = await _queryDispatcher.Dispatch(query);
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
                Message = "User information retrieved successfully.",
                Data = result.Data
            });
        }

    }
}
