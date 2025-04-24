using System.Text.Json.Serialization;

namespace MovieExplorer.Models.DataTransferObjects
{
    public class TMDbResponseDto
    {
        [JsonPropertyName("results")]
        public List<TMDbMovieDto>? Results { get; set; }
    }
}
