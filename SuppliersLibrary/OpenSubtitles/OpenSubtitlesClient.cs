using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using SuppliersLibrary.Exceptions;

namespace SuppliersLibrary.OpenSubtitles;

public class OpenSubtitlesClient
{
    private const string baseRestUrl = "https://rest.opensubtitles.org/search";
    private const string userAgentId = "SubLoad v1";

    // max 40 requests in 10 seconds per IP
    private static readonly TimeSpan throttlePeriod = new(0, 0, 0, 0, 400);

    private DateTime lastRequestTimestamp = DateTime.MinValue;

    public async Task<bool> Download(OSItem item, string savePath)
    {
        try
        {
            await CheckThrottle();
            using var client = new HttpClient();
            using var compressedStream = await client.GetStreamAsync(item.SubDownloadLink);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var uncompressed = new MemoryStream();

            await zipStream.CopyToAsync(uncompressed);

            File.WriteAllBytes(savePath, uncompressed.ToArray());
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<IReadOnlyList<OSItem>> SearchAsync(string token, string langCode = null)
    {
        try
        {
            using var client = PrepareClient();

            var queryPostUrl = baseRestUrl + $"/query-{HttpUtility.UrlEncode(token)}"
                + (string.IsNullOrWhiteSpace(langCode) ? string.Empty : $"/sublanguageid-{langCode}");

            await CheckThrottle();
            var responseQuery = await client.PostAsync(queryPostUrl, null);
            CheckStatus(responseQuery);
            var responseQueryBody = await responseQuery.Content.ReadAsStringAsync(); // this is json string

            return JsonSerializer.Deserialize<IList<OSItem>>(responseQueryBody)
                                 .DistinctBy(s => s.SubHash)
                                 .ToList();
        }
        catch (Exception)
        {
            throw new ServerFailException("Something is wrong with OS server. Try later.");
        }
    }

    public async Task<IReadOnlyList<OSItem>> SearchForFileAsync(string filePath, bool byHash = true, bool byName = false, string langCode = null, bool hashPriority = false)
    {
        using var client = PrepareClient();

        if (!File.Exists(filePath))
        {
            throw new BadFileException("Input file does not exist.");
        }

        var results = new List<OSItem>();

        if (byHash)
        {
            await CheckThrottle();
            var responseHash = await client.PostAsync(FormHashSearchUrl(filePath, langCode), null);
            var x = await responseHash.Content.ReadAsStringAsync();
            CheckStatus(responseHash);
            var responseHashBody = await responseHash.Content.ReadAsStringAsync(); // this is json string
            var resultHash = JsonSerializer.Deserialize<List<OSItem>>(responseHashBody);
            resultHash.ForEach(r => r.ResultByHash = true);
            results.AddRange(resultHash);

            if (hashPriority && results.Any())
            {
                return results;
            }
        }

        if (byName)
        {
            await CheckThrottle();
            var responseQuery = await client.PostAsync(FormQuerySearchUrl(filePath, langCode), null);
            CheckStatus(responseQuery);
            var responseQueryBody = await responseQuery.Content.ReadAsStringAsync(); // this is json string
            var resultQuery = JsonSerializer.Deserialize<IList<OSItem>>(responseQueryBody);

            results.AddRange(resultQuery);
        }

        return results.DistinctBy(s => s.SubHash).ToList();
    }

    private static void CheckStatus(HttpResponseMessage msg)
    {
        if (!msg.IsSuccessStatusCode)
        {
            throw msg.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadFileException("Something is wrong with the input file."),
                HttpStatusCode.ServiceUnavailable => new ServerFailException("Server is temporarily unavailable."),
                _ => new ServerFailException("Unknown error. Try refreshing.")
            };
        }
    }

    private static string FormHashSearchUrl(string path, string langCode = null)
    {
        var moviehash = Hasher.ComputeMovieHash(path);
        var file = new FileInfo(path);
        var movieByteSize = file.Length;

        if (movieByteSize <= 0)
        {
            throw new BadFileException("Selected file is empty.");
        }

        return baseRestUrl
            + $"/moviebytesize-{movieByteSize}"
            + $"/moviehash-{moviehash}"
            + (string.IsNullOrWhiteSpace(langCode) ? string.Empty : $"/sublanguageid-{langCode}");
    }

    private static string FormQuerySearchUrl(string path, string langCode = null)
    {
        var file = new FileInfo(path);
        var nameString = file.Name.Replace('.', ' ');

        return baseRestUrl
            + $"/query-{HttpUtility.UrlEncode(nameString)}"
            + (string.IsNullOrWhiteSpace(langCode) ? string.Empty : $"/sublanguageid-{langCode}");
    }

    private static HttpClient PrepareClient()
    {
        var client = new HttpClient
        {
            Timeout = new TimeSpan(0, 0, 0, 0, -1)
        };

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Add("X-User-Agent", userAgentId);

        return client;
    }

    private async Task CheckThrottle()
    {
        var currentTimestamp = DateTime.UtcNow;
        if (currentTimestamp - lastRequestTimestamp < throttlePeriod)
        {
            await Task.Delay(throttlePeriod - (currentTimestamp - lastRequestTimestamp));
        }
        lastRequestTimestamp = currentTimestamp;
    }
}
