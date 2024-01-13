using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TracksController : Controller
    {
        private readonly ITrackRepo _trackRepo;
        public TracksController(ITrackRepo trackRepo)
        {
            _trackRepo = trackRepo;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetTracks()
        {
            return Ok(await _trackRepo.GetAllAsync());
        }

        [HttpGet("{trackId}")]
        public async Task<IActionResult> GetTrackById(int trackId)
        {
            var track = await _trackRepo.GetByIdAsync(trackId);

            if (track == null)
                return NotFound(ExceptionMessages.EntityDoesntExist);

            return Ok(track);
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateNewTrack([FromBody] TrackDto newTrackDto)
        {
            try
            {
                // First map to domain model then pass it to services
                var newTrack = newTrackDto.Adapt<Track>();

                var createdTrack = await _trackRepo.CreateNewTrack(newTrack);

                // Map resulting domain model back to Dto for trasnfer
                var responseDto = createdTrack.Adapt<TrackDto>();
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
        }
        [HttpPut("")]
        public IActionResult UpdateTrack([FromBody] TrackDto newTrackDto)
        {
            try
            {
                var newTrack = newTrackDto.Adapt<Track>();
                var updatedTrack = _trackRepo.UpdateTrack(newTrack);

                if (updatedTrack == null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);

                var responseDto = updatedTrack.Adapt<TrackDto>();
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
        [HttpDelete("")]
        public IActionResult DeleteTrack([FromBody] TrackDto TrackDto)
        {
            try
            {
                var track = TrackDto.Adapt<Track>();
                track.Genres = null;
                var deletedTrack = _trackRepo.DeleteTrack(track);

                if (deletedTrack == null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);

                var responseDto = deletedTrack.Adapt<TrackDto>();
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
