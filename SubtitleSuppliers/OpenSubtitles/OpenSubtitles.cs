using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SubtitleSuppliers.OpenSubtitles
{
    public class OpenSubtitles : ISubtitleSupplier
    {
        private readonly string baseRestUrl = "https://rest.opensubtitles.org/search";
        private readonly string userAgentId = "SubLoad v1";

        public async Task<IList<ISubtitleResultItem>> SearchAsync(string path)
        {
            string postUrl = this.FormUrl(path);

            using (HttpClient client = new HttpClient { Timeout = new TimeSpan(0, 0, 0, 0, -1) })
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("X-User-Agent", userAgentId);

                var response = await client.PostAsync(this.FormUrl(path), null);
                string responseBody = await response.Content.ReadAsStringAsync(); // this is json string
            }

            return null;
        }

        private string FormUrl(string path)
        {
            var moviehash = GetHash.Main.ToHexadecimal(GetHash.Main.ComputeHash(path));
            FileInfo file = new System.IO.FileInfo(path);
            var movieByteSize = file.Length;
            // /moviebytesize-750005572/moviehash-319b23c54e9cf314
            // string[] nameParts = file.Name.Split('.');
            return baseRestUrl
                + $"/moviebytesize-{movieByteSize}"
                + $"/moviehash-{moviehash}";
        }
    }
}
