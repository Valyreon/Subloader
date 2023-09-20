using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp.Dtos
{
    internal class ErrorResponse : MessageResponse
    {
        [JsonPropertyName("errors")]
        public IReadOnlyList<string> Errors { get; set; }
    }
}
