using System.Text.Json.Serialization;

namespace MovieExplorer.Models.DataTransferObjects
{
    public class TMDbMovieDto
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
}
