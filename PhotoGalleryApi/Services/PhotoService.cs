using Microsoft.EntityFrameworkCore;
using PhotoGalleryApi.Data;
using PhotoGalleryApi.Entities;
using PhotoGalleryApi.Services.Interfaces;

namespace PhotoGalleryApi.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly AppDbContext _context;

        public PhotoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Photo>> GetAllPhotosAsync()
        {
            return await _context.Photos.ToListAsync();
        }

        public async Task<Photo?> GetPhotoByIdAsync(int id)
        {
            return await _context.Photos.FindAsync(id);
        }

        public async Task<Photo> CreatePhotoAsync(Photo photo)
        {
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        public async Task<Photo?> UpdatePhotoAsync(Photo photo)
        {
            var existingPhoto = await _context.Photos.FindAsync(photo.Id);
            if (existingPhoto == null)
                return null;

            if (!string.IsNullOrWhiteSpace(photo.Title))
                existingPhoto.Title = photo.Title;

            if (!string.IsNullOrWhiteSpace(photo.Url))
                existingPhoto.Url = photo.Url;

            if (!string.IsNullOrWhiteSpace(photo.Description))
                existingPhoto.Description = photo.Description;

            existingPhoto.UploadedAt = photo.UploadedAt;

            await _context.SaveChangesAsync();
            return existingPhoto;
        }


        public async Task<bool> DeletePhotoAsync(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
                return false;

            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
