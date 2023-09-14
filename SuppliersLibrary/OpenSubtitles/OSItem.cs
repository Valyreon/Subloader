using System;

namespace SuppliersLibrary.OpenSubtitles
{
    public class OSItem : IEquatable<OSItem>
    {
        public string Format => SubFormat;
        public string IDMovie { get; set; }
        public string IDMovieImdb { get; set; }
        public string IDSubMovieFile { get; set; }

        public string IDSubtitle { get; set; }
        public string IDSubtitleFile { get; set; }
        public string Language => LanguageName;
        public string LanguageID => SubLanguageID;
        public string LanguageName { get; set; }
        public string MovieByteSize { get; set; }
        public string MovieHash { get; set; }
        public string Name => SubFileName;
        public bool ResultByHash { get; set; }
        public string SubDownloadLink { get; set; }
        public string SubDownloadsCnt { get; set; }
        public string SubFileName { get; set; }

        public string SubFormat { get; set; }
        public string SubHash { get; set; }
        public string SubLanguageID { get; set; }
        public string SubRating { get; set; }
        public string SubtitlesLink { get; set; }
        public string ZipDownloadLink { get; set; }

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
