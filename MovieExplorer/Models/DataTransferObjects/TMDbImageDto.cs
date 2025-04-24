using System.Text.Json.Serialization;

namespace MovieExplorer.Models.DataTransferObjects
{
    public class TMDbImageDto
    {
        [JsonPropertyName("file_path")]
        public string? FilePath { get; set; }
    }
}
