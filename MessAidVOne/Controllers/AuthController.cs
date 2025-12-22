using System.Net;
using MassAidVOne.Application.Interfaces;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.Features.AuthManagement;
using MessAidVOne.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessAidVOne.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {

        private readonly IJwtService _jwtService;
        private readonly ICommandDispatcher _dispatcher;
        private readonly IActivityCustomRepository _activityCustomRepository;

        public AuthController(IJwtService jwtService, IActivityCustomRepository activityCustomRepository, ICommandDispatcher dispatcher)
        {
            _jwtService = jwtService;
            _activityCustomRepository = activityCustomRepository;
            _dispatcher = dispatcher;
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmail(VerifyEmailCommand command)
        {
            var result = await _dispatcher.Dispatch(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse<string>
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = result.ErrorMessage,
                    Data = null
                }
            );
            }

            return Ok(new ApiResponse<object>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Otp sent successful.",
                Data = result.Data
            });

        }

        //[HttpPost("verify-otp")]
        //public async Task<IActionResult> VerifyOtp(OtpVerificationCommand command)
        //{
        //    var result = await _dispatcher.Dispatch(command);

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
        //        Message = "OTP verification successful.",
        //        Data = result.Data
        //    });
        //}


        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordCommand command)
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

            return Ok(new ApiResponse<object>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Your reset password successfully.",
                Data = result.Data,
            });
        }


        [HttpPost("log-in")]
        public async Task<IActionResult> Login(LogInCommand command)
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

            var userInfo = result.Data;

            if (userInfo == null || userInfo.User == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "User not found after login.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Login successful.",
                Data = new
                {
                    Token = _jwtService.GenerateToken(userInfo.User, userInfo.User.Email, userInfo.User.UserType)
                }
            });
        }

        //[HttpPost("log-out")]
        //[Authorize]
        //public async Task<IActionResult> LogOut()
        //{
        //    var result = await _authService.Logout();
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(new ApiResponse<string>
        //        {
        //            HttpStatusCode = HttpStatusCode.BadRequest,
        //            Message = result.ErrorMessage,
        //            Data = null
        //        });
        //    }

        //}


        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand request)
        {
            var result = await _dispatcher.Dispatch(request);

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
                Message = "Change password successful.",
                Data = result.Data,
            });
        }


    }
}
