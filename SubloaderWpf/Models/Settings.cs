using System.Collections.Generic;

namespace SubloaderWpf.Utilities
{
    public class Settings
    {
        public bool IsByNameChecked { get; set; } = false;

        public bool IsByHashChecked { get; set; } = true;

        public bool KeepWindowOnTop { get; set; } = true;

        public bool DownloadToSubsFolder { get; set; } = false;

        public bool AllowMultipleDownloads { get; set; } = false;

        public IEnumerable<SubtitleLanguage> WantedLanguages { get; set; } = new List<SubtitleLanguage>();
    }
}
