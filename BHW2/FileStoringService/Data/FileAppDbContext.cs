using Microsoft.EntityFrameworkCore;

namespace FileStoringService.Data
{
    public class FileAppDbContext : DbContext
    {
        public FileAppDbContext(DbContextOptions<FileAppDbContext> options) : base(options)
        {
        }
        public DbSet<FileMetadata> FilesMetadata { get; set; } = null!;

    }
}