using System.Text.Json.Serialization;

namespace MovieExplorer.Models.DataTransferObjects
{
    public class TMDbCastDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("character")]
        public string? Character { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }
    }
}
