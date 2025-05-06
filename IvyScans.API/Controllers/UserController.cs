using IvyScans.API.Models.DTO;
using IvyScans.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IvyScans.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("bookmarks")]
        public async Task<IActionResult> GetUserBookmarks()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var bookmarks = await _userService.GetUserBookmarksAsync(userId);
            return Ok(bookmarks);
        }

        [HttpPost("bookmarks")]
        public async Task<IActionResult> AddBookmark([FromBody] BookmarkDto bookmarkDto)
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _userService.AddBookmarkAsync(userId, bookmarkDto.ComicId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return StatusCode(201, new { message = "Bookmark added successfully" });
        }

        [HttpDelete("bookmarks/{id}")]
        public async Task<IActionResult> RemoveBookmark(string id)
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _userService.RemoveBookmarkAsync(userId, id);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(new { message = "Bookmark removed successfully" });
        }

        [HttpGet("ratings")]
        public async Task<IActionResult> GetUserRatings()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var ratings = await _userService.GetUserRatingsAsync(userId);
            return Ok(ratings);
        }

        [HttpPost("ratings")]
        public async Task<IActionResult> AddOrUpdateRating([FromBody] RatingDto ratingDto)
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _userService.AddOrUpdateRatingAsync(userId, ratingDto.ComicId, ratingDto.Rating, ratingDto.Comment);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = "Rating updated successfully" });
        }

        [HttpDelete("ratings/{id}")]
        public async Task<IActionResult> DeleteRating(string id)
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _userService.DeleteRatingAsync(userId, id);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(new { message = "Rating deleted successfully" });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetReadingHistory()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var history = await _userService.GetReadingHistoryAsync(userId);
            return Ok(history);
        }

        [HttpPost("history")]
        public async Task<IActionResult> AddToReadingHistory([FromBody] ReadingHistoryInputDto historyDto)
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _userService.AddToReadingHistoryAsync(userId, historyDto.ComicId, historyDto.ChapterId, historyDto.ChapterNumber);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = "Reading history updated successfully" });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await _userService.GetUserProfileAsync(userId);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileDto profileDto)
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _userService.UpdateUserProfileAsync(userId, profileDto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = "Profile updated successfully" });
        }
    }
}