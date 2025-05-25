
using Microsoft.EntityFrameworkCore;
using System;

namespace FileAnalysisService.Data
{
    public class AnalysAppDbContext : DbContext
    {
        public AnalysAppDbContext(DbContextOptions<AnalysAppDbContext> options) : base(options) { }

        public DbSet<AnalysisResult> Files { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AnalysisResult>(entity =>
            {
                entity.OwnsOne(f => f.AnalysResults); 
            });
        }
    }
}
