using System.Collections.Generic;
using SubloaderWpf.Models;

namespace SubloaderWpf.Interfaces
{
    public interface ISettings
    {
        bool IsByNameChecked { get; set; }
        bool IsByHashChecked { get; set; }
        bool KeepWindowOnTop { get; set; }
        public IEnumerable<SubtitleLanguage> WantedLanguages { get; set; }
        void Save();
    }
}
