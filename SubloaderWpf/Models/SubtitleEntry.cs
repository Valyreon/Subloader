using System.ComponentModel;
using SuppliersLibrary;

namespace SubloaderWpf.Models
{
    public class SubtitleEntry : INotifyPropertyChanged
    {
        public SubtitleEntry(ISubtitleResultItem item) => Model = item;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name => Model.Name;

        public string Language => Model.Language;

        public ISubtitleResultItem Model { get; }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
