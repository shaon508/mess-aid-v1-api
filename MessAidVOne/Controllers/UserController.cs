using System.Net;
using MassAidVOne.Application.Interfaces;
using MessAidVOne.Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessAidVOne.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("user")]
        [ProducesResponseType(typeof(ApiResponse<UserInformationResponseDto>), 200)]
        public async Task<IActionResult> Register([FromForm] AddUserRequest request)
        {
            var result = await _userService.AddUserAsync(request);
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
        [ProducesResponseType(typeof(ApiResponse<UserInformationResponseDto>), 200)]
        [Authorize]
        public async Task<IActionResult> Register([FromForm] ModifyUserRequest request)
        {
            var result = await _userService.ModifyUserAsync(request);
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
        [ProducesResponseType(typeof(ApiResponse<UserInformationResponseDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetUserInfoByUser()
        {
            var result = await _userService.GetUserInfoByUserAsync();
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
