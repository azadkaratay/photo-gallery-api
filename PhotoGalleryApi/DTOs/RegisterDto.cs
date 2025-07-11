using System.ComponentModel.DataAnnotations;

namespace PhotoGalleryApi.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
