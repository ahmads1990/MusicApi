using Microsoft.AspNetCore.Mvc;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenresController : Controller
    {
        private readonly IGenreRepo _genreRepo;
        public GenresController(IGenreRepo genreRepo)
        {
            _genreRepo = genreRepo;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAllGenres()
        {
            return Ok(await _genreRepo.GetAllAsync());
        }
        [HttpGet("{genreId}")]
        public async Task<IActionResult> GetGenreById(int genreId)
        {
            return Ok(await _genreRepo.GetByIdAsync(genreId));
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateNewGenre([FromBody] GenreDto newGenreDto)
        {
            try
            {
                var newGenre = new Genre() { Id = newGenreDto.Id, Name = newGenreDto.Name };
                var createdGenre = await _genreRepo.CreateNewGenre(newGenre);

                return Ok(createdGenre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
        }
        [HttpPut("")]
        public IActionResult UpdateGenre([FromBody] Genre newGenre)
        {
            try
            {
                var updatedGenre = _genreRepo.UpdateGenre(newGenre);
                return Ok(updatedGenre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
        [HttpDelete("")]
        public IActionResult DeleteGenre([FromBody] Genre genre)
        {
            try
            {
                var deletedGenre = _genreRepo.DeleteGenre(genre);
                return Ok(deletedGenre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
