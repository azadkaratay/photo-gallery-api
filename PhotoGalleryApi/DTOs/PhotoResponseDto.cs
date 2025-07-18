﻿namespace PhotoGalleryApi.DTOs
{
    public class PhotoResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
