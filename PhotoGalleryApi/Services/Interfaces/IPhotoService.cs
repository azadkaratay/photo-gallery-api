using PhotoGalleryApi.Entities;

namespace PhotoGalleryApi.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<List<Photo>> GetAllPhotosAsync();
        Task<Photo?> GetPhotoByIdAsync(int id);
        Task<Photo> CreatePhotoAsync(Photo photo);
        Task<Photo?> UpdatePhotoAsync(Photo photo);
        Task<bool> DeletePhotoAsync(int id);
    }
}
