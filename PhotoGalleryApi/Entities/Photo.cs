using System.ComponentModel.DataAnnotations;

namespace PhotoGalleryApi.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; }
        public int UserId { get; set; }

    }

}
