using System.Text.Json.Serialization;

namespace MovieExplorer.Models.DataTransferObjects
{
    public class TMDbImagesDto
    {
        [JsonPropertyName("posters")]
        public List<TMDbImageDto>? Posters { get; set; }

        [JsonPropertyName("backdrops")]
        public List<TMDbImageDto>? Backdrops { get; set; }
    }
}
