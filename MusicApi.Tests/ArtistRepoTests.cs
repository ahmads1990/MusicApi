using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MusicApi.Models;
using MusicApi.StaticData;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicApi.Tests
{
    public class ArtistRepoTests
    {
        AppDbContext appDbContext;
        IArtistRepo artistService;
        SqliteConnection connection;
        static int seedDataCount = 5;
        static int nonExistingId = 1000;

        public IEnumerable<Artist> GetArtistSeedData()
        {
            return new List<Artist>()
            {
                new Artist { Id = 1, Name = "Artist1", Biography = "Bio1", Followers = 100, CoverPath = "cover1.jpg" },
                new Artist { Id = 2, Name = "Artist2", Biography = "Bio2", Followers = 200, CoverPath = "cover2.jpg" },
                new Artist { Id = 3, Name = "Artist3", Biography = "Bio3", Followers = 300, CoverPath = "cover3.jpg" },
                new Artist { Id = 4, Name = "Artist4", Biography = "Bio4", Followers = 400, CoverPath = "cover4.jpg" },
                new Artist { Id = 5, Name = "Artist5", Biography = "Bio5", Followers = 500, CoverPath = "cover5.jpg" }
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
                context.Artists.AddRange(GetArtistSeedData());
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

        // GetAllAsync
        [Test]
        public async Task GetAllArtists_ValidData_NotNull()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            // Act
            var result = await artistService.GetAllAsync();
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetAllArtists_ValidData_CountEqualSeed()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            // Act
            var result = await artistService.GetAllAsync();
            // Assert
            Assert.That(result.Count(), Is.EqualTo(seedDataCount));
        }

        // GetByIdAsync
        [Test]
        public async Task GetByIdAsync_ValidId_ValidArtist()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            int testId = 1;
            var expectedArtist = GetArtistSeedData().FirstOrDefault(p => p.Id == testId);
            // Act
            var result = await artistService.GetByIdAsync(testId);
            // Assert
            Assert.That(result.Id, Is.EqualTo(expectedArtist.Id));
        }

        [Test]
        public async Task GetByIdAsync_InvalidId_Null()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            int testId = -1;
            // Act
            var result = await artistService.GetByIdAsync(testId);
            // Assert
            Assert.That(result, Is.Null);
        }

        // SearchByNameAsync
        [Test]
        public async Task SearchByNameAsync_ExistingName_ValidArtist()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            string testName = "Artist1";
            var expectedArtist = GetArtistSeedData().FirstOrDefault(p => p.Name.Equals(testName, StringComparison.InvariantCultureIgnoreCase));
            // Act
            var result = await artistService.SearchByNameAsync(testName);
            // Assert
            Assert.That(result.Id, Is.EqualTo(expectedArtist.Id));
        }

        [Test]
        public async Task SearchByNameAsync_NonExistingName_Null()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            string testName = "NonExistingArtist";
            // Act
            var result = await artistService.SearchByNameAsync(testName);
            // Assert
            Assert.That(result, Is.Null);
        }

        // CheckArtistExistAsync
        [Test]
        public async Task CheckArtistExistAsync_ExistingId_True()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            int testId = 1;
            // Act
            var result = await artistService.CheckArtistExistAsync(testId);
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CheckArtistExistAsync_NonExistingId_False()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            int testId = nonExistingId;
            // Act
            var result = await artistService.CheckArtistExistAsync(testId);
            // Assert
            Assert.That(result, Is.False);
        }

        // CreateNewArtist
        [Test]
        public async Task CreateNewArtist_ValidArtist_NewArtist()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var newArtist = new Artist { Name = "NewArtist", Biography = "Bio", Followers = 1000, CoverPath = "cover.jpg" };
            // Act
            var result = await artistService.CreateNewArtist(newArtist);
            // Assert
            Assert.That(result.Name, Is.EqualTo(newArtist.Name));
        }

        [Test]
        public void CreateNewArtist_InvalidArtistName_Throws()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var newArtist = new Artist { Name = "" };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await artistService.CreateNewArtist(newArtist));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityData));
        }

        [Test]
        public void CreateNewArtist_InvalidArtistId_Throws()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var newArtist = new Artist { Id = -1, Name = "NewArtist" };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                 await artistService.CreateNewArtist(newArtist));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityId));
        }

        // UpdateArtistAsync
        [Test]
        public async Task UpdateArtistAsync_ValidArtist_UpdatedArtist()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var toUpdateArtist = GetArtistSeedData().First();
            toUpdateArtist.Name = "UpdatedArtist";
            // Act
            var result = await artistService.UpdateArtistAsync(toUpdateArtist);
            // Assert
            Assert.That(result.Id, Is.EqualTo(toUpdateArtist.Id));
            Assert.That(result.Name, Is.EqualTo(toUpdateArtist.Name));
        }

        [Test]
        public void UpdateArtistAsync_InvalidArtistName_Throws()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var toUpdateArtist = GetArtistSeedData().First();
            toUpdateArtist.Name = null;

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await artistService.UpdateArtistAsync(toUpdateArtist));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityData));
        }

        [Test]
        public void UpdateArtistAsync_InvalidArtistId_Throws()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var toUpdateArtist = new Artist { Id = -1, Name = "UpdatedArtist" };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await artistService.UpdateArtistAsync(toUpdateArtist));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityId));
        }

        [Test]
        public void UpdateArtistAsync_NonExistingArtistId_Null()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var toUpdateArtist = new Artist { Id = nonExistingId, Name = "UpdatedArtist" };

            var result = artistService.UpdateArtistAsync(toUpdateArtist);

            Assert.That(result.Result, Is.Null);
        }

        // DeleteArtistAsync
        [Test]
        public async Task DeleteArtistAsync_ValidId_DeletedArtist()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var toDeleteArtist = GetArtistSeedData().First();
            // Act
            var result = await artistService.DeleteArtistAsync(toDeleteArtist);
            // Assert
            Assert.That(result.Id, Is.EqualTo(toDeleteArtist.Id));
        }

        [Test]
        public void DeleteArtistAsync_InvalidArtistId_Throws()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var toDeleteArtist = new Artist { Id = -1, Name = "DeletedArtist" };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await artistService.DeleteArtistAsync(toDeleteArtist));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityId));
        }

        [Test]
        public void DeleteArtistAsync_NonExistingArtistId_Null()
        {
            // Arrange
            artistService = new ArtistRepo(appDbContext);
            var toDeleteArtist = new Artist { Id = nonExistingId, Name = "DeletedArtist" };

            var result = artistService.DeleteArtistAsync(toDeleteArtist);

            Assert.That(result.Result, Is.Null);
        }
    }
}
