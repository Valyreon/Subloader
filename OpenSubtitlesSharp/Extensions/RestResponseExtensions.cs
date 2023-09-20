using System.Text.Json;
using OpenSubtitlesSharp.Dtos;
using RestSharp;

namespace OpenSubtitlesSharp.Extensions;

internal static class RestResponseExtensions
{
    public static T ParseResult<T>(this RestResponse response)
    {
        if (response.IsSuccessful)
        {
            return JsonSerializer.Deserialize<T>(response.Content);
        }

        var errorResult = JsonSerializer.Deserialize<ErrorResponse>(response.Content);
        throw new RequestFailureException(errorResult.Status ?? response.StatusCode, errorResult.Message ?? errorResult.Errors?.FirstOrDefault() ?? response.ErrorMessage);
    }
}
