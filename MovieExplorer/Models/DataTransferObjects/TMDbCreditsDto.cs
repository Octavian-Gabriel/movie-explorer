using System.Text.Json.Serialization;

namespace MovieExplorer.Models.DataTransferObjects
{
    public class TMDbCreditsDto
    {
        [JsonPropertyName("cast")]
        public List<TMDbCastDto>? Cast { get; set; }
    }
}
