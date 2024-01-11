using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MusicApi.Models;
using MusicApi.StaticData;

namespace MusicApi.Tests
{
    public class GenreRepoTests
    {
        AppDbContext appDbContext;
        IGenreRepo genreRepo;
        SqliteConnection connection;
        static int seedDataCount = 5;
        static int nonExistingId = 1000;
        public IEnumerable<Genre> GetProductsSeedData()
        {
            return new List<Genre>()
            {
                new Genre { Id = 1, Name = "Genre1" },
                new Genre { Id = 2, Name = "Genre2" },
                new Genre { Id = 3, Name = "Genre3" },
                new Genre { Id = 4, Name = "Genre4" },
                new Genre { Id = 5, Name = "Genre5" }
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
                context.Genres.AddRange(GetProductsSeedData());
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
        public async Task GetAllGenres_ValidData_NotNull()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            // Act
            var result = await genreRepo.GetAllAsync();
            // Assert
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public async Task GetAllGenres_ValidData_CountEqualSeed()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            // Act
            var result = await genreRepo.GetAllAsync();
            // Assert
            Assert.That(result.Count(), Is.EqualTo(seedDataCount));
        }
        // GetById
        [Test]
        public async Task GetById_ValidId_ValidGenre()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            int testId = 1;
            var expectedGenre = GetProductsSeedData().FirstOrDefault(p => p.Id == testId);
            // Act
            var result = await genreRepo.GetByIdAsync(testId);
            // Assert
            Assert.That(result.Id, Is.EqualTo(expectedGenre.Id));
        }
        [Test]
        public async Task GetById_InvalidId_Null()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            int testId = -1;
            // Act
            var result = await genreRepo.GetByIdAsync(testId);
            // Assert
            Assert.That(result, Is.Null);
        }
        // CreateNew
        [Test]
        public async Task CreateNewGenre_ValidGenre_NewGenre()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var newGenre = new Genre { Name = "newGenre" };
            // Act
            var result = await genreRepo.CreateNewGenre(newGenre);
            // Assert
            Assert.That(result.Name, Is.EqualTo(newGenre.Name));
        }
        [Test]
        public async Task CreateNewGenre_InvalidGenreName_Throws()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var newGenre = new Genre { Name = "" };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await genreRepo.CreateNewGenre(newGenre));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityData));
        }
        [Test]
        public async Task CreateNewGenre_InvalidFormatGenreId_Throws()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var newGenre = new Genre { Id = -1, Name = "newGenre" };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                 await genreRepo.CreateNewGenre(newGenre));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityId));
        }
        // Update
        [Test]
        public void UpdateGenre_ValidGenre_UpdatedGenre()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var toUpdateGenre = GetProductsSeedData().First();
            toUpdateGenre.Name = "UpdatedGenre";
            // Act
            var result = genreRepo.UpdateGenre(toUpdateGenre);
            // Assert
            Assert.That(result.Id, Is.EqualTo(toUpdateGenre.Id));
            Assert.That(result.Name, Is.EqualTo(toUpdateGenre.Name));
        }
        [Test]
        public void UpdateGenre_InvalidGenreName_Throws()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var toUpdateGenre = GetProductsSeedData().First();
            toUpdateGenre.Name = null;

            var exception = Assert.Throws<ArgumentException>(() =>
                genreRepo.UpdateGenre(toUpdateGenre));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityData));
        }
        [Test]
        public void UpdateGenre_InvaliFormatdGenreId_Throws()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var toUpdateGenre = new Genre { Id = -1, Name = "genreName" };

            var exception = Assert.Throws<ArgumentException>(() =>
                genreRepo.UpdateGenre(toUpdateGenre));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityId));
        }
        [Test]
        public void UpdateGenre_InvaliNoExistingGenreId_Null()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var toUpdateGenre = new Genre { Id = nonExistingId, Name = "genreName" };

            var result = genreRepo.UpdateGenre(toUpdateGenre);

            Assert.That(result, Is.Null);
        }
        // Delete Track
        [Test]
        public void DeleteGenre_ValidId_toDeleteGenre()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var toDeleteGenre = GetProductsSeedData().First();
            // Act
            var result = genreRepo.DeleteGenre(toDeleteGenre);
            // Assert
            Assert.That(result.Id, Is.EqualTo(toDeleteGenre.Id));
        }
        [Test]
        public void DeleteGenre_InvalidFormatGenreId_Throws()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var toDeleteGenre = new Genre { Id = -1, Name = "trackName" };

            var exception = Assert.Throws<ArgumentException>(() =>
                genreRepo.DeleteGenre(toDeleteGenre));

            Assert.That(exception.Message, Is.EqualTo(ExceptionMessages.InvalidEntityId));
        }
        [Test]
        public void DeleteGenre_InvalidNonExistingGenreId_Throws()
        {
            // Arrange
            genreRepo = new GenreRepo(appDbContext);
            var toDeleteGenre = new Genre { Id = nonExistingId, Name = "trackName" };

            var result = genreRepo.DeleteGenre(toDeleteGenre);

            Assert.That(result, Is.Null);
        }
    }
}
