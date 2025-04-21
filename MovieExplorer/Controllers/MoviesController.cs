using Microsoft.AspNetCore.Mvc;
using MovieExplorer.Services;

namespace MovieExplorer.Controllers
{
    public class MoviesController(IMovieService movieService):Controller
    {
        public async Task<IActionResult> Latest()
        {
            var latestMovies=await movieService.GetLatestMovies();
            return View(latestMovies);
        }

        public async Task<IActionResult> TopRated()
        {
            var topMovies= await movieService.GetTopRatedMovies();
            return View(topMovies);
        }
    }
}
