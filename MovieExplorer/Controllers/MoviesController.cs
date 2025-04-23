using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieExplorer.Data;
using MovieExplorer.Models;
using MovieExplorer.Models.ViewModels;
using MovieExplorer.Services.Interfaces;
using System.Numerics;

namespace MovieExplorer.Controllers
{
    public class MoviesController(IMovieService movieService,IUserService userService,MovieExplorerDbContext dbContext) : Controller
    {
        public async Task<IActionResult> Latest()
        {
            var latestMovies = await movieService.GetLatestMovies();
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
        public async Task<IActionResult>Details([FromRoute(Name = "id")] int movieId)
        {
            try
            {
                var movieDetails = await movieService.GetMovieDetails(movieId);
                return View(movieDetails);
            }
            catch (Exception ex) {
                return View("Error", new ErrorViewModel { RequestId = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult>AddComment(int movieId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                ModelState.AddModelError("content", "Comment cannot be empty.");
            }
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            var user= await userService.FindByIdAsync(userId.Value);
            
            
            var comment = new Comment
            {
                User=user,
                MovieId = movieId,
                UserId = user.Id,
                UserName = user.UserName ?? "Anonymus",
                Content = content,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Details", new {id=movieId});

        }
    }
}
