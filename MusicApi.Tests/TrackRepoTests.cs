using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MusicApi.Models;
using MusicApi.StaticData;
using NUnit.Framework.Internal;

namespace MusicApi.Tests
{
    [TestFixture]
    public class TrackRepoTests
    {
        AppDbContext appDbContext;
        ITrackRepo trackRepo;
        SqliteConnection connection;
        static int seedDataCount = 5;
        static int nonExistingId = 1000;
        public IEnumerable<Track> GetProductsSeedData()
        {
            return new List<Track>()
            {
                new Track{Id=1,Name="Track1"},
                new Track{Id=2,Name="Track2"},
                new Track{Id=3,Name="Track3"},
                new Track{Id=4,Name="Track4"},
                new Track{Id=5,Name="Track5"},
            };
        }
        [SetUp]
        public void Setup()
        {
            // create and open new SQLite Connection
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            // configure options 
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite(connection)
               .Options;
            // seeding new context 
            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Tracks.AddRange(GetProductsSeedData());
                context.SaveChanges();
            }
            // testing context
            appDbContext = new AppDbContext(options);
        }
        [TearDown]
        public void TearDown()
        {
            appDbContext.Dispose();
            connection.Close();
        }
        // naming = method name (Testing)_Input State_ Expected Output
        // GetAll
        [Test]
        public async Task GetAllTracks_ValidData_NotNull()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            // Act
            var result = await trackRepo.GetAllAsync();
            // Assert
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public async Task GetAllTracks_ValidData_CountEqualSeed()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            // Act
            var result = await trackRepo.GetAllAsync();
            // Assert
            Assert.That(result.Count(), Is.EqualTo(seedDataCount));
        }
        // GetById
        [Test]
        public async Task GetById_ValidId_ValidTrack()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            int testId = 1;
            var expectedTrack = GetProductsSeedData().FirstOrDefault(p => p.Id == testId);
            // Act
            var result = await trackRepo.GetByIdAsync(testId);
            // Assert
            Assert.That(result.Id, Is.EqualTo(expectedTrack.Id));
        }
        [Test]
        public async Task GetById_InvalidId_Null()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            int testId = -1;
            // Act
            var result = await trackRepo.GetByIdAsync(testId);
            // Assert
            Assert.That(result, Is.Null);
        }
        // CreateNew
        [Test]
        public async Task CreateNewTrack_ValidTrack_NewTrack()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var newTrack = new Track { Name = "newTrack" };
            // Act
            var result = await trackRepo.CreateNewTrack(newTrack);
            // Assert
            Assert.That(result.Name, Is.EqualTo(newTrack.Name));
        }
        [Test]
        public async Task CreateNewTrack_InvalidTrackName_Throws()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var newTrack = new Track { Name = "" };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await trackRepo.CreateNewTrack(newTrack));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityData));
        }
        [Test]
        public async Task CreateNewTrack_InvalidFormatTrackId_Throws()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var newTrack = new Track { Id = -1, Name = "trackName" };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                   await trackRepo.CreateNewTrack(newTrack));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityId));
        }
        // Update
        [Test]
        public void UpdateTrack_ValidTrack_UpdatedTrack()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var updatedTrack = GetProductsSeedData().First();
            updatedTrack.Name = "UpdatedTrack";
            // Act
            var result = trackRepo.UpdateTrack(updatedTrack);
            // Assert
            Assert.That(result.Id, Is.EqualTo(updatedTrack.Id));
            Assert.That(result.Name, Is.EqualTo(updatedTrack.Name));
        }
        [Test]
        public void UpdateTrack_InvalidTrackName_Throws()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var updatedTrack = GetProductsSeedData().First();
            updatedTrack.Name = null;

            var exception = Assert.Throws<ArgumentException>(() =>
                trackRepo.UpdateTrack(updatedTrack));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityData));
        }
        [Test]
        public void UpdateTrack_InvalidFormatTrackId_Throws()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var toUpdateTrack = new Track { Id = -1, Name = "trackName" };

            var exception = Assert.Throws<ArgumentException>(() =>
                trackRepo.UpdateTrack(toUpdateTrack));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityId));
        }
        [Test]
        public void UpdateTrack_NonExistingTrackId_Throws()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var toUpdateTrack = new Track { Id = nonExistingId, Name = "trackName" };

            var exception = Assert.Throws<ArgumentException>(() =>
                trackRepo.UpdateTrack(toUpdateTrack));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.EntityDoesntExist));
        }
        // Delete Track
        [Test]
        public void DeleteTrack_ValidId_toDeleteTrack()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var toDeleteTrack = GetProductsSeedData().First();
            // Act
            var result = trackRepo.DeleteTrack(toDeleteTrack);
            // Assert
            Assert.That(result.Id, Is.EqualTo(toDeleteTrack.Id));
        }
        [Test]
        public void DeleteTrack_InvalidFormatTrackId_Throws()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var toDeleteTrack = new Track { Id = -1, Name = "trackName" };

            var exception = Assert.Throws<ArgumentException>(() =>
                trackRepo.DeleteTrack(toDeleteTrack));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityId));
        }
        [Test]
        public void DeleteTrack_NonExistingTrackId_Throws()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var toDeleteTrack = new Track { Id = nonExistingId, Name = "trackName" };

            var exception = Assert.Throws<ArgumentException>(() =>
                trackRepo.DeleteTrack(toDeleteTrack));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.EntityDoesntExist));
        }
    }
}
