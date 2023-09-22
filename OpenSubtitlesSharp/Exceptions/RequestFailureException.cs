using System.Net;

namespace OpenSubtitlesSharp;

public class RequestFailedException : Exception
{
    public RequestFailedException(HttpStatusCode code, string message) : base(message)
    {
        StatusCode = code;
    }

    public HttpStatusCode StatusCode { get; }
}
