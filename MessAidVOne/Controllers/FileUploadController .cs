using MassAidVOne.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessAidVOne.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;

        public UploadController(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("photo")]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file)
        {
            if (file == null)
                return BadRequest("File is required");

            var imageUrl = await _cloudinaryService.UploadImageAsync(file);

            return Ok(new
            {
                success = true,
                url = imageUrl
            });
        }
    }



}
