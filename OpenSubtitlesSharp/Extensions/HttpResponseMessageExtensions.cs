using System.Net.Http.Json;
using OpenSubtitlesSharp.Dtos;

namespace OpenSubtitlesSharp.Extensions;

internal static class HttpResponseMessageExtensions
{
    public static async Task<T> ParseResultAsync<T>(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        string message;
        if (!string.IsNullOrWhiteSpace(errorResult.Message))
        {
            message = errorResult.Message.Trim();
        }
        else if (errorResult.Errors != null && errorResult.Errors.Count > 0 && !string.IsNullOrWhiteSpace(errorResult.Errors[0]))
        {
            message = errorResult.Errors[0].Trim();
        }
        else
        {
            message = "Something went wrong.";
        }

        throw new RequestFailedException(errorResult.Status ?? response.StatusCode, message);
    }
}
