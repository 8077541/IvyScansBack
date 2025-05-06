using IvyScans.API.Data;
using IvyScans.API.Models;
using IvyScans.API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace IvyScans.API.Services
{
    public class ComicService : IComicService
    {
        private readonly ApplicationDbContext _context;

        public ComicService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResultDto> CreateComicAsync(CreateComicDto comicDto)
        {
            try
            {
                var comic = new Comic
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = comicDto.Title,
                    Cover = comicDto.Cover,
                    Description = comicDto.Description,
                    Author = comicDto.Author,
                    Artist = comicDto.Artist,
                    Released = comicDto.Released,
                    Status = comicDto.Status,
                    UpdatedAt = DateTime.UtcNow,
                    IsFeatured = comicDto.IsFeatured,
                    ComicGenres = new List<ComicGenre>(),
                    Chapters = new List<Chapter>()
                };

                // Handle genres
                if (comicDto.GenreNames != null && comicDto.GenreNames.Any())
                {
                    foreach (var genreName in comicDto.GenreNames)
                    {
                        // Check if genre exists, if not create it
                        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                        if (genre == null)
                        {
                            genre = new Genre
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = genreName
                            };
                            _context.Genres.Add(genre);
                            await _context.SaveChangesAsync(); // Save to get the genre ID
                        }

                        // Link the genre to the comic
                        comic.ComicGenres.Add(new ComicGenre
                        {
                            ComicId = comic.Id,
                            GenreId = genre.Id
                        });
                    }
                }

                // Add comic to database
                _context.Comics.Add(comic);
                await _context.SaveChangesAsync();

                return new ServiceResultDto
                {
                    Success = true,
                    Message = "Comic created successfully " + comic.Id,

                };
            }
            catch (Exception ex)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = $"Failed to create comic: {ex.Message}",

                };
            }
        }
        public async Task<ChapterDetailDto> GetChapterDetailsAsync(string id, int chapter)
        {
            var chapterEntity = await _context.Chapters
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.ComicId == id && c.Number == chapter);

            if (chapterEntity == null)
                return null;

            return new ChapterDetailDto
            {
                Id = chapterEntity.Id,  // Include the chapter ID
                Title = chapterEntity.Title,
                Number = chapterEntity.Number,
                Images = chapterEntity.Images
                    .OrderBy(i => i.Order)
                    .Select(i => i.Url)
                    .ToList()
            };
        }
        public async Task<ComicsResponseDto> GetComicsAsync(int page, int pageSize, string genre, string status, string sortBy)
        {
            var query = _context.Comics.AsQueryable();

            // Apply genre filter
            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(c => c.ComicGenres.Any(cg => cg.Genre.Name == genre));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status == status);
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "title" => query.OrderBy(c => c.Title),
                "latest" => query.OrderByDescending(c => c.UpdatedAt),
                "oldest" => query.OrderBy(c => c.UpdatedAt),
                "release" => query.OrderByDescending(c => c.Released),
                _ => query.OrderByDescending(c => c.UpdatedAt)  // Default sorting
            };

            // Get total count
            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            // Apply pagination
            var comics = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ComicDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Cover = c.Cover,
                    LatestChapter = c.Chapters.OrderByDescending(ch => ch.Number).FirstOrDefault().Title,
                    UpdatedAt = c.UpdatedAt,
                    Status = c.Status,
                    Genres = c.ComicGenres.Select(cg => cg.Genre.Name).ToList()
                })
                .ToListAsync();

            return new ComicsResponseDto
            {
                Comics = comics,
                Total = total,
                TotalPages = totalPages
            };
        }

        public async Task<List<ComicDto>> GetFeaturedComicsAsync()
        {
            return await _context.Comics
                .Where(c => c.IsFeatured)
                .Select(c => new ComicDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Cover = c.Cover,
                    LatestChapter = c.Chapters.OrderByDescending(ch => ch.Number).FirstOrDefault().Title,
                    UpdatedAt = c.UpdatedAt,
                    Status = c.Status,
                    Genres = c.ComicGenres.Select(cg => cg.Genre.Name).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<ComicDto>> GetLatestComicsAsync()
        {
            return await _context.Comics
                .OrderByDescending(c => c.UpdatedAt)
                .Take(20)
                .Select(c => new ComicDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Cover = c.Cover,
                    LatestChapter = c.Chapters.OrderByDescending(ch => ch.Number).FirstOrDefault().Title,
                    UpdatedAt = c.UpdatedAt,
                    Status = c.Status,
                    Genres = c.ComicGenres.Select(cg => cg.Genre.Name).ToList()
                })
                .ToListAsync();
        }

        public async Task<ComicDetailDto> GetComicByIdAsync(string id)
        {
            var comic = await _context.Comics
                .Where(c => c.Id == id)
                .Select(c => new ComicDetailDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Cover = c.Cover,
                    LatestChapter = c.Chapters.OrderByDescending(ch => ch.Number).FirstOrDefault().Title,
                    UpdatedAt = c.UpdatedAt,
                    Status = c.Status,
                    Genres = c.ComicGenres.Select(cg => cg.Genre.Name).ToList(),
                    Description = c.Description,
                    Author = c.Author,
                    Artist = c.Artist,
                    Released = c.Released,
                    Chapters = c.Chapters.OrderByDescending(ch => ch.Number).Select(ch => new ChapterListItemDto
                    {
                        Number = ch.Number,
                        Title = ch.Title,
                        Date = ch.Date
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return comic;
        }

        public async Task<List<ChapterListItemDto>> GetComicChaptersAsync(string id)
        {
            return await _context.Chapters
                .Where(c => c.ComicId == id)
                .OrderByDescending(c => c.Number)
                .Select(c => new ChapterListItemDto
                {
                    Number = c.Number,
                    Title = c.Title,
                    Date = c.Date
                })
                .ToListAsync();
        }




        public async Task<List<ComicDto>> SearchComicsAsync(string query)
        {
            return await _context.Comics
                .Where(c => c.Title.Contains(query) || c.Description.Contains(query))
                .Select(c => new ComicDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Cover = c.Cover,
                    LatestChapter = c.Chapters.OrderByDescending(ch => ch.Number).FirstOrDefault().Title,
                    UpdatedAt = c.UpdatedAt,
                    Status = c.Status,
                    Genres = c.ComicGenres.Select(cg => cg.Genre.Name).ToList()
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, string ChapterId)> AddChapterAsync(ChapterCreateDto chapterDto)
        {
            try
            {
                // Verify the comic exists
                var comic = await _context.Comics.FindAsync(chapterDto.ComicId);
                if (comic == null)
                {
                    return (false, "Comic not found", null);
                }

                // Check if a chapter with the same number already exists
                var existingChapter = await _context.Chapters
                    .FirstOrDefaultAsync(c => c.ComicId == chapterDto.ComicId && c.Number == chapterDto.Number);

                if (existingChapter != null)
                {
                    return (false, $"Chapter {chapterDto.Number} already exists for this comic", null);
                }

                // Create new chapter
                var chapterId = Guid.NewGuid().ToString();
                var chapter = new Chapter
                {
                    Id = chapterId,
                    ComicId = chapterDto.ComicId,
                    Number = chapterDto.Number,
                    Title = chapterDto.Title,
                    Date = DateTime.UtcNow,
                    Images = new List<ChapterImage>()
                };

                // Add images
                if (chapterDto.ImageUrls != null && chapterDto.ImageUrls.Any())
                {
                    for (int i = 0; i < chapterDto.ImageUrls.Count; i++)
                    {
                        chapter.Images.Add(new ChapterImage
                        {
                            Id = Guid.NewGuid().ToString(),
                            ChapterId = chapterId,
                            Url = chapterDto.ImageUrls[i],
                            Order = i + 1
                        });
                    }
                }

                // Add chapter to database
                _context.Chapters.Add(chapter);

                // Update the comic's UpdatedAt timestamp
                comic.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return (true, "Chapter added successfully", chapterId);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add chapter: {ex.Message}", null);
            }
        }
    }
}