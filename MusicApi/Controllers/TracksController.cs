﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using MusicApi.Helpers;
using MusicApi.Helpers.Config.FilesConfig;
using MusicApi.Services.FileServices;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TracksController : ControllerBase
    {
        private readonly ITrackRepo _trackRepo;
        private readonly IGenreRepo _genreRepo;
        private readonly IFileService _fileService;
        private readonly FileTypes fileType = FileTypes.TrackFile;
        public TracksController(ITrackRepo trackRepo, IGenreRepo genreRepo, IFileService fileService)
        {
            _trackRepo = trackRepo;
            _genreRepo = genreRepo;
            _fileService = fileService;
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
        public async Task<IActionResult> CreateNewTrack([FromForm] AddTrackDto addTrackDto)
        {
            try
            {
                // First map to domain model then pass it to services
                var newTrack = addTrackDto.Adapt<Track>();

                // check attached genres ensure all exist in the database
                if (addTrackDto.Genres.Count() == 0)
                    return BadRequest(ExceptionMessages.InvalidEntityData);
                var genres = await _genreRepo.GetAllWithIdAsync(addTrackDto.Genres);
                // check attached track file
                if (!_fileService.CheckFileSpecs(addTrackDto.TrackFile, fileType))
                    throw new ArgumentException("Check track data");
                // check returned list to be equal to attached one (throw error or attach it)
                if (newTrack.Genres.Count() != genres.Count())
                    throw new ArgumentException("Check genres data");
                newTrack.Genres = (ICollection<Genre>)genres;

                // Data file
                var trackFileSaveDto = await _fileService.SaveTrackFileHLS(addTrackDto.TrackFile, newTrack.Name);
                if (!trackFileSaveDto.isSaved)
                    throw new ArgumentException("Error saving file");

                // Update track file props
                newTrack.TrackPath = trackFileSaveDto.FileSavePathHLS;
                newTrack.LengthInSeconds = trackFileSaveDto.FileDurationInSeconds;

                var createdTrack = await _trackRepo.CreateNewTrack(newTrack);

                // Map resulting domain model back to Dto for transfer
                var responseDto = createdTrack.Adapt<TrackDto>();
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
        }
        [HttpPut("")]
        public async Task<IActionResult> UpdateTrack([FromBody] TrackDto updatedTrackDto)
        {
            try
            {
                // query database for the track to be updated
                var trackToBeUpdated = await _trackRepo.GetByIdAsync(updatedTrackDto.Id);
                if (trackToBeUpdated == null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);

                // query attached genres ids to ensure all exist in the database
                var genres = await _genreRepo.GetAllWithIdAsync(updatedTrackDto.Genres.Select(g => g.Id).ToList());

                // compare attached genres and returned from database
                if (genres.Count() != updatedTrackDto.Genres.Count())
                    throw new ArgumentException("Check genres data");

                // update the fields from the dto manually
                trackToBeUpdated.Name = updatedTrackDto.Name;
                trackToBeUpdated.Genres.Clear();
                trackToBeUpdated.Genres = (ICollection<Genre>)genres;

                var updatedTrack = _trackRepo.UpdateTrack(trackToBeUpdated);

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
