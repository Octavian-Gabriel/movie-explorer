using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using MovieExplorer.Data;
using MovieExplorer.Models;
using MovieExplorer.Models.DataTransferObjects;
using MovieExplorer.Models.ViewModels;
using MovieExplorer.Services;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MovieExplorer.Tests
{
    public class MovieServiceTests:IClassFixture<DatabaseFixture>
    {

        private readonly MovieExplorerDbContext _dbContext;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MovieServiceTests(DatabaseFixture databaseFixture)
        {
            // Set up in-memory database
            _dbContext = databaseFixture.dbContext;

            // Set up HttpClient mock
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");

            // Set up IConfiguration mock
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["TMDb:ApiKey"]).Returns("test-api-key");
            _configuration = configurationMock.Object;
        }

        [Fact]
        public async Task GetLatestMovies_ReturnsMovieListViewModels()
        {
            // Arrange
            var tmdbResponse = new TMDbResponse
            {
                Results = new List<TMDbMovie>
                {
                    new TMDbMovie { Id = 1, Title = "Movie 1", PosterPath = "/poster1.jpg", ReleaseDate = "2023-01-01" }
                }
            };
            var jsonResponse = JsonSerializer.Serialize(tmdbResponse);
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var movieService = new MovieService(_httpClient, _configuration, _dbContext);

            // Act
            var result = await movieService.GetLatestMovies();

            // Assert
            var movies = result.ToList();
            Assert.Single(movies);
            Assert.Equal(1, movies[0].Id);
            Assert.Equal("Movie 1", movies[0].Title);
            Assert.Equal("https://image.tmdb.org/t/p/w200/poster1.jpg", movies[0].PosterPath);
            Assert.Equal("2023-01-01", movies[0].ReleaseDate);
        }
        [Fact]
        public async Task GetMovieDetails_ReturnsMovieDetailsWithComments()
        {
            // Arrange
            // Mock movie details response
            var movieDetails = new TMDbMovieDetails { Id = 1, Title = "Movie 1", Overview = "Overview" };
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("movie/1?")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(movieDetails))
                });

            // Mock images response
            var imagesResponse = new TMDbImagesResponse
            {
                Backdrops = new List<TMDbImage> { new TMDbImage { FilePath = "/backdrop.jpg" } },
                Posters = new List<TMDbImage> { new TMDbImage { FilePath = "/poster.jpg" } }
            };
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("movie/1/images")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(imagesResponse))
                });

            // Mock credits response
            var creditsResponse = new TMDbCreditsResponse
            {
                Cast = new List<TMDbCast> { new TMDbCast { Name = "Actor 1", Character = "Character 1", ProfilePath = "/actor.jpg" } }
            };
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("movie/1/credits")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(creditsResponse))
                });
            var testUser = new User
            {
                Email = "Test",
                Id = 1,
                UserName="Test",
                PasswordHash="test"
            };
            // Add a comment to the in-memory database
            _dbContext.Comments.Add(new Comment
            {
                Id = 1,
                MovieId = 1,
                UserId = 1,
                User= testUser,
                UserName = "testuser",
                Content = "Great movie!",
                CreatedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            var movieService = new MovieService(_httpClient, _configuration, _dbContext);

            // Act
            var result = await movieService.GetMovieDetails(1);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal("Movie 1", result.Title);
            Assert.Equal("Overview", result.Description);
            Assert.Contains("https://image.tmdb.org/t/p/w1280/backdrop.jpg", result.ImageUrls);
            Assert.Contains("https://image.tmdb.org/t/p/w500/poster.jpg", result.ImageUrls);
            Assert.Single(result.Actors);
            Assert.Equal("Actor 1", result.Actors[0].Name);
            Assert.Single(result.Comments);
            Assert.Equal("Great movie!", result.Comments[0].Content);
        }
    }
}