using Microsoft.AspNetCore.Mvc;
using MovieExplorer.Services;

namespace MovieExplorer.Controllers
{
    public class MoviesController(IMovieService movieService):Controller
    {
        public async Task<IActionResult> Latest()
        {
            var latestMovies=await movieService.GetLatestMovies();
            Console.WriteLine($"Latest Movies Count: {latestMovies.Count()}");
            foreach (var movie in latestMovies)
            {
                Console.WriteLine($"Movie: {movie.Title}, Poster: {movie.PosterPath}, Release: {movie.ReleaseDate}");
            }
            return View(latestMovies);
        }

        public async Task<IActionResult> TopRated()
        {
            var topMovies= await movieService.GetTopRatedMovies();
            return View(topMovies);
        }
    }
}
