using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MovieExplorer.Controllers;
using MovieExplorer.Data;
using MovieExplorer.Models;
using MovieExplorer.Models.ViewModels;
using MovieExplorer.Services.Interfaces;
using System.Security.Claims;
using Xunit;

namespace MovieExplorer.Tests
{
    public class MoviesControllerTests:IClassFixture<DatabaseFixture>
    {
        private readonly Mock<IMovieService> _movieServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly MovieExplorerDbContext _dbContext;


        public MoviesControllerTests(DatabaseFixture databaseFixture)
        {
            _movieServiceMock = new Mock<IMovieService>();
            _userServiceMock = new Mock<IUserService>();
            _dbContext = databaseFixture.dbContext;
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewResultWithModel()
        {
            // Arrange
            var movieId = 1;
            var movieDetails = new MovieDetailsViewModel
            {
                Id = movieId,
                Title = "Movie 1",
                Description = "Description"
            };
            _movieServiceMock.Setup(s => s.GetMovieDetails(movieId))
                .ReturnsAsync(movieDetails);

            var controller = new MoviesController(_movieServiceMock.Object, _userServiceMock.Object, _dbContext );

            // Act
            var result = await controller.Details(movieId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MovieDetailsViewModel>(viewResult.Model);
            Assert.Equal(movieId, model.Id);
            Assert.Equal("Movie 1", model.Title);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var movieId = 1;
            _movieServiceMock.Setup(s => s.GetMovieDetails(movieId))
                .ReturnsAsync((MovieDetailsViewModel)null);

            var controller = new MoviesController(_movieServiceMock.Object, _userServiceMock.Object, _dbContext);

            // Act
            var result = await controller.Details(movieId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddComment_ValidUser_AddsCommentAndRedirects()
        {
            // Arrange
            var movieId = 1;
            var userId = 1;
            var user = new User { Id = userId, UserName = "testuser",Email="test",PasswordHash="test" };
            _userServiceMock.Setup(s => s.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var controller = new MoviesController(_movieServiceMock.Object, _userServiceMock.Object, _dbContext )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Session = new MockHttpSession();
            controller.HttpContext.Session.SetInt32("UserId", userId);

            // Act
            var result = await controller.AddComment(movieId, "Great movie!");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(movieId, redirectResult.RouteValues["id"]);

            var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.MovieId == movieId);
            Assert.NotNull(comment);
            Assert.Equal("Great movie!", comment.Content);
            Assert.Equal(userId, comment.UserId);
            Assert.Equal("testuser", comment.UserName);
        }
    }

    // Mock ISession for testing
    public class MockHttpSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new();

        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[] value)
        {
            return _sessionStorage.TryGetValue(key, out value);
        }

        // Helper methods for session testing
        public void SetInt32(string key, int value)
        {
            Set(key, BitConverter.GetBytes(value));
        }

        public int? GetInt32(string key)
        {
            if (TryGetValue(key, out var value))
            {
                return BitConverter.ToInt32(value, 0);
            }
            return null;
        }
    }
}