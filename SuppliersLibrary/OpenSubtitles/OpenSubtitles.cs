using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using SuppliersLibrary.Exceptions;

namespace SuppliersLibrary.OpenSubtitles
{
    public class OpenSubtitles : ISubtitleSupplier
    {
        private static readonly string baseRestUrl = "https://rest.opensubtitles.org/search";
        private static readonly string userAgentId = "SubLoad v1";

        public async Task<IReadOnlyList<ISubtitleResultItem>> SearchForFileAsync(string filePath, object[] parameters = null)
        {
            using var client = PrepareClient();

            bool byHash = true, byName = false;

            if (parameters != null && parameters.Length == 2)
            {
                byHash = (bool)parameters[0];
                byName = (bool)parameters[1];
            }
            else
            {
                throw new ArgumentException("Parameters should either be null or 2 provided.");
            }

            if (!File.Exists(filePath))
            {
                throw new BadFileException("Input file does not exist.");
            }

            var results = new List<OSItem>();

            try
            {
                if (byHash)
                {
                    var responseHash = await client.PostAsync(FormHashSearchUrl(filePath), null);
                    var x = await responseHash.Content.ReadAsStringAsync();
                    CheckStatus(responseHash);
                    var responseHashBody = await responseHash.Content.ReadAsStringAsync(); // this is json string
                    var resultHash = JsonSerializer.Deserialize<IList<OSItem>>(responseHashBody);

                    results.AddRange(resultHash);
                }

                if (byName)
                {
                    var responseQuery = await client.PostAsync(FormQuerySearchUrl(filePath), null);
                    CheckStatus(responseQuery);
                    var responseQueryBody = await responseQuery.Content.ReadAsStringAsync(); // this is json string
                    var resultQuery = JsonSerializer.Deserialize<IList<OSItem>>(responseQueryBody);

                    results.AddRange(resultQuery);
                }
            }
            catch (Exception)
            {
                throw new ServerFailException("Something wrong with server. Try later.");
            }

            return results.DistinctBy(s => s.SubHash)
                          .Cast<ISubtitleResultItem>()
                          .ToList();
        }

        public async Task<IReadOnlyList<ISubtitleResultItem>> SearchAsync(string token)
        {
            try
            {
                using var client = PrepareClient();

                var queryPostUrl = baseRestUrl + $"/query-{HttpUtility.UrlEncode(token)}";

                var responseQuery = await client.PostAsync(queryPostUrl, null);
                CheckStatus(responseQuery);
                var responseQueryBody = await responseQuery.Content.ReadAsStringAsync(); // this is json string

                return JsonSerializer.Deserialize<IList<OSItem>>(responseQueryBody)
                                     .DistinctBy(s => s.SubHash)
                                     .ToList();
            }
            catch (Exception)
            {
                throw new ServerFailException("Something wrong with server. Try later.");
            }
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

        private string FormHashSearchUrl(string path)
        {
            var moviehash = Hasher.ToHexadecimal(Hasher.ComputeMovieHash(path));
            var file = new FileInfo(path);
            var movieByteSize = file.Length;

            if (movieByteSize <= 0)
            {
                throw new BadFileException("Selected file is empty.");
            }

            return baseRestUrl
                + $"/moviebytesize-{movieByteSize}"
                + $"/moviehash-{moviehash}";
        }

        private string FormQuerySearchUrl(string path)
        {
            var file = new FileInfo(path);
            var nameString = file.Name.Replace('.', ' ');

            return baseRestUrl
                + $"/query-{HttpUtility.UrlEncode(nameString)}";
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


    }
}
