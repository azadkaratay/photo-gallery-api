using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PhotoGalleryApi.DTOs
{
    public class PhotoCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; } = null!;
    }
}
