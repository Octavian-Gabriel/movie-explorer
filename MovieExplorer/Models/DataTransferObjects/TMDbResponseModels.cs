using System.Text.Json.Serialization;
namespace MovieExplorer.Models.DataTransferObjects
{

    public class TMDbResponse
    {
        [JsonPropertyName("results")]
        public List<TMDbMovie>? Results { get; set; }
    }

    public class TMDbMovie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;
    }

    public class TMDbGenreListResponse
    {
        public List<TMDbMovieGenre> Genres { get; set; } = new List<TMDbMovieGenre>();
    }

    public class TMDbMovieGenre
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? GenreName { get; set; }
    }

    public class TMDbMovieDetails
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }
    }

    public class TMDbImagesResponse
    {
        [JsonPropertyName("posters")]
        public List<TMDbImage>? Posters { get; set; }

        [JsonPropertyName("backdrops")]
        public List<TMDbImage>? Backdrops { get; set; }
    }

    public class TMDbImage
    {
        [JsonPropertyName("file_path")]
        public string? FilePath { get; set; }
    }

    public class TMDbCreditsResponse
    {
        [JsonPropertyName("cast")]
        public List<TMDbCast>? Cast { get; set; }
    }

    public class TMDbCast
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("character")]
        public string? Character { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }
    }
}
