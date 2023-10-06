using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

internal class UserInfoResponse
{
    [JsonPropertyName("data")]
    public UserInfo Data { get; set; }
}
