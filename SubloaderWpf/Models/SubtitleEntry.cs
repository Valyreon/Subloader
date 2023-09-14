using System.ComponentModel;
using SuppliersLibrary.OpenSubtitles;

namespace SubloaderWpf.Utilities
{
    public class SubtitleEntry : INotifyPropertyChanged
    {
        public SubtitleEntry(OSItem item)
        {
            Model = item;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Language => Model.Language;
        public OSItem Model { get; }
        public string Name => Model.Name;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
