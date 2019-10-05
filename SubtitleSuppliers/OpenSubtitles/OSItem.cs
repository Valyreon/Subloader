using Newtonsoft.Json;
using System.Net;

namespace SubtitleSuppliers.OpenSubtitles
{
    public class OSItem : ISubtitleResultItem
    {
        [JsonProperty("IDSubMovieFile")]
        public string IDSubMovieFile { get; set; }

        [JsonProperty("MovieHash")]
        public string MovieHash { get; set; }

        [JsonProperty("MovieByteSize")]
        public string MovieByteSize { get; set; }

        [JsonProperty("IDSubtitleFile")]
        public string IDSubtitleFile { get; set; }

        [JsonProperty("SubFileName")]
        public string SubFileName { get; set; }

        [JsonProperty("SubHash")]
        public string SubHash { get; set; }

        [JsonProperty("IDSubtitle")]
        public string IDSubtitle { get; set; }

        [JsonProperty("SubLanguageID")]
        public string SubLanguageID { get; set; }

        [JsonProperty("SubFormat")]
        public string SubFormat { get; set; }

        [JsonProperty("SubRating")]
        public string SubRating { get; set; }

        [JsonProperty("SubDownloadsCnt")]
        public string SubDownloadsCnt { get; set; }

        [JsonProperty("IDMovie")]
        public string IDMovie { get; set; }

        [JsonProperty("IDMovieImdb")]
        public string IDMovieImdb { get; set; }

        [JsonProperty("LanguageName")]
        public string LanguageName { get; set; }

        [JsonProperty("SubDownloadLink")]
        public string SubDownloadLink { get; set; }

        [JsonProperty("ZipDownloadLink")]
        public string ZipDownloadLink { get; set; }

        [JsonProperty("SubtitlesLink")]
        public string SubtitlesLink { get; set; }

        public string Language => this.LanguageName;

        public string Name => this.SubFileName;

        public string Format => this.SubFormat;

        public void Download(string savePath)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(this.SubDownloadLink, savePath);
            }
        }
    }
}
