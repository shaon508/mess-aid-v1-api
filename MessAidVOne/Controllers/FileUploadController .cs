using System.Net;
using MassAidVOne.Infrastructure.Services;
using MessAidVOne.Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessAidVOne.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        public UploadController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("photo")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoRequest request)
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(request.File);

            return Ok(new ApiResponse<object>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Image upload successful.",
                Data = imageUrl,
            });
        }

    }



}
