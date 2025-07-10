using System.ComponentModel.DataAnnotations;

public class PhotoUpdateDto
{
    [Required]
    public int Id { get; set; }

    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public IFormFile? NewImageFile { get; set; }
}
