using System.Net;
using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp.Dtos
{
    internal class MessageResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("status")]
        public HttpStatusCode? Status { get; set; }
    }
}
