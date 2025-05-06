using IvyScans.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IvyScans.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Comic> Comics { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<ChapterImage> ChapterImages { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ComicGenre> ComicGenres { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserBookmark> UserBookmarks { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<ReadingHistory> ReadingHistories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Comic and Genre
            modelBuilder.Entity<ComicGenre>()
                .HasKey(cg => new { cg.ComicId, cg.GenreId });

            modelBuilder.Entity<ComicGenre>()
                .HasOne(cg => cg.Comic)
                .WithMany(c => c.ComicGenres)
                .HasForeignKey(cg => cg.ComicId);

            modelBuilder.Entity<ComicGenre>()
                .HasOne(cg => cg.Genre)
                .WithMany(g => g.ComicGenres)
                .HasForeignKey(cg => cg.GenreId);

            // Configure relationships for Comic and Chapter
            modelBuilder.Entity<Chapter>()
                .HasOne(c => c.Comic)
                .WithMany(c => c.Chapters)
                .HasForeignKey(c => c.ComicId);

            // Configure relationships for Chapter and ChapterImage
            modelBuilder.Entity<ChapterImage>()
                .HasOne(ci => ci.Chapter)
                .WithMany(c => c.Images)
                .HasForeignKey(ci => ci.ChapterId);

            // Configure relationships for User and UserBookmark
            modelBuilder.Entity<UserBookmark>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.Bookmarks)
                .HasForeignKey(ub => ub.UserId);

            modelBuilder.Entity<UserBookmark>()
                .HasOne(ub => ub.Comic)
                .WithMany(c => c.Bookmarks)
                .HasForeignKey(ub => ub.ComicId);

            // Configure relationships for User and UserRating
            modelBuilder.Entity<UserRating>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRating>()
                .HasOne(ur => ur.Comic)
                .WithMany(c => c.Ratings)
                .HasForeignKey(ur => ur.ComicId);

            // Configure relationships for User and ReadingHistory
            modelBuilder.Entity<ReadingHistory>()
                .HasOne(rh => rh.User)
                .WithMany(u => u.ReadingHistories)
                .HasForeignKey(rh => rh.UserId);

            modelBuilder.Entity<ReadingHistory>()
                .HasOne(rh => rh.Comic)
                .WithMany(c => c.ReadingHistories)
                .HasForeignKey(rh => rh.ComicId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ReadingHistory>()
                .HasOne(rh => rh.Chapter)
                .WithMany(c => c.ReadingHistories)
                .HasForeignKey(rh => rh.ChapterId);

            // Configure relationships for User and RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);
        }
    }
}