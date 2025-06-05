
using IvyScans.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IvyScans.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpDelete("{id}")]
        [Authorize] // Require authentication for delete operations
        public async Task<IActionResult> DeleteGenre(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Genre ID is required");
            }

            var result = await _genreService.DeleteGenreAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                // If genre not found, return 404
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }
                // If genre is in use, return 409 Conflict
                else if (result.Message.Contains("is used by"))
                {
                    return Conflict(result);
                }
                // Other errors return 400 Bad Request
                else
                {
                    return BadRequest(result);
                }
            }
        }
    }
}