using System;
using System.Windows.Input;

namespace SubLoad.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Action action;

        public DelegateCommand(Action action)
        {
            this.action = action;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public void Execute(object parameter)
        {
            this.action();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
