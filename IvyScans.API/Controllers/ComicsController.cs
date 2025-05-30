using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IvyScans.API.Services;
using IvyScans.API.Models;
using IvyScans.API.Models.DTO;

namespace IvyScans.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComicsController : ControllerBase
    {
        private readonly IComicService _comicService;

        public ComicsController(IComicService comicService)
        {
            _comicService = comicService;
        }
        [HttpGet]
        public async Task<IActionResult> GetComics([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int limit = 0,
            [FromQuery] string? genre = null, [FromQuery] string? status = null, [FromQuery] string? sortBy = null, [FromQuery] string? sort = null)
        {
            // Support both 'pageSize' and 'limit' parameters for compatibility
            var actualPageSize = limit > 0 ? limit : pageSize;

            // Support both 'sortBy' and 'sort' parameters for compatibility
            var actualSortBy = !string.IsNullOrEmpty(sort) ? sort : sortBy;

            var result = await _comicService.GetComicsAsync(page, actualPageSize, genre, status, actualSortBy);
            return Ok(result);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedComics()
        {
            var featuredComics = await _comicService.GetFeaturedComicsAsync();
            return Ok(featuredComics); // Return the array directly without wrapping it
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestComics()
        {
            var latestComics = await _comicService.GetLatestComicsAsync();
            return Ok(new { comics = latestComics });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComic(string id)
        {
            var comic = await _comicService.GetComicByIdAsync(id);
            if (comic == null)
                return NotFound();

            return Ok(comic);
        }
        [HttpPost]
        // [Authorize(Roles = "Admin")] //IF I EVER DECIDE TO ACTUALLY ADD ROLES XDD
        public async Task<IActionResult> CreateComic([FromBody] CreateComicDto comicDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _comicService.CreateComicAsync(comicDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [HttpGet("{id}/chapters")]
        public async Task<IActionResult> GetComicChapters(string id)
        {
            var chapters = await _comicService.GetComicChaptersAsync(id);
            if (chapters == null)
                return NotFound();

            return Ok(chapters);
        }

        [HttpGet("{id}/chapters/{chapter}")]
        public async Task<IActionResult> GetChapterDetails(string id, int chapter)
        {
            var chapterDetails = await _comicService.GetChapterDetailsAsync(id, chapter);
            if (chapterDetails == null)
                return NotFound();

            return Ok(chapterDetails);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchComics([FromQuery] string q)
        {
            var results = await _comicService.SearchComicsAsync(q);
            return Ok(new { comics = results });
        }

        [HttpPost("{id}/chapters")]
        public async Task<IActionResult> AddChapter(string id, [FromBody] ChapterCreateDto chapterDto)
        {
            if (chapterDto == null)
                return BadRequest(new { message = "Invalid chapter data" });

            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "Comic ID is required" });

            // Validate the comic ID matches the one in the route
            chapterDto.ComicId = id;

            try
            {
                var result = await _comicService.AddChapterAsync(chapterDto);

                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return StatusCode(201, new
                {
                    message = "Chapter added successfully",
                    chapterId = result.ChapterId
                });
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while adding the chapter" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComic(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "Comic ID is required" });

            try
            {
                var result = await _comicService.DeleteComicAsync(id);

                if (!result.Success)
                    return NotFound(new { message = result.Message });

                return Ok(new { message = result.Message });
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while deleting the comic" });
            }
        }

        [HttpGet("test")]
        public IActionResult TestCors()
        {
            return Ok(new { 
                message = "CORS test successful", 
                timestamp = DateTime.UtcNow,
                origin = Request.Headers["Origin"].ToString()
            });
        }
    }
}