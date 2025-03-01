using System;
using System.Windows.Input;

namespace SubloaderWpf.Mvvm;

internal class RelayCommand(Action action) : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        action();
    }
}
