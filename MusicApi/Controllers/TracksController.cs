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
        public async Task<IActionResult> CreateNewTrack([FromBody] Track newTrack)
        {
            try
            {
                var createdTrack = await _trackRepo.CreateNewTrack(newTrack);

                return Ok(createdTrack);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
        }
        [HttpPut("")]
        public IActionResult UpdateTrack([FromBody] Track newTrack)
        {
            try
            {
                var updatedTrack = _trackRepo.UpdateTrack(newTrack);

                if (updatedTrack == null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);

                return Ok(updatedTrack);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
        [HttpDelete("")]
        public IActionResult DeleteTrack([FromBody] Track track)
        {
            try
            {
                var deletedTrack = _trackRepo.DeleteTrack(track);

                if (deletedTrack == null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);

                return Ok(deletedTrack);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
