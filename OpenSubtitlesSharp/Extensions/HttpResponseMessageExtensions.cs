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

        var contentString = await response.Content.ReadAsStringAsync();
        var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        throw new RequestFailureException(errorResult.Status ?? response.StatusCode, errorResult.Message ?? errorResult.Errors?.FirstOrDefault() ?? "Something went wrong.");
    }
}
