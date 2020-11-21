using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.Json.Serialization;

namespace SuppliersLibrary.OpenSubtitles
{
    public class OSItem : ISubtitleResultItem, IEquatable<OSItem>
    {
        [JsonPropertyName("IDSubMovieFile")]
        public string IDSubMovieFile { get; set; }

        [JsonPropertyName("MovieHash")]
        public string MovieHash { get; set; }

        public string MovieByteSize { get; set; }

        public string IDSubtitleFile { get; set; }

        public string SubFileName { get; set; }

        public string SubHash { get; set; }

        public string IDSubtitle { get; set; }

        public string SubLanguageID { get; set; }

        public string SubFormat { get; set; }

        public string SubRating { get; set; }

        public string SubDownloadsCnt { get; set; }

        public string IDMovie { get; set; }

        public string IDMovieImdb { get; set; }

        public string LanguageName { get; set; }

        public string SubDownloadLink { get; set; }

        public string ZipDownloadLink { get; set; }

        public string SubtitlesLink { get; set; }

        public string Language => LanguageName;

        public string Name => SubFileName;

        public string Format => SubFormat;

        public void Download(string savePath)
        {
            using var client = new WebClient();
            using var compressedStream = new MemoryStream(client.DownloadData(SubDownloadLink));
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var uncompressed = new MemoryStream();

            _ = zipStream.CopyToAsync(uncompressed);

            File.WriteAllBytes(savePath, uncompressed.ToArray());
        }

        public bool Equals(OSItem other)
        {
            if (SubHash == other.SubHash)
            {
                return true;
            }

            return false;
        }
    }
}
