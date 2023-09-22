using System.ComponentModel;
using System.Linq;
using OpenSubtitlesSharp;

namespace SubloaderWpf.Utilities;

public class SubtitleEntry : INotifyPropertyChanged
{
    public SubtitleEntry(Subtitle item)
    {
        Model = item;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string Language => App.Settings.AllLanguages.SingleOrDefault(l => l.Code == Model.Information.Language)?.Name;
    public Subtitle Model { get; }
    public string Name => Model.Information.Release;

    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
