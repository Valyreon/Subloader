using SuppliersLibrary;
using System.ComponentModel;

namespace SubloaderWpf.Models
{
    public class SubtitleEntry: INotifyPropertyChanged
    {
        private readonly ISubtitleResultItem model;

        public SubtitleEntry(ISubtitleResultItem item)
        {
            model = item;
        }

        public string Name
        {
            get
            {
                return model.Name;
            }
        }

        public string Language
        {
            get
            {
                return model.Language;
            }
        }

        public ISubtitleResultItem Model { get => model; }

        protected void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
