using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace SuppliersLibrary.OpenSubtitles
{
    public class OSItem : ISubtitleResultItem, IEquatable<OSItem>
    {
        public string IDSubMovieFile { get; set; }

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

        public string LanguageID => SubLanguageID;

        public async void Download(string savePath)
        {
            using var client = new HttpClient();
            using var compressedStream = await client.GetStreamAsync(SubDownloadLink);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var uncompressed = new MemoryStream();

            await zipStream.CopyToAsync(uncompressed);

            File.WriteAllBytes(savePath, uncompressed.ToArray());
        }

        public bool Equals(OSItem other)
        {
            return SubHash == other.SubHash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as OSItem);
        }

        public override int GetHashCode()
        {
            return SubHash.GetHashCode();
        }
    }
}
