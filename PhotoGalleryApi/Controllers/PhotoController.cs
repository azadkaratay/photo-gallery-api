using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoGalleryApi.DTOs;
using PhotoGalleryApi.Entities;
using PhotoGalleryApi.Services.Interfaces;
using System.Security.Claims;

namespace PhotoGalleryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public PhotoController(IPhotoService photoService, IMapper mapper)
        {
            _photoService = photoService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<PhotoResponseDto>>> GetAllPhotos()
        {
            var photos = await _photoService.GetAllPhotosAsync();
            var result = _mapper.Map<List<PhotoResponseDto>>(photos);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PhotoResponseDto>> GetPhotoById(int id)
        {
            var photo = await _photoService.GetPhotoByIdAsync(id);
            if (photo == null)
                return NotFound(new { message = "Photo not found." });

            var result = _mapper.Map<PhotoResponseDto>(photo);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PhotoResponseDto>> CreatePhoto([FromForm] PhotoCreateDto dto)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized(new { message = "You must be logged in to upload a photo." });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var photo = new Photo
            {
                Title = dto.Title,
                Description = dto.Description,
                Url = $"{baseUrl}/uploads/{fileName}",
                UploadedAt = DateTime.UtcNow,
                UserId = userId
            };

            await _photoService.CreatePhotoAsync(photo);
            var response = _mapper.Map<PhotoResponseDto>(photo);
            return Ok(response);
        }

        [HttpPut]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PhotoResponseDto>> UpdatePhoto([FromForm] PhotoUpdateDto dto)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized(new { message = "You must be logged in to update a photo." });

            var photo = await _photoService.GetPhotoByIdAsync(dto.Id);
            if (photo == null)
                return NotFound(new { message = "Photo not found." });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            if (photo.UserId != userId && role != "Admin")
            {
                return StatusCode(403, new { message = "You are not allowed to update this photo." });
            }

            _mapper.Map(dto, photo);

            if (dto.NewImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                var oldPath = Path.Combine(uploadsFolder, Path.GetFileName(photo.Url));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.NewImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.NewImageFile.CopyToAsync(stream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                photo.Url = $"{baseUrl}/uploads/{fileName}";
            }

            var updated = await _photoService.UpdatePhotoAsync(photo);
            var result = _mapper.Map<PhotoResponseDto>(updated);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var photo = await _photoService.GetPhotoByIdAsync(id);
            if (photo == null)
                return NotFound(new { message = "Photo not found." });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            if (photo.UserId != userId && role != "Admin")
            {
                return StatusCode(403, new { message = "You are not allowed to delete this photo." });
            }

            var success = await _photoService.DeletePhotoAsync(id);
            if (!success)
                return BadRequest(new { message = "Failed to delete photo." });

            return Ok(new { message = "Photo deleted successfully." });
        }
    }
}
