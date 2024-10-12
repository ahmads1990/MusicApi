namespace MusicApi.Repositories.Interfaces
{
    public interface IGenreRepo
    {
        public Task<IEnumerable<Genre>> GetAllAsync();
        public Task<Genre?> GetByIdAsync(int id);
        public Task<IEnumerable<Genre>> GetAllWithIdAsync(IEnumerable<int> genreIds);
        public bool CheckGenreExist(int id);
        public Task<Genre> CreateNewGenre(Genre newGenre);
        public Genre? UpdateGenre(Genre genre);
        public Genre? DeleteGenre(Genre genre);
    }
}
