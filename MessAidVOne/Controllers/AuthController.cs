using System.Net;
using MassAidVOne.Application.Interfaces;
using MassAidVOne.Domain.Entities;
using MessAidVOne.Application.DTOs.Requests;
using MessAidVOne.Application.Interfaces;
using MessAidVOne.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessAidVOne.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IActivityOutboxService _activityOutboxService;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService, IJwtService jwtService, IActivityOutboxService activityOutboxService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _activityOutboxService = activityOutboxService;
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmail(EmailVerificationRequest request)
        {
            var result = await _authService.VerifyEmail(request);
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

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(OtpVerificationRequest request)
        {
            var result = await _authService.VerifyOtp(request);
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
                Message = "OTP verification successful.",
                Data = result.Data
            });
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request)
        {
            var result = await _authService.ForgetPassword(request);
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
        public async Task<IActionResult> Login(LogInRequest request)
        {
            var result = await _authService.Login(request);

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
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var result = await _authService.ChangePassword(request);
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

            await _activityOutboxService.EnqueueAsync(
                activityEvent: activityEvent,
                actorUserId: actorUserId,
                entityId: entityId,
                entityType: entityType,
                targetUserIds: targetUserIds,
                placeholders: null
            );

            return Ok(new ApiResponse<object>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Change password successful.",
                Data = result.Data,
            });
        }


    }
}
