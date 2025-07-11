using System.ComponentModel.DataAnnotations;

namespace PhotoGalleryApi.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "User"; // "User" veya "Admin"
    }
}
