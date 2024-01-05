using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MusicApi.Models;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicApi.Tests
{
    [TestFixture]
    public class TrackRepoTests
    {
        AppDbContext appDbContext;
        ITrackRepo trackRepo;
        SqliteConnection connection;
        static int seedDataCount = 5;
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
        public async Task CreateNewTrack_InvalidTrack_Null()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var newTrack = new Track { Name = null };
            // Act
            var result = await trackRepo.CreateNewTrack(newTrack);
            // Assert
            Assert.That(result, Is.Null);
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
        public void UpdateTrack_InvalidTrack_Null()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var updatedTrack = new Track { Name = null };
            // Act
            var result = trackRepo.UpdateTrack(updatedTrack);
            // Assert
            Assert.That(result, Is.Null);
        }
        // Delete Track
        [Test]
        public void DeleteTrack_ValidTrack_toDeleteTrack()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var toDeleteTrack = GetProductsSeedData().First();
            // Act
            var result = trackRepo.UpdateTrack(toDeleteTrack);
            // Assert
            Assert.That(result.Id, Is.EqualTo(toDeleteTrack.Id));
        }
        [Test]
        public void DeleteTrack_InvalidTrack_Null()
        {
            // Arrange
            trackRepo = new TrackRepo(appDbContext);
            var toDeleteTrack = new Track { Name = null };
            // Act
            var result = trackRepo.UpdateTrack(toDeleteTrack);
            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
