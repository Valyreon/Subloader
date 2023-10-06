using System;
using System.Windows.Input;

namespace SubloaderWpf.Mvvm;

internal class RelayCommand : ICommand
{
    private readonly Action action;

    public RelayCommand(Action action)
    {
        this.action = action;
    }

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
