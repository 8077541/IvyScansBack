using IvyScans.API.Data;
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
    }
}