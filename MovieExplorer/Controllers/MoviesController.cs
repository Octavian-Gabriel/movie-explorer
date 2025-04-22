using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieExplorer.Models.ViewModels;
using MovieExplorer.Services;

namespace MovieExplorer.Controllers
{
    public class MoviesController(IMovieService movieService) : Controller
    {
        public async Task<IActionResult> Latest()
        {
            var latestMovies = await movieService.GetLatestMovies();
            Console.WriteLine($"Latest Movies Count: {latestMovies.Count()}");
            foreach (var movie in latestMovies)
            {
                Console.WriteLine($"Movie: {movie.Title}, Poster: {movie.PosterPath}, Release: {movie.ReleaseDate}");
            }
            return View(latestMovies);
        }

        public async Task<IActionResult> TopRated()
        {
            var topMovies = await movieService.GetTopRatedMovies();
            return View(topMovies);
        }
        [HttpPost]
        public async Task<IActionResult> Search(MovieSearchViewModel viewModel)
        {
            var genres = await movieService.GetGenres();
            if (!ModelState.IsValid)
            {
                viewModel.GenreList = genres.Select(gen => new SelectListItem
                    {
                        Value = gen.Key.ToString(),
                        Text = gen.Value
                    }).Prepend(new SelectListItem { Value = "", Text = "All Genres" });
                return View(viewModel);
            }

            var movies = await movieService.SearchMovies(
                viewModel.MovieName,
                viewModel.GenreId.HasValue ? viewModel.GenreId : null
                );
            viewModel.MovieList = movies;
            viewModel.GenreList = genres.Select(gen => new SelectListItem
            {
                Value = gen.Key.ToString(),
                Text = gen.Value
            }).Prepend(new SelectListItem { Value = "", Text = "All Genres" });

            return View(viewModel);
        }
        public async Task<IActionResult> Search()
        {
            var genres= await movieService.GetGenres();
            var viewModel = new MovieSearchViewModel
            {
                GenreList = genres.Select(gen => new SelectListItem
                {
                    Value = gen.Key.ToString(),
                    Text = gen.Value
                }).Prepend(new SelectListItem { Value = "", Text = "All Genres" })
            };
            return View(viewModel);
        }
    }
}
