
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/event-images")]
    public class EventImagesController : ControllerBase
    {
        private readonly BlobStorageService _blobStorageService;

        public EventImagesController(BlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Organiser")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadImage(
            [FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Please select an image.");
            }

            if (image.Length > 5 * 1024 * 1024)
            {
                return BadRequest("Image must not exceed 5 MB.");
            }

            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            var allowedExtensions = new[]
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

            string extension =
                Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!allowedTypes.Contains(image.ContentType.ToLowerInvariant()) ||
                !allowedExtensions.Contains(extension))
            {
                return BadRequest("Only JPG, PNG and WEBP images are allowed.");
            }

            await using var stream = image.OpenReadStream();

            string blobName = await _blobStorageService.UploadImageAsync(
                stream,
                image.FileName,
                image.ContentType);

            return Ok(new
            {
                message = "Image uploaded successfully.",
                blobName,
                imageUrl = Url.ActionLink(
                    nameof(GetImage),
                    values: new { blobName })
            });
        }

        [HttpGet("{blobName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName) ||
                blobName.Contains('/') ||
                blobName.Contains('\\'))
            {
                return BadRequest("Invalid image name.");
            }

            try
            {
                var (content, contentType) =
                    await _blobStorageService.DownloadImageAsync(blobName);

                return File(content, contentType);
            }
            catch (Azure.RequestFailedException ex)
                when (ex.Status == 404)
            {
                return NotFound("Image not found.");
            }
        }
    }
}
