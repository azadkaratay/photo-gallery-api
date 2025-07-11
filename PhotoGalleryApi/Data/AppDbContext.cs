using Microsoft.EntityFrameworkCore;
using PhotoGalleryApi.Entities;

namespace PhotoGalleryApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
