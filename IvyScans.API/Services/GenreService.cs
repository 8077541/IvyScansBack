using IvyScans.API.Data;
using IvyScans.API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace IvyScans.API.Services
{
    public class GenreService : IGenreService
    {
        private readonly ApplicationDbContext _context;

        public GenreService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> GetAllGenresAsync()
        {
            return await _context.Genres
                .Select(g => g.Name)
                .ToListAsync();
        }

        public async Task<ServiceResultDto> DeleteGenreAsync(string genreId)
        {
            try
            {
                // Find the genre with all related entities
                var genre = await _context.Genres
                    .Include(g => g.ComicGenres)
                        .ThenInclude(cg => cg.Comic)
                    .FirstOrDefaultAsync(g => g.Id == genreId);

                if (genre == null)
                {
                    return new ServiceResultDto
                    {
                        Success = false,
                        Message = "Genre not found"
                    };
                }

                // Check if genre is used by any comics
                if (genre.ComicGenres != null && genre.ComicGenres.Any())
                {
                    var comicTitles = genre.ComicGenres.Select(cg => cg.Comic.Title).ToList();
                    return new ServiceResultDto
                    {
                        Success = false,
                        Message = $"Cannot delete genre '{genre.Name}' as it is used by {genre.ComicGenres.Count} comic(s): {string.Join(", ", comicTitles.Take(3))}{(comicTitles.Count > 3 ? "..." : "")}"
                    };
                }

                // Delete the genre
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();

                return new ServiceResultDto
                {
                    Success = true,
                    Message = $"Genre '{genre.Name}' has been successfully deleted"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = $"Failed to delete genre: {ex.Message}"
                };
            }
        }
    }
}