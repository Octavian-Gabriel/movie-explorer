using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MovieExplorer.Data;
using MovieExplorer.Models.ViewModels;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MovieExplorer.Services
{
    public class MovieService(HttpClient httpClient, IConfiguration configuration, MovieExplorerDbContext dbContext) : IMovieService

    {
        private readonly string _apiKey = configuration["TMDb:ApiKey"]?? throw new ArgumentNullException(nameof(configuration));
        private static readonly Dictionary<int, string> _genreCache = [];
        public async Task<IEnumerable<MovieListViewModel>> GetLatestMovies()
        {
            var response = await httpClient.GetFromJsonAsync<TMDbResponse>($"movie/now_playing?api_key={_apiKey}")
                ?? throw new InvalidOperationException("Failed to retrieve latest movies.");
            return ParseResponse(response);
        }


        public async Task<IEnumerable<MovieListViewModel>> GetTopRatedMovies()
        {
            var response = await httpClient.GetFromJsonAsync<TMDbResponse>($"movie/top_rated?api_key={_apiKey}")
                ?? throw new InvalidOperationException("Failed to retrieve top rated movies.");
            return ParseResponse(response);
        }

        public async Task<IEnumerable<MovieListViewModel>> SearchMovies(string movieName, int? genreId)
        {
            var queryParams=new List<string> { $"api_key={_apiKey}" };
            if (!string.IsNullOrEmpty(movieName))
            {
                queryParams.Add($"query={Uri.EscapeDataString(movieName)}");
            }
            if (genreId.HasValue)
            {
                queryParams.Add($"with_genres={genreId.Value}");
            }

            var queryString=string.Join("&", queryParams);
            var url=$"search/movie?{queryString}";
            var response = await httpClient.GetFromJsonAsync<TMDbResponse>(url)
                ?? throw new InvalidOperationException("Failed to retrieve movies by name and/or genre");

            return ParseResponse(response);
        }
        public async Task<Dictionary<int, string>> GetGenres()
        {
            if (_genreCache.Count != 0)
            {
                return _genreCache;
            }
            var response = await httpClient.GetFromJsonAsync<TMDbGenreListResponse>($"genre/movie/list?api_key={_apiKey}")
                ?? throw new InvalidOperationException("Failed to retrieve genres list!");
            foreach (var genre in response.Genres )
            {
                if(genre.Id!=null && !string.IsNullOrEmpty(genre.GenreName))
                {
                    _genreCache.Add((int)genre.Id, genre.GenreName);
                }
            }
            return _genreCache;
        }
        public async Task<MovieDetailsViewModel> GetMovieDetails(int movieId)
        {
            // Fetch movie details
            var movieDetails = await httpClient.GetFromJsonAsync<TMDbMovieDetails>($"movie/{movieId}?api_key={_apiKey}")
                ?? throw new InvalidOperationException("Failed to retrieve movie details.");

            // Fetch movie images
            var imagesResponse = await httpClient.GetFromJsonAsync<TMDbImagesResponse>($"movie/{movieId}/images?api_key={_apiKey}")
                ?? throw new InvalidOperationException("Failed to retrieve movie images.");

            // Fetch movie credits
            var creditsResponse = await httpClient.GetFromJsonAsync<TMDbCreditsResponse>($"movie/{movieId}/credits?api_key={_apiKey}")
                ?? throw new InvalidOperationException("Failed to retrieve movie credits.");

            var imageUrls = new List<string>();
            imageUrls.AddRange(imagesResponse.Backdrops?
                .Take(5)
                .Select(b => $"https://image.tmdb.org/t/p/w1280{b.FilePath}") ?? Enumerable.Empty<string>());
            imageUrls.AddRange(imagesResponse.Posters?
                .Take(1)
                .Select(p => $"https://image.tmdb.org/t/p/w500{p.FilePath}") ?? Enumerable.Empty<string>());
            

            var actors = creditsResponse.Cast?
                .Take(10) 
                .Select(c => new ActorViewModel
                {
                    Name = c.Name ?? "Unknown",
                    Character = c.Character ?? "N/A",
                    ProfileImgUrl = string.IsNullOrEmpty(c.ProfilePath) ? null : $"https://image.tmdb.org/t/p/w185{c.ProfilePath}"
                }) ?? Enumerable.Empty<ActorViewModel>();


            //Get comments
            var comments = await dbContext.Comments.
                Where(c => c.MovieId == movieId).
                OrderByDescending(c => c.CreatedAt).
                Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                }).ToListAsync();

            return new MovieDetailsViewModel
            {
                Id = movieId,
                Title = movieDetails.Title ?? "Unknown",
                Description = movieDetails.Overview ?? "N/A",
                ImageUrls = imageUrls.ToList(),
                Actors = actors.ToList(),
                Comments = comments
            };
        }

        private IEnumerable<MovieListViewModel> ParseResponse(TMDbResponse response)
        {
            return response.Results?.Select(r => new MovieListViewModel
            {
                Id = r.Id,
                Title = r.Title ?? "unknown",
                PosterPath = string.IsNullOrEmpty(r.PosterPath)
                ? "https://via.placeholder.com/200x300?text=No+Poster" : $"https://image.tmdb.org/t/p/w200{r.PosterPath}",
                ReleaseDate = r.ReleaseDate ?? "N/A"
            }) ?? Enumerable.Empty<MovieListViewModel>();
        }
        private class TMDbResponse
        {
            [JsonPropertyName("results")]
            public  List<TMDbMovie>? Results { get; set; }
        }

        private class TMDbMovie
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;
            [JsonPropertyName("poster_path")]
            public string PosterPath {  get; set; }=string.Empty;
            [JsonPropertyName("release_date")]
            public string ReleaseDate { get; set; } = string.Empty;
        }

        private class TMDbGenreListResponse
        {
            public List<TMDbMovieGenre> Genres { get; set; }
        }

        private class TMDbMovieGenre
        {
            [JsonPropertyName("id")]
            public int? Id { get; set; }
            [JsonPropertyName("name")]
            public string? GenreName { get; set; }
        }
        private class TMDbMovieDetails
        {
            [JsonPropertyName("id")]
            public int? Id { get; set; }

            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("overview")]
            public string? Overview { get; set; }
        }

        private class TMDbImagesResponse
        {
            [JsonPropertyName("posters")]
            public List<TMDbImage>? Posters { get; set; }

            [JsonPropertyName("backdrops")]
            public List<TMDbImage>? Backdrops { get; set; }
        }

        private class TMDbImage
        {
            [JsonPropertyName("file_path")]
            public string? FilePath { get; set; }
        }

        private class TMDbCreditsResponse
        {
            [JsonPropertyName("cast")]
            public List<TMDbCast>? Cast { get; set; }
        }

        private class TMDbCast
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("character")]
            public string? Character { get; set; }

            [JsonPropertyName("profile_path")]
            public string? ProfilePath { get; set; }
        }
    }
}
