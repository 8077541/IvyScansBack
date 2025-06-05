namespace IvyScans.API.Models.DTO
{
    public class ComicDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public string LatestChapter { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; }
        public List<string> Genres { get; set; }
    }

    public class ComicDetailDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public string LatestChapter { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; }
        public List<string> Genres { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public DateTime Released { get; set; }
        public List<ChapterListItemDto> Chapters { get; set; }
    }
    public class CreateComicDto
    {
        public string Title { get; set; }
        public string Cover { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public DateTime Released { get; set; }
        public string Status { get; set; }
        public bool IsFeatured { get; set; }
        public List<string> GenreNames { get; set; }
    }

    public class ChapterListItemDto
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }

    public class ChapterDetailDto
    {
        public string Id { get; set; }  // Add this property
        public List<string> Images { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public string Avatar { get; set; }
        public ReadingStatsDto ReadingStats { get; set; }
    }

    public class ReadingStatsDto
    {
        public int TotalRead { get; set; }
        public int CurrentlyReading { get; set; }
        public int CompletedSeries { get; set; }
        public int TotalChaptersRead { get; set; }
    }

    public class ChapterCreateDto
    {
        public string ComicId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
    }

    public class BookmarkDto
    {
        public string ComicId { get; set; }
    }

    public class RatingDto
    {
        public string ComicId { get; set; }
        public int Rating { get; set; }

        public string Comment { get; set; }
    }


    public class ReadingHistoryInputDto
    {
        public string ComicId { get; set; }
        public string ChapterId { get; set; }
        public int ChapterNumber { get; set; }
    }

    public class ReadingHistoryDto
    {
        public string ComicId { get; set; }
        public string ChapterId { get; set; }
        public int ChapterNumber { get; set; }
        public string ComicTitle { get; set; }
        public string ChapterTitle { get; set; }
        public string CoverImage { get; set; }
        public DateTime LastReadAt { get; set; }
    }

    public class UserProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }

    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserDto User { get; set; }
    }

    public class ServiceResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class ComicsResponseDto
    {
        public List<ComicDto> Comics { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
    }
}