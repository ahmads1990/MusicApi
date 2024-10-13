using Microsoft.AspNetCore.Mvc;
using MusicApi.Helpers;
using MusicApi.Repositories.Interfaces;
using MusicApi.Services.FileServices;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly IFileService _fileService;
        private readonly FileTypes fileType = FileTypes.UserImage;
        public ArtistsController(IArtistService artistService, IFileService fileService)
        {
            _artistService = artistService;
            _fileService = fileService;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAllArtists()
        {
            var artists = await _artistService.GetAllAsync();
            var responseDto = artists.Adapt<IEnumerable<ArtistDto>>();
            return Ok(responseDto);
        }
        [HttpGet("{artistId}")]
        public async Task<IActionResult> GetArtistById(int artistId)
        {
            var artist = await _artistService.GetByIdAsync(artistId);

            if (artist == null)
                return NotFound(ExceptionMessages.EntityDoesntExist);

            var responseDto = artist.Adapt<ArtistDto>();
            return Ok(responseDto);
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateNewArtist([FromBody] AddArtistDto newArtistDto)
        {
            try
            {
                // Check file
                if (!_fileService.CheckFileSpecs(newArtistDto.ArtistCoverImage, fileType))
                    return BadRequest("Invalid file constraints");
                // Save file
                string filePath = await _fileService.SaveImageFile(newArtistDto.ArtistCoverImage, fileType);
                if (string.IsNullOrEmpty(filePath))
                    return BadRequest("Invalid file constraints");

                // map to domain model then pass it to services
                var newArtist = newArtistDto.Adapt<Artist>();
                var createdGenre = await _artistService.CreateNewArtist(newArtist);

                // Map resulting domain model back to Dto for transfer
                var responseDto = createdGenre.Adapt<GenreDto>();
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500);
            }
        }
        [HttpPut("")]
        public IActionResult UpdateArtist([FromBody] ArtistDto artistDto)
        {
            try
            {
                var newArtist = artistDto.Adapt<Artist>();
                var updatedArtist = _artistService.UpdateArtistAsync(newArtist);

                if (updatedArtist == null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);

                var responseDto = updatedArtist.Adapt<ArtistDto>();
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
        [HttpDelete("")]
        public IActionResult DeleteGenre([FromBody] ArtistDto artistDto)
        {
            try
            {
                var artist = artistDto.Adapt<Artist>();
                var deletedArtist = _artistService.DeleteArtistAsync(artist);

                if (deletedArtist == null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);

                var responseDto = deletedArtist.Adapt<ArtistDto>();
                return Ok(deletedArtist);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
