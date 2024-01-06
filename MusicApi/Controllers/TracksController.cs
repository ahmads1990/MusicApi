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
        [HttpGet("tracks")]
        public async Task<IActionResult> GetTracks()
        {
            return Ok(await _trackRepo.GetAllAsync());
        }

        [HttpGet("tracks/{trackId}")]
        public async Task<IActionResult> GetTrackById(int trackId)
        {
            return Ok(await _trackRepo.GetByIdAsync(trackId));
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewTrack([FromBody] Track newTrack)
        {
            try
            {
                var createdTrack = await _trackRepo.CreateNewTrack(newTrack);

                return Ok(createdTrack);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid track data: {ex.Message}");
            }
        }
        [HttpPut("update")]
        public IActionResult UpdateTrack([FromBody] Track newTrack)
        {
            try
            {
                var updatedTrack = _trackRepo.UpdateTrack(newTrack);
                return Ok(updatedTrack);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
        [HttpDelete("delete")]
        public IActionResult DeleteTrack([FromBody] Track track)
        {
            try
            {
                var deletedTrack = _trackRepo.UpdateTrack(track);
                return Ok(deletedTrack);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
