using SuppliersLibrary;
using System.ComponentModel;

namespace SubloaderWpf.Models
{
    public class SubtitleEntry: INotifyPropertyChanged
    {
        public SubtitleEntry(ISubtitleResultItem item) => Model = item;

        public string Name => Model.Name;

        public string Language => Model.Language;

        public ISubtitleResultItem Model { get; }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
