using System.Text.Json.Serialization;

namespace MovieExplorer.Models.DataTransferObjects
{
    public class TMDbMovieDetailsDto
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }
        [JsonPropertyName("genres")]
        public List<TMDbMovieGenreDto> Genres { get; set; }
    }
}
