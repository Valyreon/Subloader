using System.Net;
using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

public class LoginInfo
{
    [JsonPropertyName("base_url")]
    public string BaseUrl { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("user")]
    public UserInfo User { get; set; }

    [JsonPropertyName("status")]
    internal HttpStatusCode Status { get; set; }
}
