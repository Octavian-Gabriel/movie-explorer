using MovieExplorer.Models.ViewModels;
namespace MovieExplorer.Services.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieListViewModel>> GetLatestMovies(int page);
        Task<IEnumerable<MovieListViewModel>> GetTopRatedMovies(int page);
        Task<IEnumerable<MovieListViewModel>> SearchMovies(string movieName, int? gendreId);
        Task<Dictionary<int, string>> GetGenres();
        Task<MovieDetailsViewModel> GetMovieDetails(int movieId);
    }
}
