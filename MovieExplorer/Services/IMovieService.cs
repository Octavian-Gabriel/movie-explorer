using MovieExplorer.Models.ViewModels;
namespace MovieExplorer.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieListViewModel>> GetLatestMovies();
        Task<IEnumerable<MovieListViewModel>> GetTopRatedMovies();

        Task<IEnumerable<MovieListViewModel>> SearchMovies(string movieName, int? gendreId);
    }
}
