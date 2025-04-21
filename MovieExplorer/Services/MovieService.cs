using MovieExplorer.Models.ViewModels;
using System.Net.Http;

namespace MovieExplorer.Services
{
    public class MovieService(HttpClient httpClient, IConfiguration configuration) : IMovieService

    {
        private readonly string _apiKey = configuration["TMDb:ApiKey"]?? throw new ArgumentNullException(nameof(configuration));

        public async Task<IEnumerable<MovieListViewModel>> GetLatestMovies()
        {
            var response = await httpClient.GetFromJsonAsync<TMDbResponse>($"movie/now_playing?api_key={_apiKey}");
            return 
                response.Results.Select(r => new MovieListViewModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    PosterPath = $"https://image.tmdb.org/t/p/w200{r.PosterPath}",
                    ReleaseDate = r.ReleaseDate,
                });
         }


        public async Task<IEnumerable<MovieListViewModel>> GetTopRatedMovies()
        {
            var response = await httpClient.GetFromJsonAsync<TMDbResponse>($"movie/top_rated?api_key={_apiKey}");
            return response.Results.Select(r => new MovieListViewModel
            {
                Id = r.Id,
                Title = r.Title,
                PosterPath = $"https://image.tmdb.org/t/p/w200{r.PosterPath}",
                ReleaseDate = r.ReleaseDate
            });
        }

        private class TMDbResponse
        {
            public  List<TMDbMovie>? Results { get; set; }
        }

        private class TMDbMovie
        {
            public int? Id { get; set; }
            public string? Title { get; set; }

            public string? PosterPath {  get; set; }
            public string? ReleaseDate { get; set; }
        }
    }
}
