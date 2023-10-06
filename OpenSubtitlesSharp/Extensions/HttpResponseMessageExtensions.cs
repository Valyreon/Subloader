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
        throw new RequestFailedException(errorResult.Status ?? response.StatusCode, errorResult.Message?.Trim() ?? errorResult.Errors?.FirstOrDefault()?.Trim() ?? "Something went wrong.");
    }
}
