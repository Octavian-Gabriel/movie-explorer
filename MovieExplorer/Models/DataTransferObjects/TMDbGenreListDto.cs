using System.Text.Json.Serialization;

namespace MovieExplorer.Models.DataTransferObjects
{
    public class TMDbGenreListDto
    {
        [JsonPropertyName("genres")]
        public List<TMDbMovieGenreDto> Genres { get; set; } = new List<TMDbMovieGenreDto>();
    }
}
