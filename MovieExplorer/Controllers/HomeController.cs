using Microsoft.AspNetCore.Mvc;
using MovieExplorer.Models;
using MovieExplorer.Services.Interfaces;
using System.Diagnostics;

namespace MovieExplorer.Controllers
{
    public class HomeController(IMovieService movieService) : Controller
    {

        public async Task<IActionResult> Index(int page = 1)
        {
            var latestMovies = await movieService.GetLatestMovies(page);
            ViewBag.CurrentPage = page;
            return View(latestMovies);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> LoadMoreLatestMovies(int page)
        {
            var movies = await movieService.GetLatestMovies(page);
            return PartialView("_MovieListPartial", movies);
        }

        public async Task<IActionResult> LoadMoreTopRatedMovies(int page)
        {
            var movies = await movieService.GetTopRatedMovies(page);
            return PartialView("_MovieListPartial", movies);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
