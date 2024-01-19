using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MusicApi.Controllers;
using MusicApi.Dtos;
using MusicApi.Models;
using MusicApi.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicApi.Tests
{
    [TestFixture]
    public class TracksControllerTests
    {
        TracksController tracksController;
        Mock<ITrackRepo> trackRepoMock;
        Mock<IGenreRepo> genreRepoMock;
        [SetUp]
        public void Setup()
        {
            trackRepoMock = new Mock<ITrackRepo>();
            genreRepoMock = new Mock<IGenreRepo>();
            tracksController = new TracksController(trackRepoMock.Object, genreRepoMock.Object);
        }
        // names = (httpVerb)_Endpoint_Input_ExpectedResult
        // GetTracks
        [Test]
        public async Task Get_GetTracks_Ok()
        {
            trackRepoMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Track>() { new Track(), new Track() });

            var result = await tracksController.GetTracks();

            trackRepoMock.Verify(x => x.GetAllAsync(), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult.Value, Is.InstanceOf<List<Track>>());
            var emp = okObjectResult.Value as List<Track>;
            Assert.That(emp.Count, Is.EqualTo(2));
        }
        // GetTrackById
        [Test]
        public async Task Get_GetTrackById_ValidId_Ok()
        {
            trackRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Track());

            var result = await tracksController.GetTrackById(100);

            trackRepoMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult.Value, Is.InstanceOf<Track>());
        }
        [Test]
        public async Task Get_GetTrackById_InValidId_NotFound()
        {
            trackRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()));

            var result = await tracksController.GetTrackById(100);

            trackRepoMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        // CreateNewTrack
        [Test]
        public async Task Post_CreateNewTrack_ValidData_Ok()
        {
            // the incoming do from the "request" for the controller
            var trackDto = new TrackDto { Name = "track", Genres = new List<GenreDto>() };
            // map it to domain model for the mock repo to return it
            var track = trackDto.Adapt<Track>();
            trackRepoMock.Setup(s => s.CreateNewTrack(It.IsAny<Track>()))
                .ReturnsAsync(track);
            genreRepoMock.Setup(s => s.GetAllWithIdAsync(It.IsAny<IEnumerable<int>>()))
              .ReturnsAsync(new List<Genre>());

            var result = await tracksController.CreateNewTrack(trackDto);

            trackRepoMock.Verify(x => x.CreateNewTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        [Test]
        public async Task Post_CreateNewTrack_InValidData_BadRequest()
        {
            var trackDto = new TrackDto { Name = "", Genres = new List<GenreDto>() };
            trackRepoMock.Setup(s => s.CreateNewTrack(It.IsAny<Track>()))
                .ThrowsAsync(new ArgumentException());
            genreRepoMock.Setup(s => s.GetAllWithIdAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<Genre>());

            var result = await tracksController.CreateNewTrack(trackDto);

            trackRepoMock.Verify(x => x.CreateNewTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
        // UpdateTrack
        [Test]
        public async Task Put_UpdateTrack_ValidData_Ok()
        {
            var trackDto = new TrackDto { Name = "track", Genres = new List<GenreDto>() };
            var track = trackDto.Adapt<Track>();
            // track repo returns found track
            trackRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(track);

            // track repo updates track correctly and return updated track
            trackRepoMock.Setup(s => s.UpdateTrack(It.IsAny<Track>()))
                .Returns(track);

            var result = await tracksController.UpdateTrack(trackDto);

            trackRepoMock.Verify(x => x.UpdateTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        [Test]
        public async Task Put_UpdateTrack_NonExsitingData_NotFound()
        {
            var trackDto = new TrackDto { Name = "", Genres = new List<GenreDto>() };
            // not specify a return as (repo couldn't find track and returned null)
            trackRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()));

            var result = await tracksController.UpdateTrack(trackDto);

            // should throw exception before updateTrack on repo is called
            trackRepoMock.Verify(x => x.UpdateTrack(It.IsAny<Track>()), Times.Never);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        [Test]
        public async Task Put_UpdateTrack_InValidData_BadRequest()
        {
            var trackDto = new TrackDto { Id = 1, Name = "", Genres = new List<GenreDto>() };
            var track = trackDto.Adapt<Track>();

            // track repo returns found track
            trackRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(track);
            // track repo tries to update track but found invalid data
            trackRepoMock.Setup(s => s.UpdateTrack(It.IsAny<Track>()))
                .Throws(new ArgumentException());

            var result = await tracksController.UpdateTrack(trackDto);

            trackRepoMock.Verify(x => x.UpdateTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
        // DeleteTrack
        [Test]
        public void Delete_DeleteTrack_ValidData_Ok()
        {
            var trackDto = new TrackDto { Name = "" };
            var track = trackDto.Adapt<Track>();
            trackRepoMock.Setup(s => s.DeleteTrack(It.IsAny<Track>()))
                .Returns(track);

            var result = tracksController.DeleteTrack(trackDto);

            trackRepoMock.Verify(x => x.DeleteTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        [Test]
        public void Delete_DeleteTrack_NoExsitingData_NotFound()
        {
            var trackDto = new TrackDto { Name = "" };
            trackRepoMock.Setup(s => s.DeleteTrack(It.IsAny<Track>()));

            var result = tracksController.DeleteTrack(trackDto);

            trackRepoMock.Verify(x => x.DeleteTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        [Test]
        public void Delete_DeleteTrack_InValidData_BadRequest()
        {
            var trackDto = new TrackDto { Name = "" };
            trackRepoMock.Setup(s => s.DeleteTrack(It.IsAny<Track>()))
                .Throws(new ArgumentException());

            var result = tracksController.DeleteTrack(trackDto);

            trackRepoMock.Verify(x => x.DeleteTrack(It.IsAny<Track>()), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
