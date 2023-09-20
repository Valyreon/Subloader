using System.Net;

namespace OpenSubtitlesSharp;

public class RequestFailureException : Exception
{
    public RequestFailureException(HttpStatusCode code, string message) : base(message)
    {
        StatusCode = code;
    }

    private RequestFailureException(string message) : base(message)
    {
    }

    private RequestFailureException(string message, Exception innerException) : base(message, innerException)
    {
    }

    private RequestFailureException() : base()
    {
    }

    public HttpStatusCode StatusCode { get; }
}
