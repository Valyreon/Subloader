using System.Net;

namespace OpenSubtitlesSharp;

public class RequestFailedException(HttpStatusCode code, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = code;
}
