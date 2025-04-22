using Microsoft.Extensions.Diagnostics.HealthChecks;
using MovieExplorer.Models.ViewModels;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MovieExplorer.Services
{
    public class MovieService(HttpClient httpClient, IConfiguration configuration) : IMovieService

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
    }
}
