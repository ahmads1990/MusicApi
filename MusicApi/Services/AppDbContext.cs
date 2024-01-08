using Microsoft.EntityFrameworkCore;
using MusicApi.Models.Join;

namespace MusicApi.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Track>()
                .HasMany(t => t.Genres)
                .WithMany(g => g.Tracks)
                .UsingEntity(j => j.ToTable("TrackGenres"));
        }
    }
}