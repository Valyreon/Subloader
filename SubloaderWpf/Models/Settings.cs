using System.Collections.Generic;
using SuppliersLibrary;

namespace SubloaderWpf.Utilities
{
    public class Settings
    {
        public bool AllowMultipleDownloads { get; set; } = false;
        public bool DownloadToSubsFolder { get; set; } = false;
        public bool IsByHashChecked { get; set; } = true;
        public bool IsByNameChecked { get; set; } = false;
        public bool KeepWindowOnTop { get; set; } = true;
        public bool OverwriteSameLanguageSub { get; set; } = false;

        public IReadOnlyList<SubtitleLanguage> WantedLanguages { get; set; } = new List<SubtitleLanguage>();
    }
}
