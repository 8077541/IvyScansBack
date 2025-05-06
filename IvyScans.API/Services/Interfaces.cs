using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IvyScans.API.Models;
using IvyScans.API.Models.DTO;

namespace IvyScans.API.Services
{
    public interface IComicService
    {
        Task<ComicsResponseDto> GetComicsAsync(int page, int pageSize, string genre, string status, string sortBy);
        Task<List<ComicDto>> GetFeaturedComicsAsync();
        Task<List<ComicDto>> GetLatestComicsAsync();
        Task<ComicDetailDto> GetComicByIdAsync(string id);
        Task<List<ChapterListItemDto>> GetComicChaptersAsync(string id);
        Task<ChapterDetailDto> GetChapterDetailsAsync(string id, int chapter);
        Task<List<ComicDto>> SearchComicsAsync(string query);
        Task<ServiceResultDto> CreateComicAsync(CreateComicDto comicDto);
        Task<(bool Success, string Message, string ChapterId)> AddChapterAsync(ChapterCreateDto chapterDto);
    }

    public interface IGenreService
    {
        Task<List<string>> GetAllGenresAsync();
    }

    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(string email, string password);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task LogoutAsync(string userId);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<UserDto> GetUserByIdAsync(string userId);
    }

    public interface IUserService
    {
        Task<List<ComicDto>> GetUserBookmarksAsync(string userId);
        Task<ServiceResultDto> AddBookmarkAsync(string userId, string comicId);
        Task<ServiceResultDto> RemoveBookmarkAsync(string userId, string comicId);
        Task<List<RatingDto>> GetUserRatingsAsync(string userId);
        Task<ServiceResultDto> AddOrUpdateRatingAsync(string userId, string comicId, int rating, string comment);
        Task<ServiceResultDto> DeleteRatingAsync(string userId, string comicId);
        Task<List<ReadingHistoryDto>> GetReadingHistoryAsync(string userId);
        // In IUserService.cs
        Task<ServiceResultDto> AddToReadingHistoryAsync(string userId, string comicId, string chapterId, int chapterNumber);
        Task<UserDto> GetUserProfileAsync(string userId);
        Task<ServiceResultDto> UpdateUserProfileAsync(string userId, UserProfileDto profileDto);
    }
}