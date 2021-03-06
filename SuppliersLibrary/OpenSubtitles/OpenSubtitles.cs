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
        private readonly string baseRestUrl = "https://rest.opensubtitles.org/search";
        private readonly string userAgentId = "SubLoad v1";

        public async Task<IList<ISubtitleResultItem>> SearchAsync(string path, object[] parameters = null)
        {
            using var client = new HttpClient { Timeout = new TimeSpan(0, 0, 0, 0, -1) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-User-Agent", userAgentId);

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

            if (!File.Exists(path))
            {
                throw new BadFileException("Input file does not exist.");
            }

            var results = new List<ISubtitleResultItem>();

            if (byHash)
            {
                var responseHash = await client.PostAsync(FormHashSearchUrl(path), null);
                var x = await responseHash.Content.ReadAsStringAsync();
                CheckStatus(responseHash);
                var responseHashBody = await responseHash.Content.ReadAsStringAsync(); // this is json string
                var resultHash = JsonSerializer.Deserialize<IList<OSItem>>(responseHashBody).ToList();

                results.AddRange(ConvertList(resultHash));
            }

            if (byName)
            {
                var responseQuery = await client.PostAsync(FormQuerySearchUrl(path), null);
                CheckStatus(responseQuery);
                var responseQueryBody = await responseQuery.Content.ReadAsStringAsync(); // this is json string
                var resultQuery = JsonSerializer.Deserialize<IList<OSItem>>(responseQueryBody).ToList();

                results.AddRange(ConvertList(resultQuery));
            }

            return results;
        }

        private List<ISubtitleResultItem> ConvertList(IEnumerable<OSItem> list)
        {
            var result = new List<ISubtitleResultItem>();
            foreach (var x in list)
            {
                result.Add(x);
            }

            return result;
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

        private void CheckStatus(HttpResponseMessage msg)
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
