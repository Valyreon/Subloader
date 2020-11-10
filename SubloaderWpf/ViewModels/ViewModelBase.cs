using System.ComponentModel;

namespace SubloaderWpf.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public void Set<T>(string propertyName, ref T field, T value)
        {
            field = value;
            RaisePropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
