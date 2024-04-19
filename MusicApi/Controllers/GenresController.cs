using Microsoft.AspNetCore.Mvc;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreRepo _genreRepo;
        public GenresController(IGenreRepo genreRepo)
        {
            _genreRepo = genreRepo;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreRepo.GetAllAsync();
            var responseDto = genres.Adapt<IEnumerable<GenreDto>>();
            return Ok(responseDto);
        }
        [HttpGet("{genreId}")]
        public async Task<IActionResult> GetGenreById(int genreId)
        {
            var genre = await _genreRepo.GetByIdAsync(genreId);

            if (genre == null)
                return NotFound(ExceptionMessages.EntityDoesntExist);

            var responseDto = genre.Adapt<GenreDto>();
            return Ok(responseDto);
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateNewGenre([FromBody] GenreDto newGenreDto)
        {
            try
            {
                // First map to domain model then pass it to services
                var newGenre = newGenreDto.Adapt<Genre>();
                var createdGenre = await _genreRepo.CreateNewGenre(newGenre);

                // Map resulting domain model back to Dto for trasnfer
                var responseDto = createdGenre.Adapt<GenreDto>();
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
        }
        [HttpPut("")]
        public IActionResult UpdateGenre([FromBody] GenreDto newGenreDto)
        {
            try
            {
                var newGenre = newGenreDto.Adapt<Genre>();
                var updatedGenre = _genreRepo.UpdateGenre(newGenre);

                if (updatedGenre == null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);

                var responseDto = updatedGenre.Adapt<GenreDto>();
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
        [HttpDelete("")]
        public IActionResult DeleteGenre([FromBody] GenreDto genreDto)
        {
            try
            {
                var genre = genreDto.Adapt<Genre>();
                var deletedGenre = _genreRepo.DeleteGenre(genre);

                if (deletedGenre == null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);

                var responseDto = deletedGenre.Adapt<GenreDto>();
                return Ok(deletedGenre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
