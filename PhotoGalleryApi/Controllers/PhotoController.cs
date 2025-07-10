using Microsoft.AspNetCore.Mvc;
using PhotoGalleryApi.DTOs;
using PhotoGalleryApi.Entities;
using PhotoGalleryApi.Services.Interfaces;
using AutoMapper;

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
        public async Task<ActionResult<List<PhotoResponseDto>>> GetAllPhotos()
        {
            var photos = await _photoService.GetAllPhotosAsync();
            var result = _mapper.Map<List<PhotoResponseDto>>(photos);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PhotoResponseDto>> GetPhotoById(int id)
        {
            var photo = await _photoService.GetPhotoByIdAsync(id);
            if (photo == null)
                return NotFound("Fotoğraf bulunamadı.");

            var result = _mapper.Map<PhotoResponseDto>(photo);
            return Ok(result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PhotoResponseDto>> CreatePhoto([FromForm] PhotoCreateDto dto)
        {
            // 1. Fotoğraf klasörü
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 2. Dosya adı
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // 3. Dosyayı diske yaz
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
   
            // 4. Veritabanı kaydı
            var photo = new Photo
            {
                Title = dto.Title,
                Description = dto.Description,
                Url = $"{baseUrl}/uploads/{fileName}", 
                UploadedAt = DateTime.UtcNow
            };

            await _photoService.CreatePhotoAsync(photo);
            var response = _mapper.Map<PhotoResponseDto>(photo);
            return Ok(response);
        }


        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PhotoResponseDto>> UpdatePhoto([FromForm] PhotoUpdateDto dto)
        {
            var photo = await _photoService.GetPhotoByIdAsync(dto.Id);
            if (photo == null)
                return NotFound("Fotoğraf bulunamadı.");

            // 1. Metin alanlarını güncelle
            _mapper.Map(dto, photo);

            // 2. Yeni görsel yüklendiyse, eskiyi sil, yeni resmi kaydet
            if (dto.NewImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Eski dosyayı sil (isteğe bağlı)
                var oldPath = Path.Combine(uploadsFolder, Path.GetFileName(photo.Url));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                // Yeni dosya
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
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var success = await _photoService.DeletePhotoAsync(id);
            if (!success)
                return NotFound("Silinecek fotoğraf bulunamadı.");

            return Ok("Silme işlemi başarılı.");
        }
    }
}
