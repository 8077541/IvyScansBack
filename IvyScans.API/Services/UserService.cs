using IvyScans.API.Data;
using IvyScans.API.Models;
using IvyScans.API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace IvyScans.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ComicDto>> GetUserBookmarksAsync(string userId)
        {
            return await _context.UserBookmarks
                .Where(b => b.UserId == userId)
                .Select(b => new ComicDto
                {
                    Id = b.Comic.Id,
                    Title = b.Comic.Title,
                    Cover = b.Comic.Cover,
                    LatestChapter = b.Comic.Chapters.OrderByDescending(ch => ch.Number).FirstOrDefault().Title,
                    UpdatedAt = b.Comic.UpdatedAt,
                    Status = b.Comic.Status,
                    Genres = b.Comic.ComicGenres.Select(cg => cg.Genre.Name).ToList()
                })
                .ToListAsync();
        }

        public async Task<ServiceResultDto> AddBookmarkAsync(string userId, string comicId)
        {
            var existingBookmark = await _context.UserBookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.ComicId == comicId);

            if (existingBookmark != null)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Comic already bookmarked"
                };
            }

            var comic = await _context.Comics.FindAsync(comicId);
            if (comic == null)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Comic not found"
                };
            }

            var bookmark = new UserBookmark
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                ComicId = comicId,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserBookmarks.Add(bookmark);
            await _context.SaveChangesAsync();

            return new ServiceResultDto
            {
                Success = true,
                Message = "Bookmark added successfully"
            };
        }

        public async Task<ServiceResultDto> RemoveBookmarkAsync(string userId, string comicId)
        {
            var bookmark = await _context.UserBookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.ComicId == comicId);

            if (bookmark == null)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Bookmark not found"
                };
            }

            _context.UserBookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();

            return new ServiceResultDto
            {
                Success = true,
                Message = "Bookmark removed successfully"
            };
        }

        public async Task<List<RatingDto>> GetUserRatingsAsync(string userId)
        {
            return await _context.UserRatings
                .Where(r => r.UserId == userId)
                .Select(r => new RatingDto
                {
                    ComicId = r.ComicId,
                    Rating = r.Rating,
                    Comment = r.Comment // Add this line to include the comment
                })
                .ToListAsync();
        }

        public async Task<ServiceResultDto> AddOrUpdateRatingAsync(string userId, string comicId, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Rating must be between 1 and 5"
                };
            }

            var comic = await _context.Comics.FindAsync(comicId);
            if (comic == null)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Comic not found"
                };
            }

            var existingRating = await _context.UserRatings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ComicId == comicId);

            if (existingRating != null)
            {
                existingRating.Rating = rating;
                existingRating.Comment = comment;
                existingRating.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var newRating = new UserRating
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    ComicId = comicId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.UserRatings.Add(newRating);
            }

            await _context.SaveChangesAsync();

            return new ServiceResultDto
            {
                Success = true,
                Message = "Rating updated successfully"
            };
        }

        public async Task<ServiceResultDto> DeleteRatingAsync(string userId, string comicId)
        {
            var rating = await _context.UserRatings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ComicId == comicId);

            if (rating == null)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Rating not found"
                };
            }

            _context.UserRatings.Remove(rating);
            await _context.SaveChangesAsync();

            return new ServiceResultDto
            {
                Success = true,
                Message = "Rating deleted successfully"
            };
        }

        public async Task<List<ReadingHistoryDto>> GetReadingHistoryAsync(string userId)
        {
            var history = await _context.ReadingHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.LastReadAt)
                .Select(h => new ReadingHistoryDto
                {
                    ComicId = h.ComicId,
                    ChapterId = h.ChapterId,
                    ChapterNumber = h.Chapter.Number,
                    ComicTitle = h.Comic.Title,
                    ChapterTitle = h.Chapter.Title,
                    CoverImage = h.Comic.Cover,
                    LastReadAt = h.LastReadAt
                })
                .ToListAsync();

            return history;
        }
        public async Task<ServiceResultDto> AddToReadingHistoryAsync(string userId, string comicId, string chapterId, int chapterNumber)
        {
            var comic = await _context.Comics.FindAsync(comicId);
            if (comic == null)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Comic not found"
                };
            }

            var chapter = await _context.Chapters.FindAsync(chapterId);
            if (chapter == null || chapter.Number != chapterNumber)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Chapter not found or chapter number mismatch"
                };
            }

            var existingHistory = await _context.ReadingHistories
                .FirstOrDefaultAsync(h => h.UserId == userId && h.ComicId == comicId && h.ChapterId == chapterId);

            if (existingHistory != null)
            {
                existingHistory.LastReadAt = DateTime.UtcNow;
            }
            else
            {
                var newHistory = new ReadingHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    ComicId = comicId,
                    ChapterId = chapterId,
                    LastReadAt = DateTime.UtcNow
                };

                _context.ReadingHistories.Add(newHistory);
            }

            await _context.SaveChangesAsync();

            return new ServiceResultDto
            {
                Success = true,
                Message = "Reading history updated successfully"
            };
        }

        public async Task<UserDto> GetUserProfileAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                JoinDate = user.JoinDate,
                Avatar = user.Avatar,
                ReadingStats = CalculateReadingStats(userId)
            };
        }

        public async Task<ServiceResultDto> UpdateUserProfileAsync(string userId, UserProfileDto profileDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Check if username is already taken by another user
            if (profileDto.Username != user.Username &&
                await _context.Users.AnyAsync(u => u.Username == profileDto.Username))
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Username already taken"
                };
            }

            // Check if email is already used by another user
            if (profileDto.Email != user.Email &&
                await _context.Users.AnyAsync(u => u.Email == profileDto.Email))
            {
                return new ServiceResultDto
                {
                    Success = false,
                    Message = "Email already in use"
                };
            }

            user.Username = profileDto.Username;
            user.Email = profileDto.Email;

            if (!string.IsNullOrEmpty(profileDto.Avatar))
            {
                user.Avatar = profileDto.Avatar;
            }

            await _context.SaveChangesAsync();

            return new ServiceResultDto
            {
                Success = true,
                Message = "Profile updated successfully"
            };
        }

        private ReadingStatsDto CalculateReadingStats(string userId)
        {
            var history = _context.ReadingHistories.Where(h => h.UserId == userId).ToList();
            var readComics = history.Select(h => h.ComicId).Distinct().Count();
            var completedComics = _context.Comics
                .Count(c => c.Status == "Completed" &&
                            history.Select(h => h.ComicId).Contains(c.Id) &&
                            c.Chapters.Count <= history.Count(h => h.ComicId == c.Id));

            return new ReadingStatsDto
            {
                TotalRead = readComics,
                CurrentlyReading = readComics - completedComics,
                CompletedSeries = completedComics,
                TotalChaptersRead = history.Count
            };
        }
    }
}