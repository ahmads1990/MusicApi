using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MusicApi.Controllers;
using MusicApi.Dtos;
using MusicApi.Helpers;
using MusicApi.Models;
using MusicApi.Repositories.Interfaces;
using MusicApi.Services.FileServices;

namespace MusicApi.Tests
{
    [TestFixture]
    public class TracksControllerTests
    {
        TracksController tracksController;
        Mock<ITrackService> trackServiceMock;
        Mock<IGenreService> genreServiceMock;
        Mock<IFileService> fileServiceMock;
        [SetUp]
        public void Setup()
        {
            trackServiceMock = new Mock<ITrackService>();
            genreServiceMock = new Mock<IGenreService>();
            fileServiceMock = new Mock<IFileService>();
            tracksController = new TracksController(trackServiceMock.Object, genreServiceMock.Object, fileServiceMock.Object);
        }
        // names = (httpVerb)_Endpoint_Input_ExpectedResult
        // GetTracks
        [Test]
        public async Task Get_GetTracks_Ok()
        {
            trackServiceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Track>() { new Track(), new Track() });

            var result = await tracksController.GetTracks();

            trackServiceMock.Verify(x => x.GetAllAsync(), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult?.Value, Is.InstanceOf<List<Track>>());
            var emp = okObjectResult.Value as List<Track>;
            Assert.That(emp?.Count, Is.EqualTo(2));
        }
        // GetTrackById
        [Test]
        public async Task Get_GetTrackById_ValidId_Ok()
        {
            trackServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Track());

            var result = await tracksController.GetTrackById(100);

            trackServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult?.Value, Is.InstanceOf<Track>());
        }
        [Test]
        public async Task Get_GetTrackById_InValidId_NotFound()
        {
            trackServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()));

            var result = await tracksController.GetTrackById(100);

            trackServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        // CreateNewTrack
        [Test]
        public async Task Post_CreateNewTrack_ValidData_Ok()
        {
            // the incoming do from the "request" for the controller
            var trackDto = new AddTrackDto { Name = "track", Genres = new List<int>() { 1 } };
            // map it to domain model for the mock repo to return it
            var track = trackDto.Adapt<Track>();
            trackServiceMock.Setup(s => s.CreateNewTrack(It.IsAny<Track>()))
                .ReturnsAsync(track);
            genreServiceMock.Setup(s => s.GetAllWithIdAsync(It.IsAny<IEnumerable<int>>()))
              .ReturnsAsync(new List<Genre>() { new Genre() });

            // fileServiceMock
            fileServiceMock.Setup(
                s => s.SaveTrackFileHLS(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync(new TrackFileSaveDto { isSaved = true });
            fileServiceMock.Setup(
                s => s.CheckFileSpecs(It.IsAny<IFormFile>(), It.IsAny<FileTypes>()))
                .Returns(true);

            var result = await tracksController.CreateNewTrack(trackDto);

            trackServiceMock.Verify(x => x.CreateNewTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        [Test]
        public async Task Post_CreateNewTrack_InValidData_BadRequest()
        {
            var trackDto = new AddTrackDto { Name = "", Genres = new List<int>() { 1 } };
            trackServiceMock.Setup(s => s.CreateNewTrack(It.IsAny<Track>()))
                .ThrowsAsync(new ArgumentException());
            genreServiceMock.Setup(s => s.GetAllWithIdAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<Genre>() { new Genre() });

            // fileServiceMock
            fileServiceMock.Setup(
                s => s.SaveTrackFileHLS(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync(new TrackFileSaveDto { isSaved = true });
            fileServiceMock.Setup(
                s => s.CheckFileSpecs(It.IsAny<IFormFile>(), It.IsAny<FileTypes>()))
                .Returns(true);

            var result = await tracksController.CreateNewTrack(trackDto);

            trackServiceMock.Verify(x => x.CreateNewTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
        // UpdateTrack
        [Test]
        public async Task Put_UpdateTrack_ValidData_Ok()
        {
            var trackDto = new TrackDto { Name = "track", Genres = new List<GenreDto>() };
            var track = trackDto.Adapt<Track>();
            // track repo returns found track
            trackServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(track);

            // track repo updates track correctly and return updated track
            trackServiceMock.Setup(s => s.UpdateTrack(It.IsAny<Track>()))
                .Returns(track);

            var result = await tracksController.UpdateTrack(trackDto);

            trackServiceMock.Verify(x => x.UpdateTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        [Test]
        public async Task Put_UpdateTrack_NonExsitingData_NotFound()
        {
            var trackDto = new TrackDto { Name = "", Genres = new List<GenreDto>() };
            // not specify a return as (repo couldn't find track and returned null)
            trackServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()));

            var result = await tracksController.UpdateTrack(trackDto);

            // should throw exception before updateTrack on repo is called
            trackServiceMock.Verify(x => x.UpdateTrack(It.IsAny<Track>()), Times.Never);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        [Test]
        public async Task Put_UpdateTrack_InValidData_BadRequest()
        {
            var trackDto = new TrackDto { Id = 1, Name = "", Genres = new List<GenreDto>() };
            var track = trackDto.Adapt<Track>();

            // track repo returns found track
            trackServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(track);
            // track repo tries to update track but found invalid data
            trackServiceMock.Setup(s => s.UpdateTrack(It.IsAny<Track>()))
                .Throws(new ArgumentException());

            var result = await tracksController.UpdateTrack(trackDto);

            trackServiceMock.Verify(x => x.UpdateTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
        // DeleteTrack
        [Test]
        public void Delete_DeleteTrack_ValidData_Ok()
        {
            var trackDto = new TrackDto { Name = "" };
            var track = trackDto.Adapt<Track>();
            trackServiceMock.Setup(s => s.DeleteTrack(It.IsAny<Track>()))
                .Returns(track);

            var result = tracksController.DeleteTrack(trackDto);

            trackServiceMock.Verify(x => x.DeleteTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        [Test]
        public void Delete_DeleteTrack_NoExsitingData_NotFound()
        {
            var trackDto = new TrackDto { Name = "" };
            trackServiceMock.Setup(s => s.DeleteTrack(It.IsAny<Track>()));

            var result = tracksController.DeleteTrack(trackDto);

            trackServiceMock.Verify(x => x.DeleteTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        [Test]
        public void Delete_DeleteTrack_InValidData_BadRequest()
        {
            var trackDto = new TrackDto { Name = "" };
            trackServiceMock.Setup(s => s.DeleteTrack(It.IsAny<Track>()))
                .Throws(new ArgumentException());

            var result = tracksController.DeleteTrack(trackDto);

            trackServiceMock.Verify(x => x.DeleteTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
